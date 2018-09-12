using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class DynamicAnnotation {

    protected int _id;
    protected float _start;
    protected Color _color;

    protected GameObject _rightHand;
    protected SteamVR_Controller.Device _rightController;

    public DynamicAnnotation(GameObject rightHand, SteamVR_Controller.Device rightController)
    {
        _id = 0;
        _start = 0.0f;
        _color = Color.cyan;

        _rightHand = rightHand;
        _rightController = rightController;
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

    public abstract void annotate();
}
