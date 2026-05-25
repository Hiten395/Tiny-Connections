using Unity.Mathematics;
using UnityEngine;

public class GroundManager : MonoBehaviour
{
    [SerializeField] NodeLimitsData nodelimits;
    [SerializeField] GameObject ground;
    [SerializeField] GameObject gridDot;
    [SerializeField] Transform parent;

    void Start()
    {
        ground.transform.localScale = new Vector3(nodelimits.width / 4, nodelimits.height / 4, 1);
    }
}
