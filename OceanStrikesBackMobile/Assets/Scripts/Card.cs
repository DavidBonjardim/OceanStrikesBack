using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.UI;
using System.Linq;

public class Card : MonoBehaviour
{
    public List<Card> deckMiddleOcean = new List<Card>();
    public GameObject cardsOwnedPanel;
    public Transform[] cardSlots, middleCardSlots;
    public bool[] availableCardSlots, availableMidSlots;

    //public PhotonView cardView;

    public int cardMidSlot;

    public int cardOwnedSlot, cardPlacedSlot;

    public PlayerController[] playerController;

    public bool isJoker = false;

    public GameObject cardImageFront, cardImageBack, cardImageShader;

    public int value;

    public GameObject buttonOceanya, buttonPolutrum, buttonXRay;

    public bool onMidDeck;

    private void Start()
    {
        playerController = FindObjectsOfType<PlayerController>();
        onMidDeck = true;
        //cardView = GetComponent<PhotonView>();
    }

    public void OnClickPlayCards()
    {
        if (transform.parent.name == "PlaceCardsPanel" || transform.parent.name == "CardsOwnedPanel")
        {
            foreach (PlayerController c in playerController)
            {
                if (c.view.IsMine && c.actionsAvailable > 0 && c.actionPhase == true && this.cardImageBack.activeSelf == false && c.hasClickedPlaceCards == false && value < 900 && onMidDeck == false)
                {
                    //c.arrayOfCardIndex = c.arrayOfCardIndex.Concat(new int[] { this.cardPlacedSlot }).ToArray();
                    //cardImageBack.SetActive(true);
                    if (cardImageShader.activeSelf == true)
                    {
                        if(transform.parent.name == "CardsOwnedPanel")
                        {
                            cardImageShader.SetActive(false);
                            c.listOfCardsPower.Remove(this);
                            c.listOfCardsIndex.Remove(this.cardOwnedSlot);
                        }
                        else if(transform.parent.name == "PlaceCardsPanel") 
                        {
                            cardImageShader.SetActive(false);
                            c.listOfCardsPower.Remove(this);
                            c.listOfCardsIndex.Remove(this.cardPlacedSlot);
                        }

                    }
                    else
                    {
                        if (transform.parent.name == "CardsOwnedPanel")
                        {
                            cardImageShader.SetActive(true);
                            c.listOfCardsPower.Add(this);
                            c.listOfCardsIndex.Add(this.cardOwnedSlot);
                        }
                        else if (transform.parent.name == "PlaceCardsPanel")
                        {
                            cardImageShader.SetActive(true);
                            c.listOfCardsPower.Add(this);
                            c.listOfCardsIndex.Add(this.cardPlacedSlot);
                        }

                    }

                    c.gameManager.audioSourceClips.PlayOneShot(c.gameManager.cardClickClip);

                }
                else if (c.view.IsMine && c.actionsAvailable > 0 && c.actionPhase == true && this.isJoker == true && c.hasClickedPlaceCards == false && value < 900 && onMidDeck == false)
                {
                    //c.arrayOfCardIndex = c.arrayOfCardIndex.Concat(new int[] { this.cardPlacedSlot }).ToArray();
                    //cardImageBack.SetActive(true);

                    if (cardImageShader.activeSelf == true)
                    {
                        cardImageShader.SetActive(false);
                        c.listOfCardsPower.Remove(this);
                        c.listOfCardsIndex.Remove(this.cardPlacedSlot);
                    }
                    else
                    {
                        cardImageShader.SetActive(true);
                        c.listOfCardsPower.Add(this);
                        c.listOfCardsIndex.Add(this.cardPlacedSlot);
                    }
                }
            }
        }

    }

    /* public void OnClickPoluteCards()
    {
        if(transform.parent.name == "CardsOwnedPanel")
        {
            foreach (PlayerController c in playerController)
            {
                if (c.view.IsMine && c.actionsAvailable > 0 && c.actionPhase == true && this.cardImageBack.activeSelf == false && c.wantToPollute && c.hasClickedPlaceCards == false)
                {
                    c.listOfCardsPower.Add(this);
                    c.listOfCardsIndex.Add(this.cardOwnedSlot);

                    c.panelOwnedCards.SetActive(false);
                    c.panelPlaceCards.SetActive(false);
                    c.gameManager.panelMiddleCards.SetActive(true);
                    c.stealCardPowerObject.SetActive(true);
                    c.closeStealPanelButton.SetActive(true);

                    c.pulliteOthersButton.transform.GetChild(0).gameObject.GetComponent<Text>().text = "Click to pollute enemy";
                    c.StealCardPower();
                }
            }
        }
    } */

    public void OnClickMiddleCard()
    {
        if(transform.parent.name == "PlayedCardsSpot" || transform.parent.name == "PolutionCardsSpot")
        {
            Debug.Log("Tried to click card");
        }
        else if (transform.parent.name == "MiddleOceanPanel" && onMidDeck && cardImageFront.activeSelf == true)
        {
            foreach (PlayerController c in playerController)
            {
                if (c.view.IsMine)
                {
                    if(c.canPlay && !c.canChoosePower)
                    {
                        c.gameManager.audioSourceClips.PlayOneShot(c.gameManager.cardClickClip);
                    }
                    c.OnClickMiddleCard(this, c);
                }
            }
        }

    }

    //TALVEZ TEREI QUE FAZER ISTO NO PLAYER CONTROLLER, PARA CONSEGUIR LIDAR MELHOR COM AS CARTAS E DAR INFORMAÇÕES AOS OUTROS JOGADORES

    //TUDO CERTO, FALTA TRANSMITIR A INFORMAÇÃO PARA OS OUTROS, PENSAR NO QUE TRANSMITIR, E COMO TRANSMITIR

    public void OnClickOwnedPanelCardJoker()
    {
        if (transform.parent.name == "CardsOwnedPanel")
        {
            foreach (PlayerController c in playerController)
            {
                if (c.view.IsMine && c.powerChosen == "JokerCardPower" && (this.value > -4 && this.value < 8))
                {
                    c.ClickOwnedPanelCardJoker(this, c);
                  

                }
            }
        }
    }

    public void OnClickOwnedPanelCardSteal()
    {
        if (transform.parent.name == "CardsOwnedPanel")
        {
            foreach (PlayerController c in playerController)
            {
                if (c.view.IsMine && c.powerChosen == "StealCardPower")
                {
                    c.ClickOwnedPanelCardSteal(this, c);

                }
            }
        }

    }



    /* public void OnClickToPlaced()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.canPlay && c.actionsAvailable > 0 && c.hasClickedPlaceCards && value < 900)
            {
                c.FunctionForCardToPlace(this, c);
            }
        }
    } */


    public void OnClickSereia()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine)
            {
                if(cardImageFront.activeSelf == true && this.transform.parent.position != c.gameManager.panelMiddleCards.transform.position && this.transform.parent.parent.parent.gameObject.GetComponent<PlayerController>().view == c.view)
                {
                    if(buttonOceanya.activeSelf == false)
                    {
                        buttonOceanya.SetActive(true);
                    }
                    else
                    {
                        buttonOceanya.SetActive(false);
                    }
                }
            }
        }
    }
    public void OnClickPoluitrum()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine)
            {
                if (cardImageFront.activeSelf == true && this.transform.parent.position != c.gameManager.panelMiddleCards.transform.position && this.transform.parent.parent.parent.gameObject.GetComponent<PlayerController>().view == c.view)
                {
                    if (buttonPolutrum.activeSelf == false)
                    {
                        buttonPolutrum.SetActive(true);
                    }
                    else
                    {
                        buttonPolutrum.SetActive(false);
                    }
                }
            }
        }
    }

    public void OnClickXRay()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine)
            {
                if (cardImageFront.activeSelf == true && this.transform.parent.position != c.gameManager.panelMiddleCards.transform.position && this.transform.parent.parent.parent.gameObject.GetComponent<PlayerController>().view == c.view)
                {
                    if (buttonXRay.activeSelf == false)
                    {
                        buttonXRay.SetActive(true);
                    }
                    else
                    {
                        buttonXRay.SetActive(false);
                    }
                }
            }
        }
    }

    public void OnClickButtonOceanya()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.hasClickedPlaceCards == false && cardImageFront.activeSelf == true)
            {
                c.FunctionUseCard(c.view.ViewID, this.cardOwnedSlot, "Oceanya");
            }
        }
    }

    public void OnClickButtonPolutrum()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.hasClickedPlaceCards == false && cardImageFront.activeSelf == true)
            {
                c.FunctionUseCard(c.view.ViewID, this.cardOwnedSlot, "Polutrum");              
            }
        }
    }

    public void OnClickButtonXRay()
    {
        foreach (PlayerController c in playerController)
        {
            if (c.view.IsMine && c.hasClickedPlaceCards == false && cardImageFront.activeSelf == true)
            {
                //c.polutionPoints += 1;
                c.closeStealPanelButton.SetActive(false);
                c.FunctionUseCard(c.view.ViewID, this.cardOwnedSlot, "XRay");
                c.StealCardXRay(c.view.ViewID);
            }
        }
    }
}
