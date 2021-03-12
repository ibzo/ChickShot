using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Player : MonoBehaviour
{
    public AudioClip ShootingAudio;
    public GameObject ShootingVFX;
    public GameObject BulletPrefab;
    public Transform BulletPosition;
    public float MovementSpeed = 5f;
    public float FireRate = 0.75f;
    public Slider HealthBar;
    Rigidbody rigidbody;

    int health = 100;
    float nextFireTime;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
    }

    // Update is called once per frame
    void Update()
    {
        Move();

        if (Input.GetKey(KeyCode.Space))
            Fire();
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
            PlayerDied();
    }

    private void PlayerDied()
    {
        gameObject.SetActive(false);
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

    private void Move()
    {
        if (Input.GetAxisRaw("Horizontal") == 0 && Input.GetAxisRaw("Vertical") == 0)
            return;

        var horizontalInput = Input.GetAxis("Horizontal");
        var verticalInput = Input.GetAxis("Vertical");

        transform.rotation = Quaternion.LookRotation(new Vector3(horizontalInput, 0, verticalInput));

        var movementDirection = transform.forward * MovementSpeed * Time.deltaTime;
        rigidbody.MovePosition(rigidbody.position + movementDirection);
    }
}
