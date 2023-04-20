using Photon.Pun;
using Photon.Pun.UtilityScripts;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class PlayerMultiplayer : MonoBehaviour, IPunObservable
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
    bool playerIsDead = false;

    PhotonView photonView;

    MeshRenderer meshRenderer;
    BoxCollider boxCollider;

    Vector3 cameraOffset;

    // Start is called before the first frame update
    void Start()
    {
        rigidbody = GetComponent<Rigidbody>();
        photonView = GetComponent<PhotonView>();
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<BoxCollider>();

        cameraOffset = Camera.main.transform.position - transform.position;
    }

    // Update is called once per frame
    private void FixedUpdate()
    {
        if (!photonView.IsMine || playerIsDead)
            return;

        Move();

        if (Input.GetKey(KeyCode.Space))
            photonView.RPC("Fire", RpcTarget.AllViaServer);

        UpdateCamera();
    }

    private void UpdateCamera()
    {
        Camera.main.transform.position = transform.position + cameraOffset;
    }

    private void OnCollisionEnter(Collision collision)
    {
        if (collision.gameObject.tag == "Bullet")
        {
            var bullet = collision.gameObject.GetComponent<BulletMultiplayer>();
            TakeDamage(bullet);
        }
    }

    private void TakeDamage(BulletMultiplayer bullet)
    {
        health -= bullet.Damage;
        HealthBar.value = health;
        if (health <= 0)
        {
            bullet.Owner.AddScore(1);
            PlayerDied();
        }
    }

    private void PlayerDied()
    {
        playerIsDead = true;
        //Hide Rhino game object
        HealthBar.gameObject.SetActive(false);
        meshRenderer.enabled = false;
        boxCollider.enabled = false;
        rigidbody.isKinematic = true;
        //wait 2 seconds
        StartCoroutine(RespawnWaitTime());
    }

    private IEnumerator RespawnWaitTime()
    {
        yield return new WaitForSeconds(2f);

        //show up rhino game object
        playerIsDead = false;
        meshRenderer.enabled = true;
        boxCollider.enabled = true;
        rigidbody.isKinematic = false;
        health = 100;
        HealthBar.value = health;
        HealthBar.gameObject.SetActive(true);
    }

    [PunRPC]
    private void Fire()
    {
        if (Time.time > nextFireTime)
        {
            nextFireTime = Time.time + FireRate;
            var bullet = Instantiate(BulletPrefab, BulletPosition.position, Quaternion.identity);
            var bulletScript = bullet.GetComponent<BulletMultiplayer>();

            if (bulletScript != null)
                bulletScript.InitializeBullet(transform.rotation * Vector3.forward, photonView.Owner);

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

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(health);
        }else
        {
            health = (int)stream.ReceiveNext();
            HealthBar.value = health;
        }
    }
}

