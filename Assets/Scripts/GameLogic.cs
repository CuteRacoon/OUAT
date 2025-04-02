using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    [SerializeField] GameObject[] berries;
    [SerializeField] GameObject[] roots;
    [SerializeField] GameObject[] herbs;

    public Collider[] bowlColliders; // ��������� �������� � ����������

    private void Awake()
    {
        if (bowlColliders == null || bowlColliders.Length != 3)
        {
            Debug.LogError("���������� ��������� 3 ���������� ����� � ������� bowlColliders � ����������!");
        }
    }

    public bool IsNearCorrectBowl(GameObject obj)
    {
        int correctBowlIndex = GetCorrectBowlIndex(obj);

        if (correctBowlIndex == -1)
        {
            return false; // ����������� ��� �������
        }

        // ���������, ���������� �� ������ ��������� ���������� �����
        return IsOverlappingCollider(obj, correctBowlIndex);
    }

    private bool IsOverlappingCollider(GameObject obj, int bowlIndex)
    {
        if (bowlColliders == null || bowlIndex < 0 || bowlIndex >= bowlColliders.Length || bowlColliders[bowlIndex] == null)
        {
            Debug.LogError("�������� ������ ���������� ��� ��������� �� ��������!");
            return false;
        }

        // ���������� ���������� Bounds ��� ����� �������� ��������
        return bowlColliders[bowlIndex].bounds.Intersects(obj.GetComponent<Collider>().bounds);
    }
    private int GetCorrectBowlIndex(GameObject obj)
    {
        if (obj.CompareTag("roots"))
        {
            return 0; // ��� roots ���������� ����� � �������� 0
        }
        else if (obj.CompareTag("bowl"))
        {
            return 1; // ��� berries ���������� ����� � �������� 1
        }
        else if (obj.CompareTag("herbs") || obj.CompareTag("berries"))
        {
            return 2; // ��� herbs ���������� ����� � �������� 2
        }
        else
        {
            Debug.LogWarning("����������� ��� �������: " + obj.tag);
            return -1; // ���� ��� �� ���������, ���������� -1
        }
    }
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
    public void HerbsOn(int index)
    {
        for (int i = 0; i < herbs.Length; i++)
        {
            herbs[i].SetActive(false);
        }
        herbs[index - 1].SetActive(true);
    }
}
