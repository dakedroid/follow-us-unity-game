﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Room : MonoBehaviour
{
    public Vector2Int mapLocation;
    public bool isRoomActive;
    public bool isDestroyed;
    public int[] posibleDirections = new int[4];
    public int threatType;

    public Transform enemiesGenerationPoint;
    public GameObject[] enemiesPrefab;

    public GameObject rightDoor;
    public GameObject leftDoor;
    public GameObject downDoor;
    public GameObject upDoor;

    public Sprite[] backgroundSprites;
    public Sprite[] doorSprites;

    public GameObject staticElementsGroup;
    public GameObject[] staticElements;

    public Transform[] corners;
    public Transform[] centipedePoints;

    public float timer;
    public float maxTimer;

    public SpriteRenderer crackEffect;
    public Sprite[] crackSprites;
    private float crackTime;

    private bool firstTime = true;

    public Sprite zombieHole;
    private int[] cornersIndex = new int[2];

    public GameObject emptyRoom;

    private bool firstTimeDestroying;

    void Awake()
    {
        posibleDirections[0] = 1;
        posibleDirections[1] = 1;
        posibleDirections[2] = 1;
        posibleDirections[3] = 1;
        timer = maxTimer;
        isDestroyed = false;
        firstTimeDestroying = true;
    }



    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = backgroundSprites[Random.Range(1, backgroundSprites.Length)];
        crackTime = maxTimer / 5;
    }

    private void OnEnable()
    {
        if (!firstTime) generateEnemies();
        else firstTime = false;
    }

    private void Update()
    {
        if (timer > 0)
        {
            timer -= Time.deltaTime;
            if (timer > crackTime * 4) crackEffect.sprite = null;
            else if (timer > crackTime * 3) crackEffect.sprite = crackSprites[0];
            else if (timer > crackTime * 2) crackEffect.sprite = crackSprites[1];
            else if (timer > crackTime * 1) crackEffect.sprite = crackSprites[2];
            else if (timer > 0) crackEffect.sprite = crackSprites[3];
        }
        else
        {
            if (firstTimeDestroying)
            {
                Instantiate(emptyRoom).transform.position = this.transform.position;
                Invoke("desactivateRoom", 5);
                GetComponent<SpriteRenderer>().sprite = backgroundSprites[0];
                firstTimeDestroying = false;
            }
        }
    }

    private void desactivateRoom()
    {
        this.isDestroyed = true;
        this.gameObject.SetActive(false);
    }

    public void generateEnemies()
    {
        int enemiesCount = 0;
        int enemyType = -1;
        switch (threatType)
        {
            case 1:
                enemyType = 0;
                enemiesCount = Random.Range(1, 3);
                break;
            case 2:
                enemyType = 0;
                enemiesCount = Random.Range(3, 5);
                break;
            case 3:
                enemyType = 0;
                enemiesCount = Random.Range(5, 7);
                break;
            case 4:
                enemyType = 1;
                enemiesCount = 1;
                break;
            case 5:
                enemyType = 2;
                enemiesCount = 2;
                break;
            case 6:
                enemyType = 3;
                enemiesCount = 1;
                break;
            case 7:
                enemyType = 4;
                enemiesCount = 6;
                break;
            case 8:
                enemyType = 5;
                break;
        }

        if(enemyType == 4)
        {
            int randPos = Random.Range(0, corners.Length);
            cornersIndex[0] = randPos;
            corners[randPos].GetComponent<SpriteRenderer>().sprite = zombieHole;
            randPos++;
            if (randPos == corners.Length) randPos = 0;
            cornersIndex[1] = randPos;
            corners[randPos].GetComponent<SpriteRenderer>().sprite = zombieHole;
            InvokeRepeating("generateZombies", 0, 1.2f);
            Invoke("CancelGeneration", 15);
        }else if (enemyType == 5)
        {
            for(int i = 0; i < 14; i++)
            {
                GameObject newCentipede = Instantiate(enemiesPrefab[enemyType], enemiesGenerationPoint);
                if(i == 0) newCentipede.transform.GetChild(1).GetComponent<Centipede>().setCentipedeAttributes(true, i, this);
                else newCentipede.transform.GetChild(1).GetComponent<Centipede>().setCentipedeAttributes(false, i, this);
                Main.enemies.Add(newCentipede.transform);
            }
        }
        else
        {
            for (int i = 0; i < enemiesCount; i++)
            {
                GameObject newEnemy = Instantiate(enemiesPrefab[enemyType], enemiesGenerationPoint);
                Main.enemies.Add(newEnemy.transform);
            }
        }
        
    }

    private void CancelGeneration()
    {
        CancelInvoke("generateZombies");
    }

    private void generateZombies()
    {
        GameObject newZombie = Instantiate(enemiesPrefab[4], enemiesGenerationPoint );
        newZombie.transform.position = corners[cornersIndex[Random.Range(0, cornersIndex.Length)]].transform.position;
        Main.enemies.Add(newZombie.transform);
    }


    public void setDoorSprites(int cols, int rows)
    {
        Sprite notPassSprite = doorSprites[0];
        if (mapLocation.x == 0) leftDoor.GetComponent<SpriteRenderer>().sprite = notPassSprite;
        else if (mapLocation.x == cols - 1) rightDoor.GetComponent<SpriteRenderer>().sprite = notPassSprite;

        if (mapLocation.y == 0) upDoor.GetComponent<SpriteRenderer>().sprite = notPassSprite;
        else if (mapLocation.y == rows - 1) downDoor.GetComponent<SpriteRenderer>().sprite = notPassSprite;
    }

    public void setPosibleDirections(int right, int left, int up, int down)
    {
        posibleDirections[0] = right;
        posibleDirections[1] = left;
        posibleDirections[2] = up;
        posibleDirections[3] = down;
    }


    public void generateStaticElements(int numElements)
    {
        if (numElements == 1)
        {
            Instantiate(staticElements[0], staticElementsGroup.transform).transform.position = staticElementsGroup.transform.position;
        }
        else
        {
            for(int i = 0; i < numElements; i++)
            {
                GameObject newStaticElement = Instantiate(staticElements[Random.Range(1, staticElements.Length)], staticElementsGroup.transform);
                newStaticElement.transform.position = Util.getRandomPosition(staticElementsGroup.transform, .5f);
            }
        }
    }
}
