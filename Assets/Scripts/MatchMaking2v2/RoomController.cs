using Ilumisoft.TwinStickShooterKit;
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class RoomController : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        TurnOffPlayerCamera();
        TurnOffPlayerWeapon();
    }

    private void TurnOffPlayerWeapon()
    {
        WeaponComponent weaponComponent = GameObject.FindObjectOfType<WeaponComponent>();
        weaponComponent.enabled = false;
    }

    private void TurnOffPlayerCamera()
    {
        GameObject playerCamera = GameObject.Find("Follow Camera");
        playerCamera.SetActive(false);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
