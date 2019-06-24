using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TapEffect : MonoBehaviour {

	[SerializeField]　GameObject tapEffectObj;              // タップエフェクト
    //[SerializeField]　Camera _camera;                        // カメラの座標

	private ParticleSystem tapEffect;
	private Camera _camera;

	private void Start(){
		_camera = gameObject.GetComponent<Camera>();
		tapEffect = tapEffectObj.GetComponent<ParticleSystem>();
	}

    void Update()
    {
        if(Input.GetMouseButtonDown(0))
        {
            // マウスのワールド座標までパーティクルを移動し、パーティクルエフェクトを1つ生成する
            var pos = _camera.ScreenToWorldPoint(Input.mousePosition + _camera.transform.forward * 2);
            tapEffect.transform.position = pos;
            tapEffect.Emit(1);
        }
    }
}
