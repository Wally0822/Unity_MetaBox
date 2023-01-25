using System.Collections;
using System.Threading;
using UnityEngine;
using Kum;

public class GameManager : MonoSingleTon<GameManager>
{
    //=====================var=======================
    [Header("Currnet Score")]
    public int Score = 0;
    
    [Header("Limit Time")]
    public int Timer = 180;
    private float timer = 0f;

    [Header("Imminent Time")]
    public int ImTime = 30;
    private bool AudioToken = true;

    [Header("Current Game Status")]
    public bool IsGameIn = false;
    public bool IsStart = false;
    public bool IsPause = false;
    public bool IsGameOver = false;

    [Header("Game Difficulty Level")]
    public int Level = 1;

    //======================timer====================
    Coroutine TickRoutine;

    private readonly WaitForSeconds wait = new(1f);

    private new void Awake()
    {
        //delegate chain
        EventReciver.ScoreModi += ScoreAddSub;

        EventReciver.GamePause += GamePasue;
        EventReciver.GameResume += GameResume;
        EventReciver.GameOver += GameOver;

        //initializing
        IsGameIn = true;
        IsStart = false;
        IsPause = false;
        IsGameOver = false;

        Score = 0;
        timer = 0f;

        StartCoroutine(nameof(BeforeStart));
    }

    private void OnDisable()
    {
        //delegate unchain
        EventReciver.ScoreModi -= ScoreAddSub;

        EventReciver.GamePause -= GamePasue;
        EventReciver.GameResume -= GameResume;
        EventReciver.GameOver -= GameOver;
    }

    //============================================Score Controll============================================
    void ScoreAddSub(int value)
    {
        Score += value;

        if (Score < 0) Score = 0;
    }

    //=========================================Game Life Controll============================================
    void GamePasue()
    {
        StopCoroutine(TickRoutine);
        IsPause = true;
        Time.timeScale = 0;
    }
    void GameResume()
    {
        TickRoutine = StartCoroutine(nameof(TickUpdate));
        IsPause = false;
        Time.timeScale = 1f;   
    }
    void GameOver()
    {
        StopCoroutine(TickRoutine);
        IsGameOver = true;
        SoundManager.Inst.SetBGM("StageClear");
        SoundManager.Inst.SetBGMUnLoop();
        SoundManager.Inst.PlayBGM();
        Time.timeScale = 0f;
    }

    //============================================Timer Controll==========================================//씬 시작 연출 수정
    public int count = 0;

    IEnumerator BeforeStart()
    {
        SoundManager.Inst.StopBGM();
        EventReciver.CallSceneStart();

        while (timer <= 6f)
        {
            timer += Time.fixedDeltaTime;
            count = (int)timer;
            yield return null;
        }

        //bgm setting
        SoundManager.Inst.SetBGM("MainBGM");
        SoundManager.Inst.SetBGMLoop();
        SoundManager.Inst.PlayBGM();

        //main loop start
        IsStart = true;
        EventReciver.CallGameStart();
        TickRoutine = StartCoroutine(nameof(TickUpdate));
    }
    IEnumerator TickUpdate()
    {
        while (!IsGameOver && IsGameIn)
        {
            yield return wait;

            Timer -= 1;
            EventReciver.CallTickCount();
            if (AudioToken && Timer == ImTime)
            {
                AudioToken = false;
                SoundManager.Inst.SetBGMSpeed(1.3f);
            }
            if (Timer == 0) EventReciver.CallGameOver();
        }
    }

}
