using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Monster : MonoBehaviour {

    public Entity_MonsterData monsterData;
    public Entity_ExpData expData;

    public int monsterId;
    public bool isEnemy;

    [System.NonSerialized] public int Hp;
    [System.NonSerialized] public int Atk;
    [System.NonSerialized] public int Def;
    [System.NonSerialized] public int Spd;
    [System.NonSerialized] public int Exp;
    [System.NonSerialized] public int Level;

    // Use this for initialization
    void Start () {
        if(!isEnemy){
            int nowExp = SaveData.GetInt("Exp",0);

            int expGroup = monsterData.sheets[0].list[monsterId].ExpGroup;
            
            switch(expGroup){
            case 60:
                for(int i=0;i<100;i++){
                    if(nowExp >= expData.sheets[0].list[i].Group60){
                        Level = i+1;
                        break;
                    }
                }
                break;
            case 80:
                for(int i=0;i<100;i++){
                    if(nowExp >= expData.sheets[0].list[i].Group80){
                        Level = i+1;    
                        break;
                    }
                }
                break;
            case 100:
                for(int i=0;i<100;i++){
                    if(nowExp >= expData.sheets[0].list[i].Group100){
                        Level = i+1;    
                        break;
                    }
                }
                break;
            case 105:
                for(int i=0;i<100;i++){
                    if(nowExp >= expData.sheets[0].list[i].Group105){
                        Level = i+1;    
                        break;
                    }
                }
                break;
            case 125:
                for(int i=0;i<100;i++){
                    if(nowExp >= expData.sheets[0].list[i].Group125){
                        Level = i+1;    
                        break;
                    }
                }
                break;
            case 160:
                for(int i=0;i<100;i++){
                    if(nowExp >= expData.sheets[0].list[i].Group160){                           
                        Level = i+1;    
                        break;
                    }
                }
                break;
            }
        }else{
            Level = 1;
        }


            Hp = monsterData.sheets[0].list[monsterId].HP;
            Atk = monsterData.sheets[0].list[monsterId].ATK;
            Def = monsterData.sheets[0].list[monsterId].DEF;
            Spd =monsterData.sheets[0].list[monsterId].SPD;
            Exp = monsterData.sheets[0].list[monsterId].EXP;

            Debug.Log("NAME:"+this.gameObject.name);
            int ExpNow = SaveData.GetInt("Exp",0);
            Debug.Log("EXP:"+ExpNow);
            Debug.Log("LEVEL:"+Level);

            transform.GetChild(2).GetComponent<Text>().text = "Lv."+Level;
            transform.GetChild(1).GetComponent<Slider>().maxValue = Hp;
            transform.GetChild(1).GetComponent<Slider>().value = Hp;
    }
}

