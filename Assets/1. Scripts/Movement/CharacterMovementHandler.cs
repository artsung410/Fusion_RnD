using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class CharacterMovementHandler : NetworkBehaviour
{
    bool isRespawnRequested = false;

    //Other components
    NetworkCharacterControllerPrototypeCustom networkCharacterContollerPrototypeCustom;
    HPHandler hPHandler;
    NetworkInGameMessages networkInGameMessages;
    NetworkPlayer networkPlayer;

    private void Awake()
    {
        networkCharacterContollerPrototypeCustom = GetComponent<NetworkCharacterControllerPrototypeCustom>();
        hPHandler = GetComponent<HPHandler>();
        networkInGameMessages = GetComponent<NetworkInGameMessages>();
        networkPlayer = GetComponent<NetworkPlayer>();
    }

    public override void FixedUpdateNetwork()
    {
        if (Object.HasStateAuthority)
        {
            if (isRespawnRequested)
            {
                Respawn();
                return;
            }

            // Don't update the clients position when they are dead
            if (hPHandler.isDead)
            {
                return;
            }
        }

        // ��Ʈ��ũ���� �Է��� �����´�.
        if (GetInput(out NetworkInputData networkInputData))
        {
            // �ܳ��ϰ��ִ� �������� ȸ��
            transform.forward = networkInputData.aimForwardVector;

            // Cancel out rotation on X axis as we don't want our character to tilt
            Quaternion rotation = transform.rotation;
            rotation.eulerAngles = new Vector3(0, rotation.eulerAngles.y, rotation.eulerAngles.z);
            transform.rotation = rotation;

            //Move
            Vector3 moveDirection = transform.forward * networkInputData.movementInput.y + transform.right * networkInputData.movementInput.x;
            moveDirection.Normalize();
            networkCharacterContollerPrototypeCustom.Move(moveDirection);

            //Jump
            if(networkInputData.isJumpPressed)
            {
                networkCharacterContollerPrototypeCustom.Jump();
            }

            // ���ϽǷ� �������� �ٸ� ������
            CheckFallRespawn();
        }
    }

    void CheckFallRespawn()
    {
        if (transform.position.y < -12)
        {
            if (Object.HasStateAuthority)
            {
                Debug.Log($"{Time.time} Respawn due to fall outside of map at position {transform.position}");
                networkInGameMessages.SendInGameRPCMessage(networkPlayer.nickName.ToString(), "fell off the world");

                Respawn();
            }
        }
    }

    public void RequestRespawn()
    {
        isRespawnRequested = true;
    }

    void Respawn()
    {
        networkCharacterContollerPrototypeCustom.TeleportToPosition(Utils.GetRandomSpawnPoint());

        hPHandler.OnRespawned();

        isRespawnRequested = false;
    }

    public void SetCharacterControllerEnabled(bool isEnabled)
    {
        networkCharacterContollerPrototypeCustom.Controller.enabled = isEnabled;
    }
}
