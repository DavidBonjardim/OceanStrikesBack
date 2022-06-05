using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class BackToLobby : MonoBehaviourPunCallbacks
{
    // Start is called before the first frame update
    /* public void OnClickBackToLobby()
    {
        if(PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        SceneManager.LoadScene("Lobby");
    } */

    public void LeaveRoom()
    {
        if (PhotonNetwork.CurrentRoom != null)
        {
            PhotonNetwork.LeaveRoom();
        }
        else
        {
            //SceneManager.LoadScene("Lobby");
            PhotonNetwork.ConnectUsingSettings();
        }
    }

    public override void OnLeftRoom()
    {
        //SceneManager.LoadScene("Lobby");
        PhotonNetwork.ConnectUsingSettings();
        base.OnLeftRoom();
    }

    public override void OnConnectedToMaster()
    {
        SceneManager.LoadScene("Lobby");
    }
}
