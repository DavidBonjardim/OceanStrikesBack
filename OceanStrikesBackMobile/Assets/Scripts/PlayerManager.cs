using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;

public class PlayerManager : MonoBehaviour
{

    PhotonView PV;

    GameManager gameManager;

    private void Awake()
    {
        PV = GetComponent<PhotonView>();
        gameManager = GameObject.Find("GameManager").GetComponent<GameManager>();
    }

    // Start is called before the first frame update
    void Start()
    {
        if (PV.IsMine)
        {
            CreateController();
        }       
    }

    void CreateController()
    {
        //GameObject canvas = GameObject.FindGameObjectWithTag("Canvas");
        GameObject a = PhotonNetwork.Instantiate("PlayerController", Vector3.zero, Quaternion.identity);
        //a.transform.SetParent(canvas.transform);

        gameManager.playerControllersInGameList.Add(a.GetComponent<PlayerController>());

    }

}
