using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Realtime;
using Photon.Pun;
using TMPro;

public class LobbyNetworkManager : MonoBehaviourPunCallbacks
{
    public static LobbyNetworkManager Instance;
    [SerializeField] private TMP_Text waitBattleText;

    private void Awake()
    {
        Instance = this;
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
        WindowsManager.Layout.OpenLayout("Loading");
    }

    void Update()
    {
        
    }

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
    }

    public override void OnJoinedLobby()
    {
        WindowsManager.Layout.OpenLayout("MainMenu");
    }

    public void ToBattleButton()
    {
        WindowsManager.Layout.OpenLayout("AutomaticBattle");
        PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinRandomFailed(short returnCode, string message)
    {
        if(returnCode == (short)ErrorCode.NoRandomMatchFound)
        {
            waitBattleText.text = "No matches have been found. We are creating a new room";
            CreateNewRoom();
        }
    }

    private string RoomNameGenerator()
    {
        short codeLength = 12;
        string roomCode = null;
        for (short i = 0; i < codeLength; i++)
        {
            char symbol = (char)Random.Range(65, 91);
            roomCode += symbol;
        }
        return roomCode;
    }

    private void CreateNewRoom()
    {
        RoomOptions currentRoom = new RoomOptions();
        currentRoom.IsOpen = true;
        currentRoom.MaxPlayers = 2;
        PhotonNetwork.CreateRoom(RoomNameGenerator(), currentRoom);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        if(returnCode == (short)ErrorCode.GameIdAlreadyExists)
        {
            CreateNewRoom();
        }
    }

    public override void OnCreatedRoom()
    {
        waitBattleText.text = "Waiting for the second player.";
    }

    public override void OnJoinedRoom()
    {
        PhotonNetwork.AutomaticallySyncScene = true;
        if (PhotonNetwork.IsMasterClient) return;
        waitBattleText.text = "The battle is about to begin !";
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        if (!PhotonNetwork.IsMasterClient)
        {
            return;
        }

        Room currentRoom = PhotonNetwork.CurrentRoom;
        currentRoom.IsOpen = false;
        waitBattleText.text = "The battle begins ! Get ready !";
        Invoke("LoadingGameMap", 3f);
    }

    private void LoadingGameMap()
    {
        PhotonNetwork.LoadLevel(1);
    }

    public void StopFindingBattleButton()
    {
        PhotonNetwork.LeaveRoom();
    }

    public override void OnLeftRoom()
    {
        WindowsManager.Layout.OpenLayout("MainMenu");
    }

}
