using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FadeOut : MonoBehaviour
{
    Vector3 point;
    SpriteRenderer sr;

    void Start()
    {
        sr = GetComponent<SpriteRenderer>();
    }


    void Update()
    {
        point = Vector3.up;

        transform.Translate(point * Time.deltaTime);

        // ∆‰¿ÃµÂ æ∆øÙ
        sr.color = new Color(sr.color.r, sr.color.g, sr.color.b, sr.color.a - 0.05f);

        if (sr.color.a <= 0)
        {
            Destroy(this.gameObject);
        }
    }
}
