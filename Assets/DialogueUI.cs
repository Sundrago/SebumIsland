using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using MyUtility;
using UnityEngine.UI;
public class DialogueUI : MonoBehaviour
{
    [SerializeField]
    private DialogueManager dialogueManager;
    
    [SerializeField]
    private Image image_left_ui, image_right_ui, name_left_ui, name_right_ui;
    [SerializeField]
    private TextMeshProUGUI name_left_text, name_right_text, dialogue_text;
    
    public void UpdateDialogueUI(DialogueManager.DialogueData data)
    {
        Image image;
        TextMeshProUGUI name;

        //Set Direction
        image_left_ui.gameObject.SetActive(data.direction == DialogueManager.Direction.Left);
        name_left_ui.gameObject.SetActive(data.direction == DialogueManager.Direction.Left);
        image_right_ui.gameObject.SetActive(data.direction == DialogueManager.Direction.Right);
        name_right_ui.gameObject.SetActive(data.direction == DialogueManager.Direction.Right);
        
        if (data.direction == DialogueManager.Direction.Left)
        {
            image = image_left_ui;
            name = name_left_text;
        }
        else
        {
            image = image_right_ui;
            name = name_right_text;
        }
        
        //Update Info
        DialogueManager.DialogueCharacter character = dialogueManager.dialogueCharacters[data.dialogueCharacterType];
        image.sprite = character.sprite;
        name.text = Localize.GetLocalizedString(character.name);
        dialogue_text.text = Localize.GetLocalizedString(data.dialogueString);

        //transition FX
        switch (data.dialogueFX)
        {
            case DialogueManager.DialogueFX.FadeIn :
                break;
        }
    }
}
