using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Box : MonoBehaviour
{
    private void OnCollisionEnter2D(Collision2D collision)
    {
        PlayerControl player = collision.gameObject.GetComponent<PlayerControl>();
        if (player && player.isRushing&&player.canDestory)
        {
            Destroy(gameObject);
        }
    }
}
