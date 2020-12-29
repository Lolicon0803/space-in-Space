using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class PlanetWalking : MonoBehaviour
{
    public float rotateSpeed;
    // 跳躍速度初速
    public float initJumpSpeed = 10f;
    // 目前垂直速度
    public float speedY;
    // 重力加速度
    public float accelerationOfGravity = 0.1f;
    public PlayerMovement player;
    public new Camera camera;
    public int[] nextScene;
    // Start is called before the first frame update
    void Start()
    {
        speedY = 0;
    }

    public int GetNextSceneIndex()
    {
        int Mod(int x, int m)
        {
            return (x % m + m) % m;
        }
        int degree = Mod((int)transform.rotation.eulerAngles.z, 360) - 45;
        int nextSceneIndex = (degree <= 270 && degree > 180) ? 3 :
                    (degree <= 180 && degree > 90) ? 2 :
                    (degree <= 90 && degree > 0) ? 1 :
                    0;
        return nextSceneIndex;
    }

    // Update is called once per frame
    void Update()
    {
        player.GetComponent<Animator>().SetBool("S", PlayerControlOnPlanet());
        TryJump();
        UpdateYPosition();
        UpdateCameraSize();
        RotateAllNPCMark();

        int nextSceneIndex = GetNextSceneIndex();
        if (player.transform.position.y >= 28 && nextScene[nextSceneIndex] != 0)
        {
            Destroy(player.gameObject);
            //SceneManager.LoadScene(nextScene[nextSceneIndex]);
            SceneController.Singleton.LoadSceneAsync(nextScene[nextSceneIndex], true);
        }
    }
    private void TryJump()
    {

        if (player.canInput && player.IsGroundOnPlanet && Input.GetKeyDown(KeyCode.Space))
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
    }
    private bool PlayerControlOnPlanet()
    {
        if (!player.canInput) return false;
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
            camera.orthographicSize = Mathf.Pow((player.transform.position.y / 14.4f), 1.8f) * 5;
        }
    }

    private void RotateAllNPCMark()
    {
        foreach (GameObject obj in GameObject.FindGameObjectsWithTag("Mark"))
        {
            obj.transform.rotation = Quaternion.Euler(0, 0, transform.rotation.z);
        }
    }
}
