using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelManager : MonoBehaviour
{

    private void Awake()
    {
        Enemy.OnEnemyKilled += OnEnemyKilled;   
    }

    private void OnEnemyKilled()
    {
        Debug.Log("Enemy Died!");
    }
}
