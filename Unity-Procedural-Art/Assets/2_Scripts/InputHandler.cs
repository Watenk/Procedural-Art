using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class InputHandler : IUpdateable
{
    private Vector2 previousMousePos;

    //-------------------------------------------------------

    public void OnUpdate(){
        MouseInputs();
    }

    //---------------------------------------------------------

    private void MouseInputs(){

        Vector3 mousePos3 = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        Vector2Int mousePos = new Vector2Int((int)mousePos3.x, -(int)mousePos3.y);

        //if (Input.GetKey(KeyCode.Mouse0)) { EventManager.Invoke(Events.OnLeftMouse); }
        if (Input.GetKeyDown(KeyCode.Mouse0)) { EventManager.Invoke(Events.OnLeftMouseDown, mousePos); }
        //if (Input.GetKeyUp(KeyCode.Mouse0)) { EventManager.Invoke(Events.OnLeftMouseUp); }
        if (Input.GetKeyDown(KeyCode.Mouse1)) { EventManager.Invoke(Events.OnRightMouseDown, mousePos); }
        //if (Input.GetKeyUp(KeyCode.Mouse1)) { EventManager.Invoke(Events.OnRightMouseUp, mousePos); }
        if (Input.GetKeyDown(KeyCode.Mouse2)) { EventManager.Invoke(Events.OnMiddleMouseDown, mousePos); }
        if (Input.GetKeyUp(KeyCode.Mouse2)) { EventManager.Invoke(Events.OnMiddleMouseUp, mousePos); }
        if (Input.GetKeyDown(KeyCode.Space)) { EventManager.Invoke(Events.OnSpaceDown); }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.S)) { EventManager.Invoke(Events.OnSave); }
        if (Input.GetKey(KeyCode.LeftControl) && Input.GetKeyDown(KeyCode.L)) { EventManager.Invoke(Events.OnLoad); }
        if (Input.mouseScrollDelta.y != 0) { EventManager.Invoke(Events.OnMouseScroll, Input.mouseScrollDelta.y); }

        Vector2 mousePos2 = new Vector2(Input.mousePosition.x, Input.mousePosition.y);
        if (mousePos2 != previousMousePos){
            previousMousePos = mousePos2;
            EventManager.Invoke(Events.OnMousePosChange, mousePos2);
        }
    }
}
