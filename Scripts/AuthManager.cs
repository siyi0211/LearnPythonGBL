using System.Collections;
using UnityEngine;
using Firebase;
using Firebase.Auth;
using TMPro;
using UnityEngine.SceneManagement;
using System.IO;
using System;

public class AuthManager : MonoBehaviour
{
    //Firebase variables
    [Header("Firebase")]
    public DependencyStatus dependencyStatus;
    public FirebaseAuth auth;
    public FirebaseUser User;

    //Login variables
    [Header("Login")]
    public TMP_InputField emailLoginField;
    public TMP_InputField passwordLoginField;
    public TMP_Text warningLoginText;
    public TMP_Text confirmLoginText;

    //Register variables
    [Header("Register")]
    public TMP_InputField usernameRegisterField;
    public TMP_InputField emailRegisterField;
    public TMP_InputField passwordRegisterField;
    public TMP_InputField passwordRegisterVerifyField;
    public TMP_Text warningRegisterText;

    //Player
    public AllPlayerData allPlayerData;
    public PlayerData player;
    public PlayerData[] allPlayer;
    private string allPlayerDataFileName = "PlayerData.json";


    void Awake()
    {
        //Check that all of the necessary dependencies for Firebase are present on the system
        FirebaseApp.CheckAndFixDependenciesAsync().ContinueWith(task =>
        {
            dependencyStatus = task.Result;
            if (dependencyStatus == DependencyStatus.Available)
            {
                //If they are avalible Initialize Firebase
                InitializeFirebase();
            }
            else
            {
                Debug.LogError("Could not resolve all Firebase dependencies: " + dependencyStatus);
            }
        });
    }

    private void InitializeFirebase()
    {
        Debug.Log("Setting up Firebase Auth");
        //Set the authentication instance object
        auth = FirebaseAuth.DefaultInstance;
    }

    //Function for the login button
    public void LoginButton()
    {
        //Call the login coroutine passing the email and password
        StartCoroutine(Login(emailLoginField.text, passwordLoginField.text));
    }
    //Function for the register button
    public void RegisterButton()
    {
        //Call the register coroutine passing the email, password, and username
        StartCoroutine(Register(emailRegisterField.text, passwordRegisterField.text, usernameRegisterField.text));
    }

    private IEnumerator Login(string _email, string _password)
    {
        //Call the Firebase auth signin function passing the email and password
        var LoginTask = auth.SignInWithEmailAndPasswordAsync(_email, _password);
        //Wait until the task completes
        yield return new WaitUntil(predicate: () => LoginTask.IsCompleted);

        if (LoginTask.Exception != null)
        {
            //If there are errors handle them
            Debug.LogWarning(message: $"Failed to register task with {LoginTask.Exception}");
            FirebaseException firebaseEx = LoginTask.Exception.GetBaseException() as FirebaseException;
            AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

            string message = "Login Failed!";
            switch (errorCode)
            {
                case AuthError.MissingEmail:
                    message = "Missing Email";
                    break;
                case AuthError.MissingPassword:
                    message = "Missing Password";
                    break;
                case AuthError.WrongPassword:
                    message = "Wrong Password";
                    break;
                case AuthError.InvalidEmail:
                    message = "Invalid Email";
                    break;
                case AuthError.UserNotFound:
                    message = "Account does not exist";
                    break;
            }
            warningLoginText.text = message;
        }
        else
        {
            User = LoginTask.Result;

            if (User.IsEmailVerified)
            {
                yield return new WaitForSeconds(1f);
                //User is now logged in
                //Now get the result

                LoadAllPlayerData();

                player = GetCurrentPlayerData();

                Debug.LogFormat("User signed in successfully: {0} ({1})", User.DisplayName, User.Email);
                warningLoginText.text = "";
                confirmLoginText.text = "Logged In";
                SceneManager.LoadScene("MainMenuScene");

            }
            else
            {
                StartCoroutine(SendVerificationEmail());
            }
           
        }
    }

    private IEnumerator Register(string _email, string _password, string _username)
    {
        if (_username == "")
        {
            //If the username field is blank show a warning
            warningRegisterText.text = "Missing Username";
        }
        else if (passwordRegisterField.text != passwordRegisterVerifyField.text)
        {
            //If the password does not match show a warning
            warningRegisterText.text = "Password Does Not Match!";
        }
        else
        {
            //Call the Firebase auth signin function passing the email and password
            var RegisterTask = auth.CreateUserWithEmailAndPasswordAsync(_email, _password);
            //Wait until the task completes
            yield return new WaitUntil(predicate: () => RegisterTask.IsCompleted);

            if (RegisterTask.Exception != null)
            {
                //If there are errors handle them
                Debug.LogWarning(message: $"Failed to register task with {RegisterTask.Exception}");
                FirebaseException firebaseEx = RegisterTask.Exception.GetBaseException() as FirebaseException;
                AuthError errorCode = (AuthError)firebaseEx.ErrorCode;

                string message = "Register Failed!";
                switch (errorCode)
                {
                    case AuthError.MissingEmail:
                        message = "Missing Email";
                        break;
                    case AuthError.MissingPassword:
                        message = "Missing Password";
                        break;
                    case AuthError.WeakPassword:
                        message = "Weak Password";
                        break;
                    case AuthError.EmailAlreadyInUse:
                        message = "Email Already In Use";
                        break;
                }
                warningRegisterText.text = message;
            }
            else
            {
                //User has now been created
                //Now get the result
                User = RegisterTask.Result;

                if (User != null)
                {
                    //Create a user profile and set the username
                    UserProfile profile = new UserProfile { DisplayName = _username };

                    //Call the Firebase auth update user profile function passing the profile with the username
                    var ProfileTask = User.UpdateUserProfileAsync(profile);
                    //Wait until the task completes
                    yield return new WaitUntil(predicate: () => ProfileTask.IsCompleted);

                    if (ProfileTask.Exception != null)
                    {
                        //If there are errors handle them
                        Debug.LogWarning(message: $"Failed to register task with {ProfileTask.Exception}");
                        FirebaseException firebaseEx = ProfileTask.Exception.GetBaseException() as FirebaseException;
                        AuthError errorCode = (AuthError)firebaseEx.ErrorCode;
                        warningRegisterText.text = "Username Set Failed!";
                    }
                    else
                    {
                        //Username is now set
                        StartCoroutine(SendVerificationEmail());
                        //Now return to login screen
                        LoginUIManager.instance.LoginScreen();
                        warningRegisterText.text = "";
                        warningLoginText.text = "";
                        confirmLoginText.text = "";
                    }
                }
            }
        }
    }

    private IEnumerator SendVerificationEmail()
    {
        if (User != null)
        {
            var emailTask = User.SendEmailVerificationAsync();

            yield return new WaitUntil(predicate: () => emailTask.IsCompleted);

            if (emailTask.Exception != null)
            {
                FirebaseException firebaseException = (FirebaseException)emailTask.Exception.GetBaseException();
                AuthError error = (AuthError)firebaseException.ErrorCode;

                string output = "Unknow error, please try again";

                switch (error)
                {
                    case AuthError.Cancelled:
                        output = "Verification Task was cancelled";
                        break;
                    case AuthError.InvalidRecipientEmail:
                        output = "Invalid Email";
                        break;
                    case AuthError.TooManyRequests:
                        output = "Too many request";
                        break;
                }

                LoginUIManager.instance.AwaitVerification(false, User.Email, output);
            }
            else
            { 
                LoginUIManager.instance.AwaitVerification(false, User.Email, null);
                Debug.Log("Email sent successfully");
            }
        }
    }

    public void LoadAllPlayerData()
    {
        // Path.Combine combines strings into a file path
        // Application.StreamingAssets points to Assets/StreamingAssets in the Editor, and the StreamingAssets folder in a build
        string filePath = Path.Combine(Application.streamingAssetsPath, allPlayerDataFileName);

        if (File.Exists(filePath))
        {
            // Read the json from the file into a string
            string dataAsJson = File.ReadAllText(filePath);
            // Pass the json to JsonUtility, and tell it to create a GameData object from it
            allPlayerData = JsonUtility.FromJson<AllPlayerData>(dataAsJson);

            // Retrieve the allRoundData property of loadedData
            if (allPlayerData == null)
            {
                allPlayerData = new AllPlayerData();
                Debug.Log("loaded data: " + allPlayerData.ToString());
                allPlayer = new PlayerData[] { };

            }
            else
            {
                if (allPlayer == null)
                {
                    allPlayer = new PlayerData[] { };
                }
                else
                { 
                    allPlayer = allPlayerData.allPlayerData;
                }
                
            }

            Debug.Log("All PLayer: " + allPlayer.ToString());
        }
        else
        {
            Debug.LogError("Cannot load player data!");
        }

    }

    public PlayerData GetCurrentPlayerData()
    {
        Debug.Log("All PLayer length: " + allPlayer.Length);

        if (allPlayer.Length != 0)
        {
            for (int i = 0; i < allPlayer.Length; i++)
            {
                string PlayerEmail = allPlayer[i].email.ToLower();

                if (PlayerEmail.Equals(User.Email.ToLower()))
                {
                    PlayerID.playerID = allPlayer[i].id;
                    return allPlayer[i];
                }

            }

        }

        //create new player

        int playerID = allPlayer.Length;
        string playerName = User.DisplayName;

        int coin = 0;
        int level = 1;
        int exp = 0; //experience point

        int[] characterOwned = { 0}; // array of owned character id
        int characterUsed = 0; //0 as default

        string icon = "default"; //icon image path

        TutorialHighestScore[] noteHighestScoresList = { };

        //PlayerData NewPlayer = new PlayerData(PlayerID, PlayerName, User.Email.ToLower(), coin, level, exp, characterOwned, characterUsed, icon, noteHighestScoresList);

        Array.Resize(ref allPlayer, allPlayer.Length + 1);
        allPlayer[allPlayer.Length - 1] = new PlayerData(playerID, playerName, User.Email.ToLower(), coin, level, exp, characterOwned, characterUsed, icon, noteHighestScoresList);

        Debug.Log("new player: " + allPlayer[allPlayer.Length - 1].name);
        saveNewPlayerData();

        PlayerID.playerID = allPlayer[allPlayer.Length - 1].id;

        return allPlayer[allPlayer.Length -1] ;

    }

    public void saveNewPlayerData()
    { 
        string filePath = Path.Combine(Application.streamingAssetsPath, allPlayerDataFileName);

        if (File.Exists(filePath))
        {
            allPlayerData.allPlayerData = allPlayer;
            string playerJSON = JsonUtility.ToJson(allPlayerData);
            Debug.Log("JSON: " + playerJSON);
            File.WriteAllText(filePath, playerJSON);
        }
        else
        {
            Debug.LogError("Cannot find player data to save data!");
        }
    }
}
