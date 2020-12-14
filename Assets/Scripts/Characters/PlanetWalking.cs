﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlanetWalking : MonoBehaviour
{
    public float rotateSpeed;
    // 跳躍速度初速
    public float initJumpSpeed= 10f;
    // 目前垂直速度
    public float speedY;
    // 重力加速度
    public float accelerationOfGravity = 0.1f;
    public PlayerMovement player;
    public new Camera camera;
    // Start is called before the first frame update
    void Start()
    {
        speedY = 0;
    }

    // Update is called once per frame
    void Update()
    {
        if (player.canInput)
        {
            if (PlayerControlOnPlanet())
            {
                player.GetComponent<Animator>().SetBool("S", true);
            }
            else
            {
                player.GetComponent<Animator>().SetBool("S", false);
            }
            TryJump();
        }
        UpdateYPosition();
        UpdateCameraSize();
    }
    private void TryJump()
    {

        if (player.IsGroundOnPlanet  && Input.GetKeyDown(KeyCode.Space))
        {
            speedY = initJumpSpeed;
        }
    }

    private void UpdateYPosition()
    {
        // 自然掉落
        speedY -= accelerationOfGravity * Time.deltaTime;
        if (player.IsGroundOnPlanetForFalling && speedY < 0)
        {
            float upliftDistance = 0;
            float upliftMax = 0.2f;
            Vector3 prePosition = player.transform.position;
            // 卡進地底，推到地面
            while (player.IsGroundOnPlanetForFalling)
            {
                player.transform.Translate(Vector3.up * 0.001f);
                upliftDistance += 0.001f;
            }
            player.transform.Translate(Vector3.down * 0.001f);
            // 異常抬伸  阻止
            if (upliftDistance > upliftMax && prePosition.y <= 14.5 && prePosition.y >= 14)
            {
                player.transform.position = prePosition;
            }
            speedY = 0;
        }
        player.transform.Translate(Vector3.up * Time.deltaTime * speedY);



        //// 自然掉落
        //if (!player.IsGroundOnPlanetForFalling)
        //{
        //speedY -= accelerationOfGravity * Time.deltaTime;
        //}
        //if (player.IsGroundOnPlanetForFalling &&
        //    player.transform.position.y < 15)
        //{
        //    Vector3 prePosition = player.transform.position;
        //    // 卡進地底，推到地面
        //    while (player.IsGroundOnPlanetForFalling)
        //    {
        //        player.transform.Translate(Vector3.up * 0.001f);
        //    }
        //    player.transform.Translate(Vector3.down * 0.001f);
        //    Vector3 postPosition = player.transform.position;
        //    // 推到異常表面，回復原位置
        //    if (postPosition.y > 15)
        //    {
        //        player.transform.position = prePosition;
        //    }
        //    speedY = 0;

        //}
        //if (player.IsGroundOnPlanetForFalling && speedY < 0)
        //{
        //    if (player.transform.position.y > 15)
        //    {
        //        Vector3 prePosition = player.transform.position;
        //        float upliftDistance = 0;
        //        float upliftMax = 0.1f;
        //        while (player.IsGroundOnPlanetForFalling)
        //        {
        //            player.transform.Translate(Vector3.up * 0.001f);
        //            upliftDistance += 0.001f;
        //        }

        //        if (upliftDistance >= upliftMax)
        //        {
        //            player.transform.position = prePosition;
        //            speedY = 0;
        //        }
        //        else
        //        {
        //            player.transform.Translate(Vector3.down * 0.001f);
        //            upliftDistance -= 0.001f;
        //        }
        //    }
        //}
        //player.transform.Translate(Vector3.up * Time.deltaTime * speedY);
    }
    private bool PlayerControlOnPlanet()
    {
        if (Input.GetKey(KeyCode.LeftArrow))
        {
            transform.Rotate(new Vector3(0, 0, -1) * Time.deltaTime * rotateSpeed);
            player.transform.localScale = new Vector3(-1, 1, 1);
            return true;
        }
        else if (Input.GetKey(KeyCode.RightArrow))
        {
            transform.Rotate(new Vector3(0, 0, 1) * Time.deltaTime * rotateSpeed);
            player.transform.localScale = new Vector3(1, 1, 1);
            return true;
        }
        return false;
    }
    private void UpdateCameraSize()
    {
        if (player.transform.position.y <= 14.4)
        {
            camera.orthographicSize = 5;
        }
        else
        {
            camera.orthographicSize = Mathf.Pow( (player.transform.position.y / 14.4f), 1.8f) * 5;
        }
    }
}