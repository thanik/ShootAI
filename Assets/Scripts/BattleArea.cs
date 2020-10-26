using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using MLAgents;

public class BattleArea : MonoBehaviour
{
    public GameObject wall;
    public GameObject ground;
    public GameObject playerPrefab;
    public Brain mlBrain1;
    public Brain mlBrain2;
    public Brain mlDiscreteBrain;
    public Brain mlDiscreteVisualBrain;
    public Material humanMaterial;
    public Material mlAgent1Material;
    public Material mlAgent2Material;
    public Material mlDiscreteAgentMaterial;
    public Text winText;
    public Text aliveStatText;
    public GameObject kdStatText;
    public int numsOfMLAgent1 = 0;
    public int numsOfMLAgent2 = 0;
    public int numsOfHuman = 0;
    public int numsOfMyAgent = 1;
    public int numsOfMLDiscreteAgent = 1;
    public int width;
    public int height;
    public int alivePlayerCount = 0;
    public float wallRandomThreshold = 0.92f;

    public bool randomWall = false;
    public bool useVisualForDiscrete = false;
    public Camera agentCam;

    public int spawnedMLAgent1 = 0;
    public int spawnedMLAgent2 = 0;
    public int spawnedHuman = 0;
    public int spawnedMyAgent = 0;
    public int spawnedMLDiscreteAgent = 0;
    int spawnedPlayer = 0;
    int resetCount = 0;

    public List<GameObject> players = new List<GameObject>();
    public List<GameObject> walls = new List<GameObject>();

    public bool isReadyForPlaying = false;
    public List<Vector3> listOfNoneWallPosition = new List<Vector3>();
    // Start is called before the first frame update
    void Start()
    {
        GetComponentInParent<AIAcademy>().battleAreas.Add(this);
        // set area
        ground.transform.localScale = new Vector3(height, 1, width);

    }

    public void generateAreaBoundWalls()
    {
        // generate walls
        GameObject topWall = Instantiate(wall, this.transform);
        float heightOffset = (height % 2 == 0) ? 0.5f : 1f;
        float widthOffset = (width % 2 == 0) ? 0.5f : 1f;
        topWall.transform.localPosition = new Vector3((height / 2) + heightOffset, 1, 0);
        topWall.transform.localScale = new Vector3(1f, 1f, width);
        topWall.tag = "AreaWall";
        GameObject bottomWall = Instantiate(wall, this.transform);
        bottomWall.transform.localPosition = new Vector3(-(height / 2) - heightOffset, 1, 0);
        bottomWall.transform.localScale = new Vector3(1f, 1f, width);
        bottomWall.tag = "AreaWall";
        GameObject leftWall = Instantiate(wall, this.transform);
        leftWall.transform.localPosition = new Vector3(0, 1, (width / 2) + widthOffset);
        leftWall.transform.localScale = new Vector3(height, 1f, 1f);
        leftWall.tag = "AreaWall";
        GameObject rightWall = Instantiate(wall, this.transform);
        rightWall.transform.localPosition = new Vector3(0, 1, -(width / 2) - widthOffset);
        rightWall.transform.localScale = new Vector3(height, 1f, 1f);
        rightWall.tag = "AreaWall";
        walls.Add(topWall);
        walls.Add(bottomWall);
        walls.Add(leftWall);
        walls.Add(rightWall);
    }

    public void clearLevel()
    {
        //isReadyForPlaying = false;
        if (kdStatText)
        {
            kdStatText.GetComponent<StatsCounter>().gameReset();
        }
        foreach (GameObject player in players)
        {
            if (player.GetComponent<PlayerAgent>().brain)
            {
                player.GetComponent<PlayerAgent>().Done();
            }
            else
            {
                Destroy(player.GetComponent<Player>().statusBar);
                Destroy(player);
            }
        }
        players.Clear();
        spawnedHuman = 0;
        spawnedMLAgent1 = 0;
        spawnedMLAgent2 = 0;
        spawnedMyAgent = 0;
        spawnedMLDiscreteAgent = 0;
        spawnedPlayer = 0;
        if (walls.Count > 0)
        {
            foreach (GameObject wall in walls)
            {
                Destroy(wall);
            }
        }
        walls.Clear();
        listOfNoneWallPosition.Clear();


    }

    public void generateLevel()
    {
        generateAreaBoundWalls();
        if (randomWall)
        {
            float heightOffset = (height % 2 == 0) ? 0.5f : 0f;
            float widthOffset = (width % 2 == 0) ? 0.5f : 0f;
            float randomHeight = Mathf.Floor(height / 2);
            float randomWidth = Mathf.Floor(width / 2);
            //Debug.Log((-randomHeight + heightOffset) + " to " + (randomHeight - heightOffset) + "," + (-randomWidth + widthOffset) + " to " + (randomWidth - widthOffset));
            for (float x = -randomHeight + heightOffset; x <= randomHeight - heightOffset; x ++)
            {
                for (float y = -randomWidth + widthOffset; y <= randomWidth - widthOffset; y ++)
                {
                    // Should we place a wall?
                    float randomValue = Random.value;
                    if (randomValue > wallRandomThreshold)
                    {
                        // Spawn a wall
                        Vector3 pos = new Vector3(x, 1f, y);
                        GameObject newWall = Instantiate(wall, transform);
                        newWall.transform.localPosition = pos;
                        walls.Add(newWall);
                    }
                    else
                    {
                        listOfNoneWallPosition.Add(new Vector3(x, 1f, y));
                    }
                   
                }
            }
            while (spawnedPlayer < numsOfHuman + numsOfMLAgent1 + numsOfMLAgent2 + numsOfMyAgent + numsOfMLDiscreteAgent)
            {
                spawnedPlayer = spawnedMLAgent1 + spawnedMLAgent2 + spawnedHuman + spawnedMyAgent + spawnedMLDiscreteAgent;
                // Spawn the player
                int randomPos = Random.Range(0, listOfNoneWallPosition.Count - 1);

                Vector3 pos = listOfNoneWallPosition[randomPos];
                if (spawnedHuman < numsOfHuman)
                {
                    spawnPlayer(2, pos);
                }
                else if (spawnedMLAgent1 < numsOfMLAgent1)
                {
                    spawnPlayer(0, pos);
                }
                else if (spawnedMLAgent2 < numsOfMLAgent2)
                {
                    spawnPlayer(4, pos);
                }
                else if (spawnedMyAgent < numsOfMyAgent)
                {
                    spawnPlayer(1, pos);

                }
                else if (spawnedMLDiscreteAgent < numsOfMLDiscreteAgent)
                {
                    spawnPlayer(3, pos);
                }
                listOfNoneWallPosition.RemoveAt(randomPos);
            }
            spawnedPlayer = spawnedMLAgent1 + spawnedMLAgent2 + spawnedHuman + spawnedMyAgent + spawnedMLDiscreteAgent;
        }
        else
        {
            while (spawnedPlayer < numsOfHuman + numsOfMLAgent1 + numsOfMLAgent2 + numsOfMyAgent + numsOfMLDiscreteAgent)
            {
                spawnedPlayer = spawnedMLAgent1 + spawnedMLAgent2 + spawnedHuman + spawnedMyAgent + spawnedMLDiscreteAgent;
                // Spawn the player
                float randomHeight = Mathf.Floor(height / 2);
                float randomWidth = Mathf.Floor(width / 2);

                Vector3 pos = new Vector3(Random.Range(-randomHeight, randomHeight), 1f, Random.Range(-randomWidth, randomWidth));
                if (spawnedHuman < numsOfHuman)
                {
                    spawnPlayer(2, pos);
                }
                else if (spawnedMLAgent1 < numsOfMLAgent1)
                {
                    spawnPlayer(0, pos);
                }
                else if (spawnedMLAgent2 < numsOfMLAgent2)
                {
                    spawnPlayer(4, pos);
                }
                else if (spawnedMyAgent < numsOfMyAgent)
                {
                    spawnPlayer(1, pos);

                }
                else if (spawnedMLDiscreteAgent < numsOfMLDiscreteAgent)
                {
                    spawnPlayer(3, pos);
                }
            }
            spawnedPlayer = spawnedMLAgent1 + spawnedMLAgent2 + spawnedHuman + spawnedMyAgent + spawnedMLDiscreteAgent;
        }

    }

    void spawnPlayer(int type, Vector3 pos)
    {
        GameObject player = Instantiate(playerPrefab, transform);
        player.transform.localPosition = pos;
        player.transform.rotation = Quaternion.Euler(new Vector3(0f, Random.Range(0, 360)));
        switch (type)
        {
            case 0: // ML agent1
                player.GetComponent<PlayerAgent>().GiveBrain(mlBrain1);
                player.GetComponent<PlayerAgent>().AgentReset();
                player.name = "MLAgent1 (" + spawnedMLAgent1 + ")";
                foreach (MeshRenderer meshRenderer in player.GetComponentsInChildren<MeshRenderer>())
                {
                    meshRenderer.material = mlAgent1Material;
                }
                spawnedMLAgent1++;
                break;
            case 1: // my agent
                player.GetComponent<PlayerAgent>().GiveBrain(null);
                player.name = "RBSAgent (" + spawnedMyAgent + ")";
                player.GetComponent<MyAgent>().enabled = true;
                spawnedMyAgent++;
                break;
            case 2: // human
                player.GetComponent<PlayerController>().enabled = true;
                player.GetComponent<PlayerAgent>().GiveBrain(null);
                player.name = "Human (" + spawnedHuman + ")";
                foreach (MeshRenderer meshRenderer in player.GetComponentsInChildren<MeshRenderer>())
                {
                    meshRenderer.material = humanMaterial;
                }
                spawnedHuman++;
                break;
            case 3: // ML discrete agent
                if (useVisualForDiscrete)
                {
                    player.GetComponent<PlayerAgent>().isVisual = true;
                    player.GetComponent<PlayerAgent>().GiveBrain(mlDiscreteVisualBrain);
                    player.GetComponent<PlayerAgent>().agentParameters.agentCameras.Add(agentCam);
                }
                else
                {
                    player.GetComponent<PlayerAgent>().GiveBrain(mlDiscreteBrain);
                }
                player.GetComponent<PlayerAgent>().AgentReset();
                player.name = "MLDisAgent (" + spawnedMLDiscreteAgent + ")";
                foreach (MeshRenderer meshRenderer in player.GetComponentsInChildren<MeshRenderer>())
                {
                    meshRenderer.material = mlDiscreteAgentMaterial;
                }
                spawnedMLDiscreteAgent++;
                break;
            case 4: // ML agent2
                player.GetComponent<PlayerAgent>().GiveBrain(mlBrain2);
                player.GetComponent<PlayerAgent>().AgentReset();
                player.name = "MLAgent2 (" + spawnedMLAgent2 + ")";
                foreach (MeshRenderer meshRenderer in player.GetComponentsInChildren<MeshRenderer>())
                {
                    meshRenderer.material = mlAgent2Material;
                }
                spawnedMLAgent2++;
                break;
        }
    }


    // Update is called once per frame
    void FixedUpdate()
    {
        //alivePlayerCount = 0;
        //for (int i = 0; i < players.Count; i++)
        //{
        //    Player player = players[i].GetComponent<Player>();
        //    if (player)
        //    {
        //        if (player.health > 0)
        //        {
        //            alivePlayerCount += 1;
        //        }
        //    }
        //}

        //if (alivePlayerCount == 1)
        if (isReadyForPlaying)
        {
            if (players.Count == 1)
            {
                if (winText)
                {
                    winText.text = players[0].name + " wins!";
                }
                if (kdStatText)
                {
                    kdStatText.GetComponent<StatsCounter>().addGameCount();
                }
                clearLevel();
                generateLevel();
            }
            else if (players.Count == 0)
            {
                Debug.Log(players.Count + " " + spawnedPlayer);
                if (winText)
                {
                    winText.text = "Draw!";
                }
                clearLevel();
                generateLevel();
            }
        }
        else if(spawnedPlayer > 0 && players.Count == spawnedPlayer)
        {
            isReadyForPlaying = true;
        }

    }

    public void addAliveTimeStat(string agentName, float time)
    {
        if (aliveStatText)
        {
            aliveStatText.text += agentName + ": " + time.ToString("#.##") + "s.\n";
        }
        else
        {
            return;
        }
    }

    public void clearAliveTimeStat()
    {
        if (aliveStatText)
        {
            aliveStatText.text = "Alive time stats:\n";
        }
        else
        {
            return;
        }
    }
}
