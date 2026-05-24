using System.Collections.Generic;
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
        public Vector2 owner = new Vector2(-1, -1);
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
        if (!IsWithinBounds(id))
        {
            node = null;
            return false;
        }

        node = nodes[(int)id.x, (int)id.y];
        return true;
    }

    // returns false, false if node can be occupied, true, false if conveyor, false, true if machine
    public bool[] CheckStatus(Vector2 id)
    {
        bool[] res = new bool[2];
        if (!TryGetNode(id, out Node node))
        {
            return res;
        }

        res[0] = node.isInUse;
        res[1] = node.isMachine;
        return res;
    }

    public bool CheckEmpty(Vector2 id)
    {
        return TryGetNode(id, out Node node) && !node.isInUse && !node.isMachine;
    }

    // returns the incoming and outgoing directions of selected nodes as absolute values
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

    // returns the owner (origin of the conveyor)
    public Vector2 CheckOwner(Vector2 id)
    {
        if (!TryGetNode(id, out Node node))
        {
            return new Vector2(-1, -1);
        }

        return node.owner;
    }

    // returns does the node contain an active transport object along with a reference
    public bool CheckObject(Vector2 gridId, out GameObject currentObject)
    {
        if (!TryGetNode(gridId, out Node node) || node.currentObject == null)
        {
            currentObject = null;
            return false;
        }

        currentObject = node.currentObject;
        return true;
    }

    public void UpdateIsMachine(Vector2 gridId, bool state)
    {
        if (TryGetNode(gridId, out Node workingNode))
        {
            workingNode.isMachine = state;
        }
    }

    // updates the nodes with the given information
    public bool UpdateNode(Vector2 selectedNode, Vector2 outgoingDirection, Vector2 owner, GameObject nodeObject)
    {
        Vector2 targetNode = selectedNode + outgoingDirection;
        if (!TryGetNode(selectedNode, out Node workingNode) || !TryGetNode(targetNode, out Node target))
        {
            return false;
        }

        workingNode.nodeObject = nodeObject;
        workingNode.isInUse = true;
        workingNode.outgoing.Add(targetNode);
        workingNode.owner = owner;
        target.incoming.Add(selectedNode);
        target.owner = owner;
        return true;
    }
    
    public bool UpdateNodeMachine(Vector2 selectedNode, Vector2 outgoingDirection, Vector2 owner, GameObject nodeObject)
    {
        Vector2 targetNode = selectedNode + outgoingDirection;
        if (!TryGetNode(selectedNode, out Node workingNode) || !TryGetNode(targetNode, out Node target))
        {
            return false;
        }

        workingNode.nodeObject = nodeObject;
        workingNode.isInUse = true;
        workingNode.outgoing.Add(targetNode);
        workingNode.owner = owner;
        return true;
    }
    public bool UpdateNode(Vector2 selectedNode, Vector2 outgoingDirection, Vector2 owner)
    {
        return UpdateNode(selectedNode, outgoingDirection, owner, null);
    }

    // sets the outgoing direction for a given node
    public void SetOutGoingDirection(Vector2 selectedNode, Vector2 relativeDirection)
    {
        if (TryGetNode(selectedNode, out Node workingNode))
        {
            workingNode.outgoing.Add(selectedNode + relativeDirection);
        }
    }

    public void SetIncomingDirection(Vector2 selectedNode, Vector2 relativeDirection)
    {
        Node workingNode = nodes[(int)selectedNode.x, (int)selectedNode.y];
        workingNode.incoming.Add(selectedNode + relativeDirection);
    }
    // updates the owner of a selected node
    public void UpdateOwner(Vector2 gridId, Vector2 owner)
    {
        if (TryGetNode(gridId, out Node node))
        {
            node.owner = owner;
        }
    }

    // resets the node, will not work if the node is occupied by a machine
    public void ResetNode(Vector2 selectedNode)
    {
        if (!TryGetNode(selectedNode, out Node workingNode) || workingNode.isMachine)
        {
            return;
        }

        workingNode.isInUse = false;
        Destroy(workingNode.nodeObject);

        if (workingNode.outgoing.Count > 0 && TryGetNode(workingNode.outgoing[0], out Node nextNode))
        {
            nextNode.incoming.Remove(selectedNode);
        }

        workingNode.outgoing.Clear();
        workingNode.nodeObject = null;
    }

    // adds a transport item to that passed node
    public void Additem(Vector2 gridId, GameObject testObject)
    {
        if (TryGetNode(gridId, out Node node))
        {
            node.currentObject = testObject;
        }
    }

    public void Removeitem(Vector2 gridId)
    {
        if (TryGetNode(gridId, out Node node))
        {
            node.currentObject = null;
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
