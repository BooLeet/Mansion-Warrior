using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class LevelEditorInput : MonoBehaviour
{
    public bool Enable { get; set; }

    private void Start()
    {
        Enable = true;
    }

    public bool GetUndo()
    {
        return Enable && Input.GetKeyDown(KeyCode.Z);
    }

    public bool GetRedo()
    {
        return Enable&& Input.GetKeyDown(KeyCode.Y);
    }

    public bool MainAction()
    {
        return Enable && Input.GetKeyDown(KeyCode.Mouse0);
    }

    public bool SecondaryAction()
    {
        return Enable && Input.GetKeyDown(KeyCode.Mouse1);
    }

    public bool SecondaryActionHold()
    {
        return Enable && Input.GetKey(KeyCode.Mouse1);
    }

    public bool GetActionModifier()
    {
        return Enable && Input.GetKey(KeyCode.LeftShift);
    }

    public bool GetReverseRotation()
    {
        return Enable && Input.GetKey(KeyCode.R);
    }

    public bool GetMaterialSelector()
    {
        return Enable && Input.GetKeyDown(KeyCode.F);
    }

    public bool GetObjectSelector()
    {
        return Enable && Input.GetKeyDown(KeyCode.G);
    }

    public bool GetSaveLevel()
    {
        return Enable && Input.GetKeyDown(KeyCode.F5);
    }

    public bool GetLoadLevel()
    {
        return Enable && Input.GetKeyDown(KeyCode.F9);
    }

    public bool GetCloseSelector()
    {
        return Enable && (Input.GetKeyDown(KeyCode.Tab) || Input.GetKeyDown(KeyCode.Escape));
    }

    public bool GetConfirm()
    {
        return Enable && Input.GetKeyDown(KeyCode.Return);
    }

    public bool GetCancel()
    {
        return Enable && (Input.GetKeyDown(KeyCode.Escape) || Input.GetKeyDown(KeyCode.Tab));
    }

    public Vector2 GetCameraMovementInput()
    {
        if (!Enable)
            return Vector2.zero;

        return new Vector2(Input.GetAxisRaw("Horizontal"), Input.GetAxisRaw("Vertical"));
    }

    public Vector2 GetCameraRotationInput()
    {
        if (!Enable)
            return Vector2.zero;

        return new Vector2(Input.GetAxisRaw("Mouse X"), Input.GetAxisRaw("Mouse Y"));
    }

    public bool GetCameraUp()
    {
        return Enable && Input.GetKey(KeyCode.Space);
    }

    public bool GetCameraDown()
    {
        return Enable && Input.GetKey(KeyCode.C);
    }

    public bool GetCameraSpeedUp()
    {
        return Enable && Input.GetKey(KeyCode.LeftShift);
    }

    public bool GetNextSelectedItem()
    {
        return Enable && Input.GetKeyDown(KeyCode.E);
    }

    public bool GetPreviousSelectedItem()
    {
        return Enable && Input.GetKeyDown(KeyCode.Q);
    }

    public bool GetChangeLighting()
    {
        return Input.GetKeyDown(KeyCode.L);
    }
}
