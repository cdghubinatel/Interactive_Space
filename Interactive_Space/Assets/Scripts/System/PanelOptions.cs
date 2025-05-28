using Meta.XR.ImmersiveDebugger.UserInterface.Generic;
using System;
using System.Collections;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.UIElements;
using Random = UnityEngine.Random;

public class PanelOptions : MonoBehaviour
{
    //Variables
    [Header("Components and GameObjects")]
    [SerializeField] private GameObject objectPlaceHolder;
    [SerializeField] private Animator objectAnimator;
    [SerializeField] private Text leftText;
    [SerializeField] private Text rightText;
    [SerializeField] private TMP_Text timerText;
    [SerializeField] private TMP_Text pointsText;
    [SerializeField] private TMP_Text startGameText;
    [SerializeField] private UnityEngine.UI.Button leftButton;
    [SerializeField] private UnityEngine.UI.Button rightButton;
    [SerializeField] public Color correctColor, wrongColor, defaultColor;

    [Header("Scripts")]
    [SerializeField] private GameManager gameManager;

    [Header("Panel Settings")]
    [HideInInspector] public bool leftPressed;
    [HideInInspector] public bool rightPressed;

    [Header("Generic")]
    QuizObject[] objects;
    string correctAnswer;

    private void Start()
    {
        gameManager = GetComponent<GameManager>();

        objects = gameManager.objects;

        leftPressed = false;
        rightPressed = false;
    }

    private void Update()
    {
        timerText.text = gameManager.roundTime.ToString("F0");
        pointsText.text = gameManager.points.ToString();

        if(gameManager.gameStarded)
            startGameText.enabled = false;
    }

    //Sorting panel every new object
    public void SortingPanel()
    {
        //Generating Objects and Answers
        int panelSide = Random.Range(0, 2);
        int selectedObject = Random.Range(0, objects.Length);
        int randomObject = Random.Range(0, objects.Length);
        objects[selectedObject].Object.SetActive(true);

        //Setting default button colors
        leftButton.image.color = defaultColor;
        rightButton.image.color = defaultColor;

        if (panelSide == 0) //Left button is correct
        {
            leftText.text = objects[selectedObject].Name;
            correctAnswer = leftText.text;
            objectPlaceHolder = objects[selectedObject].Object;
            rightText.text = objects[randomObject].Name;
        }
        else if (panelSide == 1) //Right button is correct
        {
            leftText.text = objects[randomObject].Name;
            objectPlaceHolder = objects[selectedObject].Object;
            rightText.text = objects[selectedObject].Name;
            correctAnswer = rightText.text;
        }
        else if (selectedObject == randomObject) SortingPanel();
        
        //Solving the problem that sometimes both answers are the same
        if (leftText.text == rightText.text) SortingPanel();
    }

    public void EndGamePanel() //When timer gets 0
    {
        startGameText.enabled = true;
        leftText.text = "";
        correctAnswer = null;
        objectPlaceHolder = null;
        rightText.text = "";
    }

    public bool CorrectOption()
    {
        if (leftText.text == correctAnswer && leftPressed)
        {
            leftButton.image.color = correctColor;
            return true;
        }
        else if (leftText.text != correctAnswer && leftPressed)
        {
            leftButton.image.color = wrongColor;
            return false;
        }

        else if (rightText.text == correctAnswer && rightPressed)
        {
            rightButton.image.color = correctColor;
            return true;
        }
        else
        {
            rightButton.image.color = wrongColor;
            return false;
        }
    }

    public void LeftButton()
    {
        leftPressed = true; //If press left panel button
    }

    public void RightButton()
    {
        rightPressed = true; //If press right panel button
    }
}
