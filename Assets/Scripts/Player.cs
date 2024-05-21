using System;
using System.Collections;
using UnityEngine;
using Zenject;

[RequireComponent(typeof(PlayerAnimationManager))]
public class Player : MonoBehaviour
{
    [Inject] StackManager stackManager;

    [SerializeField] CharacterController _controller;
    [SerializeField] PlayerAnimationManager _animationManager;
    [SerializeField] float _moveSpeed;
    [SerializeField] float _groundRayDistance = 0.4f;
    [SerializeField] float _fallDurationThreshold = 2f;
    [SerializeField] Transform _followLookTransformForCam;
    [SerializeField] LayerMask _groundMask;

    Vector3 _nextPath;
    const float _gravityScale = 9.81f;
    bool _isGrounded;
    float _fallTimer;
    bool _isRunning;
    bool _isDancing;
    bool _isGameOver;
    float _lockedXPos;
    float _lockedYPos;
    float _tickUpdateFrequency = 0.5f;
    bool _isRoundFinished;
    Vector3 _finishGroundPos;

    public event Action OnPlayerFellDown;
    public event Action OnRoundFinished;

    public Transform FollowLookTransformForCam => _followLookTransformForCam;

    private void OnValidate()
    {
        UpdateFollowLookTransformPosition();
    }

    void OnEnable()
    {
        stackManager.OnFirstStackPlaced += OnFirstStackPlacedEvent;
        stackManager.OnRoundFinish += OnRoundFinishEvent;
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
        _isDancing = false;
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
        _controller.Move(_gravityScale * Vector3.down * Time.deltaTime);
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

    void MoveToNextPath()
    {
        if (!_isRunning || _isDancing) return;

        var dir = _nextPath - transform.position;

        if (dir.magnitude > 0.1f)
        {
            _controller.Move(_moveSpeed * dir.normalized * Time.deltaTime);
        }
        else
        {
            UpdateNextPath();
        }

        _followLookTransformForCam.position = new Vector3(_lockedXPos, _lockedYPos, _controller.transform.position.z);
    }

    void UpdateNextPath()
    {
        var path = stackManager.GetNextPlayerPathLocation();
        path.y = transform.position.y;

        if (path.z < transform.position.z) //path node is behind the player
        {
            if (_isRoundFinished)
            {
                _isDancing = true;

                _animationManager.Dance();
                OnRoundFinished?.Invoke();
                return;
            }
            _nextPath = transform.position + Vector3.forward;
        }
        else
        {
            _nextPath = path;
        }
    }


    void UpdateFollowLookTransformPosition()
    {
        if (_followLookTransformForCam != null)
        {
            _followLookTransformForCam.position = new Vector3(_lockedXPos, _lockedYPos, _controller.transform.position.z);
        }
    }

    void OnFirstStackPlacedEvent()
    {
        UpdateNextPath();
        StartSprint();
    }

    void OnRoundFinishEvent(Vector3 finishGroundPos)
    {
        _finishGroundPos = finishGroundPos;
        _isRoundFinished = true;
    }

    public void GameOverCall()
    {
        _isGameOver = true;
    }

    public void NextRoundReady()
    {
        _isRoundFinished = false;
        _animationManager.Run();
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
