using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    public enum BuildMode
    {
        Conveyor,
        Spawner,
        Machine,
        Delete
    }

    [SerializeField] NodeLimitsData nodelimits;


    [SerializeField] GameObject testobject;
    [SerializeField] Recipe recipe;
    [SerializeField] bool spawnProcessor;
    [SerializeField] bool spawnTestobject;
    [SerializeField] BuildMode currentBuildMode = BuildMode.Conveyor;
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

        switch (currentBuildMode)
        {
            case BuildMode.Conveyor:
                if (nodeManager.CheckEmpty(gridId))
                {
                    SpawnConveyor(gridId);
                }
                else if (nodeManager.CheckStatus(gridId)[0])
                {
                    conveyorManager.Details(gridId);
                }
                break;
            case BuildMode.Spawner:
                resourceSpawnerManager.NewSpawner(testobject);
                break;
            case BuildMode.Machine:
                machineManager.NewMachine(gridId, recipe);
                break;
            case BuildMode.Delete:
                DeleteConveyor(gridId);
                break;
        }
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

    void DeleteConveyor(Vector2 gridId)
    {
        if (!nodeManager.IsWithinBounds(gridId) || nodeManager.CheckStatus(gridId)[1]) return;

        conveyorManager.DeleteConveyor(gridId);
    }

    public void SetBuildMode(BuildMode buildMode)
    {
        currentBuildMode = buildMode;
    }

    public void SetConveyorMode()
    {
        SetBuildMode(BuildMode.Conveyor);
    }

    public void SetSpawnerMode()
    {
        SetBuildMode(BuildMode.Spawner);
    }

    public void SetMachineMode()
    {
        SetBuildMode(BuildMode.Machine);
    }

    public void SetDeleteMode()
    {
        SetBuildMode(BuildMode.Delete);
    }
}
