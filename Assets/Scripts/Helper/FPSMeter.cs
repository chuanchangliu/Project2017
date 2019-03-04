using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FPSMeter : MonoBehaviour
{

    private float intervalTime = 1.0f;
    private float consumeTime;
    private int frameCount = 0;
    private float fpsMeter;

    private void Start()
    {
        consumeTime = 0.0f;
    }

    private void Update()
    {
        frameCount = frameCount + 1;
        consumeTime = consumeTime + Time.deltaTime;
        if (consumeTime > intervalTime)
        {
            fpsMeter = Mathf.Floor(frameCount / consumeTime);
            consumeTime = 0;
            frameCount = 0;
        }
    }

    void OnGUI()
    {
        GUI.skin.label.fontSize = 22;
        int w = Screen.width / 10;
        int h = Screen.height / 10;
        if (fpsMeter >= 30.0f)
        {
            GUI.skin.label.normal.textColor = Color.green;
        }
        else if (fpsMeter >= 20.0f)
        {
            GUI.skin.label.normal.textColor = Color.yellow;
        }
        else
        {
            GUI.skin.label.normal.textColor = Color.red;
        }
        GUI.Label(new Rect(w * 0.5f-32, h * 0.5f-20, 150, 100), "FPS: " + fpsMeter.ToString());

    }
}
