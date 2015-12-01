using UnityEngine;
using System;
using System.Collections;
using System.Collections.Generic;
using URandom = UnityEngine.Random;

public class PlatformGroup : MonoBehaviour
{
    private HashSet<Platform> container; //there is at least one element

    public delegate void OnGroupMovedHandler();
    public static event OnGroupMovedHandler OnGroupMoved;

    public static GridSystem gridSystem;
    //private static RobotController robot { get { return LevelController.Instance.robotController; } }

    public static PlatformGroup CreatePfGroup(Platform pf)
    {
        GameObject pfGroupObj = new GameObject("PlatformGroup");
        PlatformGroup pfGroup = pfGroupObj.AddComponent<PlatformGroup>();
        pfGroup.container = new HashSet<Platform>();
        pfGroup.AddPlatform(pf);
        //pfGroup.OnGroupMoved += robot.replanPath;
        pf.transform.parent = pfGroupObj.transform;
        pf.group = pfGroup;

        return pfGroup;
    }

    
    public static HashSet<PlatformGroup> ProcuduralInit(Int2 start, Int2 goal, float randomness, int blockFactor)
    {
        //generate one random path (may has circuit)
        randomness = Mathf.Clamp01(randomness);
        Int2 curr = start;
        List<Int2> path = new List<Int2>();
        path.Add(curr);
        Direction currDir = Direction.Forward;
        Direction nextDir = currDir;
        while (!curr.Equals(goal))
        {
            if (curr._z == goal._z)
            {
                if (curr._x > goal._x)
                    nextDir = Direction.Left;
                else nextDir = Direction.Right;
            }
            else
            {
                int allowed = 0;
                if (curr._x > 0 && currDir != Direction.Right) allowed = allowed | (int)Direction.Left;
                if (curr._x < gridSystem.gridSizeX - 1 && currDir != Direction.Left) allowed = allowed | (int)Direction.Right;
                //if (curr._z > 0 && currDir != Direction.Forward) allowed |= (int)Direction.Backward;
                if (curr._z < goal._z) allowed |= (int)Direction.Forward;

                //make a turn
                if (URandom.value <= randomness || (allowed & (int)currDir) == 0)
                {
                    int best = 0;
                    if (curr._x < goal._x) best |= (int)Direction.Right;
                    else if (curr._x > goal._x) best |= (int)Direction.Left;

                    if (curr._z < goal._z) best |= (int)Direction.Forward;
                    //else if (curr._z > goal._z) best |= (int)Direction.Backward;

                    best &= allowed;

                    do
                        nextDir = (Direction)(1 << URandom.Range(0, 3));
                    while (currDir == nextDir || ((int)nextDir & allowed) == 0);
                }
            }
            switch (nextDir)
            {
                case Direction.Left: curr._x--; break;
                case Direction.Right: curr._x++; break;
                //case Direction.Backward: curr._z--; break;
                case Direction.Forward: curr._z++; break;
            }
            path.Add(curr);
            currDir = nextDir;

        }

        //reverse path
        path.Reverse();

        //offset segments

        throw new NotImplementedException();
        
    }



    public static HashSet<PlatformGroup> Restructure(HashSet<PlatformGroup> groups, Int2 playerIdx)
    {
        //NO EMPTY GROUP ALLOWED

        bool[,] seen = new bool[gridSystem.gridSizeX, gridSystem.gridSizeZ];

        HashSet<HashSet<PlatformGroup>> components = new HashSet<HashSet<PlatformGroup>>();
        foreach (PlatformGroup group in groups)
        {
            var iter = group.container.GetEnumerator();
            iter.MoveNext();
            var pf = iter.Current;
            if (seen[pf.index._x, pf.index._z])
                continue;
            HashSet<PlatformGroup> component = new HashSet<PlatformGroup>();

            //flood fill
            Queue<Int2> indices = new Queue<Int2>();
            indices.Enqueue(pf.index);
            while (indices.Count > 0)
            {
                Int2 curr = indices.Dequeue();
                seen[curr._x, curr._z] = true;
                if (gridSystem[curr] != null)
                {
                    component.Add(gridSystem[curr].group);
                    if (curr._x > 0 && !seen[curr._x - 1, curr._z])
                        indices.Enqueue(new Int2(curr._x - 1, curr._z));
                    if (curr._x < gridSystem.gridSizeX - 1 && !seen[curr._x + 1, curr._z])
                        indices.Enqueue(new Int2(curr._x + 1, curr._z));
                    if (curr._z > 0 && !seen[curr._x, curr._z - 1])
                        indices.Enqueue(new Int2(curr._x, curr._z - 1));
                    if (curr._z < gridSystem.gridSizeZ - 1 && !seen[curr._x, curr._z + 1])
                        indices.Enqueue(new Int2(curr._x, curr._z + 1));
                }
            }

            components.Add(component);

        }

        //components should have no overlapping
        HashSet<PlatformGroup> output = new HashSet<PlatformGroup>();
        foreach (var component in components)
            output.Add(PlatformGroup.Combine(component, playerIdx));

        return output;
    }

    public static PlatformGroup Combine(PlatformGroup lhs, PlatformGroup rhs, Int2 playerIdx)
    {
        GameObject output = new GameObject("PlatformGroup");
        PlatformGroup outputGroup = output.AddComponent<PlatformGroup>();
        outputGroup.container = new HashSet<Platform>();
        //outputGroup.OnGroupMoved += robot.replanPath;
        
        //transfer ownership
        outputGroup.container.UnionWith(lhs.container);
        outputGroup.container.UnionWith(rhs.container);
      
        //transfer childObjs
        List<Transform> children = new List<Transform>();
        for (int i = 0; i < lhs.gameObject.transform.childCount; i++)
            children.Add(lhs.gameObject.transform.GetChild(i));
        foreach (Transform child in children)
            child.transform.parent = output.transform;

        children.Clear();
        for (int i = 0; i < rhs.gameObject.transform.childCount; i++)
            children.Add(rhs.gameObject.transform.GetChild(i));
        foreach (Transform child in children)
            child.transform.parent = output.transform;

        //Vector2 robotPos2d = new Vector2(robot.transform.position.x, robot.transform.position.z);
        if (ReferenceEquals(lhs, gridSystem.ComputeGroup(playerIdx)) ||
            ReferenceEquals(rhs, gridSystem.ComputeGroup(playerIdx)) ||
            ReferenceEquals(lhs, gridSystem.ComputeGroup(gridSystem.goal)) ||
            ReferenceEquals(rhs, gridSystem.ComputeGroup(gridSystem.goal)))
        {
            foreach (Transform child in output.transform)
                child.gameObject.layer = Utility.ToLayerNumber(gridSystem.lockedPfLayer);//?

        }

        foreach (Platform pf in outputGroup.container)
            pf.group = outputGroup;

        lhs.container.Clear();
        rhs.container.Clear();
        Destroy(lhs.gameObject);
        Destroy(rhs.gameObject);


        return outputGroup;
    }

    public static PlatformGroup Combine(HashSet<PlatformGroup> groups, Int2 playerIdx)
    {
        GameObject output = new GameObject("PlatformGroup");
        PlatformGroup outputGroup = output.AddComponent<PlatformGroup>();
        outputGroup.container = new HashSet<Platform>();
        //outputGroup.OnGroupMoved += robot.replanPath;
        //transfer ownership
        foreach (PlatformGroup group in groups)
            outputGroup.container.UnionWith(group.container);

        bool lockedGroup = false;
        //Vector2 robotPos2d = new Vector2(robot.transform.position.x, robot.transform.position.z);
        foreach (PlatformGroup group in groups)
        {
            if (ReferenceEquals(group, gridSystem.ComputeGroup(playerIdx)) ||
                ReferenceEquals(group, gridSystem.ComputeGroup(gridSystem.goal)))
            {
                lockedGroup = true;
                break;
            }
        }

        foreach (Platform pf in outputGroup.container)
            pf.group = outputGroup;

        //transfer childObjs
        List<Transform> children = new List<Transform>();
        foreach (PlatformGroup group in groups)
        {
            for (int i = 0; i < group.gameObject.transform.childCount; i++)
                children.Add(group.gameObject.transform.GetChild(i));
            foreach (Transform child in children)
                child.transform.parent = output.transform;

            children.Clear();
            group.container.Clear();
            Destroy(group.gameObject);
        }

        if (lockedGroup)
        {
            foreach (Transform child in output.transform)
                child.gameObject.layer = Utility.ToLayerNumber(gridSystem.lockedPfLayer);//?
        }

        return outputGroup;
    }

    public bool AddPlatform(Platform pf) //should only be called by GridSystem (?)
    {
        if (!container.Contains(pf))
        {
            container.Add(pf);
            pf.group = this;
            return true;
        }
        else return false;
    }

    //private
    private void ComputeAABB(out Int2 lower, out Int2 upper)
    {
        lower = new Int2(int.MaxValue, int.MaxValue);
        upper = new Int2(-1, -1);
        foreach (Platform pf in container)
        {
            if (pf.index._x > upper._x)
                upper._x = pf.index._x;
            if (pf.index._x < lower._x)
                lower._x = pf.index._x;

            if (pf.index._z > upper._z)
                upper._z = pf.index._z;
            if (pf.index._z < lower._z)
                lower._z = pf.index._z;
        }
    }

    //private
    private bool CheckBoundary(Int2 lower, Int2 upper)
    {
        //use AABB
        if (upper._x >= gridSystem.gridSizeX || upper._x < 0 ||
            upper._z >= gridSystem.gridSizeZ || upper._z < 0)
            return false;
        else return true;
    }

    //private
    private int ComputeMoveUnits(PlatformMoveType type, Int2 playerIdx)
    {
        //theoretically shouldn't generate out of bound exception
        Int2 lower,upper;
        ComputeAABB(out lower, out upper);
        switch (type)
        {
            case PlatformMoveType.AxisX:
                {
                    if (playerIdx._x >= lower._x && playerIdx._x <= upper._x)
                        return 0;
                    else if (playerIdx._x < lower._x)
                    {
                        //player is at left, move left then

                        //find the leftmost platforms from THIS group on each z
                        int[] thisLeftmostIdx = new int[upper._z - lower._z + 1];
                        for (int i = 0; i < thisLeftmostIdx.Length; i++)
                            thisLeftmostIdx[i] = upper._x;
                        foreach (Platform pf in container)
                        {
                            if (thisLeftmostIdx[pf.index._z - lower._z] > pf.index._x)
                                thisLeftmostIdx[pf.index._z - lower._z] = pf.index._x;
                        }
                        //find the rightmost platform from OTHER group on each z (if any)
                        int[] otherRightmostIdx = new int[upper._z - lower._z + 1];
                        for (int i = 0; i < otherRightmostIdx.Length; i++)
                        {
                            otherRightmostIdx[i] = playerIdx._x - 1;
                        }
                        for (int z = 0; z < upper._z - lower._z + 1; z++)
                        {
                            for (int x = thisLeftmostIdx[z] - 1; x >= playerIdx._x; x--)
                            {
                                if (gridSystem[x, z + lower._z] != null)
                                {
                                    otherRightmostIdx[z] = x;
                                    break;
                                }
                            }
                        }

                        //now determine the moving units of the longest possible move
                        //record groups that this will collide with (find all min values)                        
                        int maxUnit = lower._x - playerIdx._x;
                        int[] candidates = new int[upper._z - lower._z + 1];
                        for (int i = 0; i < upper._z - lower._z + 1; i++)
                        {
                            candidates[i] = thisLeftmostIdx[i] - otherRightmostIdx[i] - 1;
                            if ( candidates[i] < maxUnit)
                                maxUnit = candidates[i];
                        }
                        
                        return - maxUnit; //negative since we are moving left
                    }
                    else
                    {
                        //player is at left, move right then

                        //find the rightmost platforms from THIS group on each z
                        int[] thisRightmostIdx = new int[upper._z - lower._z + 1];
                        for (int i = 0; i < thisRightmostIdx.Length; i++)
                            thisRightmostIdx[i] = lower._x;
                        foreach (Platform pf in container)
                        {
                            if (thisRightmostIdx[pf.index._z - lower._z] < pf.index._x)
                                thisRightmostIdx[pf.index._z - lower._z] = pf.index._x;
                        }

                        //find the leftmost platforms from OTHER groups on each z (if any)
                        int[] otherLeftmostIdx = new int[upper._z - lower._z + 1];
                        for (int i = 0; i < otherLeftmostIdx.Length; i++)
                        {
                            otherLeftmostIdx[i] = playerIdx._x + 1;
                        }
                        for (int z = 0; z < upper._z - lower._z + 1; z++)                           
                        {
                            for (int x = thisRightmostIdx[z] + 1; x <= playerIdx._x; x++)
                            {
                                if (gridSystem[x, z + lower._z] != null)
                                {
                                    otherLeftmostIdx[z] = x;
                                    break;
                                }
                            }
                        }
                        //now determine the moving units of the longest possible move
                        //record groups that this will collide with (find all min values)      
                        int maxUnit = playerIdx._x - upper._x;
                        int[] candidates = new int[upper._z - lower._z + 1];
                        for (int i = 0; i < upper._z - lower._z + 1; i++)
                        {
                            candidates[i] = otherLeftmostIdx[i] - thisRightmostIdx[i] - 1;
                            if (candidates[i] < maxUnit)
                                maxUnit = candidates[i];
                        }

                            return maxUnit;
                    }
                }
            case PlatformMoveType.AxisZ:
                {
                    if (playerIdx._z >= lower._z && playerIdx._z <= upper._z)
                        return 0;
                    else if (playerIdx._z < lower._z)
                    {
                        //player is at back, move back then

                        //find the backmost(?) platforms of THIS group on each x
                        int[] thisBackmostIdx = new int[upper._x - lower._x + 1];
                        for (int i = 0; i < thisBackmostIdx.Length; i++)
                            thisBackmostIdx[i] = upper._z;
                        foreach (Platform pf in container)
                        {
                            if (pf.index._z < thisBackmostIdx[pf.index._x - lower._x])
                                thisBackmostIdx[pf.index._x - lower._x] = pf.index._z;
                        }
                        
                        //find the foremost platforms of OTHER groups on each x if any
                        int[] otherForemostIdx = new int[upper._x - lower._x + 1];
                        for (int i = 0; i < otherForemostIdx.Length; i++)
                        {
                            otherForemostIdx[i] = playerIdx._z - 1;
                        }
                        for (int x = 0; x < thisBackmostIdx.Length; x++)
                        {
                            for (int z = thisBackmostIdx[x] - 1; z >= playerIdx._z; z--)
                            {
                                if (gridSystem[x + lower._x, z] != null)
                                {
                                    otherForemostIdx[x] = z;
                                    break;
                                }
                            }
                        }

                        //now determine the moving units of the longest possible move
                        //record groups that this will collide with (find all min values)  
                        int maxUnit = lower._z - playerIdx._z;
                        int[] candidates = new int[upper._x - lower._x + 1];
                        for (int i = 0; i < upper._x - lower._x + 1; i++)
                        {
                            candidates[i] = thisBackmostIdx[i] - otherForemostIdx[i] - 1;
                            if (candidates[i] < maxUnit)
                                maxUnit = candidates[i];
                        }

                        return - maxUnit; //negative since we are moving back
                    }
                    else
                    {
                        //player is in front, move front then
                        //find the foremost platform of THIS group on each x
                        int[] thisForemostIdx = new int[upper._x - lower._x + 1];
                        for (int i = 0; i < thisForemostIdx.Length; i++)
                            thisForemostIdx[i] = lower._z;
                        foreach (Platform pf in container)
                        {
                            if (thisForemostIdx[pf.index._x - lower._x] < pf.index._z)
                                thisForemostIdx[pf.index._x - lower._x] = pf.index._z;
                        }

                        //find the backmost platform of OTHER group on each x if any
                        int[] otherBackmostIdx = new int[upper._x - lower._x + 1];
                        for (int i = 0; i < otherBackmostIdx.Length; i++)
                        {
                            otherBackmostIdx[i] = playerIdx._z + 1;
                        }
                        for (int x = 0; x < thisForemostIdx.Length; x++)
                        {
                            for (int z = thisForemostIdx[x] + 1; z <= playerIdx._z; z++)
                            {
                                if (gridSystem[x + lower._x, z] != null)
                                {
                                    otherBackmostIdx[x] = z;
                                    break;
                                }
                            }
                        }

                        //now determine the moving units of the longest possible move
                        int maxUnit = playerIdx._z - upper._z;
                        int[] candidates = new int[upper._x - lower._x + 1];
                        for (int i = 0; i < upper._x - lower._x + 1; i++)
                        {
                            candidates[i] = otherBackmostIdx[i] - thisForemostIdx[i] - 1;
                            if (candidates[i] < maxUnit)
                                maxUnit = candidates[i];
                        }

                        return maxUnit;
                    }
                }
            default:throw new NotImplementedException();
        }       
    }

    //private
    private HashSet<PlatformGroup> ComputeNeighborGroups()
    {
        HashSet<PlatformGroup> output = new HashSet<PlatformGroup>();
        foreach (Platform pf in container)
        {
            if (pf.index._x > 0)
            {
                Int2 left = new Int2(pf.index._x - 1, pf.index._z);
                if (gridSystem[left] != null && !ReferenceEquals(gridSystem[left].group, this))
                    output.Add(gridSystem[left].group);
            }
            if (pf.index._x < gridSystem.gridSizeX - 1)
            {
                Int2 right = new Int2(pf.index._x + 1, pf.index._z);
                if (gridSystem[right] != null && !ReferenceEquals(gridSystem[right].group, this))
                    output.Add(gridSystem[right].group);
            }
            if (pf.index._z > 0)
            {
                Int2 down = new Int2(pf.index._x, pf.index._z - 1);
                if (gridSystem[down] != null && !ReferenceEquals(gridSystem[down].group, this))
                    output.Add(gridSystem[down].group);
            }
            if (pf.index._z < gridSystem.gridSizeZ - 1)
            {
                Int2 up = new Int2(pf.index._x, pf.index._z + 1);
                if (gridSystem[up] != null && !ReferenceEquals(gridSystem[up].group, this))
                    output.Add(gridSystem[up].group);
            }
        }
        return output;
    }

    public void StartMoveGroup(PlatformMoveType type, Int2 playerIdx)
    {
        if (gridSystem.ExistMovingGroup)
            return;

        int units = ComputeMoveUnits(type, playerIdx);
        //HashSet<PlatformGroup> colGroups = ComputeNeighborGroups();
        if (units == 0)
        {
            HashSet<PlatformGroup> colGroups = ComputeNeighborGroups();
            colGroups.Add(this);
            PlatformGroup.Combine(colGroups, playerIdx);
            return;
        }

        gridSystem.ExistMovingGroup = true;
        StartCoroutine(MoveGroupCoroutine(type, units, playerIdx));
    }

    //private
    private IEnumerator MoveGroupCoroutine(PlatformMoveType type, int units, Int2 playerIdx)
    {        
        foreach (Platform pf in container)
            pf.StartMove(type, units);

        bool waitAll = true;
        while (waitAll)
        {
            waitAll = false;
            foreach (Platform pf in container)
            {
                if (pf.IsMoving)
                    waitAll = true;
            }
            yield return null;
        }
        
        //maintain gridSystem
        foreach (Platform pf in container)
            gridSystem[pf.index] = null;
        switch (type)
        {
            case PlatformMoveType.AxisX:
                {
                    foreach (Platform pf in container)
                    {
                        pf.index._x += units;
                        gridSystem[pf.index] = pf;
                    }
                    break;
                }
            case PlatformMoveType.AxisZ:
                {
                    foreach (Platform pf in container)
                    {
                        pf.index._z += units;
                        gridSystem[pf.index] = pf;
                    }
                    break;
                }
            default: throw new NotImplementedException();
        }

        //trigger combination
        HashSet<PlatformGroup> colGroups = ComputeNeighborGroups();
        colGroups.Add(this);

        //should update player's platform group here
        PlatformGroup.Combine(colGroups, playerIdx);

        //consider using event instead
        //LevelController.platformMoved = true;

        if (OnGroupMoved != null)
            OnGroupMoved();

        gridSystem.ExistMovingGroup = false;

        
	}
	
	public void Activate()
	{
		foreach (Platform pf in container) {
			pf.Activate();
		}
	}
	
	public void Deactivate()
	{
		foreach (Platform pf in container) {
			pf.Deactivate();
		}
	}

    private enum Direction
    {
        Left = 1,
        Right = 2,
        Forward = 4,
        //Backward = 8,
    }

}
