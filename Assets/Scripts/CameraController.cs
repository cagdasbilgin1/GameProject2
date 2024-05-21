using Cinemachine;
using System.Collections;
using UnityEngine;
using Zenject;

public class CameraController : MonoBehaviour
{
    [Inject] Player player;
    [Inject] StackManager stackManager;

    [SerializeField] CinemachineFreeLook _freeLookCam;
    [SerializeField] float _rotationSpeed = 60f;
    [SerializeField] float _stopperRotationSpeed = 130f;

    CinemachineComposer _composer;
    bool _isRotating;
    bool _stopper;
    Vector3 _defaultTrackedOffset;

    void OnEnable()
    {
        player.OnRoundFinished += OnRoundFinishedEvent;
    }

    void OnDisable()
    {
        player.OnRoundFinished -= OnRoundFinishedEvent;
    }

    void Start()
    {
        _composer = _freeLookCam.GetRig(1).GetCinemachineComponent<CinemachineComposer>();
        _defaultTrackedOffset = _composer.m_TrackedObjectOffset;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            if(_isRotating)
            {
                SetToPreviousSettings();
            }
        }

        if (_isRotating)
        {
            _freeLookCam.m_XAxis.Value += _rotationSpeed * Time.deltaTime;
        }

        if (_stopper)
        {
            _freeLookCam.m_XAxis.Value += _stopperRotationSpeed * Time.deltaTime;

            if (_freeLookCam.m_XAxis.Value % 360 < 30)
            {
                _stopper = false;

                StartCoroutine(SmoothTransitionToPreviousSettings());
            }
        }
    }

    void OnRoundFinishedEvent()
    {
        RotateAroundPlayer();
    }

    void RotateAroundPlayer()
    {
        _isRotating = true;

        _freeLookCam.Follow = player.transform;
        _freeLookCam.LookAt = player.transform;
        _composer.m_TrackedObjectOffset = Vector3.zero;
    }

    void SetToPreviousSettings()
    {
        _isRotating = false;
        _stopper = true;
    }

    IEnumerator SmoothTransitionToPreviousSettings()
    {
        var elapsedTime = 0f;
        var transitionDuration = 1f;
        var initialOffset = _composer.m_TrackedObjectOffset;
        var targetOffset = _defaultTrackedOffset;

        while (elapsedTime < transitionDuration)
        {
            float t = elapsedTime / transitionDuration;
            _composer.m_TrackedObjectOffset = Vector3.Lerp(initialOffset, targetOffset, t);
            elapsedTime += Time.deltaTime;
            yield return null;
        }

        _composer.m_TrackedObjectOffset = _defaultTrackedOffset;
        _freeLookCam.Follow = player.FollowLookTransformForCam;
        _freeLookCam.LookAt = player.FollowLookTransformForCam;
        _freeLookCam.m_XAxis.Value = 0;
        
        stackManager.NextRoundReady();
        player.NextRoundReady();
    }
}
