using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    
    [SerializeField] NodeLimitsData nodelimits;
    [SerializeField] GameObject testobject;
    NodeManager nodeManager;
    ConveyorManager conveyorManager;
    Camera mainCamera;
    int xoffset;
    int yoffset;

    void Start()
    {
        nodeManager = FindAnyObjectByType<NodeManager>();
        conveyorManager = FindAnyObjectByType<ConveyorManager>();
        xoffset = nodelimits.width / 4;
        yoffset = nodelimits.height / 4;
        mainCamera = Camera.main;

    }

    public void Test(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 gridId = GetGridId();

        if (!nodeManager.IsWithinBounds(gridId)) return;

        if (!nodeManager.CheckStatus(gridId))
        {
            GameObject test = Instantiate(testobject, new Vector3(gridId.x + 0.5f - nodelimits.width / 4, gridId.y + 0.5f - nodelimits.height / 4, -1), Quaternion.identity, transform);
            nodeManager.Additem(gridId, test);
        }
        
    }
    public void LeftClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 gridId = GetGridId();

        if (!nodeManager.IsWithinBounds(gridId)) return;
        
        if(nodeManager.CheckStatus(gridId))
        {
            SpawnConveyor(gridId);
        }
        else
        {
            conveyorManager.Details(gridId);
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
}
