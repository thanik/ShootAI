using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class Bullet : MonoBehaviour
{
    public float lifeTime = 3.0f;
    public float speed = 1.5f;
    bool moving;
    bool hasHit = false;

    public PlayerAgent fireFrom;
    void Start()
    {

        moving = true;
        Destroy(gameObject, lifeTime);
    }


    void FixedUpdate()
    {

        if (moving)
            transform.Translate(transform.forward * speed, Space.World);



    }
    void OnCollisionEnter(Collision collision)
    {
        if (!hasHit)
        {
            if (collision.gameObject.CompareTag("Body"))
            {
                Player playerData = collision.gameObject.GetComponentInParent<Player>();
                playerData.health -= 10;
                //Debug.Log("Body hit! " + playerData.ToString());
                DestroyProyectile();
            }
            else if (collision.gameObject.CompareTag("Arm"))
            {
                Player playerData = collision.gameObject.GetComponentInParent<Player>();
                playerData.health -= 5;
                //Debug.Log("Arm hit! " + playerData.ToString());
                DestroyProyectile();
            }
            else if (collision.gameObject.CompareTag("Player"))
            {
                Player playerData = collision.gameObject.GetComponent<Player>();
                playerData.health -= 50;
                if (fireFrom)
                {
                    PlayerAgent fireTo = collision.gameObject.GetComponent<PlayerAgent>();
                    fireTo.AddReward(-1f);
                    fireFrom.AddReward(1f);
                    /*if (playerData.health <= 0)
                    {
                        fireFrom.AddReward(1f);
                    }*/
                }
                //Debug.Log("Player hit! " + playerData.ToString());
                DestroyProyectile();
            }
            else if (collision.gameObject.CompareTag("Wall") || collision.gameObject.CompareTag("AreaWall"))
            {
                if (fireFrom)
                {
                    fireFrom.AddReward(-0.5f);
                }
                DestroyProyectile();
            }
            else if (collision.gameObject.CompareTag("Ground"))
            {
                DestroyProyectile();
            }
        }
        //else if (collision.gameObject.tag == "Finish")
        //{ //This is to detect if the proyectile collides with the world, i used this tag because it is standard in Unity (To prevent asset importing issues)
        //    DestroyProyectile();
        //}



    }
    void DestroyProyectile()
    {
        hasHit = true;
        moving = false;
        Destroy(gameObject);
    }
}
