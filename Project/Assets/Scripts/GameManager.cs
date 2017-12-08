//------------------------------------------------------
//  Script responsable for managing all different
//  game behaviours and components.
//------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class GameManager : MonoBehaviour {

    #region Singleton
    public static GameManager instance = null;
    private void Awake()
    {
        if (instance == null)
            instance = this;
        else
            Destroy(gameObject);
    }
    #endregion


    #region Match Settings
    private MatchSettings _matchSettings = new MatchSettings();
    public MatchSettings matchSettings {
        get { return _matchSettings; }
        protected set {  _matchSettings = value; }
    }
    #endregion


    #region Player Instance tracking
    [SerializeField] private const string PLAYER_ID_PREFIX = "Player";

    private static Dictionary<string, PlayerManager> playerList = new Dictionary<string, PlayerManager>();  // Dictionary with all current players

    public static void RegisterPlayer(string _netID, PlayerManager _player) {  // Register a player
        string _playerID = PLAYER_ID_PREFIX + " " + _netID;   // Create player ID
        playerList.Add(_playerID, _player); // Save player and player ID
        _player.transform.name = _playerID; // Set player transform name to player ID
        Debug.Log(_playerID);
    }

    public static void UnRegisterPlayer(string _playerID) { // Un registers player from player list
        playerList.Remove(_playerID);
    }

    public static PlayerManager GetPlayer(string _playerID) {
        return playerList[_playerID];
    }

    #endregion

    

}
