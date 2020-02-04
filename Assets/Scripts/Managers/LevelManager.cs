using EnhancedEditor;
using System;
using System.Linq;
using UnityEngine;

using Random = UnityEngine.Random;

#pragma warning disable
public class LevelManager : MonoBehaviour
{
    [Serializable]
    private class SpawnPoint
    {
        public SuperColor       SpawnColor =                            SuperColor.Green;
        public Transform        PlayerOneSpawn, PlayerTwoSpawn =        null;
    }

    public static event Action  OnSpawnPlayers =        null;

    #region Fields / Properties
    public static LevelManager  I =                     null;

    public MyPlayercontroller   PlayerOne               { get; private set; }

    public MyPlayercontroller    PlayerTwo              { get; private set; }

    [SerializeField, HorizontalLine(order = 0), Section("SPAWN POINTS", order = 1), Space(order = 2)]
    private SpawnPoint[]        spawnPoints =           new SpawnPoint[] { };

    [SerializeField, HorizontalLine(2, SuperColor.Indigo, order = 0), Section("REPAIRABLE", order = 1), Space(order = 2)]
    private float               repairableInterval =    5;

    [SerializeField]
    private float               repairableMaxAmount =   3;

    [SerializeField]
    private Repairable[]        repairables =           new Repairable[] { };

    [SerializeField]
    private bool isTutorial = false;
    public bool IsTutorial { get { return isTutorial; } }
    #endregion

    #region Methods
    public void CallNewRepairable()
    {
        Invoke("SpawnNewRepairable", repairableInterval);
    }

    private void SpawnNewRepairable()
    {
        RepairTpe[] _taken = repairables.Where(r => r.gameObject.activeInHierarchy).Select(r => r.RepairType).Distinct().ToArray();
        Repairable[] _available = repairables.Where(r => !r.gameObject.activeInHierarchy && !_taken.Contains(r.RepairType)).ToArray();
        if (_available.Length == 0) return;

        _available[Random.Range(0, _available.Length)].Spawn();
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
        PlayerOne = Instantiate(GameManager.I.PlayerOne, _playerOnePosition, GameManager.I.PlayerOne.transform.rotation).GetComponent<MyPlayercontroller>();
        PlayerTwo = Instantiate(GameManager.I.PlayerTwo, _playerTwoPosition, GameManager.I.PlayerTwo.transform.rotation).GetComponent<MyPlayercontroller>();

        OnSpawnPlayers?.Invoke();

        foreach (Repairable _repairable in repairables)
        {
            _repairable.gameObject.SetActive(false);
        }
        for (int _i = 0; _i < repairableMaxAmount; _i++)
        {
            SpawnNewRepairable();
        }
    }

    private void OnDrawGizmos()
    {
        foreach (SpawnPoint _spawn in spawnPoints)
        {
            Gizmos.color = _spawn.SpawnColor.GetColor();
            Gizmos.DrawSphere(_spawn.PlayerOneSpawn.position, .25f);
            Gizmos.DrawSphere(_spawn.PlayerTwoSpawn.position, .25f);
        }

        foreach (Repairable _repairable in repairables)
        {
            if (!_repairable) continue;
            switch (_repairable.RepairType)
            {
                case RepairTpe.Balls:
                    Gizmos.color = SuperColor.Red.GetColor(_repairable.gameObject.activeInHierarchy ? 1 : .5f);
                    break;

                case RepairTpe.Plant:
                    Gizmos.color = SuperColor.Lime.GetColor(_repairable.gameObject.activeInHierarchy ? 1 : .5f);
                    break;

                case RepairTpe.Shield:
                    Gizmos.color = SuperColor.HarvestGold.GetColor(_repairable.gameObject.activeInHierarchy ? 1 : .5f);
                    break;

                case RepairTpe.Saw:
                    Gizmos.color = SuperColor.Maroon.GetColor(_repairable.gameObject.activeInHierarchy ? 1 : .5f);
                    break;

                case RepairTpe.Spikes:
                    Gizmos.color = SuperColor.Turquoise.GetColor(_repairable.gameObject.activeInHierarchy ? 1 : .5f);
                    break;

                default:
                    break;
            }

            Gizmos.DrawWireCube(_repairable.transform.position, Vector3.one * .5f);
        }
    }
    #endregion
}
