using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AnimationsControl : MonoBehaviour
{
    [SerializeField] GameObject[] berries;
    [SerializeField] GameObject[] roots;
    [SerializeField] GameObject[] herbs;
    [SerializeField] GameObject[] berriesDust;

    public Animation mortarAnime;


    public Collider[] bowlColliders;

    public static AnimationsControl Instance { get; private set; }

    private void Awake()
    {
        // Синглтон, для надёжности
        if (Instance != null && Instance != this)
        {
            Destroy(this);
        }
        else
        {
            Instance = this;
            DontDestroyOnLoad(gameObject);
        }

        if (bowlColliders == null || bowlColliders.Length != 3)
        {
            Debug.LogError("Необходимо назначить 3 коллайдера мисок в массиве bowlColliders в инспекторе!");
        }
    }

    public bool IsNearCorrectBowl(GameObject obj)
    {
        int correctBowlIndex = GetCorrectBowlIndex(obj);

        if (correctBowlIndex == -1)
        {
            return false; // Неизвестный тег объекта
        }

        // Проверяем, пересекает ли объект коллайдер правильной миски
        return IsOverlappingCollider(obj, correctBowlIndex);
    }

    private bool IsOverlappingCollider(GameObject obj, int bowlIndex)
    {
        if (bowlColliders == null || bowlIndex < 0 || bowlIndex >= bowlColliders.Length || bowlColliders[bowlIndex] == null)
        {
            Debug.LogError("Неверный индекс коллайдера или коллайдер не назначен!");
            return false;
        }

        // Используем перекрытие Bounds для более надежной проверки
        return bowlColliders[bowlIndex].bounds.Intersects(obj.GetComponent<Collider>().bounds);
    }
    public int GetCorrectBowlIndex(GameObject obj)
    {
        if (obj.CompareTag("roots"))
        {
            return 0; // Для roots правильная миска с индексом 0
        }
        else if (obj.CompareTag("bowl"))
        {
            return 1; // Для berries правильная миска с индексом 1
        }
        else if (obj.CompareTag("herbs") || obj.CompareTag("berries"))
        {
            return 2; // Для herbs правильная миска с индексом 2
        }
        else
        {
            Debug.LogWarning("Неизвестный тег объекта: " + obj.tag);
            return -1; // Если тег не распознан, возвращаем -1
        }
    }
    public float PlayMortarAnimation()
    {
        if (mortarAnime != null)
        {
            mortarAnime.Play("MortarAnimation");
            return mortarAnime["MortarAnimation"].length;
        }
        else Debug.Log("Animation нет на ступке");
        return 0;
    }
    public void BerriesDustOn(int index)
    {
        BerriesOn(-1);
        for (int i = 0; i < berriesDust.Length; i++)
        {
            berriesDust[i].SetActive(false);
        }
        berriesDust[index - 1].SetActive(true);
    }
    public void BerriesOn(int index)
    {
        for (int i = 0; i < berries.Length; i++)
        {
            berries[i].SetActive(false);
        }
        if (index > 0)
        {
            berries[index - 1].SetActive(true);
        }
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
