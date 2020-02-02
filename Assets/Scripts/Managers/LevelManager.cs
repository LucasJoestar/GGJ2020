﻿using EnhancedEditor;
using System;
using UnityEngine;

using Random = UnityEngine.Random;

public class LevelManager : MonoBehaviour
{
    [Serializable]
    private class SpawnPoint
    {
        public SuperColor       SpawnColor =                            SuperColor.Green;
        public Transform        PlayerOneSpawn, PlayerTwoSpawn =        null;
    }

    #region Fields / Properties
    public static LevelManager  I =                     null;

    [SerializeField, Section("SPAWN POINTS")]
    private SpawnPoint[]        spawnPoints =           new SpawnPoint[] { };
    #endregion

    #region Methods
    public void SpawnNewRepairable()
    {

    }


    private void Awake()
    {
        if (I)
        {
            Destroy(this);
            return;
        }

        I = this;
    }

    private void OnDestroy()
    {
        if (I == this) I = null;
    }

    // Start is called before the first frame update
    void Start()
    {
        Vector2 _playerOnePosition, _playerTwoPosition;
        if (spawnPoints.Length == 0)
        {
            _playerOnePosition = new Vector2(-3, 5);
            _playerTwoPosition = new Vector2(3, 5);
        }
        else
        {
            int _spawnIndex = Random.Range(0, spawnPoints.Length);
            _playerOnePosition = spawnPoints[_spawnIndex].PlayerOneSpawn.position;
            _playerTwoPosition = spawnPoints[_spawnIndex].PlayerTwoSpawn.position;
        }

        // Instantiate players
        Instantiate(GameManager.I.PlayerOne, _playerOnePosition, GameManager.I.PlayerOne.transform.rotation);
        Instantiate(GameManager.I.PlayerTwo, _playerTwoPosition, GameManager.I.PlayerTwo.transform.rotation);
    }

    private void OnDrawGizmos()
    {
        foreach (SpawnPoint _spawn in spawnPoints)
        {
            Gizmos.color = _spawn.SpawnColor.GetColor();
            Gizmos.DrawSphere(_spawn.PlayerOneSpawn.position, .25f);
            Gizmos.DrawSphere(_spawn.PlayerTwoSpawn.position, .25f);
        }
    }
    #endregion
}