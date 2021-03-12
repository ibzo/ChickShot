using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bullet : MonoBehaviour
{
    public AudioClip BulletHitAudio;
    public GameObject BulletHitVFX;
    public float BulletSpeed;
    public int Damage = 5;
    Rigidbody rigidbody;

    private void Awake()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    public void InitializeBullet(Vector3 originalDirection)
    {
        transform.forward = originalDirection;
        rigidbody.velocity = transform.forward * BulletSpeed;
    }

    private void OnCollisionEnter(Collision collision)
    {
        AudioManager.Instance.Play3D(BulletHitAudio, transform.position, 0.1f) ;
        VFXManager.Instance.Play(BulletHitVFX, transform.position);
        Destroy(gameObject);
    }
}