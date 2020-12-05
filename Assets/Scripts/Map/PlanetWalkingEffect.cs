using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;

public class PlanetWalkingEffect : MonoBehaviour
{
    public SpriteRenderer planet;
    public Player player;
    private int direction;
    private int currentIndex;
    private float moveSpeed;
    private float rotateSpeed;
    public float totalMoveTime;
    private float currentMoveTime;
    void Start()
    {
        currentIndex = 0;
    }
    public void planetWalking(int direction)
    {
        player.movement.canInput = false;
        this.direction = direction;
        currentMoveTime = totalMoveTime;
        moveSpeed = 4 / (totalMoveTime / 5);
        rotateSpeed = 90 / totalMoveTime;
        initWalkingPlanet();
    }
    void initWalkingPlanet()
    {
        planet.GetComponent<CircleCollider2D>().offset = new Vector2(0, 0);
        planet.GetComponent<CircleCollider2D>().radius = 3.17f;
    }
    void UpdateRegularPlanet()
    {
        UpdateIndex();
        planet.GetComponent<CircleCollider2D>().radius = 3.6f;
        switch (currentIndex)
        {
            case 0:
                planet.GetComponent<CircleCollider2D>().offset = new Vector2(0, -0.45f);
                break;
            case 1:
                planet.GetComponent<CircleCollider2D>().offset = new Vector2(-0.45f, 0);
                break;
            case 2:
                planet.GetComponent<CircleCollider2D>().offset = new Vector2(0, 0.45f);
                break;
            case 3:
                planet.GetComponent<CircleCollider2D>().offset = new Vector2(0.45f, 0);
                break;
            default:
                break;
        }
    }
    void UpdateIndex()
    {
        currentIndex = (currentIndex + this.direction + 4) % 4;
    }
    void Update()
    {
        if (currentMoveTime > 0)
        {
            currentMoveTime -= Time.deltaTime;
            if (currentMoveTime <= 0)
            {
                UpdateRegularPlanet();
                player.movement.canInput = true;
            }
            if (direction == -1)
            {
                player.transform.Translate(Vector3.left * Time.deltaTime * -moveSpeed);
                player.transform.localScale = new Vector3(-1, 1, 1);
                transform.Rotate(new Vector3(0,0, direction) * Time.deltaTime * rotateSpeed);
            }
            else
            {
                player.transform.Translate(Vector3.right * Time.deltaTime * -moveSpeed);
                player.transform.localScale = new Vector3(1, 1, 1);
                transform.Rotate(new Vector3(0, 0, direction) * Time.deltaTime * rotateSpeed);
            }
        } 
    }
}
