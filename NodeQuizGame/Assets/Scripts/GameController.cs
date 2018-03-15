using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class GameController : MonoBehaviour {

    public GameObject questionDisplay;
    public GameObject endGameDisplay;
    public Text questionText;
    public Text scoreText;
    public Text timeText;
    public int playerScore;
    public Transform answerButtonParent;
    public BasicObjectPool answerButtonPool;
    DataController dataController;
    RoundData roundData;
    QuestionData[] questionPool;
    bool isRoundActive;
    int questionIndex;
    float timeRemaining;

    List<GameObject> answerButtonObjects = new List<GameObject>();

	// Use this for initialization
	void Start ()
    {
        dataController = FindObjectOfType<DataController>();
        roundData = dataController.GetCurrentRoundData();
        questionPool = roundData.questions;
        playerScore = 0;
        questionIndex = 0;
        timeRemaining = roundData.timeLimitInSeconds;
        UpdateTime();

        isRoundActive = true;
        ShowQuestions();
	}

    void ShowQuestions()
    {
        QuestionData questionData = questionPool[questionIndex];
        questionText.text = questionData.questionText;

        for (int i = 0; i < questionData.answers.Length; i++) {
            GameObject answerButtonObject = answerButtonPool.GetObject();
            answerButtonObject.transform.SetParent(answerButtonParent);
            answerButtonObjects.Add(answerButtonObject);

            AnswerButton answerButton = answerButtonObject.GetComponent<AnswerButton>();
            answerButton.Setup(questionData.answers[i]);
        }


    }

    private void RemoveAnswerButtons()
    {
        while (answerButtonObjects.Count > 0)
        {
            answerButtonPool.ReturnObject(answerButtonObjects[0]);
            answerButtonObjects.RemoveAt(0);
        }
    }

    public void AnswerClicked(bool isCorrect)
    {
        if(isCorrect) {
            playerScore += roundData.pointsAddedForCorrectAnswer;
            scoreText.text = "Score: " + playerScore.ToString();
        }
        if( questionPool.Length > questionIndex + 1 ) {
            questionIndex++;
            RemoveAnswerButtons();
            ShowQuestions();
        } else {
            EndRound();
        }
    }

    public void EndRound()
    {
        isRoundActive = false;
        questionDisplay.SetActive(false);
        endGameDisplay.SetActive(true);
    }
    
    public void PlayAgain()
    {
        isRoundActive = true;
        questionDisplay.SetActive(true);
        endGameDisplay.SetActive(false);
        questionIndex = 0;
        playerScore = 0;
        scoreText.text = "Score: " + playerScore.ToString();
        timeRemaining = roundData.timeLimitInSeconds;
        UpdateTime();

        RemoveAnswerButtons();
        ShowQuestions();
    }
    
	void UpdateTime ()
    {
        timeText.text = "Time: " + Mathf.Round(timeRemaining).ToString();
	}

    void Update()
    {
        if( isRoundActive ) {
            timeRemaining -= Time.deltaTime;
            UpdateTime();
            if( timeRemaining <= 0 ) {
                EndRound();
            }
        }
    }
}
