using Unity.VisualScripting;
using UnityEngine;

public class GameLogic : MonoBehaviour
{
    public GameObject[] objectsToOutline;

    private static GameObject currentObject;
    private AnimationsControl animationsControl;
    private int index = -1;

    private void Start()
    {
        animationsControl = FindAnyObjectByType<AnimationsControl>();
        currentObject = null;
    }
    public void SetCurrentObject(GameObject obj)
    {
        currentObject = obj;
        Debug.Log("CurrentObject is" + currentObject.name);
        index = animationsControl.GetCorrectBowlIndex(currentObject);
        OutlineObject(index, true);
    }

    public void NullCurrentObject()
    {
        currentObject = null;
        Debug.Log("CurrentObject is null");
        OutlineObject(index, false);
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
}
