using System.Collections.Generic;
using UnityEngine;

public class ResourceDepositManager : MonoBehaviour
{
    [SerializeField] GameObject depositPrefab;
    [SerializeField] NodeLimitsData nodeLimitsData;
    [SerializeField] int timeBetweenNewResourceAdded;

    LoseManager loseManager;

    NodeManager nodeManager;
    //
    List<Vector2> absorbNodes = new List<Vector2>();

    void Start()
    {
        loseManager = FindAnyObjectByType<LoseManager>();
    }

    public void SetDepositNodes()
    {
        absorbNodes.Clear();
        nodeManager = FindAnyObjectByType<NodeManager>();
        loseManager = FindAnyObjectByType<LoseManager>();
        if (nodeManager == null || depositPrefab == null || nodeLimitsData == null)
        {
            Debug.LogWarning("ResourceDepositManager is missing a required reference.");
            return;
        }

        Instantiate(depositPrefab, new Vector3(0,0,0), Quaternion.identity, transform);
        Vector2 bottomleft = new Vector2((nodeLimitsData.width / 4) - 1, (nodeLimitsData.height / 4) - 1);
        absorbNodes.Add(bottomleft);
        absorbNodes.Add(bottomleft + new Vector2(0,1));
        absorbNodes.Add(bottomleft + new Vector2(1,1));
        absorbNodes.Add(bottomleft + new Vector2(1,0));
        
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
        if (nodeManager == null || absorbNodes == null)
        {
            return;
        }

        foreach(Vector2 gridId in absorbNodes)
        {
            GameObject item;
            if (nodeManager.CheckObject(gridId, out item))
            {
                nodeManager.Removeitem(gridId);
                ID itemId = item.GetComponent<ID>();
                if (loseManager != null && itemId != null)
                {
                    loseManager.AddResource(itemId.id);
                }
                Destroy(item);
            }
        }
    }
}
