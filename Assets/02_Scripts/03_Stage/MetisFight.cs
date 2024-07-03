using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MetisFight : MonoBehaviour
{
    public GameObject dodgeTxt, doro;

    void Start()
    {
        GameManager.gm.Attack_Metis(this.gameObject, dodgeTxt, doro);

        GameManager.gm.UpdateDoroDodge(this.gameObject);
    }
}
