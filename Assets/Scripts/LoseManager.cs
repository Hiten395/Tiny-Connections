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
    [SerializeField] int quantityOfAddition;
    [SerializeField] float timeBetweenIncreaseInDepletion = 10f;
    [SerializeField] float timeBetweenNewResourceAdded = 30f;
    [SerializeField] int maxQuantity;
    [SerializeField] ObjectToIdForRecipies objectToIdForRecipies;
    [SerializeField] UnityEvent gameOverEvent;
    GameplayHUDUI gameplayHUDUI;
    Dictionary<int, int> resources = new Dictionary<int, int>();


    int currentResources = 1;
    bool gameOverTriggered;

    public List<float> supplyPercent = new List<float>();

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
        supplyPercent.Add((float)startingQuantity / (float)maxQuantity);
        gameplayHUDUI = FindAnyObjectByType<GameplayHUDUI>();
        StartCoroutine(Deplete());
        //StartCoroutine(NewResource());
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
                supplyPercent[key] = (float)resources[key] / (float)maxQuantity;
                //Debug.Log(supplyPercent[key]);

                if (resources[key] < 1)
                {
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
            Debug.Log(quantityOfDepletion + "resources depletion increased");
            quantityOfDepletion++;
        }
    }
    public void AddResource(int id, int value)
    {
        resources[id] += value;
    }
}
