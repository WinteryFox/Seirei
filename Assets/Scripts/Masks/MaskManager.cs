using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Masks {
    [RequireComponent(typeof(AudioSource))]
    public class MaskManager : MonoBehaviour {
        public GameObject foxMask;
        public GameObject rabbitMask;
        public AudioClip pickupSound;

        //public float maskScale = 1.0f;

        private AudioSource audioSource;
        public GameObject maskSpot;
        private GameObject currentMask;

        private PlayerControls playerControls;
        private List<Mask> masks = new();
        private Camera playerCamera;

        private int abilityMaskIndex;
        private Mask abilityMask;

        private bool useInput;
        private bool switchMaskInput;
        private bool activateMaskInput;

        void Awake () {
            playerControls ??= new PlayerControls();
            playerCamera = Camera.main;
            playerControls.PlayerActions.Use.performed += _ => useInput = true;
            playerControls.PlayerActions.Use.canceled += _ => useInput = false;
            playerControls.PlayerActions.SwitchMask.performed += _ => switchMaskInput = true;
            playerControls.PlayerActions.SwitchMask.canceled += _ => switchMaskInput = false;
            playerControls.PlayerActions.ActivateMask.performed += _ => activateMaskInput = true;
            playerControls.PlayerActions.ActivateMask.canceled += _ => activateMaskInput = false;
            playerControls.Enable();
            audioSource = GetComponent<AudioSource>();
        }

        void Update () {
            handleUse();
            handleSwitch();
        }

        private void handleSwitch () {
            if (!switchMaskInput || masks.Count == 0)
                return;

            abilityMaskIndex += 1;
            if (abilityMaskIndex >= masks.Count)
                abilityMaskIndex = 0;

            abilityMask = masks[abilityMaskIndex];

            if (currentMask != null)
                Destroy (currentMask);
            currentMask = abilityMask switch {
                FoxMask => Instantiate (foxMask, maskSpot.transform),
                RabbitMask => Instantiate (rabbitMask, maskSpot.transform), // TODO
                _ => throw new ArgumentOutOfRangeException()
            };
            //currentMask.transform.localScale = new Vector3 (maskScale, maskScale, maskScale);
        }

        private void handleUse () {
            if (!useInput)
                return;

            var ray = playerCamera.ScreenPointToRay (Mouse.current.position.ReadValue());
            if (!Physics.Raycast (ray, out var hit))
                return;

            var selection = hit.transform.gameObject;
            var mask = selection.GetComponent<AbilityMask>();
            if (mask == null)
                return;

            switch (mask.type) {
                case MaskType.FOX:
                    masks.Add (new FoxMask());
                    break;
                case MaskType.RABBIT:
                    masks.Add (new RabbitMask());
                    break;
                default:
                    throw new ArgumentOutOfRangeException();
            }

            StartCoroutine (playAudio());
            Destroy (selection);
        }

        private IEnumerator playAudio () {
            yield return new WaitWhile (() => audioSource.isPlaying);
            audioSource.clip = pickupSound;
            audioSource.Play(0);
        }
    }

    public interface Mask {
        public void useAbility ();
    }

    public class FoxMask : Mask {
        public void useAbility () { }
    }

    public class RabbitMask : Mask {
        public void useAbility () { }
    }
}
