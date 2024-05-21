using System;
using System.Collections;
using UnityEngine;
using Zenject;

public class Player : MonoBehaviour
{
    [Inject] StackManager stackManager;

    [SerializeField] CharacterController controller;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _groundRayDistance = 0.4f;
    [SerializeField] float _fallDurationThreshold = 2f;
    [SerializeField] Transform _followLookTransformForCam;
    [SerializeField] LayerMask _groundMask;

    Vector3 _nextPath;
    bool _isGrounded;
    float _fallTimer;
    bool _isRunning;
    bool _isGameOver;
    float _lockedXPos;
    float _lockedYPos;
    float _tickUpdateFrequency = 0.5f;

    public event Action OnPlayerFellDown;

    private void OnValidate()
    {
        UpdateFollowLookTransformPosition();
    }

    void OnEnable()
    {
        stackManager.OnFirstStackPlaced += OnFirstStackPlacedEvent;
    }

    void OnDisable()
    {
        stackManager.OnFirstStackPlaced -= OnFirstStackPlacedEvent;
    }

    void Start()
    {
        StartCoroutine(TickUpdateRoutine());
    }

    public void StartSprint()
    {
        _isRunning = true;
        _lockedXPos = transform.position.x;
        _lockedYPos = transform.position.y;
    }

    IEnumerator TickUpdateRoutine()
    {
        while (!_isGameOver)
        {
            TickUpdate();
            yield return new WaitForSeconds(_tickUpdateFrequency);
        }
    }

    void Update()
    {
        UseGravity();
        MoveToNextPath();
    }

    void TickUpdate()
    {
        GroundCheck();
        CheckFall();
    }

    void GroundCheck()
    {
        _isGrounded = Physics.CheckSphere(transform.position, _groundRayDistance, _groundMask);
    }

    void UseGravity()
    {
        controller.Move(9.81f * Vector3.down * Time.deltaTime);
    }

    void CheckFall()
    {
        if (!_isGrounded)
        {
            _fallTimer += _tickUpdateFrequency;

            if (_fallTimer >= _fallDurationThreshold)
            {
                OnPlayerFellDown?.Invoke();
            }
        }
    }

    void UpdateNextPath()
    {
        var path = stackManager.GetNextPlayerPathLocation();
        path.y = transform.position.y;

        if (path.z < transform.position.z) //path node is behind the player
        {
            _nextPath = transform.position + Vector3.forward;
        }
        else
        {
            _nextPath = path;
        }
    }

    void MoveToNextPath()
    {
        if (!_isRunning) return;

        var dir = _nextPath - transform.position;

        if (dir.magnitude > 0.1f)
        {
            controller.Move(_moveSpeed * dir.normalized * Time.deltaTime);
        }
        else
        {
            UpdateNextPath();
        }

        _followLookTransformForCam.position = new Vector3(_lockedXPos, _lockedYPos, controller.transform.position.z);
    }

    void UpdateFollowLookTransformPosition()
    {
        if (_followLookTransformForCam != null)
        {
            _followLookTransformForCam.position = new Vector3(_lockedXPos, _lockedYPos, controller.transform.position.z);
        }
    }

    void OnFirstStackPlacedEvent()
    {
        UpdateNextPath();
        StartSprint();
    }

    public void GameStartCall()
    {

    }

    public void GameOverCall()
    {
        _isGameOver = true;
    }

    void OnDrawGizmos()
    {
        if (_followLookTransformForCam != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(_followLookTransformForCam.position, 0.2f);
        }

        Gizmos.color = Color.green;
        Gizmos.DrawSphere(_nextPath, 0.1f);
    }
}
