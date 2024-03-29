﻿using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;

public class Room : MonoBehaviour
{
    public event EventHandler<OnRoomStartedArgs> OnRoomStarted;
    public class OnRoomStartedArgs : EventArgs
    {
        public float initTimer;
    }

    private Map mapController;
    public Vector2Int mapLocation;
    public bool isRoomActive;
    public bool isDestroyed;
    private bool isMainRoom;
    public int[] posibleDirections = new int[4];

    public int threatType;
    public int roomType;

    public Transform enemiesGenerationPoint;

    public Door rightDoor;
    public Door leftDoor;
    public Door downDoor;
    public Door upDoor;


    public Sprite[] backgroundSprites;
    public Sprite[] doorSprites;

    private float maxTimer;

    private bool firstTime = true;

    public GameObject emptyRoom;


    [SerializeField]
    private EnemyGenerator enemyGenerator = null;
    [SerializeField]
    private StaticObjectsManager staticObjectsGenerator = null;
    [SerializeField]
    private CrackEffectManager crackManager = null;

    public TimerManager timerManager;

    void Awake()
    {
        posibleDirections[0] = 1;
        posibleDirections[1] = 1;
        posibleDirections[2] = 1;
        posibleDirections[3] = 1;
        isDestroyed = false;
        if(PlayerComponents.Instance != null)
        {
            transform.SetParent(PlayerComponents.Instance.map);
            mapController = PlayerComponents.Instance.mapController;
        }
        maxTimer = Util.maxRoomTime;
        timerManager.OnTimerRanOut += onTimerRanOut;
    }

    private void Start()
    {
        GetComponent<SpriteRenderer>().sprite = backgroundSprites[UnityEngine.Random.Range(1, backgroundSprites.Length)];
    }

    private void onTimerRanOut(object sender, EventArgs e)
    {
        DestroyRoom();
    }

    private void OnEnable()
    {
        if (!firstTime)
        {
            if (!isMainRoom)
            {
                crackManager.startEffect(maxTimer);
                timerManager.startRunning(maxTimer);
                generateEnemies();
                generateStaticElements(0);
                //generateStaticElements(UnityEngine.Random.Range(0, 2));
            }
            else
            {
                //OnRoomStarted?.Invoke(this, new OnRoomStartedArgs { initTimer = this.maxTimer });
                timerManager.startRunning(maxTimer);
                crackManager.startEffect(maxTimer);
                //GameUIManager.Instance.setState(GameUIStates.GAME_HAS_STARTED);
                generateStaticElements(1);
            }
        }
        else firstTime = false;
    }

    public void reduceTime()
    {
        maxTimer /= 2f;
    }

    public void increaseThreatType()
    {
        threatType += 2;
        if (threatType >= 9) threatType = 9;
    }

    private void DestroyRoom()
    {
        this.isDestroyed = true;
        enemiesGenerationPoint.gameObject.SetActive(false);
        staticObjectsGenerator.gameObject.SetActive(false);
    }

    public void setRoomType(int threatType, int roomType)
    {
        this.threatType = threatType;
        this.roomType = roomType;
        if (roomType == Util.MainRoom) isMainRoom = true;
    }

    private void generateEnemies()
    {
        enemyGenerator.generate(threatType);
    }

    private void generateStaticElements(int type)
    {
        if (isMainRoom)
        {
            staticObjectsGenerator.generate(1, isMainRoom);
        }
        else
        {
            if(threatType != 9 && threatType != 0)
            {
                staticObjectsGenerator.generate(type, isMainRoom);
            }
        }
    }

    public void setDoorSprites(int cols, int rows)
    {
        Sprite notPassRight = doorSprites[0];
        Sprite notPassLeft = doorSprites[1];
        Sprite notPassUp = doorSprites[2];
        Sprite notPassDown = doorSprites[3];

        if (mapLocation.x == 0) leftDoor.GetComponent<SpriteRenderer>().sprite = notPassLeft;
        else if (mapLocation.x == cols - 1) rightDoor.GetComponent<SpriteRenderer>().sprite = notPassRight;

        if (mapLocation.y == 0) upDoor.GetComponent<SpriteRenderer>().sprite = notPassUp;
        else if (mapLocation.y == rows - 1) downDoor.GetComponent<SpriteRenderer>().sprite = notPassDown;
    }

    public void setPosibleDirections(int right, int left, int up, int down)
    {
        posibleDirections[0] = right;
        posibleDirections[1] = left;
        posibleDirections[2] = up;
        posibleDirections[3] = down;
    }

    public bool isOpen(int direction)
    {
        return posibleDirections[direction] == 0;
    }

    /*
    [PunRPC]
    public void startMainRoom()
    {
        OnRoomStarted?.Invoke(this, new OnRoomStartedArgs { initTimer = this.maxTimer });
        timerManager.startRunning(maxTimer);
        crackManager.startEffect(maxTimer);
        GameUIManager.Instance.setState(GameUIStates.GAME_HAS_STARTED);
    }

    public void OnPhotonInstantiate(PhotonMessageInfo info)
    {
        object[] roomInit = info.photonView.InstantiationData;
        this.roomType = (int)roomInit[0];
        this.threatType = (int)roomInit[1];
        int x = (int)roomInit[2];
        int y = (int)roomInit[3];
        this.mapLocation = new Vector2Int(x, y);
        this.setDoorSprites(mapController.cols, mapController.rows);
        mapController.map[x, y] = this;
        this.gameObject.SetActive(false);
        switch (this.roomType)
        {
            case Util.MainRoom:
                isMainRoom = true;
                break;
            case Util.GoodRoom:
                GetComponent<SpriteRenderer>().color = Color.blue;
                break;
            case Util.BadRoom:
                GetComponent<SpriteRenderer>().color = Color.red;
                break;
        }
    }

    public void OnPhotonSerializeView(PhotonStream stream, PhotonMessageInfo info)
    {
        if (stream.IsWriting)
        {
            stream.SendNext(isRoomActive);
        }
        else if (stream.IsReading)
        {
            isRoomActive = (bool)stream.ReceiveNext();
            if (isRoomActive) gameObject.SetActive(true);
        }
    }*/
}
