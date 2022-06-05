using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class StealFirstPlayer : MonoBehaviour
{
    public GameObject playerContrObject;
    public PlayerController playerPlaying;
    public PlayerController playerController;
    public GameObject mainStealObject;
    public Power power;

    // Start is called before the first frame update
    void Start()
    {
        power.GetComponent<Power>();
    }

    public void OnClickStealFirst()
    {
        // open the first guy ownedPanel
        if(playerContrObject != null)
        {
            playerController = playerContrObject.GetComponent<PlayerController>();          

            if(playerController.panelOwnedCards.transform.childCount > 20 && playerPlaying.actionName == "" && playerController.previousPower != "FullDefense")
            {
                playerPlaying.view.RPC("PassController", RpcTarget.All, playerController.view.ViewID);
                GameObject child = playerContrObject.transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;
                childPOwnedPanel.SetActive(true);
                mainStealObject.SetActive(false);
            }
            else if((playerController.panelOwnedCards.transform.childCount <= 20 && playerPlaying.actionName == "") || (playerController.previousPower == "FullDefense" && playerPlaying.actionName == ""))
            {
                StartCoroutine(playerController.gameManager.FadeTextToZeroAlpha(5f, "Can't on that player"));
                playerPlaying.closeStealPanelButton.SetActive(true);
            }
        }

        // click 1 card and steal (do on the player Controller )
    }

    public void OnClickNegativeOnEnemy()
    {
        // open the first guy ownedPanel
        if (playerContrObject != null)
        {
            playerController = playerContrObject.GetComponent<PlayerController>();

            if (playerController.panelOwnedCards.transform.childCount < 35 && playerPlaying.actionName == "NegativeOnEnemy" && playerController.previousPower != "FullDefense")
            {
                playerPlaying.view.RPC("PassController", RpcTarget.All, playerController.view.ViewID);
                playerPlaying.ActionNegativeOnEnemy(playerPlaying, playerController);
                mainStealObject.SetActive(false);
            }
            else if((playerController.previousPower == "FullDefense" && playerPlaying.actionName == "NegativeOnEnemy") ||(playerPlaying.actionName == "NegativeOnEnemy" && playerController.panelOwnedCards.transform.childCount >= 35))
            {
                StartCoroutine(playerController.gameManager.FadeTextToZeroAlpha(5f, "Can't on that player"));
                playerPlaying.closeStealPanelButton.SetActive(true);
            }
        }

        // click 1 card and steal (do on the player Controller )
    }

    public void OnClickStealPoints()
    {
        // open the first guy ownedPanel
        if (playerContrObject != null)
        {
            playerController = playerContrObject.GetComponent<PlayerController>();

            if (playerPlaying.actionName == "StealPoints" && playerController.victoryPoints >= 5 && playerController.previousPower != "FullDefense")
            {
                playerPlaying.view.RPC("PassController", RpcTarget.All, playerController.view.ViewID);
                playerPlaying.view.RPC("StealPoints", RpcTarget.All, playerPlaying.view.ViewID);
                mainStealObject.SetActive(false);
            }
            else if((playerPlaying.actionName == "StealPoints" && playerController.previousPower == "FullDefense") || (playerPlaying.actionName == "StealPoints" && playerController.victoryPoints < 5))
            {
                StartCoroutine(playerController.gameManager.FadeTextToZeroAlpha(5f, "Can't on that player"));
                playerPlaying.closeStealPanelButton.SetActive(true);
            }
        }

        // click 1 card and steal (do on the player Controller )
    }

    public void OnClickCloseMainPanel()
    {
        if(playerPlaying.canClickButtons)
        {
            mainStealObject.SetActive(false);
            playerPlaying.powerChosen = "";
            playerPlaying.actionName = "";
        }

        if(playerPlaying.actionName == "NegativeOnEnemy")
        {
            playerPlaying.listOfCardsPower.Clear();
            playerPlaying.listOfCardsIndex.Clear();
        }
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
