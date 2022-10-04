using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class NetworkPlayer : NetworkBehaviour, IPlayerLeft
{
    public static NetworkPlayer Local { get; set; }

    public Transform playerModel;

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
            Debug.Log("Spawned remote player");
        }

        // �ٸ� �÷��̾�� ���ϱ�
        transform.name = $"P_{Object.Id}";
    }


    public void PlayerLeft(PlayerRef player)
    {
        if (player == Object.InputAuthority)
        {
            Runner.Despawn(Object);
        }    
    }
}
