using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TabInitializeManager : MonoBehaviour
{   
    //Home
    public GameObject home_tempView;
    public GameObject home_topScreen;
    public GameObject home_questSelectScreen;

    //Monster
    public GameObject monster_topScreen;
    public GameObject monster_box;
    public GameObject monster_menuScreen;
    public GameObject monster_editScreen;
    public GameObject monster_trainingScreen;
    public GameObject monster_skillEditScreen;
    public GameObject monster_sellScreen;

    private void ChangeState(GameObject screen,bool active){
        screen.SetActive(active);
        if(screen.GetComponent<CanvasGroup>()){
            screen.GetComponent<CanvasGroup>().alpha = 1;
        }
    }

    //ホームタブの初期化
    public void HomeTab(){
        ChangeState(home_tempView,false);
        ChangeState(home_topScreen,true);
        ChangeState(home_questSelectScreen,false);
    }

    //モンスタータブの初期化
    public void MonsterTab(){
        ChangeState(monster_topScreen,false);
        ChangeState(monster_box,true);
        ChangeState(monster_menuScreen,true);
        ChangeState(monster_editScreen,false);
        ChangeState(monster_trainingScreen,false);
        ChangeState(monster_skillEditScreen,false);
        ChangeState(monster_sellScreen,false);

        
    }
}
