using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Photon.Pun;
using Photon.Realtime;

public class Launcher : MonoBehaviourPunCallbacks
{
    public static Launcher instance;

    [SerializeField] InputField usernameInput;
    [SerializeField] InputField roomName;
    [SerializeField] Text roomNameText;
    [SerializeField] Text errorText;

    [Header("Room List")]
    [SerializeField] Transform roomListContent;
    [SerializeField] GameObject roomListItemPrefab;

    [Header("Player List")]
    [SerializeField] Transform playerListContent;
    [SerializeField] GameObject playerListItemPrefab;

    [Header("Start Button")]
    [SerializeField] GameObject startButton;

    void Awake()
    {
        if (instance == null)
        {
            instance = this;
        }
    }

    void Start()
    {
        PhotonNetwork.ConnectUsingSettings();
    }

    #region Photon Callbacks

    public override void OnConnectedToMaster()
    {
        PhotonNetwork.JoinLobby();
        PhotonNetwork.AutomaticallySyncScene = true;
    }

    public override void OnJoinedLobby()
    {
        MenuManager.instance.OpenMenu("MainMenu");

        if (PlayerPrefs.HasKey("username"))
        {
            usernameInput.text = PlayerPrefs.GetString("username");
            OnUsernameInputValueChanged();
        }
        else
        {
            usernameInput.text = "Player" + Random.Range(0, 10000).ToString("0000");
            OnUsernameInputValueChanged();
        }

        // PhotonNetwork.JoinRandomRoom();
    }

    public override void OnJoinedRoom()
    {
        roomNameText.text = PhotonNetwork.CurrentRoom.Name;
        MenuManager.instance.OpenMenu("RoomMenu");

        Player[] players = PhotonNetwork.PlayerList;

        foreach (Transform child in playerListContent)
        {
            Destroy(child.gameObject);
        }

        foreach (Player player in players)
        {
            Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(player);
        }

        if (players.Length >= 8)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        else
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }

        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnPlayerLeftRoom(Player otherPlayer)
    {
        Player[] players = PhotonNetwork.PlayerList;

        if (players.Length >= 8)
        {
            PhotonNetwork.CurrentRoom.IsVisible = false;
        }
        else
        {
            PhotonNetwork.CurrentRoom.IsVisible = true;
        }
    }

    public override void OnMasterClientSwitched(Player newMasterClient)
    {
        startButton.SetActive(PhotonNetwork.IsMasterClient);
    }

    public override void OnCreateRoomFailed(short returnCode, string message)
    {
        errorText.text = "Room creation failed: " + message;
        MenuManager.instance.OpenMenu("ErrorMenu");
    }

    public override void OnLeftRoom()
    {
        MenuManager.instance.OpenMenu("MainMenu");
    }

    public override void OnRoomListUpdate(List<RoomInfo> roomList)
    {
        foreach (Transform roomContent in roomListContent)
        {
            Destroy(roomContent.gameObject);
        }

        foreach (RoomInfo room in roomList)
        {
            if (room.RemovedFromList)
            {
                continue;
            }
            Instantiate(roomListItemPrefab, roomListContent).GetComponent<RoomListItem>().SetUp(room);
        }
    }

    public override void OnPlayerEnteredRoom(Player newPlayer)
    {
        Instantiate(playerListItemPrefab, playerListContent).GetComponent<PlayerListItem>().SetUp(newPlayer);
    }

    #endregion

    #region Custom Methods

    public void CreateRoom()
    {
        if (string.IsNullOrEmpty(roomName.text))
        {
            return;
        }

        RoomOptions roomOptions = new RoomOptions();

        PhotonNetwork.CreateRoom(roomName.text, new RoomOptions { MaxPlayers = 8 });
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public void LeaveRoom()
    {
        PhotonNetwork.LeaveRoom();
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public void JoinRoom(RoomInfo info)
    {
        PhotonNetwork.JoinRoom(info.Name);
        MenuManager.instance.OpenMenu("LoadingMenu");
    }

    public void StartGame()
    {
        PhotonNetwork.CurrentRoom.IsVisible = false;
        PhotonNetwork.LoadLevel(1);
    }

    public void OnUsernameInputValueChanged()
    {
        PhotonNetwork.NickName = usernameInput.text;
        PlayerPrefs.SetString("username", usernameInput.text);
    }

    public void ExitGame()
    {
        Application.Quit();
    }

    #endregion

}
