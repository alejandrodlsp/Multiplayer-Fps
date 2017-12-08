using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class JoinGame : MonoBehaviour {

	private NetworkManager networkMan;
    private List<GameObject> roomList = new List<GameObject>();

    [SerializeField] private Text statusText;
    [SerializeField] private GameObject roomlistItemPrefab;

    [SerializeField] private Transform roomListParent;

    private void Start()
    {
        refreshNetMan();
        RefreshRoomList();
    }

    private void refreshNetMan() {
        networkMan = NetworkManager.singleton;
        if (networkMan.matchMaker != null)
        {
            networkMan.StopMatchMaker();
            networkMan.StartMatchMaker();
        }
    }
    public void RefreshRoomList() {
        refreshNetMan();
        ClearRooms();
        networkMan.matchMaker.ListMatches(0, 20, "", false, 0, 0,  OnMatchList);
        statusText.text = "Loading...";
    }

    public void OnMatchList(bool success, string extendedInfo, List<MatchInfoSnapshot> matches) {
        statusText.text = "";

        if (!success)
        {
            statusText.text = "Couldn't retrieve match list";
            return;
        }

        ClearRooms();

        foreach(MatchInfoSnapshot _inf in matches)
        {
            GameObject _roomListItem = (GameObject) Instantiate(roomlistItemPrefab);
            _roomListItem.transform.SetParent(roomListParent);


            roomListItem roomLItem = _roomListItem.GetComponent<roomListItem>();
            if (roomLItem != null)
                roomLItem.Setup(_inf, joinRoom);

            roomList.Add(_roomListItem);
        }

        if (roomList.Count == 0)
            statusText.text = "No games found.";
        else
            statusText.text = "";
    }

    public void joinRoom(MatchInfoSnapshot _match) {
        networkMan.matchMaker.JoinMatch(_match.networkId, "", "", "", 0, 0, networkMan.OnMatchJoined);
    }

    private void ClearRooms() {
        for (int i = 0; i < roomList.Count; i++)
        {
            Destroy(roomList[i]);
        }

        roomList.Clear();
    }
}
//TODO documentate