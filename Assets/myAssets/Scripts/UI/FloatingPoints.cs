using UnityEngine;
using UnityEngine.UI;
using System.Collections;

public class FloatingPoints : MonoBehaviour {

    public Text damagePoints;
    public Text healingPoints;
    public Text armorPoints;
    public Text rushPoints;
    public Text invisiblePoints;
    public Image killImage;

    private Animator damageAnim;
    private Animator healingAnim;
    private Animator armorAnim;
    private Animator rushAnim;
    private Animator invisibleAnim;
    private Animator killAnim;
        
    void Start () {
        damageAnim = damagePoints.GetComponent<Animator>();
        damagePoints = damagePoints.GetComponent<Text>();

        healingAnim = healingPoints.GetComponent<Animator>();
        healingPoints = healingPoints.GetComponent<Text>();

        armorAnim = armorPoints.GetComponent<Animator>();
        armorPoints = armorPoints.GetComponent<Text>();

        rushAnim = rushPoints.GetComponent<Animator>();
        rushPoints = rushPoints.GetComponent<Text>();

        invisibleAnim = invisiblePoints.GetComponent<Animator>();
        invisiblePoints = invisiblePoints.GetComponent<Text>();

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

    public void startRushAnimation(float amount)
    {
        rushAnim.SetTrigger("damage");
        rushPoints.text = "+ " + (int)amount + " Second \n Super Rush";
    }

    public void startInvisibleAnimation(float amount)
    {
        invisibleAnim.SetTrigger("damage");
        invisiblePoints.text = "+ " + (int)amount + " Second \n Invisibility";
    }


    public void startKillAnimation()
    {
        killAnim.SetTrigger("kill");
    }
}
