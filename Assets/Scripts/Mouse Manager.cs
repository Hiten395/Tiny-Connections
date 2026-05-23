using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    [SerializeField] NodeLimitsData nodelimits;


    /// <summary>
    /// The below two variable i.e. testobject and recipe tell the spwner and processor
    /// resource / recipe to follow. Function to maipulate these values can be added for usage
    /// </summary>
    [SerializeField] GameObject testobject;
    [SerializeField] Recipe recipe;
    [SerializeField] bool spawnProcessor;
    [SerializeField] bool spawnTestobject;
    NodeManager nodeManager;
    ConveyorManager conveyorManager;
    ResourceSpawnerManager resourceSpawnerManager;
    MachineManage machineManager;
    Camera mainCamera;
    int xoffset;
    int yoffset;

    void Start()
    {
        nodeManager = FindAnyObjectByType<NodeManager>();
        conveyorManager = FindAnyObjectByType<ConveyorManager>();
        resourceSpawnerManager = FindAnyObjectByType<ResourceSpawnerManager>();
        machineManager = FindAnyObjectByType<MachineManage>();
        xoffset = nodelimits.width / 4;
        yoffset = nodelimits.height / 4;
        mainCamera = Camera.main;
    }

    public void Test(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 gridId = GetGridId();

        if (nodeManager.CheckEmpty(gridId) && spawnProcessor)
        {
            machineManager.NewMachine(gridId, recipe);
            return;
        }

        if (!nodeManager.IsWithinBounds(gridId)) return;

        if (nodeManager.CheckEmpty(gridId) && spawnTestobject)
        {
            GameObject test = Instantiate(testobject, new Vector3(gridId.x + 0.5f - nodelimits.width / 4, gridId.y + 0.5f - nodelimits.height / 4, -1), Quaternion.identity, transform);
            nodeManager.Additem(gridId, test);
        }
        else if (!spawnTestobject)
        {
            resourceSpawnerManager.NewSpawner(gridId, testobject);
        }
    }

    public void Direction(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 dir = context.ReadValue<Vector2>();
        conveyorManager.SetOutGoingDirection(dir);
        resourceSpawnerManager.SpawnDirection(dir);
        machineManager.SetOutGoingDirection(dir);
    }

    public void LeftClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 gridId = GetGridId();

        if (!nodeManager.IsWithinBounds(gridId)) return;

        if (nodeManager.CheckEmpty(gridId))
        {
            SpawnConveyor(gridId);
        }
        else if (nodeManager.CheckStatus(gridId)[0])
        {
            conveyorManager.Details(gridId);
        }
    }

    public void RightClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 gridId = GetGridId();

        if (!nodeManager.IsWithinBounds(gridId) || nodeManager.CheckStatus(gridId)[1]) return;

        conveyorManager.DeleteConveyor(gridId);
    }

    Vector2 GetGridId()
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        Vector3 worldPos = mainCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 0));
        Vector2 nodeID = new Vector2((int)Math.Floor(worldPos.x) + xoffset, (int)Math.Floor(worldPos.y) + yoffset);
        return nodeID;
    }

    void SpawnConveyor(Vector2 gridId)
    {
        conveyorManager.NewConveyor(gridId);
    }
}
