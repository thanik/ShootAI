using MLAgents;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
public class PlayerController : MonoBehaviour
{
    private RayPerception3D rayPer;
    private Player player;

    public List<float> rayPerceived;
    public float rayDistance = 50f;
    string[] detectableObjects = { "Player", "Wall", "AreaWall", "Bullet" };
    public float[] rayAngles = { 0f, 180f, 90f, -90f };
    public float angleInDegrees;

    void Start()
    {

        rayPer = GetComponent<RayPerception3D>();
        player = GetComponent<Player>();
    }

    void Update()
    {
        if (player && rayPer)
        {
            rayPerceived = rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f);
            float inputHorizontal = Input.GetAxis("Horizontal");
            float inputVertical = Input.GetAxis("Vertical");
            player.Move(inputHorizontal, inputVertical);
            if(Input.GetMouseButton(0))
            {
                player.Fire();
            }

            Vector3 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
            mousePos.y = transform.position.y;
            //mousePointer.transform.position = mousePos;
            float deltaY = mousePos.z - transform.position.z;
            float deltaX = mousePos.x - transform.position.x;
            angleInDegrees = Mathf.Atan2(deltaY, deltaX) * 180 / Mathf.PI;
            player.updateAim(angleInDegrees);

        }
    
    }
}
