using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

#pragma warning disable
public class GameManager : MonoBehaviour
{
    #region Events
    public static event Action      OnScoreUpdate =         null;
    #endregion

    #region Fields / Properties
    /*********************
     ***   CONSTANTS   ***
     ********************/


    /**********************
     *****   STATIC   *****
     *********************/

    public static GameManager       I =                     null;


    /**********************
     *****   FIELDS   *****
     *********************/

    private List<MyPlayercontroller> playersReady = new List<MyPlayercontroller>();


    [SerializeField, Header("SCORE")]
    private int                     playerOneScore =        0;

    [SerializeField]
    private int                     playerTwoScore =        0;


    [SerializeField]
    private GameObject              playerOne =             null;

    [SerializeField]
    private GameObject              playerTwo =             null;


    [SerializeField, Header("MUSIC")]
    private AudioSource             musicSource =           null;

    // Done
    [SerializeField]
    private AudioClip               tutorial =              null;

    // Done
    [SerializeField]
    private AudioClip               musicOne =              null;

    // Done
    [SerializeField]
    private AudioClip               musicTwo =              null;

    // Done
    [SerializeField]
    private AudioClip               musicThree =            null;

    // Done
    [SerializeField]
    private AudioClip               victory =               null;
    public AudioClip                Victory { get { return victory; } }


    [SerializeField, Header("SOUNDS")]
    private AudioClip               fight =             null;
    public AudioClip                Fight { get { return fight; } }

    [SerializeField]
    private AudioClip               initSound =             null;
    public AudioClip                InitSound { get { return initSound; } }

    [SerializeField]
    private AudioClip               scoreSound = null;
    public AudioClip                Scoresound { get { return scoreSound; } }

    [SerializeField]
    private AudioClip               finalRound =            null;
    public AudioClip                FinalRound { get { return finalRound; } }

    // Done
    [SerializeField]
    private AudioClip               playerOneWin =          null;
    public AudioClip                PlayerOneWin { get { return playerOneWin; } }

    // Done
    [SerializeField]
    private AudioClip               playerTwoWin =          null;
    public AudioClip                PlayerTwoWin { get { return playerTwoWin; } }


    [SerializeField]
    private AudioClip               roundOne =              null;
    public AudioClip                RoundOne { get { return roundOne; } }

    [SerializeField]
    private AudioClip               roundTwo =              null;
    public AudioClip                RoundTwo { get { return roundTwo; } }

    [SerializeField]
    private AudioClip               roundThree =            null;
    public AudioClip                RoundThree { get { return roundThree; } }

    [SerializeField]
    private AudioClip               roundFour =             null;
    public AudioClip                RoundFour { get { return roundFour; } }


    // Check volume
    [SerializeField]
    private AudioClip               jumpSound =             null;
    public AudioClip                JumpSound { get { return jumpSound; } }

    // Done
    [SerializeField]
    private AudioClip               organicImpact =         null;
    public AudioClip                OrganicImpact { get { return organicImpact; } }

    // Done
    [SerializeField]
    private AudioClip               organicShot =           null;
    public AudioClip                OrganicShot { get { return organicShot; } }

    // Check volume
    [SerializeField]
    private AudioClip               repairLoop =            null;
    public AudioClip                RepairLoop { get { return repairLoop; } }

    // Check volume
    [SerializeField]
    private AudioClip               warpSound =             null;
    public AudioClip                WarpSound { get { return warpSound; } }


    // Check volume
    [SerializeField]
    private AudioClip               trapSpawn =             null;
    public AudioClip                TrapSound { get { return trapSpawn; } }

    // Check volume
    [SerializeField]
    private AudioClip               turretInstall =         null;
    public AudioClip                TurretInstall { get { return turretInstall; } }

    // Check volume
    [SerializeField]
    private AudioClip               spikeDeath =            null;
    public AudioClip                SpikeDeath { get { return spikeDeath; } }

    //Check volume
    [SerializeField]
    private AudioClip               ballWhoosh =            null;
    public AudioClip                BallWhoosh { get { return ballWhoosh; } }

    // Done
    [SerializeField]
    private AudioClip               shieldAura =            null;
    public AudioClip                ShieldAura { get { return shieldAura; } }

    [SerializeField]
    private AudioClip               sawTrapSpawn =          null;
    public AudioClip                SawTrapSpawn { get { return sawTrapSpawn; } }

    [SerializeField]
    private AudioClip               sawTrapLoop =           null;
    public AudioClip                SawTrapLoop { get { return sawTrapLoop; } }


    /**********************
     ***   PROPERTIES   ***
     *********************/

    public int PlayerOneScore
    {
        get { return playerOneScore; }
    }

    public int PlayerTwoScore
    {
        get { return playerTwoScore; }
    }


    public GameObject PlayerOne { get { return playerOne; } }

    public GameObject PlayerTwo { get { return playerTwo; } }
    #endregion

    #region Memory & Coroutines
    /**********************
     ***   COROUTINES   ***
     *********************/




    /**********************
     *****   MEMORY   *****
     *********************/


    #endregion

    #region Methods

    #region Original Methods
    /*************************
     *******   SCORE   *******
     ************************/

    public void IncreaseScore(bool _isPlayerOne)
    {
        // Set players score
        if (_isPlayerOne) playerOneScore++;
        else playerTwoScore++;

        // Update UI
        UIManager.I?.UpdatePlayersScore(_isPlayerOne);
    }

    /*************************
     *******   SOUND   *******
     ************************/

    public void PlayVictory()
    {
        // Play victory
        musicSource.clip = victory;
        musicSource.time = 0;
        musicSource.Play();
    }

    public static void PlayClipAtPoint(AudioClip _clip, Vector2 _position, float _volume = 1)
    {
        if (_clip) AudioSource.PlayClipAtPoint(_clip, _position, _volume);
    }


    /*************************
     *******   LEVEL   *******
     ************************/

    public void LoadRandomLevel()
    {
        int _index = SceneManager.GetActiveScene().buildIndex;
        int _newIndex = _index;

        _newIndex = Random.Range(1, SceneManager.sceneCount);
        /*while (_newIndex == _index)
        {
            
        }*/

        SceneManager.LoadScene(_newIndex);
    }

    private void OnSceneLoaded(Scene _scene, LoadSceneMode _mode)
    {
        AudioClip _clip = null;

        if (_scene.buildIndex == 0)
        {
            _clip = tutorial;
            playerOneScore = 0;
            playerTwoScore = 0;
            playersReady.Clear();
        }
        else
        {
            int _higherScore = playerTwoScore > playerOneScore ? playerTwoScore : playerOneScore;
            switch (_higherScore)
            {
                case 0:
                    _clip = musicOne;
                    break;

                case 1:
                    _clip = musicTwo;
                    break;

                case 2:
                    _clip = musicThree;
                    break;

                default:
                    break;
            }
        }

        // Changed music if needed
        if (_clip != musicSource.clip)
        {
            musicSource.clip = _clip;
            musicSource.time = 0;
            musicSource.Play();
        }
    }

    public void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }

    public void ReloadTuto()
    {
        SceneManager.LoadScene(0);
    }

    public void SetGameReady(MyPlayercontroller _player)
    {
        if (playersReady.Contains(_player)) return;
        else
        {
            playersReady.Add(_player);
            if (playersReady.Count == 2) LoadRandomLevel();
        }
    }
    #endregion

    #region Unity Methods
    /*****************************
     *****   UNITY METHODS   *****
     ****************************/

    // Start is called before the first frame update
    private void Awake()
    {
        if (I)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(gameObject);

        SceneManager.sceneLoaded += OnSceneLoaded;
    }
    #endregion

    #endregion
}
