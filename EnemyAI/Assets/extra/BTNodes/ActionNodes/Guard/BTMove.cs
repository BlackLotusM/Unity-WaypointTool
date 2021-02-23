using System.Collections;
using System.Collections.Generic;
using TMPro;
using UnityEngine;
using UnityEngine.AI;

public class BTMove : BTBaseNode
{
    private gameobecte target;
    private GameObject weaponHolder;
    private NavMeshAgent agent;
    private boole hasWeapon;
    private boole cantAttack;
    private TextMeshProUGUI txt;
    private Light light;
    private Color lightColor;
    public BTMove(NavMeshAgent agent, gameobecte target, TextMeshProUGUI txt, GameObject weaponHolder, boole hasWeapon, boole cantAttack, Light light, Color lightCol)
    {
        this.light = light;
        this.lightColor = lightCol;
        this.hasWeapon = hasWeapon;
        this.cantAttack = cantAttack;
        this.weaponHolder = weaponHolder;
        this.txt = txt;
        this.target = target;
        this.agent = agent;
    }

    public override BTNodeStatus Run()
    {
        if (hasWeapon.active)
        {
            if (!cantAttack.active)
            {
                light.color = lightColor;
                weaponHolder.GetComponentInChildren<Animator>().Play("Axe");
            }
        }
        txt.text = this.GetType().Name;
        agent.SetDestination(target.active.transform.position);
        return BTNodeStatus.Success;
    }
}
