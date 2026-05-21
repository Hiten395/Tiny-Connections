using System;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;


public class NodeManager : MonoBehaviour
{
    [SerializeField] NodeLimitsData nodeLimitsData;

    ResourceDepositManager resourceDepositManager;

    Node[,] nodes;
    class Node
    {
        public bool isInUse = false;
        public bool isMachine = false;
        public Vector2 owner = new Vector2(-1,-1);
        public GameObject currentObject = null;
        public GameObject nodeObject = null;
        public List<Vector2> incoming = new List<Vector2>();
        public List<Vector2> outgoing = new List<Vector2>();
    }
    
    void Start()
    {
        nodes = new Node[nodeLimitsData.width / 2, nodeLimitsData.height / 2];
        
        for (int x = 0; x < nodes.GetLength(0); x++)
        {
            for (int y = 0; y < nodes.GetLength(1); y++)
            {
                nodes[x, y] = new Node();
            }
        }
        
        resourceDepositManager = FindAnyObjectByType<ResourceDepositManager>();
        resourceDepositManager.SetDepositNodes();
    }

    // returns false, false if node can be occupied, true, false if conveyor, false, true if machine
    public bool[] CheckStatus(Vector2 id)
    {
        // returns false, false if node can be occupied
        // true, false if conveyor
        // false, true if machine
        Node node = nodes[(int)id.x, (int)id.y];
        bool[] res = new bool[2];
        res[0] = node.isInUse;
        res[1] = node.isMachine;
        return res;
    }

    public bool CheckEmpty(Vector2 id)
    {
        Node node = nodes[(int)id.x, (int)id.y];
        if (node.isInUse == false && node.isMachine == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }
    // returns the incoming and outgoing directions of selected nodes as absolute values
    public void CheckConnections(Vector2 id, out List<Vector2>[] status)
    {
        status = new List<Vector2>[2];
        status[0] = nodes[(int)id.x, (int)id.y].incoming;
        status[1] = nodes[(int)id.x, (int)id.y].outgoing;
    }

    // returns the owner (origin of the conveyor)
    public Vector2 CheckOwner(Vector2 id)
    {
        return nodes[(int)id.x, (int)id.y].owner;
    }
    
    // returns does the node contain an active transport object along with a refrence
    public bool CheckObject(Vector2 gridId, out GameObject currentObject)
    {
        if (nodes[(int)gridId.x, (int)gridId.y].currentObject == null)
        {
            currentObject = null;
            return false;
        }
        else
        {
            currentObject = nodes[(int)gridId.x, (int)gridId.y].currentObject;
            return true;
        }
    }

    public void UpdateIsMachine(Vector2 gridId, bool state)
    {
        Node workingNode = nodes[(int)gridId.x, (int)gridId.y];
        workingNode.isMachine = state;
    }
    

    // updates the nodes with the given information
    public void UpdateNode(Vector2 selectedNode, Vector2 outgoingDirection, Vector2 owner, GameObject nodeObject)
    {
        Node workingNode = nodes[(int)selectedNode.x, (int)selectedNode.y];
        workingNode.nodeObject = nodeObject;
        workingNode.isInUse = true;
        workingNode.outgoing.Add(outgoingDirection + selectedNode);
        workingNode.owner = owner;
        workingNode = nodes[(int)selectedNode.x + (int)outgoingDirection.x, (int)selectedNode.y + (int)outgoingDirection.y];
        workingNode.incoming.Add(selectedNode);
        workingNode.owner = owner;
    }

    // sets the outgoing direction for a given node
    public void SetOutGoingDirection(Vector2 selectedNode, Vector2 relativeDirection)
    {
        Node workingNode = nodes[(int)selectedNode.x, (int)selectedNode.y];
        workingNode.outgoing.Add(selectedNode + relativeDirection);
    }

    // updates the owner of a selected node
    public void UpdateOwner(Vector2 gridId, Vector2 owner)
    {
        nodes[(int)gridId.x, (int)gridId.y].owner = owner;
    }

    // resets the node, will not work if the node is occupied by a machine
    public void ResetNode(Vector2 selectedNode)
    {
        Node workingNode = nodes[(int)selectedNode.x, (int)selectedNode.y];
        if (!workingNode.isMachine)
        {
            workingNode.isInUse = false;
            Destroy(workingNode.nodeObject);
            Node nextNode = nodes[(int)workingNode.outgoing[0].x, (int)workingNode.outgoing[0].y];
            workingNode.outgoing.Clear();
            nextNode.incoming.Remove(selectedNode);
        }
    }

    // adds an transport item to that passed node
    public void Additem(Vector2 gridId, GameObject testObject)
    {
        nodes[(int)gridId.x, (int)gridId.y].currentObject = testObject;
    }
    
    public void Removeitem(Vector2 gridId)
    {
        nodes[(int)gridId.x, (int)gridId.y].currentObject = null;
    }
    public void UpdateNodePostion(Vector2 gridId, Vector2 dir)
    {
        nodes[(int)(gridId.x + dir.x), (int)(gridId.y + dir.y)].currentObject = nodes[(int)gridId.x, (int)gridId.y].currentObject;
        nodes[(int)gridId.x, (int)gridId.y].currentObject = null;
        // GameObject temp;
        // Debug.Log(CheckObject(gridId, out temp));
        // Debug.Log(CheckObject(gridId + dir, out temp));
    }
}
