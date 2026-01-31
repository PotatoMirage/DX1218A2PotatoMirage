using UnityEngine;
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
        if (Input.GetKeyDown(KeyCode.Space))
        {
            animator.enabled = !animator.enabled;
            isRagdoll = !isRagdoll;
            ActivateRagdoll(isRagdoll);
        }
    }
    private void ActivateRagdoll(bool active)
    {
        foreach (Rigidbody rb in
        GetComponentsInChildren<Rigidbody>())
        {
            rb.isKinematic = !active;
        }
    }
}