using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] GameObject[] berries;
    [SerializeField] GameObject[] roots;
    [SerializeField] GameObject[] bowls;
    [SerializeField] Collider[] bowlsColliders;

    public void BerriesOn(int index)
    {
        for (int i = 0; i < berries.Length; i++)
        {
            berries[i].SetActive(false);
        }
        berries[index - 1].SetActive(true);
    }
    public void RootsOn(int index)
    {
        for (int i = 0; i < roots.Length; i++)
        {
            roots[i].SetActive(false);
        }
        roots[index - 1].SetActive(true);
    }
}
