using System.Collections.Generic;
using System.Collections;
using System.Linq;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Events;

public class LoseManager : MonoBehaviour
{
    [SerializeField] int startingQuantity = 10;
    [SerializeField] float timeBetweenDepletion = 10;
    [SerializeField] float timeBetweenNewResourceAdded = 30f;
    [SerializeField] ObjectToIdForRecipies objectToIdForRecipies;
    [SerializeField] UnityEvent gameOverEvent;
    Dictionary<int, int> resources = new Dictionary<int, int>();


    int currentResources = 1;

    void Start()
    {
        resources.Add(0, startingQuantity);
        StartCoroutine(Deplete());
        StartCoroutine(NewResource());
    }

    IEnumerator Deplete()
    {
        yield return new WaitForSeconds(timeBetweenDepletion);

        Debug.Log("Resource Depleted");

        var keys = resources.Keys.ToList();  

        foreach (var key in keys)
        {
            resources[key]--;

            if (resources[key] < 1)
            {
                Debug.Log("gameOver");
                gameOverEvent.Invoke();
            }
        }
    }

    IEnumerator NewResource()
    {
        yield return new WaitForSeconds(timeBetweenNewResourceAdded);

        if (currentResources < objectToIdForRecipies.data.Count)
        {
            resources.Add(currentResources, startingQuantity);
        }
    }

    public void AddResource(int id)
    {
        resources[id]++;
        Debug.Log("Resource Added");
    }
}
