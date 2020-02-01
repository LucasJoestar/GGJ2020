using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

using Random = UnityEngine.Random;

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

    [SerializeField]
    private int                     playerOneScore =        0;

    [SerializeField]
    private int                     playerTwoScore =        0;


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
    /*********************************
     ***   MANAGER INSTANTIATION   ***
     ********************************/

    [RuntimeInitializeOnLoadMethod(RuntimeInitializeLoadType.AfterSceneLoad)]
    public static void CreateInstance()
    {
        I = new GameObject("Game Manager").AddComponent<GameManager>();
        DontDestroyOnLoad(I);
    }


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
     *******   LEVEL   *******
     ************************/

    public static void LoadRandomLevel()
    {
        int _index = SceneManager.GetActiveScene().buildIndex;
        int _newIndex = _index;

        while (_newIndex == _index)
        {
            _newIndex = Random.Range(1, SceneManager.sceneCount);
        }

        SceneManager.LoadScene(_newIndex);
    }

    public static void ReloadLevel()
    {
        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    #endregion

    #region Unity Methods
    /*****************************
     *****   UNITY METHODS   *****
     ****************************/

    // Start is called before the first frame update
    private void Start()
    {

    }
    #endregion

    #endregion
}
