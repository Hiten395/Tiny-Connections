using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using System;
using NUnit.Framework;
public class MachineManage : MonoBehaviour
{
    class Machine
    {
        public Vector2 gridId;
        public GameObject machine;
        public GameObject currentOutput = null;
        public Vector2 outgoingDirection;
        public Recipe recipe;
        

        public Machine(Vector2 gridId, Vector2 outgoingDirection, GameObject machine, Recipe recipe)
        {
            this.gridId = gridId;
            this.outgoingDirection = outgoingDirection;
            this.machine = machine;
            this.recipe = recipe;
        }
    }
    class MovingObject
    {
        public Vector3 dir;
        public GameObject thing;
        public bool toDestroy;
        public Vector3 gridId;

        public MovingObject(Vector2 pos, Vector2 dir2, GameObject thing, bool toDestroy)
        {
            dir = new Vector3(dir2.x, dir2.y, 0);
            gridId = new Vector3(pos.x, pos.y, 0);
            
            this.thing = thing;
            this.toDestroy = toDestroy;
        }
    }
    [SerializeField] NodeLimitsData nodeLimitsData;
    [SerializeField] GameObject machine;
    NodeManager nodeManager;
    float elapsed = 0f;
    int spawnDirection = 0;
    public float waitTimeForProcessing = 1f;

    List<MovingObject> movingObjects = new List<MovingObject>();
    Dictionary<Vector2, Machine> machines = new Dictionary<Vector2, Machine>();

    void Start()
    {
        nodeManager = FindAnyObjectByType<NodeManager>();
    }

    void Update()
    {
        elapsed += Time.deltaTime;

        if (elapsed <= 1)
        {
            foreach (var block in movingObjects)
            {
                Debug.Log(block.dir);
                block.thing.transform.Translate(block.dir * Time.deltaTime);
            }
        }
        else
        {
            foreach (var block in movingObjects)
            {
                if (block.toDestroy)
                {
                    Destroy(block.thing); 
                }
                else
                {
                    nodeManager.Additem(block.dir + block.gridId, block.thing);
                }
            }

            elapsed = 0;
            movingObjects.Clear();
        }
    }



    public void NewMachine(Vector2 gridId, Recipe recipe)
    {
        if (nodeManager.CheckEmpty(gridId))
        {
            Debug.Log(gridId + " " + OutGoingDirection(spawnDirection));
            Debug.Log(gridId + OutGoingDirection(spawnDirection));
            GameObject instance = Instantiate(machine, new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -10), Quaternion.Euler(0,0,spawnDirection), transform);
            Machine machine2 = new Machine(gridId, gridId + OutGoingDirection(spawnDirection), instance, recipe);
            machine2.recipe.ResetTracker();
            machines.Add(gridId, machine2);
            nodeManager.UpdateIsMachine(gridId, true);

        }
    }
    public void Process()
    {
        foreach(var machine in machines)
        {
            Vector2 gridId = machine.Value.gridId;
            Recipe recipe = machine.Value.recipe;
            GameObject currentOutput = machine.Value.currentOutput;
            Vector2 outgoingDirection = machine.Value.outgoingDirection;
            GameObject gridObject;

            
            if (nodeManager.CheckObject(gridId, out gridObject))
            {
                int id = gridObject.GetComponent<ID>().id;
                nodeManager.Removeitem(gridId);
                recipe.inputTracker.Remove(id);
                if (recipe.inputTracker.Count == 0 && currentOutput == null)
                {
                    StartCoroutine(ManufatureOutput(machine.Value));
                }
            }
        }
    }

    IEnumerator ManufatureOutput(Machine machine)
    {
        Vector2 gridId = machine.gridId;
        Recipe recipe = machine.recipe;
        Vector2 outgoingDirection = machine.outgoingDirection;
        GameObject gridObject;

        yield return new WaitForSeconds(waitTimeForProcessing);
        GameObject currentItem = Instantiate(recipe.GetOutPut(recipe.output), new Vector3(gridId.x + 0.5f - nodeLimitsData.width / 4, gridId.y + 0.5f - nodeLimitsData.height / 4, -2f), Quaternion.identity, transform);
        GameObject temp;
        Vector2 dir = outgoingDirection - gridId;
        Debug.Log(dir);
        Debug.Log(outgoingDirection + " " + gridId);
        if (!nodeManager.CheckObject(outgoingDirection, out temp))
        {
            if (nodeManager.CheckEmpty(outgoingDirection))
            {
                movingObjects.Add(new MovingObject(gridId, dir, currentItem, true));
            }
            else
            {
                movingObjects.Add(new MovingObject(gridId, dir, currentItem, false));
            }
        }
        recipe.ResetTracker();

    }

    Vector2 OutGoingDirection(int dir)
    {
        switch (dir)
        {
            case 0:
                return new Vector2(1, 0);
            case 90:
                return new Vector2(0, 1);
            case 180:
                return new Vector2(-1, 0);
            case 270:
                return new Vector2(0, -1);
            default:
                Debug.Log("invalid direction return up");
                return new Vector2(0, 1);
        }
    }

    // sets direction using user input
    public void SetOutGoingDirection(Vector2 dir)
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

}
