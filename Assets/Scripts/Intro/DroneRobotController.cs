using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class DroneRobotController : MonoBehaviour
{
    public Animator robotAnimator;
    public Animation robotAnimation;
    public Material faceMat;
    public Material bodyMat;
    private RobotSound sound;

    private Material bodyMatInst;
    private Material faceMatInst;

	public Image hintBubble;
	public Image hintBubble2;

    public bool IsMoving { get; private set; }

    private void Awake()
    {
        IsMoving = false;
        sound = GetComponent<RobotSound>();
        sound.PlayMajorSound(RobotSound.MajorSoundType.Idle);

        bodyMatInst = Material.Instantiate(bodyMat);
        faceMatInst = Material.Instantiate(faceMat);

        MeshRenderer[] renderers = GetComponentsInChildren<MeshRenderer>();
        foreach (MeshRenderer r in renderers)
        {
            if (r.gameObject.name == "face")
                r.material = faceMatInst;
            else
                r.material = bodyMatInst;
        }
		hintBubble.enabled = false;
		hintBubble2.enabled = false;
        //ChangeExpression(Expression.Smile);
    }

    public void Wave()
    {
        robotAnimator.SetTrigger("Wave");
        sound.PlayOtherSound(RobotSound.OtherSoundType.Wave);
    }

    public void Celebrate()
    {
        robotAnimator.SetTrigger("Celebrate");
        sound.PlayMajorSound(RobotSound.MajorSoundType.Happy);
    }

    public void Warn()
    {
        sound.PlayMajorSound(RobotSound.MajorSoundType.Scared);
    }

    public void Talk()
    {
        sound.PlayOtherSound(RobotSound.OtherSoundType.Talk);
    }

    public void Move(Vector3 delta, float speed)
    {
        IsMoving = true;
        sound.PlayMajorSound(RobotSound.MajorSoundType.Walk);
        StartCoroutine(MoveCoroutine(delta, speed));
        robotAnimator.SetFloat("PlanarSpeed", speed);
        robotAnimator.SetFloat("YSpeed", 0f);
    }

    public void RotateTo(Quaternion target, float angularSpeed)
    {
        IsMoving = true;
        sound.PlayMajorSound(RobotSound.MajorSoundType.Walk);
        StartCoroutine(RotateToCoroutine(target, angularSpeed));
    }

    private IEnumerator MoveCoroutine(Vector3 delta, float period)
    {
        Vector3 old = transform.position;
        float timer = 0f;      
        while (Vector3.Distance(transform.position, old + delta) > 0.01f)
        {
            timer += Time.deltaTime;
            transform.position = Vector3.Lerp(old, old + delta, Mathf.Clamp01(timer / period));
            yield return null;
        }
        //transform.position = old + delta;
        IsMoving = false;
        sound.PlayMajorSound(RobotSound.MajorSoundType.None);
        robotAnimator.SetFloat("PlanarSpeed", 0f);
    }

    private IEnumerator RotateToCoroutine(Quaternion target, float period)
    {
        float elapedTime = 0f;
        Quaternion start = transform.rotation;
        while (Quaternion.Angle(transform.rotation, target) > 0.5f)
        {
            elapedTime += Time.deltaTime;
            transform.rotation = Quaternion.Slerp(start, target, Mathf.Clamp01(elapedTime / period));
            yield return null;
        }
        transform.rotation = target;
        IsMoving = false;
    }

    public void ChangeExpression(Expression exp)
    {
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
        Smile,Happy,Scared,Surprised,Stoic
    }

    public void ChangeColor(Color color)
    {
        bodyMatInst.color = color;
    }

    public void PlayPresetAnimation()
    {
        robotAnimation.Play();
    }
}
