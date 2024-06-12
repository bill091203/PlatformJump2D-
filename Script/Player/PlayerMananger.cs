using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlayerMananger : MonoBehaviour
{
    public GameObject Player;
    public GameObject PlayerSquat;
    public Transform PlayerPosition;
    public Transform PlayerSquatPosition;

    private PlayerControl player;
    private PlayerController playerSquat;
    private bool canChange=true;
    private PlayerState ps=PlayerState.normal;
    private float dirction;
    //private bool canChange = true;

    public enum PlayerState { normal,squat}
    // Start is called before the first frame update
    void Start()
    {
        player = Player.GetComponent<PlayerControl>();
        playerSquat = PlayerSquat.GetComponent<PlayerController>();
    }

    // Update is called once per frame
    void Update()
    {
        Manager();
        if (Input.GetAxisRaw("Horizontal") != 0)
        {
            dirction = Input.GetAxisRaw("Horizontal");
        }
    }

    private void Manager()
    {
        if (Input.GetKey(KeyCode.S) && (player.isGround ))
        {
            player.gameObject.SetActive(false);
            playerSquat.gameObject.SetActive(true);
            playerSquat.dirction = dirction;
            ps=PlayerState.squat;
            if (canChange)
            {
                playerSquat.transform.position = PlayerSquatPosition.transform.position;
                canChange = false;
            }

        }
        if (Input.GetKeyUp(KeyCode.S)&& ps == PlayerState.squat)
        {
            if (playerSquat.physicsCheck.isGroundHead)
            {
                return;
            }
            ps = PlayerState.normal;
            canChange = true;
            player.gameObject.SetActive(true);
            playerSquat.gameObject.SetActive(false);
            player.direction = dirction;
            player.isWalking = false;
            player.isSmashingDown = false;
            player.isRushing = false;
            player.canRush = true;
            player.canSmashDown = true;
            player.rb.gravityScale = 1.0f;
            player.transform.position = PlayerPosition.transform.position;
        }
    }
}
