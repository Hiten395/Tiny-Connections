using System.Collections.Generic;
using UnityEngine;

public class ResourceDepositManager : MonoBehaviour
{
    [SerializeField] GameObject depositPrefab;
    [SerializeField] NodeLimitsData nodeLimitsData;

    NodeManager nodeManager;
    GameObject deposit;
    int sideLength;

    List<Vector2> absorbNodes = new List<Vector2>();
    List<Vector2> inputNodes = new List<Vector2>();

    public void SetDepositNodes()
    {
        
        absorbNodes.Clear();
        inputNodes.Clear();

        nodeManager = FindAnyObjectByType<NodeManager>();
        if (nodeManager == null || depositPrefab == null || nodeLimitsData == null)
        {
            Debug.LogWarning("ResourceDepositManager is missing a required reference.");
            return;
        }

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
            if (nodeManager.IsWithinBounds(node))
            {
                nodeManager.UpdateIsMachine(node, true);
            }
        }
    }

    public void AbsorbResource()
    {
        //
        if (nodeManager == null)
        {
            return;
        }

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
