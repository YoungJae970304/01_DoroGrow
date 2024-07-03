using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// ���� �̵� / ���� ���� ǥ��(text), ��ư ���� ���� ���� Ŭ����
public class UIManager
{
    public void Btn_Quit()
    {
        // Ŭ�� �� ���ø����̼� ����
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    // Ŭ�� �� Ÿ��Ʋ ������ �̵�
    public void Btn_GoToTitle()
    {
        SceneManager.LoadScene("00_Title");
        Time.timeScale = 1;
    }

    // Ŭ�� �� �κ� ������ �̵�
    public void Btn_GoToRobby()
    {
        SceneManager.LoadScene("01_Robby");
        Time.timeScale = 1;
    }

    //Pause�� Play ��ư�� ���� �Լ�
    public void Btn_TimeScale()
    {
        if (Time.timeScale > 0)
        {
            //�÷��� ���¶�� �Ͻ�����
            Time.timeScale = 0;
        }
        else
        {
            //�Ͻ����� ���¶�� �÷���
            Time.timeScale = 1;
        }
    }

    ////////////////////////////////////////////// �κ�� ���õ� �Լ��� /////////////////////////////////////////////////

    //�Ű������� ���� ���ڿ��� ���� �� �̵�
    public void Btn_GoToStage(string toggleName)
    {
        switch (toggleName)
        {
            case "ToggleDrake":
                SceneManager.LoadScene("02_Stage1");
                Time.timeScale = 1;
                break;
            case "ToggleMaxwell":
                SceneManager.LoadScene("03_Stage2");
                Time.timeScale = 1;
                break;
            case "ToggleLaplace":
                SceneManager.LoadScene("04_Stage3");
                Time.timeScale = 1;
                break;
            default:
                Debug.Log("�������� ����");
                break;
        }
    }


    //������ �ؽ�Ʈ�� �������°� �����ϴ� �Լ�
    //Text_Display(�ؽ�Ʈ ������Ʈ ����, GameManager�ȿ� �ִ� ����)
    public void Text_Display(TextMeshProUGUI textGo, double doroVar)
    {
        switch (textGo.name)
        {
            case "LevelDisplay":
                textGo.text = "Lv " + doroVar;
                break;

            case "ATK":
                textGo.text = doroVar.ToString("n0");
                break;

            case "HP":
                textGo.text = doroVar.ToString("n0");
                break;

            case "Dodge":
                textGo.text = (doroVar / 10).ToString("f1") + "%";
                break;

            case "Accuracy":
                textGo.text = (doroVar / 10).ToString("f1") + "%";
                break;

            case "Sp":
                textGo.text = doroVar.ToString("n0");
                break;

            case "Mold":
                textGo.text = doroVar.ToString("n0");
                break;

            case "Timer":
                textGo.text = doroVar.ToString("f2");
                break;

            case "HaveDrake":
                textGo.text = doroVar.ToString("n0");
                break;

            case "HaveMaxwell":
                textGo.text = doroVar.ToString("n0");
                break;

            case "HaveLaplace":
                textGo.text = doroVar.ToString("n0");
                break;

            case "LvDrake":
                textGo.text = "Lv " + doroVar.ToString("n0");
                break;

            case "LvMaxwell":
                textGo.text = "Lv " + doroVar.ToString("n0");
                break;

            case "LvLaplace":
                textGo.text = "Lv " + doroVar.ToString("n0");
                break;

            default:
                Debug.Log("������ ���̽��� �ƴ�");
                break;
        }
    }

    //ExpBarDisplay(�����̴� ����, GameManager�ȿ� �ִ� doro_Exp, GameManager�ȿ� �ִ� doro_ExpMax)
    public void ExpBarDisplay(Slider expbar, double exp, double expMax)
    {
        expbar.value = (float)(exp / expMax);
    }

    //��ư�� ������ �� ��ư�� �ش��ϴ� ������ �ø��� �Լ�
    public void Btn_StatUp(Button btnGo)
    {
        //sp�� 0���� Ŭ ���� �۵�
        if (GameManager.gm.doro_Sp > 0)
        {
            switch (btnGo.name)
            {
                case "UP_ATK":
                    //���η��� ���ݷ� ����
                    GameManager.gm.doro_Atk++;
                    break;
                case "UP_HP":
                    //�ִ� HP ����
                    GameManager.gm.doro_HpMax += 5;
                    break;
                case "UP_Dodge":
                    //ȸ���� ����
                    GameManager.gm.doro_Dodge_Origin++; 
                    break;
                case "UP_Accuracy":
                    //���߷� ����
                    GameManager.gm.doro_Accuracy++; 
                    break;
                default:
                    Debug.Log("������ ���̽��� �ƴ�");
                    break;
            }
            //��������Ʈ ����
            GameManager.gm.doro_Sp--;
        }
    }

    public void Camera_Select_View(GameObject doro, TMP_Dropdown viewMode)
    {
        //ȭ������� ���õǾ� ���� ��
        if (viewMode.value == 0)
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = Mathf.Clamp( doro.transform.position.x, -5.5f, 5.5f);
            camPos.y = Camera.main.transform.position.y;
            camPos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = camPos;
        }

        //�����̵��� ���õǾ� ���� ��
        else
        { 
            //���콺�� ��ġ �Է��� ����� ī�޶� ������ ���� �̵�
            float moveSpeed = -0.1f; //ī�޶� �̵� �ӵ�
            Vector3 camPos = Camera.main.transform.position;

            // ��� �̹����� ��ġ�� ũ�� ��������

            //���콺 ������ ��ư�� ���� ��
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");

                //camPos.x += mouseX * moveSpeed;

                // ���ο� ī�޶� ��ġ ��� �� �̵� ���� ����
                camPos.x = Mathf.Clamp(camPos.x + mouseX * moveSpeed, -5.5f, 5.5f);
                //Mathf.Clamp(Value, min, max) �Լ��� Value ����
                //min���� �۾����� min���� ��ü�ǰ� max���� �������� max�� ��ü, �� ���� ��쿡�� value���� ����

                Camera.main.transform.position = camPos;
            }
        }
    }

    /////////////////////////////////////////// ���������� ���õ� �Լ��� //////////////////////////////////////////////
    
    //���� HP�� ������� �����ִ� �ռ�
    public void HpBarDisplay(Slider hpbar, TextMeshProUGUI text, double hp, double hpMax)
    {
        //hpbar�� value�� 0���� 1������ �Ǽ��̹Ƿ� ���� hp�� �ִ�hp�� ������ 0���� 1������ �Ǽ��� �������� ����
        hpbar.value = (float)(hp / hpMax);

        //�̸��� ���� hpbar�� ǥ�õǴ� �ؽ�Ʈ�� �޶���
        if (hpbar.name == "DoroHP")
        {
            text.text = "[ Lv " + GameManager.gm.doro_Level + " ���η� ] " + (hpbar.value * 100).ToString("f2") + "%";
        }

        if (hpbar.name == "DrakeHP")
        {
            text.text = "[ Lv " + GameManager.gm.drake_Level + " �巹��ũ ] " + (hpbar.value * 100).ToString("f2") + "%";
        }
        else if (hpbar.name == "MaxwellHP")
        {
            text.text = "[ Lv " + GameManager.gm.maxwell_Level + " �ƽ��� ] " + (hpbar.value * 100).ToString("f2") + "%";
        }
        else if (hpbar.name == "LaplaceHP")
        {
            text.text = "[ Lv " + GameManager.gm.laplace_Level + " ���ö� ] " + (hpbar.value * 100).ToString("f2") + "%";
        }        

        //value�� 0�̵Ǿ����� fill area�� ���� �����־��µ�, 0�� �Ǹ� �����ִ°ŷ� ������ �ʰ� Fill  Area�� ��Ȱ��ȭ
        if (hpbar.value <= 0)
        {
            hpbar.transform.Find("Fill Area").gameObject.SetActive(false);
        }
        else
        {
            hpbar.transform.Find("Fill Area").gameObject.SetActive(true);
        }
    }

    //���� ����� ���� ���â�� ǥ�����ִ� �Լ�
    public void End_UI_Window(GameObject window, TextMeshProUGUI endTxt, string result, TextMeshProUGUI mold, string getmold)
    {
        // window = ���â(��Ȱ��ȭ) / endTxt = ����� ǥ�õ� �ؽ�Ʈ / result = �����(�¸�, �й�) / mold = ȹ���� ���尡 ǥ�õ� �ؽ�Ʈ / getmold = ȹ���� ���尹��
        window.SetActive(true);

        endTxt.text = result;
        mold.text = getmold;
    }
}
