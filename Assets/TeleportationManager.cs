using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.XR.Interaction.Toolkit;

public class TeleportationManager : MonoBehaviour
{
    [SerializeField] InputActionAsset actionAsset;
    InputAction _thumbstick;

    [SerializeField] TeleportationProvider provider;

    [SerializeField] XRRayInteractor rayInteractor;
    bool isActive;

    void Start()
    {
        rayInteractor.enabled=false;

        var inputActionMap = actionAsset.FindActionMap("XRI LeftHand");

        var activate = inputActionMap.FindAction("Teleport Mode Activate");
        activate.Enable();
        activate.performed += OnTeleportActivate;

        var cancel = inputActionMap.FindAction("Teleport Mode Cancel");
        cancel.Enable();
        cancel.performed += OnTeleportCancel;

        _thumbstick = inputActionMap.FindAction("Move");
        _thumbstick.Enable();
    }

    void Update()
    {
        if(!isActive)
            return;
        
        if(_thumbstick.triggered)
            return;

        if (!rayInteractor.TryGetCurrent3DRaycastHit(out var hit))
        {
            TurnOffTeleport();
            return;
        }

        TeleportRequest request=new TeleportRequest(){
            destinationPosition=hit.point
        };

        provider.QueueTeleportRequest(request);
        TurnOffTeleport();
    }

    void OnTeleportActivate(InputAction.CallbackContext context){
        rayInteractor.enabled=true;
        isActive=true;
    }

    void OnTeleportCancel(InputAction.CallbackContext context){
        TurnOffTeleport();
    }

    private void TurnOffTeleport()
    {
        rayInteractor.enabled = false;
        isActive = false;
    }
}
