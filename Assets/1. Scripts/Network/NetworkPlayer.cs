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
        // 월드상 입력권한을 가진 내 자신의 캐릭터가 스폰되었을 때
        if (Object.HasInputAuthority)
        {
            Local = this;

            // 카메라상에 보이는 내 오브젝트 안보이게 하기
            Utils.SetRenderLayerInChildren(playerModel, LayerMask.NameToLayer("LocalPlayerModel"));

            // 메인 카메라 비활성화
            Camera.main.gameObject.SetActive(false);

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
            Debug.Log("Spawned remote player");
        }

        // 다른 플레이어에게 말하기
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
