using UnityEngine;
using System.Collections;

public class Herbs : Interactable
{
    private Animation anime;
    private GameLogic gameLogic;
    private bool needToReturn;


    protected override void Start()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Herbs");
        }

        gameLogic = FindAnyObjectByType<GameLogic>();

        if (tag != "herbs") Debug.LogWarning("Тэг объекта не Herbs");

    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && gameLogic.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play("HerbsAnimation");
            yield return new WaitForSeconds(anime["HerbsAnimation"].length);
            needToReturn = true;
            gameLogic.HerbsOn(this.index);

        }
        DropObject();
        if (needToReturn)
        {
            gameObject.SetActive(false);
            gameObject.transform.localScale = initialScale;
            gameObject.transform.position = initialPosition;
            needToReturn = false;
        }
    }
}

