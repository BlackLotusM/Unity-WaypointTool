using UnityEngine;
using UnityEngine.AI;

public class BTGoForWeapon : BTBaseNode
{
    private GameObject target;
    private NavMeshAgent agent;
    private boole hasWeapon;
    private boole needWeapon;
    private GameObject weaponObject;
    private GameObject weaponHolder;
    private Light light;
    private Color lightColor;
    public BTGoForWeapon(NavMeshAgent agent, GameObject target, boole hasWeapon, boole needsWeaponm, GameObject weaponHolder, GameObject weaponObject, Light light, Color lightCol)
    {
        this.light = light;
        this.lightColor = lightCol;
        this.hasWeapon = hasWeapon;
        this.needWeapon = needsWeaponm;
        this.weaponHolder = weaponHolder;
        this.weaponObject = weaponObject;
        this.target = target;
        this.agent = agent;
    }

    public override BTNodeStatus Run()
    {
        agent.SetDestination(target.transform.position);
        light.color = lightColor;
        if (agent.remainingDistance <= 1f)
        {
            hasWeapon.active = true;
            needWeapon.active = false;
            weaponObject.SetActive(false);
            weaponHolder.SetActive(true);
            return BTNodeStatus.Success;
        }
        else { return BTNodeStatus.Running; }
    }
}
