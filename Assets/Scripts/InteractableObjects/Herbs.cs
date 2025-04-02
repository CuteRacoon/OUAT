using UnityEngine;
using System.Collections;

public class Herbs : Interactable
{
    private Animation anime;
    private AnimationsControl animationsControl;
    private bool needToReturn;


    protected override void Start()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Herbs");
        }

        animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (tag != "herbs") Debug.LogWarning("Тэг объекта не Herbs");

    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            anime.Play("HerbsAnimation");
            yield return new WaitForSeconds(anime["HerbsAnimation"].length);
            needToReturn = true;
            animationsControl.HerbsOn(this.index);
            gameObject.SetActive(false);
            float time = animationsControl.PlayMortarAnimation();
            yield return new WaitForSeconds(time);
        }
        DropObject();
        if (needToReturn)
        {
            gameObject.transform.localScale = initialScale;
            gameObject.transform.position = initialPosition;
            needToReturn = false;
        }
    }
}

