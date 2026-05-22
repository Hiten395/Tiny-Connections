using System.Collections.Generic;
using UnityEngine;


public class NodeManager : MonoBehaviour
{
    [SerializeField] NodeLimitsData nodeLimitsData;

    Node[,] nodes;
    class Node
    {
        public bool isInUse = false;
        public Vector2 owner = new Vector2(-1,-1);
        public GameObject currentObject = null;
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
        
        //Debug.Log("Nodes: " + nodes[0, 0].owner);
    }

    public bool IsWithinBounds(Vector2 id)
    {
        if (nodes == null)
        {
            return false;
        }

        int x = (int)id.x;
        int y = (int)id.y;

        return x >= 0 && y >= 0 && x < nodes.GetLength(0) && y < nodes.GetLength(1);
    }

    bool TryGetNode(Vector2 id, out Node node)
    {
        if (nodes == null || !IsWithinBounds(id))
        {
            node = null;
            return false;
        }

        node = nodes[(int)id.x, (int)id.y];
        return true;
    }

    public bool CheckStatus(Vector2 id)
    {
        return TryGetNode(id, out Node node) && !node.isInUse;
    }

    public void CheckConnections(Vector2 id, out List<Vector2>[] status)
    {
        status = new List<Vector2>[2];

        if (!TryGetNode(id, out Node node))
        {
            status[0] = new List<Vector2>();
            status[1] = new List<Vector2>();
            return;
        }

        status[0] = node.incoming;
        status[1] = node.outgoing;
    }

    public Vector2 CheckOwner(Vector2 id)
    {
        if (!TryGetNode(id, out Node node))
        {
            return new Vector2(-1, -1);
        }

        return node.owner;
    }
    
    public bool CheckObject(Vector2 gridId, out GameObject currentObject)
    {
        if (!TryGetNode(gridId, out Node node) || node.currentObject == null)
        {
            currentObject = null;
            return false;
        }
        else
        {
            currentObject = node.currentObject;
            return true;
        }
    }

    public bool UpdateNode(Vector2 selectedNode, Vector2 outgoingDirection, Vector2 owner)
    {
        Vector2 targetNode = selectedNode + outgoingDirection;
        if (!TryGetNode(selectedNode, out Node workingNode) || !TryGetNode(targetNode, out Node target))
        {
            return false;
        }

        workingNode.isInUse = true;
        workingNode.outgoing.Add(outgoingDirection + selectedNode);
        workingNode.owner = owner;
        target.incoming.Add(selectedNode);
        target.owner = owner;

        return true;
        
    }

    public void Additem(Vector2 gridId, GameObject testObject)
    {
        if (TryGetNode(gridId, out Node node))
        {
            node.currentObject = testObject;
        }
    }

    public bool UpdateNodePostion(Vector2 gridId, Vector2 dir)
    {
        Vector2 targetNode = gridId + dir;
        if (!TryGetNode(gridId, out Node currentNode) || !TryGetNode(targetNode, out Node nextNode))
        {
            return false;
        }

        nextNode.currentObject = currentNode.currentObject;
        currentNode.currentObject = null;
        return true;
    }
}
