using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class RobotController : MonoBehaviour
{
    private static GridSystem gridSystem { get { return LevelController.Instance.gridSystem; } }

    public Animator robotAnim;

    public Material faceMat;
    private Material faceMatInst;
    public RobotSound sound;

	public GameObject body;
	private float delay = .1f;
	private float speedFactor = 0.75f;
	private float moveThreshold = 0.1f;
	private float smoothing = 7f;

	private bool isMoving;
	private Vector2[] path;
	private int currentIdx;
	private Int2 currentPos;
	private Vector2 nextPos;
	private Int2 finalPos;
	private float initialTime;

    private PlatformGroup prevGroup;

    private void Start()
    {
		isMoving = false;
		initialTime = Time.time;
        Debug.Assert(robotAnim != null);
        Debug.Assert(sound != null);
        updatePlayerLayer();
    }

	public void setFinalPosition (Int2 final) {
		finalPos = final;
	}

	// Update is called once per frame
	void Update () {
        if (isMoving && (initialTime + delay) < Time.time)
        {
            move();
        }
    }

    //path finding here
	public void replanPath() {
		currentPos = new Int2 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.z));
		Int2[] spots = PathFinder.getPath (currentPos, finalPos, gridSystem);
		path = getSmoothedPath (spots, 4);
		for (int i = 0; i < path.Length; i++) {
			Debug.Log (path[i]);
		}
		initialTime = Time.time;
		currentIdx = path.Length - 1;
		nextPos = path [0];
		isMoving = true;
        //add sound here, a bit dirty
        sound.PlayMajorSound(RobotSound.MajorSoundType.Walk);
        sound.PlayOtherSound(RobotSound.OtherSoundType.None);

        LevelController.Instance.player.autoFollow = true;
    }

	private Vector2[] getSmoothedPath (Int2[] iSpots, int level) {
		Vector2[] path = new Vector2[iSpots.Length];
		for (int i = 0; i < iSpots.Length; i++) {
			path[i] = new Vector2(iSpots[i]._x, iSpots[i]._z);
		}
		return smoothPath (path, level);
	}

	private Vector2[] smoothPath(Vector2[] path, int level) {
		if (path.Length < 3) {
			return path;
		}
		if (level <= 0) {
			return path;
		} else {
			Vector2[] newPath = new Vector2[path.Length + 1];
			newPath [0] = path [0];
			newPath[newPath.Length - 1] = path [path.Length - 1];
			for (int i = 0; i < (path.Length - 1); i++) {
					newPath[i + 1] = (path[i] + path[i + 1])/2;
			}
			return smoothPath (newPath, level - 1);
		}
	}

	private void move () {
		
		if ((transform.position - new Vector3 (nextPos.x, transform.position.y, nextPos.y)).magnitude < moveThreshold) {
			robotAnim.SetFloat("PlanarSpeed", 0f);
			updatePlayerLayer();
			isMoving = false;
			return;
		}

		Vector2 pos2 = new Vector2 (transform.position.x, transform.position.z);
		if ((pos2 - path [currentIdx]).magnitude < moveThreshold) {
			currentIdx -= 1;
		}

        
        Int2 pos = new Int2 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.z));
		Vector3 nextPos3 = new Vector3 (path [currentIdx].x, transform.position.y, path [currentIdx].y);
		robotAnim.SetFloat("PlanarSpeed", (nextPos3 - transform.position).magnitude * speedFactor);
		Debug.Log ("Speed is: ");
		Debug.Log (transform.gameObject.GetComponent<Rigidbody> ().velocity);
		transform.position = Vector3.MoveTowards (transform.position, nextPos3, moveThreshold / 4);
		
		Vector3 rotV = nextPos3 - transform.position;
		Quaternion rot = Quaternion.LookRotation(rotV.normalized);
		// Slerp to it over time:
		body.transform.rotation = Quaternion.Slerp(body.transform.rotation, rot, smoothing * Time.deltaTime);

		robotAnim.SetFloat("YSpeed", 0f);


		/*
		Int2 pos = new Int2 (Mathf.RoundToInt (transform.position.x), Mathf.RoundToInt (transform.position.z));
		if (pos.Equals(nextPos) && currentIdx > 0) {
			currentIdx -= 1;
			nextPos = path[currentIdx];
		}

		Vector3 nextPos3 = new Vector3(nextPos._x, transform.position.y, nextPos._z);

        transform.Translate((nextPos3 - transform.position) * speedFactor * Time.deltaTime);
		body.transform.rotation = Quaternion.LookRotation (nextPos3 - transform.position);
        robotAnim.SetFloat("PlanarSpeed", (nextPos3 - transform.position).magnitude * speedFactor);
        robotAnim.SetFloat("YSpeed", 0f);

        updatePlayerLayer();

        if ((nextPos3 - transform.position).magnitude < moveThreshold) {
            robotAnim.SetFloat("PlanarSpeed", 0f);
            isMoving = false;
        }
        */
	}

    private void updatePlayerLayer()
    {
        if (prevGroup == null)
        {
            prevGroup = gridSystem.ComputeGroup(new Vector2(transform.position.x, transform.position.z));
            foreach (Transform child in prevGroup.gameObject.transform)
                child.gameObject.layer = Utility.ToLayerNumber(gridSystem.lockedPfLayer);//(?)
        }

        var currGroup = gridSystem.ComputeGroup(new Vector2(transform.position.x, transform.position.z));
        if (currGroup != prevGroup)
        {
            foreach (Transform child in prevGroup.gameObject.transform)
                child.gameObject.layer = Utility.ToLayerNumber(gridSystem.movablePfLayer);//(?)

            foreach (Transform child in currGroup.gameObject.transform)
                child.gameObject.layer = Utility.ToLayerNumber(gridSystem.lockedPfLayer);//(?)

            prevGroup = currGroup;
        }
    }

    public void BehaveScare()
    {
        robotAnim.SetTrigger("BehaveScare");
        ChangeExpression(Expression.Scared);
        sound.PlayMajorSound(RobotSound.MajorSoundType.Scared);
    }

    public void PointExit()
    {
        robotAnim.SetTrigger("Pointing");
        sound.PlayOtherSound(RobotSound.OtherSoundType.Point); 
    }

    public void ChangeExpression(Expression exp)
    {
        if (faceMatInst == null)
        {
            faceMatInst = Material.Instantiate(faceMat);
            MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
            foreach (MeshRenderer r in renderers)
            {
                if (r.gameObject.name == "face")
                {
                    r.material = faceMatInst;
                    break;
                }
            }
        }
        switch (exp)
        {
            case Expression.Smile: { faceMatInst.SetTextureOffset("_MainTex", new Vector2(0f, 0f)); break; }
            case Expression.Happy: { faceMatInst.SetTextureOffset("_MainTex", new Vector2(0f, 0.75f)); break; }
            case Expression.Scared: { faceMatInst.SetTextureOffset("_MainTex", new Vector2(0f, 0.5f)); break; }
            case Expression.Surprised: { faceMatInst.SetTextureOffset("_MainTex", new Vector2(0f, 0.25f)); break; }
            case Expression.Stoic: { faceMatInst.SetTextureOffset("_MainTex", new Vector2(0.5f, 0f)); break; }
            default: { break; }
        }
    }

    public enum Expression
    {
        Smile, Happy, Scared, Surprised, Stoic
    }
}
