using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DialogueEditor;
using UnityEngine.SceneManagement;

public class TitleConversation : MonoBehaviour
{
    public NPCConversation skip;

    private bool convoStarted;
    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        if(ConversationManager.Instance.IsConversationActive){
            if(Input.GetKeyDown(KeyCode.Return))
                ConversationManager.Instance.PressSelectedOption();

            if(Input.GetKeyDown(KeyCode.DownArrow) || Input.GetKeyDown(KeyCode.S))
                ConversationManager.Instance.SelectNextOption();

            if(Input.GetKeyDown(KeyCode.UpArrow) || Input.GetKeyDown(KeyCode.W))
                ConversationManager.Instance.SelectPreviousOption();

        }else{
            if(convoStarted){
                if(ConversationManager.Instance.GetBool("finish")){
                Invoke("triggerWorldSwap", 4f);
                }
            }
            
        }
    }

    public void startConvo(){
        ConversationManager.Instance.StartConversation(skip);
        convoStarted = true;
    }

    private void triggerWorldSwap(){
        if(ConversationManager.Instance.GetBool("skipTutorial")){
            PlayerPrefs.SetInt("skipTutorial" , 1);
        }
        if(ConversationManager.Instance.GetBool("skipDialogue")){
            PlayerPrefs.SetInt("skipDialogue", 1);
            SceneManager.LoadScene("Main", LoadSceneMode.Single);
        }else{
            SceneManager.LoadScene("JoshTest", LoadSceneMode.Single);
        }
        

        
    }
}
