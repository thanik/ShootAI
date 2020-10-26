using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.Linq;

public class StatsCounter : MonoBehaviour
{
    private int gameCount = 0;
    private List<float> mlSurviveTimes = new List<float>();
    private List<float> mlDisSurviveTimes = new List<float>();
    private List<float> rbsSurviveTimes = new List<float>();
    private List<float> humanSurviveTimes = new List<float>();
    private Text kdStatText;
    private int resetCount = -1;
    void Start()
    {
        kdStatText = GetComponent<Text>();
    }

    // Update is called once per frame
    void Update()
    {
        if (resetCount == 200)
        {
            // write result to file
            List<string> outputLines = new List<string>();
            outputLines.Add("gameCount," + resetCount);
            outputLines.Add("successGameCount," + gameCount);
            outputLines.Add("failedGameCount," + (resetCount - gameCount));
            outputLines.Add("mlSurviveTimes");
            foreach(float mlTime in mlSurviveTimes)
            {
                outputLines.Add(mlTime.ToString());
            }
            outputLines.Add("mlDisSurviveTimes");
            foreach (float mlTime in mlDisSurviveTimes)
            {
                outputLines.Add(mlTime.ToString());
            }
            outputLines.Add("rbsSurviveTimes");
            foreach (float mlTime in rbsSurviveTimes)
            {
                outputLines.Add(mlTime.ToString());
            }
            outputLines.Add("humanSurviveTimes");
            foreach (float mlTime in humanSurviveTimes)
            {
                outputLines.Add(mlTime.ToString());
            }
            System.IO.File.WriteAllLines(@"output.csv", outputLines);
            Debug.Break();
        }
        string statsText = "Games count: ";
        statsText += resetCount + "\n";
        statsText += "Success Games count: ";
        statsText += gameCount + "\n";
        statsText += "Failed games count: " + (resetCount - gameCount) + "\n";
        statsText += "(ML, MLDis, RBS, Human)\nSurvive Time Mean: \n";
        statsText += (mlSurviveTimes.Count > 0 ? mlSurviveTimes.Average() : 0) + " / " + (mlDisSurviveTimes.Count > 0 ? mlDisSurviveTimes.Average() : 0) + " / " + (rbsSurviveTimes.Count > 0 ? rbsSurviveTimes.Average() : 0) + " / " + (humanSurviveTimes.Count > 0 ? humanSurviveTimes.Average() : 0) + "\n";
        statsText += "Survive Time Min: \n";
        statsText += (mlSurviveTimes.Count > 0 ? mlSurviveTimes.Min() : 0) + " / " + (mlDisSurviveTimes.Count > 0 ? mlDisSurviveTimes.Min() : 0) + " / " + (rbsSurviveTimes.Count > 0 ? rbsSurviveTimes.Min() : 0) + " / " + (humanSurviveTimes.Count > 0 ? humanSurviveTimes.Min() : 0) + "\n";
        statsText += "Survive Time Max: \n";
        statsText += (mlSurviveTimes.Count > 0 ? mlSurviveTimes.Max() : 0) + " / " + (mlDisSurviveTimes.Count > 0 ? mlDisSurviveTimes.Max() : 0) + " / " + (rbsSurviveTimes.Count > 0 ? rbsSurviveTimes.Max() : 0) + " / " + (humanSurviveTimes.Count > 0 ? humanSurviveTimes.Max() : 0) + "\n";
        statsText += "Agents' Deaths: \n";
        statsText += mlSurviveTimes.Count + " / " + mlDisSurviveTimes.Count + " / " + rbsSurviveTimes.Count + " / " + humanSurviveTimes.Count;
        kdStatText.text = statsText;
    }
    public void addMLSurviveTime(float time)
    {
        mlSurviveTimes.Add(time);
    }
    public void addRBSSurviveTime(float time)
    {
        rbsSurviveTimes.Add(time);
    }
    public void addHumanSurviveTime(float time)
    {
        humanSurviveTimes.Add(time);
    }
    public void addMLDisSurviveTime(float time)
    {
        mlDisSurviveTimes.Add(time);
    }
    public void addGameCount()
    {
        gameCount++;
    }

    public void clearStats()
    {
        mlSurviveTimes.Clear();
        rbsSurviveTimes.Clear();
        humanSurviveTimes.Clear();
        mlDisSurviveTimes.Clear();
        gameCount = 0;
        resetCount = -1;
    }

    public void gameReset()
    {
        resetCount++; 
    }
}
