using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Enemy : MonoBehaviour
{
    public AudioClip ShootingAudio;
    public GameObject ShootingVFX;
    public GameObject BulletPrefab;
    public Transform BulletPosition;
    public Slider HealthBar;
    public float FireRate = 2.75f;

    int health = 100;
    float nextFireTime;

    public delegate void EnemyKilled();
    public static event EnemyKilled OnEnemyKilled;

    private void OnTriggerStay(Collider other)
    {
        if(other.tag == "Player")
        {
            transform.LookAt(other.transform);
            Fire();
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            var bullet = collision.gameObject.GetComponent<Bullet>();
            TakeDamage(bullet.Damage);
        }
    }

    private void TakeDamage(int damage)
    {
        health -= damage;
        HealthBar.value = health;
        if (health <= 0)
            EnemyDied();
    }

    private void EnemyDied()
    {
        gameObject.SetActive(false);
        OnEnemyKilled?.Invoke();
    }

    private void Fire()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + FireRate;
            var bullet = Instantiate(BulletPrefab, BulletPosition.position, Quaternion.identity);
            var bulletScript = bullet.GetComponent<Bullet>();

            if (bulletScript != null)
                bulletScript.InitializeBullet(transform.rotation * Vector3.forward);

            AudioManager.Instance.Play3D(ShootingAudio, BulletPosition.position, 0.1f);
            VFXManager.Instance.Play(ShootingVFX, BulletPosition.position);
        }


    }
}
