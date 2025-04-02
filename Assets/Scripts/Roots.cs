using UnityEngine;
using System.Collections;

public class Roots : Interactable
{
    private Animation anime;
    private GameLogic gameLogic;

    protected override void Start()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("��� ���������� Animation �� ������� Roots");
        }

        gameLogic = FindAnyObjectByType<GameLogic>();

        if (tag != "roots") Debug.LogWarning("��� ������� �� Roots");

    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null)
        {
            anime.Play("RootsAnimation");
            yield return new WaitForSeconds(anime["RootsAnimation"].length);
            gameLogic.RootsOn(this.index);

        }
        DropObject();
        gameObject.SetActive(false);
        gameObject.transform.localScale = initialScale;
        gameObject.transform.position = initialPosition;

    }
}
