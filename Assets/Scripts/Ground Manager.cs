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
        ground.transform.localScale = new Vector3(nodelimits.width / 2, nodelimits.height / 2, 1);
        for (int i = 1; i < nodelimits.width / 2; i++)
        {
            GameObject gridline = Instantiate(gridDot, new Vector3(i - nodelimits.width / 4, 0, 0), quaternion.identity, parent);
            gridline.transform.localScale = new Vector3(0.1f, nodelimits.height / 2, 0.1f);
        }
        for (int i = 1; i < nodelimits.height / 2; i++)
        {
            GameObject gridline = Instantiate(gridDot, new Vector3(0, i - nodelimits.height / 4, 0), quaternion.identity, parent);
            gridline.transform.localScale = new Vector3(nodelimits.width / 2, 0.1f, 0.1f);
        }
    }
}
