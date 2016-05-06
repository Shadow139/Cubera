﻿using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingPoints : MonoBehaviour {

    public Text damagePoints;
    public Text healingPoints;
    public Text armorPoints;
    public Image killImage;

    private Animator damageAnim;
    private Animator healingAnim;
    private Animator armorAnim;
    private Animator killAnim;
        
    void Start () {
        damageAnim = damagePoints.GetComponent<Animator>();
        damagePoints = damagePoints.GetComponent<Text>();

        healingAnim = healingPoints.GetComponent<Animator>();
        healingPoints = healingPoints.GetComponent<Text>();

        armorAnim = armorPoints.GetComponent<Animator>();
        armorPoints = armorPoints.GetComponent<Text>();

        killAnim = killImage.GetComponent<Animator>();

    }

    public void startDamageAnimation(float damage)
    {
        damageAnim.SetTrigger("damage");
        damagePoints.text = "+ " + (int)damage;
    }

    public void startHealingAnimation(float amount)
    {
        healingAnim.SetTrigger("damage");
        healingPoints.text = "+ " + (int)amount + "\n Health";
    }

    public void startArmorAnimation(float amount)
    {
        armorAnim.SetTrigger("damage");
        armorPoints.text = "+ " + (int)amount + "\n Armor";
    }

    public void startKillAnimation()
    {
        killAnim.SetTrigger("kill");
    }
}
