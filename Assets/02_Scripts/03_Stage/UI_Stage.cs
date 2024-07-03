using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;
using UnityEngine.SceneManagement;
using TMPro;

public class UI_Stage : MonoBehaviour
{
    public Button btn_Home, btn_Pause, btn_Play, btn_End;
    public Slider slider_Metis, slider_Doro;
    public TextMeshProUGUI doro_HpTxt, metis_HpTxt, timer, endtxt, moldtxt;
    public GameObject end_Window;
    double time;

    void Start()
    {
        btn_Home.onClick.AddListener(GameManager.gm.um.Btn_GoToRobby);
        btn_Pause.onClick.AddListener(GameManager.gm.um.Btn_TimeScale);
        btn_Play.onClick.AddListener(GameManager.gm.um.Btn_TimeScale);
        btn_End.onClick.AddListener(GameManager.gm.um.Btn_GoToRobby);

        time = GameManager.gm.time * 0.2f;
    }

    
    void Update()
    {
        GameManager.gm.um.Text_Display(timer, GameManager.gm.time -= Time.deltaTime );
        if (GameManager.gm.time <= time)
        {
            timer.color = Color.red;
        }

        GameManager.gm.um.HpBarDisplay(slider_Doro, doro_HpTxt, GameManager.gm.doro_Hp, GameManager.gm.doro_HpMax);

        if (slider_Metis.name == "DrakeHP")
        {
            GameManager.gm.um.HpBarDisplay(slider_Metis, metis_HpTxt, GameManager.gm.drake_Hp, GameManager.gm.drake_HpMax);
        }

        else if (slider_Metis.name == "MaxwellHP")
        {
            GameManager.gm.um.HpBarDisplay(slider_Metis, metis_HpTxt, GameManager.gm.maxwell_Hp, GameManager.gm.maxwell_HpMax);
        }

        else
        {
            GameManager.gm.um.HpBarDisplay(slider_Metis, metis_HpTxt, GameManager.gm.laplace_Hp, GameManager.gm.laplace_HpMax);
        }

        GameManager.gm.EndBattle(slider_Doro, slider_Metis, end_Window, endtxt, moldtxt);
    }
}
