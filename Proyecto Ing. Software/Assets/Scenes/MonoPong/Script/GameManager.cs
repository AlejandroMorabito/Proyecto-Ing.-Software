/*
This Script manages the states of the game
*/

using UnityEngine;
using UnityEngine.UI;
using System;
using System.Collections;
using System.Collections.Generic;

public class GameManager : MonoBehaviour {

    /*<summary>Current number of lives<summary>*/
    public int Lives = 1;

    /*<summary>number of lives when starting a new game<summary>*/
    public int DefaultLives = 1;

    /*<summary>Ball Prefab<summary>*/
    public GameObject Ball;

    /*<summary>List of the Rules the game can follow<summary>*/
    public enum GameTypes 
    {
        NoResizing,
        ResizeOverTime,
        PlusMinusBalls,
    }

    /*<summary>the currect rule the game is following<summary>*/
    public GameTypes GameType = GameTypes.ResizeOverTime;

    /*<summary>Variables for CollisionResize GameType<summary>*/
    public ResizeOverTimeVars resizeOverTimeVars;

    /*<summary>Variables for PlusMinusBalls GameType<summary>*/
    public PlusMinusBallsVars plusMinusBallsVars;

    [System.Serializable]
    public struct ResizeOverTimeVars
    {

        /*<summary>the curve that will be resizing by<summary>*/
        public AnimationCurve ResizeCurve;

        /*<summary>MinusBall Prefab<summary>*/
        [HideInInspector]
        public float StartTime;

        /*<summary>How many seconds it will take to play through the entire ResizeCurve<summary>*/
        public float SecondsToZero;

        /*<summary>whether or not we'll reset the size after each Life<summary>*/
        public bool ResetAfterEachLife;

    }

    [System.Serializable]
    public struct PlusMinusBallsVars
    {

        /*<summary>MinusBall Prefab<summary>*/
        public GameObject MinusBall;

        /*<summary>PlusBall Prefab<summary>*/
        public GameObject PlusBall;

        /*<summary>the Last Time a Minus/Plus Ball was created<summary>*/
        [HideInInspector]
        public float LastAppearTime;

        /*<summary>how often a Minus/Plus Ball should be created<summary>*/
        public float MPBallAppearRate;

        /*<summary>how much the MPBallAppearRate should change over time<summary>*/
        public float MPBallAppearFlux;
    }

    /*<summary>the RacketController<summary>*/
    private RacketController racketController;

    /*<summary>Canvas Animator<summary>*/
    private Animator CanvasAnim;

    /*<summary>Best Score Text<summary>*/
    private Text BestScore;

    /*<summary>Lives Count Text<summary>*/
    private Text LivesCount;

    /*<summary>The point where new balls will be created<summary>*/
    private Transform SpawnPoint;

    /*<summary>The score script to check the score<summary>*/
    private ScoreScript scoreScript; 

    /*<summary>Possible Game States<summary>*/
    public enum GameStates
    {
        PLAYING,
        DEATH,
        RESTART,
        IDLE,
    }

    /*<summary>the current Game State<summary>*/
    public GameStates State = GameStates.IDLE;

    /*<summary>Maps the methods to the states<summary>*/
    protected Dictionary<GameStates, Action> fms = new Dictionary<GameStates, Action>();

    void Awake()
    {
        //set the default number of Lives
        Lives = DefaultLives;

        //assign values to variables
        SpawnPoint = GameObject.Find("BallSpawnPoint").transform;
        CanvasAnim = GameObject.Find("Canvas").GetComponent<Animator>();
        BestScore = GameObject.Find("BestScore").GetComponent<Text>();
        scoreScript = GameObject.FindObjectOfType<ScoreScript>();
        LivesCount = GameObject.Find("LivesCount").GetComponent<Text>();
        racketController =  GameObject.FindObjectOfType<RacketController>();

        //maps the States to the methods
        fms.Add (GameStates.PLAYING,Playing);
        fms.Add (GameStates.DEATH,Death);
        fms.Add (GameStates.RESTART,Restart);
        fms.Add (GameStates.IDLE,Idle);

        //set the best score text
        if (!PlayerPrefs.HasKey("BestScore"))
        {
            PlayerPrefs.SetInt("BestScore",0);
            BestScore.text = "Best: 0";
        }
        else
        {
            BestScore.text = "Best: " + PlayerPrefs.GetInt("BestScore").ToString();
        }

        if (DefaultLives == 1)
        {
            LivesCount.enabled = false;
        }

    }

    //this method is used for setting states
    public void SetState(GameStates NextState)
    {
        State =  NextState;

        if (State == GameStates.PLAYING)
        {
            GameObject NewBall = GameObject.Instantiate(Ball,SpawnPoint.position,SpawnPoint.rotation) as GameObject;
            CanvasAnim.SetInteger("GameStatusNum",1);

            plusMinusBallsVars.LastAppearTime = Time.time + 3f;

            if (resizeOverTimeVars.ResetAfterEachLife)
            {
                resizeOverTimeVars.StartTime = Time.time;
            }


            if (Lives == 0)
            {
                Lives = DefaultLives;
                UpdateLives();
                scoreScript.ResetScore();
                resizeOverTimeVars.StartTime = Time.time;
                racketController.ResetSize();
            }
            else
            {
                
            }

        }

        if (State == GameStates.IDLE)
        {
            CanvasAnim.SetInteger("GameStatusNum",0);


            if (scoreScript.Score > PlayerPrefs.GetInt("BestScore"))
            {
                PlayerPrefs.SetInt("BestScore",scoreScript.Score);
                BestScore.text = "Best: " + PlayerPrefs.GetInt("BestScore").ToString();
            }
        }

        PlayerPrefs.Save();
    }

    void Update () 
    {
        fms[State].Invoke();
    }

    //this method will be executed during play
    private void Playing()
    {
        if (GameType == GameTypes.ResizeOverTime)
        {
            racketController.Size = racketController.MaxSize * (resizeOverTimeVars.ResizeCurve.Evaluate( (Time.time - resizeOverTimeVars.StartTime) / resizeOverTimeVars.SecondsToZero) );
        }
        else if (GameType == GameTypes.PlusMinusBalls)
        {
            if (Time.time - plusMinusBallsVars.LastAppearTime > plusMinusBallsVars.MPBallAppearRate)
            {
                CreateResizeBall();

                plusMinusBallsVars.MPBallAppearRate += UnityEngine.Random.Range(- (plusMinusBallsVars.MPBallAppearFlux/2f) ,plusMinusBallsVars.MPBallAppearFlux);
                plusMinusBallsVars.MPBallAppearRate = Mathf.Clamp(plusMinusBallsVars.MPBallAppearRate,0.5f,30f); //claming MPBallAppearRate
                plusMinusBallsVars.LastAppearTime = Time.time;
            }
        } 
        else if (GameType == GameTypes.NoResizing)
        {
            //nothing here
        }

    }

    //this method will be executed during Death
    private void Death()
    {
        Lives -= 1;
        UpdateLives();

        if (Lives == 0)
        {
            SetState(GameStates.IDLE);
        }
        else
        {   
            SetState(GameStates.RESTART);
        }

        ReSizeBall[] ReSizeBalls =  GameObject.FindObjectsOfType<ReSizeBall>();

        for (int i = 0; i < ReSizeBalls.Length; i++)
        {
            ReSizeBalls[i].RemoveMe();
        }

    }

    //this method will be executed during Restaring
    private void Restart()
    {
        SetState(GameStates.PLAYING);
    }

    //this method will be executed during Idle
    private void Idle()
    {

    }

    //update the Lives Text to show the correct number of lives
    private void UpdateLives()
    {
        LivesCount.text = "Lives: " + Lives.ToString();
    }

    //create a Plus or Minus Ball here
    private void CreateResizeBall()
    {
        //we'll decide based on a random number (2 out of 3 will be minus)
        float BallType = UnityEngine.Random.Range(-10f,5f);

        if (BallType > 0 && !racketController.IsMaxSize)
        {
            CreatePlusBall();
        }
        else
        {
            CreateMinusBall();
        }
    }

    //creates a Minus ball
    private void CreateMinusBall()
    {
        GameObject NewBall = GameObject.Instantiate(plusMinusBallsVars.MinusBall,SpawnPoint.position,SpawnPoint.rotation) as GameObject;
        NewBall.GetComponent<ReSizeBall>().Launch();

//        print("MinusBall at " + Time.time.ToString());
    }

    //creates a Plus ball
    private void CreatePlusBall()
    {
        GameObject NewBall = GameObject.Instantiate(plusMinusBallsVars.PlusBall,SpawnPoint.position,SpawnPoint.rotation) as GameObject;
        NewBall.GetComponent<ReSizeBall>().Launch();

//        print("PlusBall at " + Time.time.ToString());
    }

        
}
