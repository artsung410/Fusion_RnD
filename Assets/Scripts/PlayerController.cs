using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;
using UnityEngine.UI;
using TMPro;
public class PlayerController : NetworkBehaviour
{
    [SerializeField]
    private NetworkCharacterControllerPrototype networkCharacterController = null;

    [SerializeField]
    private Bullet bulletPrefab;

    [SerializeField]
    private Image hpBar = null;

    [SerializeField]
    private MeshRenderer meshRenderer = null;

    [SerializeField]
    private float moveSpeed = 15f;

    [SerializeField]
    private int maxHp = 100;

    [SerializeField]
    private TextMeshProUGUI nickNameTMPro;

    // OnChanaged = 함수에 리스너를 등록하면, 어트리뷰트로 지정한 변수가 바뀔때 마다 리스너를 호출하게할수 있음
    // ex) OnHpChanged(리스너) 플레이어
    [Networked(OnChanged = nameof(OnHpChanged))]
    public int Hp { get; set; }


    public NetworkButtons ButtonsPrevious { get; set; }


    [Networked(OnChanged = nameof(OnNickNameChanged))]
    public NetworkString<_16> playerNickName { get; set; }


    public override void Spawned()
    {
        if (Object.HasStateAuthority)
        {
            Hp = maxHp;
        }
    }

    public override void FixedUpdateNetwork()
    {
        if (GetInput(out NetworkInputData data))
        {
            NetworkButtons buttons = data.buttons;
            var pressed = buttons.GetPressed(ButtonsPrevious);
            ButtonsPrevious = buttons;

            Vector3 moveVector = data.movementInput.normalized;
            networkCharacterController.Move(moveSpeed * moveVector * Runner.DeltaTime);

            if (pressed.IsSet(InputButtons.JUMP))
            {
                networkCharacterController.Jump();
            }

            if (pressed.IsSet(InputButtons.FIRE))
            {
                Runner.Spawn(
                    bulletPrefab,
                    transform.position + transform.TransformDirection(Vector3.forward),
                    Quaternion.LookRotation(transform.TransformDirection(Vector3.forward)),
                    Object.InputAuthority);
            }
            
        }

        if (Hp <= 0 || networkCharacterController.transform.position.y <= -5f)
        {
            Respawn();
        }
    }

    private void Respawn()
    {
        networkCharacterController.transform.position = Vector3.up * 2;
        Hp = maxHp;
    }
    
    public void TakeDamage(int damage)
    {
        if (Object.HasStateAuthority)
        {
            Hp -= damage;
        }
    }

    private static void OnHpChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.hpBar.fillAmount = (float)changed.Behaviour.Hp / changed.Behaviour.maxHp;
    }

    private void Update()
    {
        if (Input.GetKeyDown(KeyCode.R))
        {
            ChangeColor_RPC(Color.red);
        }

        if (Input.GetKeyDown(KeyCode.G))
        {
            ChangeColor_RPC(Color.green);
        }

        if (Input.GetKeyDown(KeyCode.B))
        {
            ChangeColor_RPC(Color.blue);
        }


        if (Object.HasInputAuthority && Input.GetKeyDown(KeyCode.Return))
        {
            RPC_SendMessage(nickNameTMPro.text);
        }

        if (Object.HasInputAuthority)
        {
            RPC_SendNickName(GameManager.Instance.nickName);
        }
    }

    // Material_Settings
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    private void ChangeColor_RPC(Color newColor)
    {
        meshRenderer.material.color = newColor;
    }

    // ChattingSystem_Settings
    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendMessage(string nickName, RpcInfo info = default)
    {
        //if (info.Source == Runner.Simulation.LocalPlayer)
        //    message = $"You: {message}\n";
        //else
        //    message = $"Other: {message}\n";

        PlayerHUD.Instance.chatText.text += $"{nickName} :  + Hi\n";
    }

    // NickName_Settings
    static void OnNickNameChanged(Changed<PlayerController> changed)
    {
        changed.Behaviour.SetNickName();
    }

    private void SetNickName()
    {
        nickNameTMPro.text = playerNickName.ToString();
    }

    [Rpc(RpcSources.InputAuthority, RpcTargets.All)]
    public void RPC_SendNickName(string name)
    {
        playerNickName = name;
    }

}
