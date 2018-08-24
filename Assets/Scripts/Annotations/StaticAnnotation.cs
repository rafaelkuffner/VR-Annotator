using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class StaticAnnotation {

    protected float _start;
    protected float _duration;
    protected Type _annotationType;

    protected SteamVR_Controller.Device _rightController;
    protected GameObject _rightHand;
    protected CloudVideoPlayer _video;

    public enum Type
    {
        HIGHLIGHTPOINTS, SCRIBBLER, SPEECHTOTEXT, MARK
    }

    public StaticAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController)
    {
        _video = video;
        _rightHand = rightHand;
        _rightController = rightController;

        _start = 0.0f;
        _duration = 0.0f;
    }

    public abstract void annotate();


}
