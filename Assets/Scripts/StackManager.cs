using DG.Tweening;
using System.Collections.Generic;
using UnityEngine;

public class StackManager : MonoBehaviour
{
    [SerializeField] List<StackGround> _stackPool;
    [SerializeField] List<Material> _materials;
    [SerializeField] StackGround _prevStack;
    [SerializeField] float _movementXOffset;
    [SerializeField] float _moveDuration;
    [SerializeField] float _defaultZScale;
    [SerializeField] float _alignmentTolerance = 0.15f;
    [SerializeField] float _minStackSize = 0.1f;

    StackGround _currentStack;
    float zPosition;
    float _prevLeftXBound;
    float _prevRightXBound;
    int _usedStackCount;
    bool _isGameOver;

    void Start()
    {
        MoveNextStack();
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            StackProcess();
        }
    }

    void MoveNextStack()
    {
        if (_isGameOver) return;

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
        if (_isGameOver) return;

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
        }
        else if (rightXBound - _prevRightXBound > _alignmentTolerance)
        {
            var excessSize = rightXBound - _prevRightXBound;

            if (EvaluateGameOver(excessSize)) return;
            ArrangeCurrentStackTransformAfterCut(excessSize, CutDirection.Right);
            CreateExcessPartOfStack(leftXBound, rightXBound, excessSize, CutDirection.Right);
        }
        else
        {
            _currentStack.transform.position = new Vector3(_prevStack.transform.position.x, _currentStack.transform.position.y, _currentStack.transform.position.z);
        }

        _usedStackCount++;
        _prevStack = _currentStack;
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
    }

    bool EvaluateGameOver(float excessSize)
    {
        if (_currentStack.transform.localScale.x - excessSize < _minStackSize)
        {
            GameOver();
            return true;
        }
        return false;
    }

    void GameOver()
    {
        Debug.Log("GAMEOVER");
        _isGameOver = true;
        _currentStack.FallDownAndScaleDown();
        _usedStackCount = 0;
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
        zPosition = _usedStackCount == 0 ? _prevStack.transform.position.z + (_prevStack.transform.localScale.z / 2) + (_defaultZScale / 2) : zPosition + _defaultZScale;
        _currentStack.transform.position = new Vector3(_prevStack.transform.position.x, _prevStack.transform.position.y, zPosition);
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
}

enum CutDirection
{
    None,
    Left,
    Right
}
