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
            Debug.LogWarning("��� ���������� Animation �� ������� Roots");
        }

        animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (tag != "roots") Debug.LogWarning("��� ������� �� Roots");

    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play("RootsAnimation");
            yield return new WaitForSeconds(anime["RootsAnimation"].length);
            animationsControl.ObjectsOn(this.index, objectIndicator);
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
            base.ReturnToInitialPosition();
            needToDelete = false;
        }
    }
}
