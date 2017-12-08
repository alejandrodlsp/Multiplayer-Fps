using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Networking;

public class weaponManager : NetworkBehaviour {

    [SerializeField] private string weaponLayerName = "Weapon";

    [SerializeField] private Weapon primaryWeapon;
    [SerializeField] private Weapon secondaryWeapon;

    [SerializeField] private Transform weaponHolder;

    private Weapon currentWeapon;
    private WeaponGraphics currentWeaponGraphics;

	void Start () {
        EquipWeapon(primaryWeapon);
	}

    void EquipWeapon(Weapon _weapon) {
        currentWeapon = _weapon;
        GameObject _weaponIns = (GameObject) Instantiate(_weapon.graphics, weaponHolder.position, weaponHolder.rotation);
        _weaponIns.transform.SetParent(weaponHolder);


        currentWeaponGraphics = _weaponIns.GetComponent<WeaponGraphics>();
        if (currentWeaponGraphics == null)
            Debug.LogError("No weapon graphics component in current weapon");


        if (isLocalPlayer)
        {
            Utility.SetLayerRecursively(_weaponIns, LayerMask.NameToLayer(weaponLayerName));
        }
    }

    public Weapon getCurrentWeapon() {
        return currentWeapon;
    }
    public WeaponGraphics getCurrentWeaponGraphics()
    {
        return currentWeaponGraphics;
    }


}//TODO: documentate document
