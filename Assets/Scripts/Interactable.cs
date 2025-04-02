using UnityEngine;
using System.Collections;

public class Interactable : MonoBehaviour
{
    protected float pickupHeight = 0.5f;
    protected LayerMask tableLayer;
    public int index = -1;

    protected Rigidbody rb;
    protected Camera mainCamera;

    protected bool isHoldingObject = false;
    protected bool isMouseOver = false;
    protected bool isReturning = false;
    protected float initialYOffset;
    protected float returnSpeed = 4f;

    protected Vector3 initialPosition;
    protected Vector3 initialScale;
    protected Vector3 objectWorldPosition;

    protected Outline outlineComponent;
    protected OutlineSettings outlineSettings;

    protected virtual void Start()
    {
        tableLayer = LayerMask.GetMask("Table");
        rb = GetComponent<Rigidbody>();
        if (rb == null)
        {
            Debug.LogError("Объект должен иметь компонент Rigidbody!");
            enabled = false;
            return;
        }

        rb.useGravity = true;
        rb.isKinematic = false;

        mainCamera = Camera.main;
        if (mainCamera == null)
        {
            Debug.LogError("Главная камера не найдена!");
            enabled = false;
            return;
        }

        outlineComponent = GetComponent<Outline>();
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

        if (this.index == 0) Debug.Log("Не назначен индекс объекта");
    }

    protected virtual void Update()
    {
        HandleMouseInteraction();

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
            ReturnToInitialPosition();
        }
    }

    protected virtual IEnumerator HandleObjectRelease()
    {
        DropObject();
        yield return null;
    }

    protected virtual void PickupObject()
    {
        isHoldingObject = true;
        rb.useGravity = false;
        if (!rb.isKinematic)
        {
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
        rb.isKinematic = true;

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tableLayer))
        {
            initialYOffset = transform.position.y - hit.point.y;
            objectWorldPosition = hit.point;
        }
        else
        {
            objectWorldPosition = transform.position;
            initialYOffset = 0;
            objectWorldPosition.y -= pickupHeight;
        }
    }

    protected virtual void DropObject()
    {
        isHoldingObject = false;
        rb.useGravity = true;
        rb.isKinematic = false;
        isReturning = true;
    }

    protected virtual void MoveObject()
    {
        Vector3 mousePosition = Input.mousePosition;
        mousePosition.z = Vector3.Distance(transform.position, mainCamera.transform.position);
        Vector3 worldPosition = mainCamera.ScreenToWorldPoint(mousePosition);

        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;
        if (Physics.Raycast(ray, out hit, Mathf.Infinity, tableLayer))
        {
            objectWorldPosition = hit.point;
        }

        Vector3 targetPosition = new Vector3(objectWorldPosition.x, objectWorldPosition.y + pickupHeight + initialYOffset, objectWorldPosition.z);
        transform.position = targetPosition;
    }

    private void HandleMouseInteraction()
    {
        Ray ray = mainCamera.ScreenPointToRay(Input.mousePosition);
        RaycastHit hit;

        if (Physics.Raycast(ray, out hit) && hit.collider.gameObject == this.gameObject)
        {
            if (!isMouseOver)
            {
                isMouseOver = true;
                EnableOutline(true);
            }
        }
        else
        {
            if (isMouseOver)
            {
                isMouseOver = false;
                EnableOutline(false);
            }
        }
    }

    protected virtual void ReturnToInitialPosition()
    {
        transform.position = Vector3.Lerp(transform.position, initialPosition, Time.deltaTime * returnSpeed);

        if (Vector3.Distance(transform.position, initialPosition) < 0.01f)
        {
            transform.position = initialPosition;
            isReturning = false;
            rb.linearVelocity = Vector3.zero;
            rb.angularVelocity = Vector3.zero;
        }
    }

    protected void EnableOutline(bool enable)
    {
        if (outlineComponent != null)
        {
            if (outlineSettings != null)
            {
                outlineComponent.OutlineColor = outlineSettings.basicOutlineColor;
                outlineComponent.OutlineWidth = outlineSettings.outlineWidth;
            }

            outlineComponent.enabled = enable;
        }
    }
}
