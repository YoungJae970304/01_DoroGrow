using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UI_Title : MonoBehaviour
{
    public Button btn_Start, btn_Quit_Title, btn_Guide;

    void Start()
    {
        btn_Start.onClick.AddListener(GameManager.gm.um.Btn_GoToRobby);
        btn_Quit_Title.onClick.AddListener(GameManager.gm.um.Btn_Quit);
    }


    void Update()
    {
        
    }
}
