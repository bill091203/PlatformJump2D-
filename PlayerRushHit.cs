using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerRushHit : MonoBehaviour
{
    private PlayerControl player;
    private void Awake()
    {
        player = GetComponent<PlayerControl>();
    }
    private void OnTriggerStay2D(Collider2D collision)
    {
        if (player.isRushing && collision.gameObject.CompareTag("Box"))
        {
            Destroy(collision.gameObject);
        }
        if (player.isSmashingDown&& collision.gameObject.CompareTag("Box"))
        {
            Destroy(collision.gameObject);
        }
    }
}
