using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class LeaveRoomAfterClientChange : MonoBehaviour
{

    public void OnClickLeaveRoom()
    {
        SceneManager.LoadScene("Lobby");
    }

}
