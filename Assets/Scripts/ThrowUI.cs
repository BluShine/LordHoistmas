using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ThrowUI : MonoBehaviour {

    public BallThrower thrower;
    public Collider target;
    public Transform rotatingParent;
    public Transform crosshair;
    public Transform innerCircle;
    public LineRenderer targetLine;
    public Transform targetDot;
    Rigidbody lastBall;

    public float spinSpeed = 10;
    public float aimSpeed = 10;
    public float scaleSpeed = .5f;
    public float startScale = .1f;
    float targetScale = .5f;
    float throwScale = .1f;
    float targetRotation = 0;
    Vector3 ballSpin;
    bool drawLine = true;
    enum TossState
    {
        Aim, Throwing, Wait
    };
    TossState state = TossState.Aim;

    static float LINETIMESTEP = .1f;
    static float LINEDRAG = .5f;

    //throwing properties
    public float maxForce = 20;
    public float barrierSpin = 10;

	// Use this for initialization
	void Start () {
        updateLine();
    }
	
	// Update is called once per frame
	void Update () {
        if (state == TossState.Throwing)
        {
            targetLine.gameObject.SetActive(false);
            targetDot.gameObject.SetActive(false);
            rotatingParent.gameObject.SetActive(true);
            rotatingParent.localRotation = Quaternion.Euler(rotatingParent.localEulerAngles + spinSpeed * Vector3.forward * -Input.GetAxis("Horizontal"));
            throwScale = Mathf.Min(throwScale + Time.deltaTime * scaleSpeed, 1);
            target.transform.localScale = Vector3.one * throwScale;

            if(throwScale == 1 || Input.GetButtonDown("Jump"))
            {
                endThrow();
            }
        } else if (state == TossState.Aim)
        {
            targetLine.gameObject.SetActive(true);
            targetDot.gameObject.SetActive(true);
            rotatingParent.gameObject.SetActive(false);
            targetScale = Mathf.Clamp(targetScale + Input.GetAxis("Vertical") * (aimSpeed / 90f) * Time.deltaTime, startScale, 1);
            targetRotation = Mathf.Clamp(targetRotation + Input.GetAxis("Horizontal") * aimSpeed * Time.deltaTime, -180, 180);
            thrower.transform.rotation = Quaternion.Euler(thrower.transform.rotation.eulerAngles.x, targetRotation, 0);
            innerCircle.localScale = targetScale * Vector3.one;
            if(drawLine && Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                updateLine();
            }
            if(Input.GetButtonDown("Jump"))
            {
                startThrow();
            }
        } else if (state == TossState.Wait)
        {
            targetLine.gameObject.SetActive(false);
            targetDot.gameObject.SetActive(false);
            rotatingParent.gameObject.SetActive(false);
            if (lastBall == null || lastBall.velocity.magnitude < .1f)
            {
                state = TossState.Aim;
            }
        }
    }

    public void startThrow()
    {
        rotatingParent.localRotation = Quaternion.Euler(Vector3.zero);
        state = TossState.Throwing;
        throwScale = startScale;
        target.transform.localScale = Vector3.one * throwScale;
        ballSpin = Vector3.zero;
    }

    public void endThrow()
    {
        state = TossState.Wait;
        lastBall = thrower.tossBall(thrower.transform.forward * throwScale * maxForce, ballSpin).GetComponent<Rigidbody>();
    }

    void updateLine()
    {
        int step = 0;
        Vector3 pos = thrower.transform.position;
        Vector3 vel = thrower.transform.forward * targetScale * maxForce;
        List<Vector3> line = new List<Vector3>();
        line.Add(pos);
        bool ended = false;
        RaycastHit rayHit = new RaycastHit();
        while (step < 200 && !ended)
        {
            vel = (vel + Physics.gravity * LINETIMESTEP) * (1.002f - (LINETIMESTEP * LINEDRAG));
            pos += vel * LINETIMESTEP;
            line.Add(pos);
            Ray ray = new Ray(pos, vel.normalized);
            ended = Physics.Raycast(ray, out rayHit, vel.magnitude * 1.5f * LINETIMESTEP);
            step++;
        }
        targetLine.positionCount = line.Count;
        targetLine.SetPositions(line.ToArray());
        targetDot.transform.position = rayHit.point + Vector3.up * .05f;
    }

    public void hit(Collider t)
    {
        
    }
}
