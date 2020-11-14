using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerAnimationManager : MonoBehaviour
{
    private Animator animator;
    private PlayerMovement player;

    private bool walkParameter;
    private readonly int walkR = Animator.StringToHash("WalkR");
    private readonly int walkL = Animator.StringToHash("WalkL");

    // Start is called before the first frame update
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerMovement>();
        player.OnWalk += PlayWalkAnimation;
        player.OnFireBag += PlayWalkAnimation;
        walkParameter = false;
    }

    private void PlayWalkAnimation(Vector2 direction)
    {
        // Change sprite direction.
        if (direction == -(Vector2)transform.right)
            transform.localScale = new Vector3(-1, 1, 1);
        else if (direction == (Vector2)transform.right)
            transform.localScale = new Vector3(1, 1, 1);

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
        while (Vector2.Distance(transform.position, player.movePoint) >= player.NowSpeed * Time.deltaTime)
            yield return null;
        animator.SetBool(walkL, false);
        animator.SetBool(walkR, false);
    }

}
