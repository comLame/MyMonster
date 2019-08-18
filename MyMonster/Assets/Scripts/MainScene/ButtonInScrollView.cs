using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using UnityEngine.EventSystems;
using System;

public class ButtonInScrollView : MonoBehaviour,IPointerClickHandler
{
    public Action onclick= null;

    public void OnPointerClick(PointerEventData eventData){
        if(!(onclick == null)){
            onclick();
        }
    }

}
