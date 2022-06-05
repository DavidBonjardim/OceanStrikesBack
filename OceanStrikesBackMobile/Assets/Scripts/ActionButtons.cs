using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionButtons : MonoBehaviour
{
    public GameManager gameManager;

    // Start is called before the first frame update
    void Start()
    {
        gameManager = FindObjectOfType<GameManager>();
    }

    public void OnClickSequenceInc()
    {
        foreach (PlayerController p in gameManager.playerControllersInGame)
        {
            if (p.view.IsMine && p.actionPhase == true && p.actionsAvailable > 0)
            {
                p.ActionSequenceInc(p);
            }
        }
    }

    public void OnClickSequence()
    {
        foreach (PlayerController p in gameManager.playerControllersInGame)
        {
            if (p.view.IsMine && p.actionPhase == true && p.actionsAvailable > 0)
            {
                p.ActionSequence(p);
            }
        }
    }

    public void OnClickTrio()
    {
        foreach (PlayerController p in gameManager.playerControllersInGame)
        {
            if(p.view.IsMine && p.actionPhase == true && p.actionsAvailable > 0)
            {
                p.ActionTrioActivate(p);
            }
        }
    }

    public void OnClickSoloCard()
    {
        foreach (PlayerController p in gameManager.playerControllersInGame)
        {
            if (p.view.IsMine && p.actionPhase == true && p.actionsAvailable > 0)
            {
                p.ActionSoloCard(p);
            }
        }
    }

    public void OnClickNegativeTrio()
    {
        foreach (PlayerController p in gameManager.playerControllersInGame)
        {
            if (p.view.IsMine && p.actionPhase == true && p.actionsAvailable > 0)
            {
                p.ActionNegativeTrioActivate(p);
            }
        }
    }

    public void OnClickStealCard()
    {
        foreach (PlayerController p in gameManager.playerControllersInGame)
        {
            if (p.view.IsMine && p.hasClickedPlaceCards == false && p.polutionPoints >= 4 && p.polutionPoints < 7 && p.actionPhase == true && p.actionsAvailable > 0)
            {
                p.StealCardXRay(p.view.ViewID);
                p.view.RPC("UpdateMidAction", RpcTarget.All, p.view.ViewID);
            }
            else if (p.view.IsMine)
            {
                StartCoroutine(p.FadeTextToZeroAlpha(1f, "Can't do that"));
            }
        }
    }
    
    public void OnClickStealFivePoints()
    {
        foreach (PlayerController p in gameManager.playerControllersInGame)
        {
            if (p.view.IsMine && p.actionPhase == true && p.actionsAvailable > 0 && p.polutionPoints == 6)
            {
                p.actionName = "StealPoints";
                p.StealCardPower();
            }
            else if(p.view.IsMine)
            {
                StartCoroutine(p.FadeTextToZeroAlpha(1f, "Can't do that"));
            }
        }
    }

}
