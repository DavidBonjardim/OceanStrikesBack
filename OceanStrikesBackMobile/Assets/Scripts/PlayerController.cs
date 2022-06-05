using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using Photon.Realtime;
using System;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class PlayerController : MonoBehaviourPunCallbacks, IComparable
{
    public GameObject myGameObject;

    public int victoryPoints = 0;
    public int polutionPoints = 2;

    public string powerChosen;
    public string previousPower;

    public List<Card> myOwnedCards = new List<Card>();
    public List<Card> myPlacedCards = new List<Card>();

    public GameObject panelOwnedCards, panelPlaceCards, boardGamePanel;

    public bool[] availableCardSlots, availablePlacedCardSlots;
    public Transform[] cardSlots, placedCardSlots;

    public GameManager gameManager;

    public GameObject allButtons, deckPoluicao, deckAmbiente, buttonObjectOwnedCards, buttonObjectPlaceCards, middleGameObject;

    public bool canGetCards;

    public PhotonView view;

    public bool canPlay;

    public bool canChoosePower;

    public bool powerHasBeenChosen = false;

    public int cardsPerRound;

    public int actionsAvailable = 2;

    public bool fullDefense = false;

    public bool actionPhase = false;

    public string namePlayer = "";

    public bool doOnceButtons = false;

    public List<string> stringNames = new List<string>();

    public bool canSeeOtherCards;

    public Image powerImage;
    public Image pointsImage;
    public Image polutionImage;

    public GameObject stealCardPowerObject;

    public Image ownedPanelImage, placedPanelImage;

    public GameObject[] playerObjects = new GameObject[4];

    public StealFirstPlayer stealFirst;
    public StealSecondPlayer stealSecond;
    public StealThirdPlayer stealThird;

    public bool canClickButtons = true;

    public PlayerController playerControllerToSteal;

    //placeCards
    public bool hasClickedPlaceCards;
    public GameObject buttonPlaceCards;

    public Image infoImage;
    public Text infoText;

    public Text buttonPlaceCardsText;

    public GameObject allActionButtons;
    public GameObject myActionsButton;
    public GameObject pulliteOthersButton;
    public GameObject closeStealPanelButton;

    public GameObject endActionButton;
    public bool midAction;

    public int amountOfCardsPlaced;

    public Text actionText;
    public Image imageColor;

    //5 minutos
    public float timeLeft = 60;

    public string actionName = "";

    public int[] arrayOfCardIndex = new int[10];
    public List<int> listOfCardsIndex = new List<int>();
    public List<Card> listOfCardsPower = new List<Card>();

    public bool wantToPollute;

    public bool isWinner;
    //public bool canWin;

    public bool gameOver;

    public GameObject playerSereiaObject;

    public AudioSource backgroundMusic;


    private void Awake()
    {
        view = GetComponent<PhotonView>();

        canClickButtons = true;
        canSeeOtherCards = false;
        allButtons.SetActive(false);
        deckAmbiente.SetActive(false);
        deckPoluicao.SetActive(false);

    }


    // Start is called before the first frame update
    void Start()
    {
        if (view.IsMine)
        {
            victoryPoints = 0;
            polutionPoints = 2;
            powerChosen = "";

            myGameObject.GetComponent<GameObject>();

            if (PhotonNetwork.LocalPlayer.IsLocal)
            {
                gameObject.name = "Local";
                deckAmbiente.SetActive(true);
                deckPoluicao.SetActive(true);

                allButtons.SetActive(true);

                hasClickedPlaceCards = false;

            }


        }

        gameOver = false;

        gameManager = FindObjectOfType<GameManager>().GetComponent<GameManager>();
        backgroundMusic = GameObject.FindGameObjectWithTag("MainCamera").GetComponent<AudioSource>();

        boardGamePanel = GameObject.Find("BoardGamePanel");
        middleGameObject = GameObject.Find("MiddleOceanPanel");

        stealCardPowerObject = gameManager.transform.GetChild(2).gameObject;

        myActionsButton.SetActive(true);
        buttonPlaceCards.SetActive(true);
        pulliteOthersButton.SetActive(true);
        panelOwnedCards.SetActive(false);
        panelPlaceCards.SetActive(false);

        if (view.IsMine)
        {
            gameManager.sereiaObject = this.playerSereiaObject;
            playerSereiaObject.SetActive(false);
        }

    }

    public void UpdateButtons(GameObject allButtons)
    {
        Transform childOpenButtons, childCardOwned, childCardsPlaced;

        childOpenButtons = allButtons.transform.GetChild(0);
        childOpenButtons.transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.NickName + " decks";

        childCardOwned = allButtons.transform.GetChild(1);
        childCardOwned.transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.NickName + " cards";

        childCardsPlaced = allButtons.transform.GetChild(2);
        childCardsPlaced.transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.NickName + " beach";


    }

    public void StealCardPower()
    {
        stealCardPowerObject.SetActive(true);
        if (this.powerChosen == "JokerCardPower")
        {
            closeStealPanelButton.SetActive(false);
        }
        else if (this.powerChosen == "StealCardPower")
        {
            closeStealPanelButton.SetActive(true);
        }
        if (this.actionName == "StealPoints")
        {
            panelOwnedCards.SetActive(false);
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);
        }
    }

    public void StealCardXRay(int viewID)
    {

        if (gameManager.playerControllersInGame.Length > 1)
        {
            if (actionPhase == true && actionsAvailable > 0)
            {
                closeStealPanelButton.SetActive(false);
                view.RPC("UseStealPowerToXRay", RpcTarget.All, viewID);
                panelOwnedCards.SetActive(false);
                panelPlaceCards.SetActive(false);
                gameManager.panelMiddleCards.SetActive(true);
                stealCardPowerObject.SetActive(true);
            }
            else if (actionPhase == true)
            {
                StartCoroutine(FadeTextToZeroAlpha(2f, "Not on action phase"));
                return;
            }
        }
        else
        {
            StartCoroutine(FadeTextToZeroAlpha(5f, "Noone to steal"));
            return;
        }



    }

    [PunRPC]
    public void UseStealPowerToXRay(int viewID)
    {
        PlayerController a = GetPlayerByID(viewID);
        a.powerChosen = "StealCardPower";
    }

    public void JokerCardPower()
    {
        this.canClickButtons = false;
        StartCoroutine(TimeToClickActions(6f));
        panelOwnedCards.SetActive(true);

    }

    //NAO POSSO ENVIAR LISTAS NEM CARTAS, TEM QUE SER INTS OU BYTES OU W/E

    /* public void OnClickPlaceCards()
    {
        if (hasClickedPlaceCards == false && actionPhase && wantToPollute == false)
        {
            canClickButtons = false;
            buttonPlaceCardsText.text = "Finish action";
            view.RPC("ClickedPlaceCardsToPlace", RpcTarget.Others, this.view.ViewID);
            StartCoroutine(FadeTextToZeroAlpha(2f, "Click Cards"));
            hasClickedPlaceCards = true;
        }
        else if (actionPhase && wantToPollute == false)
        {
            canClickButtons = true;
            this.actionsAvailable -= 1;
            buttonPlaceCardsText.text = "Click to place cards";
            view.RPC("ClickedPlaceCardsToNotPlace", RpcTarget.Others, this.view.ViewID);
            StartCoroutine(FadeTextToZeroAlpha(2f, "Cards played"));
            hasClickedPlaceCards = false;
        }

    }

    public void FunctionForCardToPlace(Card c, PlayerController playerContr)
    {
        for (int j = 0; j < playerContr.availablePlacedCardSlots.Length; j++)
        {
            if (playerContr.availablePlacedCardSlots[j] == true)
            {
                if (hasClickedPlaceCards && actionPhase && actionsAvailable > 0)
                {
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Click cards to play"));
                    view.RPC("UpdateCardsToPlaced", RpcTarget.All, c.cardOwnedSlot, j);
                }

                return;
            }
            else if (j == playerContr.availablePlacedCardSlots.Length - 1)
            {
                StartCoroutine(FadeTextToZeroAlpha(2f, "Beach is full"));
                return;
            }
        }
    } */

    [PunRPC]
    public void UpdateCardsToPlaced(int cardIndex, int slotPlaced)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().canPlay && players[i].GetComponent<PlayerController>().hasClickedPlaceCards)
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();
                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;
                GameObject childPlacedPanel = child.transform.GetChild(3).gameObject;

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPOwnedPanel.transform.childCount; n++)
                {
                    if (childPOwnedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPOwnedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardOwnedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }


                Card cardComponent = childCardWeWant.GetComponent<Card>();

                //Card

                childCardWeWant.transform.parent = childPlacedPanel.transform;
                childCardWeWant.transform.position = playerController.placedCardSlots[slotPlaced].transform.position;

                cardComponent.cardPlacedSlot = slotPlaced;

                cardComponent.cardImageBack.SetActive(false);
                cardComponent.cardImageFront.SetActive(true);


                //NAO SEI SE O JOKER CONTA
                //cardComponent.isJoker = true;

                //Player
                playerController.availableCardSlots[cardComponent.cardOwnedSlot] = true;
                playerController.availablePlacedCardSlots[slotPlaced] = false;

                playerController.myPlacedCards.Add(cardComponent);

                playerController.powerChosen = "";
                playerController.canClickButtons = true;

                playerController.amountOfCardsPlaced += 1;

                return;
            }
        }
    }

    public void DrawCardAmbiente()
    {
        if (gameManager.deckAmbiente.Count >= 1 && canPlay && canGetCards && powerHasBeenChosen)
        {
            int random = UnityEngine.Random.Range(0, gameManager.deckAmbiente.Count);
            Card randCard = gameManager.deckAmbiente[random];

            for (int i = 0; i < gameManager.availableMidSlots.Length; i++)
            {
                if (gameManager.availableMidSlots[i] == true)
                {
                    GameObject cardIstant = PhotonNetwork.Instantiate(randCard.gameObject.name, new Vector3 (0,0,0), Quaternion.identity);

                    view.RPC("SetParentByStringAmbiente", RpcTarget.All, "MiddleOceanPanel", i, random);

                    return;
                }
            }
        }
    }

    public void DrawCardPoluicao()
    {
        if (gameManager.deckPoluicao.Count >= 1 && canPlay && canGetCards && powerHasBeenChosen)
        {
            int random = UnityEngine.Random.Range(0, gameManager.deckPoluicao.Count);
            Card randCard = gameManager.deckPoluicao[random];

            for (int i = 0; i < gameManager.availableMidSlots.Length; i++)
            {
                if (gameManager.availableMidSlots[i] == true)
                {
                    GameObject cardIstant = PhotonNetwork.Instantiate(randCard.gameObject.name, new Vector3(0, 0, 0), Quaternion.identity);

                    view.RPC("SetParentByStringPoluicao", RpcTarget.All, "MiddleOceanPanel", i, random);

                    return;
                }
            }
        }
    }

    public void ClickOwnedPanelCardSteal(Card card, PlayerController playerContr)
    {
        for (int j = 0; j < playerContr.availableCardSlots.Length; j++)
        {
            if (playerContr.availableCardSlots[j] == true)
            {
                playerContr.canClickButtons = false;

                view.RPC("CardStealToMyOwned", RpcTarget.All, card.cardOwnedSlot, j);

                return;
            }
        }
    }

    [PunRPC]
    public void CardStealToMyOwned(int cardIndex, int j)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().view.ViewID == playerControllerToSteal.view.ViewID)
            {
                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;

                PlayerController playerController = players[i].GetComponent<PlayerController>();

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPOwnedPanel.transform.childCount; n++)
                {
                    if (childPOwnedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPOwnedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardOwnedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }

                for (int u = 0; u < players.Length; u++)
                {
                    if (players[u].GetComponent<PlayerController>().powerChosen == "StealCardPower" && childCardWeWant.GetComponent<Card>() != null)
                    {
                        Card cardComponent = childCardWeWant.GetComponent<Card>();
                        PlayerController pController2 = players[u].GetComponent<PlayerController>();

                        GameObject childSecondPlayer = players[u].transform.GetChild(0).gameObject;
                        GameObject childPOwnedPanelSecondPlayer = childSecondPlayer.transform.GetChild(2).gameObject;

                        childCardWeWant.transform.parent = childPOwnedPanelSecondPlayer.transform;
                        childCardWeWant.transform.position = pController2.cardSlots[j].transform.position;

                        playerController.availableCardSlots[cardIndex] = true;
                        pController2.availableCardSlots[j] = false;

                        cardComponent.cardOwnedSlot = j;
                        cardComponent.cardImageBack.SetActive(false);
                        cardComponent.cardImageFront.SetActive(true);

                        pController2.myOwnedCards.Add(cardComponent);

                        if (pController2.actionPhase == true && pController2.actionsAvailable > 0)
                        {
                            pController2.actionsAvailable -= 1;
                        }

                        pController2.powerChosen = "";
                        pController2.canClickButtons = true;

                    }
                }

                return;
            }
        }

        //playerContr.canClickButtons = true;
    }


    public void ClickOwnedPanelCardJoker(Card card, PlayerController playerContr)
    {

        for (int j = 0; j < playerContr.availablePlacedCardSlots.Length; j++)
        {
            if (playerContr.availablePlacedCardSlots[j] == true)
            {
                view.RPC("CardOwnedToPlacedJoker", RpcTarget.All, card.cardOwnedSlot, j);

                return;
            }
            else if (j == playerContr.availablePlacedCardSlots.Length - 1)
            {
                StartCoroutine(FadeTextToZeroAlpha(2f, "Beach is full"));
                return;
            }
        }
    }

    public void CardsFaceDown()
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            PlayerController playerController = players[i].GetComponent<PlayerController>();

            if (!playerController.view.IsMine && playerController.view.ViewID != this.view.ViewID)
            {
                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;

                childPOwnedPanel.transform.GetChild(18).gameObject.SetActive(false);

                int childCardCount = childPOwnedPanel.transform.childCount;

                for (int j = 0; j < childCardCount; j++)
                {
                    GameObject tempObject = childPOwnedPanel.transform.GetChild(j).gameObject;
                    if (tempObject.GetComponent<Card>() != null)
                    {
                        if (childPOwnedPanel != this.panelOwnedCards)
                        {
                            GameObject childWeWant = childPOwnedPanel.transform.GetChild(j).gameObject;
                            Card cardChildWeWant = childWeWant.GetComponent<Card>();

                            cardChildWeWant.cardImageFront.SetActive(false);
                            cardChildWeWant.cardImageShader.SetActive(false);
                            cardChildWeWant.cardImageBack.SetActive(true);

                            if(cardChildWeWant.value == 1000)
                            {
                                cardChildWeWant.buttonOceanya.SetActive(false);
                            }
                            else if(cardChildWeWant.value == 2000)
                            {
                                cardChildWeWant.buttonPolutrum.SetActive(false);
                            }
                            else if(cardChildWeWant.value == 3000)
                            {
                                cardChildWeWant.buttonXRay.SetActive(false);
                            }

                        }
                    }
                    /* else
                    {
                        continue;
                    } */
                }

            }
        }
    }

    [PunRPC]
    public void CardOwnedToPlacedJoker(int cardIndex, int placedCardSlotsInt)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().powerChosen == "JokerCardPower")
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();

                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;
                GameObject childPlacedPanel = child.transform.GetChild(3).gameObject;

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPOwnedPanel.transform.childCount; n++)
                {
                    if (childPOwnedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPOwnedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardOwnedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }

                Card cardComponent = childCardWeWant.GetComponent<Card>();

                //Card

                childCardWeWant.transform.parent = childPlacedPanel.transform;
                childCardWeWant.transform.position = playerController.placedCardSlots[placedCardSlotsInt].transform.position;

                cardComponent.cardPlacedSlot = placedCardSlotsInt;

                cardComponent.cardImageFront.SetActive(false);
                cardComponent.cardImageBack.SetActive(true);

                //NAO SEI SE O JOKER CONTA
                cardComponent.isJoker = true;


                //Player
                playerController.availableCardSlots[cardComponent.cardOwnedSlot] = true;
                playerController.availablePlacedCardSlots[placedCardSlotsInt] = false;

                playerController.myPlacedCards.Add(cardComponent);

                playerController.powerChosen = "";
                playerController.canClickButtons = true;

                playerController.amountOfCardsPlaced += 1;

                return;
            }
        }


    }

    [PunRPC]
    public void SetParentByStringPoluicao(string parentName, int availableMidSlot, int random)
    {

        GameObject[] findCards = GameObject.FindGameObjectsWithTag("Card");

        for (int i = 0; i < findCards.Length; i++)
        {
            Transform nameGameObject = findCards[i].GetComponent<Transform>();
            Card u = findCards[i].GetComponent<Card>();

            if (u.onMidDeck == true && nameGameObject.parent == null) { //u.onMidDeck == true && 

                Debug.Log("Card: " + findCards[i].name);
                Card c = findCards[i].GetComponent<Card>();
                //Transform nameGameObject = findCards[i].GetComponent<Transform>();

                findCards[i].GetComponent<Card>().cardMidSlot = availableMidSlot;
                //findCards[i].transform.SetParent(GameObject.Find(parentName).transform);

                findCards[i].transform.SetParent(boardGamePanel.transform.GetChild(9));

                findCards[i].transform.localScale = new Vector3(1, 1, 1);

                findCards[i].transform.position = gameManager.middleCardSlots[availableMidSlot].transform.position;

                c.gameObject.SetActive(true);

                gameManager.availableMidSlots[availableMidSlot] = false;

                gameManager.deckMiddleOcean.Add(c);

                gameManager.deckPoluicao.RemoveAt(random);

            }
            else
            {
                continue;
            }

        }
    }

    [PunRPC]
    public void SetParentByStringAmbiente(string parentName, int availableMidSlot, int random)
    {

        GameObject[] findCards = GameObject.FindGameObjectsWithTag("Card");

        for (int i = 0; i < findCards.Length; i++)
        {
            Transform nameGameObject = findCards[i].GetComponent<Transform>();
            Card u = findCards[i].GetComponent<Card>();

            if (u.onMidDeck == true && nameGameObject.parent == null) { //u.onMidDeck == true && 

                Debug.Log("Card: " + findCards[i].name);
                Card c = findCards[i].GetComponent<Card>();
                //Transform nameGameObject = findCards[i].GetComponent<Transform>();

                findCards[i].GetComponent<Card>().cardMidSlot = availableMidSlot;
                //findCards[i].transform.SetParent(GameObject.Find(parentName).transform);

                findCards[i].transform.SetParent(boardGamePanel.transform.GetChild(9));

                findCards[i].transform.localScale = new Vector3(1, 1, 1);

                findCards[i].transform.position = gameManager.middleCardSlots[availableMidSlot].transform.position;

                c.gameObject.SetActive(true);

                gameManager.availableMidSlots[availableMidSlot] = false;

                //GameObject.Find(middleStrings[i]).transform.SetParent(GameObject.Find(parentName).transform);
                //deckMiddleOcean.Add(GameObject.Find( findCards[i]).GetComponent<Card>());

                gameManager.deckMiddleOcean.Add(c);

                gameManager.deckAmbiente.RemoveAt(random);

            }
            else
            {
                continue;
            }

        }

        //cardsInstantiated.Clear();
    }

    [PunRPC]
    public void RemoveMiddleCard(int cardIndex, int j, int playerViewID)
    {
        //Preciso de adicionar esta carta nos outros, ao inventário do player certo

        Card[] midCards = GameObject.FindObjectsOfType<Card>(true);

        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < midCards.Length; i++)
        {

            Card c = midCards[i].GetComponent<Card>();
            //Debug.Log("c.CardMidSlot: " + c.cardMidSlot + " cardIndex: " + cardIndex);

            if (c != null && c.cardMidSlot == cardIndex && c.onMidDeck)
            {

                for (int h = 0; h < players.Length; h++)
                {

                    if (players[h].GetComponent<PhotonView>().ViewID == playerViewID)
                    {
                        GameObject child = players[h].transform.GetChild(0).gameObject;
                        GameObject childOwnedPanel = child.transform.GetChild(2).gameObject;
                        PlayerController playerController = players[h].GetComponent<PlayerController>();

                        //Debug.Log("Controller ID: " + playerController.view.ViewID + " ID: " + players[h].GetComponent<PhotonView>().ViewID);

                        c.gameObject.transform.parent = childOwnedPanel.transform;
                        c.gameObject.transform.position = playerController.cardSlots[j].transform.position;
                        c.cardOwnedSlot = j;
                        c.cardMidSlot = -5;
                        c.onMidDeck = false;

                        playerController.listOfCardsIndex.Clear();
                        playerController.listOfCardsPower.Clear();

                        playerController.availableCardSlots[j] = false;

                        gameManager.deckMiddleOcean.Remove(c);

                        gameManager.availableMidSlots[cardIndex] = true;

                        c.cardImageShader.SetActive(false);

                        //i = 0;

                        Debug.Log("----------------------");
                        return;
                    }
                }

            }
        }
    }

    //fiquei na parte de adicionar o controller
    public void OnClickMiddleCard(Card card, PlayerController playerController)
    {
        if ((view.IsMine == true && canGetCards == true && canPlay && cardsPerRound < 2 && powerHasBeenChosen) || (view.IsMine && actionPhase && actionsAvailable > 0))
        {
            if (card.gameObject.transform.parent.name == "MiddleOceanPanel")
            {
                int a = 0;

                for (int j = 0; j < availableCardSlots.Length; j++)
                {
                    if (availableCardSlots[j] == true)
                    {
                        int playerViewID = playerController.view.ViewID;

                        view.RPC("RemoveMiddleCard", RpcTarget.All, card.cardMidSlot, j, playerViewID);

                        myOwnedCards.Add(card);


                        if (actionPhase == true)
                        {
                            actionsAvailable--;
                            view.RPC("UpdateActionsPlayer", RpcTarget.Others, this.view.ViewID);
                        }

                        if (cardsPerRound < 2 && actionPhase == false)
                        {
                            cardsPerRound++;

                            if (cardsPerRound == 2)
                            {
                                actionPhase = true;
                                StartCoroutine(gameManager.FadeTextToZeroAlpha(2f, "Action phase!"));
                                view.RPC("UpdateActions", RpcTarget.Others, this.view.ViewID);
                            }
                        }

                        return;
                    }
                    else
                    {
                        a++;
                    }
                }

                if (a == 15 && cardsPerRound < 2)
                {
                    cardsPerRound = 2;
                    actionPhase = true;
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(2f, "Action phase!"));
                    view.RPC("UpdateActions", RpcTarget.Others, this.view.ViewID);
                }
                else if (a == 15)
                {
                    StartCoroutine(gameManager.FadeTextToZeroAlpha(5f, "Your deck is full"));
                }
            }
        }
    }

    public void UpdateActionText()
    {
        for (int i = 0; i < gameManager.playerControllersInGame.Length; i++)
        {
            if (gameManager.playerControllersInGame[i].actionPhase == true)
            {
                gameManager.playerControllersInGame[i].actionText.text = ("Actions: " + gameManager.playerControllersInGame[i].actionsAvailable);
                if (gameManager.playerControllersInGame[i].polutionPoints < 0)
                {
                    gameManager.playerControllersInGame[i].polutionPoints = 0;
                }
            }
            else
            {
                gameManager.playerControllersInGame[i].actionText.text = "Actions: 0";

                if (gameManager.playerControllersInGame[i].polutionPoints < 0)
                {
                    gameManager.playerControllersInGame[i].polutionPoints = 0;
                }
            }
        }
    }

    public PlayerController GetPlayerByID(int viewValue)
    {
        for (int i = 0; i < gameManager.playerControllersInGame.Length; i++)
        {
            if (gameManager.playerControllersInGame[i].view.ViewID == viewValue)
            {
                return gameManager.playerControllersInGame[i];
            }
        }
        return null;
    }

    [PunRPC]
    public void UpdateActions(int viewValue)
    {
        PlayerController a = GetPlayerByID(viewValue);
        a.cardsPerRound = 2;
        a.actionPhase = true;
    }

    [PunRPC]
    public void UpdateMidAction(int viewValue)
    {
        PlayerController a = GetPlayerByID(viewValue);
        a.actionsAvailable--;
        a.midAction = false;
    }

    [PunRPC]
    public void UpdateActionsPlayer(int viewValue)
    {
        PlayerController a = GetPlayerByID(viewValue);
        a.actionsAvailable--;
    }

    [PunRPC]
    public void ClickedPlaceCardsToPlace(int viewValue)
    {
        PlayerController a = GetPlayerByID(viewValue);
        a.hasClickedPlaceCards = true;
        //a.buttonPlaceCardsText.text = "Finish Action";
    }

    [PunRPC]
    public void ClickedPlaceCardsToNotPlace(int viewValue)
    {
        PlayerController a = GetPlayerByID(viewValue);
        //a.buttonPlaceCardsText.text = "Place cards";
        a.actionsAvailable--;
        a.hasClickedPlaceCards = false;

    }

    [PunRPC]
    public void UpdatePlayers(int viewValue, string stringValue)
    {
        PlayerController a = GetPlayerByID(viewValue);

        Power power = FindObjectOfType<Power>();

        a.powerHasBeenChosen = true;
        a.canChoosePower = false;
        a.canGetCards = true;
        a.powerChosen = stringValue;
        a.previousPower = stringValue;

        if (a.polutionPoints < 6)
            a.actionsAvailable = 2;
        else
            a.actionsAvailable = 1;


        if (stringValue == "FullDefensePower")
        {
            a.fullDefense = true;

            for (int i = 0; i < power.spotsAvailableFullDefense.Length; i++)
            {
                if (power.spotsAvailableFullDefense[i] == true)
                {
                    a.powerImage.transform.position = gameManager.fullDefenseSpots[i].transform.position;
                    return;
                }
            }

        }
        else if (stringValue == "TripleActionPower")
        {
            a.actionsAvailable += 1;

            for (int i = 0; i < power.spotsAvailableTripleAction.Length; i++)
            {
                if (power.spotsAvailableTripleAction[i] == true)
                {
                    a.powerImage.transform.position = gameManager.extraActionSpots[i].transform.position;
                    return;
                }
            }

        }
        else if (stringValue == "JokerCardPower")
        {
            for (int i = 0; i < power.spotsAvailableJoker.Length; i++)
            {
                if (power.spotsAvailableJoker[i] == true)
                {
                    a.powerImage.transform.position = gameManager.jokerSpots[i].transform.position;
                    return;
                }
            }
        }
        else if (stringValue == "StealCardPower")
        {
            for (int i = 0; i < power.spotsAvailableStealCard.Length; i++)
            {
                if (power.spotsAvailableStealCard[i] == true)
                {
                    a.powerImage.transform.position = gameManager.stealCardSpots[i].transform.position;
                    return;
                }
            }
        }

    }

    [PunRPC]
    public void UpdatePlayerPolution(int viewID)
    {
        PlayerController a = GetPlayerByID(viewID);
        a.polutionPoints += 1;
    }

    public void OnClickOpenButtons()
    {
        if (canClickButtons)
        {
            //panelOwnedCards.SetActive(false);
            //panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);

            if (buttonObjectOwnedCards.activeSelf == true)
            {
                buttonObjectOwnedCards.SetActive(false);
                buttonObjectPlaceCards.SetActive(false);
            }
            else
            {
                buttonObjectOwnedCards.SetActive(true);
                buttonObjectPlaceCards.SetActive(true);
            }
        }
    }

    public void OnClickOpenActionButtons()
    {
        if (actionPhase == true && actionsAvailable > 0)
        {
            if (allActionButtons.activeSelf == true)
            {
                allActionButtons.SetActive(false);
            }
            else
            {
                allActionButtons.SetActive(true);
            }
        }
    }

    public void OnClickEndActionButtons()
    {
        if (midAction)
        {
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);
            view.RPC("UpdateMidAction", RpcTarget.All, this.view.ViewID);
        }
        else if (!midAction)
        {
            allActionButtons.SetActive(false);
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);
        }
    }

    public void OnClickOpenOwnedPanel()
    {
        if (panelOwnedCards.activeSelf == false && canClickButtons)
        {
            if (listOfCardsPower.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    c.cardImageShader.SetActive(false);
                }
            }

            listOfCardsPower.Clear();
            listOfCardsIndex.Clear();
            Array.Clear(arrayOfCardIndex, 0, arrayOfCardIndex.Length);

            panelOwnedCards.SetActive(true);
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(false);
        }
    }

    public void OnClickCloseOwnedPanel()
    {
        if (canClickButtons && panelOwnedCards.activeSelf == true)
        {
            if (listOfCardsPower.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    c.cardImageShader.SetActive(false);
                }
            }

            listOfCardsPower.Clear();
            listOfCardsIndex.Clear();
            Array.Clear(arrayOfCardIndex, 0, arrayOfCardIndex.Length);

            panelOwnedCards.SetActive(false);
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);

            actionName = "";
            wantToPollute = false;


            /*if (hasClickedPlaceCards)
            {
                canClickButtons = true;
                this.actionsAvailable -= 1;
                buttonPlaceCardsText.text = "Place cards";
                view.RPC("ClickedPlaceCardsToNotPlace", RpcTarget.Others, this.view.ViewID);
                hasClickedPlaceCards = false;
            } */

        }
    }

    public void OnClickClosePlacePanel()
    {
        if (canClickButtons && panelPlaceCards.activeSelf == true)
        {
            if (listOfCardsPower.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    c.cardImageShader.SetActive(false);
                }
            }

            listOfCardsPower.Clear();
            listOfCardsIndex.Clear();
            Array.Clear(arrayOfCardIndex, 0, arrayOfCardIndex.Length);

            panelOwnedCards.SetActive(false);
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);
        }
    }

    public void OnClickOpenPlacePanel()
    {
        if (panelPlaceCards.activeSelf == false && canClickButtons && !hasClickedPlaceCards)
        {
            if (listOfCardsPower.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    c.cardImageShader.SetActive(false);
                }
            }

            listOfCardsPower.Clear();
            listOfCardsIndex.Clear();
            Array.Clear(arrayOfCardIndex, 0, arrayOfCardIndex.Length);

            gameManager.panelMiddleCards.SetActive(false);
            panelOwnedCards.SetActive(false);
            panelPlaceCards.SetActive(true);

        }
    }

    // Update is called once per frame
    void Update()
    {
        if (!view.IsMine)
            return;

        //gameManager.timerText.text = TimeSpan.FromSeconds(((int)timeLeft)).ToString();

        if (gameManager.playerControllersInGame.Length == PhotonNetwork.PlayerList.Length && view.IsMine)
        {
            for (int i = 0; i < gameManager.playerControllersInGame.Length; i++)
            {
                if (gameManager.playerControllersInGame[i].canPlay)
                {
                    PlayerController a = gameManager.playerControllersInGame[i];

                    if(i == 0)
                    {
                        gameManager.sliderColor.color = Color.red;

                        if(gameManager.playerControllersInGame.Length > 1)
                        {
                            gameManager.sliderColorNext.color = Color.blue;
                        }
                        else
                        {
                            gameManager.sliderColorNext.color = Color.red;
                        }

                    }
                    else if(i == 1)
                    {
                        gameManager.sliderColor.color = Color.blue;

                        if (gameManager.playerControllersInGame.Length > 2)
                        {
                            gameManager.sliderColorNext.color = Color.yellow;
                        }
                        else
                        {
                            gameManager.sliderColorNext.color = Color.red;
                        }

                    }
                    else if(i == 2)
                    {
                        gameManager.sliderColor.color = Color.yellow;
                        if (gameManager.playerControllersInGame.Length > 3)
                        {
                            gameManager.sliderColorNext.color = Color.green;
                        }
                        else
                        {
                            gameManager.sliderColorNext.color = Color.red;
                        }

                    }
                    else if(i == 3)
                    {
                        gameManager.sliderColor.color = Color.green;
                        gameManager.sliderColorNext.color = Color.red;
                    }

                    timeLeft -= Time.deltaTime;
                    gameManager.sliderTime.maxValue = gameManager.timerToPlay;
                    gameManager.sliderTime.value = timeLeft;
                    gameManager.sliderImage.sprite = a.powerImage.sprite;

                    if (timeLeft < 0)
                    {
                        timeLeft = 0;
                        a.actionsAvailable = 0;
                    }
                }
            }

            if(panelOwnedCards.activeSelf == true && listOfCardsIndex.Count > 0)
            {
                buttonPlaceCards.SetActive(true);
                pulliteOthersButton.SetActive(true);
            }
            else
            {
                buttonPlaceCards.SetActive(false);
                pulliteOthersButton.SetActive(false);
            }
        }

        GameOverFunction();

    }

    public void GameOverFunction()
    {
        int gameOverCount = 0;

        foreach (PlayerController playerC in gameManager.playerControllersInGame)
        {
            if (playerC.gameOver == true)
            {
                gameOverCount++;
            }
        }

        if (view.IsMine)
        {
            if (gameOverCount == gameManager.playerControllersInGame.Length && gameManager.playerControllersInGame.Length == PhotonNetwork.PlayerList.Length)
            {
                StopAllCoroutines();
                backgroundMusic.Stop();

                if (PhotonNetwork.IsMasterClient && PhotonNetwork.PlayerList.Length == 1)
                {
                    if (isWinner == true)
                    {
                        SceneManager.LoadScene("WinScene");
                    }
                    else
                    {
                        SceneManager.LoadScene("LoseScene");
                    }
                }
                else
                {
                    if (isWinner == true)
                    {
                        SceneManager.LoadScene("WinScene");
                    }
                    else
                    {
                        SceneManager.LoadScene("LoseScene");
                    }
                }
            }

        }
    }

    public int CompareTo(object obj)
    {
        if (obj == null) return 1;

        PlayerController otherPlayer = obj as PlayerController;
        if (otherPlayer != null)
            return this.view.ViewID.CompareTo(otherPlayer.view.ViewID);
        else
            throw new NotImplementedException();
    }


    public void OnClickNegativeOnEnemy()
    {
        if (this.view.IsMine && actionPhase == true && actionsAvailable > 0 && gameManager.playerControllersInGame.Length > 1 && !hasClickedPlaceCards && !wantToPollute && polutionPoints >= 2 && polutionPoints < 7 && listOfCardsPower.Count == 1)
        {
            if(listOfCardsPower[0].value < 0)
            {
                actionName = "NegativeOnEnemy";
                wantToPollute = true;

                panelOwnedCards.SetActive(false);
                panelPlaceCards.SetActive(false);
                gameManager.panelMiddleCards.SetActive(true);
                stealCardPowerObject.SetActive(true);
                closeStealPanelButton.SetActive(true);

                StealCardPower();
            }
            else
            {
                StartCoroutine(FadeTextToZeroAlpha(1f, "Can't do that"));
            }

        }

        else if (gameManager.playerControllersInGame.Length < 2 || listOfCardsPower.Count > 1)
        {

            if(listOfCardsPower.Count > 0)
            {
                foreach(Card c in listOfCardsPower)
                {
                    c.cardImageShader.SetActive(false);
                }

                listOfCardsPower.Clear();
                listOfCardsIndex.Clear();
            }

            StartCoroutine(FadeTextToZeroAlpha(1f, "Can't do that"));
        }
        else if (polutionPoints < 2 || polutionPoints == 7)
        {

            if (listOfCardsPower.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    c.cardImageShader.SetActive(false);
                }

                listOfCardsPower.Clear();
                listOfCardsIndex.Clear();
            }

            StartCoroutine(FadeTextToZeroAlpha(1f, "Can't do that"));
        }
    }

    public void LateUpdate()
    {
        if (doOnceButtons == false && gameManager.playerControllersInGame.Length == PhotonNetwork.PlayerList.Length && PhotonNetwork.LocalPlayer.IsLocal)
        {
            //view.RPC("UpdateNamesForDecks", RpcTarget.All, namePlayer);

            for (int i = 0; i < gameManager.playerControllersInGame.Length; i++)
            {

                Transform childOpenButtons, childCardOwned, childCardsPlaced, imageOwnedPanel, imagePlacedPanel;

                childOpenButtons = gameManager.playerControllersInGame[i].allButtons.transform.GetChild(0);
                childOpenButtons.transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName + " decks";

                childCardOwned = gameManager.playerControllersInGame[i].allButtons.transform.GetChild(1);
                childCardOwned.transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName + " cards";

                childCardsPlaced = gameManager.playerControllersInGame[i].allButtons.transform.GetChild(2);
                childCardsPlaced.transform.GetChild(0).GetComponent<Text>().text = PhotonNetwork.PlayerList[i].NickName + " beach";

                imageOwnedPanel = gameManager.playerControllersInGame[i].panelOwnedCards.transform.GetChild(0);
                imageOwnedPanel.GetComponent<Image>().sprite = gameManager.playerControllersInGame[i].powerImage.sprite;

                imagePlacedPanel = gameManager.playerControllersInGame[i].panelPlaceCards.transform.GetChild(0);
                imagePlacedPanel.GetComponent<Image>().sprite = gameManager.playerControllersInGame[i].powerImage.sprite;

                gameManager.playerControllersInGame[i].imageColor.GetComponent<Image>().sprite = gameManager.playerControllersInGame[i].powerImage.sprite;

                if (!gameManager.playerControllersInGame[i].view.IsMine)
                {
                    gameManager.playerControllersInGame[i].myActionsButton.SetActive(false);
                    gameManager.playerControllersInGame[i].buttonPlaceCards.SetActive(false);
                    gameManager.playerControllersInGame[i].pulliteOthersButton.SetActive(false);
                }

                gameManager.playerControllersInGame[i].closeStealPanelButton = gameManager.gameObject.transform.GetChild(2).transform.GetChild(7).gameObject;

                Debug.Log(gameManager.playerControllersInGame[i].namePlayer);

            }
        }

        //NEED THE 3RD AND 4TH PLAYER ASWELL

        if (!doOnceButtons && gameManager.playerControllersInGame.Length == PhotonNetwork.PlayerList.Length && PhotonNetwork.LocalPlayer.IsLocal)
        {
            for (int i = 0; i < gameManager.playerControllersInGame.Length; i++)
            {
                GameObject panelFirst = gameManager.transform.GetChild(2).gameObject;

                gameManager.playerControllersInGame[i].stealFirst = panelFirst.transform.GetChild(0).gameObject.GetComponent<StealFirstPlayer>();
                gameManager.playerControllersInGame[i].stealSecond = panelFirst.transform.GetChild(1).gameObject.GetComponent<StealSecondPlayer>();
                gameManager.playerControllersInGame[i].stealThird = panelFirst.transform.GetChild(2).gameObject.GetComponent<StealThirdPlayer>();

                gameManager.playerControllersInGame[i].allButtons.SetActive(true);

                for (int j = 0; j < gameManager.playerControllersInGame.Length; j++)
                {
                    gameManager.playerControllersInGame[i].playerObjects[j] = gameManager.playerControllersInGame[j].myGameObject;
                }

                gameManager.playerControllersInGame[i].doOnceButtons = true;
            }

            //alone I cant steal I guess
            if (gameManager.playerControllersInGame.Length == 1)
            {
                stealFirst.playerPlaying = this;
                gameManager.textStealPlayer1.text = "Non-Existant";
                gameManager.textStealPlayer2.text = "Non-Existant";
                gameManager.textStealPlayer3.text = "Non-Existant";
            }
            //2players
            else if (gameManager.playerControllersInGame.Length == 2)
            {

                if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[0].view.ViewID)
                {
                    Debug.Log("Here");
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[1];
                    stealFirst.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[1].NickName;
                    gameManager.textStealPlayer2.text = "Non-Existant";
                    gameManager.textStealPlayer3.text = "Non-Existant";

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[1].powerImage.sprite;

                    GameObject panelFirst = gameManager.transform.GetChild(2).gameObject;
                    panelFirst.transform.GetChild(1).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(2).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(5).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(6).gameObject.SetActive(false);

                }
                else if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[1].view.ViewID)
                {
                    Debug.Log("Wtf");
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[0];
                    stealFirst.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[0].NickName;
                    gameManager.textStealPlayer2.text = "Non-Existant";
                    gameManager.textStealPlayer3.text = "Non-Existant";

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[0].powerImage.sprite;

                    GameObject panelFirst = gameManager.transform.GetChild(2).gameObject;
                    panelFirst.transform.GetChild(1).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(2).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(5).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(6).gameObject.SetActive(false);
                }

            }
            //3players
            else if (gameManager.playerControllersInGame.Length == 3)
            {
                if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[0].view.ViewID)
                {
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[1];
                    stealFirst.playerPlaying = this;
                    stealSecond.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[2];
                    stealSecond.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[1].NickName;
                    gameManager.textStealPlayer2.text = "" + PhotonNetwork.PlayerList[2].NickName;
                    gameManager.textStealPlayer3.text = "Non-Existant";

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[1].powerImage.sprite;
                    one.transform.GetChild(5).GetComponent<Image>().sprite = gameManager.playerControllersInGame[2].powerImage.sprite;

                    GameObject panelFirst = gameManager.transform.GetChild(2).gameObject;
                    panelFirst.transform.GetChild(2).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(6).gameObject.SetActive(false);

                }
                else if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[1].view.ViewID)
                {
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[0];
                    stealFirst.playerPlaying = this;
                    stealSecond.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[2];
                    stealSecond.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[0].NickName;
                    gameManager.textStealPlayer2.text = "" + PhotonNetwork.PlayerList[2].NickName;
                    gameManager.textStealPlayer3.text = "Non-Existant";

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[0].powerImage.sprite;
                    one.transform.GetChild(5).GetComponent<Image>().sprite = gameManager.playerControllersInGame[2].powerImage.sprite;

                    GameObject panelFirst = gameManager.transform.GetChild(2).gameObject;
                    panelFirst.transform.GetChild(2).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(6).gameObject.SetActive(false);

                }
                else if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[2].view.ViewID)
                {
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[0];
                    stealFirst.playerPlaying = this;
                    stealSecond.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[1];
                    stealSecond.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[0].NickName;
                    gameManager.textStealPlayer2.text = "" + PhotonNetwork.PlayerList[1].NickName;
                    gameManager.textStealPlayer3.text = "Non-Existant";

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[0].powerImage.sprite;
                    one.transform.GetChild(5).GetComponent<Image>().sprite = gameManager.playerControllersInGame[1].powerImage.sprite;

                    GameObject panelFirst = gameManager.transform.GetChild(2).gameObject;
                    panelFirst.transform.GetChild(2).gameObject.SetActive(false);
                    panelFirst.transform.GetChild(6).gameObject.SetActive(false);
                }

            }
            //4players
            else if (gameManager.playerControllersInGame.Length == 4)
            {
                if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[0].view.ViewID)
                {

                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[1];
                    stealSecond.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[2];
                    stealThird.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[3];
                    stealFirst.playerPlaying = this;
                    stealSecond.playerPlaying = this;
                    stealThird.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[1].NickName;
                    gameManager.textStealPlayer2.text = "" + PhotonNetwork.PlayerList[2].NickName;
                    gameManager.textStealPlayer3.text = "" + PhotonNetwork.PlayerList[3].NickName;

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[1].powerImage.sprite;
                    one.transform.GetChild(5).GetComponent<Image>().sprite = gameManager.playerControllersInGame[2].powerImage.sprite;
                    one.transform.GetChild(6).GetComponent<Image>().sprite = gameManager.playerControllersInGame[3].powerImage.sprite;

                }
                else if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[1].view.ViewID)
                {
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[0];
                    stealSecond.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[2];
                    stealThird.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[3];
                    stealFirst.playerPlaying = this;
                    stealSecond.playerPlaying = this;
                    stealThird.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[0].NickName;
                    gameManager.textStealPlayer2.text = "" + PhotonNetwork.PlayerList[2].NickName;
                    gameManager.textStealPlayer3.text = "" + PhotonNetwork.PlayerList[3].NickName;

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[0].powerImage.sprite;
                    one.transform.GetChild(5).GetComponent<Image>().sprite = gameManager.playerControllersInGame[2].powerImage.sprite;
                    one.transform.GetChild(6).GetComponent<Image>().sprite = gameManager.playerControllersInGame[3].powerImage.sprite;

                }
                else if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[2].view.ViewID)
                {
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[0];
                    stealSecond.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[1];
                    stealThird.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[3];
                    stealFirst.playerPlaying = this;
                    stealSecond.playerPlaying = this;
                    stealThird.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[0].NickName;
                    gameManager.textStealPlayer2.text = "" + PhotonNetwork.PlayerList[1].NickName;
                    gameManager.textStealPlayer3.text = "" + PhotonNetwork.PlayerList[3].NickName;

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[0].powerImage.sprite;
                    one.transform.GetChild(5).GetComponent<Image>().sprite = gameManager.playerControllersInGame[1].powerImage.sprite;
                    one.transform.GetChild(6).GetComponent<Image>().sprite = gameManager.playerControllersInGame[3].powerImage.sprite;

                }
                else if (view.IsMine && this.view.ViewID == gameManager.playerControllersInGame[3].view.ViewID)
                {
                    stealFirst.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[0];
                    stealSecond.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[1];
                    stealThird.playerContrObject = gameManager.playerControllersInGame[0].playerObjects[2];
                    stealFirst.playerPlaying = this;
                    stealSecond.playerPlaying = this;
                    stealThird.playerPlaying = this;

                    gameManager.textStealPlayer1.text = "" + PhotonNetwork.PlayerList[0].NickName;
                    gameManager.textStealPlayer2.text = "" + PhotonNetwork.PlayerList[1].NickName;
                    gameManager.textStealPlayer3.text = "" + PhotonNetwork.PlayerList[2].NickName;

                    Transform one = gameManager.transform.GetChild(2);
                    one.transform.GetChild(4).GetComponent<Image>().sprite = gameManager.playerControllersInGame[0].powerImage.sprite;
                    one.transform.GetChild(5).GetComponent<Image>().sprite = gameManager.playerControllersInGame[1].powerImage.sprite;
                    one.transform.GetChild(6).GetComponent<Image>().sprite = gameManager.playerControllersInGame[2].powerImage.sprite;
                }
            }

            stealCardPowerObject.SetActive(false);

        }

        /* for (int g = 0; g < gameManager.playerControllersInGame.Length; g++)
        {
            if (PhotonNetwork.PlayerList.Length == gameManager.playerControllersInGame.Length && PhotonNetwork.PlayerList[g].IsInactive)
            {
                view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[g].view.ViewID);
            }
        } */

        if (doOnceButtons && PhotonNetwork.PlayerList.Length < gameManager.playerControllersInGame.Length && PhotonNetwork.InRoom)
        {
            //2players
            if (PhotonNetwork.PlayerList.Length == 1)
            {
                if (gameManager.playerControllersInGame[0].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                {
                    view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[1].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                }
                else
                {
                    view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[0].view.ViewID, gameManager.playerControllersInGame[1].view.ViewID);
                }

            }
            //3players
            else if (PhotonNetwork.PlayerList.Length == 2)
            {
                if (gameManager.playerControllersInGame[0].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                {
                    if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[1].NickName)
                    {
                        view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                    }
                    else
                    {
                        view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[1].view.ViewID, gameManager.playerControllersInGame[2].view.ViewID);
                    }
                }
                else if (gameManager.playerControllersInGame[0].namePlayer == PhotonNetwork.PlayerList[1].NickName)
                {
                    if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                    {
                        view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                    }
                    else
                    {
                        view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[1].view.ViewID, gameManager.playerControllersInGame[2].view.ViewID);
                    }
                }
                else
                {
                    view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[0].view.ViewID, gameManager.playerControllersInGame[1].view.ViewID);
                }
            }
            //4players
            else if (PhotonNetwork.PlayerList.Length == 3)
            {
                if (gameManager.playerControllersInGame[0].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                {
                    if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[1].NickName)
                    {
                        if (gameManager.playerControllersInGame[2].namePlayer == PhotonNetwork.PlayerList[2].NickName)
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[3].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                        }
                        else
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[3].view.ViewID);
                        }
                    }
                    else if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[2].NickName)
                    {
                        if (gameManager.playerControllersInGame[2].namePlayer == PhotonNetwork.PlayerList[1].NickName)
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[3].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                        }
                        else
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[3].view.ViewID);
                        }
                    }
                    else
                    {
                        view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[1].view.ViewID, gameManager.playerControllersInGame[2].view.ViewID);
                    }

                }
                else if (gameManager.playerControllersInGame[0].namePlayer == PhotonNetwork.PlayerList[1].NickName)
                {
                    if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                    {
                        if (gameManager.playerControllersInGame[2].namePlayer == PhotonNetwork.PlayerList[2].NickName)
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[3].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                        }
                        else
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[3].view.ViewID);
                        }
                    }
                    else if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[2].NickName)
                    {
                        if (gameManager.playerControllersInGame[2].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[3].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                        }
                        else
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[3].view.ViewID);
                        }
                    }
                    else
                    {
                        view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[1].view.ViewID, gameManager.playerControllersInGame[2].view.ViewID);
                    }

                }
                else if (gameManager.playerControllersInGame[0].namePlayer == PhotonNetwork.PlayerList[2].NickName)
                {
                    if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                    {
                        if (gameManager.playerControllersInGame[2].namePlayer == PhotonNetwork.PlayerList[1].NickName)
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[3].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                        }
                        else
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[3].view.ViewID);
                        }
                    }
                    else if (gameManager.playerControllersInGame[1].namePlayer == PhotonNetwork.PlayerList[1].NickName)
                    {
                        if (gameManager.playerControllersInGame[2].namePlayer == PhotonNetwork.PlayerList[0].NickName)
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[3].view.ViewID, gameManager.playerControllersInGame[0].view.ViewID);
                        }
                        else
                        {
                            view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[2].view.ViewID, gameManager.playerControllersInGame[3].view.ViewID);
                        }
                    }
                    else
                    {
                        view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[1].view.ViewID, gameManager.playerControllersInGame[2].view.ViewID);
                    }

                }
                else
                {
                    view.RPC("UpdateWhenLeavingRoom", RpcTarget.All, gameManager.playerControllersInGame[0].view.ViewID, gameManager.playerControllersInGame[1].view.ViewID);
                }
            }
        }

        //doOnceButtons = true;

        if (!canSeeOtherCards)
            CardsFaceDown();

        UpdateActionText();

    }

    [PunRPC]
    public void PassController(int viewID)
    {
        for (int i = 0; i < gameManager.playerControllersInGame.Length; i++)
        {
            if (gameManager.playerControllersInGame[i].view.ViewID == viewID)
            {
                playerControllerToSteal = gameManager.playerControllersInGame[i];
            }
        }

    }

    public void FunctionUseCard(int viewID, int cardIndex, string stringValue)
    {
        if (stringValue == "Oceanya" && actionPhase)
        {
            view.RPC("UsedOceanya", RpcTarget.All, viewID, cardIndex);
            panelOwnedCards.SetActive(false);
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);
        }
        else if (stringValue == "Polutrum" && actionPhase)
        {
            view.RPC("UsedPolutrum", RpcTarget.All, viewID, cardIndex);
            panelOwnedCards.SetActive(false);
            panelPlaceCards.SetActive(false);
            gameManager.panelMiddleCards.SetActive(true);
        }
        else if (stringValue == "XRay" && actionPhase && gameManager.playerControllersInGame.Length > 1)
        {
            view.RPC("UsedXRay", RpcTarget.All, viewID, cardIndex);
        }
        else if (stringValue != null)
        {
            StartCoroutine(FadeTextToZeroAlpha(2f, "Not on action phase"));
        }
    }

    [PunRPC]
    public void UsedOceanya(int viewID, int cardIndex)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().view.ViewID == viewID)
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();

                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPOwnedPanel.transform.childCount; n++)
                {
                    if (childPOwnedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPOwnedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardOwnedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }

                Card cardComponent = childCardWeWant.GetComponent<Card>();

                //Card
                childCardWeWant.transform.parent = gameManager.playedCardsSpot.transform;
                childCardWeWant.transform.position = gameManager.playedCardsSpot.transform.position;

                cardComponent.cardImageFront.SetActive(false);
                cardComponent.cardImageBack.SetActive(true);

                cardComponent.buttonOceanya.SetActive(false);
                cardComponent.onMidDeck = false;

                cardComponent.cardMidSlot = -1;

                //Player
                playerController.availableCardSlots[cardComponent.cardOwnedSlot] = true;

                playerController.victoryPoints += 5;

                playerController.actionsAvailable -= 1;

                //shader
                this.gameManager.sereiaTime = 0;
                this.gameManager.sereiaPlayed = true;
                this.gameManager.sereiaObject.SetActive(true);

                StartCoroutine(gameManager.FadeTextToZeroAlpha(2f, PhotonNetwork.PlayerList[i].NickName + " used Oceanya"));

                return;

            }
        }
    }

    [PunRPC]
    public void UsedPolutrum(int viewID, int cardIndex)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().view.ViewID == viewID)
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();

                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPOwnedPanel.transform.childCount; n++)
                {
                    if (childPOwnedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPOwnedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardOwnedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }

                Card cardComponent = childCardWeWant.GetComponent<Card>();

                //Card

                childCardWeWant.transform.parent = gameManager.playedCardsSpot.transform;
                childCardWeWant.transform.position = gameManager.playedCardsSpot.transform.position;

                cardComponent.cardImageFront.SetActive(false);
                cardComponent.cardImageBack.SetActive(true);

                cardComponent.buttonPolutrum.SetActive(false);
                cardComponent.onMidDeck = false;

                cardComponent.cardMidSlot = -2;

                //Player
                playerController.availableCardSlots[cardComponent.cardOwnedSlot] = true;

                playerController.polutionPoints -= 3;

                playerController.actionsAvailable -= 1;

                this.gameManager.sereiaTime = 0;
                this.gameManager.poluitrumPlayed = true;
                this.gameManager.sereiaObject.SetActive(true);

                StartCoroutine(gameManager.FadeTextToZeroAlpha(2f, PhotonNetwork.PlayerList[i].NickName + " used Polutrum"));

                return;

            }
        }

    }

    [PunRPC]
    public void UsedXRay(int viewID, int cardIndex)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().view.ViewID == viewID)
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();

                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPOwnedPanel.transform.childCount; n++)
                {
                    if (childPOwnedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPOwnedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardOwnedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }

                Card cardComponent = childCardWeWant.GetComponent<Card>();

                //Card
                childCardWeWant.transform.parent = gameManager.playedCardsSpot.transform;
                childCardWeWant.transform.position = gameManager.playedCardsSpot.transform.position;

                cardComponent.cardImageFront.SetActive(false);
                cardComponent.cardImageBack.SetActive(true);

                cardComponent.buttonXRay.SetActive(false);
                cardComponent.onMidDeck = false;

                cardComponent.cardMidSlot = -1;

                //Player
                playerController.availableCardSlots[cardComponent.cardOwnedSlot] = true;

                playerController.polutionPoints += 1;

                StartCoroutine(gameManager.FadeTextToZeroAlpha(2f, PhotonNetwork.PlayerList[i].NickName + " used XRay"));

                return;

            }
        }
    }

    [PunRPC]
    public void StealPoints(int viewID)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().view.ViewID == viewID)
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();

                if (playerController.playerControllerToSteal.victoryPoints >= 5)
                {
                    playerController.victoryPoints += 5;
                    playerController.polutionPoints += 1;
                    playerController.playerControllerToSteal.victoryPoints -= 5;
                    playerController.actionsAvailable -= 1;
                    playerController.actionName = "";
                }

                return;
            }
        }
    }

    [PunRPC]
    public void NegativeOnEnemy(int viewID, int viewIDPolluted, int cardIndex)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        int j = 0;

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().view.ViewID == viewIDPolluted)
            {
                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPlacedPanel = child.transform.GetChild(3).gameObject;

                PlayerController playerControllerPoluted = players[i].GetComponent<PlayerController>();

                GameObject childCardWeWant = null;

                for (int u = 0; u < players.Length; u++)
                {
                    if (players[u].GetComponent<PlayerController>().view.ViewID == viewID)
                    {
                        PlayerController pControllerThatIsPolluting = players[u].GetComponent<PlayerController>();

                        GameObject childSecondPlayer = players[u].transform.GetChild(0).gameObject;
                        GameObject childPOwnedPanelSecondPlayer = childSecondPlayer.transform.GetChild(2).gameObject;

                        for (int c = 0; c < playerControllerPoluted.availablePlacedCardSlots.Length; c++)
                        {
                            if (playerControllerPoluted.availablePlacedCardSlots[c] == true)
                            {
                                j = c;
                                Debug.Log("J value: " + j);

                                break;
                            }
                        }

                        for (int n = 0; n < childPOwnedPanelSecondPlayer.transform.childCount; n++)
                        {
                            if (childPOwnedPanelSecondPlayer.transform.GetChild(n).GetComponent<Card>() != null)
                            {
                                Card cardapio = childPOwnedPanelSecondPlayer.transform.GetChild(n).GetComponent<Card>();
                                if (cardapio.cardOwnedSlot == cardIndex)
                                {
                                    childCardWeWant = cardapio.gameObject;
                                    break;
                                }
                            }
                        }

                        Card cardComponent = childCardWeWant.GetComponent<Card>();

                        childCardWeWant.transform.parent = childPlacedPanel.transform;
                        childCardWeWant.transform.position = playerControllerPoluted.placedCardSlots[j].transform.position;

                        pControllerThatIsPolluting.availableCardSlots[cardIndex] = true;
                        playerControllerPoluted.availablePlacedCardSlots[j] = false;

                        cardComponent.cardPlacedSlot = j;
                        cardComponent.cardImageBack.SetActive(false);
                        cardComponent.cardImageFront.SetActive(true);

                        playerControllerPoluted.myPlacedCards.Add(cardComponent);

                        pControllerThatIsPolluting.polutionPoints += 1;
                        pControllerThatIsPolluting.actionsAvailable -= 1;
                        pControllerThatIsPolluting.actionName = "";
                        pControllerThatIsPolluting.canClickButtons = true;
                        pControllerThatIsPolluting.wantToPollute = false;

                    }
                }

                return;
            }
        }
    }

    [PunRPC]
    public void ActionTrio(int viewID, int cardIndex, int powerIncrease, int powerDecrease, int powerTable)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().view.ViewID == viewID)
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();

                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPlacedPanel = child.transform.GetChild(3).gameObject;

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPlacedPanel.transform.childCount; n++)
                {
                    if (childPlacedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPlacedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardPlacedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }

                Card cardComponent = childCardWeWant.GetComponent<Card>();

                if (powerTable == 1)
                {
                    //Card
                    childCardWeWant.transform.parent = gameManager.playedCardsSpot.transform;
                    childCardWeWant.transform.position = gameManager.playedCardsSpot.transform.position;

                    cardComponent.cardImageFront.SetActive(false);
                    cardComponent.cardImageBack.SetActive(true);

                    cardComponent.onMidDeck = false;

                    cardComponent.cardMidSlot = -1;

                    //Player
                    playerController.availablePlacedCardSlots[cardComponent.cardPlacedSlot] = true;

                    playerController.victoryPoints += powerIncrease;

                }
                else if (powerTable == 0)
                {
                    //Card
                    childCardWeWant.transform.parent = gameManager.polutionCardsSpot.transform;
                    childCardWeWant.transform.position = gameManager.polutionCardsSpot.transform.position;

                    cardComponent.cardImageFront.SetActive(false);
                    cardComponent.cardImageBack.SetActive(true);

                    cardComponent.onMidDeck = false;

                    cardComponent.cardMidSlot = -1;

                    //Player
                    playerController.availablePlacedCardSlots[cardComponent.cardPlacedSlot] = true;

                    playerController.polutionPoints += powerDecrease;
                    if (playerController.polutionPoints < 0)
                    {
                        playerController.polutionPoints = 0;
                    }
                }

                return;

            }
        }
    }

    public void ActionTrioActivate(PlayerController playerContr)
    {
        if (playerContr.view.IsMine && playerContr.listOfCardsIndex.Count >= 3)
        {
            int value = 0;
            bool valid = true;
            int powerIncrease = 0;

            foreach (Card c in listOfCardsPower)
            {
                if (c.value > 0 && c.value < 40)
                {
                    value = c.value;
                    break;
                }
                else if (c.value < 0 && c.isJoker == true)
                {
                    continue;
                }
            }

            foreach (Card c in listOfCardsPower)
            {
                if ((value == 4 && c.value == 47) || (value == 7 && c.value == 47) || (value == 7 && c.value == 712) || (value == 12 && c.value == 712))
                {
                    continue;
                }
                else if ((c.value < 0 && c.isJoker == false) || c.value != value && c.isJoker == false)
                {
                    valid = false;
                }
            }

            if (valid)
            {
                arrayOfCardIndex = listOfCardsIndex.ToArray();

                for (int i = 0; i < arrayOfCardIndex.Length; i++)
                {
                    if (i == arrayOfCardIndex.Length - 1)
                    {
                        powerIncrease = value + (listOfCardsPower.Count - 3);
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], powerIncrease, 0, 1);
                        view.RPC("UpdateActionsPlayer", RpcTarget.All, playerContr.view.ViewID);
                    }
                    else
                    {
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], 0, 0, 1);
                    }

                }
                Debug.Log("Value: " + value);
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
            else
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Trio"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
        }
        else if (playerContr.listOfCardsIndex.Count < 3)
        {
            if (playerContr.listOfCardsIndex.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Need atleast 3 cards"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
        }
    }

    public void ActionNegativeTrioActivate(PlayerController playerContr)
    {
        if (playerContr.view.IsMine && playerContr.listOfCardsIndex.Count >= 3)
        {
            int value = 0;
            bool valid = true;
            int powerDecrease = 0;

            foreach (Card c in listOfCardsPower)
            {
                if (c.value < 0 && c.value > -100)
                {
                    value = c.value;
                    break;
                }
                else if (c.isJoker == true)
                {
                    continue;
                }
            }

            foreach (Card c in listOfCardsPower)
            {
                if ((c.value > 0 && c.isJoker == false) || (c.value != value && c.isJoker == false))
                {
                    valid = false;
                }
            }

            if (valid)
            {
                arrayOfCardIndex = listOfCardsIndex.ToArray();

                for (int i = 0; i < arrayOfCardIndex.Length; i++)
                {
                    if (i == arrayOfCardIndex.Length - 1)
                    {
                        powerDecrease = value - (listOfCardsPower.Count - 3);
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], 0, powerDecrease, 0);
                        view.RPC("UpdateActionsPlayer", RpcTarget.All, playerContr.view.ViewID);
                    }
                    else
                    {
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], 0, 0, 0);
                    }

                }
                Debug.Log("Value: " + value);
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
            else
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Trio"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
        }
        else if (playerContr.listOfCardsIndex.Count < 3)
        {
            if (playerContr.listOfCardsIndex.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Need atleast 3 cards"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
        }
    }

    public void ActionSoloCard(PlayerController playerContr)
    {
        if (playerContr.view.IsMine && playerContr.listOfCardsIndex.Count >= 1)
        {
            int value = 0;
            bool valid = true;
            int powerIncrease = 0;

            foreach (Card c in listOfCardsPower)
            {
                value++;
            }

            foreach (Card c in listOfCardsPower)
            {
                if (c.value == 7 || c.value == 12)
                {
                    continue;
                }
                else
                {
                    valid = false;
                }
            }

            if (valid)
            {
                arrayOfCardIndex = listOfCardsIndex.ToArray();

                for (int i = 0; i < arrayOfCardIndex.Length; i++)
                {
                    if (i == arrayOfCardIndex.Length - 1)
                    {
                        powerIncrease = value;
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], powerIncrease, 0, 1);
                        view.RPC("UpdateActionsPlayer", RpcTarget.All, playerContr.view.ViewID);
                    }
                    else
                    {
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], 0, 0, 1);
                    }

                }
                Debug.Log("Value: " + value);
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
            else
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Solo Cards"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
        }
        else if (playerContr.listOfCardsIndex.Count < 1)
        {
            if (playerContr.listOfCardsIndex.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Need atleast 1 card"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
        }
    }

    public void ActionSequence(PlayerController playerContr)
    {
        if (playerContr.view.IsMine && playerContr.listOfCardsIndex.Count == 5)
        {
            int value = 9;
            bool valid = false;
            int powerIncrease = 0;
            int tempValue1 = 0;
            bool first47 = true, first74 = false;
            bool first712 = true, first127 = false;

            int[] tempValue = new int[5];

            int fullValue = 0;

            List<int> cardValue = new List<int>();

            foreach (Card c in listOfCardsPower)
            {
                cardValue.Add(c.value);
            }

            foreach (Card c in listOfCardsPower)
            {
                if ((c.value < 0 && c.isJoker == false))
                {
                    valid = false;
                }
                //value = Joker
                else if (c.isJoker == true)
                {
                    if (!cardValue.Contains(1))
                    {
                        fullValue += 1;
                        cardValue[tempValue1] = 1;
                    }
                    else if (!cardValue.Contains(2))
                    {
                        fullValue += 2;
                        cardValue[tempValue1] = 2;
                    }
                    else if (!cardValue.Contains(4))
                    {
                        fullValue += 4;
                        cardValue[tempValue1] = 4;
                    }
                }
                //value = 47 = 4
                else if (c.value == 47 && first47 == true && first74 == false)
                {
                    if (!cardValue.Contains(4))
                    {
                        fullValue += 4;
                        first47 = false;
                        first74 = true;
                    }
                    else
                    {

                        fullValue += 7;
                        first47 = true;
                        first74 = false;
                        first712 = false;
                        first127 = true;
                    }
                }
                //value = 47 = 7
                else if (c.value == 47 && first47 == false && first74 == true)
                {
                    fullValue += 7;
                    first47 = true;
                    first74 = false;
                    first712 = false;
                    first127 = true;
                }
                //value = 712 = 7
                else if (c.value == 712 && first127 == false && first712 == true)
                {
                    if (!cardValue.Contains(7))
                    {
                        fullValue += 7;
                        first47 = true;
                        first74 = false;
                        first712 = false;
                        first127 = true;
                    }
                    else
                    {
                        fullValue += 12;
                        first712 = true;
                        first127 = false;
                    }
                }
                //value = 712 = 12
                else if (c.value == 712 && first127 == true && first712 == false)
                {
                    fullValue += 12;
                    first712 = true;
                    first127 = false;
                }
                else if (c.value > 0 && c.value < 13 && c.isJoker == false)
                {
                    fullValue += c.value;
                    if (c.value == 4)
                    {
                        first47 = false;
                        first74 = true;
                    }
                    else if (c.value == 7)
                    {
                        first47 = true;
                        first74 = false;
                        first712 = false;
                        first127 = true;
                    }
                    else if (c.value == 12)
                    {
                        first712 = true;
                        first127 = false;
                    }
                }

                tempValue1++;
            }

            if (fullValue == 26)
            {
                if (cardValue.Contains(12) || cardValue.Contains(712))
                {
                    if (cardValue.Contains(7) || cardValue.Contains(47) || (cardValue.Contains(12) && cardValue.Contains(712)) || (cardValue.Contains(712) && cardValue.Contains(712)))
                        valid = true;
                }
            }

            if (valid)
            {
                arrayOfCardIndex = listOfCardsIndex.ToArray();

                for (int i = 0; i < arrayOfCardIndex.Length; i++)
                {
                    if (i == arrayOfCardIndex.Length - 1)
                    {
                        powerIncrease = value;
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], powerIncrease, 0, 1);
                        view.RPC("UpdateActionsPlayer", RpcTarget.All, playerContr.view.ViewID);
                    }
                    else
                    {
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], 0, 0, 1);
                    }

                }
                Debug.Log("Value: " + value);
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
            else
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                Debug.Log("FullValue: " + fullValue);
                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Sequence"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
        }
        else if (playerContr.listOfCardsIndex.Count != 5)
        {
            if (playerContr.listOfCardsIndex.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Sequence"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
        }
    }

    public void ActionSequenceInc(PlayerController playerContr)
    {
        if (playerContr.view.IsMine && playerContr.listOfCardsIndex.Count == 3)
        {
            int value = 3;
            bool valid = false;
            int powerIncrease = 0;
            int tempValue1 = 0;

            int[] tempValue = new int[5];

            int fullValue = 0;

            List<int> cardValue = new List<int>();

            foreach (Card c in listOfCardsPower)
            {
                cardValue.Add(c.value);
            }

            foreach (Card c in listOfCardsPower)
            {
                if ((c.value < 0 && c.isJoker == false))
                {
                    valid = false;
                }
                //value = Joker
                else if (c.isJoker == true)
                {
                    if (!cardValue.Contains(1))
                    {
                        fullValue += 1;
                        cardValue[tempValue1] = 1;
                    }
                    else if (!cardValue.Contains(2))
                    {
                        fullValue += 2;
                        cardValue[tempValue1] = 2;
                    }
                    else if (!cardValue.Contains(4))
                    {
                        fullValue += 4;
                        cardValue[tempValue1] = 4;
                    }
                }
                //value = 47
                else if (c.value == 47)
                {
                    if (!cardValue.Contains(4))
                    {
                        fullValue += 4;
                        cardValue[tempValue1] = 4;
                    }
                }
                else if (c.value > 0 && c.value < 5 && c.isJoker == false)
                {
                    fullValue += c.value;
                }

                tempValue1++;
            }

            if (fullValue == 7)
            {
                if (cardValue.Contains(4) || cardValue.Contains(47))
                {
                    valid = true;
                }
            }

            if (valid)
            {
                arrayOfCardIndex = listOfCardsIndex.ToArray();

                for (int i = 0; i < arrayOfCardIndex.Length; i++)
                {
                    if (i == arrayOfCardIndex.Length - 1)
                    {
                        powerIncrease = value;
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], powerIncrease, 0, 1);
                        view.RPC("UpdateActionsPlayer", RpcTarget.All, playerContr.view.ViewID);
                    }
                    else
                    {
                        view.RPC("ActionTrio", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i], 0, 0, 1);
                    }

                }
                Debug.Log("Value: " + value);
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
            else
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                Debug.Log("FullValue: " + fullValue);
                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Sequence"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
        }
        else if (playerContr.listOfCardsIndex.Count != 3)
        {
            if (playerContr.listOfCardsIndex.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Sequence"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
        }
    }

    public void ActionNegativeOnEnemy(PlayerController playerContr, PlayerController playerContrSteal)
    {
        if (playerContr.view.IsMine && playerContr.listOfCardsIndex.Count == 1)
        {
            bool valid = true;

            foreach (Card c in listOfCardsPower)
            {
                if (c.value > 0)
                {
                    valid = false;
                }
            }

            if (valid)
            {
                view.RPC("NegativeOnEnemy", RpcTarget.All, this.view.ViewID, playerContrSteal.view.ViewID, listOfCardsIndex[0]);
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
            else
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Invalid Card"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();
            }
        }
        else if (playerContr.listOfCardsIndex.Count != 1)
        {
            if (playerContr.listOfCardsIndex.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Select only 1 card"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
        }
    }

    [PunRPC]
    public void ActionToPlaceCards(int viewID, int cardIndex)
    {
        GameObject[] players = GameObject.FindGameObjectsWithTag("Player");

        for (int i = 0; i < players.Length; i++)
        {
            if (players[i].GetComponent<PlayerController>().canPlay && players[i].GetComponent<PlayerController>().view.ViewID == viewID)
            {
                PlayerController playerController = players[i].GetComponent<PlayerController>();
                GameObject child = players[i].transform.GetChild(0).gameObject;
                GameObject childPOwnedPanel = child.transform.GetChild(2).gameObject;
                GameObject childPlacedPanel = child.transform.GetChild(3).gameObject;

                GameObject childCardWeWant = null;

                for (int n = 0; n < childPOwnedPanel.transform.childCount; n++)
                {
                    if (childPOwnedPanel.transform.GetChild(n).GetComponent<Card>() != null)
                    {
                        Card cardapio = childPOwnedPanel.transform.GetChild(n).GetComponent<Card>();
                        if (cardapio.cardOwnedSlot == cardIndex)
                        {
                            childCardWeWant = cardapio.gameObject;
                            break;
                        }
                    }
                }

                Card cardComponent = childCardWeWant.GetComponent<Card>();

                int hadPlace = 0;

                for(int f = 0; f < playerController.availablePlacedCardSlots.Length; f++)
                {
                    if(availablePlacedCardSlots[f] == true)
                    {
                        hadPlace = 1;

                        //Card cardComponent = childCardWeWant.GetComponent<Card>();
                        //Card

                        childCardWeWant.transform.parent = childPlacedPanel.transform;
                        childCardWeWant.transform.position = playerController.placedCardSlots[f].transform.position;

                        cardComponent.cardPlacedSlot = f;

                        cardComponent.cardImageBack.SetActive(false);
                        cardComponent.cardImageFront.SetActive(true);
                        cardComponent.cardImageShader.SetActive(false);


                        //NAO SEI SE O JOKER CONTA
                        //cardComponent.isJoker = true;

                        //Player
                        playerController.availableCardSlots[cardComponent.cardOwnedSlot] = true;
                        playerController.availablePlacedCardSlots[f] = false;

                        playerController.myPlacedCards.Add(cardComponent);

                        playerController.powerChosen = "";
                        playerController.canClickButtons = true;

                        playerController.amountOfCardsPlaced += 1;

                        return;
                    }
                }

                if(hadPlace == 0)
                {
                    cardComponent.cardImageShader.SetActive(false);
                }

                
            }
        }
    }

    public void ActionToPlace()
    {
        PlayerController playerContr = this;

        foreach(PlayerController p in gameManager.playerControllersInGame)
        {
            if (p.view.IsMine)
            {
                playerContr = p;
            }
        }


        if (playerContr.view.IsMine && playerContr.listOfCardsIndex.Count >= 1 && playerContr.actionPhase && playerContr.actionsAvailable > 0)
        {
            arrayOfCardIndex = listOfCardsIndex.ToArray();

            for (int i = 0; i < arrayOfCardIndex.Length; i++)
            {
                view.RPC("ActionToPlaceCards", RpcTarget.All, playerContr.view.ViewID, arrayOfCardIndex[i]);
            }
            view.RPC("UpdateActionsPlayer", RpcTarget.All, playerContr.view.ViewID);

            listOfCardsIndex.Clear();
            listOfCardsPower.Clear();
           
        }
        else if (playerContr.listOfCardsIndex.Count == 0)
        {
            if (playerContr.listOfCardsIndex.Count > 0)
            {
                foreach (Card c in listOfCardsPower)
                {
                    if (c.isJoker == false)
                    {
                        c.cardImageBack.SetActive(false);
                        c.cardImageFront.SetActive(true);
                    }
                    c.cardImageShader.SetActive(false);
                }

                StartCoroutine(playerContr.FadeTextToZeroAlpha(2f, "Need atleast 1 card"));
                listOfCardsIndex.Clear();
                listOfCardsPower.Clear();

            }
        }
    }

    [PunRPC]
    public void UpdateWhenLeavingRoom(int viewID, int viewIDActivation)
    {
        List<PlayerController> listPlayers = new List<PlayerController>();
        for (int i = 0; i < gameManager.playerControllersInGame.Length; i++)
        {
            listPlayers.Add(gameManager.playerControllersInGame[i]);
        }

        PlayerController a = null;
        int k = 0;
        PlayerController b = null;

        for (int j = 0; j < gameManager.playerControllersInGame.Length; j++)
        {

            if (gameManager.playerControllersInGame[j].view.ViewID == viewID)
            {
                a = gameManager.playerControllersInGame[j];
                k = j;                              
            }
            else if(gameManager.playerControllersInGame[j].view.ViewID == viewIDActivation)
            {
                b = gameManager.playerControllersInGame[j];
            }

            if(a != null && b != null)
            {
                if(a.canPlay == true)
                {
                    b.timeLeft = 240;
                    b.actionsAvailable = 2;
                    b.canPlay = true;
                    b.canChoosePower = true;
                    b.canGetCards = false;
                    b.powerHasBeenChosen = false;
                    b.powerChosen = "";
                    b.fullDefense = false;
                    b.cardsPerRound = 0;
                    b.actionPhase = false;
                }

                a.canPlay = false;
                a.actionsAvailable = 0;
                a.gameManager.playerControllersInGame[j].gameObject.SetActive(false);

                listPlayers.RemoveAt(j);
                a.gameManager.playerControllersInGame = listPlayers.ToArray();
                Array.Sort(gameManager.playerControllersInGame);

                gameManager.playerPlayingImage.sprite = b.powerImage.sprite;
            }
        }
    } 

    public IEnumerator FadeTextToZeroAlpha(float t, string textString)
    {
        Text i = this.infoText;
        infoText.text = textString;
        Image e = this.infoImage;

        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        e.color = new Color(e.color.r, e.color.g, e.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            e.color = new Color(e.color.r, e.color.g, e.color.b, e.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator TimeToClickActions(float t)
    {
        float a = 0;
        while (a < (t + 1))
        {
            a += Time.deltaTime;
            yield return null;
        }

        if (a >= t)
        {
            this.canClickButtons = true;
        }
    }

    public void OnApplicationQuit()
    {

    }

    public void OnApplicationFocus(bool focus)
    {
        //doest nothing for now
    }

    public override void OnLeftRoom()
    {
        SceneManager.LoadScene("MasterLeftScene");
        base.OnLeftRoom();
    }

    /* public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("MasterLeftScene");
        base.OnConnectedToMaster();
    } */


    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        if (PhotonNetwork.InRoom)
        {
            backgroundMusic.Stop();
            PhotonNetwork.LeaveRoom();
        }

        base.OnMasterClientSwitched(newMasterClient);
    }

}