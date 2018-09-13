using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public abstract class StaticAnnotation {

    protected int _id;
    protected float _start;
    protected float _duration;
    protected Type _annotationType;
    protected GameObject _annotationIdGO;
    protected TextMesh _annotationID;
    protected bool _hasBeenCreated;
    protected bool _editing;

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

        _id = 0;
        _start = 0.0f;
        _duration = 3.0f;
        _hasBeenCreated = false;
        _editing = false;

        _annotationIdGO = GameObject.Instantiate(Resources.Load("Prefabs/AnnotationID")) as GameObject;
        _annotationIdGO.transform.localScale = new Vector3(0.1f, 0.1f, 0.1f);
        _annotationID = _annotationIdGO.GetComponent<TextMesh>();
        _annotationID.text = "0";
        _annotationIdGO.SetActive(false);

    }

    public void setID(int id)
    {
        _id = id;
    }

    public int getID()
    {
        return _id;
    }

    public void setStart(float start)
    {
        _start = start;
    }

    public float getStart()
    {
        return _start;
    }

    public void setDuration(float duration)
    {
        _duration = duration;
    }

    public float getDuration()
    {
        return _duration;
    }

    public bool getHasBeenCreated()
    {
        return _hasBeenCreated;
    }

    public bool getEditing()
    {
        return _editing;
    }

    public void setEditing(bool editing)
    {
        _editing = editing;
    }
    public abstract void annotate();

    public abstract void play();

    public abstract void stop();

    public abstract void edit();

    public abstract void reset();
}
