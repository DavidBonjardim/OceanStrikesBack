using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class Power : MonoBehaviour
{
    public PlayerController[] playerController;
    public bool getPlayersPower;


    PhotonView view;

    public GameManager gameManager;

    public bool doOnceManager;

    public bool[] spotsAvailableJoker = new bool[] { true, true };
    public bool[] spotsAvailableFullDefense = new bool[] { true, true };
    public bool[] spotsAvailableTripleAction = new bool[] { true, true };
    public bool[] spotsAvailableStealCard = new bool[] { true, true };
    
    private void Start()
    {
        this.view = GetComponent<PhotonView>();
        getPlayersPower = true;
        //gameManager = FindObjectOfType<GameManager>();

    }

    [PunRPC]
    public void updateSpotsAvailableJoker(int i, string strValue)
    {
        if (strValue == "False")
            spotsAvailableJoker[i] = false;
        else
            spotsAvailableJoker[i] = true;
    }
    [PunRPC]
    public void updateSpotsAvailableFullDefense(int i, string strValue)
    {
        if (strValue == "False")
            spotsAvailableFullDefense[i] = false;
        else
            spotsAvailableFullDefense[i] = true;
    }
    [PunRPC]
    public void updateSpotsAvailableTripleAction(int i, string strValue)
    {
        if(strValue == "False")
            spotsAvailableTripleAction[i] = false;
        else
            spotsAvailableTripleAction[i] = true;
    }
    [PunRPC]
    public void updateSpotsAvailableStealCard(int i, string strValue)
    {
        if (strValue == "False")
            spotsAvailableStealCard[i] = false;
        else
            spotsAvailableStealCard[i] = true;
    }

    public void TripleActionPower()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.canChoosePower && c.previousPower != "TripleActionPower")
            {
                int a = 0;
                for(int i = 0; i < 2; i++)
                {
                    if(spotsAvailableTripleAction[i] == true)
                    {
                        if (c.previousPower == "FullDefensePower")
                        {
                            for(int j = 0; j < 2; j++)
                            {
                                if(spotsAvailableFullDefense[j] == false)
                                {
                                    view.RPC("updateSpotsAvailableFullDefense", RpcTarget.All, j, "True");
                                }
                            }
                        }
                        else if (c.previousPower == "StealCardPower")
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (spotsAvailableStealCard[j] == false)
                                {
                                    view.RPC("updateSpotsAvailableStealCard", RpcTarget.All, j, "True");
                                }
                            }
                        }
                        else if (c.previousPower == "JokerCardPower")
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (spotsAvailableJoker[j] == false)
                                {
                                    view.RPC("updateSpotsAvailableJoker", RpcTarget.All, j, "True");
                                }
                            }
                        }

                        c.view.RPC("UpdatePlayers", RpcTarget.All, c.view.ViewID, "TripleActionPower");

                        view.RPC("updateSpotsAvailableTripleAction", RpcTarget.All, i, "False");

                        gameManager.photonView.RPC("FadeTextToZeroAlphaRPC", RpcTarget.All, 5f, (c.namePlayer + " Triple Action"));

                        Debug.Log("Triple action: " + c.actionsAvailable);


                        return;
                    }
                    else
                    {
                        a++;
                    }
                }

                if(a == 2)
                {
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Power spots are full"));
                }

            }
            else if (c.previousPower == "TripleActionPower")
            {
                StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Can't choose same power"));

            }
        }
    }

    public void FullDefensePower()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.canChoosePower && c.previousPower != "FullDefensePower")
            {
                int a = 0;
                for (int i = 0; i < 2; i++)
                {
                    if (spotsAvailableFullDefense[i] == true)
                    {
                        if (c.previousPower == "TripleActionPower")
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (spotsAvailableTripleAction[j] == false)
                                {
                                    view.RPC("updateSpotsAvailableTripleAction", RpcTarget.All, j, "True");
                                }
                            }
                        }
                        else if (c.previousPower == "StealCardPower")
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (spotsAvailableStealCard[j] == false)
                                {
                                    view.RPC("updateSpotsAvailableStealCard", RpcTarget.All, j, "True");
                                }
                            }
                        }
                        else if (c.previousPower == "JokerCardPower")
                        {
                            for (int j = 0; j < 2; j++)
                            {
                                if (spotsAvailableJoker[j] == false)
                                {
                                    view.RPC("updateSpotsAvailableJoker", RpcTarget.All, j, "True");
                                }
                            }
                        }

                        c.view.RPC("UpdatePlayers", RpcTarget.All, c.view.ViewID, "FullDefensePower");

                        view.RPC("updateSpotsAvailableFullDefense", RpcTarget.All, i, "False");

                        gameManager.photonView.RPC("FadeTextToZeroAlphaRPC", RpcTarget.All, 5f, (c.namePlayer + " Full Defense"));

                        Debug.Log("FullDefenseActivated");


                        return;
                    }
                    else
                    {
                        a++;
                    }
                }

                if (a == 2)
                {
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Power spots are full"));
                }

            }
            else if(c.previousPower == "FullDefensePower")
            {
                StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Can't choose same power"));
                //gameManager.photonView.RPC("FadeTextToZeroAlphaRPC", RpcTarget.All, 5f, (c.namePlayer + " can't choose same power"));
            }

        }
    }

    public void StealCardPower()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.canChoosePower && c.previousPower != "StealCardPower")
            {
                if (c.gameManager.playerControllersInGame.Length > 1)
                {
                    int a = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        if (spotsAvailableStealCard[i] == true)
                        {
                            if (c.previousPower == "FullDefensePower")
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (spotsAvailableFullDefense[j] == false)
                                    {
                                        view.RPC("updateSpotsAvailableFullDefense", RpcTarget.All, j, "True");
                                    }
                                }
                            }
                            else if (c.previousPower == "TripleActionPower")
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (spotsAvailableTripleAction[j] == false)
                                    {
                                        view.RPC("updateSpotsAvailableTripleAction", RpcTarget.All, j, "True");
                                    }
                                }
                            }
                            else if (c.previousPower == "JokerCardPower")
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (spotsAvailableJoker[j] == false)
                                    {
                                        view.RPC("updateSpotsAvailableJoker", RpcTarget.All, j, "True");
                                    }
                                }
                            }

                            c.view.RPC("UpdatePlayers", RpcTarget.All, c.view.ViewID, "StealCardPower");

                            view.RPC("updateSpotsAvailableStealCard", RpcTarget.All, i, "False");

                            gameManager.photonView.RPC("FadeTextToZeroAlphaRPC", RpcTarget.All, 5f, (c.namePlayer + " Stealing Card"));

                            c.StealCardPower();

                            Debug.Log("Can Steal Card");

                            return;
                        }
                        else
                        {
                            a++;
                        }
                    }
                    if (a == 2)
                    {
                        StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Power spots are full"));
                    }
                }
                else
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Noone to steal"));


            }
            else if (c.previousPower == "StealCardPower")
            {
                StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Can't choose same power"));
            }
        }
    }

    public void JokerCardPower()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.canChoosePower && c.previousPower != "JokerCardPower")
            {
                if(c.panelPlaceCards.transform.childCount == 34)
                {
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(2f, "Beach is full"));
                }
                else if (c.myOwnedCards.Count > 0)
                {
                    int a = 0;
                    for (int i = 0; i < 2; i++)
                    {
                        if (spotsAvailableJoker[i] == true)
                        {
                            if (c.previousPower == "FullDefensePower")
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (spotsAvailableFullDefense[j] == false)
                                    {
                                        view.RPC("updateSpotsAvailableFullDefense", RpcTarget.All, j, "True");
                                    }
                                }
                            }
                            else if (c.previousPower == "TripleActionPower")
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (spotsAvailableTripleAction[j] == false)
                                    {
                                        view.RPC("updateSpotsAvailableTripleAction", RpcTarget.All, j, "True");
                                    }
                                }
                            }
                            else if (c.previousPower == "StealCardPower")
                            {
                                for (int j = 0; j < 2; j++)
                                {
                                    if (spotsAvailableStealCard[j] == false)
                                    {
                                        view.RPC("updateSpotsAvailableStealCard", RpcTarget.All, j, "True");
                                    }
                                }
                            }

                            c.view.RPC("UpdatePlayers", RpcTarget.All, c.view.ViewID, "JokerCardPower");

                            view.RPC("updateSpotsAvailableJoker", RpcTarget.All, i, "False");

                            gameManager.photonView.RPC("FadeTextToZeroAlphaRPC", RpcTarget.All, 5f, (c.namePlayer + " Joker Card"));

                            c.JokerCardPower();

                            Debug.Log("Joker Card Power");

                            return;
                        }
                        else
                        {
                            a++;
                        }
                    }

                    if (a == 2)
                    {
                        StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Power spots are full"));
                    }
                }
                else
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "You need cards first"));
                
            }
            else if (c.previousPower == "JokerCardPower")
            {
                StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Can't choose same power"));
            }
        }
    }


    // Update is called once per frame
    void Update()
    {
        if (!doOnceManager)
        {
            gameManager = FindObjectOfType<GameManager>();
            doOnceManager = true;
        }
    }

    private void LateUpdate()
    {
        if (getPlayersPower)
        {
            playerController = FindObjectsOfType<PlayerController>();
            getPlayersPower = false;

        }
    }
}
