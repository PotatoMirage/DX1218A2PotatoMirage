using UnityEngine;

public class FootIK : MonoBehaviour
{
    [SerializeField] private Animator animator;
    [SerializeField] private LayerMask groundLayer; // Set this to "Default" or "Ground"

    [Range(0, 1f)][SerializeField] private float distanceToGround = 0.5f; // Check distance down
    [SerializeField] private float pelvisOffset = 0f;

    private void OnAnimatorIK(int layerIndex)
    {
        if (animator == null) return;

        // Calculate and Set Left Foot Position/Rotation
        AdjustFoot(AvatarIKGoal.LeftFoot);

        // Calculate and Set Right Foot Position/Rotation
        AdjustFoot(AvatarIKGoal.RightFoot);
    }

    private void AdjustFoot(AvatarIKGoal foot)
    {
        // 1. Get the position the animation wants the foot to be at
        Vector3 footPosition = animator.GetIKPosition(foot);

        // 2. Raycast straight down from that position to find the actual stairs/slope
        RaycastHit hit;
        // We start the ray slightly above the foot position to ensure we hit the step
        if (Physics.Raycast(footPosition + Vector3.up * distanceToGround, Vector3.down, out hit, distanceToGround * 2f, groundLayer))
        {
            // 3. Set the new IK position to the hit point + a small offset so the foot isn't inside the mesh
            Vector3 targetPos = hit.point;
            targetPos.y += pelvisOffset; // Adjust this if feet clip into ground

            animator.SetIKPositionWeight(foot, 1f);
            animator.SetIKPosition(foot, targetPos);

            // 4. Set Rotation to match the slope normal
            Quaternion footRotation = Quaternion.LookRotation(transform.forward, hit.normal);
            animator.SetIKRotationWeight(foot, 1f);
            animator.SetIKRotation(foot, footRotation);
        }
        else
        {
            // If no ground is found, let the animation play normally
            animator.SetIKPositionWeight(foot, 0f);
            animator.SetIKRotationWeight(foot, 0f);
        }
    }
}