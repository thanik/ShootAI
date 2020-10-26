using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class PlayerAgent : Agent
{
    private AIAcademy aiAcademy;
    private RayPerception3D rayPer;
    private Player player;
    private BattleArea battleArea;

    public List<float> rayPerceived;
    public float rayDistance = 60f;
    string[] detectableObjects = { "Player", "Wall", "AreaWall", "Bullet" };
    public float[] rayAngles = { 0f, 15f, -15f, 30f, -30f, 45f, -45f, 60f, -60f, 75f, -75f, 90f, -90f, 105f, -105f, 120f, -120f, 135f, -135f, 150f, -150f, 165f, -165f, 180f };
    public bool isVisual = false;
    public Camera agentCam;
    public override void InitializeAgent()
    {
        base.InitializeAgent();
        battleArea = GetComponentInParent<BattleArea>();
        rayPer = GetComponent<RayPerception3D>();
        player = GetComponent<Player>();
        aiAcademy = FindObjectOfType<AIAcademy>();


    }

    public override void CollectObservations()
    {
        if (isVisual)
        {
            AddVectorObs(player.health / 100);
            AddVectorObs(player.ammo / player.maxAmmo[(int)player.currentWeapon]);
            AddVectorObs(System.Convert.ToInt32(player.reloading));
        }
        else
        {
            AddVectorObs(player.health / 100);
            AddVectorObs(player.ammo / player.maxAmmo[(int)player.currentWeapon]);
            AddVectorObs(System.Convert.ToInt32(player.reloading));
            AddVectorObs(rayPer.Perceive(rayDistance, rayAngles, detectableObjects, 0f, 0f));
            AddVectorObs(new Vector2(transform.localPosition.z / (battleArea.width / 2), transform.localPosition.x / (battleArea.height / 2)));
        }

        if (brain.brainParameters.vectorActionSpaceType == SpaceType.discrete)
        {
            // mask action
            float yNormalized = transform.localPosition.x / (battleArea.height / 2);
            float xNormalized = transform.localPosition.z / (battleArea.width / 2);

            if (xNormalized <= -1.0f)
            {
                //Debug.Log("can't go right");
                SetActionMask(0, 2);
            }
            else if (xNormalized >= 1.0)
            {
                //Debug.Log("can't go left");
                SetActionMask(0, 1);
            }

            if (yNormalized <= -1.0f)
            {
                //Debug.Log("can't go down");
                SetActionMask(1, 2);
            }
            else if (yNormalized >= 1.0)
            {
                //Debug.Log("can't go up");
                SetActionMask(1, 1);
            }
        }
    }

    public override void AgentAction(float[] vectorAction, string textAction)
    {
        //base.AgentAction(vectorAction, textAction);
        //shootAI
        AddReward(-0.05f);
        //surviveAI
        //AddReward(0.005f);
        if (player && rayPer)
        {
            if (brain.brainParameters.vectorActionSpaceType == SpaceType.continuous)
            {
                player.Move(vectorAction[0], vectorAction[1]);
                if (Mathf.Clamp(vectorAction[3], -1f, 1f) > 0.5f)
                {
                    player.Fire();
                }

                // new rotation
                if (brain.brainParameters.vectorActionDescriptions.Length == 6)
                {
                    int rotationDirection = 0;

                    if (vectorAction[2] < 0)
                    {
                        rotationDirection = 1;
                    }
                    else if (vectorAction[2] > 0)
                    {
                        rotationDirection = 2;
                    }
                    player.rotateAim(rotationDirection, vectorAction[5]);
                }
                else
                {
                    player.updateAim(vectorAction[2] * 180);
                }
            }
            else
            {
                /* vector action for discrete
                 * 0 = inputHorizontal
                 * 1 = inputVertical
                 * 2 = rotateDirection
                 * 3 = fire
                 * 4 = rotateSpeed
                 */
                var inputHorizontal = (int)vectorAction[0];
                var inputVertical = (int)vectorAction[1];
                float realHorizontal = 0f;
                float realVertical = 0f;

                if (inputHorizontal == 1)
                {
                    realHorizontal = -1f;
                }
                else if(inputHorizontal == 2)
                {
                    realHorizontal = 1f;
                }

                if (inputVertical == 1)
                {
                    realVertical = 1f;
                }
                else if(inputVertical == 2)
                {
                    realVertical = -1f;
                }

                player.Move(realHorizontal, realVertical);
                
                if ((int) vectorAction[3] == 1)
                {
                    player.Fire();
                }
                player.rotateAim((int)vectorAction[2], (int)vectorAction[4]);
            }

        }

    }

    public override void AgentReset()
    {
        //base.AgentReset();
        //float randomHeight = Mathf.Floor(battleArea.height / 2);
        //float randomWidth = Mathf.Floor(battleArea.width / 2);
        //transform.localPosition = new Vector3(Random.Range(-randomHeight, randomHeight), 1f, Random.Range(-randomWidth, randomWidth));
        player.health = 100;
        player.ammo = 12;
        player.currentWeapon = WeaponType.PISTOL;

    }

    public override void AgentOnDone()
    {
        battleArea.players.Remove(gameObject);
        Destroy(player.GetComponent<Player>().statusBar);
        Destroy(gameObject);
        //base.AgentOnDone();
    }

    public void FixedUpdate()
    {
        if (isVisual)
        {
            if (agentCam)
            {
                agentCam.Render();
            }
        }
    }

}
