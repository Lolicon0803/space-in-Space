using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement movement;

    private bool walkParameter;
    private bool idleParameter;
    private readonly int idle = Animator.StringToHash("Idle");
    private readonly int walkR = Animator.StringToHash("WalkR");
    private readonly int walkL = Animator.StringToHash("WalkL");
    private readonly int lie = Animator.StringToHash("Lie");

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        movement = GetComponent<PlayerMovement>();
        movement.OnFireBag += PlayWalkAnimation;
        movement.OnStop += PlayIdleAnimation;
        walkParameter = false;
        idleParameter = false;
        if (ObjectTempoControl.Singleton != null)
            ObjectTempoControl.Singleton.AddToBeatAction(PlayIdleAnimation, TempoActionType.Whole);
    }

    public void PlayIdleAnimation()
    {
        animator.SetBool(idle, idleParameter);
        idleParameter = !idleParameter;
    }

    public void BackWalkToIdle()
    {
        animator.SetBool(walkL, false);
        animator.SetBool(walkR, false);
    }

    public void PlayWalkAnimation()
    {
        // Left or right foot.
        walkParameter = !walkParameter;
        if (walkParameter)
            animator.SetBool(walkL, true);
        else
            animator.SetBool(walkR, true);
    }

    private void PlayWalkAnimation(Vector2 direction)
    {
        // Left or right foot.
        walkParameter = !walkParameter;
        if (walkParameter)
            animator.SetBool(walkL, true);
        else
            animator.SetBool(walkR, true);
        StartCoroutine(WaitPlayerStop());
    }

    private IEnumerator WaitPlayerStop()
    {
        while (Vector2.Distance(transform.position, movement.movePoint) >= movement.NowSpeed * Time.deltaTime)
            yield return null;
        animator.SetBool(walkL, false);
        animator.SetBool(walkR, false);
    }

    public void PlayLie()
    {
        animator.SetBool(lie, true);
        ObjectTempoControl.Singleton.AddToBeatAction(PlayStandOnGround, TempoActionType.Half);
    }

    private void PlayStandOnGround()
    {
        //GetComponent<Animation>().Play(,)
        movement.BackToGrid();
        animator.SetBool(lie, false);
        ObjectTempoControl.Singleton.RemoveToBeatAction(PlayStandOnGround, TempoActionType.Half);
    }
}
