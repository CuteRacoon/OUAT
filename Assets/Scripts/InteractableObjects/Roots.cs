using UnityEngine;
using System.Collections;

public class Roots : Interactable
{
    private Animation anime;
    private AnimationsControl animationsControl;
    private bool needToDelete = false;

    private int objectIndicator = 0;

    protected void Awake()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Roots");
        }

        animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (tag != "roots") Debug.LogWarning("Тэг объекта не Roots");

    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play("RootsAnimation");
            yield return new WaitForSeconds(anime["RootsAnimation"].length);
            animationsControl.ObjectsOn(this.index, objectIndicator);
            needToDelete = true;
            gameLogic.AddToObjectsList(index, objectIndicator);
            if (index == 1)
            {
                gameLogic.Roots[2].GetComponent<Roots>().enabled = false;
            }
            else gameLogic.Roots[1].GetComponent<Roots>().enabled = false;
        }
        else
        {
            isReturning = true;
        }
        DropObject();
        if (needToDelete)
        {
            gameObject.SetActive(false);
            base.ReturnToInitialPosition();
            needToDelete = false;
        }
    }
}
