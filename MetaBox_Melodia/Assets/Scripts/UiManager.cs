using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;
using TMPro;
using System.Collections;

public class UiManager : MonoBehaviour
{
    [Header("Panel")]
    [SerializeField] GameObject myPanelUntouchable;
    [SerializeField] GameObject myPanelNextStage;
    [SerializeField] GameObject myPanelGameClear;
    [SerializeField] GameObject myPanelGameOver;
    [SerializeField] GameObject myPanelOption;

    [Header("Buttons")]
    [SerializeField] Button myButtonOption;
    [SerializeField] Button myButtonReplay;
    [SerializeField] Button myButtonNext;

    [Header("Image")]
    [SerializeField] Image myCountDown;
    [SerializeField] GameObject myGameStart;

    [Header("Text")]
    [SerializeField] TextMeshProUGUI myTimer;
    [SerializeField] TextMeshProUGUI playTime;

    [Header("bird Control")]
    [SerializeField] AnimationCurve BirdPosCurve; // = new AnimationCurve(new Keyframe[] { new Keyframe(0f, 0f), new Keyframe(0.5f, 0.2f), new Keyframe(0.7f, 0f) });

    [SerializeField] Fade fade = null;
    [SerializeField] ScriptableObj scriptableImg = null;

    public float ReplayCoolTime { get; set; }

    private void Awake()
    {
        myButtonOption.onClick.AddListener(OnClickOption);
        myButtonReplay.onClick.AddListener(OnClickReplay);
        myButtonNext.onClick.AddListener(OnClickNextStage);
        // observe game status 
        GameManager.Inst.myDelegateGameStatus += curGameStatus;
        GameManager.Inst.DelegateTimer = playTimer;

    }

    private void Start()
    {
        ReplayCoolTime = GameManager.Inst.MelodiaData.replayCooltime;
    }

    void curGameStatus(GameStatus curStatus)
    {
        switch (curStatus)
        {
            case GameStatus.Idle:
                {
                    StartGame();
                }
                break;

            case GameStatus.GamePlaying:
                {
                    myPanelUntouchable.SetActive(false);
                }
                break;
            case GameStatus.Pause:
                {
                    myPanelUntouchable.SetActive(true);
                }
                break;

            case GameStatus.GetAllQNotes:
                {
                    myPanelNextStage.SetActive(true);
                }
                break;

            case GameStatus.Fail:
                {
                    myPanelGameOver.SetActive(true);
                }
                break;

            case GameStatus.GameClear:
                {
                    myPanelGameClear.SetActive(true);
                    playTime.text = string.Format("{0:D2} : {1:D2}", ((GameManager.Inst.MelodiaData.countDown - GameManager.Inst.MyPlayableTime) / 60), ((GameManager.Inst.MelodiaData.countDown - GameManager.Inst.MyPlayableTime) % 60));
                }
                break;
        }
    }

    public void StartGame()
    {
        
        // text for ready 
        myTimer.text = "";

        // to disable touch interaction 
        myPanelUntouchable.SetActive(true);
        myPanelNextStage.SetActive(false);
        myPanelGameClear.SetActive(false);
        myPanelGameOver.SetActive(false);
        myPanelOption.SetActive(false);

        myCountDown.gameObject.SetActive(false);
        myGameStart.SetActive(false);
    }



    // timer for start
    void playTimer(int t)
    {
        if (GameManager.Inst.CurStatus.Equals(GameStatus.Idle))
        {
            if(t > 0)
            {
                myCountDown.gameObject.SetActive(false);
                myCountDown.gameObject.SetActive(true);
                myCountDown.sprite = scriptableImg.CountDownImg[t - 1];
            }
            else
            {
                myCountDown.gameObject.SetActive(false);
                if (myGameStart.activeSelf) myGameStart.SetActive(false);
                else myGameStart.SetActive(true);
            }
            
        }
        else myTimer.text = string.Format("{0:D2} : {1:D2}", (t / 60), (t % 60));
    }


    // Replay Music ==========================================================
    public void OnClickReplay()
    {
        SoundManager.Inst.SFXPlay(SFX.ReplayTouch);

        StartCoroutine(nameof(BirdShow));
        SoundManager.Inst.RePlay();

        myButtonReplay.interactable = false;

        Invoke(nameof(readyReplay), ReplayCoolTime);
    }

    void readyReplay()
    {
        myButtonReplay.interactable = true;
    }

    IEnumerator BirdShow()
    {
        for (int i = 0; i < 3; i++)
        {
            float startTime = 0;
            Vector3 birdOriPos = myButtonReplay.transform.position;
            Vector3 birdCurPos = myButtonReplay.transform.position;

            while (BirdPosCurve.keys[BirdPosCurve.keys.Length - 1].time >= startTime)
            {
                birdCurPos.y = birdOriPos.y + BirdPosCurve.Evaluate(startTime);
                myButtonReplay.transform.position = birdCurPos;

                startTime += Time.deltaTime;
                yield return null;
            }

            if (GameManager.Inst.CurStatus.Equals(GameStatus.GetAllQNotes) ||
                    GameManager.Inst.CurStatus.Equals(GameStatus.Pause) ||
                    GameManager.Inst.CurStatus.Equals(GameStatus.Fail)) yield break;
        }
        
    }

    // Replay Music ==========================================================






    public void OnClickOption()
    {
        GameManager.Inst.UpdateCurProcess(GameStatus.Pause);
        myPanelOption.SetActive(true);
    }


    // Move to next stage ==========================================================
    void OnClickNextStage()
    {
        GameManager.Inst.UpdateCurProcess(GameStatus.Idle);
    }


    // Exit game and return to start scene ===========================================
    public void OnClickHome()
    {
        SoundManager.Inst.StopMusic();

        fade.FadeOut();
    }


    // Restart stage from beginning =================================================
    public void OnClickReStart()
    {
        SoundManager.Inst.StopMusic();

        GameManager.Inst.UpdateCurProcess(GameStatus.Restart);
    }

}
