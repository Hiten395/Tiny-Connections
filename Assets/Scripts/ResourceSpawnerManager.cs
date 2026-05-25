using System.Collections.Generic;
using System.Collections;
using UnityEngine;
using System;

public class ResourceSpawnerManager : MonoBehaviour
{
    [SerializeField] GameObject spawner;
    [SerializeField] GameObject testObject;
    [SerializeField] NodeLimitsData nodeLimitsData;
    [SerializeField] int ticksBetweenSpawns;
    [SerializeField] int maxSpawns = 10;
    [SerializeField] float timeBetweenNewSpawner;
    int ticks;
    List<Spanwer> spanwers = new List<Spanwer>();
    List<MovingObject> movingObjects = new List<MovingObject>();
    NodeManager nodeManager;
    float elapsed = 0;
    int spawnCounter = 0;
    int spawnDirection = 0;

    class MovingObject
    {
        public GameObject thing;
        public Vector2 dir;
        public bool toDestroy;
        public MovingObject(Vector2 dir2, GameObject thing, bool toDestroy)
        {
            dir = new Vector3(dir2.x, dir2.y, -2);
            this.thing = thing;
            
            this.toDestroy = toDestroy;
        }
    }

    class Spanwer
    {
        public Vector2 outGoingDirection;
        public Vector2 nodeId = new Vector2(-1, -1);
        public GameObject resource;
        public GameObject currentResource;
        public GameObject self;

        public Spanwer(Vector2 nodeId, Vector2 outGoingDirection, GameObject resource, GameObject self)
        {
            this.outGoingDirection = outGoingDirection;
            this.nodeId = nodeId;
            this.resource = resource;
            this.self = self;
        }
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed <= 1)
        {
            foreach (var movingObject in movingObjects)
            {
                if (movingObject.thing == null)
                {
                    continue;
                }

                movingObject.thing.transform.Translate(new Vector3(movingObject.dir.x, movingObject.dir.y, 0)* Time.deltaTime);
            }
        }
        else
        {
            foreach(var block in movingObjects)
            {
                if (block.thing == null)
                {
                    continue;
                }

                if (block.toDestroy)
                {
                    Destroy(block.thing);
                }
                else
                {
                    block.thing.transform.position = new Vector3((float)Math.Floor(block.thing.transform.position.x) + 0.5f, (float)Math.Floor(block.thing.transform.position.y) + 0.5f, -1.5f);
                }
            }
            elapsed = 0;
            movingObjects.Clear();
        }
    }


    void Start()
    {
        ticks = ticksBetweenSpawns - 1;
        nodeManager = FindAnyObjectByType<NodeManager>();
        NewSpawner(testObject);
        StartCoroutine(NewSpawnerRoutine());
    }

    public void NewResource()
    {
        if (nodeManager == null || nodeLimitsData == null)
        {
            return;
        }

        if (!(ticks == ticksBetweenSpawns))
        {
            ticks++;
            return;
        }

        ticks = 0;
        
        foreach (var spawner in spanwers)
        {

            GameObject item;
            if (!nodeManager.CheckObject(spawner.nodeId, out item))
            {
                GameObject newItem = Instantiate(spawner.resource, new Vector3(spawner.nodeId.x + 0.5f - nodeLimitsData.width / 4, spawner.nodeId.y + 0.5f - nodeLimitsData.height / 4, -2f), Quaternion.identity, transform);
                spawner.currentResource = newItem;
                nodeManager.Additem(spawner.nodeId, newItem);                
            }
            else if(!nodeManager.CheckObject(spawner.outGoingDirection, out item))
            {
                nodeManager.CheckObject(spawner.nodeId, out spawner.currentResource);
                Vector2 dir = spawner.outGoingDirection - spawner.nodeId;
                if (nodeManager.CheckEmpty(spawner.outGoingDirection))
                {
                    if (spawner.currentResource != null)
                    {
                        movingObjects.Add(new MovingObject(dir, spawner.currentResource, true));
                    }
                    nodeManager.Removeitem(spawner.nodeId);
                }
                else
                {
                    if (spawner.currentResource != null && nodeManager.UpdateNodePostion(spawner.nodeId, dir))
                    {
                        movingObjects.Add(new MovingObject(dir, spawner.currentResource, false));
                    }
                }
            }
        }
    }
    
    IEnumerator NewSpawnerRoutine()
    {
        while(true)
        {
            yield return new WaitForSeconds(timeBetweenNewSpawner);
            NewSpawner(testObject);
        }
    }
    public void NewSpawner(GameObject resource)
    {
        if (nodeManager == null || spawner == null || nodeLimitsData == null)
        {
            return;
        }
        if (spawnCounter >= maxSpawns)
        {
            return;
        }

        Vector2 gridId;
        Vector2 outgoingDirection;


        spawnCounter++;
        
        RandomSpawn(out gridId, out outgoingDirection);

        GameObject self = Instantiate(spawner, new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -1.5f), Quaternion.Euler(0, 0, spawnDirection), transform);
        if (!nodeManager.UpdateNodeMachine(gridId, outgoingDirection, gridId, self))
        {
            Destroy(self);
            return;
        }

        spanwers.Add(new Spanwer(gridId, gridId + outgoingDirection, resource, self));
    }

    void RandomSpawn(out Vector2 pos, out Vector2 dir)
    {
        System.Random rnd = new System.Random();
        int upperWidth = (int)(nodeLimitsData.width / 2);
        int upperHeight = (int)(nodeLimitsData.height / 2);
        int left = (int)(nodeLimitsData.width / 8);
        int right = (int)(nodeLimitsData.width * (3f/8f));
        int up = (int)(nodeLimitsData.height * (3f/8f));
        int down = (int)(nodeLimitsData.height / 2);

        int leftcoor = rnd.Next(0, left + 1);
        int rightcoor = rnd.Next(right, upperWidth);
        int upcoor = rnd.Next(up, upperHeight);
        int downcoor = rnd.Next(0, down + 1);

        int[] temp1 = {leftcoor, rightcoor};
        int[] temp2 = {upcoor, downcoor};
        int[] temp3 = {0,90,180,270};

        Vector2 randPos = new Vector2(temp1[rnd.Next(0,temp1.Length)], temp2[rnd.Next(0,temp2.Length)]);
        Vector2 randDir = OutGoingDirection(temp3[rnd.Next(0,temp3.Length)]);

        while (!(nodeManager.CheckEmpty(randPos) && nodeManager.CheckEmpty(randPos + randDir) && nodeManager.IsWithinBounds(randPos + randDir)))
        {
            randPos = new Vector2(temp1[rnd.Next(0,temp1.Length)], temp2[rnd.Next(0,temp2.Length)]);
            randDir = OutGoingDirection(temp3[rnd.Next(0,temp3.Length)]);
        }    
        SpawnDirection(randDir);
        pos = randPos;
        dir = randDir;
        return;
    }

    public void SpawnDirection(Vector2 dir)
    {
        if (dir.x == 0)
        {
            if (dir.y == 1)
            {
                spawnDirection = 90;
            }
            else if (dir.y == -1)
            {
                spawnDirection = 270;
            }
        }
        else if (dir.y == 0)
        {
            if (dir.x == 1)
            {
                spawnDirection = 0;
            }
            else if (dir.x == -1)
            {
                spawnDirection = 180;
            }
        }
    }

     Vector2 OutGoingDirection(int dir)
    {
        switch(dir)
        {
            case 0:
                return new Vector2(1,0);
            case 90: 
                return new Vector2(0,1);
            case 180:
                return new Vector2(-1,0);
            case 270:
                return new Vector2(0,-1);
            default:
                Debug.Log("invalid direction return up");
                return new Vector2(0,1);
        }
    }
    
    
}
