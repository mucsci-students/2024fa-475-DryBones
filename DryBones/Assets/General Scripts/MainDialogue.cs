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


        if(currentSize != scale.minScale){
            if(currentSize == -11){
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 1 collected").GetComponent<NPCConversation>());
            }else if(currentSize == -12){
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 2 collected").GetComponent<NPCConversation>());
            }else if(currentSize == -13){
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 3 collected").GetComponent<NPCConversation>());
            }else{
                ConversationManager.Instance.StartConversation(GameObject.Find("shrink orb 4 collected").GetComponent<NPCConversation>());
            }
        }

    }


    private void triggerWorldSwap(){
        if(ConversationManager.Instance.IsConversationActive){
            ConversationManager.Instance.EndConversation();
        }
        
        SceneManager.LoadScene("", LoadSceneMode.Single);
    }
}
