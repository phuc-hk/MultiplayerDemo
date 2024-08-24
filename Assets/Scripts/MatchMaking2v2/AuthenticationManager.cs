using Firebase;
using Firebase.Auth;
using Firebase.Extensions;
using Photon.Pun;
using Photon.Realtime;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.SceneManagement;

public class AuthenticationManager : MonoBehaviour
{
    public static AuthenticationManager Instance { get; private set; }

    // Firebase Auth variables
    private FirebaseAuth auth;
    private FirebaseUser currentUser;
    public string userId { get; private set; }
    //public UnityEvent<string> OnSignIn;

    void Awake()
    {
        if (Instance == null)
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }
        else
        {
            Destroy(gameObject);
        }
    }


    void Start()
    {
        auth = FirebaseAuth.DefaultInstance;
        //// Initialize Firebase Auth
        //FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        //{
        //    if (task.Result == DependencyStatus.Available)
        //    {
        //        auth = FirebaseAuth.DefaultInstance;
        //        Debug.Log("Check OK");
        //        SignInAnonymously();
        //    }
        //    else
        //    {
        //        Debug.LogError("Could not resolve all Firebase dependencies: " + task.Result);
        //    }
        //});
    }

    // Anonymous Sign-In
    public void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsCanceled || task.IsFaulted)
            {
                Debug.LogError("Anonymous sign-in failed: " + task.Exception);
                return;
            }

            // Sign-in successful
            currentUser = task.Result.User;
            Debug.Log("Anonymous User signed in: " + currentUser.UserId);

            // Call DatabaseManager to add the player
            //string userId = currentUser.UserId;
            userId = "PlayerId" + Random.Range(1, 10000).ToString();
            DatabaseManager.Instance.AddPlayerToDatabase(userId);
            Debug.Log("Photon user Id before any change" + PhotonNetwork.LocalPlayer.UserId);
            PhotonManager.Instance.SetCurrentUser(userId);
            Debug.Log("Photon user Id" + PhotonNetwork.LocalPlayer.UserId);
            Debug.Log("Photon user Id after change" +  PhotonNetwork.LocalPlayer.CustomProperties["FirebaseUserId"]);
            SceneManager.LoadScene("1MainScene");
            //OnSignIn.Invoke(userId);

        });
    }

    // Getter for FirebaseUser
    public FirebaseUser GetCurrentUser()
    {
        return currentUser;
    }
}
