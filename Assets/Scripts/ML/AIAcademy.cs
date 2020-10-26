using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MLAgents;

public class AIAcademy : Academy
{
    public List<BattleArea> battleAreas = new List<BattleArea>();
    public override void AcademyReset()
    {
        foreach(BattleArea area in battleAreas)
        {
            area.isReadyForPlaying = false;
            area.clearLevel();
            area.generateLevel();
        }
        base.AcademyReset();

    }
}
