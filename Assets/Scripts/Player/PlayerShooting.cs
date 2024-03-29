﻿using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class PlayerShooting : MonoBehaviour
{
    [SerializeField]
    PlayerController playerController = null;

    [SerializeField]
    private Transform firePoint = null;
    [SerializeField]
    private BulletPool bulletPool = null;
    [SerializeField]
    private BulletPool outsideBulletPool = null;

    public float bulletForce = 15f;

    private int maxBulletsCount;
    private int bulletsCount;
    public TextMeshProUGUI bulletsCountText;

    public GameObject gun;

    public Joystick shootingJoystick;
    private bool shoot = false;

    private Vector2 lookAt;

    private void Awake()
    {
        setPlayerComponents();
    }

    private void Start()
    {
        initShootingValues();
        
    }

    void Update()
    {
        if (GameController.Instance.runningOnPC)
        {
            if (Input.GetButtonDown("Fire1"))
            {
                Shoot();
            }
        }
        else
        {
            lookAt.x = shootingJoystick.Horizontal;
            lookAt.y = shootingJoystick.Vertical;
            if(Mathf.Abs(lookAt.x) > .2f || Mathf.Abs(lookAt.y) > .2f)
            {
                if (lookAt.x < .2f) flipGun(false);
                else flipGun(true);

                float angle = Mathf.Atan2(lookAt.y, lookAt.x) * Mathf.Rad2Deg - 90f;
                gun.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));

                shoot = true;
            }else if(shoot && !shootingJoystick.moving)
            {
                Shoot();
                InvokeRepeating("Shoot", 0f, 0.15f);
                Invoke("CancelShoot", 0.45f);
                shoot = false;
            }
        }
        
    }

    private void initShootingValues()
    {
        maxBulletsCount = 300;
        bulletsCount = maxBulletsCount;
        updateBulletsCountText();
        lookAt = new Vector2(0, 0);
        if (GameController.Instance.runningOnPC) shootingJoystick.gameObject.SetActive(false);
    }

    private void setPlayerComponents()
    {
        bulletsCountText = PlayerComponents.Instance.bulletsCountText;
        shootingJoystick = PlayerComponents.Instance.aimingJoystick;
    }

    private void flipGun(bool flip)
    {
        if (playerController.living)
        {
            playerController.spriteRenderer.flipX = flip;
            playerController.gunSpriteRenderer.flipX = flip;
        }
        
    }

    private void CancelShoot()
    {
        CancelInvoke("Shoot");
    }

    public void Shoot()
    {
        if(!(bulletsCount <= 0) && playerController.living && GameUIManager.Instance.currentState != GameUIStates.WAITING)
        {
            /*   SHOOT AT NEAREST TARGET 
            Transform target = getNearestTarget(Main.enemies);
            
            
            if(target != null)
            {
                Vector2 lookAtDir = target.position - gun.transform.position;
                float angle = Mathf.Atan2(lookAtDir.y, lookAtDir.x) * Mathf.Rad2Deg - 90f;
                if(target.position.x > playerController.transform.position.x) playerController.flipPlayer(-1);
                else playerController.flipPlayer(1);
                gun.transform.localRotation = Quaternion.Euler(new Vector3(0, 0, angle));
            }
            else
            {
                Debug.Log("ninguno");
                playerController.targeting = false;
            }*/

            GameObject bullet = bulletPool.getBullet();
            bullet.transform.position = firePoint.position;
            bullet.transform.rotation = firePoint.rotation;
            bullet.SetActive(true);
            Rigidbody2D rb = bullet.GetComponent<Rigidbody2D>();
            rb.AddForce(firePoint.up * (bulletForce * CurseManager.bulletSpeed), ForceMode2D.Impulse);
            /*float posX = bullet.transform.position.x;
            float posY = bullet.transform.position.y;
            Vector3 rotation = bullet.transform.rotation.eulerAngles;
            playerController.photonView.RPC("displayBullet", RpcTarget.Others, posX, posY, rotation.z);*/
            bulletsCount--;
            updateBulletsCountText();
        }
        
    }

    private Transform getNearestTarget(List<Transform> elements) 
    {
        Transform nearestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (Transform potentialTarget in elements)
        {
            if (!potentialTarget.gameObject.activeInHierarchy) break;
            Vector3 directionToTarget = potentialTarget.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                nearestTarget = potentialTarget;
            }
        }

        return nearestTarget;
    }

    private void updateBulletsCountText()
    {
        bulletsCountText.text = (bulletsCount + "/" + maxBulletsCount).ToString();
    }

    public void reloadBullets(int bullets)
    {
        bulletsCount += bullets;
        if (bulletsCount >= maxBulletsCount) bulletsCount = maxBulletsCount;
        updateBulletsCountText();
    }

    /*
    #region RPC

    [PunRPC]
    public void displayBullet(float startPosX, float startPosY, float eulerAngleZ)
    {
        GameObject newBullet = outsideBulletPool.getBullet();
        newBullet.transform.position = new Vector2(startPosX, startPosY);
        newBullet.transform.rotation = Quaternion.Euler(new Vector3(0, 0, eulerAngleZ));
        newBullet.SetActive(true);
        Rigidbody2D rb = newBullet.GetComponent<Rigidbody2D>();
        rb.AddForce(newBullet.transform.up * (bulletForce * CurseManager.bulletSpeed), ForceMode2D.Impulse);
    }

    #endregion*/
}
