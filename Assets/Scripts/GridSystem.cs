using UnityEngine;
using System;
using System.Collections.Generic;

/// <summary>
/// Grid System for the whole map (should be only one instance).
/// All platforms are aligned to the grid.
/// The grid is currently 2D (no y axis), though it could be 3D
/// </summary>
public class GridSystem : MonoBehaviour
{
    public float cellSize { get; private set; }
    public int gridSizeX { get; private set; }
    public int gridSizeZ { get; private set; }
    public Int2 goal { get; private set; }

	public Platform pfPrefab;
    public LayerMask movablePfLayer;
    public LayerMask lockedPfLayer;

    private List<List<Platform>> pfContainer;

    public Platform this[int x, int z]
    {
        get { return pfContainer[x][z]; }
        set { pfContainer[x][z] = value; }
    }

    public Platform this[Int2 index]
    {
        get { return pfContainer[index._x][index._z]; }
        set { pfContainer[index._x][index._z] = value; }
    }

    public bool ExistMovingGroup { get; set; }

    public void Init(Int2 size, float cell, Int2 goal)
    {
        cellSize = cell;
        gridSizeX = size._x;
        gridSizeZ = size._z;
        Debug.Assert(goal._x >= 0 && goal._x < size._x &&
            goal._z >= 0 && goal._z < size._z);
        this.goal = goal;
        pfContainer = new List<List<Platform>>();
        for (int x = 0; x < gridSizeX; x++)
        {
            pfContainer.Add(new List<Platform>());
            for (int z = 0; z < gridSizeZ; z++)
                pfContainer[x].Add(null);
        }
    }

    // Use this function to place new platform
    public PlatformGroup PlacePlatform(int x, int z)
    {
        if (pfContainer[x][z] != null)
        {
            //throw new UnityException("platform already exist");
            return null;
        }

        pfContainer[x][z] = Instantiate(pfPrefab,
            (transform.position + new Vector3(x, 0f, z)) * cellSize, Quaternion.identity) as Platform;
        pfContainer[x][z].gridSystem = this;
        pfContainer[x][z].index = new Int2(x, z);

        return PlatformGroup.CreatePfGroup(pfContainer[x][z]);
    }

    public Int2 ComputeIdx(Vector2 pos2d)
    {
        Vector2 origin = new Vector2(transform.position.x, transform.position.z);
        Vector2 floatCoord = (pos2d - origin) / cellSize;
        return new Int2(Mathf.RoundToInt(floatCoord.x), Mathf.RoundToInt(floatCoord.y));
    }


    public PlatformGroup ComputeGroup(Vector2 pos2d)
    {
        Int2 idx = ComputeIdx(pos2d);
        if (this[idx] != null)
            return this[idx].group;
        else return null;
    }

    public PlatformGroup ComputeGroup(Int2 idx)
    {
        if (this[idx] != null)
            return this[idx].group;
        else return null;
    }
}

/// <summary>
/// A simple wrapper for 2D integer vector
/// </summary>
public struct Int2 : IEquatable<Int2>
{
    public int _x, _z;
    public Int2(int x, int z)
    { _x = x; _z = z; }
	
	public float magnitude {
		get {
			return Mathf.Sqrt (_x * _x + _z * _z);
		}
	}

    public bool Equals(Int2 other)
    { return _x == other._x && _z == other._z; }

    public override bool Equals(object obj)
    {
        if (obj == null)
            return false;
        if (!(obj is Int2))
            return false;
        Int2 other = (Int2)obj;
        return _x == other._x && _z == other._z;
    }

    public override int GetHashCode()
    {
        unchecked
        {
            int hash = 17;
            hash = hash * 23 + _x.GetHashCode();
            hash = hash * 23 + _z.GetHashCode();
            return hash;
        }
    }
	
	public static Int2 operator +(Int2 c1, Int2 c2) {
		return new Int2(c1._x + c2._x, c1._z + c2._z);
	}
	
	public static Int2 operator -(Int2 c1, Int2 c2) {
		return new Int2(c1._x - c2._x, c1._z - c2._z);
	}

    public override string ToString()
    {
        return "(" + _x.ToString() + "," + _z.ToString() + ")";
    }
}