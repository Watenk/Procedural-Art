using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : IUpdateable
{
    private Vector2 previousMousePos;

    //--------------------------------------------------------

    public void OnUpdate(){
        MouseInputs();
    }

    //---------------------------------------------------------

    private void MouseInputs(){
        if (Input.GetKey(KeyCode.Mouse0)) { EventManager.Invoke(Events.OnLeftMouse); }
        if (Input.GetKeyDown(KeyCode.Mouse0)) { EventManager.Invoke(Events.OnLeftMouseDown); }
        if (Input.GetKeyUp(KeyCode.Mouse0)) { EventManager.Invoke(Events.OnLeftMouseUp); }
        if (Input.GetKeyDown(KeyCode.Mouse1)) { EventManager.Invoke(Events.OnRightMouseDown); }
        if (Input.GetKeyUp(KeyCode.Mouse1)) { EventManager.Invoke(Events.OnRightMouseUp); }
        if (Input.GetKeyDown(KeyCode.Mouse2)) { EventManager.Invoke(Events.OnMiddleMouseDown); }
        if (Input.GetKeyUp(KeyCode.Mouse2)) { EventManager.Invoke(Events.OnMiddleMouseUp); }
        if (Input.GetKeyDown(KeyCode.Space)) { EventManager.Invoke(Events.OnSpaceDown); }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) { EventManager.Invoke(Events.OnSave); }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) { EventManager.Invoke(Events.OnLoad); }

        if (Input.mouseScrollDelta.y != 0) { EventManager.Invoke(Events.OnMouseScroll, Input.mouseScrollDelta.y); }
        
        Vector2 mousePos = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (mousePos != previousMousePos){
            previousMousePos = mousePos;
            EventManager.Invoke(Events.OnMousePosChange);
        }
    }
}
