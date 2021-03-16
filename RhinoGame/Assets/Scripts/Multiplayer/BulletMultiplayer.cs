using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BulletMultiplayer : MonoBehaviour
{
    public AudioClip BulletHitAudio;
    public GameObject BulletHitVFX;
    public float BulletSpeed;
    public int Damage = 5;
    public Photon.Realtime.Player Owner;

    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void InitializeBullet(Vector3 originalDirection, Photon.Realtime.Player player)
    {
        transform.forward = originalDirection;
        rigidbody.velocity = transform.forward * BulletSpeed;
        Owner = player;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position, 0.1f);
        VFXManager.Instance.Play(BulletHitVFX, transform.position);
        Destroy(gameObject);
    }
}