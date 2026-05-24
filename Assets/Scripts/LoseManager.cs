using System.Collections.Generic;
using System.Collections;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

public class LoseManager : MonoBehaviour
{
    [SerializeField] int startingQuantity = 10;
    [SerializeField] float timeBetweenDepletion = 10;
    [SerializeField] int quantityOfDepletion;
    [SerializeField] float timeBetweenIncreaseInDepletion = 10f;
    [SerializeField] float timeBetweenNewResourceAdded = 30f;
    [SerializeField] ObjectToIdForRecipies objectToIdForRecipies;
    [SerializeField] UnityEvent gameOverEvent;
    Dictionary<int, int> resources = new Dictionary<int, int>();


    int currentResources = 1;
    bool gameOverTriggered;

    public float SupplyPercent
    {
        get
        {
            if (resources.Count == 0 || startingQuantity <= 0)
            {
                return 1f;
            }

            int lowestResource = resources.Values.Min();
            return Mathf.Clamp01((float)lowestResource / startingQuantity);
        }
    }

    public int LowestResourceCount
    {
        get
        {
            if (resources.Count == 0)
            {
                return startingQuantity;
            }

            return resources.Values.Min();
        }
    }

    void Start()
    {
        resources.Add(0, startingQuantity);
        StartCoroutine(Deplete());
        StartCoroutine(NewResource());
        StartCoroutine(IncreaseDepletion());
    }

    IEnumerator Deplete()
    {
        while (!gameOverTriggered)
        {
            yield return new WaitForSeconds(timeBetweenDepletion);

            var keys = resources.Keys.ToList();

            foreach (var key in keys)
            {
                resources[key] -= quantityOfDepletion;

                if (resources[key] < 1)
                {
                    Debug.Log("gameOver");
                    gameOverTriggered = true;
                    gameOverEvent.Invoke();
                    yield break;
                }
            }
        }
    }
    IEnumerator IncreaseDepletion()
    {
        while (!gameOverTriggered)
        {
            yield return new WaitForSeconds(timeBetweenIncreaseInDepletion);
            quantityOfDepletion++;
        }
    }
    IEnumerator NewResource()
    {
        while (!gameOverTriggered)
        {
            yield return new WaitForSeconds(timeBetweenNewResourceAdded);
            Debug.Log(currentResources);
            Debug.Log(objectToIdForRecipies.data.Count);

            if (objectToIdForRecipies != null && currentResources < objectToIdForRecipies.data.Count)
            {
                Debug.Log("new resource to add to account for");
                resources.Add(currentResources, startingQuantity);
                currentResources++;
            }
        }
    }
    public void AddResource(int id)
    {
        resources[id]++;
        //Debug.Log("Resource Added");
    }
}
