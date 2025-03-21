using System;
using System.Collections;
using System.Collections.Generic;
using NUnit.Framework;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Experimental.AI;
using UnityEngine.UI;
using static UnityEngine.Rendering.DebugUI;

public class GameManager : MonoBehaviour
{
    public int[] cards;

    public List<int> playerCards = new List<int>();
    public List<int> opponentCards = new List<int>();
    public List<GameObject> savedCardUIA = new List<GameObject>();
    public List<GameObject> savedCardUIB = new List<GameObject>();
    public List<string> powerCards = new List<string>();

    public int selectedPlayerCard = -1;
    public int selectedOpponentCard = -1;
    public int selectedIndexA = -1;
    public int selectedIndexB = -1;
    public int moveCount = 0;
    public int multiplier = 1;
    public int scoreRequired;

    public PowerCardSystem powerCardSystem;
    public ScoreManager scoreManager;

    public float score = 0;

    public bool matchup = false;
    public bool levelShift = false;

    public GameObject[] PlayerCardPrefabs;
    public GameObject[] OpponentCardPrefabs;

    public GameObject startUI;
    public GameObject shuffleUI;
    public GameObject playerSCard;

    public Transform PlayerCardSpawner;
    public Transform OpponentCardSpawner;
    public Transform playerCardPos;

    public Vector3 cardPos;
        void Start()
    {
        cards = new int[13];
        cards[0] = 1; cards[1] = 2; cards[2] = 3; cards[3] = 4; cards[4] = 5; cards[5] = 6; cards[6] = 7; cards[7] = 8; cards[8] = 9; cards[9] = 10; cards[10] = 11; cards[11] = 12; cards[12] = 13;
        scoreRequired = 1;
    }

    void Update()
    {
        CardSelect();
        CardCondition();
        CardDiscard();

        if (moveCount == 0 || score >= scoreRequired) GameOver();
    }
    public void CardSetup()
    {

        if (playerCards.Count <= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, cards.Length);
                int randomName = cards[randomIndex];
                Vector3 localCardPosition = PlayerCardSpawner.position + new Vector3(i * 1, 0, 0);
                playerCards.Add(randomName);
                GameObject instantiatedCards = Instantiate(PlayerCardPrefabs[randomName - 1], localCardPosition, Quaternion.identity);
                savedCardUIA.Add(instantiatedCards);
            }

            for (int i = 0; i < 3; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, cards.Length);
                int randomName = cards[randomIndex];
                Vector3 localCardPosition = OpponentCardSpawner.position + new Vector3(i * 2, 0, 0);
                opponentCards.Add(randomName);
                savedCardUIB.Add(Instantiate(OpponentCardPrefabs[randomName - 1], localCardPosition, Quaternion.identity));
            }

            Debug.Log("Player :" + string.Join(", ", playerCards));
            Debug.Log("Opponent :" + string.Join(", ", opponentCards));
        }

        RandomPlayerCard();
        score = 0;
        startUI.SetActive(false);
        shuffleUI.SetActive(true);
    }
    void RandomPlayerCard()
    {
        selectedIndexA = UnityEngine.Random.Range(0, playerCards.Count);
        selectedPlayerCard = playerCards[selectedIndexA];
        playerSCard = Instantiate(PlayerCardPrefabs[selectedPlayerCard - 1], new Vector3(playerCardPos.transform.position.x, playerCardPos.transform.position.y, playerCardPos.transform.position.z), Quaternion.identity);
        Debug.Log("Player Selected Card : " + selectedPlayerCard);
        cardPos = savedCardUIA[selectedIndexA].transform.position;
    }
    void CardSelect()
    {
        if (opponentCards.Count > 0)
        {
            if (Input.GetKeyDown(KeyCode.F5)) { selectedOpponentCard = opponentCards[0]; selectedIndexB = 0; moveCount--; }
            if (Input.GetKeyDown(KeyCode.F6)) { selectedOpponentCard = opponentCards[1]; selectedIndexB = 1; moveCount--; }
            if (Input.GetKeyDown(KeyCode.F7)) { selectedOpponentCard = opponentCards[2]; selectedIndexB = 2; moveCount--; }

        }
        if (selectedPlayerCard != -1 && selectedOpponentCard != -1) { Debug.Log("opponent selected card : " + selectedOpponentCard); matchup = true; }
    }
    void CardCondition()
    {

        if (selectedPlayerCard > selectedOpponentCard && matchup)
        {
            powerCardSystem.IcarisCard();
            powerCardSystem.LokiCard();
            ScoringSystem(selectedPlayerCard, selectedOpponentCard);
            Debug.Log("Player Won!!");
            RoundReset();
        }
        if (selectedPlayerCard < selectedOpponentCard && matchup)
        {
            Debug.Log("Player Lost!!");
            RoundReset();
        }
        if (selectedPlayerCard == selectedOpponentCard && matchup)
        {
            Debug.Log("Draw Clash Again !!!");
            RoundReset();
        }
    }

    public void PlayerCardShuffler()
    {
        playerCards.Clear();
        foreach (GameObject obj in savedCardUIA)
        {
            Destroy(obj);
        }
        savedCardUIA.Clear();
        cardPos.Set(0, 0, 0);
        selectedPlayerCard = -1;
        selectedIndexA = -1;

        if (playerCards.Count <= 0)
        {
            for (int i = 0; i < 3; i++)
            {
                int randomIndex = UnityEngine.Random.Range(0, cards.Length);
                int randomName = cards[randomIndex];
                Vector3 localCardPosition = PlayerCardSpawner.position + new Vector3(i * 1, 0, 0);
                playerCards.Add(randomName);
                savedCardUIA.Add(Instantiate(PlayerCardPrefabs[randomName - 1], localCardPosition, Quaternion.identity));
            }
        }
        moveCount--;

        Debug.Log("Player :" + string.Join(", ", playerCards));
        Debug.Log("Opponent :" + string.Join(", ", opponentCards));
        Destroy(playerSCard);
        RandomPlayerCard();
    }
    void RoundReset()
    {
        if (playerCards.Count != 0 && opponentCards.Count != 0)
        {
            matchup = false;

            playerCards.RemoveAt(selectedIndexA);
            opponentCards.RemoveAt(selectedIndexB);
            CardChangeOver(playerCards, opponentCards);

            selectedPlayerCard = -1;
            selectedOpponentCard = -1;

            selectedIndexA = -1;
            selectedIndexB = -1;

            multiplier = 1;

            CardShuffle(opponentCards);


            Debug.Log("Player :" + string.Join(", ", playerCards));
            Debug.Log("Opponent :" + string.Join(", ", opponentCards));
            Destroy(playerSCard);
            RandomPlayerCard();
        }
        else
        {
            Debug.Log("List in empty");
        }

    }
    void CardShuffle(List<int> list)
    {
        System.Random rng = new System.Random();
        int n = list.Count;
        while (n > 1)
        {
            n--; 
            int k = rng.Next(n + 1);
            int value = list[k];
            list[k] = list[n];
            list[n] = value;
        }
    }
    void CardChangeOver(List<int> playerCard, List<int> opponentCard)
    {
        int randomIndex = UnityEngine.Random.Range(0, cards.Length);
        int randomName = cards[randomIndex];
        Destroy(savedCardUIA[selectedIndexA]);
        savedCardUIA.RemoveAt(selectedIndexA);
        playerCard.Insert(selectedIndexA, randomName);
        GameObject instantiatedCards = Instantiate(PlayerCardPrefabs[randomName - 1], cardPos, Quaternion.identity);
        savedCardUIA.Add(instantiatedCards);

        randomIndex =   UnityEngine.Random.Range(0, cards.Length);
        randomName = cards[randomIndex];
        opponentCard.Add(randomName);
    }
    void ScoringSystem(int ValueA, int ValueB)
    {
        if (score == 0 && matchup)
        {
            score = MathF.Abs(ValueA - ValueB) * multiplier;
        }
        else
        {
            score = (MathF.Abs(ValueA - ValueB) * multiplier) + score;
        }
        Debug.Log("Player Score: " + score);

    }
    void GameOver()
    {
        playerCards.Clear();
        opponentCards.Clear();

        cardPos.Set(0,0,0);
        selectedPlayerCard = -1;
        selectedOpponentCard = -1;

        selectedIndexA = -1;
        selectedIndexB = -1;

        startUI.SetActive(true);
        foreach (GameObject obj in savedCardUIA)
        {
            Destroy(obj);
        }
        foreach (GameObject obj in savedCardUIB)
        {
            Destroy(obj);
        }
        Destroy(playerSCard);
        savedCardUIA.Clear();
        savedCardUIB.Clear();
        levelShift = true;
        LevelSet();
        moveCount = 5;
        shuffleUI.SetActive(false);
        Debug.Log("gameover");

    }
    void LevelSet()
    {
        if(scoreManager.selectedLevelID == "1" && levelShift)
        {
            levelShift = false;
            score = 0;
            scoreManager.selectedLevelID = "2";
            scoreRequired = scoreManager.levelID[scoreManager.selectedLevelID];
        }
        if (scoreManager.selectedLevelID == "2" && levelShift)
        {
            levelShift = false;
            score = 0;
            scoreManager.selectedLevelID = "3";
            scoreRequired = scoreManager.levelID[scoreManager.selectedLevelID];
        }
        if (scoreManager.selectedLevelID == "3" && levelShift)
        {
            levelShift = false;
            score = 0;
            scoreManager.selectedLevelID = "4";
            scoreRequired = scoreManager.levelID[scoreManager.selectedLevelID];
        }
        else
        {
            levelShift = false;
            Debug.Log("Won");
        }
    }
    void CardDiscard()
    {
        if (playerCards.Count > 0 && opponentCards.Count > 0 && Input.GetKeyDown(KeyCode.Space)) GameOver();
    }
}
