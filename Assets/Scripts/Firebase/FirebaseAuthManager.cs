using Firebase.Auth;
using UnityEngine;
using System.Threading.Tasks;
using Firebase.Extensions;
using UnityEngine.SceneManagement;

public class FirebaseAuthManager : MonoBehaviour
{
    private FirebaseAuth auth;

    void Start()
    {
        // Initialize Firebase Authentication
        auth = FirebaseAuth.DefaultInstance;
        //SignInAnonymously();
    }

    // Method to sign in anonymously
    public void SignInAnonymously()
    {
        auth.SignInAnonymouslyAsync().ContinueWithOnMainThread(task =>
        {
            if (task.IsFaulted)
            {
                Debug.LogError("Anonymous sign-in failed: " + task.Exception);
            }
            else if (task.IsCompleted)
            {
                AuthResult newUser = task.Result;
                Debug.Log("Signed in anonymously successfully. User ID: " + newUser.AdditionalUserInfo);
                SceneManager.LoadScene("StartScene");
                //SceneManager.LoadScene("2MainScene");
                // Optionally, you can handle additional logic after successful sign-in here
            }
        });
    }
}
