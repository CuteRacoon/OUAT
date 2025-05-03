using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ActionController : MonoBehaviour
{
    private CameraBehaviour cameraBehaviour;
    private DialogueController dialogueController;
    private InteractionController interactionController;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        cameraBehaviour = FindAnyObjectByType<CameraBehaviour>();
        dialogueController = FindAnyObjectByType<DialogueController>();
        interactionController = FindAnyObjectByType<InteractionController>();

        cameraBehaviour.currentCameraIndex = 3;
        StartCoroutine(startDialogue());
    }
    private IEnumerator startDialogue()
    {
        yield return new WaitForSeconds(1f);

        dialogueController.PlayPartOfPlot("beginning");
    }
}
