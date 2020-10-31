﻿
using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Random = UnityEngine.Random;

public abstract class Enemy : MonoBehaviour
{
    public float speed;
    protected Vector2[] spots;
    protected int currentDestination;

    protected float waitTime;
    public float startWaitTime;

    public int lifePoints;


    public BulletPool bulletsPool;
    protected Rigidbody2D rb;

    protected bool collidingStaticObject;

    public GameObject[] collectables;
    protected bool lootMaker = true;

    protected Room currentRoom;

    private void Start()
    {
        initEnemy();
    }

    private void FixedUpdate()
    {
        move();
    }

    private void OnCollisionStay2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag.Equals("StaticObject")) collidingStaticObject = true;
    }

    private void OnCollisionExit2D(Collision2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag.Equals("StaticObject")) collidingStaticObject = false;
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        /*if (collision.gameObject.GetInstanceID() == bulletId) return;
        bulletId = collision.gameObject.GetInstanceID();*/
        if (tag.Equals("PlayerBullet"))
        {
            removeLifePoints(40);
            collision.gameObject.SetActive(false);
            /*Bullet b = collision.gameObject.GetComponent<Bullet>();
            if (b.bulletId == lastBulletId) return;
            lastBulletId = b.bulletId;
            //Physics2D.IgnoreCollision(collision, GetComponent<Collider2D>());
            
            
            b.activate(false);*/
        }
    }

    private void removeLifePoints(int points)
    {
        this.lifePoints -= points;
        if (this.lifePoints <= 0)
        {
            if (Random.Range(0, 100) >= 10 && lootMaker)
            {
                GameObject collectable = Instantiate(collectables[Random.Range(0, collectables.Length)]);
                collectable.transform.position = transform.position;
            }
            Destroy(gameObject);
            Main.enemies.Remove(this.transform);
        }
    }

    public abstract void initEnemy();
    public abstract void move();

    public void setCurrentRoom(Room r)
    {
        currentRoom = r;
    }
}
