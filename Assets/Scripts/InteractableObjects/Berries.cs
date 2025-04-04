using UnityEngine;
using System.Collections;

public class Berries : Interactable
{
    public GameObject berriesObject; // Refactor berries -> berriesObject
    private Vector3[] initialChildPositions;
    private Animation anime;
    private AnimationsControl animationsControl;
    private bool needToReturn = false;

    private int objectIndicator = 1;

    protected override void Start()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Berries");
        }

        animationsControl = FindAnyObjectByType<AnimationsControl>();

        if (berriesObject != null)
        {
            Transform[] childTransforms = berriesObject.GetComponentsInChildren<Transform>();
            initialChildPositions = new Vector3[childTransforms.Length - 1];

            int i = 0;
            foreach (Transform child in childTransforms)
            {
                if (child != berriesObject.transform)
                {
                    initialChildPositions[i] = child.position;
                    i++;
                }
            }
        }

        if (tag != "berries") Debug.LogWarning("Тэг объекта не Berries");
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
        // Если у объекта тег "berries", проигрываем анимацию и ждем ее завершения
        if (anime != null && animationsControl.IsNearCorrectBowl(this.gameObject))
        {
            needToReturn = true;
            anime.Play("BerriesAnimation");

            Rigidbody[] allRigidbodies = berriesObject.GetComponentsInChildren<Rigidbody>();
            // Включаем гравитацию для всех Rigidbody
            foreach (Rigidbody rb in allRigidbodies)
            {
                rb.useGravity = true;
                rb.isKinematic = false;
            }

            // Ждем пока анимация не закончит проигрываться.
            yield return new WaitForSeconds(anime["BerriesAnimation"].length);

            // Выключаем гравитацию и включаем кинематику для всех Rigidbody
            foreach (Rigidbody rb in allRigidbodies)
            {
                rb.useGravity = false;
                rb.isKinematic = true;
            }
            if (berriesObject != null && this.index > 0)
            {
                berriesObject.SetActive(false);
                if (animationsControl.isFull)
                {
                    animationsControl.CleanDust();
                }
                animationsControl.ObjectsOn(this.index, objectIndicator);
                
            }
            DropObject();
            
            float time = animationsControl.PlayMortarAnimation();
            yield return new WaitForSeconds(time);
            animationsControl.ObjectsDustOn(this.index, objectIndicator);
        }
        else
        {
            //isReturning = true;
            DropObject();
        }
    }

    protected override void ReturnToInitialPosition()
    {
        base.ReturnToInitialPosition(); // Вызываем базовую реализацию

        if (needToReturn) 
        {
            RestoreChildPositions();
            needToReturn = false;
        }
    }

    // Восстановление начальных позиций потомков
    private void RestoreChildPositions()
    {
        if (berriesObject != null && initialChildPositions != null)
        {
            Transform[] childTransforms = berriesObject.GetComponentsInChildren<Transform>();
            int i = 0;
            foreach (Transform child in childTransforms)
            {
                if (child != berriesObject.transform)
                {
                    // Если позиция ребенка не была изменена вручную, восстанавливаем ее
                    if (child.GetComponent<Rigidbody>() != null && child.GetComponent<Rigidbody>().isKinematic)
                    {
                        child.position = initialChildPositions[i];
                        child.rotation = Quaternion.identity;
                    }
                    i++;
                }
            }
        }
    }
}
