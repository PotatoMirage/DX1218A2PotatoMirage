using NUnit.Framework;
using System.Collections;
using System.Collections.Generic;
using Unity.Cinemachine;
using Unity.VisualScripting;
using Unity.VisualScripting.Antlr3.Runtime.Tree;
using UnityEditor;
using UnityEditor.Experimental.GraphView;
using UnityEditor.Search;
using UnityEngine;
using UnityEngine.InputSystem;
using static Unity.VisualScripting.Member;
using static UnityEngine.UIElements.UxmlAttributeDescription;

public class PlayerController : MonoBehaviour
{
    [SerializeField] private BoxCollider collider;
    [SerializeField] private CinemachineImpulseSource source;
    [SerializeField] private Animator _animator;
    [SerializeField]
    private CharacterController
    _characterController;
    [SerializeField] private PlayerInput _playerInput;
    private InputActionAsset _inputActions;
    [SerializeField] private CinemachineCamera _virtualCamera;
    [SerializeField] private CinemachineCamera _freeLookCamera;
    private List<IEnumerator> _attackQueue = new();

    private bool _isAttack = false;
    private int _attackStep = 0;
    private readonly string[] _attackNames = { "Attack1", "Attack2", "Attack3" };
    // Start is called before the first frame update
    void Start()
    {
        _inputActions = _playerInput.actions;
    }

    // Update is called once per frame
    void Update()
    {
        Vector2 input =
_inputActions["Move"].ReadValue<Vector2>();
        Vector3 moveDirection = new(input.x, 0,
        Mathf.Clamp01(input.y));
        if (moveDirection.magnitude > 0)
        {
            _animator.SetBool("IsWalking", true);
            moveDirection =
Quaternion.AngleAxis(Camera.main.transform.eulerAngles.y,
Vector3.up) * moveDirection;
            Quaternion targetRotation =
Quaternion.LookRotation(moveDirection, Vector3.up);
            transform.rotation =
Quaternion.RotateTowards(transform.rotation, targetRotation,
Time.deltaTime * 100f);
        }
        else if (_inputActions["Attack"].IsPressed())
        {
            if (_attackQueue.Count < 3)
            {
                _attackQueue.Add(PerformAttack());
            }
            if (_attackQueue.Count == 1)
            {
                StartCombo();
            }
        }
        else
        {
            ResetCombo();
            _animator.SetBool("IsWalking", false);
            _animator.SetBool("IsAttack", false);
        }
        if (_inputActions["Previous"].IsPressed())
        {
            _virtualCamera.gameObject.SetActive(true);
            _freeLookCamera.gameObject.SetActive(false);
        }
        else if (_inputActions["Next"].IsPressed())
        {
            _virtualCamera.gameObject.SetActive(false);
            _freeLookCamera.gameObject.SetActive(true);
        }
        LayerMask layer = LayerMask.GetMask("Target");
        if (collider.enabled)
        {
            Collider[] hitColliders =
Physics.OverlapSphere(collider.transform.position,
collider.contactOffset, layer);
            for (int i = 0; i < hitColliders.Length; i++)
            {
                collider.enabled = false;
                // Perform damage on other object, show feedback, etc
                // Step 7 - Generate Impulse
                source.GenerateImpulse(Camera.main.transform.forward);
            }
        }
    }
    private void OnAnimatorMove()
    {
        Vector3 velocity = _animator.deltaPosition;
        _characterController.Move(velocity);
    }
    private void StartCombo()
    {
        _isAttack = true;
        _animator.SetBool("IsAttack", _isAttack);
        StartCoroutine(_attackQueue[0]);
    }
    private IEnumerator PerformAttack()
    {
        _attackStep++;
        _animator.SetInteger("AttackStep", _attackStep);
        while (!IsCurrentAnimationReadyForNextStep(_attackNames[_attackStep - 1]))
        {
            yield return null;
        }
        StartCoroutine(_attackQueue[_attackStep]);
        if (_attackStep >= _attackQueue.Count)
        {
            ResetCombo();
        }
        else
        {
            StartCoroutine(_attackQueue[_attackStep]);
        }
        
    }
    private bool IsCurrentAnimationReadyForNextStep(string name)
    {
        AnimatorStateInfo stateInfo = _animator.GetCurrentAnimatorStateInfo(0);
        return stateInfo.normalizedTime >= 1.5f && stateInfo.IsName(name); // Adjust based on when you want to allow transitions
}
    private void ResetCombo()
    {
        _isAttack = false;
        _attackStep = 0;
        _animator.SetInteger("AttackStep", _attackStep);
        _animator.SetBool("IsAttack", false);
        _attackQueue.Clear();
    }
}
