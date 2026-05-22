using System.Collections.Generic;
using UnityEngine;

public class ResourceDepositManager : MonoBehaviour
{
    [SerializeField] GameObject depositPrefab;
    [SerializeField] NodeLimitsData nodeLimitsData;

    NodeManager nodeManager;
    GameObject deposit;
    int sideLength;

    List<Vector2> absorbNodes;
    List<Vector2> inputNodes;

    public void SetDepositNodes()
    {
        
        nodeManager = FindAnyObjectByType<NodeManager>();

        deposit = Instantiate(depositPrefab, new Vector3(0,0,0), Quaternion.identity, transform);
        sideLength = nodeLimitsData.width / 20;
        Vector2 bottomleft = new Vector2((nodeLimitsData.width / 4) - 1, (nodeLimitsData.height / 4) - 1);
        absorbNodes.Add(bottomleft);
        absorbNodes.Add(bottomleft + new Vector2(0,1));
        absorbNodes.Add(bottomleft + new Vector2(1,1));
        absorbNodes.Add(bottomleft + new Vector2(1,0));

        // inputNodes.Add(bottomleft + new Vector2(-1, 0));
        // inputNodes.Add(bottomleft + new Vector2(-1, 1));
        // inputNodes.Add(bottomleft + new Vector2(0, 2));
        // inputNodes.Add(bottomleft + new Vector2(1, 2));
        // inputNodes.Add(bottomleft + new Vector2(2, 1));
        // inputNodes.Add(bottomleft + new Vector2(2, 0));
        // inputNodes.Add(bottomleft + new Vector2(1, -1));
        // inputNodes.Add(bottomleft + new Vector2(0, -1));
        
        foreach (Vector2 node in absorbNodes)
        {
            nodeManager.UpdateIsMachine(node, true);
        }
    }

    public void AbsorbResource()
    {
        //
        foreach(Vector2 gridId in absorbNodes)
        {
            GameObject item;
            if (nodeManager.CheckObject(gridId, out item))
            {
                nodeManager.Removeitem(gridId);
            }
        }
    }
}
