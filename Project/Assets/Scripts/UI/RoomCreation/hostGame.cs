using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class hostGame : MonoBehaviour {

    [SerializeField]
    private uint roomSizeMaximum = 8;

    private string roomName = "Room";

    private NetworkManager networkMan;

    private void Start()
    {
        networkMan = NetworkManager.singleton;
        if(networkMan.matchMaker == null)
        {
            networkMan.StartMatchMaker();
        }
    }

    public void SetRoomName(string _name) {
        roomName = _name;
    }

    public void CreateRoom() {
        if(roomName != "" && roomName != null)
        {
            Debug.Log("Creating room: " + roomName + " with " + roomSizeMaximum + " players");

            networkMan.matchMaker.CreateMatch(roomName, roomSizeMaximum, true, "", "", "", 0, 0, networkMan.OnMatchCreate);
        }
    }
}

// TODO documentate
