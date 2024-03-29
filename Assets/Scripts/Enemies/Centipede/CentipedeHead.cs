﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CentipedeHead : CentipedeBody
{
    private CentipedePoint lastTarget;

    private bool fixedHead = false;


    public void createFixedHead(Transform nPos)
    {
        this.transform.position = nPos.position;
        CentipedePoint nearestPoint = Util.getNearestTarget(transform, points);
        this.nextTarget = nearestPoint.getRandomPath(nearestPoint);
        this.lastTarget = nextTarget;
        rotate(nextTarget.transform);
        fixedHead = true;
    }


    public override void manageCollision(Collider2D collision)
    {
        string tag = collision.gameObject.tag;
        if (tag.Equals("PlayerBullet"))
        {
            if (collision.gameObject.GetComponent<Bullet>().hit) return;
            collision.gameObject.GetComponent<Bullet>().hit = true;
            collision.gameObject.SetActive(false);
            if (removeLifePoints(40) <= 0)
            {
                if (lastBody != null)
                {
                    this.lifePoints = 150;

                    GameObject last = lastBody.gameObject;
                    transform.position = last.transform.position;

                    if (lastBody.lastBody != null)
                    {
                        lastBody.lastBody.nextBody = this;
                        lastBody = lastBody.lastBody;
                    }
                    last.transform.GetComponent<CentipedeTail>().canDestroy = true;
                    Destroy(last.gameObject);
                }
                else
                {
                    this.canDestroy = true;
                    removeEnemy();
                }
            }
        }
    }

    public override void initCentipedeBody()
    {
        if (!fixedHead)
        {
            int randomPosition = Random.Range(0, points.Length);
            transform.position = points[randomPosition].transform.position;
            this.nextTarget = points[randomPosition];
            this.lastTarget = nextTarget;
            rotate(nextTarget.transform);
        }
    }

    public override void manageMovement()
    {
        rb.MovePosition(Vector2.MoveTowards(transform.position, nextTarget.transform.position, (speed * CurseManager.enemiesSpeed) * Time.deltaTime));
        if (Vector2.Distance(transform.position, nextTarget.transform.position) < 0.2f || collidingStaticObject)
        {
            CentipedePoint temp = nextTarget;
            nextTarget = nextTarget.getRandomPath(lastTarget);
            lastTarget = temp;
            rotate(nextTarget.transform);
        }
    }

    
}
