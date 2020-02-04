using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

#pragma warning disable
public class UIManager : MonoBehaviour
{
    #region Events
    public static event Action<bool>    OnEndGame =             null;
    #endregion

    #region Fields / Properties
    /*********************
     ***   CONSTANTS   ***
     ********************/


    /**********************
     *****   STATIC   *****
     *********************/

    public static UIManager             I =                     null;


    /**********************
     *****   FIELDS   *****
     *********************/

    [SerializeField]
    private GameObject                  scoreAnchor =           null;


    [SerializeField]
    private GameObject                  playerOneVictory =      null;

    [SerializeField]
    private GameObject                  playerTwoVictory =      null;


    [SerializeField]
    private Animator[]                  playerOneScore =        new Animator[] { };

    [SerializeField]
    private Animator[]                  playerTwoScore =        new Animator[] { };


    [SerializeField]
    private Image                       p1RepairGauge =         null;

    [SerializeField]
    private Image                       p2RepairGauge =         null;


    [SerializeField]
    private Animator                    animator =     null;


    /**********************
     ***   PROPERTIES   ***
     *********************/


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
     *****   ANIMATION   *****
     ************************/

    private void PlayStartAnim()
    {
        if (!animator || (SceneManager.GetActiveScene().buildIndex == 0)) return;

        LevelManager.I.PlayerOne.IsPlayable = false;
        LevelManager.I.PlayerTwo.IsPlayable = false;
        animator.SetTrigger("Play");
    }

    public void PlayStartAnimSound()
    {
        int _totalScore = GameManager.I.PlayerOneScore + GameManager.I.PlayerTwoScore;
        AudioClip _clip = null;

        switch (_totalScore)
        {
            case 0:
                _clip = GameManager.I.RoundOne;
                break;

            case 1:
                _clip = GameManager.I.RoundTwo;
                break;

            case 2:
                _clip = GameManager.I.RoundThree;
                break;

            case 3:
                _clip = GameManager.I.RoundFour;
                break;

            case 4:
                _clip = GameManager.I.FinalRound;
                break;

            default:
                break;
        }

        if (_clip != null) GameManager.PlayClipAtPoint(_clip, Camera.main.transform.position);
    }

    public void EndStartAnim()
    {
        LevelManager.I.PlayerOne.IsPlayable = true;
        LevelManager.I.PlayerTwo.IsPlayable = true;
    }


    /**************************
     *******   REPAIR   *******
     *************************/

    public void ActiveRepair(bool _isPlayerOne, Vector2 _position)
    {
        Image _gauge = _isPlayerOne ? p1RepairGauge : p2RepairGauge;

        _gauge.transform.position = _position;
        _gauge.gameObject.SetActive(true);
    }

    private void OnLoadScene(Scene _scene, LoadSceneMode _mode)
    {
        scoreAnchor.SetActive(false);
        p1RepairGauge.gameObject.SetActive(false);
        p2RepairGauge.gameObject.SetActive(false);

        if (_scene.buildIndex == 0)
        {
            playerOneVictory.SetActive(false);
            playerTwoVictory.SetActive(false);
        }
    }

    public void SetReppairPercent(bool _isPlayerOne, float _percent)
    {
        Image _gauge = _isPlayerOne ? p1RepairGauge : p2RepairGauge;

        if (_percent == 1)
        {
            _gauge.gameObject.SetActive(false);
            return;
        }

        _gauge.fillAmount = _percent;
    }


    /*************************
     *******   SCORE   *******
     ************************/

    private IEnumerator DoUpdatePlayersSore(bool _isPlayerOneVictory)
    {
        yield return new WaitForSeconds(1);

        scoreAnchor.SetActive(true);

        yield return new WaitForSeconds(.5f);

        GameManager.PlayClipAtPoint(GameManager.I?.Scoresound, Camera.main.transform.position);
        int _score = _isPlayerOneVictory ? GameManager.I.PlayerOneScore : GameManager.I.PlayerTwoScore;

        if (_isPlayerOneVictory) playerOneScore[_score - 1].SetTrigger("Activate");
        else playerTwoScore[_score - 1].SetTrigger("Activate");

        if (_score == playerOneScore.Length)
        {
            GameManager.PlayClipAtPoint(_isPlayerOneVictory ? GameManager.I?.PlayerOneWin : GameManager.I?.PlayerTwoWin, Camera.main.transform.position);
        }

        yield return new WaitForSeconds(1.5f);

        // Call game end of player victory
        if (_score == playerOneScore.Length)
        {
            OnEndGame?.Invoke(_isPlayerOneVictory);

            if (_isPlayerOneVictory) playerOneVictory.SetActive(true);
            else playerTwoVictory.SetActive(true);

            foreach (Animator _scoreAnim in playerOneScore)
            {
                _scoreAnim.SetTrigger("Reset");
            }
            foreach (Animator _scoreAnim in playerTwoScore)
            {
                _scoreAnim.SetTrigger("Reset");
            }

            GameManager.I?.PlayVictory();
            yield return new WaitForSeconds(GameManager.I.Victory.length + 1);

            GameManager.I?.ReloadTuto();
        }
        else GameManager.I?.LoadRandomLevel();
    }

    public void UpdatePlayersScore(bool _isPlayerOneVictory) => StartCoroutine(DoUpdatePlayersSore(_isPlayerOneVictory));
    #endregion

    #region Unity Methods
    /*****************************
     *****   UNITY METHODS   *****
     ****************************/

    private void Awake()
    {
        if (I)
        {
            Destroy(gameObject);
            return;
        }

        I = this;
        DontDestroyOnLoad(this);
        LevelManager.OnSpawnPlayers += PlayStartAnim;
    }

    // Start is called before the first frame update
    private void Start()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }
    #endregion

    #endregion
}
