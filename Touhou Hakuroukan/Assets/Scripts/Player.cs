using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Player : MonoBehaviour
{
    public Rigidbody2D rb;
    public float moveSpeedHigh = 8.0f;
    public float moveSpeedLow = 4.0f;

    private Vector2 moveDirection;

    private void Awake()
    {
        rb = gameObject.GetComponent<Rigidbody2D>();
    }

    // Update is called once per frame
    void Update()
    {
        moveDirection.x = (int)Input.GetAxisRaw("Horizontal");
        moveDirection.y = (int)Input.GetAxisRaw("Vertical");
    }

    private void FixedUpdate()
    {
        if (moveDirection.x != 0 && moveDirection.y != 0)       //对角线移动，削减速度
        {
            if (Input.GetKey(KeyCode.LeftShift))
                rb.MovePosition(rb.position + moveDirection * moveSpeedLow * Time.deltaTime * 0.707f);
            else rb.MovePosition(rb.position + moveDirection * moveSpeedHigh * Time.deltaTime * 0.707f);
        }
        else
        {
            if (Input.GetKey(KeyCode.LeftShift))
                rb.MovePosition(rb.position + moveDirection * moveSpeedLow * Time.deltaTime);
            else rb.MovePosition(rb.position + moveDirection * moveSpeedHigh * Time.deltaTime);
        }
    }
}
