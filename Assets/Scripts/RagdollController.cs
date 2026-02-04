using UnityEngine;
using UnityEngine.InputSystem;

public class RagdollController : MonoBehaviour
{
    [SerializeField] private Animator animator;
    private bool isRagdoll = false;

    void Awake()
    {
        ActivateRagdoll(isRagdoll);
    }

    // Update is called once per frame
    void Update()
    {
        if (Keyboard.current != null && Keyboard.current.spaceKey.wasPressedThisFrame)
        {
            animator.enabled = !animator.enabled;
            isRagdoll = !isRagdoll;
            ActivateRagdoll(isRagdoll);
        }
    }

    private void ActivateRagdoll(bool active)
    {
        foreach (Rigidbody rb in GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !active;
        }
    }
}