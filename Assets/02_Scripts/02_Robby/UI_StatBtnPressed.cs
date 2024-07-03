using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using UnityEngine.EventSystems;


public class UI_StatBtnPressed : MonoBehaviour, IPointerDownHandler, IPointerUpHandler
{
    Button button; // UnityEditor���� ������ ��ư
    float repeatRate = 0.05f; // �ݺ� ȣ�� ����

    [SerializeField]    //�ʵ尡 ������ �ʾƼ� �ߴ� ��� �߻��� �Ž����� ���
    bool isPressed;

    void Start()
    {
        isPressed = false;
        button = GetComponent<Button>();

        // ��ư�� �̺�Ʈ �����ʸ� ����
        button.onClick.AddListener(OnButtonHold);
    }

    public void OnPointerDown(PointerEventData eventData)
    {
        isPressed = true;
        InvokeRepeating(nameof(OnButtonHold), 0.5f, repeatRate); // ��ư�� �� ������ ���� �� �Լ��� �ݺ������� ȣ���մϴ�.
    }

    public void OnPointerUp(PointerEventData eventData)
    {
        isPressed = false;
        CancelInvoke(nameof(OnButtonHold)); // ��ư�� �� �� ȣ���� �����մϴ�.
    }

    private void OnButtonHold()
    {
        // ��ư�� �� ������ �ִ� ���� ȣ��Ǵ� �Լ�
        GameManager.gm.um.Btn_StatUp(button);
    }
}
