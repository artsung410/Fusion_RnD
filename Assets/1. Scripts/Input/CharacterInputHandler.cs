using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CharacterInputHandler : MonoBehaviour
{
    Vector2 moveInputVector = Vector2.zero;
    Vector2 viewInputVector = Vector2.zero;
    bool isJumpButtonPressed = false;
    bool isFireButtonPressed = false;

    // Ohter components
    LocalCameraHandler localCameraHandler;
    CharacterMovementHandler characterMovementHandler;

    private void Awake()
    {
        localCameraHandler = GetComponentInChildren<LocalCameraHandler>();
        characterMovementHandler = GetComponent<CharacterMovementHandler>();
    }

    private void Start()
    {
        Cursor.lockState = CursorLockMode.Locked;
        Cursor.visible = false;
    }

    private void Update()
    {
        if(!characterMovementHandler.Object.HasInputAuthority)
        {
            return;
        }

        // View input
        viewInputVector.x = Input.GetAxis("Mouse X");
        viewInputVector.y = Input.GetAxis("Mouse Y") * -1; // Invert the mouse look

        // Move input
        moveInputVector.x = Input.GetAxis("Horizontal");
        moveInputVector.y = Input.GetAxis("Vertical");

        // Jump input
        if (Input.GetButtonDown("Jump"))
        {
            isJumpButtonPressed = true;
        }

        // Fire
        if (Input.GetButtonDown("Fire1"))
        {
            isFireButtonPressed = true;
        }

        // Set view
        localCameraHandler.SetViewInputVector(viewInputVector);
    }

    public NetworkInputData GetNetworkInput()
    {
        NetworkInputData networkInputData = new NetworkInputData();

        // View data
        networkInputData.aimForwardVector = localCameraHandler.transform.forward;

        // Move data
        networkInputData.movementInput = moveInputVector;

        // Jump data
        networkInputData.isJumpPressed = isJumpButtonPressed;

        // Fire data
        networkInputData.isFireButtonPressed = isFireButtonPressed;

        // Reset variables now that we have read their states
        isJumpButtonPressed = false;
        isFireButtonPressed = false;

        return networkInputData;
    }
}
