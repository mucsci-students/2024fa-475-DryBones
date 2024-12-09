using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Scripted1 : scriptedMoves
{
    public bool isFacing = false;
    public GameObject toFace;
    public GameObject toMoveTo;
    public GameObject player;
    public GameObject platform;

    void Start(){
        player = GameObject.Find("FPSController");
        toFace = GameObject.Find("platform");
        toMoveTo = toFace;
    }
    
    void Update()
    {
        if(called == true){
           if(isFacing == false){
                player.transform.LookAt(toFace.transform);
                if(player.transform.hasChanged == false){
                    isFacing = true;
                    if(toFace != toMoveTo && isFacing == true){
                        called = false;
                    }
                }
            }else{
                while(player.transform.position != toMoveTo.transform.position){
                    player.transform.Translate(Vector3.forward * Time.deltaTime);
                }
                if(player.transform.position == toMoveTo.transform.position){
                    isFacing = false;
                    toFace = GameObject.Find("Scientist");

                }
            } 
        }
        
    }
}
