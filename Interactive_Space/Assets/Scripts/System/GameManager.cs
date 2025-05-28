using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class GameManager : MonoBehaviour
{
    //Variables
    [Header("Game System Variables")]
    [SerializeField] private float timeToNextObject;
    [SerializeField] private float timeChangingObject;
    [SerializeField] public float roundTime;
    [SerializeField] public int points;
    [SerializeField] public QuizObject[] objects;
    [HideInInspector] public int answers;
    [HideInInspector] private float totalTime;
    [HideInInspector] public bool gameStarded;
    [SerializeField] private AudioSource audioSource;

    [Header("Scoring System")]
    [SerializeField] private float maxTimeForBonus = 3f; // Tempo máximo para ganhar bônus (reduzido de 5 para 3)
    [SerializeField] private int basePoints = 5; // Pontos base por resposta correta (reduzido de 10 para 5)
    [SerializeField] private int maxBonusPoints = 20; // Pontos máximos de bônus (reduzido de 50 para 20)

    [Header("Components and GameObjects")]
    [SerializeField] private GameObject objectPlaceHolder;
    [SerializeField] private Animator objectAnimator;
    [SerializeField] private VirtualKeyboard virtualKeyboard;

    [Header("Scripts")]
    [SerializeField] protected PanelOptions panelOptions;

    //Tick Control
    public class OnTickEventArgs : EventArgs
    {
        public int tick;
    }
    public static event EventHandler<OnTickEventArgs> OnTick;
    private const float TICK_TIMER_MAX = 5f;
    private int tick;
    private float tickTimer;

    private float questionStartTime; // Tempo quando a pergunta foi exibida
    bool answered = false;

    private void Awake()
    {
        tick = 0;
    }

    void Start()
    {
        panelOptions = GetComponent<PanelOptions>();
        gameStarded = false;
        answered = false;
    }

    void Update()
    {
        if (gameStarded)
        {
            roundTime -= Time.deltaTime;
            if (roundTime <= 0)
            {
                roundTime = 0;
                gameStarded = false;
                virtualKeyboard.OpenKeyboard();
                panelOptions.EndGamePanel();
                objectAnimator.SetInteger("transition", 0);
                audioSource.Stop();
            }

            tickTimer += Time.deltaTime;
            if (tickTimer >= TICK_TIMER_MAX)
            {
                tickTimer -= TICK_TIMER_MAX;
                tick++;
                if (OnTick != null) OnTick(this, new OnTickEventArgs { tick = tick });
                GameStarted();
            }

            Answer();
        }
    }

    public void StarGame()
    {
        if (!gameStarded)
        {
            answers = 0;
            points = 0;
            roundTime = 60;
            audioSource.Play();
            panelOptions.leftPressed = false;
            panelOptions.rightPressed = false;
            gameStarded = true;
            objectAnimator.SetInteger("transition", 1);
            GameStarted();
        }
    }

    public void GameStarted()
    {
        answered = false;
        objectAnimator.SetInteger("transition", 1);

        // Registrar o tempo quando a nova pergunta é exibida
        questionStartTime = Time.time;

        //Desativar todos os objetos primeiro
        for (int i = 0; i < objects.Length; i++)
        {
            objects[i].Object.SetActive(false);
        }

        //Primeira organização do painel
        panelOptions.SortingPanel();
    }

    public void Answer()
    {
        if (answered) return;

        //Se botão esquerdo ou direito pressionado = ir para animação Anim_Object_Fade
        if (panelOptions.leftPressed || panelOptions.rightPressed)
        {
            if (panelOptions.CorrectOption())
            {
                // Calcular tempo de resposta
                float responseTime = Time.time - questionStartTime;

                // Calcular pontos com bônus por velocidade
                int earnedPoints = CalculatePointsBasedOnResponseTime(responseTime);
                points += earnedPoints;

                Debug.Log($"Resposta correta em {responseTime:F2}s! Pontos ganhos: {earnedPoints}");
            }

            objectAnimator.SetInteger("transition", 2);

            //Resetar tudo para o próximo objeto
            panelOptions.leftPressed = false;
            panelOptions.rightPressed = false;
            answered = true;
            answers++;
        }
    }

    private int CalculatePointsBasedOnResponseTime(float responseTime)
    {
        // Se respondeu depois do tempo máximo, ganha apenas os pontos base
        if (responseTime > maxTimeForBonus)
        {
            return basePoints;
        }

        // Calcula o bônus proporcional ao tempo de resposta
        float timeRatio = 1 - (responseTime / maxTimeForBonus);
        int bonusPoints = Mathf.RoundToInt(timeRatio * maxBonusPoints);

        return basePoints + bonusPoints;
    }
}

[System.Serializable]
public class QuizObject
{
    public string Name;
    public GameObject Object;
}