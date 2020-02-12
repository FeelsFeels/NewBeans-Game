﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using System.Linq;

public class PlayerSelectBehaviour : MonoBehaviour
{

    public Animator animator;

    [Space]

    public PlayerInputInfo[] playerInfoArray = new PlayerInputInfo[4];      //Player Input scriptable object
    public CharacterData[] characterDataArray = new CharacterData[4];       //Specific character data scriptable objects
    public PlayerSelectCharacterSelector[] characterSelection = new PlayerSelectCharacterSelector[4];
    public Dictionary<PlayerSelectCharacterSelector, int> playerCharacterChoices = new Dictionary<PlayerSelectCharacterSelector, int>();

    public List<int> playersJoined = new List<int>();   //Player numbers

    bool canStartGame;

    public string SceneToLoad;


    private void Awake()
    {
        //Deselects all characters
        foreach (PlayerInputInfo player in playerInfoArray)
        {
            player.chosenCharacterData = null;
            player.chosenCharacterIndex = 0;
            player.forceActive = false;
        }
    }

    private void Update()
    {
        for (int i = 1; i <= 4; i++) //check, for up to 4 controllers,
        {
            if (playersJoined.Contains(i))
            {
                continue;
            }

            if (Input.GetButtonDown("AButton (Controller " + i + ")")) //if there's an A button input from one of the controllers
            {
                playersJoined.Add(i);
                AddNewPlayer(i); //add a new controller and join the game
                break;
            }
        }

        if (canStartGame)
        {
            if (Input.GetButtonDown("AButton (Controller " + playersJoined.First() + ")"))  //Take only player 1's input to start game
            {
                SceneManagement sceneManagement = FindObjectOfType<SceneManagement>();
                if (sceneManagement)
                {
                    sceneManagement.LoadSceneWithLoadingScreen(SceneToLoad);
                }
                else
                    SceneManager.LoadScene(SceneToLoad);
            }
        }
    }

    void AddNewPlayer(int controllerNumber)
    {
        int latestPlayer = playersJoined.Count - 1;    //Making controller number and playernumber unrelated.

        //pressToJoinSprites[latestPlayer].enabled = false;

        characterSelection[latestPlayer].PlayerJoined(playerInfoArray[latestPlayer]);
        playerInfoArray[latestPlayer].SetInputStrings(controllerNumber);
        playerInfoArray[latestPlayer].chosenCharacterIndex = 0; //Setting as default character selection
        CheckIfCanStartGame();
    }

    void RemovePlayer(int controllerNummber)
    {

    }

    public bool ChooseCharacter(PlayerInputInfo playerToChange, int characterIndex)
    {

        CheckIfCanStartGame();
        return true;
    }

    public void UnchooseCharacter(PlayerInputInfo playerToChange, int characterIndex)
    {


        CheckIfCanStartGame();
    }

    public bool CheckIfCharacterTaken(PlayerInputInfo playerToCheck, int characterIndex)
    {
        //Check if character was already chosen
        bool characterTaken = false;

        if (playersJoined.Count < 2)
            return true;

        for (int i = 0; i < playersJoined.Count; i++)
        {
            //This player has not picked a character. Skip his ass
            if (playerInfoArray[i].chosenCharacterData == null)
                continue;

            int chosenCharIndex = playerInfoArray[i].chosenCharacterIndex;
            if (chosenCharIndex == characterIndex)
            {
                characterTaken = true;
                break;
            }
            else
            {
                characterTaken = false;
                continue;
            }
        }

        if (characterTaken)
            return false;
        else
            return true;
    }

    void CheckIfCanStartGame()
    {

        //Must be more than one player
        if (playersJoined.Count < 2)
            return;

        List<int> chosenCharacterIndexes = new List<int>();

        //If two people chose the same character, cannot start game
        for (int i = 0; i < playersJoined.Count; i++)
        {
            if (playerInfoArray[i].chosenCharacterData == null)
            {
                //A player has not chosen a character. Cannot start game.
                canStartGame = false;
                animator.SetBool("ReadyToStart", canStartGame);
                return;
            }

            //Checking if two people chose the same character
            int chosenChar = playerInfoArray[i].chosenCharacterIndex;
            if (chosenCharacterIndexes.Contains(chosenChar))
            {
                canStartGame = false;
                animator.SetBool("ReadyToStart", canStartGame);
                return;
            }
            else
            {
                chosenCharacterIndexes.Add(chosenChar);
            }
        }

        canStartGame = true;

        animator.SetBool("ReadyToStart", canStartGame);

    }
}