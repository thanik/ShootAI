using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class ControlPanel : MonoBehaviour
{
    public BattleArea battleArea;

    public InputField numOfMLAgents1Txt;
    public InputField numOfMLAgents2Txt;
    public InputField numOfMLDisAgentsTxt;
    public InputField numOfRBSAgentsTxt;
    public Toggle enableHumanPlayer;
    public InputField battleAreaWidthTxt;
    public InputField battleAreaHeightTxt;
    public InputField CameraZoomTxt;
    public Toggle generateWall;

    public void applySettings()
    {
        int.TryParse(numOfMLAgents1Txt.text, out battleArea.numsOfMLAgent1);
        int.TryParse(numOfMLAgents2Txt.text, out battleArea.numsOfMLAgent2);
        int.TryParse(numOfMLDisAgentsTxt.text,out battleArea.numsOfMLDiscreteAgent);
        int.TryParse(numOfRBSAgentsTxt.text, out battleArea.numsOfMyAgent);
        int.TryParse(battleAreaWidthTxt.text, out battleArea.width);
        int.TryParse(battleAreaHeightTxt.text, out battleArea.height);
        battleArea.numsOfHuman = (enableHumanPlayer.isOn) ? 1 : 0;
        battleArea.randomWall = generateWall.isOn;
        battleArea.clearLevel();
    }

    public void applyCameraZoomValue()
    {
        if(float.TryParse(CameraZoomTxt.text, out float cameraZoom))
        {
            if (cameraZoom > 0)
            {
                Camera.main.orthographicSize = cameraZoom;
            }
            else
            {
                Camera.main.orthographicSize = 10f;
            }
        }
        else
        {
            Camera.main.orthographicSize = 10f;
        }
        
    }
}
