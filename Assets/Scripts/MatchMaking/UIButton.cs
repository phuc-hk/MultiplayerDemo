using System;
using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
//using UnityEngine.UIElements;

public class UIButton : MonoBehaviour
{
    public Button buttonReady;
    // Start is called before the first frame update
    void Start()
    {
        buttonReady.onClick.AddListener(Ready);
    }

    private void Ready()
    {
        MatchMaking.Instance.SetPlayerReady();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
