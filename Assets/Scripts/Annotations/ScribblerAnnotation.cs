﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class ScribblerAnnotation : StaticAnnotation {

    private GameObject lineRendererGO { get; set; }
    public LineRenderer lineRenderer { get; set; }
    public bool IsActive { get; set; }

    struct PositionFrame {
        public Vector3 position;
        public float time;
    }

    private List<PositionFrame> _myPoints;

    public bool triggerPressed;
    private Vector3 midPoint;
    
    public ScribblerAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,Color c, GameObject head) :
        base(video, rightHand, rightController, head)
    {
        _myPoints = new List<PositionFrame>();
        IsActive = false;
        triggerPressed = false;
        midPoint = Vector3.zero;
 
        lineRendererGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/LineRendererPrefab")) as GameObject;
        lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;//new Color(c.r / 0.2f, c.g / 0.2f, c.b / 0.2f);
        lineRenderer.positionCount = 0;
        
    }

    public override void annotate()
    {

        Debug.Log("annotate method scribbler");

        if (!IsActive)
            _myPoints.Clear();
        else
        {
           
            if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            { 
                triggerPressed = true;
               // _start = Time.time;
                Debug.Log("start = " + _start);
            }
            
            if(triggerPressed)
            {
                PositionFrame p;
                p.position = _rightHand.transform.position;
                p.time = _video.getTime();
                
                _myPoints.Add(p);

                if (_myPoints != null)
                {
                    lineRenderer.positionCount = _myPoints.Count;
                    lineRenderer.SetPosition(_myPoints.Count - 1,p.position);
                }
            }

            if(_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
               
                triggerPressed = false;
                IsActive = false;
                _start = _myPoints[0].time;
                //_duration = Time.deltaTime - _start;
                _hasBeenCreated = true;
                Debug.Log("duration = " + _duration);
            }
        } 
    }


    public override void play()
    {
        
        lineRenderer.positionCount = 0;
        int i =0;
        foreach (PositionFrame p in _myPoints)
        {
            if (p.time <= _video.getTime())
            {
                lineRenderer.positionCount = i+1;
                lineRenderer.SetPosition(i, p.position);
                i++;
            }
        
        }
        lineRendererGO.SetActive(true);
    }

    public Vector3 getCenter()
    {
        Vector3 res = Vector3.zero;
        foreach (PositionFrame p in _myPoints)
        {
            res += p.position;
        }
        res = res / _myPoints.Count;
        return res;
    }

    public override void stop()
    {
        lineRendererGO.SetActive(false);
        _annotationIdGO.SetActive(false);
    }

    public override int edit()
    {
      
        if (lineRendererGO.activeSelf) { 
            _annotationIdGO.SetActive(true);
            Vector3 rot = _head.transform.forward;
            rot.y = 0.0f;
            _annotationIdGO.transform.rotation = Quaternion.LookRotation(rot);
            Vector3 pos = getCenter();
            _annotationIdGO.transform.position = new Vector3(pos.x, pos.y + 0.3f, pos.z);
            _editing = true;

            return _id;
        }

        return -1;

    }

    public override void increaseDuration()
    {
        if (lineRendererGO.activeSelf) { 
            _duration += 0.01f;
            _annotationID.text = Convert.ToString(Convert.ToString(Math.Round(_duration, 1)));
        }
        //Debug.Log("duration = " + _duration);
    }

    public override void decreaseDuration()
    {
        if (lineRendererGO.activeSelf && _duration >= 0) { 
            _duration -= 0.01f;
            _annotationID.text = Convert.ToString(Convert.ToString(Math.Round(_duration, 1)));
        }
    }

    public override void disableDurationGO()
    {
        if(_annotationIdGO.activeSelf)
            _annotationIdGO.SetActive(false);
    }


    public override void reset()
    {
        if (lineRendererGO != null) GameObject.Destroy(lineRendererGO);
    }
}
