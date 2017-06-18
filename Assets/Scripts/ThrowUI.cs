using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ThrowUI : MonoBehaviour {

    public BallThrower thrower;
    public Collider target;
    public Transform rotatingParent;
    public Transform crosshair;
    public Transform innerCircle;
    public LineRenderer targetLine;
    public Transform targetDot;
    public Text buttonPrompt;
    public Text ballCountText;
    List<Rigidbody> lastBalls;
    Transform petard;
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
    bool Player2AI = true;
    bool aimToggle = false;
    enum TossState
    {
        Aim, Throwing, Wait
    };
    TossState state = TossState.Aim;
    float waitTimer = 0;
    static float WAITAMOUNT = .2f;

    static float LINETIMESTEP = .1f;
    static float LINEDRAG = .5f;

    List<int> bag;

    //throwing properties
    public float maxForce = 20;
    public float barrierSpinForce = 10;

    bool thrownPetard = false;
    bool p1Turn = true;

    static float aiAimA = -.00105f;
    static float aiAimB = .0679f;
    static float aiAimC = .1132f;

    Vector3 aiSpin;

	// Use this for initialization
	void Start () {
        updateLine();
        bag = new List<int>();
        barriers = new List<GameObject>();
        mainUI = FindObjectOfType<MainUI>();
        lastBalls = new List<Rigidbody>();
        buttonPrompt.enabled = false;
        mainUI.ShowPetardText();
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

            if(Mathf.Abs(throwScale - targetScale) < .05f)
            {
                buttonPrompt.enabled = true;
                buttonPrompt.canvasRenderer.SetAlpha(1 - Mathf.Abs(throwScale - targetScale) / .05f);
            } else
            {
                buttonPrompt.enabled = false;
            }

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
                targetLine.enabled = false;
                targetDot.transform.position = Vector3.down * 20;
                enabled = false;
                mainUI.showHoistText();
            } else if (!thrownPetard) {
                //mainUI.ShowPetardText();
                ballCountText.text = "Petard";
            }
            else if (p1Turn)
            {
                ballCountText.text = "Ball " + Mathf.FloorToInt(1 + ballsThrown / 2) + "/" + ballsPerPlayer;
                mainUI.ShowP1Text();
            } else
            {
                mainUI.ShowP2Text();
            }
            targetLine.gameObject.SetActive(true);
            targetDot.gameObject.SetActive(true);
            rotatingParent.gameObject.SetActive(false);
            if (!p1Turn && Player2AI)
            {
                Vector3 dir = thrower.transform.position - petard.position;
                targetRotation = Mathf.Atan(dir.x / dir.z) * Mathf.Rad2Deg;
                float mag = new Vector2(dir.x, dir.z).magnitude;
                targetElevation = -20;
                targetScale = Mathf.Clamp((aiAimA * mag * mag + aiAimB * mag + aiAimC) * .75f, startScale, 1);
                aiSpin = Vector3.zero;
                aiSpin += Vector3.right * Random.Range(-2, 2);
                aiSpin += (Vector3.down + Vector3.forward) * Random.Range(-2, 2);
                aiSpin *= barrierSpinForce;
            }
            else
            {
                if (Input.GetButtonDown("Toggle")) aimToggle = !aimToggle;
                if (!aimToggle)
                {
                    targetScale = Mathf.Clamp(targetScale + Input.GetAxis("Vertical") * (aimSpeed / 90f) * Time.deltaTime, startScale, 1);
                }
                else
                {
                    targetElevation = Mathf.Clamp(targetElevation + Input.GetAxis("Vertical") * aimSpeed * Time.deltaTime, -80, 0);
                }
                targetRotation = Mathf.Clamp(targetRotation + Input.GetAxis("Horizontal") * aimSpeed * Time.deltaTime, -180, 180);
            }
            thrower.transform.rotation = Quaternion.Euler(targetElevation, targetRotation, 0);
            innerCircle.localScale = targetScale * Vector3.one;
            if(drawLine && Input.GetAxis("Vertical") != 0 || Input.GetAxis("Horizontal") != 0)
            {
                updateLine();
            }
            if((p1Turn || !Player2AI) && Input.GetButtonDown("Jump"))
            {
                startThrow();
            } else if (!p1Turn && Player2AI)
            {
                aiThrow();
            }
        } else if (state == TossState.Wait)
        {
            waitTimer -= Time.deltaTime;
            targetLine.gameObject.SetActive(false);
            targetDot.gameObject.SetActive(false);
            rotatingParent.gameObject.SetActive(false);
            float maxVel = 0;
            foreach(Rigidbody b in lastBalls)
            {
                if(b != null)
                {
                    maxVel = Mathf.Max(maxVel, b.velocity.magnitude);
                }
            }
            if(maxVel >= .1f)
            {
                waitTimer = WAITAMOUNT;
            }
            if (waitTimer <= 0)
            {
                if (petard == null)
                {
                    PetardFail();
                    state = TossState.Aim;
                }
                else if (new Vector2(thrower.transform.position.x - petard.transform.position.x,
                  thrower.transform.position.z - petard.transform.position.z).magnitude < 5f)
                {
                    SelfHoist();
                    petard.GetComponent<Petard>().Hoist();
                    FindObjectOfType<CameraMover>().HoistedByOwnPetard();
                    waitTimer = 9999;
                }
                else
                {
                    state = TossState.Aim;
                }
            }
        }
    }

    void startThrow()
    {
        spawnBarriers();
        rotatingParent.localRotation = Quaternion.Euler(Vector3.zero);
        state = TossState.Throwing;
        throwScale = startScale;
        target.transform.localScale = Vector3.one * throwScale;
        ballSpin = Vector3.zero;
    }

    void aiThrow()
    {
        throwScale = targetScale;
        ballSpin = aiSpin;
        endThrow();
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
                    Barrier bar = GameObject.Instantiate<GameObject>(barrierPrefabs[pullBag()], transform).GetComponent<Barrier>();
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
                    Barrier bar = GameObject.Instantiate<GameObject>(barrierPrefabs[pullBag()], transform).GetComponent<Barrier>();
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
        waitTimer = WAITAMOUNT;
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
                lastBalls.Add(thrower.tossP1Ball(thrower.transform.forward * throwScale * maxForce, ballSpin).GetComponent<Rigidbody>());
            } else
            {
                lastBalls.Add(thrower.tossP2Ball(thrower.transform.forward * throwScale * maxForce, ballSpin).GetComponent<Rigidbody>());
            }
            ballsThrown++;
            p1Turn = !p1Turn;
        } else
        {
            petard = thrower.tossPetard(thrower.transform.forward * throwScale * maxForce, ballSpin).transform;
            lastBalls.Add(petard.GetComponent<Rigidbody>());
            thrownPetard = true;
        }

        targetElevation = -20;
        targetRotation = 0;
        targetScale = .5f;
        updateLine();
        buttonPrompt.enabled = false;
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

    void PetardFail()
    {
        mainUI.ShowPetardFail();
        thrownPetard = false;
        p1Turn = true;
    }

    void SelfHoist()
    {
        mainUI.ShowSelfHoist();
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
