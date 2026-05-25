using System.Collections;
using System.Collections.Generic;
using UnityEngine;

namespace CameraDoorScript
{
    public class CameraOpenDoor : MonoBehaviour {
        public float DistanceOpen = 3;
        public GameObject text; 
        
        [Header("Paper UI")]
        public GameObject paperUIPanel; 
        public static bool isPaperCollected = false; 
        
        void Start () {}
        
        void Update () {
            if (paperUIPanel != null && paperUIPanel.activeSelf) {
                if (Input.GetKeyDown(KeyCode.E)) {
                    paperUIPanel.SetActive(false);
                    Time.timeScale = 1f; 
                }
                return; 
            }

            if (Time.timeScale == 0f) return;

            RaycastHit hit;
            if (Physics.Raycast (transform.position, transform.forward, out hit, DistanceOpen)) {
                
                // --- PAPER INTERACTION ---
                if (hit.transform.CompareTag("InteractablePaper")) {
                    text.SetActive(true);
                    if (Input.GetKeyDown(KeyCode.E)) {
                        text.SetActive(false);
                        hit.transform.gameObject.SetActive(false); 
                        isPaperCollected = true;
                        
                        if (paperUIPanel != null) {
                            paperUIPanel.SetActive(true);
                            Time.timeScale = 0f; 
                        }
                    }
                }
                // --- DOOR INTERACTION ---
                else if (hit.transform.GetComponent<DoorScript.Door> ()) {
                    DoorScript.Door hitDoor = hit.transform.GetComponent<DoorScript.Door>();
                    CoreGameLogic gameLogic = FindObjectOfType<CoreGameLogic>();
                    
                    if (gameLogic == null || gameLogic.CanPlayerOpenThisDoor(hitDoor)) {
                        text.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.E))
                            hitDoor.OpenDoor();
                    } else {
                        text.SetActive(false); 
                    }
                }
                // --- PC INTERACTION ---
                else if (hit.transform.CompareTag("InteractablePC")) {
                    CoreGameLogic gameLogic = FindFirstObjectByType<CoreGameLogic>();
                    if (gameLogic != null && gameLogic.isPCInteractable) {
                        text.SetActive(true);
                        if (Input.GetKeyDown(KeyCode.E)) {
                            gameLogic.InteractWithPC();
                        }
                    } else {
                        text.SetActive(false); 
                    }
                }
                // --- MAGICAL DOOR INTERACTION (NEW) ---
                else if (hit.transform.GetComponent<MagicalEndDoor>()) {
                    text.SetActive(true); // "Press E" wala text show karega
                    if (Input.GetKeyDown(KeyCode.E)) {
                        hit.transform.GetComponent<MagicalEndDoor>().PlayEndingSequence();
                    }
                }
                else {
                    text.SetActive(false);
                }
            } else {
                text.SetActive(false);
            }
        }
    }
}