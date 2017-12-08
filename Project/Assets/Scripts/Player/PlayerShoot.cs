//------------------------------------------------------
//  Script responsable for managing user fire input
//  and managing shooting mechanics.
//------------------------------------------------------

using UnityEngine;
using UnityEngine.Networking;

[RequireComponent(typeof(weaponManager))]
public class PlayerShoot : NetworkBehaviour {

    // SHOOTING --
    [SerializeField]private LayerMask hitMask;   // What will the bullet collide with
    [SerializeField]private Camera theCamera;   // Camera from where we will shoot
    private const string playerTag = "Player";  // Player tag for collision detection

    //WEAPONS ---
    private weaponManager weaponMan;    // Weapon manager
    private Weapon currentWeapon;    // Selected weapon

    private void Start()
    {

        if (theCamera == null) {    // Fi we do not have a camera referenced
            Debug.LogError("PlayerShoot:  No camera referenced!");  // Quit
            this.enabled = false;
        }
        weaponMan = GetComponent<weaponManager>();  // Get weapon manager component
        currentWeapon = weaponMan.getCurrentWeapon();
    }

    private void Update()   
    {
        currentWeapon = weaponMan.getCurrentWeapon();   // Update current weapon
        if (currentWeapon.fireRate == 0)
        { // If fire of rate = 0 (single shot mode)
            if (Input.GetButtonDown("Fire1"))
            { // Get input from user to shoot once

                Shoot();
            }
        } else {
            if (Input.GetButtonDown("Fire1"))   // On click
            {
                InvokeRepeating("Shoot" , 0f, currentWeapon.fireRate);  // invoke repeating (w/ fire rate)
            }else if (Input.GetButtonUp("Fire1"))   // If we stop pressing fire button
            {
                CancelInvoke("Shoot");  // Cancel repeated shoot invoking
            }
        }

    }

    [Command]
    private void CmdOnShoot()   // Called on server when a player shoots
    {
        RpcDoShootEffects();    // Shoot flash effects played
    }

    [ClientRpc] 
    private void RpcDoShootEffects() {  // Its called on all clients when player shoots to display shoot effects
        weaponMan.getCurrentWeaponGraphics().muzzleFlash.Play();
    }

    [Command]
    private void CmdOnHit(Vector3 _pos, Vector3 _normal){       // Called on server when something is shot (Takes in hit point and normal of surface)
        RpcDoHitEffect(_pos, _normal);  
    }

    [ClientRpc]
    private void RpcDoHitEffect(Vector3 _pos, Vector3 _normal)   // Its called on all clients when something is shot    (Takes in hit point and normal of surface)
    {
        GameObject _ins = (GameObject)Instantiate(weaponMan.getCurrentWeaponGraphics().hitEffect, _pos, Quaternion.LookRotation(_normal));    // Hit effects spawned
        Destroy(_ins, 2); // Destroy effect after 2 seconds

    }



    [Client]
    private void Shoot() {  // Shoot only on client
        if (!isLocalPlayer) // If we're the local player shooting
            return;

        CmdOnShoot();   // Call server to tell player has shot 

        weaponMan.getCurrentWeaponGraphics().muzzleFlash.Play();

        RaycastHit hit;
        if (Physics.Raycast(theCamera.transform.position, theCamera.transform.forward, out hit, currentWeapon.range, hitMask)) {    // Send raycast
            if (hit.collider.tag == playerTag) { // If what we have hit is a player
                CmdPlayerShot(hit.collider.name, currentWeapon.damage, currentWeapon.name, hit.distance);   // Send info to server about shot
            }
            CmdOnHit(hit.point, hit.normal);    // Calls server to instantiate on all clients hit effects
        }
    }

    [Command]   // Send info to server about a player being shot
    void CmdPlayerShot(string _playerID, int _damage, string _weaponName, float _distance) {  // Tells server a player has been shot
        Debug.Log(_playerID + " has been shot with a " + _weaponName + " at " + _distance + "m");

        PlayerManager _player = GameManager.GetPlayer(_playerID);
        _player.RpcTakeDamage(_damage);
    }
}
