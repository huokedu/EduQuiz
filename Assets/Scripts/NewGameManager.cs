﻿using UnityEngine;
using UnityEditor;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using System.Collections;
using System.Collections.Generic;

public class NewGameManager : MonoBehaviour {

	private Button answer1;
	private Button answer2;
	private Button answer3;
	private Button answer4;
	private Button hoveredButton;

	private Text questionText;

	private QuestionSetManager questionSetManager;
	private QuestionSet questionSet;

	private int questionIndex;
	private int timeDelay;

	private Image hoverImage;
	private float timeSinceLastCall;
	private bool hoverEngaged;
	private bool buttonPressed;
	private string correctAnswer;
	ColorBlock cb;

	// Use this for initialization
	void Start () {

		hoverImage = GameObject.Find("Image").GetComponent<Image>();

		answer1 = GameObject.Find("answer1_button").GetComponent<Button>();
		answer2 = GameObject.Find("answer2_button").GetComponent<Button>();
		answer3 = GameObject.Find("answer3_button").GetComponent<Button>();
		answer4 = GameObject.Find("answer4_button").GetComponent<Button>();

		questionText = GameObject.Find("question_text").GetComponent<Text>();

		// read settings from file
		string[] preferences = PreferencesManager.read();
		string sound = preferences[0];
		int font_size = int.Parse (preferences[1]);
		timeDelay = int.Parse (preferences[2]);

		// set font size
		answer1.GetComponentInChildren<Text> ().fontSize = font_size;
		answer2.GetComponentInChildren<Text> ().fontSize = font_size;
		answer3.GetComponentInChildren<Text> ().fontSize = font_size;
		answer4.GetComponentInChildren<Text> ().fontSize = font_size;
		questionText.GetComponentInChildren<Text> ().fontSize = font_size;

		questionSetManager = new QuestionSetManager ();
		questionSet = questionSetManager.importQuestions();

		questionIndex = -1;
		timeSinceLastCall = 0f;
		hoverEngaged = false;
		hoverImage.gameObject.SetActive(true);
		hoverImage.fillAmount = 0f;
		hoveredButton = null;
		buttonPressed = false;

		loadNewQuestion();
	}
	
	// Update is called once per frame
	void Update () {
		if (hoverEngaged && !buttonPressed) {
//			hoverImage.transform.position = Input.mousePosition;
			timeSinceLastCall += Time.deltaTime;
			if (timeSinceLastCall >= getDelayFraction(timeDelay)) {
				if (hoverImage.fillAmount + 0.01f > 1) {
					if (string.Compare(hoveredButton.GetComponentInChildren<Text>().text, correctAnswer) == 0) {
						EditorUtility.DisplayDialog("Informacija", "Tocan odgovor", "Nastavi");
					} else {
						EditorUtility.DisplayDialog("Informacija", "Netocan odgovor", "Nastavi");
					}

					buttonPressed = true;
					loadNewQuestion();
					hoverImage.fillAmount = 0;
				}
				hoverImage.fillAmount += 0.01f;
				timeSinceLastCall = 0;   // reset timer back to 0
			}
		}
	}

	private double getDelayFraction(int delay) {
		int time = 1;
		// cap at ~ 1 second
		if (delay > 1) {
			time = delay - 1;
		}
		return time/100.0;
	}

	private bool hasNextQuestion() {
		if (questionIndex + 1 <= questionSet.getQuestionList().Count - 1) {
			return true;
		}
		return false;
	}

	private Question getNextQuestion() {
		if (hasNextQuestion()) {
			questionIndex++;
			return questionSet.getQuestionList()[questionIndex];
		}

		return null;
	}

	public void loadNewQuestion() {

		Question question = getNextQuestion();
		if (question == null) {
			EditorUtility.DisplayDialog("Informacija", "Kraj igre", "Povratak u glavni izbornik");
			SceneManager.LoadScene("game_menu");
			return;
		}

		buttonPressed = false;

		List<Answer> answers = question.getAnswers();

		foreach (Answer answer in answers) {
			if (answer.isCorrect()) {
				correctAnswer = answer.getText();
			}
		}

		Question.QuestionType mode = question.getQuestionMode();
		questionText.text = question.getQuestionText();

		// button resizing and positioning based on the question mode
		if (mode == Question.QuestionType.MODE2_V) {
			// create button size
			Vector2 buttonSize = new Vector2 (430, 350);

			// set buttons 1 and 2 transformations and translations
			answer1.image.rectTransform.sizeDelta = buttonSize;
			answer1.transform.position = new Vector3(-215 + buttonSize.x, buttonSize.y/2, 0);

			answer2.image.rectTransform.sizeDelta = buttonSize;
			answer2.transform.position = new Vector3(215 + buttonSize.x, buttonSize.y/2, 0);

			// activate buttons 1 and 2
			answer1.gameObject.SetActive(true);
			answer2.gameObject.SetActive(true);
			answer3.gameObject.SetActive(false);
			answer4.gameObject.SetActive(false);

			// set answers text
			answer1.GetComponentInChildren<Text>().text = answers[0].getText();
			answer2.GetComponentInChildren<Text>().text = answers[1].getText();
		} else if (mode == Question.QuestionType.MODE2_H) {
			// create button size
			Vector2 buttonSize = new Vector2 (860, 175);

			// set buttons 1 and 3 transformations and translations
			answer1.image.rectTransform.sizeDelta = buttonSize;
			answer1.transform.position = new Vector3(buttonSize.x/2, 49.5f + buttonSize.y + 38, 0);

			answer3.image.rectTransform.sizeDelta = buttonSize;
			answer3.transform.position = new Vector3(buttonSize.x/2, -125.5f + buttonSize.y + 38, 0);

			// activate buttons 1 and 3
			answer1.gameObject.SetActive(true);
			answer3.gameObject.SetActive(true);
			answer2.gameObject.SetActive(false);
			answer4.gameObject.SetActive(false);

			// set answers text
			answer1.GetComponentInChildren<Text>().text = answers[0].getText();
			answer3.GetComponentInChildren<Text>().text = answers[1].getText();
		} else if (mode == Question.QuestionType.MODE4) {
			// create button size
			Vector2 buttonSize = new Vector2 (430, 175);

			// set all four buttons transformations and translations
			answer1.image.rectTransform.sizeDelta = buttonSize;
			answer1.transform.position = new Vector3(-215 + buttonSize.x, 49.5f + buttonSize.y + 38, 0);

			answer2.image.rectTransform.sizeDelta = buttonSize;
			answer2.transform.position = new Vector3(215 + buttonSize.x, 49.5f + buttonSize.y + 38, 0);

			answer3.image.rectTransform.sizeDelta = buttonSize;
			answer3.transform.position = new Vector3(-215 + buttonSize.x, -125.5f + buttonSize.y + 38, 0);

			answer4.image.rectTransform.sizeDelta = buttonSize;
			answer4.transform.position = new Vector3(215 + buttonSize.x, -125.5f + buttonSize.y + 38, 0);

			// activate all four buttons
			answer1.gameObject.SetActive(true);
			answer3.gameObject.SetActive(true);
			answer2.gameObject.SetActive(true);
			answer4.gameObject.SetActive(true);

			// set answers text
			answer1.GetComponentInChildren<Text>().text = answers[0].getText();
			answer2.GetComponentInChildren<Text>().text = answers[1].getText();
			answer3.GetComponentInChildren<Text>().text = answers[2].getText();
			answer4.GetComponentInChildren<Text>().text = answers[3].getText();
		}
	}

	public void onButtonEnter() {
		hoverEngaged = true;
	}

	public void onButtonExit() {
		hoverEngaged = false;
		timeSinceLastCall = 0;
		hoverImage.fillAmount = 0;
	}

	public void updateHoveredButton(Button button) {
		hoveredButton = button;
	}
}
