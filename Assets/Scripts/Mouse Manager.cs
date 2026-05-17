using System;
using Unity.GraphToolkit.Editor;
using UnityEngine;
using UnityEngine.InputSystem;

public class MouseManager : MonoBehaviour
{
    
    [SerializeField] NodeLimitsData nodelimits;
    NodeManager nodeManager;
    ConveyorManager conveyorManager;
    Camera camera;
    int xoffset;
    int yoffset;

    void Start()
    {
        nodeManager = FindAnyObjectByType<NodeManager>();
        conveyorManager = FindAnyObjectByType<ConveyorManager>();
        xoffset = nodelimits.width / 4;
        yoffset = nodelimits.height / 4;
        camera = Camera.main;

    }
    public void LeftClick(InputAction.CallbackContext context)
    {
        if (!context.performed) return;
        Vector2 gridId = GetGridId();
        
        if(nodeManager.CheckStatus(gridId))
        {
            SpawnConveyor(gridId);
        }
    }

    Vector2 GetGridId()
    {
        Vector2 pos = Mouse.current.position.ReadValue();
        Vector3 worldPos = camera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 0));
        Vector2 nodeID = new Vector2((int)Math.Floor(worldPos.x) + xoffset, (int)Math.Floor(worldPos.y) + yoffset);
        return nodeID;
    }

    void SpawnConveyor(Vector2 gridId)
    {
        conveyorManager.NewConveyor(gridId);
    }
}
