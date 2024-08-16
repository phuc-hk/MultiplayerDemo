using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Chat;
using ExitGames.Client.Photon;
using Photon.Pun;
using UnityEngine.UIElements;
using TMPro;
using UnityEditor.VersionControl;

public class DemoPhotonChat : MonoBehaviour, IChatClientListener
{
    private bool isConnected = false;
    private ChatClient chatClient;
    public ScrollView scrollView;
    public GameObject messagePrefab;
    public TMP_InputField inputField;
    public string chatChannelName = "global";
    public GameObject content;

    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {
        if (isConnected == true)
        {
            chatClient.Service();
        }
    }

    public  void JoinChat()
    {
        if (!isConnected)
        {
            chatClient = new ChatClient(this);
            chatClient.Connect(PhotonNetwork.PhotonServerSettings.AppSettings.AppIdChat,
                               PhotonNetwork.AppVersion,
                               new AuthenticationValues("Phuc" + Random.Range(1, 1000).ToString()));
            isConnected = true;
            Debug.Log(chatClient.CanChat);
        }
        else
        {
            chatClient.Disconnect();
            isConnected = false;
        }
    }

    public void SendMessage()
    {
        if (inputField.text != "")
        {
            chatClient.PublishMessage(chatChannelName, inputField.text);
            inputField.text = "";
        }

    }

    public void DebugReturn(DebugLevel level, string message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnChatStateChange(ChatState state)
    {
        //throw new System.NotImplementedException();
    }

    public void OnConnected()
    {
        //string[] channels = { "All", "Team" };
        //chatClient.Subscribe(channels);
        chatClient.Subscribe(new string[] { chatChannelName });
    }

    public void OnDisconnected()
    {
        //throw new System.NotImplementedException();
    }

    public void OnGetMessages(string channelName, string[] senders, object[] messages)
    {
        //throw new System.NotImplementedException();
        for (int i = 0; i < messages.Length; i++)
        {
            GameObject message = Instantiate(messagePrefab, Vector3.zero, Quaternion.identity, content.transform);
            message.GetComponent<TextMeshProUGUI>().text = senders[i] + ": " + messages[i];
        }
    }

    public void OnPrivateMessage(string sender, object message, string channelName)
    {
        //throw new System.NotImplementedException();
    }

    public void OnStatusUpdate(string user, int status, bool gotMessage, object message)
    {
        //throw new System.NotImplementedException();
    }

    public void OnSubscribed(string[] channels, bool[] results)
    {
        for (int i = 0; i < channels.Length; i++)
        {
            Debug.Log("OnSubscribed: " + channels[i]);
        }
    }

    public void OnUnsubscribed(string[] channels)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserSubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    public void OnUserUnsubscribed(string channel, string user)
    {
        //throw new System.NotImplementedException();
    }

    
}
