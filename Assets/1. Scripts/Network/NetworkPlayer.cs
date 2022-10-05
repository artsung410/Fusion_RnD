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
        // ����� �Է±����� ���� �� �ڽ��� ĳ���Ͱ� �����Ǿ��� ��
        if (Object.HasInputAuthority)
        {
            Local = this;

            // ī�޶�� ���̴� �� ������Ʈ �Ⱥ��̰� �ϱ�
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // ���� ī�޶� ��Ȱ��ȭ
            Camera.main.gameObject.SetActive(false);

            // PlayerPrefs���� ���� ������ ����ִ�.
            RPC_SetNickName(PlayerPrefs.GetString("PlayerNickname"));

            Debug.Log("Spawned Local Player");
        }

        // �����÷��̾ �ƴ� �ٸ� Ŭ���̾�Ʈ�� ������Ʈ�� �����Ǿ��� ��
        else
        {
            // �����÷��̾ �ƴϸ� ī�޶�� ��Ȱ��ȭ ó��
            Camera localCamera = GetComponentInChildren<Camera>();
            localCamera.enabled = false;

            // �����÷��̾ �ƴϸ� ����� ������ ����.
            AudioListener audioListener = GetComponentInChildren<AudioListener>();
            audioListener.enabled = false;

            // Disable UI for remote player
            localUI.SetActive(false);

            Debug.Log("Spawned remote Player");
        }

        // Set the player as a player object
        Runner.SetPlayerObject(Object.InputAuthority, Object);

        // �ٸ� �÷��̾�� ���ϱ�
        transform.name = $"P_{Object.Id}";
    }


    public void PlayerLeft(PlayerRef player)
    { 
        if (Object.HasStateAuthority)
        {
            // ���� ��Ʈ��ũ ������Ʈ�� ����
            // ������ �����ɷ� �߱⶧����, Ư�� �г��Ӹ� �ߵ��� ������.

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

    // Ŭ���̾�Ʈ�� RPC �޼����� ������ ������.
    // �� �̸��� ���������Դϴ� �˸���, �׸��� �ٸ� Ŭ���̾�Ʈ���Ե� ǥ�ð� �ȴ�.
    // RPC�� ����Ϸ��� NetworkBehaviour�� ��ӹ޾ƾ� �Ѵ�.
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
