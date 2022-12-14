using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Fusion;

public class WeaponHandler : NetworkBehaviour
{
    [Networked(OnChanged = nameof(OnFireChanged))]
    public bool isFiring { get; set; }

    public ParticleSystem fireParticleSystem;
    public Transform aimPoint;
    public LayerMask collisionLayers;

    float lastTimeFired = 0;

    // Other components
    HPHandler hpHandler;
    NetworkPlayer networkPlayer;

    private void Awake()
    {
        hpHandler = GetComponent<HPHandler>();
        networkPlayer = GetBehaviour<NetworkPlayer>();
    }

    // Update is called once per frame
    public override void FixedUpdateNetwork()
    {
        if (hpHandler.isDead)
        {
            return;
        }

        if (GetInput(out NetworkInputData networkInputData))
        {
            if (networkInputData.isFireButtonPressed)
            {
                Fire(networkInputData.aimForwardVector);
            }
        }
     
    }

    void Fire(Vector3 aimForewardVector)
    {
        // 발사주기 설정한다.
        if (Time.time - lastTimeFired < 0.15f)
        {
            return;
        }

        StartCoroutine(FireEffectCO());

        Runner.LagCompensation.Raycast(aimPoint.position, aimForewardVector, 100, Object.InputAuthority, out var hitinfo, collisionLayers, HitOptions.IgnoreInputAuthority);

        float hitDistance = 100;
        bool isHitOtherPlayer = false;

        if (hitinfo.Distance > 0)
        {
            hitDistance = hitinfo.Distance;
        }

        if (hitinfo.Hitbox != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit hitbox {hitinfo.Hitbox.transform.root.name}");

            if (Object.HasStateAuthority)
            {
                hitinfo.Hitbox.transform.root.GetComponent<HPHandler>().OnTakeDamage(networkPlayer.nickName.ToString());
            }

            isHitOtherPlayer = true;
        }

        else if (hitinfo.Collider != null)
        {
            Debug.Log($"{Time.time} {transform.name} hit PhysX hitbox {hitinfo.Collider.transform.name}");
        }

        // Debug
        if (isHitOtherPlayer)
        {
            Debug.DrawRay(aimPoint.position, aimForewardVector * hitDistance, Color.red, 1);
        }
        else
        {
            Debug.DrawRay(aimPoint.position, aimForewardVector * hitDistance, Color.green, 1);
        }

        lastTimeFired = Time.time;
    }

    IEnumerator FireEffectCO()
    {
        isFiring = true;

        fireParticleSystem.Play();

        yield return new WaitForSeconds(0.09f);

        isFiring = false;
    }    

    static void OnFireChanged(Changed<WeaponHandler> changed)
    {
        //Debug.Log($"{Time.time} OnFireChanged value { changed.Behaviour.isFiring}");

        bool isFiringCurrent = changed.Behaviour.isFiring;

        // Load the old vlaue
        changed.LoadOld();

        bool isFiringOld = changed.Behaviour.isFiring;

        if (isFiringCurrent && !isFiringOld)
        {
            changed.Behaviour.OnFireRemote();
        }
    }

    void OnFireRemote()
    {
        if (!Object.HasInputAuthority)
        {
            fireParticleSystem.Play();
        }
    }
}
