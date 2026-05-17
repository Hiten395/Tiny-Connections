using System.Collections.Generic;
using Unity.VisualScripting;
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

    public bool CheckStatus(Vector2 id)
    {
        Node node = nodes[(int)id.x, (int)id.y];
        if (node.isInUse == false)
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void CheckConnections(Vector2 id, out List<Vector2>[] status)
    {
        status = new List<Vector2>[2];
        status[0] = nodes[(int)id.x, (int)id.y].incoming;
        status[1] = nodes[(int)id.x, (int)id.y].outgoing;
    }

    public Vector2 CheckOwner(Vector2 id)
    {
        return nodes[(int)id.x, (int)id.y].owner;
    }
    
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

    public void UpdateNode(Vector2 selectedNode, Vector2 outgoingDirection, Vector2 owner)
    {
        Node workingNode = nodes[(int)selectedNode.x, (int)selectedNode.y];
        workingNode.isInUse = true;
        workingNode.outgoing.Add(outgoingDirection + selectedNode);
        workingNode.owner = owner;
        workingNode = nodes[(int)selectedNode.x + (int)outgoingDirection.x, (int)selectedNode.y + (int)outgoingDirection.y];
        workingNode.incoming.Add(selectedNode);
        workingNode.owner = owner;
        
    }

    public void Additem(Vector2 gridId, GameObject testObject)
    {
        nodes[(int)gridId.x, (int)gridId.y].currentObject = testObject;
    }

    public void UpdateNodePostion(Vector2 gridId, Vector2 dir)
    {
        nodes[(int)(gridId.x + dir.x), (int)(gridId.y + dir.y)].currentObject = nodes[(int)gridId.x, (int)gridId.y].currentObject;
        nodes[(int)gridId.x, (int)gridId.y].currentObject = null;
    }
}
