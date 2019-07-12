using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.UI;
using System;

public class TextController : MonoBehaviour 
{	
	public GameObject selifPrefab;
	public GameObject containerSelif;
	//public string[] scenarios;

	[SerializeField][Range(0.001f, 0.3f)]
	float intervalForCharacterDisplay = 0.05f;	// 1文字の表示にかかる時間

	private Text uiText; //文字を表示するテキスト
	private string currentText = string.Empty;	// 現在の文字列
	private float timeUntilDisplay = 0;	// 表示にかかる時間
	private float timeElapsed = 1;	// 文字列の表示を開始した時間
	private int currentLine = 0;
	private int lastUpdateCharacter = -1;	// 表示中の文字数
	private List<SelifInformation> selifInformationList = new List<SelifInformation>();
	private bool canDisplayText = false; //文字を表示していいかどうか

	private float topMargin_txtSelif = 124.75f; //セリフテキストのマージン
	private float bottomMargin_txtSelif = 21.15f;

	// 文字の表示が完了しているかどうか
	public bool IsCompleteDisplayText 
	{
		get { return  Time.time > timeElapsed + timeUntilDisplay; }
	}

	void Start()
	{	
		SetSelifData();
		SetNextLine();
	}

	void Update () 
	{	
		if(!canDisplayText)return;

		// 文字の表示が完了してるならクリック時に次の行を表示する
		if( IsCompleteDisplayText ){
			if(currentLine < selifInformationList.Count && Input.GetMouseButtonDown(0)){
				canDisplayText = false;
				SetNextLine();
			}
		}else{
		// 完了してないなら文字をすべて表示する
			if(Input.GetMouseButtonDown(0)){
				timeUntilDisplay = 0;
			}
		}

		int displayCharacterCount = (int)(Mathf.Clamp01((Time.time - timeElapsed) / timeUntilDisplay) * currentText.Length);
		if( displayCharacterCount != lastUpdateCharacter ){
			uiText.text = currentText.Substring(0, displayCharacterCount);
			lastUpdateCharacter = displayCharacterCount;
		}
	}


	void SetNextLine()
	{	
		SelifInformation selifInfo = selifInformationList[currentLine];

		//プレハブの生成
		GameObject selifObj = (GameObject)Instantiate(selifPrefab);
		selifObj.transform.SetParent(containerSelif.transform);
		selifObj.transform.SetSiblingIndex(0); //一番上に持ってくる
		selifObj.transform.localScale = new Vector3(1,1,1);
		//矢印
		GameObject leftArrow = selifObj.transform.GetChild(0).gameObject;
		GameObject rightArrow = selifObj.transform.GetChild(1).gameObject;
		if(selifInfo.speakerPosition == "left"){
			//左だったら左を表示して右を非表示
			leftArrow.SetActive(true);
			rightArrow.SetActive(false);
		}else{
			//右だったらその逆
			leftArrow.SetActive(false);
			rightArrow.SetActive(true);
		}
		//名前
		GameObject txt_speakerName = selifObj.transform.GetChild(2).GetChild(0).gameObject;
		txt_speakerName.GetComponent<Text>().text = selifInfo.speakerName;
		//セリフテキスト
		string selifStr = selifInfo.selif;
		string[] del = {"@n"}; //改行するときに使用する文字列
		string[] arr =  selifStr.Split(del, StringSplitOptions.None); //セリフを改行文字で分割
		currentText = "";
		for(int i=0;i<arr.Length;i++){
			currentText += arr[i];
			if(i!=arr.Length-1)currentText += "\n";
		}
		uiText = selifObj.transform.GetChild(3).gameObject.GetComponent<Text>();
		//改行数に合わせてボックスの高さを変更
		Vector2 nowSizeDelta = selifObj.GetComponent<RectTransform>().sizeDelta;
		uiText.text = currentText;
		float preferredHeight = uiText.preferredHeight;
		selifObj.GetComponent<RectTransform>().sizeDelta = new Vector2(nowSizeDelta.x,
			topMargin_txtSelif+preferredHeight+bottomMargin_txtSelif);
		uiText.text = "";
		//その他の情報
		timeUntilDisplay = currentText.Length * intervalForCharacterDisplay;
		timeElapsed = Time.time;
		currentLine ++;
		lastUpdateCharacter = -1;

		StartCoroutine(DelayMethod(0.2f,() => {
			canDisplayText = true;
		}));

	}

	private void SetSelifData(){
		SelifInformation slif1 = new SelifInformation(
			"ライバル",
			"right",
			"今日もどっちがたくさんモンスター@nと仲良くなれるか勝負な！"
		);
		selifInformationList.Add(slif1);

		SelifInformation slif2 = new SelifInformation(
			"ライバル",
			"right",
			"うわー！"
		);
		selifInformationList.Add(slif2);

		SelifInformation slif3 = new SelifInformation(
			"主人公",
			"left",
			"！？"
		);
		selifInformationList.Add(slif3);

		SelifInformation slif4 = new SelifInformation(
			"ライバル",
			"right",
			"なんかあのモンスターが急にっ！@n仲良くなったモンスターに@n手伝ってもらおう！"
		);
		selifInformationList.Add(slif4);
	}

	//ディレイメソッド
	private IEnumerator DelayMethod(float waitTime, Action action)
	{
		yield return new WaitForSeconds(waitTime);
		action();
	}
}