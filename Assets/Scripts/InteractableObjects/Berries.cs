using UnityEngine;
using System.Collections;

public class Berries : Interactable
{
    public GameObject berriesObject; // Refactor berries -> berriesObject
    private Vector3[] initialChildPositions;
    private Animation anime;
    private GameLogic gameLogic;
    private bool needToReturn = false;

    protected override void Start()
    {
        base.Start();

        anime = GetComponent<Animation>();
        if (anime == null)
        {
            Debug.LogWarning("Нет компонента Animation на объекте Berries");
        }

        gameLogic = FindAnyObjectByType<GameLogic>();

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
    protected override IEnumerator HandleObjectRelease()
    {
        // Если у объекта тег "berries", проигрываем анимацию и ждем ее завершения
        if (anime != null && gameLogic.IsNearCorrectBowl(this.gameObject))
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
                gameLogic.BerriesOn(this.index);
            }
        }
        else
        {
            isReturning = true;
        }
        DropObject();

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
