using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public class PhysicsCheck : MonoBehaviour
{
    public Vector2 bottomOffset;
    public Vector2 topOffset;
    public float checkRaduis;
    public LayerMask grounLayer;
    public LayerMask boxLayer;
    public bool isGround;
    public bool isGroundHead;

    private void Update()
    {
        Check();
    }
    //ºÏ≤‚µÿ√Ê
    public void Check()
    {
       isGround = Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, grounLayer)||
            Physics2D.OverlapCircle((Vector2)transform.position + bottomOffset, checkRaduis, boxLayer); ;
       isGroundHead= Physics2D.OverlapCircle((Vector2)transform.position + topOffset, 0.3f, grounLayer);
    }
    private void OnDrawGizmosSelected()
    {
        Gizmos.DrawWireSphere((Vector2)transform.position + bottomOffset, checkRaduis);
        Gizmos.DrawWireSphere((Vector2)transform.position + topOffset, 0.3f);
    }
}
