using UnityEngine;
using System.Collections;

public class Roots : Interactable
{
    private Animation anime;
    private GameLogic gameLogic;
    private bool needToDelete = false;

    protected override void Start()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Roots");
        }

        gameLogic = FindAnyObjectByType<GameLogic>();

        if (tag != "roots") Debug.LogWarning("Тэг объекта не Roots");

    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && gameLogic.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play("RootsAnimation");
            yield return new WaitForSeconds(anime["RootsAnimation"].length);
            gameLogic.RootsOn(this.index);
            needToDelete = true;
        }
        else
        {
            isReturning = true;
        }
        DropObject();
        if (needToDelete)
        {
            gameObject.SetActive(false);
            gameObject.transform.localScale = initialScale;
            gameObject.transform.position = initialPosition;
            needToDelete = false;
        }
    }
}
