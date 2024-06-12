using System.Collections;
using System.Collections.Generic;
using System.Runtime.CompilerServices;
using UnityEngine;

public class PlayerAnimation : MonoBehaviour
{
    private Animator animator;
    private PlayerControl player;
    void Start()
    {
        animator = GetComponent<Animator>();
        player = GetComponent<PlayerControl>();
    }

    // Update is called once per frame
    void Update()
    {
        animator.SetBool("isFalling", player.isFalling);
        animator.SetBool("isJumping", player.isJumping);
        animator.SetBool("isPrepareRush", player.isPrepareRush);
        animator.SetBool("isRushing", player.isRushing);
        animator.SetBool("isSmashingDown", player.isSmashingDown);
        animator.SetBool("isPrepareSmashDown", player.isPrepareSmashDown);
        animator.SetBool("isWalking", player.isWalking);
        animator.SetBool("isPrepareSmashDown", player.isPrepareSmashDown);
    }
}
