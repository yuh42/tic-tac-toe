using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIManager : MonoSingleton<UIManager>
{
    // Start is called before the first frame update
    public GameObject State;
    public Text[] Score=new Text[2];
    public GameObject Player;
    int[] score=new int[2];
    public void PlayResult(int player,bool bDraw){
        State.gameObject.SetActive(true);

        if(!bDraw){
            State.transform.GetChild(player).gameObject.SetActive(true);
            score[player]++;
            Score[player].text=score[player].ToString(); 
        }
        else{
            State.transform.GetChild(2).gameObject.SetActive(true);
        }
        SoundManager.Instance.PlaySound(4);
    }

    public void ResetUI(bool bClear){
        for(int i=0;i<3;i++){
            State.transform.GetChild(i).gameObject.SetActive(false);
        }
        State.gameObject.SetActive(false);
        if(bClear){
            for(int i=0;i<2;i++){
                score[i]=0;
            }
        }
    }

    public void SetPlayer(int player){
       if(player==0){
        Player.transform.GetChild(0).gameObject.SetActive(true);
        Player.transform.GetChild(1).gameObject.SetActive(false);
       }else{
        Player.transform.GetChild(1).gameObject.SetActive(true);
        Player.transform.GetChild(0).gameObject.SetActive(false);
       }
    }

}
