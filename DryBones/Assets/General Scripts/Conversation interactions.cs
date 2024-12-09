using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Events;
using DialogueEditor;

public class Conversationinteractions : MonoBehaviour
{
    int currentConvo = 0;

    public NPCConversation startingConvo;

    scriptedMoves [] scripts;

    public void Awake(){
        scripts = new scriptedMoves[] {gameObject.GetComponent<Scripted1>(), gameObject.GetComponent<Scripted2>()};
    }

    public void Start(){
        ConversationManager.Instance.StartConversation(startingConvo);
    }

    public void triggerScripts(){
        scripts[currentConvo].called = true;
        currentConvo++;
    }
}
