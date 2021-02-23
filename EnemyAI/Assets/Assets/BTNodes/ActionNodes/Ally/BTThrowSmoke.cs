using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class BTThrowSmoke : BTBaseNode
{
    private GameObject smoke;
    private GameObject allyPos;
    private boole underAttack;
    private boole hasAttacked;
    public BTThrowSmoke(GameObject smoke, GameObject allyPos, boole underAttack, boole hasAttacked)
    {
        this.smoke = smoke;
        this.allyPos = allyPos;
        this.underAttack = underAttack;
        this.hasAttacked = hasAttacked;
    }
    public override BTNodeStatus Run()
    {
        GameObject t = GameObject.Instantiate(smoke, allyPos.transform.position, Quaternion.identity);
        t.GetComponent<Rigidbody>().velocity = (t.transform.up * 50);
        underAttack.active = false;
        hasAttacked.active = true;
        return BTNodeStatus.Success;
    }
}
