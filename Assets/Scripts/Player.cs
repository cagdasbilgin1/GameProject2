using System.Collections;
using UnityEngine;

public class Player : MonoBehaviour
{
    [SerializeField] CharacterController controller;
    [SerializeField] float moveSpeed;
    [SerializeField] float groundRayDistance = 0.4f;
    [SerializeField] float fallDurationThreshold = 3f;
    [SerializeField] Transform followLookTransformForCam;
    [SerializeField] LayerMask groundMask;

    bool isGrounded;
    float fallTimer;
    bool isRunning;
    bool isGameOver;
    float lockedXPos;
    float lockedYPos;

    float tickUpdateFrequency = 0.5f;

    private void OnValidate()
    {
        UpdateFollowLookTransformPosition();
    }

    void Start()
    {
        StartCoroutine(TickUpdateRoutine());
    }

    public void StartSprint()
    {
        isRunning = true;
        lockedXPos = transform.position.x;
        lockedYPos = transform.position.y;
    }

    IEnumerator TickUpdateRoutine()
    {
        while (!isGameOver)
        {
            TickUpdate();
            yield return new WaitForSeconds(tickUpdateFrequency);
        }
    }

    void Update()
    {
        UseGravity();
        MoveForward();
    }

    void TickUpdate()
    {
        GroundCheck();
        CheckFall();
    }

    void GroundCheck()
    {
        isGrounded = Physics.CheckSphere(transform.position, groundRayDistance, groundMask);
    }

    void UseGravity()
    {
        controller.Move(9.81f * Vector3.down * Time.deltaTime);
    }

    void CheckFall()
    {
        if (!isGrounded)
        {
            fallTimer += tickUpdateFrequency;

            if (fallTimer >= fallDurationThreshold)
            {
                GameOver();
            }
        }
    }

    void MoveForward()
    {
        if (!isRunning) return;

        controller.Move(moveSpeed * Vector3.forward * Time.deltaTime);
        followLookTransformForCam.position = new Vector3(lockedXPos, lockedYPos, controller.transform.position.z);
    }

    void GameOver()
    {
        isGameOver = true;
        Debug.Log("GAME OVER");
    }

    void UpdateFollowLookTransformPosition()
    {
        if (followLookTransformForCam != null)
        {
            followLookTransformForCam.position = new Vector3(lockedXPos, lockedYPos, controller.transform.position.z);
        }
    }

    void OnDrawGizmos()
    {
        if (followLookTransformForCam != null)
        {
            Gizmos.color = Color.red;
            Gizmos.DrawSphere(followLookTransformForCam.position, 0.2f);
        }
    }
}
