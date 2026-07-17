//AUTHOR Luka Brännlund
//
//Connects the local player into the session. The interface between the main menu and the lobby.


using System.Collections;
using System.Collections.Generic;
using Unity.Services.Core;
using Unity.Services.Authentication;
using Unity.Services.Relay;
using Unity.Services.Relay.Models;
using UnityEngine;
using Unity.Netcode;
using Unity.Netcode.Transports.UTP;
using System.Threading.Tasks;
using TMPro;

public class RelayManager : MonoBehaviour
{
    #region Singleton
        public static RelayManager Instance { get; private set; }
        
        private void Awake() 
        { 
            // If there is an instance, and it's not me, delete myself.
            
            if (Instance != null && Instance != this) 
            { 
                Destroy(this); 
            } 
            else 
            { 
                Instance = this; 
            } 
        }
    #endregion

    [SerializeField] private TMP_Text sessionCodeTMP;

    public bool isConnecting;
    private string currentJoinCode;
    public bool offlineMode;

    private async Task<bool> CreateRelay(){ //NO IDEA HOW THIS WORKS!!!!


        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try{
            Allocation allocation = await RelayService.Instance.CreateAllocationAsync(3);

            string joinCode = await RelayService.Instance.GetJoinCodeAsync(allocation.AllocationId);
            currentJoinCode = joinCode;

            Debug.Log("The current joining code is: " + currentJoinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetHostRelayData(
                allocation.RelayServer.IpV4,
                (ushort)allocation.RelayServer.Port,
                allocation.AllocationIdBytes,
                allocation.Key,
                allocation.ConnectionData
            );

            if (!NetworkManager.Singleton.StartHost())
            {

            }
            
            return true;
        }
        catch(RelayServiceException e){
            Debug.Log(e);
            return true;
        }
        
    }

    private async Task<bool> JoinRelay(string joinCode){ //NO IDEA HOW THIS WORKS!!!!


        await UnityServices.InitializeAsync();

        await AuthenticationService.Instance.SignInAnonymouslyAsync();

        try{
            JoinAllocation joinAllocation = await RelayService.Instance.JoinAllocationAsync(joinCode);

            NetworkManager.Singleton.GetComponent<UnityTransport>().SetClientRelayData(
                joinAllocation.RelayServer.IpV4,
                (ushort)joinAllocation.RelayServer.Port,
                joinAllocation.AllocationIdBytes,
                joinAllocation.Key,
                joinAllocation.ConnectionData,
                joinAllocation.HostConnectionData

            );

            if (!NetworkManager.Singleton.StartClient())
            {
                return false;
            }
            return true;
        }
        catch(RelayServiceException e){
            Debug.Log(e);
            return false;
        }

    }

    public void RelaySignOut()
    {
        currentJoinCode = "";
        //Console.Instance.ShowMessageInConsole("RelayScript.cs", "Signing out from relay service...");
        AuthenticationService.Instance.SignOut();
    }

    public async void AttemptJoining(string joinCode)
    {
        isConnecting = true;
        Debug.Log("Attempting to join using: " + joinCode);
        if (joinCode == "")
        {
            isConnecting = false;
            return;
        }

        if (!await JoinRelay(joinCode))
        {
            isConnecting = false;
            return;
        }

        while (NetworkManager.Singleton.IsListening == false)
        {
            await Task.Yield();
        }
        Lobby.Instance.JoinLobby();
        isConnecting = false;
    }

    public async void AttemptHosting()
    {
        isConnecting = true;

        if (offlineMode)
        {

            if (!NetworkManager.Singleton.StartHost())
            {
                isConnecting = false;
                return;
            }
        }
        else if (!await CreateRelay())
        {
            isConnecting = false;
            return;
        }

        while (NetworkManager.Singleton.IsListening == false)
        {
            await Task.Yield();
        }
        sessionCodeTMP.text = "Session code:" + GetJoinCode();

        Lobby.Instance.HostLobby();

        isConnecting = false;
    }

    public string GetJoinCode()
    {
        return currentJoinCode;
    }
}
