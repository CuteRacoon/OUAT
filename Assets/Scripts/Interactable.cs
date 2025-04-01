using UnityEngine;
using System.Collections;
using UnityEditor.Search;

public class Interactable : MonoBehaviour
{
    public float pickupHeight = 0.5f;   // Высота, на которую поднимаем объект над столом.
    public LayerMask tableLayer;       // Слой, к которому относится ваш стол. Это важно для определения высоты
    public GameObject berries;
    public int index = -1;

    private Rigidbody rb;
    private Camera mainCamera;

    private bool isHoldingObject = false;
    private bool isMouseOver = false;
    private bool isReturning = false;
    private float initialYOffset;
    private float returnSpeed = 4f;

    private Vector3 objectWorldPosition;
    private Vector3 initialPosition;
    private Vector3 initialScale;
    private Vector3[] initialChildPositions;

    private Outline outlineComponent;  // Ссылка на компонент Outline
    private OutlineSettings outlineSettings;
    private Animation anime;
    private GameLogic gameLogic;


    void Start()
    {
        // Получаем компонент Outline
        gameLogic = FindAnyObjectByType<GameLogic>();
        outlineComponent = GetComponent<Outline>();
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Объект должен иметь компонент Rigidbody!");
            EnableOutline(false); // Отключаем скрипт, если нет Rigidbody.
            return;
        }

        rb.useGravity = true; // Убедитесь, что гравитация включена изначально.
        rb.isKinematic = false; // Убедитесь, что объект не kinematic изначально

        mainCamera = Camera.main; // Получаем ссылку на главную камеру.
        if (mainCamera == null)
        {
            Debug.LogError("Главная камера не найдена!");
            EnableOutline(false); // Отключаем скрипт, если нет камеры.
            return;
        }

        if (outlineComponent == null)
        {
            Debug.LogWarning("Компонент 'Outline' не найден на объекте");
        }
        else
        {
            EnableOutline(false);
        }
        outlineSettings = FindAnyObjectByType<OutlineSettings>();
        initialPosition = transform.position;
        initialScale = transform.localScale;

        // Получаем компонент Animation
        anime = GetComponent<Animation>();
        if (anime == null && (gameObject.CompareTag("berries") || gameObject.CompareTag("roots")))
        //Добавить проверку на остальные теги, чтобы у каждой миски была либо анимация, либо тег
        {
            Debug.LogWarning("Нет компонента Animation на объекте с тегом " + tag);
        }
        if (this.index == 0 && (gameObject.CompareTag("berries") || gameObject.CompareTag("roots") || gameObject.CompareTag("herbs"))) Debug.Log("Не назначен индекс объекта");
        if (berries != null)
        {
            Transform[] childTransforms = berries.GetComponentsInChildren<Transform>();
            initialChildPositions = new Vector3[childTransforms.Length - 1]; // Исключаем сам berries

            int i = 0;
            foreach (Transform child in childTransforms)
            {
                if (child != berries.transform) // Пропускаем сам объект berries
                {
                    initialChildPositions[i] = child.position;
                    i++;
                }
            }
        }
    }

    void Update()
    {
        // Проверка на наведение мыши (независимо от нажатия кнопки)
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
        {
            if (!isMouseOver) // Если мышь только что навелась
            {
                isMouseOver = true;
                EnableOutline(true);
            }
        }
        else
        {
            if (isMouseOver) // Если мышь только что ушла с объекта
            {
                isMouseOver = false;
                EnableOutline(false);
            }
        }

        if (Input.GetMouseButtonDown(0) && isMouseOver)
        {
            PickupObject();
        }

        if (Input.GetMouseButtonUp(0) && isHoldingObject)
        {
            StartCoroutine(HandleObjectRelease());
        }

        if (isHoldingObject)
        {
            MoveObject();
        }

        if (isReturning && gameObject.activeSelf)
        {
            // Плавный возврат к начальной позиции
            transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * returnSpeed);

            // Проверяем, достаточно ли близко мы к начальной позиции
            if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
            {
                transform.position = initialPosition; // Фиксируем позицию
                isReturning = false;  // Прекращаем возврат
                rb.linearVelocity = Vector3.zero; // Останавливаем движение
                rb.angularVelocity = Vector3.zero; // Останавливаем вращение

                RestoreChildPositions();
            }
        }
    }
    IEnumerator HandleObjectRelease()
    {
        // Если у объекта тег "berries", проигрываем анимацию и ждем ее завершения
        if (gameObject.CompareTag("berries") && anime != null)
        {
            anime.Play("BerriesAnimation");

            Rigidbody[] allRigidbodies = berries.GetComponentsInChildren<Rigidbody>();
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
            if (berries != null && this.index > 0)
            {
                berries.SetActive(false);
                gameLogic.BerriesOn(this.index);
            }

        }

        if (gameObject.CompareTag("roots") && anime != null)
        {
            anime.Play("RootsAnimation");
            yield return new WaitForSeconds(anime["RootsAnimation"].length);
            gameLogic.RootsOn(this.index);

        }
        DropObject();
        if (gameObject.CompareTag("roots"))
        {
            gameObject.SetActive(false);
            gameObject.transform.localScale = initialScale;
            gameObject.transform.position = initialPosition;
        }
            

    }

    void PickupObject()
    {
        isHoldingObject = true;
        rb.useGravity = false; // Отключаем гравитацию, чтобы объект не падал.
        rb.linearVelocity = Vector3.zero; // Обнуляем скорость, чтобы избежать нежелательного движения.
        rb.angularVelocity = Vector3.zero; // Обнуляем угловую скорость
        rb.isKinematic = true; //Делаем объект kinematic на время перемещения

        // Получаем позицию объекта относительно стола.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tableLayer)) // Используем LayerMask
        {
            initialYOffset = transform.position.y - hit.point.y;
            objectWorldPosition = hit.point;
        }
        else
        {
            // Если не попали в стол, поднимаем на фиксированную высоту относительно текущей позиции.
            objectWorldPosition = transform.position;
            initialYOffset = 0;
            objectWorldPosition.y -= pickupHeight;
        }
    }

    void DropObject()
    {
        isHoldingObject = false;
        rb.useGravity = true; // Включаем гравитацию, чтобы объект упал.
        rb.isKinematic = false; //Объект перестает быть kinematic

        isReturning = true;
    }

    void MoveObject()
    {
        // Преобразуем координаты мыши в мировые координаты на фиксированной высоте.
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Vector3.Distance(transform.position, mainCamera.transform.position); // Важно: задаем глубину!
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        //Вычисляем необходимую высоту
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tableLayer))
        {
            objectWorldPosition = hit.point;
        }
        Vector3 targetPosition = new Vector3(objectWorldPosition.x, objectWorldPosition.y + pickupHeight + initialYOffset, objectWorldPosition.z);

        transform.position = targetPosition;
    }

    //Вспомогательная функция для включения/выключения компонента Outline
    private void EnableOutline(bool enable)
    {
        if (outlineComponent != null)
        {
            if (outlineSettings != null) // Проверяем, что OutlineColorSettings найден.
            {
                outlineComponent.OutlineColor = outlineSettings.basicOutlineColor;
                outlineComponent.OutlineWidth = outlineSettings.outlineWidth;
            }

            outlineComponent.enabled = enable;
        }
    }

    // Восстановление начальных позиций потомков
    private void RestoreChildPositions()
    {
        if (berries != null && initialChildPositions != null)
        {
            Transform[] childTransforms = berries.GetComponentsInChildren<Transform>();
            int i = 0;
            foreach (Transform child in childTransforms)
            {
                if (child != berries.transform)
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
