//------------------------------------------------------
//  Script responsable for hiding , showing and
//  managing pause and death menu
//------------------------------------------------------


using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.Networking;
using UnityEngine.Networking.Match;

public class PauseMenu : NetworkBehaviour
{

    [SerializeField] private GameObject pauseMenuInstance;
    [SerializeField] private KeyCode openMenuKey; // menu key

    [SerializeField] private Text respawnTimeText;  // Time to respawn text
    [SerializeField] private Button respawnButton; // Respawn button
    private bool respawnReady = true;   // Has time elapsed?

    PlayerManager localPlayer;

    private float timeSinceDeath;
    private float respawnTime;

   private  NetworkManager networkMan;

    private void Start()
    {
        respawnButton.enabled = false;  // Disable button click
        respawnTimeText.text = "Re-Spawn"; // Set default respawn text

        respawnTime = GameManager.instance.matchSettings.respawnTime;
        timeSinceDeath = respawnTime;

        pauseMenuInstance.SetActive(false);
        localPlayer = GameManager.GetPlayer(gameObject.transform.name);

        networkMan = NetworkManager.singleton;
    }

    private void Update()   // For pause menu
    {
        if (!isLocalPlayer)
            return;

        if (Input.GetKey(openMenuKey) && !localPlayer.isDead)  // If we press menu key and player is not dead
        {
            GetComponent<PlayerManager>().stopMovement();   // Stop player movement
            Show();
        }
        else if(pauseMenuInstance.activeSelf == true && !localPlayer.isDead)
        {      // Else
            Hide();
        }

        if (!respawnReady) {   // If player is dead, move respawn time clock
            if (timeSinceDeath >= 0)
            {
                timeSinceDeath -= Time.deltaTime;
                respawnTimeText.text = "Re-Spawn (" + Mathf.RoundToInt(timeSinceDeath) + ")";
            }
            else {
                respawnButton.enabled = true;  // Enable button click
                respawnTimeText.text = "Re-Spawn";
                respawnReady = true;
            }
        }  

    }

    public void Disconnect() {
        networkMan = NetworkManager.singleton;
        MatchInfo matchInfo = networkMan.matchInfo;
        if(matchInfo != null) { 
           // networkMan.matchMaker.DropConnection(matchInfo.networkId, matchInfo.nodeId, 0, networkMan.OnDropConnection);
            networkMan.StopHost();
        }
        else
        {
            Debug.Log("Couldn't disconnect from server; no match info found");
        }
    }

    public void Show()  // Show menu    (for death and for pause)
    {
        if (!isLocalPlayer)
            return;

        Cursor.lockState = CursorLockMode.None;
        Cursor.visible = true;
        respawnButton.enabled = false;  // Disable button click

        pauseMenuInstance.SetActive(true);

        if (localPlayer.isDead) {   // If player is dead, start minimum death cooldown clock.
            respawnReady = false;
        }
    }


    public void Hide() {    //  (for pause)
        if (!localPlayerAuthority)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuInstance.SetActive(false); // Hide menu
        GetComponent<PlayerManager>().resumeMovement();

    }

    public void playerRespawn() {   // (for death)
        if (!localPlayerAuthority)
            return;

        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
        pauseMenuInstance.SetActive(false);
        localPlayer.Respawn();
    }

}
