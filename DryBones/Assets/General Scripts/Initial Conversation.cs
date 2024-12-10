using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor;
using UnityStandardAssets.Characters.FirstPerson;

public class InitialConversation : MonoBehaviour
{
    public NPCConversation startingConvo;

    public FirstPersonController fpsScript;

    public void Start(){
        ConversationManager.Instance.StartConversation(startingConvo);
        
        fpsScript = GameObject.Find("FPSController").GetComponent<FirstPersonController>();

        fpsScript.enabled = false;
    }

    public void Update(){
        
        if(ConversationManager.Instance.IsConversationActive){
            

            GameObject.Find("ThirdPersonCharacter").transform.LookAt(GameObject.Find("Scientist").transform);
            if (Input.GetKeyDown(KeyCode.Return) && ConversationManager.Instance.GetInt("triggerNum") != 2)
                ConversationManager.Instance.PressSelectedOption();

            if(ConversationManager.Instance.GetInt("triggerNum") == 1 && ConversationManager.Instance.GetBool("trigger") == true){
                GameObject.Find("on switch handle 1").GetComponent<MeshRenderer>().enabled = false;
                GameObject.Find("on switch handle 2").GetComponent<MeshRenderer>().enabled = true;

                GameObject.Find("on switch ball 1").GetComponent<MeshRenderer>().enabled = false;
                GameObject.Find("on switch ball 2").GetComponent<MeshRenderer>().enabled = true;
            }

            if(ConversationManager.Instance.GetInt("triggerNum") == 2 && ConversationManager.Instance.GetBool("trigger") == true){
                ConversationManager.Instance.SetBool("trigger", false);
                Invoke("triggerWorldSwap", 3f);
            }
        }
    }

    private void triggerWorldSwap(){
        if(ConversationManager.Instance.IsConversationActive){
            ConversationManager.Instance.EndConversation();
        }
        
        //implement world switch later
    }
        
}
