using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking.Match;
using UnityEngine.Networking;

public class roomListItem : MonoBehaviour {

    public delegate void JoinGameDelegate(MatchInfoSnapshot _match);
    public JoinGameDelegate joinRoomDelegate;

    [SerializeField]private Text roomListNameText;

    private MatchInfoSnapshot match;

    public void Setup(MatchInfoSnapshot _match, JoinGameDelegate _joinRoomCallback) {
        match = _match;

        joinRoomDelegate = _joinRoomCallback;

        roomListNameText.text = match.name + "    (" + match.currentSize + " / " + match.maxSize + ")";
    }

    public void JoinRoomCall() {
        joinRoomDelegate.Invoke(match);
    }


}

//TODO description
