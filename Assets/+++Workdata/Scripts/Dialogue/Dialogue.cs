using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;

public class Dialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueData
    {
        public string speaker;
        [TextArea(3, 10)]
        public string line;
    }

    [SerializeField] private DialogueData[] dialogueData;

    [SerializeField] private TextMeshProUGUI dialogueTextComponent;
    [SerializeField] private TextMeshProUGUI speakerTextComponent;

    [SerializeField] private float textSpeed;

    private int dialogueDataIndex;

    // Start is called before the first frame update
    void Start()
    {
        dialogueTextComponent.text = string.Empty;
        speakerTextComponent.text = string.Empty;   
        StartDialogue();
    }


    //new input system
    public void Talking(InputAction.CallbackContext context)
    {
        if (context.performed)
        {
            //once the text in the dialogue box matches what the complete line aka when it has finished typing out
            if (dialogueTextComponent.text == dialogueData[dialogueDataIndex].line)
            {
                NextLine();
            }
            //basically skips the typing out process and jumps to the next line
            else
            {
                StopAllCoroutines();
                dialogueTextComponent.text = dialogueData[dialogueDataIndex].line;
                speakerTextComponent.text = dialogueData[dialogueDataIndex].speaker;
            }
        }
    }





    private void StartDialogue()
    {
        dialogueDataIndex = 0;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach (char c in dialogueData[dialogueDataIndex].line.ToCharArray())
        {
            speakerTextComponent.text = dialogueData[dialogueDataIndex].speaker;

            dialogueTextComponent.text += c;
            yield return new WaitForSeconds(textSpeed);
        }
    }


    private void NextLine()
    {
        if (dialogueDataIndex < dialogueData.Length - 1)
        {
            dialogueDataIndex++;
            dialogueTextComponent.text = string.Empty;
            speakerTextComponent.text= string.Empty;
            StartCoroutine (TypeLine());
        }

        //what happens after all dialogue has been exhausted, in this case probably loading the next scene
        else
            gameObject.SetActive(false);
    }
}
