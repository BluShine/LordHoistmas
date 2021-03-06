﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class MainUI : MonoBehaviour {

    ThrowUI throwUI;
    List<Hoistable> player1Hoists;
    List<Hoistable> player2Hoists;
    bool finished = false;
    bool won = false;

    public Text text;

    public Color p1Color;
    public Color p2Color;
    public Color neutralColor;

    public string opponentName = "Opponent";

    public string nextScene;

    bool readyRestart = false;
    bool readyContinue = false;

	// Use this for initialization
	void Start () {
        throwUI = FindObjectOfType<ThrowUI>();
        player1Hoists = new List<Hoistable>();
        player2Hoists = new List<Hoistable>();
        ShowPetardText();
    }
	
	// Update is called once per frame
	void Update () {
		if(Input.GetButtonDown("Jump"))
        {
            if(readyRestart)
            {
                SceneManager.LoadScene(SceneManager.GetActiveScene().name);
            } else if (readyContinue)
            {
                SceneManager.LoadScene(nextScene);
            }
        }
	}

    public void ShowPetardFail()
    {
        text.text = "Petard lost. Try again!";
        text.color = neutralColor;
    }

    public void ShowSelfHoist()
    {
        text.text = "HOISTED BY YOUR OWN PETARD!\npress space";
        text.color = neutralColor;
        readyRestart = true;
    }

    public void ShowPetardText()
    {
        text.text = "Place the Petard";
        text.color = neutralColor;
    }

    public void ShowP1Text()
    {
        text.text = "Throw towards the Petard.";
        text.color = p1Color;
    }

    public void ShowP2Text()
    {
        text.text = opponentName + " is throwing.";
        text.color = p2Color;
    }

    public void showHoistText()
    {
        text.text = "HOIST!";
        text.color = neutralColor;
    }

    public void finishHoist(Hoistable hoistable)
    {
        if(hoistable.team == 0)
        {
            player1Hoists.Add(hoistable);
        } else
        {
            player2Hoists.Add(hoistable);
        }

        bool allDone = true;
        foreach(Hoistable h in FindObjectsOfType<Hoistable>())
        {
            if (!h.finished) allDone = false;
        }
        if(allDone)
        {
            foreach(Hoistable h in FindObjectsOfType<Hoistable>())
            {
                h.finish();
            }
            finished = true;
            DeclareWinner();
        }
    }

    void DeclareWinner()
    {
        Hoistable p1Max = null;
        Hoistable p2Max = null;
        Petard petard = FindObjectOfType<Petard>();
        foreach(Hoistable h in FindObjectsOfType<Hoistable>())
        {
            if(h.team == 0)
            {
                if(p1Max == null || p1Max.highestPoint.y < h.highestPoint.y)
                {
                    p1Max = h;
                }
            } else
            {
                if (p2Max == null || p2Max.highestPoint.y < h.highestPoint.y)
                {
                    p2Max = h;
                }
            }
        }

        CameraMover cam = FindObjectOfType<CameraMover>();
        cam.spinMode = true;
        if (p1Max == null && p2Max == null)
        {
            text.text = "FAILURE TO HOIST!";
            text.color = neutralColor;
            cam.spinTarget = petard.transform;
            readyRestart = true;
        } else if (p1Max == null)
        {
            text.text = opponentName + " won by default!";
            text.color = p2Color;
            cam.spinTarget = p2Max.transform;
            readyRestart = true;
        } else if (p2Max == null)
        {
            text.text = "You win by default!";
            text.color = p1Color;
            won = true;
            cam.spinTarget = p1Max.transform;
            readyContinue = true;
        } else if (p1Max.highestPoint.y == p2Max.highestPoint.y)
        {
            text.text = "DRAW! Hoisted " + p1Max.highestPoint.y.ToString("F2") + "m";
            text.color = neutralColor;
            cam.spinTarget = petard.transform;
            readyRestart = true;
        } else if (p1Max.highestPoint.y > p2Max.highestPoint.y)
        {
            text.text = "YOU WIN! Hoisted " + p1Max.highestPoint.y.ToString("F2") + "m";
            text.color = p1Color;
            won = true;
            cam.spinTarget = p1Max.transform;
            readyContinue = true;
        } else
        {
            text.text = opponentName + " won! Hoisted " + p1Max.highestPoint.y.ToString("F2") + "m";
            text.color = p2Color;
            cam.spinTarget = p2Max.transform;
            readyRestart = true;
        }
        text.text = text.text + "\npress space";
    }
}
