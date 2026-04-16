// using UnityEngine;
// using TMPro;
// using UnityEngine.UI;
// using UnityEngine.InputSystem;
// using System.Collections;
//
// public class DistorsionScene : MonoBehaviour
// {
//     [SerializeField] private GameObject narrativeCanvas; 
//     [SerializeField] private GameObject gameHUD;     
//     
//     
//     [Header("Images")] 
//     public Image RolandSeenAngelica;
//     public RawImage AngelicaTalks;
//     public Image BackScreen;
//     public RawImage Distorsion;
//     public Image DistorsedRoland;
//     public RawImage TimeParadox;
//     
//     [Header("sounds")]
//     public AudioClip[] dialogueSounds;
//     public AudioSource audioSource;
//     
//     [Header("Dialoge")]
//     [SerializeField] private bool canSkip = true;
//     [SerializeField] private float typingSpeed = 0.05f;
//     [TextArea(3, 5)] public string[] sentences;
//     public TextMeshProUGUI dialogueText;
//     
//     [Header("PlayerReferences")]
//     [SerializeField] private PlayerInput playerInput;
//     [SerializeField] private PlayerAttackSystem playerAttack;
//     
//     
//     
//     private int index;
//     private bool isTyping;
//     
//
//
//     private void Awake()
//     {
//         Time.timeScale = 0f;
//         
//         if (gameHUD != null) gameHUD.SetActive(false);
//         if (narrativeCanvas != null) narrativeCanvas.SetActive(true);
//     }
//     
//     private void Start()
//     {
//         DistorsionSceneRoland();
//     }
//
//
//     private void DistorsionSceneRoland()
//     {
//         if (playerInput != null) playerInput.enabled = true;
//             
//         playerInput.SwitchCurrentActionMap("UI");
//
//         if (playerAttack != null) playerAttack.enabled = false;
//         Cursor.visible = true;
//         Cursor.lockState = CursorLockMode.None;
//     }
//
//     private void EndDistorsionSceneRoland()
//     { 
//         Time.timeScale = 1f;
//                  
//         if (narrativeCanvas != null) narrativeCanvas.SetActive(false);
//         if (gameHUD != null) gameHUD.SetActive(true);
//         
//         if (playerInput != null) 
//         {
//           playerInput.enabled = true; 
//           playerInput.SwitchCurrentActionMap("Player"); 
//         }
//         
//           Cursor.visible = false;
//           Cursor.lockState = CursorLockMode.Locked;
//     }
//     
//     public void SkipAllDialogue()
//     {
//         StopAllCoroutines();
//         if (audioSource != null) audioSource.Stop();
//         FinishDialogue();
//     }
//
//     public void StartDialogue()
//     {
//
//         StartCoroutine(TypeSentence());
//     }
//     
//     IEnumerator TypeSentence()
//     {
//         isTyping = true;
//         dialogueText.text = "";
//         
//         if (audioSource != null && dialogueSounds.Length > index && dialogueSounds[index] != null)
//         {
//             audioSource.clip = dialogueSounds[index];
//             audioSource.Play();
//         }
//
//         foreach (char letter in sentences[index].ToCharArray())
//         {
//             dialogueText.text += letter;
//             yield return new WaitForSecondsRealtime(typingSpeed);
//         }
//         isTyping = false;
//     }
//     
//     
// }
//
