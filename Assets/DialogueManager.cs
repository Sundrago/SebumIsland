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

public class DialogueManager : SerializedMonoBehaviour
{
    // [SerializeField, TableList]
    // private List<DialogueCharacter> dialogueCharacters;
    
    [SerializeField]
    public Dictionary <DialogueCharacterType, DialogueCharacter> dialogueCharacters;

    [SerializeField]
    private DialogueUI dialogueUI;
    private List<DialogueData> dialogueDatas;
    private int currentDialogueIdx = 0;

    [Button]
    public void StartTestDialogue()
    {
        List<DialogueData> datas = new List<DialogueData>();
        
        datas.Add(new DialogueData(Direction.Left, DialogueCharacterType.Prince, "안녕?"));
        datas.Add(new DialogueData(Direction.Right, DialogueCharacterType.Princess, "안녕!?"));
        datas.Add(new DialogueData(Direction.Left, DialogueCharacterType.Princess, "왼쪽!"));
        datas.Add(new DialogueData(Direction.Right, DialogueCharacterType.Princess, "오른쪽!"));
        datas.Add(new DialogueData(Direction.Left, DialogueCharacterType.King, "으갸갸갸갹"));
        datas.Add(new DialogueData(Direction.Right, DialogueCharacterType.Hero, "난 히어로야"));
        datas.Add(new DialogueData(Direction.Left, DialogueCharacterType.Heroin, "난 히로인이야"));
        
        InitDialogue(datas);
    }
    
    private void InitDialogue(List<DialogueData> _dialogueDatas)
    {
        dialogueDatas = _dialogueDatas;
        currentDialogueIdx = 0;
        
        gameObject.SetActive(true);
        dialogueUI.UpdateDialogueUI(dialogueDatas[0]);
    }

    [Button]
    public void NextBtnClicked()
    {
        currentDialogueIdx += 1;
        if (currentDialogueIdx >= dialogueDatas.Count)
        {
            gameObject.SetActive(false);
            return;
        }
        
        dialogueUI.UpdateDialogueUI(dialogueDatas[currentDialogueIdx]);
    }
    public class DialogueData
    {
        public Direction direction;
        public DialogueCharacterType dialogueCharacterType;
        public string dialogueString;
        public DialogueFX dialogueFX;

        public DialogueData(Direction _driection, DialogueCharacterType _dialogueCharacterType, string _dialogueString, DialogueFX _dialogueFX = DialogueFX.FadeIn)
        {
            direction = _driection;
            dialogueCharacterType = _dialogueCharacterType;
            dialogueString = _dialogueString;
            dialogueFX = _dialogueFX;
        }
    }

    [Serializable]
    public class DialogueCharacter
    {
        [HorizontalGroup]
        public string name;
        [HorizontalGroup]
        public Sprite sprite;
    }
    
    public enum Direction { Left, Right }
    public enum DialogueFX { FadeIn, }
    public enum DialogueCharacterType { Princess, Prince, King, Hero, Heroin }

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
