using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using PlayFab;
using PlayFab.ClientModels;
using PlayFab.MultiplayerModels;
using Mirror;

public class PlayFabManager : MonoBehaviour
{

    public int money = 168;
    public NetworkManager networkManager;
    public Configuration configuration;
    public TelepathyTransport telepathyTransport;

    void Start()
    {
        if (configuration.buildType != BuildType.REMOTE_CLIENT) return;
        Login();
    }

    public void Login()
    {
        var request = new LoginWithCustomIDRequest
        {
            TitleId = PlayFabSettings.TitleId,
            CustomId = "Test User",
            CreateAccount = true
        };
        PlayFabClientAPI.LoginWithCustomID(request, OnLoginSuccess, OnLoginFailure);
    }

    void OnLoginSuccess(LoginResult result)
    {
        Debug.Log("Login successful!");
        GetPlayerData();

        if (configuration.ipAddress == "")
        {   //We need to grab an IP and Port from a server based on the buildId. Copy this and add it to your Configuration.
            RequestMultiplayerServer();
        }
        else
        {
            ConnectRemoteClient();
        }
    }

    void OnLoginFailure(PlayFabError error)
    {
        Debug.Log("Login failed");
        Debug.LogError(error.GenerateErrorReport());
    }

    public void GetPlayerData()
    {
        PlayFabClientAPI.GetUserData(new GetUserDataRequest(), OnDataReceived, OnError);
    }

    public void UpdatePlayerData()
    {
        var request = new UpdateUserDataRequest
        {
            Data = new Dictionary<string, string>{
                { "Money", money.ToString() }
            }
        };
        PlayFabClientAPI.UpdateUserData(request, OnDataUpdated, OnError);
    }

    void OnDataReceived(GetUserDataResult result)
    {
        Debug.Log("PlayerData received successfully");
        if (result.Data != null)
        {
            if (result.Data.ContainsKey("Money"))
            {
                Debug.Log("Money: " + result.Data["Money"].Value);
            }
            else
            {
                Debug.Log("PlayerData \"Money\" not found, creating new one...");
                UpdatePlayerData();
            }
        }
    }

    void OnDataUpdated(UpdateUserDataResult result)
    {
        Debug.Log("PlayerData updated successfully.");
    }

    void OnError(PlayFabError error)
    {
        Debug.Log(error.GenerateErrorReport());
    }

    private void RequestMultiplayerServer()
    {
        Debug.Log("RequestMultiplayerServer");
        RequestMultiplayerServerRequest requestData = new RequestMultiplayerServerRequest();
        requestData.BuildId = configuration.buildId;
        requestData.SessionId = System.Guid.NewGuid().ToString();
        requestData.PreferredRegions = new List<string>() { "EastUs" };
        PlayFabMultiplayerAPI.RequestMultiplayerServer(requestData, OnRequestMultiplayerServer, OnError);
    }

    private void OnRequestMultiplayerServer(RequestMultiplayerServerResponse response)
    {
        Debug.Log(response.ToString());
        ConnectRemoteClient(response);
    }

    private void ConnectRemoteClient(RequestMultiplayerServerResponse response = null)
    {
        if (response == null)
        {
            networkManager.networkAddress = configuration.ipAddress;
            telepathyTransport.port = configuration.port;
        }
        else
        {
            Debug.Log("**** ADD THIS TO YOUR CONFIGURATION **** -- IP: " + response.IPV4Address + " Port: " + (ushort)response.Ports[0].Num);
            networkManager.networkAddress = response.IPV4Address;
            telepathyTransport.port = (ushort)response.Ports[0].Num;
        }

        networkManager.StartClient();
    }

}
