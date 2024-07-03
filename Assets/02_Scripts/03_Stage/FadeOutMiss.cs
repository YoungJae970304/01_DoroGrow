using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;

public class FadeOutMiss : MonoBehaviour
{
    Vector3 point;
    TextMeshProUGUI txt;

    void Start()
    {
        txt = GetComponentInChildren<TextMeshProUGUI>();
    }


    void Update()
    {
        point = Vector3.up;

        transform.Translate(point * Time.deltaTime);


        // ∆‰¿ÃµÂ æ∆øÙ
        txt.color = new Vector4(txt.color.r, txt.color.g, txt.color.b, txt.color.a - 0.05f);

        if (txt.color.a <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
