using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public GameObject BulletPrefab;
    public Transform BulletPosition;
    public float MovementSpeed = 5f;
    public float FireRate = 0.75f;
    Rigidbody rigidbody;

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

    private void Fire()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + FireRate;
            var bullet = Instantiate(BulletPrefab, BulletPosition.position, Quaternion.identity);
            var bulletScript = bullet.GetComponent<Bullet>();
            
            if (bulletScript != null)
                bulletScript.InitializeBullet(transform.rotation * Vector3.forward);

            //bullet.GetComponent<Bullet>()?.InitializeBullet(transform.rotation * Vector3.forward);
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
