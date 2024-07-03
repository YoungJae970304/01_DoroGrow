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
    //��� UI
    [Header("��� UI ��ư")]
    public Button btn_Title;
    public Button btn_Quit_Robby;
    public Button btn_GoStage;
    public Button btn_End;

    [Header("��� UI �ؽ�Ʈ")]
    public TextMeshProUGUI level;
    public TextMeshProUGUI atk;
    public TextMeshProUGUI hp;
    public TextMeshProUGUI dodge;
    public TextMeshProUGUI accuracy;
    public TextMeshProUGUI sp;
    public TextMeshProUGUI mold;

    [Space(15f)]
    [Tooltip("������ �� ��")]
    public TextMeshProUGUI hdrake;
    [Tooltip("������ �� ��")]
    public TextMeshProUGUI hmaxwell;
    [Tooltip("������ �� ��")]
    public TextMeshProUGUI hlaplace;

    [Space(15f)]
    [Tooltip("������ ����")]
    public TextMeshProUGUI ldrake;
    [Tooltip("������ ����")]
    public TextMeshProUGUI lmaxwell;
    [Tooltip("������ ����")]
    public TextMeshProUGUI llaplace;

    [Header("��� UI ��Ÿ")]
    public Slider exp;
    public TMP_Dropdown viewMode;

    //�ϴ� UI
    [Header("�ϴ� UI ��۱׷�")]
    public ToggleGroup tg_Stage;
    public ToggleGroup tg_Shop;

    [Header("�ϴ� UI ��ư")]
    public Button btn_up_Atk;
    public Button btn_up_Hp;
    public Button btn_up_Dodge;
    public Button btn_up_Speed;
    public Button btn_Buy;

    //���� ������Ʈ
    [Header("���� ������Ʈ")]
    public GameObject prefab_ExpUp;
    public GameObject prefab_LevelUp;
    public GameObject drake;
    public GameObject maxwell;
    public GameObject laplace;

    [Space(15f)]
    public GameObject doro;
    public GameObject endDoro;

    //���õ� �������� ���
    Toggle activeToggle;
    int endTriggerValue = 150;

    //[Range(0.5f, 1.5f)]   ������ �����̴� �������� ǥ���ϰ� �ּ�, �ִ� ���� �����ִ� �Ӽ�
    //[Multiline(3)]        ���ڿ��� �ۼ��ϱ� ���ϵ��� ������ ���� �÷��ִ� �Ӽ�
    //[TextArea(3,5)]       ��Ƽ ���ΰ� ���������� �ּ� ũ��� �ִ� ũ�⸦ �������ִ� �Ӽ� = �ּ� 3��, 5�ٱ��� �������� ���ķδ� ��ũ�ѹ� ����
    //[SerializeField]      praviate ������ �ν����Ϳ� ǥ���ϴ� �Ӽ�
    //[HideInInspector]     public ������ �ν����Ϳ��� ����� �Ӽ�
    //[AddComponentMenu("���")]

    void Start()
    {
        //��ư Ŭ���� �ش� �Լ� ȣ��
        //��� ��ư
        btn_Title.onClick.AddListener(GameManager.gm.um.Btn_GoToTitle);
        btn_Quit_Robby.onClick.AddListener(GameManager.gm.um.Btn_Quit);
        btn_End.onClick.AddListener(() => GameManager.gm.EndingStart(endDoro));

        //�ϴ� �������� �̵� ��ư
        btn_GoStage.onClick.AddListener(delegate    //delegate�� ����
        {
            //delegate�� �޼��峪 �Լ��� �����ϴ� �������� �Լ��� ������ �Ҵ��ϰų� �Ű������� �����ϰ�, ���ϰ����� ���� �� �ִ�//
            if (activeToggle != null)
            {
                //"����" ��ư�� Ŭ���� activeToggle�� null �ƴ� ���� ���������� �̵��ϴ� �Լ��� ȣ��
                GameManager.gm.um.Btn_GoToStage(activeToggle.name);
            }
            //�� ���ǹ��� ������� ������ �ƹ��͵� ���õ��� ���� ���·� "����"��ư�� ������ NullReference�� �� -> Allow Switch Off�� false�� �ؼ� �ذ������� �ϴ��� �־��
        });

        //�ϴ� ���� ���� ��ư
        btn_Buy.onClick.AddListener(() =>       //���ٽ����� ����
        {
            if (activeToggle != null)
            {
                GameManager.gm.Btn_BuyMetisPet(activeToggle.name);
            }
        });

        Set_ActiveToggle(tg_Stage);   //activeToggle�� ��� �׷� ���� �ִ� ����� ���°� ����� ���� ���õ� ����� ����
        Set_ActiveToggle(tg_Shop);
    }

    void Update()
    {
        //Debug.Log(GameManager.gm.spawnedPets.Count);
        // ��� UI ( Profile �κ� Lv�� ����ġ�� )
        //Text_Display(�ؽ�Ʈ ������Ʈ ����, GameManager�ȿ� �ִ� ����)
        GameManager.gm.um.Text_Display(level, GameManager.gm.doro_Level);
        //ExpBarDisplay(�����̴� ����, GameManager�ȿ� �ִ� doro_Exp, GameManager�ȿ� �ִ� doro_ExpMax)
        GameManager.gm.um.ExpBarDisplay(exp, GameManager.gm.doro_Exp, GameManager.gm.doro_ExpMax);


        // �ϴ� UI - ����
        GameManager.gm.um.Text_Display(atk, GameManager.gm.doro_Atk);
        GameManager.gm.um.Text_Display(hp, GameManager.gm.doro_HpMax);
        GameManager.gm.um.Text_Display(dodge, (long)GameManager.gm.doro_Dodge_Origin);
        GameManager.gm.um.Text_Display(accuracy, (long)GameManager.gm.doro_Accuracy);
        GameManager.gm.um.Text_Display(sp, GameManager.gm.doro_Sp);

        //�ϴ� UI - ����
        GameManager.gm.um.Text_Display(mold, GameManager.gm.mold);
        GameManager.gm.um.Text_Display(hdrake, GameManager.gm.drake_Pet);
        GameManager.gm.um.Text_Display(hmaxwell, GameManager.gm.maxwell_Pet);
        GameManager.gm.um.Text_Display(hlaplace, GameManager.gm.laplace_Pet);

        //�ϴ� UI - ��������
        GameManager.gm.um.Text_Display(ldrake, GameManager.gm.drake_Level);
        GameManager.gm.um.Text_Display(lmaxwell, GameManager.gm.maxwell_Level);
        GameManager.gm.um.Text_Display(llaplace, GameManager.gm.laplace_Level);

        //ī�޶� ����
        GameManager.gm.um.Camera_Select_View(doro, viewMode);

        //��ġ�� ����
        GameManager.gm.ClickEvent_DependingScene(doro, prefab_ExpUp, prefab_LevelUp);

        //���� ���ǿ� ������ ����
        if (GameManager.gm.doro_Level > 10 && GameManager.gm.doro_Atk > endTriggerValue-100 && GameManager.gm.doro_HpMax > endTriggerValue)
        {
            //���� ����ϴ� ��ư Ȱ��ȭ
            btn_End.gameObject.SetActive(true);
        }
    }

    void Set_ActiveToggle(ToggleGroup tg)
    {
        //foreach ������ ���� tg_Stage�� �ڽ����� Toggle ������Ʈ�� ���� �ִ� ��� ������Ʈ�� ��ȯ 
        foreach (Toggle toggle in tg.GetComponentsInChildren<Toggle>())
        {
            //�� ����� �ݺ��ϸ� onValueChanged �̺�Ʈ�� ���� �����ʸ� ���, �����ʴ� ����� ���°� ����� ������ ȣ��
            toggle.onValueChanged.AddListener((isOn) =>
            {
                //���� ǥ�������� (isOn)�� �Ű����� =>�� �Ű������� �Լ� ������ �����ϴ� ȭ��ǥ { ... }�� ���� �Լ��� ����//
                if (isOn)
                {
                    //isOn�� ���� ���� �ʿ���� �����Ϸ��� �ڵ����� �߷�//
                    //����� ���°� ���� ��쿡�� activeToggle_Stage�� ���� toggle ����
                    activeToggle = toggle;
                }
                /*else  //Allow Switch Off�� ������ ���� �ڵ�� ���ص� �ǵ��� ��
                {
                    //����� �����ִ� ��쿡�� null�� �����ؼ� �������� ������ �ȵǵ���
                    activeToggle_Stage = null;
                }*/
    });
        }
    }
}
