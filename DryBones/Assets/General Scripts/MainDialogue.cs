using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityStandardAssets.Characters.FirstPerson;
using UnityEngine.SceneManagement;
using DialogueEditor;

public class MainDialogue : MonoBehaviour
{
    bool hasFinished = false;

    float currentSize;

    PlayerShrink scale;

    public FirstPersonController fpsScript;

    bool startDialogue = false;
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
        fpsScript = GameObject.Find("FPSController").GetComponent<FirstPersonController>();

        scale = FindObjectOfType<PlayerShrink>();

        currentSize = scale.minScale;
    }

    // Update is called once per frame
    void Update()
    {
        if(ConversationManager.Instance.IsConversationActive){
            fpsScript.enabled = false;
        }else{
            fpsScript.enabled = true;
        }

        if(skipping){
            if(!ConversationManager.Instance.IsConversationActive){
                skipTutorial = ConversationManager.Instance.GetBool("skipTutorial");
                skipDialogue = ConversationManager.Instance.GetBool("skipDialogue");
                skipping = false;
            }
            
        }


        if(hasFinished){
            if(Input.GetKeyDown(KeyCode.Return)){
                ConversationManager.Instance.StartConversation(GameObject.Find("return options").GetComponent<NPCConversation>());
            }

            if(ConversationManager.Instance.GetBool("finish")){
                Invoke("triggerWorldSwap", 5f);
            }
        }

        if(ConversationManager.Instance.IsConversationActive){
            if(Input.GetKeyDown(KeyCode.Return)){
                if(ConversationManager.Instance.GetBool("finish")){
                    ConversationManager.Instance.PressSelectedOption();
                    ConversationManager.Instance.SetBool("finish", false);
                    hasFinished = true;
                    ConversationManager.Instance.StartConversation(GameObject.Find("return options").GetComponent<NPCConversation>());
                    
                }else{
                    ConversationManager.Instance.PressSelectedOption();
                }
            }
            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                ConversationManager.Instance.SelectNextOption();

            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                ConversationManager.Instance.SelectPreviousOption();
        }

        if(PlayerPrefs.GetInt("startDialogue") == 1 && !dialogueStarted){
            PlayerPrefs.SetInt("startDialogue", 2);
            startDialogue = true;
        }


        if(startDialogue && !skipDialogue && !dialogueStarted){
            dialogueStarted = true;
            ConversationManager.Instance.StartConversation(GameObject.Find("world intro").GetComponent<NPCConversation>());
            if(!ConversationManager.Instance.IsConversationActive){
                startTutorial = true;
                startDialogue = false;
            }
        }

        if(startTutorial && !skipTutorial && !tutorialStarted){
            tutorialStarted = false;
            ConversationManager.Instance.StartConversation(GameObject.Find("intro tutorial").GetComponent<NPCConversation>());
            if(!ConversationManager.Instance.IsConversationActive){
                startTutorial = false;
            }
        }

        if(PlayerCollision._coinAmount > 0 && !skipTutorial && noCoins){
            noCoins = false;
            ConversationManager.Instance.StartConversation(GameObject.Find("Shop tutorial").GetComponent<NPCConversation>());
        }



        if(currentSize != scale.minScale && !skipDialogue){
            if(scale.minScale == -11){
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 1 collected").GetComponent<NPCConversation>());
            }else if(scale.minScale == -12){
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 2 collected").GetComponent<NPCConversation>());
            }else if(scale.minScale == -13){
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 3 collected").GetComponent<NPCConversation>());
            }else{
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 4 collected").GetComponent<NPCConversation>());
            }
            currentSize = scale.minScale;
        }




        if(sprintNotBought && ButtonManager._isSprintBought && !skipTutorial){
            sprintNotBought = false;
            ConversationManager.Instance.StartConversation(GameObject.Find("sprintDescription").GetComponent<NPCConversation>());
        }
        if(DJNotBought && ButtonManager._isDoubleJumpBought && !skipTutorial){
            DJNotBought = false;
            ConversationManager.Instance.StartConversation(GameObject.Find("DJDescription").GetComponent<NPCConversation>());
        }
        if(dashNotBought && ButtonManager._isDashBought && !skipTutorial){
            dashNotBought = false;
            ConversationManager.Instance.StartConversation(GameObject.Find("dashDescription").GetComponent<NPCConversation>());
        }
        if(wallRunNotBought && ButtonManager._isWallRunningBought && !skipTutorial){
            wallRunNotBought = false;
            ConversationManager.Instance.StartConversation(GameObject.Find("wallRunDescription").GetComponent<NPCConversation>());
        }

    }


    private void triggerWorldSwap(){
        if(ConversationManager.Instance.IsConversationActive){
            ConversationManager.Instance.EndConversation();
        }
        
        SceneManager.LoadScene("JoshTest", LoadSceneMode.Single);
    }

    public void skipD(){
        ConversationManager.Instance.StartConversation(GameObject.Find("skip?").GetComponent<NPCConversation>());
        skipping = true;
    }
}
