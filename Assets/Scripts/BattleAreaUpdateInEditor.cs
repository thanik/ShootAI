using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[ExecuteInEditMode]
public class BattleAreaUpdateInEditor : MonoBehaviour
{
    private BattleArea battleArea;
    private void Start()
    {
        battleArea = GetComponent<BattleArea>();
    }
    
    void Update()
    {
        battleArea.ground.transform.localScale = new Vector3(battleArea.height, 1, battleArea.width);
    }
}
