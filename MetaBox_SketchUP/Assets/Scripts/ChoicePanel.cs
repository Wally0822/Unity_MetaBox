using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class ChoicePanel : MonoBehaviour
{
    [Header("[Obj]")]
    [SerializeField] GameObject obj = null;
    [SerializeField] GameObject clearAnimation = null;

    [Header("[Button]")]
    [SerializeField] Button choiceOneBut = null;
    [SerializeField] Button choiceTwoBut = null;
    [SerializeField] Button choiceThreeBut = null;
    [SerializeField] Button choiceFourBut = null;

    [Header("[Button Text]")]
    TextMeshProUGUI butOneText = null;
    TextMeshProUGUI butTwoText = null;
    TextMeshProUGUI butThreeText = null;
    TextMeshProUGUI butFourText = null;

    [Header("[Audio Index]")]
    [SerializeField] private int audioIndex = 0;

    private int objIndex;
    GameObject instEffect = null;

    public int AudioIndex { get { return audioIndex; } }
    public string Answers { get ;  set ; }

    RandomChoiceKeyWord choickKeyWord;
    WaitForSeconds waittime;
    ClearAnimation clearAnimaition = null;

    public int ObjIndex { get { return objIndex; } }
    DrawLineCurve drawline = null;

    void Awake()
    {
        clearAnimation.gameObject.SetActive(true);
        InGamePanelSet.Inst.StageClearPanelSet(false);
        clearAnimation.TryGetComponent<ClearAnimation>(out clearAnimaition);
        TryGetComponent<RandomChoiceKeyWord>(out choickKeyWord);
        Answers = choickKeyWord.answer;
        obj.TryGetComponent<DrawLineCurve>(out drawline);
        waittime = new WaitForSeconds(0.2f);
    }

    void Start()
    {
        choiceOneBut.onClick.AddListener(delegate { CheckString(choiceOneBut, butOneText); 
            SoundManager.Inst.ButtonEffect(choiceOneBut.transform.position); });

        choiceTwoBut.onClick.AddListener(delegate { CheckString(choiceTwoBut, butTwoText); 
            SoundManager.Inst.ButtonEffect(choiceTwoBut.transform.position); });

        choiceThreeBut.onClick.AddListener(delegate { CheckString(choiceThreeBut, butThreeText);
            SoundManager.Inst.ButtonEffect(choiceThreeBut.transform.position); });

        choiceFourBut.onClick.AddListener(delegate { CheckString(choiceFourBut, butFourText); 
            SoundManager.Inst.ButtonEffect(choiceFourBut.transform.position); });
    }

    void CheckString(Button butNum, TextMeshProUGUI butText)
    {
        butNum.transform.GetChild(0).TryGetComponent<TextMeshProUGUI>(out butText);

        if (butText.text == Answers)
        {
            InGamePanelSet.Inst.InGameSet(false); // X 버튼 닫기

            SetObjIndex(5); // 오브젝트 인덱스 바꿔주기
            ClearCountDown(); // 클리어 카운트 1 다운

            PlayTTS();
            Invoke(nameof(PanelSet), 1.5f);
        }
        else
            SoundManager.Inst.SelectPanelNoNameSFXPlay();
    }

    int SetObjIndex(int objIndex)
    {
        drawline.ObjIndex = objIndex;
        if (obj.gameObject.active == false)
        {
            return drawline.ObjIndex;
        }
        else
            return 0;
    }

    int ClearCountDown()
    {
        InGamePanelSet.Inst.ClearCount -= 1;
        return InGamePanelSet.Inst.ClearCount;
    }

    void PlayTTS()
    {
        clearAnimaition.StartCoroutine(clearAnimaition.Moving());
        this.gameObject.SetActive(false);
        SoundManager.Inst.AnimalAudioPlay(AudioIndex);
    }

    void PanelSet()
    {
        clearAnimation.gameObject.SetActive(false);
        if (InGamePanelSet.Inst.ClearCount > 0)
        {
            InGamePanelSet.Inst.StageClearPanelSet(true);
            InGamePanelSet.Inst.InstAnimalClearEffect();
        }
        if (InGamePanelSet.Inst.ClearCount == 0)
        {
            InGamePanelSet.Inst.SelectPanelSet(false);
        }
    }
}