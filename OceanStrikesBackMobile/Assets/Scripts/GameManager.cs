using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System;

public class GameManager : MonoBehaviourPunCallbacks
{
    public Card card1, card2, card4, card7, card12, cardNegative1, cardNegative2, cardNegative3, cardJoker4_7, cardJoker7_12, cardPoluitrum, cardOceanya, cardXRay;
    public List<Card> deckAmbiente = new List<Card>();
    public List<Card> deckPoluicao = new List<Card>();
    public List<Card> deckMiddleOcean = new List<Card>();

    public List<Card> tempDeck2nfPhase = new List<Card>();

    //Card[] deckAmbiente;
    public Transform[] middleCardSlots;
    public bool[] availableMidSlots;

    public GameObject panelMiddleCards;

    public bool drawMidOnce;

    public List<GameObject> cardsInstantiated = new List<GameObject>();

    public string[] stringNamesMiddleCards;

    public GameObject[] playersInGame;

    public List<PlayerController> playerControllersInGameList = new List<PlayerController>();
    public PlayerController[] playerControllersInGame;

    public PlayerController[] playerControllersOrdered;

    public GameObject textObject;
    public Text text;

    public GameObject imageObject;
    public Image image;

    public Text textStealPlayer1, textStealPlayer2, textStealPlayer3;

    public GameObject imageStealPlayer1, imageStealPlayer2, imageStealPlayer3;

    public Image[] imagesPlayer = new Image[4];
    public Image[] imagesPlayerPoints = new Image[4];
    public Image[] imagesPlayerPolution = new Image[4];

    public Transform[] jokerSpots;
    public Transform[] extraActionSpots;
    public Transform[] fullDefenseSpots;
    public Transform[] stealCardSpots;

    public SpotScript[] spotScriptsPoints;
    public SpotScript[] spotScriptsPolution;

    public Transform playedCardsSpot, polutionCardsSpot;

    public Text timerText;
    public float timerToPlay = 240;

    List<int> randomsSecondPhase = new List<int>();

    public bool secondPhase;

    public bool gameOver;

    public Transform sendCardsToNarnia;

    public Image playerPlayingImage;

    //SHADER
    public Material mat;
    public Material sereiaMat;
    public bool sereiaPlayed;
    public bool poluitrumPlayed;
    public float sereiaTime;
    public GameObject sereiaObject;
    float dissolveSlider;

    bool doOnceShader = false;
    bool doTime = false;
    public bool thirdPhase = false;

    float timeShader = 0;

    public AudioSource audioSourceMusic;
    public AudioSource audioSourceClips;
    public AudioClip oceanyaClip;
    public AudioClip poluitrumClip;
    public AudioClip cardClickClip;

    public float timeLeft;

    public Slider sliderTime;
    public Image sliderImage;
    public Image sliderColor;
    public Image sliderColorNext;

    private void Start()
    {
        PhotonNetwork.AutomaticallySyncScene = false;
        PhotonNetwork.KeepAliveInBackground = 20f;

        sereiaTime = 0;
        dissolveSlider = 1.1f;
        timerToPlay = 240;

        textObject.GetComponent<GameObject>();
        text.GetComponent<Text>();

        imageObject.GetComponent<GameObject>();
        image.GetComponent<Image>();

        drawMidOnce = true;
        stringNamesMiddleCards = new string[6];

        deckAmbiente.Clear();
        deckPoluicao.Clear();
        deckMiddleOcean.Clear();

        //deckMiddleOcean
        deckMiddleOcean.Add(card1);
        deckMiddleOcean.Add(card1);
        deckMiddleOcean.Add(card2);
        deckMiddleOcean.Add(card4);
        deckMiddleOcean.Add(cardNegative1);
        deckMiddleOcean.Add(cardNegative1);
        deckMiddleOcean.Add(cardNegative2);
        deckMiddleOcean.Add(cardNegative3);

        if (PhotonNetwork.IsMasterClient)
        {
            int[] randomMiddle = RandomMiddleNumbers(6);
            SetMiddleDeck(randomMiddle);

            for(int i = 0; i < cardsInstantiated.Count; i++)
            {
                stringNamesMiddleCards[i] = cardsInstantiated[i].name;
            }
        }

        photonView.RPC("SetParentByString", RpcTarget.All, "MiddleOceanPanel");

        //card 1 2
        for (int i = 0; i < 9; i++)
        {
            deckAmbiente.Add(card1);
            deckAmbiente.Add(card2);
        }
        //card 4
        for (int i = 0; i < 11; i++)
        {
            deckAmbiente.Add(card4);
        }
        //card 7
        for (int i = 0; i < 6; i++)
        {
            deckAmbiente.Add(card7);
        }
        //card 12
        for (int i = 0; i < 4; i++)
        {
            deckAmbiente.Add(card12);
        }
        //card negative 1
        for (int i = 0; i < 18; i++)
        {
            deckPoluicao.Add(cardNegative1);
        }
        //card negative 2
        for (int i = 0; i < 15; i++)
        {
            deckPoluicao.Add(cardNegative2);
        }
        //card negative 3
        for (int i = 0; i < 9; i++)
        {
            deckPoluicao.Add(cardNegative3);
        }
        //card joker 4/7
        for (int i = 0; i < 5; i++)
        {
            deckAmbiente.Add(cardJoker4_7);
        }
        //card joker 7/12
        for (int i = 0; i < 3; i++)
        {
            deckAmbiente.Add(cardJoker7_12);
        }
        //card Oceanya Poluitrum
        for (int i = 0; i < 4; i++)
        {
            deckAmbiente.Add(cardOceanya);
            deckPoluicao.Add(cardPoluitrum);
        }
        //card RayX
        for (int i = 0; i < 3; i++)
        {
            deckAmbiente.Add(cardXRay);
        }

    }

    [PunRPC]
    public void SetParentByString(string parentName)
    {

        deckMiddleOcean.Clear();

        GameObject[] findCards = GameObject.FindGameObjectsWithTag("Card");

        for (int i = 0; i < findCards.Length; i ++)
        {
            findCards[i].GetComponent<Card>().cardMidSlot = i;
            findCards[i].transform.SetParent(GameObject.Find(parentName).transform);

            findCards[i].transform.localScale = new Vector3(1, 1, 1);

            findCards[i].SetActive(true);

            cardsInstantiated.Add(findCards[i]);

            availableMidSlots[i] = false;

            deckMiddleOcean.Add(findCards[i].GetComponent<Card>());
        }

        //cardsInstantiated.Clear();
    }

    public int[] RandomMiddleNumbers(int amount)
    {
        List<int> randoms = new List<int>();

        do
        {
            int random = UnityEngine.Random.Range(0, deckMiddleOcean.Count);
            if (!randoms.Contains(random))
            {
                randoms.Add(random);
            }

        } while (randoms.Count < 6);

        return randoms.ToArray();
    }

    //[PunRPC]
    public void SetMiddleDeck(int[] numbers)
    {
        //List<Card> initialCards = new List<Card>();

        for (int i = 0; i < 6; i++)
        {
            Card randCard = deckMiddleOcean[numbers[i]];
            for (int j = 0; j < availableMidSlots.Length; j++)
            {
                if (availableMidSlots[i] == true)
                {
                    GameObject cardInstantiated = PhotonNetwork.InstantiateRoomObject(randCard.gameObject.name, middleCardSlots[i].transform.position, Quaternion.identity);

                    break;
                }
            }
        }
    }

    [PunRPC]
    public void PlayerPlaying(int playerPlaying)
    {
        for(int j = 0; j < playerControllersInGame.Length; j++)
        {
            //SWAP THE - TO + TO THE TOP
            if(playerControllersInGame[j].view.ViewID == playerPlaying)
            {
                playerControllersInGame[j + 1].actionsAvailable = 2;
                playerControllersInGame[j + 1].canPlay = true;
                playerControllersInGame[j + 1].canChoosePower = true;
                playerControllersInGame[j + 1].canGetCards = false;
                playerControllersInGame[j + 1].powerHasBeenChosen = false;
                playerControllersInGame[j + 1].powerChosen = "";
                playerControllersInGame[j + 1].fullDefense = false;
                playerControllersInGame[j + 1].cardsPerRound = 0;
                playerControllersInGame[j + 1].actionPhase = false;
                playerControllersInGame[j + 1].canClickButtons = true;

                playerPlayingImage.sprite = playerControllersInGame[j+1].powerImage.sprite;

                playerControllersInGame[j].timeLeft = timerToPlay;
                playerControllersInGame[j].canPlay = false;
                playerControllersInGame[j].canGetCards = false;
                playerControllersInGame[j].canChoosePower = false;
                playerControllersInGame[j].powerHasBeenChosen = true;
                playerControllersInGame[j].powerChosen = "";
                playerControllersInGame[j].fullDefense = false;
                playerControllersInGame[j].cardsPerRound = 0;
                playerControllersInGame[j].actionPhase = false;

                //Debug.Log("J value: " + j + " Length: " + (playerControllersInGame.Length - 1));

                imageObject.SetActive(true);
                textObject.SetActive(true);
                text.text = PhotonNetwork.PlayerList[j + 1].NickName + " playing.";
                StartCoroutine(FadeTextToZeroAlpha(5f, PhotonNetwork.PlayerList[j + 1].NickName + " playing."));
                Invoke("StartCoroutineAlpha", 1f);
            }
            else
            {
                playerControllersInGame[j].timeLeft = timerToPlay;
            }
        }
    }

    [PunRPC]
    public void PlayerPlayingLast(int playerPlaying)
    {
        for (int j = 0; j < playerControllersInGame.Length; j++)
        {
            if (playerControllersInGame[j].view.ViewID == playerPlaying)
            {
                playerControllersInGame[0].actionsAvailable = 2;
                playerControllersInGame[0].canPlay = true;
                playerControllersInGame[0].canChoosePower = true;
                playerControllersInGame[0].canGetCards = false;
                playerControllersInGame[0].powerHasBeenChosen = false;
                playerControllersInGame[0].powerChosen = "";
                playerControllersInGame[0].fullDefense = false;
                playerControllersInGame[0].cardsPerRound = 0;
                playerControllersInGame[0].actionPhase = false;

                playerPlayingImage.sprite = playerControllersInGame[0].powerImage.sprite;

                playerControllersInGame[j].timeLeft = timerToPlay;
                playerControllersInGame[j].canPlay = false;
                playerControllersInGame[j].canGetCards = false;
                playerControllersInGame[j].canChoosePower = false;
                playerControllersInGame[j].powerHasBeenChosen = true;
                playerControllersInGame[j].powerChosen = "";
                playerControllersInGame[j].fullDefense = false;
                playerControllersInGame[j].cardsPerRound = 0;
                playerControllersInGame[j].actionPhase = false;

                //Debug.Log("J value: " + j + " Length: " + (playerControllersInGame.Length - 1));

                imageObject.SetActive(true);
                textObject.SetActive(true);
                text.text = PhotonNetwork.PlayerList[0].NickName + " playing.";
                StartCoroutine(FadeTextToZeroAlpha(5f, PhotonNetwork.PlayerList[0].NickName + " playing."));
                Invoke("StartCoroutineAlpha", 1f);
            }
            else
            {
                playerControllersInGame[j].timeLeft = timerToPlay;
            }
        }
    }

    [PunRPC]
    public void PlayerPlayingAlone(int playerPlaying)
    {
        for (int j = 0; j < playerControllersInGame.Length; j++)
        {
            if (playerControllersInGame[j].view.ViewID == playerPlaying)
            {
                playerControllersInGame[j].timeLeft = timerToPlay;
                playerControllersInGame[j].actionsAvailable = 2;
                playerControllersInGame[j].canPlay = true;
                playerControllersInGame[j].canChoosePower = true;
                playerControllersInGame[j].canGetCards = false;
                playerControllersInGame[j].powerHasBeenChosen = false;
                playerControllersInGame[j].powerChosen = "";
                playerControllersInGame[j].fullDefense = false;
                playerControllersInGame[j].cardsPerRound = 0;
                playerControllersInGame[j].actionPhase = false;
                playerPlayingImage.sprite = playerControllersInGame[j].powerImage.sprite;

                imageObject.SetActive(true);
                textObject.SetActive(true);
                text.text = PhotonNetwork.PlayerList[j].NickName + " playing.";
                StartCoroutine(FadeTextToZeroAlpha(5f, PhotonNetwork.PlayerList[j].NickName + " playing."));
                Invoke("StartCoroutineAlpha", 1f);

                Debug.Log("Length: " + (playerControllersInGame.Length - 1));
            }
        }
    }

    public void WhoIsPlaying()
    {
        if (playerControllersInGame.Length == PhotonNetwork.PlayerList.Length)
        {
            for (int j = 0; j < playerControllersInGame.Length; j++)
            {
                if (playerControllersInGame[j].canPlay && playerControllersInGame[j].actionsAvailable == 0 && playerControllersInGame.Length > 1)
                {
                    if (j < playerControllersInGame.Length - 1)
                    {                       
                        int playerPlaying = playerControllersInGame[j].view.ViewID;
                        photonView.RPC("PlayerPlaying", RpcTarget.All, playerPlaying);

                    }
                    else if(j == playerControllersInGame.Length - 1)
                    {
                        int playerPlaying = playerControllersInGame[j].view.ViewID;
                        photonView.RPC("PlayerPlayingLast", RpcTarget.All, playerPlaying);

                    }

                }
                else if (playerControllersInGame[j].actionsAvailable == 0 && playerControllersInGame.Length == 1)
                {               
                    int playerPlaying = playerControllersInGame[j].view.ViewID;
                    photonView.RPC("PlayerPlayingAlone", RpcTarget.All, playerPlaying);

                }
                else
                {
                    continue;
                }
            }
        }
    }

    private void LateUpdate()
    {

        if (playerControllersInGame.Length < PhotonNetwork.PlayerList.Length)
        {
            playerControllersInGame = FindObjectsOfType<PlayerController>();
        }

        if (drawMidOnce && (playerControllersInGame.Length == PhotonNetwork.PlayerList.Length))
        {

            Array.Sort(playerControllersInGame);

            Debug.Log("Num of connections: " + PhotonNetwork.PlayerList.Length);
            Debug.Log("Num of player Controllers " + playerControllersInGame.Length);

            Vector3 moveButtonsUp = new Vector3(0, 0.575f, 0);

            playerControllersInGame[0].namePlayer = PhotonNetwork.PlayerList[0].NickName;
            playerControllersInGame[0].timeLeft = timerToPlay;
            playerControllersInGame[0].canPlay = true;
            playerControllersInGame[0].canChoosePower = true;
            playerControllersInGame[0].canGetCards = false;
            playerControllersInGame[0].allButtons.transform.position = playerControllersInGame[0].allButtons.transform.position + moveButtonsUp;
            playerControllersInGame[0].powerImage = imagesPlayer[0];
            playerControllersInGame[0].pointsImage = imagesPlayerPoints[0];
            playerControllersInGame[0].polutionImage = imagesPlayerPolution[0];
            playerControllersInGame[0].playerSereiaObject.SetActive(false);
            moveButtonsUp = moveButtonsUp + new Vector3(0, 2.3f, 0);

            imageObject.SetActive(true);
            textObject.SetActive(true);
            text.text = PhotonNetwork.PlayerList[0].NickName + " playing.";
            StartCoroutine(FadeTextToZeroAlpha(5f, PhotonNetwork.PlayerList[0].NickName + " playing."));
            Invoke("StartCoroutineAlpha", 1f);

            playerPlayingImage.sprite = playerControllersInGame[0].powerImage.sprite;

            if (playerControllersInGame.Length > 1)
            {
                for (int i = 1; i < playerControllersInGame.Length; i++)
                {
                    playerControllersInGame[i].namePlayer = PhotonNetwork.PlayerList[i].NickName;
                    playerControllersInGame[i].timeLeft = timerToPlay;
                    playerControllersInGame[i].powerImage = imagesPlayer[i];
                    playerControllersInGame[i].pointsImage = imagesPlayerPoints[i];
                    playerControllersInGame[i].polutionImage = imagesPlayerPolution[i];
                    playerControllersInGame[i].canPlay = false;
                    playerControllersInGame[i].canGetCards = false;
                    playerControllersInGame[i].canChoosePower = false;
                    playerControllersInGame[i].playerSereiaObject.SetActive(false);
                    playerControllersInGame[i].allButtons.transform.position = playerControllersInGame[i].allButtons.transform.position + moveButtonsUp;
                    moveButtonsUp = moveButtonsUp + new Vector3(0, 2.3f, 0);
                }
            }

            drawMidOnce = false;
        }

        if(PhotonNetwork.LocalPlayer.IsMasterClient)
        {
            WhoIsPlaying();
        }

        if(playerControllersInGame.Length == PhotonNetwork.PlayerList.Length)
        {
            for(int b = 0; b < playerControllersInGame.Length; b++)
            {
                int a = playerControllersInGame[b].polutionPoints;
                if(a > -1)
                {
                    playerControllersInGame[b].polutionImage.transform.position = spotScriptsPolution[a].transform.position;
                }
                else
                {
                    a = 0;
                    playerControllersInGame[b].polutionImage.transform.position = spotScriptsPolution[a].transform.position;
                }

                int c = playerControllersInGame[b].victoryPoints;
                if(c < 47 && c > -1)
                {
                    playerControllersInGame[b].pointsImage.transform.position = spotScriptsPoints[c].transform.position;
                }
                else if(c < 0)
                {
                    c = 0;
                    playerControllersInGame[b].pointsImage.transform.position = spotScriptsPoints[c].transform.position;
                }
                else if(c > 46)
                {
                    c = 46;
                    playerControllersInGame[b].pointsImage.transform.position = spotScriptsPoints[c].transform.position;
                }


            }
        }

       
        if(deckAmbiente.Count <= 0 && deckPoluicao.Count > 0 && secondPhase == false)
        {
            StartCoroutine(FadeTextToZeroAlpha(5f, "2nd Phase Started"));
            secondPhase = true;

            int countCards = playedCardsSpot.childCount;
            int totalCount = countCards + deckPoluicao.Count;

            Debug.Log("TotalCount: " + totalCount);

            for (int i = 0; i < countCards; i++)
            {
                Card card = playedCardsSpot.GetChild(i).gameObject.GetComponent<Card>();
                card.transform.position = sendCardsToNarnia.transform.position;

                if(card.value == 1)
                {
                    tempDeck2nfPhase.Add(card1);
                }
                else if (card.value == 2)
                {
                    tempDeck2nfPhase.Add(card2);
                }
                else if (card.value == 4)
                {
                    tempDeck2nfPhase.Add(card4);
                }
                else if (card.value == 7)
                {
                    tempDeck2nfPhase.Add(card7);
                }
                else if (card.value == 12)
                {
                    tempDeck2nfPhase.Add(card12);
                }
                else if (card.value == 47)
                {
                    tempDeck2nfPhase.Add(cardJoker4_7);
                }
                else if (card.value == 712)
                {
                    tempDeck2nfPhase.Add(cardJoker7_12);
                }
                else if (card.value == 1000)
                {
                    tempDeck2nfPhase.Add(cardOceanya);
                }
                else if (card.value == 2000)
                {
                    tempDeck2nfPhase.Add(cardPoluitrum);
                }
                else if (card.value == 3000)
                {
                    tempDeck2nfPhase.Add(cardXRay);
                }

            }

            foreach(Card c in deckPoluicao)
            {
                tempDeck2nfPhase.Add(c);         
            }

            deckAmbiente.Clear();
            deckPoluicao.Clear();

            bool addedToAmbiente = false;

            if (PhotonNetwork.IsMasterClient)
            {
                do
                {
                    int random = UnityEngine.Random.Range(0, tempDeck2nfPhase.Count);
                    if (addedToAmbiente && !randomsSecondPhase.Contains(random))
                    {
                        //randomsSecondPhase.Add(random);
                        //deckPoluicao.Add(tempDeck2nfPhase[random]);
                        photonView.RPC("FillTheDecksSecondPhasePolution", RpcTarget.All, random);
                        addedToAmbiente = false;
                    }
                    else if (!addedToAmbiente && !randomsSecondPhase.Contains(random))
                    {
                        //randomsSecondPhase.Add(random);
                        //deckAmbiente.Add(tempDeck2nfPhase[random]);
                        photonView.RPC("FillTheDecksSecondPhaseAmbient", RpcTarget.All, random);
                        addedToAmbiente = true;
                    }

                } while (randomsSecondPhase.Count < totalCount);
            }          

        }
        else if (deckPoluicao.Count <= 0 && deckAmbiente.Count > 0 && secondPhase == false)
        {
            StartCoroutine(FadeTextToZeroAlpha(5f, "2nd Phase Started"));
            secondPhase = true;

            int countCards = playedCardsSpot.childCount;
            int totalCount = countCards + deckAmbiente.Count;

            Debug.Log("TotalCount: " + totalCount);

            for (int i = 0; i < countCards; i++)
            {
                Card card = playedCardsSpot.GetChild(i).gameObject.GetComponent<Card>();
                card.transform.position = sendCardsToNarnia.transform.position;

                if (card.value == 1)
                {
                    tempDeck2nfPhase.Add(card1);
                }
                else if (card.value == 2)
                {
                    tempDeck2nfPhase.Add(card2);
                }
                else if (card.value == 4)
                {
                    tempDeck2nfPhase.Add(card4);
                }
                else if (card.value == 7)
                {
                    tempDeck2nfPhase.Add(card7);
                }
                else if (card.value == 12)
                {
                    tempDeck2nfPhase.Add(card12);
                }
                else if (card.value == 47)
                {
                    tempDeck2nfPhase.Add(cardJoker4_7);
                }
                else if (card.value == 712)
                {
                    tempDeck2nfPhase.Add(cardJoker7_12);
                }
                else if (card.value == 1000)
                {
                    tempDeck2nfPhase.Add(cardOceanya);
                }
                else if (card.value == 2000)
                {
                    tempDeck2nfPhase.Add(cardPoluitrum);
                }
                else if (card.value == 3000)
                {
                    tempDeck2nfPhase.Add(cardXRay);
                }

            }

            foreach (Card c in deckAmbiente)
            {
                tempDeck2nfPhase.Add(c);
            }

            deckAmbiente.Clear();
            deckPoluicao.Clear();

            bool addedToAmbiente = false;

            if (PhotonNetwork.IsMasterClient)
            {
                do
                {
                    int random = UnityEngine.Random.Range(0, tempDeck2nfPhase.Count);
                    if (addedToAmbiente && !randomsSecondPhase.Contains(random))
                    {
                        //randomsSecondPhase.Add(random);
                        //deckPoluicao.Add(tempDeck2nfPhase[random]);
                        photonView.RPC("FillTheDecksSecondPhasePolution", RpcTarget.All, random);
                        addedToAmbiente = false;
                    }
                    else if (!addedToAmbiente && !randomsSecondPhase.Contains(random))
                    {
                        //randomsSecondPhase.Add(random);
                        //deckAmbiente.Add(tempDeck2nfPhase[random]);
                        photonView.RPC("FillTheDecksSecondPhaseAmbient", RpcTarget.All, random);
                        addedToAmbiente = true;
                    }

                } while (randomsSecondPhase.Count < totalCount);
            }

            //Debug.Log("TotalCountRandoms: " + randomsSecondPhase.Count);

        }

        if (secondPhase == true && doOnceShader == false)
        {
            mat.SetFloat("SecondPhaseValue", 2);

            doOnceShader = true;
        }
        else if (doOnceShader == true && timeShader < 12)
        {
            timeShader += Time.deltaTime;
        }
        else if (timeShader >= 11 && !doTime)
        {
            mat.SetFloat("SecondPhaseValue", 1);

            doTime = true;
        }

        //gameOver
        if (secondPhase && (deckAmbiente.Count <= 0 || deckPoluicao.Count <= 0) && randomsSecondPhase.Count > 10 && PhotonNetwork.IsMasterClient)
        {
            photonView.RPC("SetThirdPhase", RpcTarget.All);       
        }

        if (thirdPhase)
        {
            int[] victoryPoints = new int[playerControllersInGame.Length];
            int[] polutionPoints = new int[playerControllersInGame.Length];

            for (int i = 0; i < playerControllersInGame.Length; i++)
            {
                if (playerControllersInGame[i].gameManager.randomsSecondPhase.Count > 10 && playerControllersInGame[i].gameOver == false)
                {
                    victoryPoints[i] = playerControllersInGame[i].victoryPoints;
                    polutionPoints[i] = playerControllersInGame[i].polutionPoints;

                    if (polutionPoints[i] == 1)
                    {
                        playerControllersInGame[i].victoryPoints -= 3;
                        victoryPoints[i] -= 3;
                    }
                    else if (polutionPoints[i] == 2)
                    {
                        playerControllersInGame[i].victoryPoints -= 6;
                        victoryPoints[i] -= 6;
                    }
                    else if (polutionPoints[i] == 3)
                    {
                        playerControllersInGame[i].victoryPoints -= 9;
                        victoryPoints[i] -= 9;
                    }
                    else if (polutionPoints[i] == 4)
                    {
                        playerControllersInGame[i].victoryPoints -= 12;
                        victoryPoints[i] -= 12;
                    }
                    else if (polutionPoints[i] >= 5)
                    {
                        playerControllersInGame[i].victoryPoints = 0;
                        victoryPoints[i] = 0;
                    }
                }
            }

            Array.Sort(victoryPoints);
            Array.Reverse(victoryPoints);

            for (int u = 0; u < playerControllersInGame.Length; u++)
            {
                if (playerControllersInGame[u].gameManager.randomsSecondPhase.Count > 10 && playerControllersInGame[u].gameOver == false)
                {
                    if (playerControllersInGame[u].victoryPoints == victoryPoints[0])
                    {
                        playerControllersInGame[u].isWinner = true;
                        playerControllersInGame[u].gameOver = true;
                    }
                    else
                    {
                        playerControllersInGame[u].isWinner = false;
                        playerControllersInGame[u].gameOver = true;
                    }
                }
            }
        }

    }

    [PunRPC]
    public void SetThirdPhase()
    {
        thirdPhase = true;
    }


    [PunRPC]
    public void FillTheDecksSecondPhaseAmbient(int random)
    {
        randomsSecondPhase.Add(random);
        deckAmbiente.Add(tempDeck2nfPhase[random]);
    }

    [PunRPC]
    public void FillTheDecksSecondPhasePolution(int random)
    {
        randomsSecondPhase.Add(random);
        deckPoluicao.Add(tempDeck2nfPhase[random]);
    }

    public void StartCoroutineAlpha()
    {
        StartCoroutine(FadeTextToZeroAlpha(5f, "Choose a power"));
    }

    public IEnumerator FadeTextToFullAlpha(float t, Text i, Image e)
    {
        i.color = new Color(i.color.r, i.color.g, i.color.b, 0);
        e.color = new Color(e.color.r, e.color.g, e.color.b, 0);
        while (i.color.a < 1.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a + (Time.deltaTime / t));
            e.color = new Color(e.color.r, e.color.g, e.color.b, e.color.a + (Time.deltaTime / t));
            yield return null;
        }
    }

    public IEnumerator FadeTextToZeroAlpha(float t, string textString)
    {
        Text i = this.text;
        text.text = textString;
        Image e = this.image;

        i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
        e.color = new Color(e.color.r, e.color.g, e.color.b, 1);
        while (i.color.a > 0.0f)
        {
            i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
            e.color = new Color(e.color.r, e.color.g, e.color.b, e.color.a - (Time.deltaTime / t));
            yield return null;
        }
    }

    private void Update()
    {      
        /* if(this.playerControllersInGame.Length == PhotonNetwork.PlayerList.Length)
        {
            foreach(PlayerController p in playerControllersInGame)
            {
                if (p.canPlay)
                {
                    sliderTime.value = p.timeLeft;
                    sliderImage.sprite = p.powerImage.sprite;
                    //OnTimeToSlider(p.powerImage);
                }
            }
        } */



        //SHADER
        if (sereiaPlayed)
        {
            if (sereiaTime <= 0.5f)
            {
                audioSourceMusic.Pause();
                audioSourceClips.PlayOneShot(oceanyaClip, 0.05f);
            
            }

            //1 is for sereia
            sereiaMat.SetFloat("oceToPolu", 1);
            if (sereiaTime <= 4)
            {
                if(dissolveSlider >= -1)
                {
                    dissolveSlider -= (Time.deltaTime / 2);
                }

                sereiaMat.SetFloat("_dissolveSlider", dissolveSlider);

                //Debug.Log("_dissolveSlider: " + dissolveSlider);
                //Debug.Log("Time: " + sereiaTime);

                sereiaTime += Time.deltaTime;
            }
            else if(sereiaTime >= 4 && sereiaTime < 8)
            {
                if(dissolveSlider <= 1.1)
                {
                    dissolveSlider += (Time.deltaTime / 2);
                }
                sereiaMat.SetFloat("_dissolveSlider", dissolveSlider);

                //Debug.Log("_dissolveSlider: " + dissolveSlider);
                //Debug.Log("Time: " + sereiaTime);

                sereiaTime += Time.deltaTime;
            }
            else if(sereiaTime >= 7.5f)
            {

                dissolveSlider = 1.1f;
                foreach(PlayerController p in playerControllersInGame)
                {
                    p.playerSereiaObject.SetActive(false);
                }
                sereiaTime = 0;
                sereiaPlayed = false;
                sereiaMat.SetFloat("oceToPolu", 0);
                audioSourceClips.Stop();
                audioSourceMusic.UnPause();
            }
        }
        else if (poluitrumPlayed)
        {
            if(sereiaTime <= 0.5f)
            {
                audioSourceMusic.Pause();
                audioSourceClips.PlayOneShot(poluitrumClip, 0.05f);
            }

            //2 is for poluitrum
            sereiaMat.SetFloat("oceToPolu", 2);
            if (sereiaTime <= 4)
            {
                if (dissolveSlider >= -1)
                {
                    dissolveSlider -= (Time.deltaTime / 2);
                }

                sereiaMat.SetFloat("_dissolveSlider", dissolveSlider);

                //Debug.Log("_dissolveSlider: " + dissolveSlider);
                //Debug.Log("Time: " + sereiaTime);

                sereiaTime += Time.deltaTime;
            }
            else if (sereiaTime >= 4 && sereiaTime < 8)
            {
                if (dissolveSlider <= 1.1)
                {
                    dissolveSlider += (Time.deltaTime / 2);
                }
                sereiaMat.SetFloat("_dissolveSlider", dissolveSlider);

                //Debug.Log("_dissolveSlider: " + dissolveSlider);
                //Debug.Log("Time: " + sereiaTime);

                sereiaTime += Time.deltaTime;
            }
            else if (sereiaTime >= 7.5f)
            {
                dissolveSlider = 1.1f;
                foreach (PlayerController p in playerControllersInGame)
                {
                    p.playerSereiaObject.SetActive(false);
                }
                //sereiaObject.SetActive(false);
                sereiaTime = 0;
                poluitrumPlayed = false;
                sereiaMat.SetFloat("oceToPolu", 0);
                audioSourceClips.Stop();
                audioSourceMusic.UnPause();
            }
        }
    }

    [PunRPC]
    public IEnumerator FadeTextToZeroAlphaRPC(float t, string textString)
    {
        if(text != null)
        {
            Text i = this.text;
            text.text = textString;
            Image e = this.image;

            i.color = new Color(i.color.r, i.color.g, i.color.b, 1);
            e.color = new Color(e.color.r, e.color.g, e.color.b, 1);
            while (i.color.a > 0.0f)
            {
                if(i != null && e != null)
                {
                    i.color = new Color(i.color.r, i.color.g, i.color.b, i.color.a - (Time.deltaTime / t));
                    e.color = new Color(e.color.r, e.color.g, e.color.b, e.color.a - (Time.deltaTime / t));
                    yield return null;
                }
            }
        }
    }

}
