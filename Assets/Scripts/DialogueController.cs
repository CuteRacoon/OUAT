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
                phrase += "Похоже на правду, но как будто где-то ошибку допустила...";
                phrase += "\nНе страшно, попробую ещё раз";
                break;
            case 2:
                phrase += "Ой-ой-ой, батюшки, что-то оно вообще не так себя ведёт";
                phrase += "\nЭх... Надо повнимательнее в книгу посмотреть";
                break;
            case 3:
                phrase += "Выглядит отлично, да и пахнет тоже. Похоже, то что надо!";
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
