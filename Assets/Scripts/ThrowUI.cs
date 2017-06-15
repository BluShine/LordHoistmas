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
    MainUI mainUI;

    public GameObject[] barrierPrefabs;

    List<GameObject> barriers;

    public int ballsPerPlayer = 4;
    int ballsThrown = 0;

    public float spinSpeed = 10;
    public float aimSpeed = 10;
    public float scaleSpeed = .5f;
    public float startScale = .1f;
    float targetScale = .5f;
    float throwScale = .1f;
    float targetRotation = 0;
    float targetElevation = -20f;
    Vector3 ballSpin;
    bool drawLine = true;
    bool aimToggle = false;
    enum TossState
    {
        Aim, Throwing, Wait
    };
    TossState state = TossState.Aim;

    static float LINETIMESTEP = .1f;
    static float LINEDRAG = .5f;

    List<int> bag;

    //throwing properties
    public float maxForce = 20;
    public float barrierSpinForce = 10;

    bool thrownPetard = false;
    bool p1Turn = true;

	// Use this for initialization
	void Start () {
        updateLine();
        bag = new List<int>();
        barriers = new List<GameObject>();
        mainUI = FindObjectOfType<MainUI>();
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
            if (ballsThrown >= ballsPerPlayer * 2)
            {
                FindObjectOfType<Petard>().Hoist();
                thrower.enabled = false;
                enabled = false;
                mainUI.showHoistText();
            } else if (!thrownPetard) {
                mainUI.ShowPetardText();
            } else if (p1Turn)
            {
                mainUI.ShowP1Text();
            } else
            {
                mainUI.ShowP2Text();
            }
            targetLine.gameObject.SetActive(true);
            targetDot.gameObject.SetActive(true);
            rotatingParent.gameObject.SetActive(false);
            if (Input.GetButtonDown("Toggle")) aimToggle = !aimToggle;
            if (!aimToggle)
            {
                targetScale = Mathf.Clamp(targetScale + Input.GetAxis("Vertical") * (aimSpeed / 90f) * Time.deltaTime, startScale, 1);
            } else
            {
                targetElevation = Mathf.Clamp(targetElevation + Input.GetAxis("Vertical") * aimSpeed * Time.deltaTime, -80, 0);
            }
            targetRotation = Mathf.Clamp(targetRotation + Input.GetAxis("Horizontal") * aimSpeed * Time.deltaTime, -180, 180);
            thrower.transform.rotation = Quaternion.Euler(targetElevation, targetRotation, 0);
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
        spawnBarriers();
        rotatingParent.localRotation = Quaternion.Euler(Vector3.zero);
        state = TossState.Throwing;
        throwScale = startScale;
        target.transform.localScale = Vector3.one * throwScale;
        ballSpin = Vector3.zero;
    }

    void spawnBarriers()
    {
        refreshBag();
        barriers = new List<GameObject>();
        for (int i = 0; i < 7; i++)//layers 
        {
            if(i < 3)
            {
                for(int j = 0; j < 2; j++)
                {
                    Barrier bar = GameObject.Instantiate<GameObject>(barrierPrefabs[pullBag()]).GetComponent<Barrier>();
                    bar.curveDirection *= barrierSpinForce;
                    bar.transform.position = transform.position;
                    bar.transform.localScale = Vector3.one * (.5f + Mathf.Pow(6, i));
                    bar.setSize(j);
                    barriers.Add(bar.gameObject);
                }
            } else
            {
                refreshBag();
                for (int j = 0; j < 3; j++)
                {
                    Barrier bar = GameObject.Instantiate<GameObject>(barrierPrefabs[pullBag()]).GetComponent<Barrier>();
                    bar.curveDirection *= barrierSpinForce;
                    bar.transform.position = transform.position;
                    bar.transform.localScale = Vector3.one * (.5f + Mathf.Pow(6, i));
                    bar.setSize(j/2f);
                    barriers.Add(bar.gameObject);
                }
            }
        }
    }

    void refreshBag()
    {
        bag = new List<int>();
        List<int> shuffle = new List<int>();
        shuffle.Add(0);
        shuffle.Add(1);
        shuffle.Add(2);
        shuffle.Add(3);

        for (int i = 0; i < 4; i++)
        {
            int j = Random.Range(0, shuffle.Count);
            bag.Add(shuffle[j]);
            shuffle.RemoveAt(j);
        }
    }

    int pullBag()
    {
        if(bag.Count == 0)
        {
            refreshBag();
        }
        int result = bag[Random.Range(0, bag.Count)];
        bag.Remove(result);
        return result;
    }

    public void endThrow()
    {
        state = TossState.Wait;
        //remove barriers
        foreach (GameObject bar in barriers)
        {
            if (bar != null) Destroy(bar);
        }
        //throw object
        if (thrownPetard)
        {
            if(p1Turn)
            {
                lastBall = thrower.tossP1Ball(thrower.transform.forward * throwScale * maxForce, ballSpin).GetComponent<Rigidbody>();
            } else
            {
                lastBall = thrower.tossP2Ball(thrower.transform.forward * throwScale * maxForce, ballSpin).GetComponent<Rigidbody>();
            }
            ballsThrown++;
            p1Turn = !p1Turn;
        } else
        {
            lastBall = thrower.tossPetard(thrower.transform.forward * throwScale * maxForce, ballSpin).GetComponent<Rigidbody>();
            thrownPetard = true;
        }
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
        if (ended)
        {
            targetDot.transform.position = rayHit.point + Vector3.up * .05f;
        } else
        {
            targetDot.transform.position = Vector3.down * 10;
        }
    }

    public void hit(Collider t)
    {
        Barrier bar = t.GetComponentInParent<Barrier>();
        if(bar != null)
        {
            ballSpin += bar.curveDirection;
            Destroy(bar.gameObject);
        }
    }
}
