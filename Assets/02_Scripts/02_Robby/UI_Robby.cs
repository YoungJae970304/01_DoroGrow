using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;
using System.Linq;
using Unity.Collections.LowLevel.Unsafe;

public class UI_Robby : MonoBehaviour
{
    //상단 UI
    [Header("상단 UI 버튼")]
    public Button btn_Title;
    public Button btn_Quit_Robby;
    public Button btn_GoStage;
    public Button btn_End;

    [Header("상단 UI 텍스트")]
    public TextMeshProUGUI level;
    public TextMeshProUGUI atk;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI dodge;
    public TextMeshProUGUI accuracy;
    public TextMeshProUGUI sp;
    public TextMeshProUGUI mold;

    [Space(15f)]
    [Tooltip("보유한 펫 수")]
    public TextMeshProUGUI hdrake;
    [Tooltip("보유한 펫 수")]
    public TextMeshProUGUI hmaxwell;
    [Tooltip("보유한 펫 수")]
    public TextMeshProUGUI hlaplace;

    [Space(15f)]
    [Tooltip("보스의 레벨")]
    public TextMeshProUGUI ldrake;
    [Tooltip("보스의 레벨")]
    public TextMeshProUGUI lmaxwell;
    [Tooltip("보스의 레벨")]
    public TextMeshProUGUI llaplace;

    [Header("상단 UI 기타")]
    public Slider exp;
    public TMP_Dropdown viewMode;

    //하단 UI
    [Header("하단 UI 토글그룹")]
    public ToggleGroup tg_Stage;
    public ToggleGroup tg_Shop;

    [Header("하단 UI 버튼")]
    public Button btn_up_Atk;
    public Button btn_up_Hp;
    public Button btn_up_Dodge;
    public Button btn_up_Speed;
    public Button btn_Buy;

    //게임 오브젝트
    [Header("게임 오브젝트")]
    public GameObject prefab_ExpUp;
    public GameObject prefab_LevelUp;
    public GameObject drake;
    public GameObject maxwell;
    public GameObject laplace;

    [Space(15f)]
    public GameObject doro;
    public GameObject endDoro;

    //선택된 스테이지 토글
    Toggle activeToggle;
    int endTriggerValue = 150;

    //[Range(0.5f, 1.5f)]   변수를 슬라이더 형식으로 표시하고 최소, 최대 값을 정해주는 속성
    //[Multiline(3)]        문자열을 작성하기 편하도록 라인의 수를 늘려주는 속성
    //[TextArea(3,5)]       멀티 라인과 유사하지만 최소 크기와 최대 크기를 설정해주는 속성 = 최소 3줄, 5줄까지 생성가능 이후로는 스크롤바 생성
    //[SerializeField]      praviate 변수를 인스펙터에 표시하는 속성
    //[HideInInspector]     public 변수를 인스펙터에서 숨기는 속성
    //[AddComponentMenu("경로")]

    void Start()
    {
        //버튼 클릭시 해당 함수 호출
        //상단 버튼
        btn_Title.onClick.AddListener(GameManager.gm.um.Btn_GoToTitle);
        btn_Quit_Robby.onClick.AddListener(GameManager.gm.um.Btn_Quit);
        btn_End.onClick.AddListener(() => GameManager.gm.EndingStart(endDoro));

        //하단 스테이지 이동 버튼
        btn_GoStage.onClick.AddListener(delegate    //delegate로 만들어봄
        {
            //delegate는 메서드나 함수를 참조하는 형식으로 함수를 변수에 할당하거나 매개변수로 전달하고, 리턴값으로 받을 수 있다//
            if (activeToggle != null)
            {
                //"진입" 버튼을 클릭시 activeToggle가 null 아닐 때만 스테이지를 이동하는 함수를 호출
                GameManager.gm.um.Btn_GoToStage(activeToggle.name);
            }
            //이 조건문을 사용하지 않으면 아무것도 선택되지 않은 상태로 "진입"버튼을 누르면 NullReference가 뜸 -> Allow Switch Off를 false로 해서 해결했지만 일단은 넣어둠
        });

        //하단 상점 구매 버튼
        btn_Buy.onClick.AddListener(() =>       //람다식으로 만들어봄
        {
            if (activeToggle != null)
            {
                GameManager.gm.Btn_BuyMetisPet(activeToggle.name);
            }
        });

        Set_ActiveToggle(tg_Stage);   //activeToggle는 토글 그룹 내에 있는 토글의 상태가 변경될 때만 선택된 토글을 대입
        Set_ActiveToggle(tg_Shop);
    }

    void Update()
    {
        //Debug.Log(GameManager.gm.spawnedPets.Count);
        // 상단 UI ( Profile 부분 Lv과 경험치바 )
        //Text_Display(텍스트 오브젝트 변수, GameManager안에 있는 변수)
        GameManager.gm.um.Text_Display(level, GameManager.gm.doro_Level);
        //ExpBarDisplay(슬라이더 변수, GameManager안에 있는 doro_Exp, GameManager안에 있는 doro_ExpMax)
        GameManager.gm.um.ExpBarDisplay(exp, GameManager.gm.doro_Exp, GameManager.gm.doro_ExpMax);


        // 하단 UI - 스탯
        GameManager.gm.um.Text_Display(atk, GameManager.gm.doro_Atk);
        GameManager.gm.um.Text_Display(hp, GameManager.gm.doro_HpMax);
        GameManager.gm.um.Text_Display(dodge, (long)GameManager.gm.doro_Dodge_Origin);
        GameManager.gm.um.Text_Display(accuracy, (long)GameManager.gm.doro_Accuracy);
        GameManager.gm.um.Text_Display(sp, GameManager.gm.doro_Sp);

        //하단 UI - 상점
        GameManager.gm.um.Text_Display(mold, GameManager.gm.mold);
        GameManager.gm.um.Text_Display(hdrake, GameManager.gm.drake_Pet);
        GameManager.gm.um.Text_Display(hmaxwell, GameManager.gm.maxwell_Pet);
        GameManager.gm.um.Text_Display(hlaplace, GameManager.gm.laplace_Pet);

        //하단 UI - 스테이지
        GameManager.gm.um.Text_Display(ldrake, GameManager.gm.drake_Level);
        GameManager.gm.um.Text_Display(lmaxwell, GameManager.gm.maxwell_Level);
        GameManager.gm.um.Text_Display(llaplace, GameManager.gm.laplace_Level);

        //카메라 동작
        GameManager.gm.um.Camera_Select_View(doro, viewMode);

        //터치시 동작
        GameManager.gm.ClickEvent_DependingScene(doro, prefab_ExpUp, prefab_LevelUp);

        //엔딩 조건에 맞으면 실행
        if (GameManager.gm.doro_Level > 10 && GameManager.gm.doro_Atk > endTriggerValue-100 && GameManager.gm.doro_HpMax > endTriggerValue)
        {
            //엔딩 재생하는 버튼 활성화
            btn_End.gameObject.SetActive(true);
        }
    }

    void Set_ActiveToggle(ToggleGroup tg)
    {
        //foreach 루프를 통해 tg_Stage의 자식으로 Toggle 컴포넌트를 갖고 있는 모든 오브젝트를 반환 
        foreach (Toggle toggle in tg.GetComponentsInChildren<Toggle>())
        {
            //각 토글을 반복하며 onValueChanged 이벤트에 대한 리스너를 등록, 리스너는 토글의 상태가 변경될 때마다 호출
            toggle.onValueChanged.AddListener((isOn) =>
            {
                //람다 표현식으로 (isOn)은 매개변수 =>은 매개변수와 함수 본문을 구분하는 화살표 { ... }은 람다 함수의 본문//
                if (isOn)
                {
                    //isOn은 따로 선언 필요없이 컴파일러가 자동으로 추론//
                    //토글의 상태가 켜진 경우에만 activeToggle_Stage에 켜진 toggle 대입
                    activeToggle = toggle;
                }
                /*else  //Allow Switch Off를 꺼놔서 굳이 코드로 안해도 되도록 함
                {
                    //토글이 꺼져있는 경우에는 null로 대입해서 스테이지 진입이 안되도록
                    activeToggle_Stage = null;
                }*/
    });
        }
    }
}
