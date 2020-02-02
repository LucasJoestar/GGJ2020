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

    private void OnLoadScene(Scene _scene, LoadSceneMode _mode)
    {
        scoreAnchor.SetActive(false);
        if (_scene.buildIndex == 0)
        {
            playerOneVictory.SetActive(false);
            playerTwoVictory.SetActive(false);

            foreach (Animator _score in playerOneScore)
            {
                _score.SetTrigger("Reset");
            }
            foreach (Animator _score in playerTwoScore)
            {
                _score.SetTrigger("Reset");
            }
        }
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

        if (_isPlayerOneVictory) playerOneScore[_score - 1].SetTrigger("Activate");
        else playerTwoScore[_score - 1].SetTrigger("Activate");

        yield return new WaitForSeconds(1.5f);

        // Call game end of player victory
        if (_score == playerOneScore.Length)
        {
            OnEndGame?.Invoke(_isPlayerOneVictory);

            if (_isPlayerOneVictory) playerOneVictory.SetActive(true);
            else playerTwoVictory.SetActive(true);

            yield return new WaitForSeconds(3);

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
            Destroy(this);
            return;
        }

        I = this;
        DontDestroyOnLoad(this);
    }

    // Start is called before the first frame update
    private void Start()
    {
        SceneManager.sceneLoaded += OnLoadScene;
    }
    #endregion

    #endregion
}
