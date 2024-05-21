using DG.Tweening;
using System;
using System.Collections.Generic;
using UnityEngine;
using Zenject;
using Random = UnityEngine.Random;

public class StackManager : MonoBehaviour
{
    [Inject] SoundManager soundManager;
    [Inject] CollectableManager collectableSpawner;

    [SerializeField] List<StackGround> _stackPool;
    [SerializeField] List<Material> _materials;
    [SerializeField] StackGround _prevStack;
    [SerializeField] float _movementXOffset;
    [SerializeField] float _moveDuration;
    [SerializeField] float _defaultZScale;
    [SerializeField] float _alignmentTolerance = 0.15f;
    [SerializeField] float _minStackSize = 0.1f;
    [SerializeField] float _totalStackForReachToFinish;
    [SerializeField] GameObject _finishLine;
    [SerializeField] Material _startingGroundMaterial;

    StackGround _currentStack;
    StackGround _finishGround;
    List<Vector3> _playerPath = new();
    Vector3 _startingGroundScale;
    Vector3 _startingGroundPosition;
    float _zPosition;
    float _prevLeftXBound;
    float _prevRightXBound;
    float _finishLineZScale;
    int _perfectAlignmentCount;
    int _usedStackCount;
    bool _isGameStart;
    bool _isGameOver;
    bool _isFinishGroundReady;
    bool _isRoundFinish;


    public event Action OnFirstStackPlaced;
    public event Action<Vector3> OnRoundFinish;

    bool _veryFastStackGeneration => _playerPath.Count > 3;

    void Start()
    {
        MoveNextStack();
        ArrangeFinishLinePosition();
    }

    void ArrangeFinishLinePosition()
    {
        _finishLineZScale = _finishLine.GetComponent<MeshFilter>().mesh.bounds.size.z;
        var length = _prevStack.transform.position.z + _prevStack.transform.localScale.z / 2 + _defaultZScale * _totalStackForReachToFinish + _finishLineZScale / 2;
        _finishLine.transform.position = new Vector3(0, 0, length);
    }

    void Update()
    {
        if (!_isGameStart || _veryFastStackGeneration) return;

        if (Input.GetMouseButtonDown(0))
        {
            StackProcess();
        }
    }

    void MoveNextStack()
    {
        if (_isGameOver || _isRoundFinish) return;

        if (_totalStackForReachToFinish == _usedStackCount) return;

        _currentStack = GetNextStackFromPool();

        ArrangeCurrentStackTransformBeforeMove();
        StartStackMovement(_currentStack, _moveDuration);
    }

    void StackProcess()
    {
        _currentStack.transform.DOKill();

        CutStack();
        MoveNextStack();
    }

    void CutStack()
    {
        if (_isGameOver || _isRoundFinish) return;

        if (_usedStackCount == 0) //first stack
        {
            _prevLeftXBound = GetLeftBoundPos(_prevStack.transform);
            _prevRightXBound = GetRightBoundPos(_prevStack.transform);
        }

        var leftXBound = GetLeftBoundPos(_currentStack.transform);
        var rightXBound = GetRightBoundPos(_currentStack.transform);

        if (_prevLeftXBound - leftXBound > _alignmentTolerance)
        {
            var excessSize = _prevLeftXBound - leftXBound;

            if (EvaluateGameOver(excessSize)) return;
            ArrangeCurrentStackTransformAfterCut(excessSize, CutDirection.Left);
            CreateExcessPartOfStack(leftXBound, rightXBound, excessSize, CutDirection.Left);
            soundManager.PlayCutStackSoundRandomly();
            _perfectAlignmentCount = 0;
        }
        else if (rightXBound - _prevRightXBound > _alignmentTolerance)
        {
            var excessSize = rightXBound - _prevRightXBound;

            if (EvaluateGameOver(excessSize)) return;
            ArrangeCurrentStackTransformAfterCut(excessSize, CutDirection.Right);
            CreateExcessPartOfStack(leftXBound, rightXBound, excessSize, CutDirection.Right);
            soundManager.PlayCutStackSoundRandomly();
            _perfectAlignmentCount = 0;
        }
        else
        {
            _currentStack.transform.position = new Vector3(_prevStack.transform.position.x, _currentStack.transform.position.y, _currentStack.transform.position.z);

            _perfectAlignmentCount++;
            soundManager.PlayPerfectAlignmentSound(_perfectAlignmentCount);
            AddCurrentStackLocationToPath();
        }

        if (_usedStackCount == 0)
        {
            _startingGroundScale = _prevStack.transform.localScale;
            _startingGroundPosition = _prevStack.transform.localPosition;

            OnFirstStackPlaced?.Invoke();
        }

        _usedStackCount++;

        if (_totalStackForReachToFinish == _usedStackCount)
        {
            var zOffset = _currentStack.transform.position.z + _currentStack.transform.localScale.z / 2;
            var finishLinePathLoc = _currentStack.transform.position;
            finishLinePathLoc.z = zOffset;
            _playerPath.Add(finishLinePathLoc);
            var finishGroundPathLoc = _startingGroundPosition;
            _playerPath.Add(finishGroundPathLoc);

            _isRoundFinish = true;

            OnRoundFinish?.Invoke(finishGroundPathLoc);

            _usedStackCount = 0;
        }


        _prevStack = _currentStack;
        ArrangeFinishGround();
    }

    void ArrangeFinishGround()
    {
        if (_isFinishGroundReady) return;

        if (_totalStackForReachToFinish - _usedStackCount < 2)
        {
            _isFinishGroundReady = true;

            _finishGround = GetNextStackFromPool();
            _finishGround.gameObject.SetActive(false);
            _finishGround.transform.localScale = _startingGroundScale;
            var zOffset = _finishLine.transform.position.z + _finishLineZScale / 2 + _startingGroundScale.z / 2;
            _finishGround.transform.position = new Vector3(_startingGroundPosition.x, _startingGroundPosition.y, zOffset);
            _startingGroundPosition = _finishGround.transform.position;
            _finishGround.SetMaterial(_startingGroundMaterial);


            _finishGround.RiseUp();
        }
    }

    void CreateExcessPartOfStack(float leftXBound, float rightXBound, float excessSize, CutDirection cutDirection)
    {
        var excessPartOfStack = GetNextStackFromPool();
        var excessPartScale = _currentStack.transform.localScale;
        excessPartScale.x = excessSize;

        var excessPartPos = _currentStack.transform.position;

        switch (cutDirection)
        {
            case CutDirection.Left:
                leftXBound = _prevLeftXBound;
                _prevRightXBound = rightXBound;
                excessPartPos.x = leftXBound - excessSize / 2;
                break;
            case CutDirection.Right:
                rightXBound = _prevRightXBound;
                _prevLeftXBound = leftXBound;
                excessPartPos.x = rightXBound + excessSize / 2;
                break;
        }

        excessPartOfStack.transform.localScale = excessPartScale;
        excessPartOfStack.transform.position = excessPartPos;
        excessPartOfStack.SetMaterial(_currentStack.Material);
        excessPartOfStack.gameObject.SetActive(true);
        excessPartOfStack.FallDownAndScaleDown();
    }

    void ArrangeCurrentStackTransformAfterCut(float excessSize, CutDirection cutDirection)
    {
        var newScale = _currentStack.transform.localScale;
        newScale.x -= excessSize;

        var newPosition = _currentStack.transform.position;

        switch (cutDirection)
        {
            case CutDirection.Left:
                newPosition.x += excessSize / 2;
                break;
            case CutDirection.Right:
                newPosition.x -= excessSize / 2;
                break;
        }

        _currentStack.transform.localScale = newScale;
        _currentStack.transform.position = newPosition;

        AddCurrentStackLocationToPath();
    }

    void AddCurrentStackLocationToPath()
    {
        var newPathLocation = _currentStack.transform.position;
        newPathLocation.y += _currentStack.transform.localScale.y / 2;
        newPathLocation.z -= _currentStack.transform.localScale.z / 2;
        _playerPath.Add(newPathLocation);
    }

    bool EvaluateGameOver(float excessSize)
    {
        if (_currentStack.transform.localScale.x - excessSize < _minStackSize)
        {
            GameOverCall();
            return true;
        }
        return false;
    }

    public void GameStartCall()
    {
        _isGameStart = true;
    }

    public void GameOverCall()
    {
        _isGameOver = true;
        _currentStack.FallDownAndScaleDown();
        _usedStackCount = 0;
        _perfectAlignmentCount = 0;
    }

    void SetCurrentStackMaterialRamdonly()
    {
        Material randomMaterial;
        do
        {
            randomMaterial = _materials[Random.Range(0, _materials.Count)];

        } while (randomMaterial.Equals(_prevStack.Material));

        _currentStack.SetMaterial(randomMaterial);
    }

    StackGround GetNextStackFromPool()
    {
        var nextStack = _stackPool[_stackPool.Count - 1];
        _stackPool.RemoveAt(_stackPool.Count - 1);
        _stackPool.Insert(0, nextStack);
        return nextStack;
    }

    void ArrangeCurrentStackTransformBeforeMove()
    {
        _currentStack.transform.localScale = new Vector3(_prevStack.transform.localScale.x, _prevStack.transform.localScale.y, _defaultZScale);
        _zPosition = _usedStackCount == 0 ? _prevStack.transform.position.z + (_prevStack.transform.localScale.z / 2) + (_defaultZScale / 2) : _zPosition + _defaultZScale;
        _currentStack.transform.position = new Vector3(_prevStack.transform.position.x, _prevStack.transform.position.y, _zPosition);
        SetCurrentStackMaterialRamdonly();
        _currentStack.gameObject.SetActive(true);
    }

    void StartStackMovement(StackGround stack, float moveDuration)
    {
        _movementXOffset *= -1;

        stack.transform.DOMoveX(_movementXOffset, moveDuration)
                        .SetEase(Ease.Linear)
                        .SetLoops(-1, LoopType.Yoyo)
                        .From(-_movementXOffset);
    }

    float GetLeftBoundPos(Transform objTransform)
    {
        return objTransform.position.x - (objTransform.localScale.x / 2);
    }

    float GetRightBoundPos(Transform objTransform)
    {
        return objTransform.position.x + (objTransform.localScale.x / 2);
    }

    public Vector3 GetNextPlayerPathLocation()
    {
        if (_playerPath.Count == 0) return Vector3.zero;

        var nextPath = _playerPath[0];
        _playerPath.RemoveAt(0);
        return nextPath;
    }

    public void NextRoundReady()
    {
        _isFinishGroundReady = false;
        _isRoundFinish = false;
        _prevStack = _finishGround;
        MoveNextStack();
        ArrangeFinishLinePosition();
        var minZ = _prevStack.transform.position.z + _defaultZScale;
        var maxZ = _prevStack.transform.position.z + _totalStackForReachToFinish * _defaultZScale;
        collectableSpawner.HandOutAllCollectables(minZ, maxZ);
    }
}

enum CutDirection
{
    None,
    Left,
    Right
}
