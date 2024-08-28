using Cinemachine;
using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

public class SpawnManager : MonoBehaviour
{
    Vector3 spawnPosition;
    public CinemachineVirtualCamera virtualCamera; // Reference to the Cinemachine Virtual Camera
    // Start is called before the first frame update
    void Start()
    {
        float randomXPosition = Random.Range(-3, 3);
        float randomZPosition = Random.Range(-3, 3);
        spawnPosition = new(randomXPosition, 1, randomZPosition);
        GameObject player = PhotonNetwork.Instantiate("Player", spawnPosition, Quaternion.identity);

        // Set the Cinemachine camera to follow and look at the player
        if (virtualCamera.gameObject != null)
            StartCoroutine(AssignCamera(player));
    }

    // Coroutine to wait and then assign the camera to the player
    IEnumerator AssignCamera(GameObject player)
    {
        // Wait until the player has been instantiated completely
        yield return new WaitForEndOfFrame();
        
        // Set the Cinemachine camera to follow and look at the player
        virtualCamera.Follow = player.transform;
        virtualCamera.LookAt = player.transform;
    }
    // Update is called once per frame
    void Update()
    {
        
    }
}
