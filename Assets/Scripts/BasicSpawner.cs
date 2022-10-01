using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using System;
using UnityEngine.SceneManagement;

public class BasicSpawner : MonoBehaviour, INetworkRunnerCallbacks
{

    [SerializeField]
    private NetworkRunner networkRunner = null; // NetworkRunner�� PhotonView�� �����ϸ�, �������� �Ҵ�ȴ�.

    [SerializeField]
    private NetworkPrefabRef playerPrefab; // Join ������ ��Ÿ���� �÷��̾� ������

    [SerializeField]
    private GameMode gameMode; // ���� or Ŭ���̾�Ʈ ����

    private Dictionary<PlayerRef, NetworkObject> playerList = new Dictionary<PlayerRef, NetworkObject>(); // �÷��̾��� ������ �����ϱ� ���� ��ųʸ�

    private void Start()
    {
        StartGame(gameMode);
    }

    async void StartGame(GameMode mode)
    {
        networkRunner.ProvideInput = true; // Runner�� �Է��� ������ �ֵ��� ��.

        // async await�� ��⸦ �ϴ°��� �ǹ��Ѵ�.
        await networkRunner.StartGame(new StartGameArgs()
        {
            GameMode = mode,
            SessionName = "Fusion Room",
            Scene = SceneManager.GetActiveScene().buildIndex,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });
    }

    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        // �÷��̾� ��ġ ����
        Vector3 spawnPosition = Vector3.up * 2;
        NetworkObject networkPlayerObject = runner.Spawn(playerPrefab, spawnPosition, Quaternion.identity, player);

        playerList.Add(player, networkPlayerObject);
    }

    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player)
    {
        // Find and remove the players avatar
        if (playerList.TryGetValue(player, out NetworkObject networkObject))
        {
            runner.Despawn(networkObject);
            playerList.Remove(player);
        }
    }

    // Ư�� �÷��̾�(runner)�� �Է��� ����ȭ ó��
    public void OnInput(NetworkRunner runner, NetworkInput input)
    {
        var data = new NetworkInputData();

        if (Input.GetKey(KeyCode.W))
        {
            data.movementInput += Vector3.forward;
            Debug.Log("W");
        } 
        
        if (Input.GetKey(KeyCode.S))
        {
            data.movementInput += Vector3.back;
            Debug.Log("S");
        }

        if (Input.GetKey(KeyCode.A))
        {
            data.movementInput += Vector3.left;
            Debug.Log("A");
        }

        if (Input.GetKey(KeyCode.D))
        {
            data.movementInput += Vector3.right;
            Debug.Log("D");
        }

        data.buttons.Set(InputButtons.JUMP, Input.GetKey(KeyCode.Space));
        data.buttons.Set(InputButtons.FIRE, Input.GetKey(KeyCode.Mouse0));

        input.Set(data);
    }


    public void OnConnectedToServer(NetworkRunner runner)
    {

    }

    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason)
    {

    }

    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token)
    {

    }

    public void OnCustomAuthenticationResponse(NetworkRunner runner, Dictionary<string, object> data)
    {

    }

    public void OnDisconnectedFromServer(NetworkRunner runner)
    {

    }

    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {

    }



    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input)
    {

    }




    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ArraySegment<byte> data)
    {

    }

    public void OnSceneLoadDone(NetworkRunner runner)
    {

    }

    public void OnSceneLoadStart(NetworkRunner runner)
    {

    }

    public void OnSessionListUpdated(NetworkRunner runner, List<SessionInfo> sessionList)
    {

    }

    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason)
    {

    }

    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message)
    {

    }
}

