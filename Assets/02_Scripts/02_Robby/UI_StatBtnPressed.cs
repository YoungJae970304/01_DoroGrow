using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UI_StatBtnPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Button button; // UnityEditor에서 연결할 버튼
    float repeatRate = 0.05f; // 반복 호출 간격

    [SerializeField]    //필드가 사용되지 않아서 뜨는 경고 발생이 거슬려서 사용
    bool isPressed;

    void Start()
    {
        isPressed = false;
        button = GetComponent<Button>();

        // 버튼의 이벤트 리스너를 설정
        button.onClick.AddListener(OnButtonHold);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        InvokeRepeating(nameof(OnButtonHold), 0.5f, repeatRate); // 버튼을 꾹 누르고 있을 때 함수를 반복적으로 호출합니다.
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        CancelInvoke(nameof(OnButtonHold)); // 버튼을 뗄 때 호출을 중지합니다.
    }

    private void OnButtonHold()
    {
        // 버튼을 꾹 누르고 있는 동안 호출되는 함수
        GameManager.gm.um.Btn_StatUp(button);
    }
}
