using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using Fusion.Sockets;
using UnityEngine.SceneManagement;
using System.Threading.Tasks;
using System;
using System.Linq;


public class NetworkRunnerHandler : MonoBehaviour
{
    public NetworkRunner networkRunnerPrefab;

    NetworkRunner networkRunner;


    void Start()
    {
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner";

        // AutoHostOrClient : 호스트가 없으면 첫번째 클라이언트가 호스트
        var clientTask = InitializeNetworkRunner(networkRunner, GameMode.AutoHostOrClient, NetAddress.Any(), SceneManager.GetActiveScene().buildIndex, null);

        Debug.Log($"Server NetworkRunner started.");
    }

    public void StartHostMigration(HostMigrationToken hostMigrationToken)
    {
        // 기존 러너가 삭제되고, 호스트가 이동하면서 새로운 러너가 생성된다.
        networkRunner = Instantiate(networkRunnerPrefab);
        networkRunner.name = "Network runner - Migrated";

        // AutoHostOrClient : 호스트가 없으면 첫번째 클라이언트가 호스트
        var clientTask = InitializeNetworkRunnerHostMigration(networkRunner, hostMigrationToken);

        Debug.Log($"Host migration started.");
    }

    INetworkSceneManager GetSceneManager(NetworkRunner runner)
    {
        var sceneManager = runner.GetComponents(typeof(MonoBehaviour)).OfType<INetworkSceneManager>().FirstOrDefault();

        if (sceneManager == null)
        {
            // Handle networked objects that already exitis in the scene
            sceneManager = runner.gameObject.AddComponent<NetworkSceneManagerDefault>();
        }

        return sceneManager;
    } 


    protected virtual Task InitializeNetworkRunner(NetworkRunner runner, GameMode gameMode, NetAddress address, SceneRef scene, Action<NetworkRunner> initialized)
    {
        var sceneManager = GetSceneManager(runner);

        // runner가 입력을 받을 수 있도록 처리.
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            GameMode = gameMode,
            Address = address,
            Scene = scene,
            SessionName = "TestRoom",
            Initialized = initialized,
            SceneManager = sceneManager
        });
    }

    protected virtual Task InitializeNetworkRunnerHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken)
    {
        var sceneManager = GetSceneManager(runner);

        // runner가 입력을 받을 수 있도록 처리.
        runner.ProvideInput = true;

        return runner.StartGame(new StartGameArgs
        {
            // GameMode = gameMode, // ignored, Game Mode comes with the HostMigrationToken
            // Address = address,
            // Scene = scene,
            // SessionName = "TestRoom",
            // Initialized = initialized,
            SceneManager = sceneManager,
            HostMigrationToken = hostMigrationToken, // contains all necessary info to restart the Runner
            HostMigrationResume = HostMigrationResume // contains all necessary info to restart the Runner
        });
    }

    void HostMigrationResume(NetworkRunner runner)
    {

    }
}
