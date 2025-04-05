using System.Collections;
using UnityEngine;
using UnityEngine.UI;

public class DialogueController : MonoBehaviour
{
    public Text text;
    public GameObject textThing;
    private GameLogic gameLogic;
    // Start is called once before the first execution of Update after the MonoBehaviour is created
    void Start()
    {
        text.text = null;
        textThing.SetActive(false);
        gameLogic = FindAnyObjectByType<GameLogic>();
    }

    // Update is called once per frame
    void Update()
    {
        
    }
    public IEnumerator EndGame(int dialogueIndex)
    {
        string phrase = "";
        switch (dialogueIndex)
        {
            case 1:
                phrase += "������ �� ������, �� ��� ����� ���-�� ������ ���������...";
                phrase += "\n�� �������, �������� ��� ���";
                break;
            case 2:
                phrase += "��-��-��, �������, ���-�� ��� ������ �� ��� ���� ����";
                phrase += "\n��... ���� �������������� � ����� ����������";
                break;
            case 3:
                phrase += "�������� �������, �� � ������ ����. ������, �� ��� ����!";
                break;
        }
        text.text = phrase;
        textThing.SetActive(true);

        yield return new WaitForSeconds(3f);
        textThing.SetActive(false);
        if (dialogueIndex != 3)
        {
            gameLogic.ResetGame();
        }
    }
}
