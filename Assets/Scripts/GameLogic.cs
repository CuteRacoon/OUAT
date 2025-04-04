using System.Collections.Generic;
using System.Runtime.Serialization;
using Unity.VisualScripting;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public GameObject[] objectsToOutline;

    private static GameObject currentObject;
    private AnimationsControl animationsControl;
    private int neededBowlIndex = -1;

    // ������� ��� �������� �������� �� ����� � ��������
    public Dictionary<int, GameObject> Roots { get; private set; } = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> Berries { get; private set; } = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> Bowls { get; private set; } = new Dictionary<int, GameObject>();
    public Dictionary<int, GameObject> Herbs { get; private set; } = new Dictionary<int, GameObject>();

    public List<KeyValuePair<int, int>> CollectedObjects { get; private set; } = new List<KeyValuePair<int, int>>();

    private void Start()
    {
        animationsControl = FindAnyObjectByType<AnimationsControl>();
        currentObject = null;
        PopulateResources();
        Bowls[2].GetComponent<Bowls>().enabled = false; // ��������� ������ ����� �� ��������������
        
    }

    public void AddToObjectsList(int index, int objectIndicator)
    {
        CollectedObjects.Add(new KeyValuePair<int, int>(index, objectIndicator));
        Debug.Log("� ������ ������������ �������� " + index + "-� ������ ������ " + objectIndicator);
    }
    public void AccessBowlsInteraction(bool scriptOn)
    { 
        // ��������� ������� � ���� Roots, Berries � Herbs
        DisableScripts(Roots, scriptOn);
        DisableScripts(Berries, scriptOn);
        DisableScripts(Herbs, scriptOn);
        DisableScripts(Bowls, scriptOn);
    }

    // ��������������� ����� ��� ���������� �������� � ���� �������� � �������
    private void DisableScripts(Dictionary<int, GameObject> dictionary, bool scriptOn)
    {
        foreach (var pair in dictionary)
        {
            GameObject obj = pair.Value;

            // �������� � ��������� ������� � ����������� �� ���� �������
            if (obj.GetComponent<Roots>() != null)
            {
                obj.GetComponent<Roots>().enabled = scriptOn;
            }
            if (obj.GetComponent<Berries>() != null)
            {
                obj.GetComponent<Berries>().enabled = scriptOn;
            }
            if (obj.GetComponent<Herbs>() != null)
            {
                obj.GetComponent<Herbs>().enabled = scriptOn;
            }
            if (obj.GetComponent<Bowls>() != null)
            {
                obj.GetComponent<Bowls>().enabled = scriptOn;
            }
        }
    }
    public void SetCurrentObject(GameObject obj)
    {
        currentObject = obj;
        Debug.Log("CurrentObject is" + currentObject.name);
        neededBowlIndex = animationsControl.GetCorrectBowlIndex(currentObject);
        OutlineObject(neededBowlIndex, true);
    }

    public void NullCurrentObject()
    {
        currentObject = null;
        Debug.Log("CurrentObject is null");
        OutlineObject(neededBowlIndex, false);
    }
    public void OutlineObject(int index, bool enable)
    {
        Interactable interactable = objectsToOutline[index].GetComponent<Interactable>();
        if (interactable != null)
        {
            interactable.OutlineOn(enable);
        }
        else
        {
            Debug.Log("Interactable is null in ControlOneOutline");
        }
    }

    void PopulateResources()
    {
        // ����� �������� �� ���� � ���������� �� � ��������������� �������
        FindAndAddResources("roots", Roots);
        FindAndAddResources("berries", Berries);
        FindAndAddResources("bowl", Bowls);
        FindAndAddResources("herbs", Herbs);
    }

    void FindAndAddResources(string tag, Dictionary<int, GameObject> dictionary)
    {
        GameObject[] objects = GameObject.FindGameObjectsWithTag(tag);

        foreach (GameObject obj in objects)
        {
            int index = obj.GetComponent<Interactable>().Get�omponentIndex();
            if (!dictionary.ContainsKey(index))
            {
                dictionary.Add(index, obj);
            }
            else
            {
                Debug.LogWarning($"Duplicated index {index} for tag {tag}. Object {obj.name} will be ignored.");
            }
        }
    }
}
