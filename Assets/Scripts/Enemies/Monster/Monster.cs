﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Monster : Enemy
{
    private Vector2[] targetsPositions;
    private int nextTargetIndex;
    private Transform parent;
    private bool shooting = false;

    private int shootingCount = 0;
    
    public override void initEnemy()
    {
        lifePoints = 150;
        waitTime = startWaitTime;
        parent = transform.parent;
        transform.position = parent.position;

        targetsPositions = new Vector2[9];

        targetsPositions[0] = new Vector2(parent.position.x, parent.position.y);
        targetsPositions[1] = new Vector2(transform.position.x + Util.playableArea-.5f, transform.position.y);
        targetsPositions[2] = new Vector2(transform.position.x + Util.playableArea-.5f, transform.position.y + Util.playableArea-.5f);
        targetsPositions[3] = new Vector2(transform.position.x, transform.position.y + Util.playableArea-.5f);
        targetsPositions[4] = new Vector2(transform.position.x - Util.playableArea+.5f, transform.position.y + Util.playableArea-.5f);
        targetsPositions[5] = new Vector2(transform.position.x - Util.playableArea+.5f, transform.position.y);
        targetsPositions[6] = new Vector2(transform.position.x - Util.playableArea+.5f, transform.position.y - Util.playableArea+.5f);
        targetsPositions[7] = new Vector2(transform.position.x, transform.position.y - Util.playableArea+.5f);
        targetsPositions[8] = new Vector2(transform.position.x + Util.playableArea-.5f, transform.position.y - Util.playableArea+.5f);
        

        nextTargetIndex = Random.Range(1, targetsPositions.Length);
    }

    private void shoot()
    {
        GameObject[] bullets = new GameObject[20];
        float angle = 0;
        for (int i = 0; i < bullets.Length; i++)
        {
            bullets[i] = bulletsPool.getBullet();
            bullets[i].transform.position = transform.position;
            bullets[i].transform.rotation = Quaternion.Euler(0, 0, angle);
            bullets[i].SetActive(true);
            Rigidbody2D rb = bullets[i].GetComponent<Rigidbody2D>();
            rb.AddForce(bullets[i].transform.up * 2.3f, ForceMode2D.Impulse);
            angle += 18f;
        }
        shooting = true;

        shootingCount++;

        if(shootingCount >= 5)
        {
            GameObject newEnemy = Instantiate(EnemyPrefabManager.Instance.evilFairy, transform.parent.parent);
            newEnemy.transform.position = transform.position;
            shootingCount = 0;
            Main.Instance.enemiesCount++;
        }

    }

    public override void move()
    {
        rb.MovePosition(Vector2.MoveTowards(transform.position, targetsPositions[nextTargetIndex], (speed * Util.enemiesSpeed) * Time.deltaTime));
        if (Vector2.Distance(transform.position, targetsPositions[nextTargetIndex]) < 0.2f || collidingStaticObject)
        {
            if (anim != null) anim.SetBool("shooting", true);
            if (!shooting) shoot();
            if (waitTime < 0)
            {
                //Change destination target
                if (nextTargetIndex == 0) nextTargetIndex = Random.Range(1, targetsPositions.Length);
                else nextTargetIndex = 0;
                waitTime = 0.5f;
                shooting = false;
                if (anim != null) anim.SetBool("shooting", false);
            }
            else
            {
                waitTime -= Time.deltaTime;
            }
        }
    }
}