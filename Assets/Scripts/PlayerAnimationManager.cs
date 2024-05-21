using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    readonly string RUN_ANIM_KEY = "run";
    readonly string DANCE_ANIM_KEY = "dance";

    [SerializeField] Animator _animator;

    public void Dance()
    {
        _animator.SetTrigger(DANCE_ANIM_KEY);
    }

    public void Run()
    {
        _animator.SetTrigger(RUN_ANIM_KEY);
    }
}
