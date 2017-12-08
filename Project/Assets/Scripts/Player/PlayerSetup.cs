//------------------------------------------------------
//  Script responsable for setting up player parameters
//  when player is initialized.
//------------------------------------------------------

using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;
using UnityStandardAssets.Utility;

[RequireComponent(typeof(PlayerManager))]
public class PlayerSetup : NetworkBehaviour {

    [SerializeField] Behaviour[] componentsToDisable;   // Components to disable when not local player
    Camera sceneCamera;

    [SerializeField]string remoteLayerName = "RemotePlayer";    // Name of remote player graphics layer not to be drawn

    [SerializeField] GameObject playerGraphics; // Local player graphics not to be drawn locally
    [SerializeField] string dontDrawLayer = "DontDraw";

    [SerializeField] GameObject playerUIPrefab; // Player UI prefab
    private GameObject playerUIInstance;    // Player UI instance
   

    private void Start()
    {
        if (!isLocalPlayer) // If we're not controlling this object
        {
            DisableComponents();
            AssignRemoteLayer();
        }
        else
        {  // If we're local player
            sceneCamera = Camera.main;     // Save camera
            if (sceneCamera)
            {
                sceneCamera.gameObject.SetActive(false);    // And disable it
            }

            SetLayerRecursively(playerGraphics, LayerMask.NameToLayer(dontDrawLayer));  //Set local player graphics to dont draw layer

            playerUIInstance = Instantiate(playerUIPrefab); // Instantiate player UI
            playerUIInstance.name = playerUIPrefab.name;

            GameObject[] minimapCamera = GameObject.FindGameObjectsWithTag("minimapCamera");
            if (minimapCamera == null)
                Debug.LogError("No minimap camera found");
            else
                minimapCamera[0].GetComponent<FollowTarget>().target = gameObject.transform;
        }

        GetComponent<PlayerManager>().Setup();
    }

    private void SetLayerRecursively(GameObject _object, int _layer) {   // local player graphics layer

        _object.layer = _layer;
        foreach (Transform _child in _object.transform) {
            SetLayerRecursively(_child.gameObject, _layer);
        }

    }

    public override void OnStartClient()    // On player starts localy in network
    {
        base.OnStartClient();

        string _netID = GetComponent<NetworkIdentity>().netId.ToString();   // Get netID
        PlayerManager _player = GetComponent<PlayerManager>();  // Get player manager

        GameManager.RegisterPlayer(_netID, _player);    // Register player to game manager
    }

    private void DisableComponents() {
        for (int i = 0; i < componentsToDisable.Length; i++)    // Disable all components
        {
            componentsToDisable[i].enabled = false;
        }
    }


    private void AssignRemoteLayer() {  // Set remote layer for Physics raycasting purposes
        gameObject.layer = LayerMask.NameToLayer(remoteLayerName);
    }


    private void OnDisable()    // When player object is disabled
    {
        Destroy(playerUIInstance);

        if (sceneCamera != null)
            sceneCamera.gameObject.SetActive(true); // Reactivate camera

        GameManager.UnRegisterPlayer(transform.name);   //UnRegisters player from game manager
    }

}
