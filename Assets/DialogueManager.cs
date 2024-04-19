using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Sirenix.OdinInspector;
using UnityEngine.UI;
using DG.Tweening;
using TMPro;
using MyUtility;
using UnityEditor;
using UnityEngine.Serialization;

public enum DialogueCharacterType { Princess, Prince, King, Hero, Heroin }
public enum DialogueTransitionFX { FadeIn, }
public enum ShowDirection { Left, Right }

/// <summary>
/// Represents a character in a dialogue.
/// </summary>
[Serializable]
public class DialogueCharacter
{
    [HorizontalGroup]
    public string name;
    [HorizontalGroup]
    public Sprite sprite;
}

/// <summary>
/// Represents the data for a single dialogue in the game.
/// </summary>
public class DialogueData
{
    public ShowDirection ShowDirection;
    public DialogueCharacterType dialogueCharacterType;
    public string dialogueString;
    public DialogueTransitionFX dialogueTransitionFX;

    public DialogueData(ShowDirection _driection, DialogueCharacterType _dialogueCharacterType, string _dialogueString, DialogueTransitionFX dialogueTransitionFX = DialogueTransitionFX.FadeIn)
    {
        ShowDirection = _driection;
        dialogueCharacterType = _dialogueCharacterType;
        dialogueString = _dialogueString;
        this.dialogueTransitionFX = dialogueTransitionFX;
    }
}


/// <summary>
/// Manages overall dialogue features in game.
/// </summary>
public class DialogueManager : SerializedMonoBehaviour
{
    public Dictionary <DialogueCharacterType, DialogueCharacter> dialogueCharacters;
    [FormerlySerializedAs("dialogueUI")] [SerializeField] private DialogueUIController dialogueUIController;

    private List<DialogueData> dialogueDatas;
    private int currentDialogueIdx = 0;
    
    public void StartTestDialogue()
    {
        List<DialogueData> datas = new List<DialogueData>();
        InitDialogue(datas);
    }
    
    private void InitDialogue(List<DialogueData> _dialogueDatas)
    {
        dialogueDatas = _dialogueDatas;
        currentDialogueIdx = 0;
        
        gameObject.SetActive(true);
        dialogueUIController.UpdateDialogueUI(dialogueDatas[0]);
    }
    
    public void NextBtnClicked()
    {
        currentDialogueIdx += 1;
        if (currentDialogueIdx >= dialogueDatas.Count)
        {
            gameObject.SetActive(false);
            return;
        }
        
        dialogueUIController.UpdateDialogueUI(dialogueDatas[currentDialogueIdx]);
    }

#if UNITY_EDITOR
    [Button]
    void AddDialogueTypeToTheList()
    {
        foreach (DialogueCharacterType type in Enum.GetValues(typeof(DialogueCharacterType)))
        {
            if(!dialogueCharacters.ContainsKey(type)) dialogueCharacters.Add(type, new DialogueCharacter());
        }
    }
#endif
}