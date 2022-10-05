using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using TMPro;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public TextMeshProUGUI playerNickNameTM;
    public static NetworkPlayer Local { get; set; }
    public Transform playerModel;

    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> nickName { get; set; }

    bool isPublicJoinMessageSent = false;

    public LocalCameraHandler localCameraHandler;
    public GameObject localUI;


    // Other components
    NetworkInGameMessages networkInGameMessages;

    void Awake()
    {
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
    }

    public override void Spawned()
    {
        // 월드상 입력권한을 가진 내 자신의 캐릭터가 스폰되었을 때
        if (Object.HasInputAuthority)
        {
            Local = this;

            // 카메라상에 보이는 내 오브젝트 안보이게 하기
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // 메인 카메라 비활성화
            Camera.main.gameObject.SetActive(false);

            // PlayerPrefs에는 많은 정보가 담겨있다.
            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));

            Debug.Log("Spawned Local Player");
        }

        // 로컬플레이어가 아닌 다른 클라이언트의 오브젝트가 스폰되었을 때
        else
        {
            // 로컬플레이어가 아니면 카메라는 비활성화 처리
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            // 로컬플레이어가 아니면 오디오 리스너 끄기.
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            // Disable UI for remote player
            localUI.SetActive(false);

            Debug.Log("Spawned remote Player");
        }

        // Set the player as a player object
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        // 다른 플레이어에게 말하기
        transform.name = $"P_{Object.Id}";
    }


    public void PlayerLeft(PlayerRef player)
    { 
        if (Object.HasStateAuthority)
        {
            // 떠난 네트워크 오브젝트를 리턴
            // 여러명 나간걸로 뜨기때문에, 특정 닉네임만 뜨도록 설정함.

            if (Runner.TryGetPlayerObject(player, out NetworkObject playerLeftNetworkObject))
            {
                if (playerLeftNetworkObject == Object)
                {
                    Local.GetComponent<NetworkInGameMessages>().SendInGameRPCMessage(playerLeftNetworkObject.GetComponent<NetworkPlayer>().nickName.ToString(), "left");
                }
            }
        }


        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }    
    }

    static void OnNickNameChanged(Changed<NetworkPlayer> changed)
    {
        Debug.Log($"{Time.time} OnHPchanged value {changed.Behaviour.nickName}");
        changed.Behaviour.OnNickNameChanged();
    }

    // 클라이언트는 RPC 메세지를 서버로 보낸다.
    // 내 이름은 누구누구입니다 알리고, 그리고 다른 클라이언트에게도 표시가 된다.
    // RPC를 사용하려면 NetworkBehaviour를 상속받아야 한다.
    private void OnNickNameChanged()
    {
        Debug.Log($"Nickname changed for player to {nickName} for Player {gameObject.name}");
        playerNickNameTM.text = nickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.StateAuthority)]
    public void RPC_SetNickName(string nickName, RpcInfo info = default)
    {
        Debug.Log($"[RPC] SetNickName {nickName}");
        this.nickName = nickName;

        if (!isPublicJoinMessageSent)
        {
            networkInGameMessages.SendInGameRPCMessage(nickName, "joined");

            isPublicJoinMessageSent = true;
        }
    }
}
