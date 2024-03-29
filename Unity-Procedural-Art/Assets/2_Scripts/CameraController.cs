using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController
{
    private bool moveCam;
    private Vector2 previousMousePos;

    //-------------------------------------------------------------------

    public CameraController(){
        EventManager.AddListener<Vector2>(Events.OnMousePosChange, MousePosChange);
        EventManager.AddListener<Vector2Int>(Events.OnMiddleMouseDown, MiddleMouseDown);
        EventManager.AddListener<Vector2Int>(Events.OnMiddleMouseUp, MiddleMouseUp);
        EventManager.AddListener<float>(Events.OnMouseScroll, MouseScroll);
    }

    //--------------------------------------------------------------------

    private void MousePosChange(Vector2 mousePos2){
        if (moveCam == true){
            Vector2 mousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);

            float xDifference = mousePos.x - previousMousePos.x;
            float yDifference = mousePos.y - previousMousePos.y;

            Vector3 newCamPos = new Vector3(Camera.main.transform.position.x - xDifference, Camera.main.transform.position.y - yDifference, -10);
            Camera.main.transform.position = newCamPos;
        }
    }

    private void MiddleMouseDown(Vector2Int mousePos){
        previousMousePos = Camera.main.ScreenToWorldPoint(Input.mousePosition);
        moveCam = true;
    }

    private void MiddleMouseUp(Vector2Int mousePos){
        moveCam = false;
    }

    private void MouseScroll(float scrollDelta){
        if (scrollDelta > 0f && Camera.main.orthographicSize > Settings.Instance.MinCamSize)
        {
            Camera.main.orthographicSize -= Camera.main.orthographicSize * Settings.Instance.ScrollSpeed * 0.01f;
        }

        if (scrollDelta < 0f && Camera.main.orthographicSize < Settings.Instance.MaxCamSize)
        {
            Camera.main.orthographicSize += Camera.main.orthographicSize * Settings.Instance.ScrollSpeed * 0.01f;
        }
    }
}
