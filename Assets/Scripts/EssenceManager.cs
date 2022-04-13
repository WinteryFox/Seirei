using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;

public class EssenceManager : MonoBehaviour {
    private Camera playerCamera;
    private PlayerControls playerControls;
    private int essence;
    private bool useInput;

    void Awake () {
        playerCamera = Camera.main;
        playerControls ??= new PlayerControls();
        playerControls.PlayerActions.Use.performed += _ => useInput = true;
        playerControls.PlayerActions.Use.canceled += _ => useInput = false;
        playerControls.Enable();
    }
    
    void Update() {
        if (!useInput)
            return;
        
        var ray = playerCamera.ScreenPointToRay (Mouse.current.position.ReadValue());
        if (!Physics.Raycast (ray, out var hit))
            return;

        var selection = hit.transform.gameObject;
        var shrine = selection.GetComponent<Shrine>();
        if (shrine == null)
            return;

        var t = playerCamera.transform;
        var originalCameraPosition = t.localPosition;
        var shrinePosition = shrine.transform.position;
        t.localPosition = shrinePosition + new Vector3 (-5f, 0f, 0f);
        // TODO: Play cool animation and some mysterious music
        essence += 100;
        // TODO: Reset camera to original position
    }
}
