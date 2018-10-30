using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using UnityEngine.EventSystems;
using System;

public class InputVisualizer : MonoBehaviour
{
    [SerializeField]
    Texture2D ball;

    //[AutoUI]
    [SerializeField]
    private bool debugInput = false;

    public static float ball_size = 1;

    public delegate void OnTouchDown(Vector2 p);
    public static event OnTouchDown TouchEvent;

    public void trigger_tuio_event(Vector2 p)
    {
        if (TouchEvent != null)
            TouchEvent(p);
    }

    // Use this for initialization
    void Awake(){
#if UNITY_EDITOR
       // debugInput = false;
#endif
    }

	void Start () {
        Debug.LogWarning("InputVisualizer is not fully implemented yet!!!!");
        //Cursor.visible = false;
    }

    void Update () 
	{
        if (EventSystem.current == null || EventSystem.current.currentInputModule == null)
            return;

        BaseInput input = EventSystem.current.currentInputModule.input;

        if (input.GetMouseButton(0))
        {
            trigger_tuio_event(input.mousePosition);
        }

        for (int i = 0; i < input.touchCount; i++)
        {
            Touch t = input.GetTouch(i);
            trigger_tuio_event(t.position);
        }
    }

    void OnGUI(){
        if (!debugInput)
            return;

        if (EventSystem.current == null || EventSystem.current.currentInputModule == null)
            return;

        BaseInput input = EventSystem.current.currentInputModule.input;

        if(input.GetMouseButton(0))
        {
            DrawPointer(input.mousePosition);
        }

        for(int i=0; i<input.touchCount; i++)
        {
            Touch t = input.GetTouch(i);
            DrawPointer(t.position);
        }
	}

    void DrawPointer(Vector2 position)
    {
        float x = position.x - ball.width / 2 * ball_size;
        float y = Screen.height - position.y - ball.height / 2 * ball_size;
        Rect ballRect = new Rect(x, y, ball.width * ball_size, ball.height * ball_size);
        GUI.DrawTexture(ballRect, ball);
    }
}
