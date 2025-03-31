using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
    public float pickupHeight = 0.5f;   // Высота, на которую поднимаем объект над столом.
    public LayerMask tableLayer;       // Слой, к которому относится ваш стол. Это важно для определения высоты
    public GameObject berries;

    private Rigidbody rb;
    private Camera mainCamera;

    private bool isHoldingObject = false;
    private bool isMouseOver = false;
    private bool isReturning = false;
    private float initialYOffset;
    private float returnSpeed = 4f;

    private Vector3 objectWorldPosition;
    private Vector3 initialPosition;

    private Outline outlineComponent;  // Ссылка на компонент Outline
    private OutlineSettings outlineSettings;
    private Animation animation;
    
    

    void Start()
    {
        // Получаем компонент Outline
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
            Debug.LogWarning("Компонент 'Outline' не найден на объекте.  Убедитесь, что он добавлен.");
        }
        else
        {
            EnableOutline(false);
        }
        outlineSettings = FindAnyObjectByType<OutlineSettings>();
        initialPosition = transform.position;

        // Получаем компонент Animation
        animation = GetComponent<Animation>();
        if (animation == null || !gameObject.CompareTag("berries")) //Если у объекта тег Berries, то ищем аниматор, иначе - не нужно.
        {
            Debug.LogWarning("Нет компонента Animation или не установлен тег");
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

        if (isReturning)
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
            }
        }
    }
    IEnumerator HandleObjectRelease()
    {
        // Если у объекта тег "berries", проигрываем анимацию и ждем ее завершения
        if (gameObject.CompareTag("berries"))
        {
            animation.Play("BerriesAnimation");

            Rigidbody berriesrb;
            if (berries != null)
            {
                berriesrb = berries.GetComponent<Rigidbody>();
                berriesrb.useGravity = true; // Включаем гравитацию, чтобы объект упал.
                berriesrb.isKinematic = false;
            }
            else Debug.Log("Не назначен Berries или проблемы с RigidBody");

            // Ждем пока анимация не закончит проигрываться.
            yield return new WaitForSeconds(animation["BerriesAnimation"].length);

            if (berries != null)
            {
                berriesrb = berries.GetComponent<Rigidbody>();
                berriesrb.useGravity = false; // Включаем гравитацию, чтобы объект упал.
                berriesrb.isKinematic = true;
                berries.SetActive(false);
            }
        }

        DropObject();
        isReturning = true;
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
                outlineComponent.OutlineColor = outlineSettings.outlineColor;
            }

            outlineComponent.enabled = enable;
        }
    }
}
