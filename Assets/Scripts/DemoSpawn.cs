using Photon.Pun;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DemoSpawn : MonoBehaviour
{
    Vector3 spawnPosition;
    // Start is called before the first frame update
    void Start()
    {
        float randomXPosition = Random.Range(-3, 3);
        float randomZPosition = Random.Range(-3, 3);
        spawnPosition = new(randomXPosition, 1, randomZPosition);
        PhotonNetwork.Instantiate("MyRobotKyle", spawnPosition, Quaternion.identity);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
