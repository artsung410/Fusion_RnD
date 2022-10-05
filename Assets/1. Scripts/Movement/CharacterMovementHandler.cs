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

        // 네트워크에서 입력을 가져온다.
        if (GetInput(out NetworkInputData networkInputData))
        {
            // 겨냥하고있는 방향으로 회전
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

            // 지하실로 떨어지면 다리 리스폰
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
