using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HighlightPointsAnnotation : StaticAnnotation {


    public bool IsActive { get; set; }
    private Color highlightColor;
    private float[] _myPoints;
    private int _mpPos;
    private bool triggerPressed;

    public HighlightPointsAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController) :
        base(video, rightHand, rightController)
    {
        IsActive = false;
        triggerPressed = false;
        highlightColor = Color.cyan;
        _myPoints = new float[500];
    }

    void resetMyPoints()
    {
        for (int i = 0; i < _myPoints.Length; i++)
        {
            _myPoints[i] = 0;
        }
        _mpPos = 0;
    }

    public override void annotate()
    {
        if(!IsActive)
            resetMyPoints();

        else {

            if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerPressed = true;
                _start = Time.time;
                Debug.Log("start = " + _start);
            }

            if (triggerPressed)
            {
                Debug.Log("ANNOTATING HIGHLIGHTPOINTS");
            }

            if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerPressed = false;
                IsActive = false;
                _duration = Time.time - _start;
                Debug.Log("duration = " + _duration);
            }
        }
    }


}
