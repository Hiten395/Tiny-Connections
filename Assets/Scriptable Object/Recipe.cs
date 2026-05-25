using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;

[CreateAssetMenu(fileName = "Recipe", menuName = "Scriptable Objects/Recipe")]
public class Recipe : ScriptableObject
{
    public List<int> input = new List<int>();
    public ObjectToIdForRecipies data;
    [HideInInspector] public List<int> inputTracker = new List<int>();
    public int output;

    public string description;

    public void ResetTracker()
    {
        inputTracker.Clear();
        inputTracker = new List<int>(input);
    } 

    public GameObject GetOutPut(int id)
    {
        return data.data[id];
    }
}
