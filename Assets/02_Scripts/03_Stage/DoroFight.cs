using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;

public class DoroFight : MonoBehaviour
{
    public GameObject missTxt, metis;
    Animator anim;
    public bool isAttacking = false;

    private void Awake()
    {
        anim = GetComponent<Animator>();
    }

    void Update()
    {
        GameManager.gm.AttackMotion_Doro(this.gameObject);

        bool attackState = anim.GetBool("isAttack");

        if (attackState)
        {
            GameManager.gm.doro_Dodge = 0;
            isAttacking = true;
        }
        else
        {
            GameManager.gm.doro_Dodge = GameManager.gm.doro_Dodge_Origin;
            isAttacking = false;
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        GameManager.gm.Atk_Doro(other, missTxt, this);
    }
}
