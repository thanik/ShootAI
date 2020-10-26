using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public enum WeaponType { KNIFE, PISTOL, MACHINEGUN }

public class Player : MonoBehaviour
{

    public int health = 100;
    public int ammo = 12;
    public float moveSpeed = 10.0f;
    public WeaponType currentWeapon = WeaponType.PISTOL;
    public bool reloading = false;
    public bool destroyOnDied = false;
    public float aliveTime = 0f;

    public GameObject projectilePrefab;
    public Transform gunPivot;
    public GameObject statusBarPrefab;

    RectTransform canvasRect;
    public GameObject statusBar;

    Rigidbody myRigidBody;
    PlayerAgent agent;
    BattleArea battleArea;

    private float[] fireTime = { 0.15f, 0.3f, 0.1f };
    private float[] reloadTime = { 0f, 1.15f, 1.5f };
    public int[] maxAmmo = { 0, 12, 30 };
    private float lastFireTime = 0f;
    private float lastReloadTime = 0f;
    private float rotation = 0f;
    

    void Start()
    {
        battleArea = GetComponentInParent<BattleArea>();
        myRigidBody = GetComponent<Rigidbody>();
        Canvas canvas = FindObjectOfType<Canvas>();
        canvasRect = canvas.GetComponent<RectTransform>();
        agent = GetComponent<PlayerAgent>();

        statusBar = Instantiate(statusBarPrefab, canvas.transform);
        battleArea.players.Add(this.gameObject);
        rotation = transform.eulerAngles.y;
    }

    // Update is called once per frame
    void FixedUpdate()
    {
        
        if (reloading && Time.time > lastReloadTime + reloadTime[(int) currentWeapon])
        {
            ammo = maxAmmo[(int)currentWeapon];
            reloading = false;
        }

        if (health <= 0)
        {
            battleArea.addAliveTimeStat(gameObject.name, aliveTime);
            if (battleArea.kdStatText)
            {
                if (gameObject.name.StartsWith("MLAgent"))
                {
                    battleArea.kdStatText.GetComponent<StatsCounter>().addMLSurviveTime(aliveTime);
                }
                else if (gameObject.name.StartsWith("MLDisAgent"))
                {
                    battleArea.kdStatText.GetComponent<StatsCounter>().addMLDisSurviveTime(aliveTime);
                }
                else if (gameObject.name.StartsWith("RBSAgent"))
                {
                    battleArea.kdStatText.GetComponent<StatsCounter>().addRBSSurviveTime(aliveTime);
                }
                else if (gameObject.name.StartsWith("Human"))
                {
                    battleArea.kdStatText.GetComponent<StatsCounter>().addHumanSurviveTime(aliveTime);
                }
            }

            if (agent.brain)
            {
                agent.AddReward(-1f);
                agent.Done();
                //gameObject.SetActive(false);
               
            }
            else
            {
                if (destroyOnDied)
                {
                    battleArea.players.Remove(gameObject);
                    Destroy(statusBar);
                    Destroy(gameObject);
                }
            }
        }
        else
        {
            aliveTime += Time.deltaTime;
        }

        if (statusBar)
        {
            //uiText.GetComponent<Text>().text = "Health: " + health + "\nAmmo: " + ((reloading) ? "RE.." : ammo.ToString()) + "\n" + agent.GetReward().ToString("0.000");
            statusBar.GetComponent<StatusBar>().updateBar(health, ammo, maxAmmo[(int)currentWeapon], reloading);
            // Offset position above object bbox (in world space)
            float offsetPosX = transform.position.x + 0.75f;

            // Final position of marker above GO in world space
            Vector3 offsetPos = new Vector3(offsetPosX, transform.position.y, transform.position.z);

            // Calculate *screen* position (note, not a canvas/recttransform position)
            Vector2 canvasPos;
            Vector2 screenPoint = Camera.main.WorldToScreenPoint(offsetPos);

            // Convert screen position to Canvas / RectTransform space <- leave camera null if Screen Space Overlay
            RectTransformUtility.ScreenPointToLocalPointInRectangle(canvasRect, screenPoint, null, out canvasPos);

            // Set
            statusBar.transform.localPosition = canvasPos;
        }

    }
    public void Fire()
    {
        if (!reloading && health > 0)
        {
            if (Time.time > lastFireTime + fireTime[(int)currentWeapon])
            {
                lastFireTime = Time.time;
                switch (currentWeapon)
                {
                    case WeaponType.KNIFE:
                        break;
                    case WeaponType.PISTOL:
                        GameObject bullet = GameObject.Instantiate(projectilePrefab, gunPivot.position, gunPivot.rotation) as GameObject;
                        if (agent)
                        {
                            bullet.GetComponent<Bullet>().fireFrom = agent;
                        }

                        bullet.transform.Rotate(0, Random.Range(-5.5f, 5.5f), 0);
                        ammo -= 1;
                        break;
                    case WeaponType.MACHINEGUN:

                        break;
                }

                if (ammo == 0)
                {
                    Reload();
                }
            }
        }
    }

    public void Reload()
    {
        if (health > 0)
        {
            reloading = true;
            lastReloadTime = Time.time;
        }
    }

    public void Move(float inputHorizontal, float inputVertical)
    {
        if (myRigidBody && health > 0)
        {
            Vector3 newVelocity = new Vector3(inputVertical * moveSpeed, 0.0f, inputHorizontal * -moveSpeed);
            myRigidBody.velocity = newVelocity;
        }
    }

    public void updateAim(float aimAngle)
    {
        if (health > 0)
        {
            rotation = aimAngle;
            transform.eulerAngles = new Vector3(0, -rotation, 0);
        }
    }

    public void rotateAim(int direction, int speed)
    {
        if (health > 0)
        {
            switch(speed)
            {
                case 0:
                    speed = 1;
                    break;
                case 1:
                    speed = 5;
                    break;
                case 2:
                    speed = 10;
                    break;
                case 3:
                    speed = 20;
                    break;
                case 4:
                    speed = 40;
                    break;
            }
            if (direction == 1)
            {
                rotation += speed;
            }
            else if (direction == 2)
            {
                rotation -= speed;
            }
            transform.eulerAngles = new Vector3(0, -rotation, 0);
        }
    }

    public void rotateAim(int direction, float speed)
    {
        if (health > 0)
        {
            if (speed >= 1)
            {
                speed = 40;
            }
            else if (speed <= 0)
            {
                speed = 1;
            }
            else
            {
                speed *= 40;
            }
            
            if (direction == 1)
            {
                rotation += speed;
            }
            else if (direction == 2)
            {
                rotation -= speed;
            }
            transform.eulerAngles = new Vector3(0, -rotation, 0);
        }
    }
}
