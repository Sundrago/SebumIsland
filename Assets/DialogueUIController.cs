using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using DG.Tweening;
using TMPro;
using MyUtility;
using UnityEngine.UI;

/// <summary>
/// Responsible for managing the user interface elements related to dialogues in a game.
/// The class updates the UI based on the data provided by the DialogueManager.
/// </summary>
public class DialogueUIController : MonoBehaviour
{
    [SerializeField] private DialogueManager dialogueManager;
    
    [SerializeField] private Image image_left_ui, image_right_ui, name_left_ui, name_right_ui;
    [SerializeField] private TextMeshProUGUI name_left_text, name_right_text, dialogue_text;
    
    public void UpdateDialogueUI(DialogueData data)
    {
        SetDirection(data);

        Image image;
        TextMeshProUGUI name;
        if (data.ShowDirection == ShowDirection.Left)
        {
            image = image_left_ui;
            name = name_left_text;
        }
        else
        {
            image = image_right_ui;
            name = name_right_text;
        }
        
        UpdateUIInformation(data, image, name);
        InitTransitionFX(data);
    }

    private static void InitTransitionFX(DialogueData data)
    {
        switch (data.dialogueTransitionFX)
        {
            case DialogueTransitionFX.FadeIn :
                break;
        }
    }

    private void UpdateUIInformation(DialogueData data, Image image, TextMeshProUGUI name)
    {
        DialogueCharacter character = dialogueManager.dialogueCharacters[data.dialogueCharacterType];
        image.sprite = character.sprite;
        name.text = Localize.GetLocalizedString(character.name);
        dialogue_text.text = Localize.GetLocalizedString(data.dialogueString);
    }

    private void SetDirection(DialogueData data)
    {
        image_left_ui.gameObject.SetActive(data.ShowDirection == ShowDirection.Left);
        name_left_ui.gameObject.SetActive(data.ShowDirection == ShowDirection.Left);
        image_right_ui.gameObject.SetActive(data.ShowDirection == ShowDirection.Right);
        name_right_ui.gameObject.SetActive(data.ShowDirection == ShowDirection.Right);
    }
}
