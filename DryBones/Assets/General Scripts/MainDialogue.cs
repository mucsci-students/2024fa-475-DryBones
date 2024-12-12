using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using DialogueEditor;

public class MainDialogue : MonoBehaviour
{

    public NPCConversation worldIntro;
    public NPCConversation introTutorial;
    public NPCConversation shopTutorial;
    public NPCConversation collected1Orb;
    public NPCConversation collected2Orb;
    public NPCConversation collected3Orb;
    public NPCConversation collected4Orb;
    public NPCConversation collectedTOrb;
    public NPCConversation DJ;
    public NPCConversation sprint;
    public NPCConversation wallRun;
    public NPCConversation dash;
    public NPCConversation skip;
    public NPCConversation returnOptions;

    bool hasFinished = false;

    int currentSize = -1;

    PlayerShrink scale;

    public ThirdPersonController fpsScript;

    bool dialogueStarted = false;

    bool startTutorial = false;
    bool tutorialStarted = false;

    bool noCoins = true;

    bool firstCoin = false;

    bool skipping = false;

    bool skipDialogue = false;

    bool skipTutorial = false;

    bool sprintNotBought = true;

    bool DJNotBought = true;

    bool dashNotBought = true;

    bool wallRunNotBought = true; 

    void Start(){
            fpsScript = GameObject.Find("Player").GetComponent<ThirdPersonController>();
                if(PlayerPrefs.GetInt("skipTutorial") == 1)
                    skipTutorial = true;
        
            
                if(PlayerPrefs.GetInt("skipDialogue") == 1)
                    skipDialogue = true;

            dialogueStarted = true;
            ConversationManager.Instance.StartConversation(worldIntro);

        
    }

    // Update is called once per frame
    void Update()
    {
        if(ConversationManager.Instance != null && SceneManager.GetActiveScene().name == "Main"){
            if(ConversationManager.Instance.IsConversationActive){
                fpsScript.enabled = false;
            }else{
                fpsScript.enabled = true;
            }


            if(hasFinished){
                if(Input.GetKeyDown(KeyCode.Backspace)){
                    ConversationManager.Instance.StartConversation(returnOptions);
                }

                if(ConversationManager.Instance.GetBool("finish")){
                    Invoke("triggerCanvas", 5f);
                }
            }

            if(ConversationManager.Instance.IsConversationActive){
                if(Input.GetKeyDown(KeyCode.Return)){
                    if(ConversationManager.Instance.GetBool("finish")){
                        ConversationManager.Instance.PressSelectedOption();
                        ConversationManager.Instance.SetBool("finish", false);
                        hasFinished = true;
                        ConversationManager.Instance.StartConversation(returnOptions);
                        
                    }else{
                        ConversationManager.Instance.PressSelectedOption();
                    }
                }
                if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                    ConversationManager.Instance.SelectNextOption();

                if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                    ConversationManager.Instance.SelectPreviousOption();
            }



            if(!skipDialogue && dialogueStarted){
                if(!ConversationManager.Instance.IsConversationActive){
                    startTutorial = true;
                }
            }

            if(startTutorial && !skipTutorial && !tutorialStarted){
                tutorialStarted = true;
                ConversationManager.Instance.StartConversation(introTutorial);
                if(!ConversationManager.Instance.IsConversationActive){
                    startTutorial = false;
                }
            }

            if(PlayerCollision._coinAmount > 0 && !skipTutorial && noCoins){
                noCoins = false;
                ConversationManager.Instance.StartConversation(shopTutorial);
            }


            
            if(!skipDialogue && PlayerPrefs.HasKey("shrink") && currentSize != PlayerPrefs.GetInt("shrink")){
                currentSize = PlayerPrefs.GetInt("shrink");
                if(currentSize == 1){
                    PlayerPrefs.SetInt("hasPlayed",1);
                    ConversationManager.Instance.StartConversation(collected1Orb);
                }else if(currentSize == 2){
                    ConversationManager.Instance.StartConversation(collected2Orb);
                }else if(currentSize == 3){
                    ConversationManager.Instance.StartConversation(collected3Orb);
                }else if(currentSize == 4){
                    ConversationManager.Instance.StartConversation(collected4Orb);
                }else if(currentSize == 0){
                    ConversationManager.Instance.StartConversation(collectedTOrb);
                }
            }




            if(sprintNotBought && ButtonManager._isSprintBought && !skipTutorial){
                sprintNotBought = false;
                ConversationManager.Instance.StartConversation(sprint);
            }
            if(DJNotBought && ButtonManager._isDoubleJumpBought && !skipTutorial){
                DJNotBought = false;
                ConversationManager.Instance.StartConversation(DJ);
            }
            if(dashNotBought && ButtonManager._isDashBought && !skipTutorial){
                dashNotBought = false;
                ConversationManager.Instance.StartConversation(dash);
            }
            if(wallRunNotBought && ButtonManager._isWallRunningBought && !skipTutorial){
                wallRunNotBought = false;
                ConversationManager.Instance.StartConversation(wallRun);
            }
        }

    }


    private void triggerCanvas(){
        
        //happy birthday
    }
}
