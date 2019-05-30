﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;

public class GameManager : MonoBehaviour
{

    public static GameManager instance = null;

    private AudioSource theAudio;

    private List<PlayerController> playerScript = new List<PlayerController>();

    [Header("Events")]
    private EventsManager eventsManager;
    public float timeSinceLastHazard;


    public Text player1ScoreText;
    public Text player2ScoreText;
    public Text player3ScoreText;
    public Text player4ScoreText;

    [Header("Check For Player Deaths")]
    public static bool onePlayerIsKilled;
    public bool playerOneDied;
    public bool playerTwoDied;

    [Header("GUI Variables")]
    public int killScoreToAdd;
    public int deathScoreToDeduct;

    [Header("Round End Variables")]
    public Image roundEndScreen;
    public bool roundHasEnded;
    public Text firstPlaceScore;
    public Text secondPlaceScore;
    public Text thirdPlaceScore;
    public Text fourthPlaceScore;

    //public Text player1WinText;
    //public Text player2WinText;
    //public Text player3WinText;
    //public Text player4WinText;
    //public Text roundEndWithDraw;

    [Header("Timer")]
    public float timeLeftInSeconds;
    public Text timerText;

    public Text commentaryText;

    public Image pauseScreen;
    public bool isPaused;

    [Header("Audio")]
    private AudioSource audioSource;
    public AudioClip bgm;

    // Awake is always called before any Start functions
    void Awake()
    {
        // Check if instance already exists;
        if (instance == null)
        {
            // If not, set instance to this.
            instance = this;
        }

        // If instance already exists and is not this;
        else if (instance != this)
        {
            // Then destroy this. (There can only be one instance of a GameManager).
            Destroy(gameObject);
        }

        // Don't destroy this object otherwise.
        //DontDestroyOnLoad(gameObject);


        eventsManager = FindObjectOfType<EventsManager>();
        theAudio = GetComponent<AudioSource>();
        PlayerController[] tempPCList = FindObjectsOfType<PlayerController>();

        foreach (PlayerController pc in tempPCList)
            playerScript.Add(pc);
        playerScript.Sort(delegate (PlayerController p1, PlayerController p2) { return p1.playerNumber.CompareTo(p2.playerNumber); });
    }

    // Start is called before the first frame update
    void Start()
    {
        //player1WinText.gameObject.SetActive(false);
        //player2WinText.gameObject.SetActive(false);
        //roundEndWithDraw.gameObject.SetActive(false);
        audioSource = GetComponent<AudioSource>();
        StartTimerCount();
        //commentaryText.text = (" ");
    }

    // Update is called once per frame
    void Update()
    {
        // Pause & Resume Game.
        if (Input.GetButtonDown("Start (All Controllers)"))
        {
            // Pause game.
            if (isPaused == false)
            {
                PauseGame();
                isPaused = true;
                return;
            }
            // Resume game.
            else
            {
                ResumeGame();
                isPaused = false;
                return;
            }
        }

        if (onePlayerIsKilled == true)
        {
        }

        // Setting players as alive after dying
        if (onePlayerIsKilled == true)
        {

            playerOneDied = false;
            playerTwoDied = false;
            onePlayerIsKilled = false;
        }
        if (Input.GetKeyDown(KeyCode.P))
        {
            SceneManager.LoadScene("Alpha Game");
        }

    }

    // Starts the count down of round time.
    public void StartTimerCount()
    {
        //timeLeftInSeconds = 180;
        timerText.text = ("Time Left: 0:00");
        InvokeRepeating("UpdateTimer", 0.0f, 0.01667f);

    }

    // Updates the timer every millisecond.
    public void UpdateTimer()
    {
        string minutes, seconds;

        if (timeLeftInSeconds > 0)
        {
            timeLeftInSeconds -= Time.deltaTime;
            minutes = Mathf.Floor(timeLeftInSeconds / 60).ToString("0");
            seconds = (timeLeftInSeconds % 60).ToString("00");
            timerText.text = "Time Left: " + minutes + ":" + seconds;
            timeSinceLastHazard += Time.deltaTime;
        }
        else
        {
            roundHasEnded = true;

            minutes = "00";
            seconds = "00";
            timerText.text = "Time Left: " + minutes + ":" + seconds;

            RoundEnd();
        }
    }

    public void UpdateScore()
    {
        player1ScoreText.text = playerScript[0].currentScore.ToString();
        player2ScoreText.text = playerScript[1].currentScore.ToString();
        player3ScoreText.text = playerScript[2].currentScore.ToString();
        //player2ScoreText.text = playerScript[3].currentScore.ToString();
    }

    public void PauseGame()
    {
        pauseScreen.gameObject.SetActive(true);
        Time.timeScale = 0;
    }

    public void ResumeGame()
    {
        Time.timeScale = 1;
        pauseScreen.gameObject.SetActive(false);
    }

    public void RoundEnd()
    {
        if (roundHasEnded == true)
        {
            //player1FinalScore.text = ("Player 1: " + playerScript[0].currentScore.ToString());
            //player2FinalScore.text = ("Player 2: " + playerScript[1].currentScore.ToString());
            //player3FinalScore.text = ("Player 3: " + playerScript[2].currentScore.ToString());
            //player4ScoreText.text = ("Player 4: " + playerScript[3].currentScore.ToString());


            playerScript.Sort(delegate (PlayerController p1, PlayerController p2) { return p1.currentScore.CompareTo(p2.currentScore); });
            playerScript.Reverse();

            roundEndScreen.gameObject.SetActive(true);

            firstPlaceScore.text = string.Format("Player {0}: {1}", playerScript[0].playerNumber, playerScript[0].currentScore);
            secondPlaceScore.text = string.Format("Player {0}: {1}", playerScript[1].playerNumber, playerScript[1].currentScore);
            thirdPlaceScore.text = string.Format("Player {0}: {1}", playerScript[2].playerNumber, playerScript[2].currentScore);
            //fourthPlaceScore.text = string.Format("Player {0}: {1}", playerScript[3].playerNumber, playerScript[3].currentScore);

            Time.timeScale = 0;
        }
    }

    public void LoadScene(string sceneName)
    {
        Time.timeScale = 1;
        SceneManager.LoadScene(sceneName);
    }
}