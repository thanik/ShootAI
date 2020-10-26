using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;
using System.Linq;

public class MyAgent : MonoBehaviour
{
    private Player player;
    private BattleArea battleArea;
    private RayPerception3D rayPer;
    private float randomSpreadAim = 0f;
    private Dictionary<GameObject, float> playersDistance = new Dictionary<GameObject, float>();
    private float lastMovementDecisionTime = 0f;
    private float[] movementNumbers = { -1f, 0f, 1f };

    public float inputHorizontal = 0f;
    public float inputVertical = 0f;

    public List<float> rayPerceived;
    public float rayDistance = 60f;
    string[] detectableObjects = { "Player", "Wall", "AreaWall", "Bullet" };
    public float[] rayAngles = { 0f };
    public float movementDecisionInterval = 0.5f;


    void Start()
    {
        battleArea = GetComponentInParent<BattleArea>();
        rayPer = GetComponent<RayPerception3D>();
        player = GetComponent<Player>();
    }


    void FixedUpdate()
    {
        if (battleArea && rayPer)
        {
            rayPerceived = rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f);
            float positionOffset = 0.75f;

            // check for walls in 4 directions
            bool top = Physics.Linecast(new Vector3(transform.position.x + positionOffset, 0f, transform.position.z), new Vector3(transform.position.x + positionOffset, 1f, transform.position.z));
            bool bottom = Physics.Linecast(new Vector3(transform.position.x - positionOffset, 0f, transform.position.z), new Vector3(transform.position.x - positionOffset, 1f, transform.position.z));
            bool left = Physics.Linecast(new Vector3(transform.position.x, 0f, transform.position.z + positionOffset), new Vector3(transform.position.x, 1f, transform.position.z + positionOffset));
            bool right = Physics.Linecast(new Vector3(transform.position.x, 0f, transform.position.z - positionOffset), new Vector3(transform.position.x, 1f, transform.position.z - positionOffset));
            //Debug.Log("t:" + top + ", b:" + bottom + ", l:" + left + ", r:" + right);
            /* rayPerceived
             * 0 = Player
             * 1 = Wall
             * 2 = AreaWall
             * 3 = bullet
             * 
             * 5 = distance
             */

            // dodge very near bullet
            Collider[] overlappedObjects = Physics.OverlapSphere(transform.position, 1.35f);
            foreach (Collider collider in overlappedObjects)
            {
                if (collider.CompareTag("Bullet") && !collider.gameObject.GetComponent<Bullet>().fireFrom.Equals(GetComponent<PlayerAgent>()))
                {
                    Vector3 distance = collider.transform.position - transform.position;
                    //Debug.Log(distance);
                    //player.Move(distance.z, -distance.x);
                    transform.position = Vector3.MoveTowards(transform.position, collider.transform.position, -1 * player.moveSpeed * Time.deltaTime);
                    return;
                }
            }

            // lock on closest target
            GameObject closestPlayer = GetClosestPlayer(battleArea.players.Where(go => go != gameObject).ToArray());

            if (closestPlayer)
            {
                float deltaZ = closestPlayer.transform.position.z - transform.position.z;
                float deltaX = closestPlayer.transform.position.x - transform.position.x;
                float angleInDegrees = Mathf.Atan2(deltaZ, deltaX) * 180 / Mathf.PI;
                player.updateAim(angleInDegrees + randomSpreadAim);

                Vector3 directionToTarget = closestPlayer.transform.position - transform.position;
                float dSqrToTarget = directionToTarget.sqrMagnitude;
                if (Time.time > lastMovementDecisionTime + movementDecisionInterval)
                {
                    if (dSqrToTarget < 60f)
                    {
                    // random value to make it difficult to guess movement

                    inputHorizontal = movementNumbers[Random.Range(0, 3)];
                    inputVertical = movementNumbers[Random.Range(0, 3)];

                    }
                    else
                    {
                        // move to target
                        //Debug.Log(dSqrToTarget);
                        if (directionToTarget.x > 0)
                        {
                            inputVertical = 1f;
                        }
                        else if (directionToTarget.x < 0)
                        {
                            inputVertical = -1f;
                        }

                        if (directionToTarget.z > 0)
                        {
                            inputHorizontal = -1f;
                        }
                        else if (directionToTarget.z < 0)
                        {
                            inputHorizontal = 1f;
                        }
                    }

                    // dodge the wall first
                    if (top)
                    {
                        //Debug.Log("top: ");
                        inputVertical = -1;
                    }

                    if (bottom)
                    {
                        //Debug.Log("down: ");
                        inputVertical = 1;
                    }

                    if (left)
                    {
                        //Debug.Log("left: ");
                        inputHorizontal = 1;
                    }

                    if (right)
                    {
                        //Debug.Log("right: ");
                        inputHorizontal = -1;
                    }
                    // movement
                    movementDecisionInterval = Random.Range(0f, 1f);
                    lastMovementDecisionTime = Time.time;
                }
                player.Move(inputHorizontal, inputVertical);
                //if (rayPerceived[1] >= 1f)
                //{
                //    // this means wall
                //    // move randomly to deal with wall
                //    player.Move(inputHorizontal, inputVertical);
                //}
                //else
                //{
                //    // move around the enemy
                //    //transform.position += transform.forward * Time.deltaTime * (player.moveSpeed / 2);
                //}
            }

            if(rayPerceived[0] >= 1f && !player.reloading)
            {
                randomSpreadAim = Random.Range(-6f, 6f);
                player.Fire();
                
            }
        }
    }

    GameObject GetClosestPlayer(GameObject[] players)
    {
        GameObject bestTarget = null;
        float closestDistanceSqr = Mathf.Infinity;
        Vector3 currentPosition = transform.position;
        foreach (GameObject potentialTarget in players)
        {
            Vector3 directionToTarget = potentialTarget.transform.position - currentPosition;
            float dSqrToTarget = directionToTarget.sqrMagnitude;
            if (dSqrToTarget < closestDistanceSqr)
            {
                closestDistanceSqr = dSqrToTarget;
                bestTarget = potentialTarget;
            }
        }

        return bestTarget;
    }
}
