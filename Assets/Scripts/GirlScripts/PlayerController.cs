using UnityEngine;

public class PlayerController : MonoBehaviour
{
    private Animator animator;
    private bool canMove = true;

    private void Awake()
    {
        animator = GetComponent<Animator>();
    }

    public void SetMovement(bool state)
    {
        canMove = state;
        animator.applyRootMotion = state; // Инвертируем состояние
        animator.enabled = state;
    }
}