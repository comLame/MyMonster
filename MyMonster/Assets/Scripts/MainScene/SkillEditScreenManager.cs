using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SkillEditScreenManager : MonoBehaviour {

	public SkillData skillData;
	public LearnSkillData learnSkillData;
	public BaseStatsData baseStatsData;
	public Monster mons;
	public GameObject containerMonsterInfo;
	public GameObject txt_Name;
	public GameObject txt_Level;
	public GameObject img_Monster;
	public GameObject txt_Hp;
	public GameObject txt_Attack;
	public GameObject txt_Defense;
	public GameObject txt_Speed;
	public GameObject containerSkill;
	public GameObject containerSkillBtn;
	public GameObject skillBtnPrefab;


	public void Display(){
		//モンスター情報の表示
		//変数宣言
		int No = mons.No;
		BaseStatsData.Param baseStats = baseStatsData.sheets[0].list[No-1];
		Sprite monsterSprite = Resources.Load<Sprite>("Img_Monster/" + No);;
		string name = baseStats.Name;
		int level = mons.level;
		int hp = baseStats.Hp;
		int attack = baseStats.Attack;
		int defense = baseStats.Defense;
		int speed = baseStats.Speed;
		//変数代入
		img_Monster.GetComponent<Image>().sprite = monsterSprite;
		txt_Name.GetComponent<Text>().text = name;
		txt_Level.GetComponent<Text>().text = "LV:" + level.ToString() + "/100";
		txt_Hp.GetComponent<Text>().text = hp.ToString();
		txt_Attack.GetComponent<Text>().text = attack.ToString();
		txt_Defense.GetComponent<Text>().text = defense.ToString();
		txt_Speed.GetComponent<Text>().text = speed.ToString();
		//おぼえているスキルの表示
		for(int i=0;i<4;i++){
			int skillNum = mons.skills[i];
			GameObject skillBtn = containerSkill.transform.GetChild(i).gameObject;
			GameObject skillBtnImage = skillBtn.transform.GetChild(1).gameObject;
			GameObject skillBtnFrame = skillBtn.transform.GetChild(0).gameObject;
			GameObject skillTxt = skillBtn.transform.GetChild(2).gameObject;
			string type = skillData.sheets[0].list[skillNum-1].Type;
			string skillName = skillData.sheets[0].list[skillNum-1].Name;
			string explain = skillData.sheets[0].list[skillNum-1].Explain;

			skillBtnImage.GetComponent<Image>().color = GetTypeColor(type);
			skillTxt.GetComponent<Text>().text = skillName;

		}

		//スキルの表示
		int totalCount = learnSkillData.sheets[No-1].list.Count;
		for(int i=0;i<totalCount;i++){
			int skillNum = learnSkillData.sheets[No-1].list[i].SkillNum;
			int learnLevel = learnSkillData.sheets[No-1].list[i].Level;
			string type = skillData.sheets[0].list[skillNum-1].Type;
			string skillName = skillData.sheets[0].list[skillNum-1].Name;
			string explain = skillData.sheets[0].list[skillNum-1].Explain;

			GameObject skillBtn = (GameObject)Instantiate(skillBtnPrefab);
			skillBtn.transform.SetParent(containerSkillBtn.transform);
			skillBtn.GetComponent<RectTransform>().localScale = new Vector3(1,1,1);
			GameObject frame = skillBtn.transform.GetChild(0).gameObject;
			GameObject bg = skillBtn.transform.GetChild(1).gameObject;
			GameObject txt_learnLevel = skillBtn.transform.GetChild(2).gameObject;
			GameObject txt_skillName = skillBtn.transform.GetChild(3).gameObject;
			GameObject black = skillBtn.transform.GetChild(4).gameObject;

			bg.GetComponent<Image>().color = GetTypeColor(type);
			txt_learnLevel.GetComponent<Text>().text = "Lv. " + learnLevel.ToString();
			txt_skillName.GetComponent<Text>().text = skillName;
			if(learnLevel > level)black.SetActive(true);

		}
	}

	//属性カラー取得
	private Color GetTypeColor(string type){
		switch(type){
		case "Normal":
			return new Color(0.736f,0.736f,0.736f);
		case "Fire":
			return new Color(0.925f,0.280f,0.214f);
		case "Water":
			return new Color(0.212f,0.368f,0.925f);
		case "Grass":
			return new Color(0.338f,0.868f,0.168f);
		case "Lightning":
			return new Color(0.900f,0.915f,0.324f);
		case "Darkness":
			return new Color(0.858f,0.150f,0.722f);
		default:
			return new Color(0.736f,0.736f,0.736f);
		}
	}
}
