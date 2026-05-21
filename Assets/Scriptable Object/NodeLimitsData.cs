using UnityEngine;

[CreateAssetMenu(fileName = "NodeLimitsData", menuName = "Scriptable Objects/NodeLimitsData")]
public class NodeLimitsData : ScriptableObject
{
    [Tooltip("Ensure both width and height are divisible by 20")]
    public int width;
    [Tooltip("Ensure both width and height are divisible by 20")]
    public int height;
}
