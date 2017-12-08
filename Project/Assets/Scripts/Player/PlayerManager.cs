//------------------------------------------------------
//  Script responsable for managing all different
//  player components and behaviours, as well as 
//  storing player information.
//------------------------------------------------------

using System.Collections;
using UnityEngine;
using UnityEngine.Networking;

public class PlayerManager : NetworkBehaviour {

    [SyncVar]
    private bool _isDead = false;   // private is dead boolead
    public bool isDead {            // Is dead setter and getter
        get { return _isDead; }
        protected set { _isDead = value; }
    }

    [SerializeField] private int maxHealth = 100; 

    [SyncVar] private int currentHealth;

    [SerializeField] private Behaviour[] disableOnPauseComponents;  // All components to be disabled on pause (disabled as well on death)
    [SerializeField] private GameObject[] disableOnDeathComponents;  // All components to be disabled exclusivelly on death

    private bool[] wasEnabledDeath;  // Stores what components were enabled on death
    Collider playerCollider;    // Player collider


    public void Setup() // Player spawn setup
    {
        wasEnabledDeath = new bool[disableOnPauseComponents.Length]; // Define wasenabled with disableOnDeathComponents length
        for (int i = 0; i < wasEnabledDeath.Length; i++) 
        {
            wasEnabledDeath[i] = disableOnPauseComponents[i].enabled;    // Sets all wasEnabled values to corresponding component value
        }

        setDefaults();  // Sets default values
    }

    private void Update()
    {
        // Temporal suicide feature
        if (Input.GetKeyDown(KeyCode.K) && isLocalPlayer)
        {
            RpcTakeDamage(9999);
        }
    }


    #region health and lives

    [ClientRpc] // makes sure method is called on all clients
    public void RpcTakeDamage(int _damage) {
        if (isDead) // If we are alive
            return;

        currentHealth -= _damage;

        if (currentHealth <= 0 && localPlayerAuthority) // If we're dead and we're local player, start dead sequence
        {
            Die();
        }

        Debug.Log(transform.name + " now has " + currentHealth + " / " + maxHealth);
    }

    private void Die() {

        isDead = true;  // Set is dead to true

        Debug.Log(transform.name + " IS DEAD!");

        CmdDisableMeshrenderers();  // Disable player mesh renderers on ALL CLIENTS

       if (isLocalPlayer)   // We show death menu (Wont work if we're not local player, as only local player has pause menu component)
       {
            stopMovement(); // We stop the player from moving
            GetComponent<PauseMenu>().Show();
       }

    }

                                    
    public void Respawn() { // Respawn method
        isDead = false; // Set is dead to false

        Transform _spawnPoint = NetworkManager.singleton.GetStartPosition();    // Get spawn position from network manager
        transform.position = _spawnPoint.position;  // respawn
        transform.rotation = _spawnPoint.rotation;

        setDefaults(); // Set default value (health, etc)

        CmdEnableMeshRenderers();   // Enables ALL CLIENTS player mesh renderers
        resumeMovement();   // Resume movements
        Debug.Log(transform.name + " respawned");
    }

    public void setDefaults()   // Set default values to player after death
    {
        isDead = false;
        currentHealth = maxHealth;

        playerCollider = GetComponent<Collider>();  // Special collider case
        if (playerCollider != null)
            playerCollider.enabled = true;

    }

    #endregion


    #region controls and movement

    [Command]
    private void CmdDisableMeshrenderers() {
        RpcDisableMeshRenderers();
    }

    [ClientRpc]
    private void RpcDisableMeshRenderers() {    // Disables dead player mesh renderers and colliders on ALL CLIENTS
        playerCollider.enabled = false;
        foreach (GameObject _beh in disableOnDeathComponents)    // Disables all player onDeath componentes
        {
            _beh.SetActive(false);
        }
    }

    [Command]
    private void CmdEnableMeshRenderers() {
        RpcEnableMeshRenderers();
    }

    [ClientRpc]
    private void RpcEnableMeshRenderers() {     // Enables dead player mesh renderers and colliders on ALL CLIENTS
        playerCollider.enabled = true;
        foreach (GameObject _beh in disableOnDeathComponents)   // Enable all player onDeath componentes
        {
            _beh.SetActive(true);
        }
    }

    public void stopMovement()
    {
        for (int i = 0; i < disableOnPauseComponents.Length; i++)   // Set all components to disabled value
        {
            disableOnPauseComponents[i].enabled = !wasEnabledDeath[i];
        }
    }

    public void resumeMovement()
    {
        for (int i = 0; i < disableOnPauseComponents.Length; i++)   // Set all components to enabled value
        {
            disableOnPauseComponents[i].enabled = wasEnabledDeath[i];
        }
    }
    #endregion
}
