using System;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;

public static class PlayfabAuth
{
    // Const - Save email/password
    public const string PlayfabAuthPlayerPrefsKeyUsername = "playfab_auth_username";
    public const string PlayfabAuthPlayerPrefsKeyEmail = "playfab_auth_email";
    public const string PlayfabAuthPlayerPrefsKeyPassword = "playfab_auth_password";

    // Getter
    public static bool IsLoggedIn
    {
        get
        {
            return PlayFabClientAPI.IsClientLoggedIn();
        }
    }

    // Functions
    public static void TryRegisterWithEmail(string email, string password, Action registerResultCallback, Action errorCallback)
    {
        PlayfabAuth.TryRegisterWithEmail(email, password, email, registerResultCallback, errorCallback);
    }

    public static void TryRegisterWithEmail(string email, string password, string username, Action registerResultCallback, Action errorCallback)
    {
        var request = new RegisterPlayFabUserRequest
        {
            Email = email,
            Password = password,
            Username = username
        };

        PlayFabClientAPI.RegisterPlayFabUser(request, result =>
        {
            // Registration success
            registerResultCallback.Invoke();
        }, error =>
        {
            // Registration error
            errorCallback.Invoke();
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    public static void TryLoginWithEmail(string email, string password, Action loginResultCallback, Action errorCallback)
    {
        var request = new LoginWithEmailAddressRequest
        {
            Email = email,
            Password = password
        };

        PlayFabClientAPI.LoginWithEmailAddress(request, result =>
        {
            // Login success
            loginResultCallback.Invoke();
        }, error =>
        {
            // Login error
            errorCallback.Invoke();
            Debug.LogError(error.GenerateErrorReport());
        });
    }

    // Logout
    public static void Logout(Action logoutResultCallback, Action errorCallback)
    {
        PlayFabClientAPI.ForgetAllCredentials();

        // Clear all keys from PlayerPrefs
        PlayerPrefs.DeleteKey(PlayfabAuth.PlayfabAuthPlayerPrefsKeyUsername);
        PlayerPrefs.DeleteKey(PlayfabAuth.PlayfabAuthPlayerPrefsKeyEmail);
        PlayerPrefs.DeleteKey(PlayfabAuth.PlayfabAuthPlayerPrefsKeyPassword);

        // Callback
        logoutResultCallback.Invoke();
    }
}