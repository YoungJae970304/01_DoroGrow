using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DoroMetisMov : MonoBehaviour
{
    public GameObject charac;
    Rigidbody characRig;
    SpriteRenderer characSprite;

    void Start()
    {
        characRig = GetComponent<Rigidbody>();
        characSprite = GetComponent<SpriteRenderer>();

        if (charac.name == "Dororong")
        {
            GameManager.gm.StartDoroAction(charac);
        }
        else
        {
            GameManager.gm.StartMetisAction(charac);
        }
    }

    void Update()
    {
        if (characRig.velocity.x != 0)
        {
            characSprite.flipX = characRig.velocity.x > 0;
        }
    }

    private void OnCollisionEnter(Collision collision)
    {
        //GameManager.gm.ChangeDirToTrigger(collision, charac);
    }
    private void OnTriggerEnter(Collider other)
    {
        GameManager.gm.ChangeDirToTrigger(other, charac);
    }
}
