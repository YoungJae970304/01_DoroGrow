using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.SceneManagement;
using TMPro;

// 씬의 이동 / 씬의 정보 표기(text), 버튼 동작 등을 담은 클래스
public class UIManager
{
    public void Btn_Quit()
    {
        // 클릭 시 어플리케이션 종료
#if UNITY_EDITOR
        UnityEditor.EditorApplication.isPlaying = false;
#else
        UnityEngine.Application.Quit();
#endif
    }

    // 클릭 시 타이틀 씬으로 이동
    public void Btn_GoToTitle()
    {
        SceneManager.LoadScene("00_Title");
        Time.timeScale = 1;
    }

    // 클릭 시 로비 씬으로 이동
    public void Btn_GoToRobby()
    {
        SceneManager.LoadScene("01_Robby");
        Time.timeScale = 1;
    }

    //Pause와 Play 버튼에 들어가는 함수
    public void Btn_TimeScale()
    {
        if (Time.timeScale > 0)
        {
            //플레이 상태라면 일시정지
            Time.timeScale = 0;
        }
        else
        {
            //일시정지 상태라면 플레이
            Time.timeScale = 1;
        }
    }

    ////////////////////////////////////////////// 로비와 관련된 함수들 /////////////////////////////////////////////////

    //매개변수로 받은 문자열에 따라 씬 이동
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
                Debug.Log("스테이지 선택");
                break;
        }
    }


    //간단한 텍스트가 보여지는걸 관리하는 함수
    //Text_Display(텍스트 오브젝트 변수, GameManager안에 있는 변수)
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
                Debug.Log("지정된 케이스가 아님");
                break;
        }
    }

    //ExpBarDisplay(슬라이더 변수, GameManager안에 있는 doro_Exp, GameManager안에 있는 doro_ExpMax)
    public void ExpBarDisplay(Slider expbar, double exp, double expMax)
    {
        expbar.value = (float)(exp / expMax);
    }

    //버튼을 누르면 그 버튼에 해당하는 스탯을 올리는 함수
    public void Btn_StatUp(Button btnGo)
    {
        //sp가 0보다 클 때만 작동
        if (GameManager.gm.doro_Sp > 0)
        {
            switch (btnGo.name)
            {
                case "UP_ATK":
                    //도로롱의 공격력 증가
                    GameManager.gm.doro_Atk++;
                    break;
                case "UP_HP":
                    //최대 HP 증가
                    GameManager.gm.doro_HpMax += 5;
                    break;
                case "UP_Dodge":
                    //회피율 증가
                    GameManager.gm.doro_Dodge_Origin++; 
                    break;
                case "UP_Accuracy":
                    //명중률 증가
                    GameManager.gm.doro_Accuracy++; 
                    break;
                default:
                    Debug.Log("지정된 케이스가 아님");
                    break;
            }
            //스탯포인트 감소
            GameManager.gm.doro_Sp--;
        }
    }

    public void Camera_Select_View(GameObject doro, TMP_Dropdown viewMode)
    {
        //화면고정이 선택되어 있을 때
        if (viewMode.value == 0)
        {
            Vector3 camPos = Camera.main.transform.position;
            camPos.x = Mathf.Clamp( doro.transform.position.x, -5.5f, 5.5f);
            camPos.y = Camera.main.transform.position.y;
            camPos.z = Camera.main.transform.position.z;
            Camera.main.transform.position = camPos;
        }

        //자유이동이 선택되어 있을 때
        else
        { 
            //마우스나 터치 입력을 사용해 카메라를 유저가 직접 이동
            float moveSpeed = -0.1f; //카메라 이동 속도
            Vector3 camPos = Camera.main.transform.position;

            // 배경 이미지의 위치와 크기 가져오기

            //마우스 오른쪽 버튼을 누를 때
            if (Input.GetMouseButton(1))
            {
                float mouseX = Input.GetAxis("Mouse X");

                //camPos.x += mouseX * moveSpeed;

                // 새로운 카메라 위치 계산 및 이동 범위 제한
                camPos.x = Mathf.Clamp(camPos.x + mouseX * moveSpeed, -5.5f, 5.5f);
                //Mathf.Clamp(Value, min, max) 함수는 Value 값이
                //min보다 작아지면 min으로 대체되고 max보다 높아지면 max로 대체, 그 외의 경우에는 value값이 유지

                Camera.main.transform.position = camPos;
            }
        }
    }

    /////////////////////////////////////////// 스테이지와 관련된 함수들 //////////////////////////////////////////////
    
    //현재 HP를 백분율로 보여주는 합수
    public void HpBarDisplay(Slider hpbar, TextMeshProUGUI text, double hp, double hpMax)
    {
        //hpbar의 value는 0에서 1사이의 실수이므로 현재 hp를 최대hp로 나눠서 0에서 1사이의 실수가 나오도록 해줌
        hpbar.value = (float)(hp / hpMax);

        //이름에 따라 hpbar에 표시되는 텍스트가 달라짐
        if (hpbar.name == "DoroHP")
        {
            text.text = "[ Lv " + GameManager.gm.doro_Level + " 도로롱 ] " + (hpbar.value * 100).ToString("f2") + "%";
        }

        if (hpbar.name == "DrakeHP")
        {
            text.text = "[ Lv " + GameManager.gm.drake_Level + " 드레이크 ] " + (hpbar.value * 100).ToString("f2") + "%";
        }
        else if (hpbar.name == "MaxwellHP")
        {
            text.text = "[ Lv " + GameManager.gm.maxwell_Level + " 맥스웰 ] " + (hpbar.value * 100).ToString("f2") + "%";
        }
        else if (hpbar.name == "LaplaceHP")
        {
            text.text = "[ Lv " + GameManager.gm.laplace_Level + " 라플라스 ] " + (hpbar.value * 100).ToString("f2") + "%";
        }        

        //value가 0이되었을때 fill area가 조금 남아있었는데, 0이 되면 남아있는거로 보이지 않게 Fill  Area를 비활성화
        if (hpbar.value <= 0)
        {
            hpbar.transform.Find("Fill Area").gameObject.SetActive(false);
        }
        else
        {
            hpbar.transform.Find("Fill Area").gameObject.SetActive(true);
        }
    }

    //전투 종료시 전투 결과창을 표시해주는 함수
    public void End_UI_Window(GameObject window, TextMeshProUGUI endTxt, string result, TextMeshProUGUI mold, string getmold)
    {
        // window = 결과창(비활성화) / endTxt = 결과가 표시될 텍스트 / result = 결과값(승리, 패배) / mold = 획득한 몰드가 표시될 텍스트 / getmold = 획득한 몰드갯수
        window.SetActive(true);

        endTxt.text = result;
        mold.text = getmold;
    }
}
