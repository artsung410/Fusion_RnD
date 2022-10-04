using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LocalCameraHandler : MonoBehaviour
{
    public Transform cameraAnchorPoint;

    // Input
    Vector2 viewInput;

    //Rotation
    float cameraRotationX = 0;
    float cameraRotationY = 0;

    // Other components
    NetworkCharacterControllerPrototypeCustom networkCharacterControllerPrototypeCustom;
    Camera localCamera;
    private void Awake()
    {
        localCamera = GetComponent<Camera>();
        networkCharacterControllerPrototypeCustom = GetComponentInParent<NetworkCharacterControllerPrototypeCustom>();
    }

    void Start()
    {
        // �θ�-�ڽİ��踦 ���������, ���̷�Űâ���� ī�޶� �ٱ����� ������.
        if (localCamera.enabled)
        {
            localCamera.transform.parent = null;
        }
    }

    // Update is called once per frame
    void LateUpdate()
    {
        if (cameraAnchorPoint == null)
        {
            return;
        }

        if (!localCamera.enabled)
        {
            return;
        }

        // ī�޶� ��ǥ�� �÷��̾� ���������� �ű��.
        localCamera.transform.position = cameraAnchorPoint.position;

        // Calculation rotation
        cameraRotationX += viewInput.y * Time.deltaTime * networkCharacterControllerPrototypeCustom.viewUpDownRotationSpeed;
        cameraRotationX = Mathf.Clamp(cameraRotationX, -90, 90);

        cameraRotationY += viewInput.x * Time.deltaTime * networkCharacterControllerPrototypeCustom.rotationSpeed;

        // Apply rotation
        localCamera.transform.rotation = Quaternion.Euler(cameraRotationX, cameraRotationY, 0);
    }

    public void SetViewInputVector(Vector2 viewInput)
    {
        this.viewInput = viewInput;
    }
}
