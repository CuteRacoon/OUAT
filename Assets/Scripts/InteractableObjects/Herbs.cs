using UnityEngine;
using System.Collections;

public class Herbs : Interactable
{
    private Animation anime;
    private AnimationsControl animationsControl;
    private bool needToReturn;

    private MeshRenderer meshRenderer;
    private Collider boxCollider;

    private int objectIndicator = 2;


    protected override void Start()
    {
        base.Start();
        meshRenderer = GetComponent<MeshRenderer>();
        boxCollider = GetComponent<Collider>();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("��� ���������� Animation �� ������� Herbs");
        }

        this.animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (tag != "herbs") Debug.LogWarning("��� ������� �� Herbs");

    }
    protected override void PickupObject()
    {
        base.PickupObject();
        if (animationsControl.isFull)
        {
            animationsControl.CleanDust();
        }
    }
    protected override IEnumerator HandleObjectRelease()
    {
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            needToReturn = true;
            anime.Play("HerbsAnimation");

            yield return new WaitForSeconds(anime["HerbsAnimation"].length);
            
            this.animationsControl.ObjectsOn(this.index, this.objectIndicator);

            DropObject();
            meshRenderer.enabled = false;
            boxCollider.enabled = false;

            gameLogic.AccessHerbsAndBerriesInteraction(false);

            float time = this.animationsControl.PlayMortarAnimation();
            yield return new WaitForSeconds(time);
            gameLogic.AccessBowls(3, true);

            //gameLogic.AccessHerbsAndBerriesInteraction(false);

            this.animationsControl.ObjectsDustOn(this.index, this.objectIndicator);
            gameLogic.AddToObjectsList(index, objectIndicator);

        }
        else
        {
            DropObject();
        }
        if (needToReturn)
        {
            base.ReturnToInitialPosition();
            needToReturn = false;
            gameObject.SetActive(false);
        }
        /*if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            gameObject.SetActive(false);
            meshRenderer.enabled = true;
            boxCollider.enabled = true;
        }*/
    }
}