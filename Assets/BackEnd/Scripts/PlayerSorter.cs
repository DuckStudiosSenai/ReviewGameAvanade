using UnityEngine;
using UnityEngine.Rendering;

public class PlayerSorter : MonoBehaviour
{
    private SortingGroup sg;

    void Awake()
    {
        sg = GetComponent<SortingGroup>();
    }

    void LateUpdate()
    {
        sg.sortingOrder = 5000 -(int)(transform.position.y * 100);
    }
}
