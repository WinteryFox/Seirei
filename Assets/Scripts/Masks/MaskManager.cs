using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;

namespace Masks {
    [RequireComponent (typeof(AudioSource))]
    [RequireComponent (typeof(PlayerLocomotion))]
    public class MaskManager : MonoBehaviour {
        public GameObject foxMask;
        public GameObject rabbitMask;
        public AudioClip pickupSound;
        public GameObject player;
        public AudioClip cooldown;

        //public float maskScale = 1.0f;

        private PlayerLocomotion locomotion;
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

        private float lastSwitchTime;

        void Awake () {
            locomotion = GetComponent<PlayerLocomotion>();
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
            activateMask();
        }

        private void activateMask () {
            if (!activateMaskInput || abilityMask == null)
                return;

            abilityMask.useAbility (cooldown, audioSource, locomotion);
        }

        private void handleSwitch () {
            if (!switchMaskInput || masks.Count == 0)
                return;

            var currentTime = Time.time;
            if (currentTime - lastSwitchTime < 2.0)
                return;
            lastSwitchTime = currentTime;

            abilityMaskIndex += 1;
            if (abilityMaskIndex >= masks.Count)
                abilityMaskIndex = 0;

            abilityMask = masks[abilityMaskIndex];

            if (currentMask != null)
                Destroy (currentMask);
            switch(abilityMask) {
                case FoxMask:
                    currentMask = Instantiate (foxMask, maskSpot.transform);
                    locomotion.canJump = false;
                    locomotion.canSprint = true;
                    break;
                case RabbitMask:
                    currentMask = Instantiate (rabbitMask, maskSpot.transform);
                    locomotion.canJump = true;
                    locomotion.canSprint = false;
                    break;
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

            var oldClip = audioSource.clip;
            audioSource.clip = pickupSound;
            audioSource.Play (0);
            StartCoroutine (playAudio (oldClip));
            Destroy (selection);
        }

        private IEnumerator playAudio (AudioClip clip) {
            yield return new WaitWhile (() => audioSource.isPlaying);
            audioSource.clip = clip;
            audioSource.Play (0);
        }
    }

    public class Mask {
        private float cooldown { get; set; } = 2.5f;
        private float lastUseTime = Time.time;

        public void useAbility (AudioClip c, AudioSource source, PlayerLocomotion locomotion) {
            var currentTime = Time.time;
            if (currentTime - lastUseTime < cooldown) {
                source.clip = c;
                source.Play();
                return;
            }

            Debug.Log ("Mask was used!");
            lastUseTime = currentTime;
        }
    }

    public class FoxMask : Mask {
        public new void useAbility (AudioClip c, AudioSource source, PlayerLocomotion locomotion) {
            base.useAbility (c, source, locomotion);
            //locomotion.playerRigidBody.velocity = locomotion.moveDirection * 100;
            //player.transform.localPosition += new Vector3 (0, 10, 0);
        }
    }

    public class RabbitMask : Mask {
        public new void useAbility (AudioClip c, AudioSource source, PlayerLocomotion locomotion) {
            base.useAbility (c, source, locomotion);
        }
    }
}
