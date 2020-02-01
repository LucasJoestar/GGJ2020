using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

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
    private Sprite                      ScorePointSprite =      null;

    [SerializeField]
    private Sprite                      ScoreEmptySprite =      null;



    [SerializeField]
    private GameObject                  scoreAnchor =           null;

    [SerializeField]
    private Image[]                     playerOneScore =        new Image[] { };

    [SerializeField]
    private Image[]                     playerTwoScore =        new Image[] { };


    [SerializeField]
    private RectTransform               repairAnchor =          null;

    [SerializeField]
    private Image                       repairGauge =           null;


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
    /**************************
     *******   REPAIR   *******
     *************************/

    public void ActiveRepair(Vector2 _position)
    {
        repairAnchor.position = _position;
        repairAnchor.gameObject.SetActive(true);
    }

    public void SetReppairPercent(float _percent)
    {
        if (_percent == 1)
        {
            repairAnchor.gameObject.SetActive(false);
            return;
        }

        repairGauge.fillAmount = _percent;
    }


    /*************************
     *******   SCORE   *******
     ************************/

    private IEnumerator DoUpdatePlayersSore(bool _isPlayerOneVictory)
    {
        yield return new WaitForSeconds(1);

        scoreAnchor.SetActive(true);

        yield return new WaitForSeconds(.5f);

        int _score = _isPlayerOneVictory ? GameManager.I.PlayerOneScore : GameManager.I.PlayerTwoScore;

        if (_isPlayerOneVictory) playerOneScore[_score - 1].sprite = ScorePointSprite;
        else playerTwoScore[_score - 1].sprite = ScorePointSprite;

        yield return new WaitForSeconds(1);

        // Call game end of player victory
        if (_score == playerOneScore.Length) OnEndGame?.Invoke(_isPlayerOneVictory);
        GameManager.LoadRandomLevel();
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
            Destroy(this);
            return;
        }

        I = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        SceneManager.sceneLoaded += (Scene _scene, LoadSceneMode _loadMode) => scoreAnchor.SetActive(false);
    }
    #endregion

    #endregion
}
