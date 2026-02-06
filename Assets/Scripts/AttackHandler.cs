using System;
using Unity.Cinemachine;
using UnityEngine;
using static UnityEditor.Experimental.GraphView.GraphView;

public class AttackHandler : MonoBehaviour
{
    [SerializeField] private SphereCollider[] detectors;
    [SerializeField] private CinemachineImpulseSource[] source;
    public void EnableCollider(int index)
    {
        SphereCollider detector = detectors[index];
        detector.enabled = true;
        // Only detect collision with any object on Target layer
        LayerMask layer = LayerMask.GetMask("Target");
        Collider[] hitColliders =  Physics.OverlapSphere(detector.transform.position, detector.radius, layer);
        for (int i = 0; i < hitColliders.Length; i++)
        {
            detector.enabled = false;
            // Perform damage on other object, show feedback, etc
            source[index].GenerateImpulse(Camera.main.transform.forward);
        }
    }
    public void DisableCollider()
    {
        foreach(SphereCollider detector in detectors)
        {
            detector.enabled = false;
        }
    }
}