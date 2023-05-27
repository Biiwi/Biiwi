using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;
using Unity.VisualScripting;
using UnityEngine.SceneManagement;

public class QuestionAdministration : MonoBehaviour
{
    //Estados lineales del juego
   enum GameState
    {
        SelectQuestion,         //Busqueda de pregunta y alternativas
        WaitInput,              //Esperando selecci�n de alternativa
        CorrectAlternative,     //Si la respuesta es correcta
        IncorrectAlternative,   //Si la respuesta es incorrecta
        Result                  //Cambio a pantalla de resultados
    };

    [SerializeField] TextMeshProUGUI questionText;                  //Texto de preguntas
    [SerializeField] TextMeshProUGUI superiorText;                  //Texto superior en escena
    [SerializeField] TextMeshProUGUI scoreText;                     //Texto de valor de puntaje
    [SerializeField] TextMeshProUGUI[] questionAlternativesText;    //Arreglo de textos para alternativas en botones
    [SerializeField] TextMeshProUGUI clockText;
    [SerializeField] Questions[] questionsAnswers;                  //Arreglo de structura preguntas y respuesta
    [SerializeField] int[] alternativeList;                         //Arreglo con los indices de alternativas elegidas de forma aleatoria
    [HideInInspector] public int correctAlternative;                //Indice de alternativa correcta
    [HideInInspector] public int numQuestion;                       //limite de preguntas a realizar
    [HideInInspector] public bool scoreOk;                          //flag para indicar cuando ya se sum� el puntaje
    [SerializeField] Button[] Buttons;                              //Arreglo de botones
    public int score;                                               //Valor de puntaje acumulado
    float waitTimer;                                                //variable utilizada como timer de espera
    float timeWait = 0.5f;                                          //umbral de tiempo que existe entre mostrar la respuesta seleccionada y el cambio de pantalla
    static float timeQuestion = 11f;                                       //umbral que se utilizar� para asignar al tiempo por pregunta
    float questionTimer = timeQuestion;                                            //variable utilizada como timer general 
    private string scoreScene = "ScoreScene";                       //Nombre de escena de resultados
    //private string mainScene = "MainScene";                         //Nombre de escena inicial
    GameState gameState;                                            //variable para switch-case de estados
    //private ResultGame ResultGame;
    private string prefScore ="Score";
    private int hasRestart = 0;
    private int numMaxQuestion = 10;
    private bool outTimeLimit;
    

 
    private void Update()
    {
        Color transparentColor = new Color(0f, 0f, 0f, 0.8f);
        //Color highColor = new Color(0f, 0f, 0f, 0.95f);
        //Color pressColor = new Color(255f, 255f, 255f, 1f);
        hasRestart = PlayerPrefs.GetInt("Restart");
        if (hasRestart == 1)
        {
            RestartGame();
        }
        switch (gameState)
        {
            case GameState.SelectQuestion:
                if(numQuestion >= numMaxQuestion)                                                    //Si el l�mite de preguntas se alcanza cambia de estado
                {
                    gameState = GameState.Result;
                    break;
                }
                SelectQuestion();                                                       //Si sigue en la escena de preguntas, debe buscar nuevamente una pregunta no utilizada  
                superiorText.text = "";                                                 //blanquear el texto superior
                scoreOk = false;                                                        //volver flag de puntaje sumado a falso 
                for (int i = 0; i < Buttons.Length; i++)                                //Habilitar nuevamente los botones y recuperar color base
                {
                    //ColorBlock colorVar = Buttons[i].colors;
                    Buttons[i].enabled = true;
                    Buttons[i].GetComponent<Image>().color = transparentColor;
                    //colorVar.highlightedColor = highColor;
                    //colorVar.pressedColor = pressColor;
                    

                }
                gameState = GameState.WaitInput;                                        //Cambiar al estado de selecci�n de alternativas
                break;
            case GameState.WaitInput:
                questionTimer -= Time.deltaTime;
                clockText.text = ((int)(questionTimer)).ToString();
                if(questionTimer <= 0)
                {
                    outTimeLimit = true;
                    gameState = GameState.IncorrectAlternative;
                }
                break;
            case GameState.CorrectAlternative:
                waitTimer += Time.deltaTime;                                            //Empieza a correr el timer general de resultado antes de pasar a la siguiente pantalla
                if (!scoreOk)                                                           //Evaluar si ya se sum� el puntaje de la pregunta actual
                {
                    superiorText.text = "Correcto! +100 puntos";                        //Impresion de texto superior
                    score += 100;                                                       //suma de puntaje
                    
                    scoreText.text = score.ToString();                                  //Impresion del nuevo puntaje acumulado en el texto de puntaje superior
                    scoreOk = true;                                                     //cambio de estado del flag de puntaje sumado
                }
                if (waitTimer >= timeWait)                                              //si se super� el umbral
                {               
                    waitTimer = 0f;                                                     //reseteo del timer
                    gameState = GameState.SelectQuestion;                               //cambio de estado
                    questionTimer = timeQuestion;
                }
                break;
            case GameState.IncorrectAlternative:                            
                waitTimer += Time.deltaTime;
                if (outTimeLimit)
                {
                    superiorText.text = "Se acabo el tiempo";
                }
                else
                {
                    superiorText.text = "Equivocado!!";
                }
                if (waitTimer >= timeWait)
                {
                    waitTimer = 0f;
                    gameState = GameState.SelectQuestion;
                    outTimeLimit = false;
                    questionTimer = timeQuestion;
                }
                break;
            case GameState.Result:
                PlayerPrefs.SetInt(prefScore, score);
                SceneManager.LoadScene(scoreScene);                                     //Cambio de escena a resultados
                break;
        }
    }

    //Metodo para seleccionar de forma aleatoria las alternativas
    public void SelectAlternatives(int question)
    {
        int numRandom;                                                                          
        correctAlternative = Random.Range(0, questionAlternativesText.Length);                  //Se elige de forma aleatoria el espacio que ocupar� la alternativa correcta
        alternativeList[correctAlternative] = questionsAnswers[question].answers.Length;        //el largo de las alternativas posibles indicar� que ese �ndice corresponde a la alternativa correcta (arreglo[7]= indices del 0 al 6 -> si indice == 7 entonces alternativa correcta)
        for (int i = 0; i < questionAlternativesText.Length;i++)                                //ciclo para seleccionar las alternativas de forma aleatoria en los espacios faltantes
        {
            if (i != correctAlternative)                                                        //asegura que no sobreescribir� el espacio de la alternativa correcta
            {
                numRandom = Random.Range(0, questionsAnswers[question].answers.Length);         //guarda valor aleatorio entre 0 y largo de preguntas disponibles
                alternativeList[i] = numRandom;                                                 //guarda el valor en el arreglo de alternativas seleccionadas
                for (int j = 0; j < i; j++)                                                     //recorre el arreglo de alternativas seleccionadas para evaluar si ya existe o no
                {
                    if (numRandom == alternativeList[j])                                        //si se repiti�, se restar� uno a la cuenta del for, as� se evaluar� nuevamente el valor repetido
                    {
                        i--;
                        break;
                    }
                }
            }
        } 
    }


    //Metodo para elegir la pregunta
    public void SelectQuestion()
    {
        int questionSelected;                                                                                               //valor de la posici�n de la pregunta elegida
        numQuestion++;                                                                                                      //contador de preguntas ya utilizadas
        do                                                                                                                  //Selecci�n aleatoria de preguntas, asegurandose de no repetir
        {
            questionSelected = Random.Range(0, questionsAnswers.Length);
        } while (questionsAnswers[questionSelected].usedQuestion);                                                          //usedQuestion indica si la pregunta se utiliz� o no
        questionsAnswers[questionSelected].usedQuestion = true;
        questionText.text = questionsAnswers[questionSelected].question;                                                    //Se escribe la pregunta en el texto de la escena
        SelectAlternatives(questionSelected);                                                                               //Llamada del m�todo que busca y ordena las alternativas -> entrega los �ndices de posiciones dentro del arreglo
        for (int i = 0;i < questionAlternativesText.Length; i++)                                                            //imprime los textos de las alternativas de acuerdo a los indices entregados anteriormente
        {
            if (alternativeList[i] == questionsAnswers[questionSelected].answers.Length)
            {
                questionAlternativesText[i].text = questionsAnswers[questionSelected].correctAnswer;
            }
            else
            {
                questionAlternativesText[i].text = questionsAnswers[questionSelected].answers[alternativeList[i]];
            }
        }
         
        
    }

    //M�todo de presi�n del bot�n
    public void PressOption(int button)
    {
        if(button == correctAlternative)                                                            //Evalua si el bot�n presionado corresponde a la alternativa correcta
        {
            Buttons[button].GetComponent<Image>().color = Color.green;
            gameState = GameState.CorrectAlternative;
        }
        else
        {
            Buttons[button].GetComponent<Image>().color = Color.red;
            gameState = GameState.IncorrectAlternative;
            Buttons[correctAlternative].GetComponent<Image>().color = Color.green;
        }
        for(int i = 0; i < Buttons.Length; i++)                                                     //Deshabilita los botones para no poder volver a seleccionar una alternativa
        {
            Buttons[i].enabled = false;
        }
    }

    public int RefreshScore()
    {
        return (PlayerPrefs.GetInt(prefScore,0));
    }
    

    public void RestartGame()
    {
        score = 0;
        for (int i = 0; i < questionsAnswers.Length; i++)
        {
            questionsAnswers[i].usedQuestion = false;
            numQuestion = 0;
        }
        gameState = GameState.SelectQuestion;
    }


    //Clase Question para guardar preguntas, respuestas e indicador de uso de preguntas
    [System.Serializable]                                              
    public class Questions
    {
        public string question;
        public bool usedQuestion;
        public string[] answers;
        public string correctAnswer;

    };
}
