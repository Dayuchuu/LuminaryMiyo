using System.Collections;
using UnityEngine;
using TMPro;
using UnityEngine.InputSystem;
using UnityEngine.UI;

public class Dialogue : MonoBehaviour
{
    [System.Serializable]
    public class DialogueData
    {
        public string speaker;
        [TextArea(3, 10)]
        public string line;
        public Sprite portrait;
    }

    [SerializeField] private DialogueData[] dialogueData;

    [SerializeField] private TextMeshProUGUI dialogueTextComponent;
    [SerializeField] private TextMeshProUGUI speakerTextComponent;
    [SerializeField] private Image speakerPortrait;
    [SerializeField] private bool isPreTutorial;
    
    [SerializeField] private float textSpeed;

    public bool isPlaying;

    private int dialogueDataIndex;

    // Start is called before the first frame update
    void Start()
    {
        dialogueTextComponent.text = string.Empty;
        speakerTextComponent.text = string.Empty;
        speakerPortrait.sprite = null;
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
                speakerPortrait.sprite = dialogueData[dialogueDataIndex].portrait;
            }
        }
    }





    private void StartDialogue()
    {
        dialogueDataIndex = 0;
        isPlaying = true;
        StartCoroutine(TypeLine());
    }

    private IEnumerator TypeLine()
    {
        foreach (char c in dialogueData[dialogueDataIndex].line.ToCharArray())
        {
            dialogueTextComponent.text += c;
            speakerTextComponent.text = dialogueData[dialogueDataIndex].speaker;
            speakerPortrait.sprite = dialogueData[dialogueDataIndex].portrait;
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
            speakerPortrait.sprite = null;
            StartCoroutine (TypeLine());
        }

        //what happens after all dialogue has been exhausted, in this case activating movement and closing Dialogue ui.
        else
        {
            if (!isPreTutorial)
            {
                GameObject.FindGameObjectWithTag("Player").GetComponent<PlayerMovement>().disableMovement = false;
                isPlaying = false;
                UIManager.Instance.CloseMenu(UIManager.Instance.tutorialDialogue,UIManager.Instance.levelDialogue, CursorLockMode.Locked, 1f);
                UIManager.Instance.inGameUi.SetActive(true);
                gameObject.transform.parent.gameObject.SetActive(false);
            }
            else
            {
                UIManager.Instance.CloseMenu(UIManager.Instance.preTutorial.transform.parent.gameObject, CursorLockMode.None, 1f);
                UIManager.Instance.OpenMenu(UIManager.Instance.levelSelectionScreen, CursorLockMode.None, 1f);
            }
        }
    }

    private void AfterText()
    {
      
    }
}
