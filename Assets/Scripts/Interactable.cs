using UnityEngine;

public class Interactable : MonoBehaviour
{
    public float pickupHeight = 0.5f;   // ������, �� ������� ��������� ������ ��� ������.
    public LayerMask tableLayer;       // ����, � �������� ��������� ��� ����. ��� ����� ��� ����������� ������.

    private Rigidbody rb;
    private bool isHoldingObject = false;
    private Vector3 objectWorldPosition;
    private Camera mainCamera;
    private float initialYOffset;
    private bool isMouseOver = false;
    private MonoBehaviour interactableScript;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("������ ������ ����� ��������� Rigidbody!");
            enabled = false; // ��������� ������, ���� ��� Rigidbody.
            return;
        }

        rb.useGravity = true; // ���������, ��� ���������� �������� ����������.
        rb.isKinematic = false; // ���������, ��� ������ �� kinematic ����������

        mainCamera = Camera.main; // �������� ������ �� ������� ������.
        if (mainCamera == null)
        {
            Debug.LogError("������� ������ �� �������!");
            enabled = false; // ��������� ������, ���� ��� ������.
            return;
        }
        Component scriptComponent = GetComponent<Outline>();
        if (scriptComponent != null)
        {
            interactableScript = (MonoBehaviour)scriptComponent;

            //Отключаем скрипт interactable в начале
            if (interactableScript != null && interactableScript.enabled)
                interactableScript.enabled = false;
        }
        else
        {
            Debug.LogWarning("Скрипт \"Outline\" не найден на объекте.");
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
                EnableInteractableScript(true);
            }
        }
        else
        {
            if (isMouseOver) // Если мышь только что ушла с объекта
            {
                isMouseOver = false;
                EnableInteractableScript(false);
            }
        }

        if (Input.GetMouseButtonDown(0) && isMouseOver)
        {
            PickupObject();
        }

        if (Input.GetMouseButtonUp(0) && isHoldingObject)
        {
            DropObject();
        }

        if (isHoldingObject)
        {
            MoveObject();
        }
    }

    void PickupObject()
    {
        isHoldingObject = true;
        rb.useGravity = false; // ��������� ����������, ����� ������ �� �����.
        rb.linearVelocity = Vector3.zero; // �������� ��������, ����� �������� �������������� ��������.
        rb.angularVelocity = Vector3.zero; // �������� ������� ��������
        rb.isKinematic = true; //������ ������ kinematic �� ����� �����������

        // �������� ������� ������� ������������ �����.
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tableLayer)) // ���������� LayerMask
        {
            initialYOffset = transform.position.y - hit.point.y;
            objectWorldPosition = hit.point;
        }
        else
        {
            // ���� �� ������ � ����, ��������� �� ������������� ������ ������������ ������� �������.
            objectWorldPosition = transform.position;
            initialYOffset = 0;
            objectWorldPosition.y -= pickupHeight;
        }
    }

    void DropObject()
    {
        isHoldingObject = false;
        rb.useGravity = true; // �������� ����������, ����� ������ ����.
        rb.isKinematic = false; //������ ��������� ���� kinematic
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
    void EnableInteractableScript(bool enable)
    {
        if (interactableScript != null)
        {
            interactableScript.enabled = enable;
        }
    }
}
