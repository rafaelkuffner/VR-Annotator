using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;
using System.Text;

public class FloorAnnotation : StaticAnnotation {

    private GameObject lineRendererGO { get; set; }
    public LineRenderer lineRenderer { get; set; }
    public bool IsActive { get; set; }
    public GameObject floor;

    struct PositionFrame {
        public Vector3 position;
        public float time;
    }

    private List<PositionFrame> _myPoints;

    public bool triggerPressed;
    private Vector3 midPoint;

    public FloorAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController, GameObject head) :
       base(video, rightHand, rightController, head)
    {
        _myPoints = new List<PositionFrame>();
        IsActive = false;
        triggerPressed = false;
        midPoint = Vector3.zero;

        floor = GameObject.Find("VRPlane");

        lineRendererGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/LineRendererPrefab")) as GameObject;
        lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        lineRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        lineRenderer.startColor = Color.cyan;
        lineRenderer.endColor = Color.cyan;//new Color(c.r / 0.2f, c.g / 0.2f, c.b / 0.2f);
        lineRenderer.positionCount = 0;
    }

    public FloorAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,Color c, GameObject head) :
        base(video, rightHand, rightController, head)
    {
        _myPoints = new List<PositionFrame>();
        IsActive = false;
        triggerPressed = false;
        midPoint = Vector3.zero;

        floor = GameObject.Find("VRPlane");

        lineRendererGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/LineRendererPrefab")) as GameObject;
        lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Legacy Shaders/Particles/Additive"));
        lineRenderer.transform.localScale = new Vector3(0.5f, 0.5f, 0.5f);
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;//new Color(c.r / 0.2f, c.g / 0.2f, c.b / 0.2f);
        lineRenderer.positionCount = 0;
    }

    public override void annotate()
    {
        if (!IsActive)
            _myPoints.Clear();
        else
        {
           
            if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            { 
                triggerPressed = true;
                _start = _video.getVideoTime(); 
            }
            
            if(triggerPressed)
            {
                PositionFrame p;
                p.position = new Vector3(_rightHand.transform.position.x, floor.transform.position.y + 0.025f, _rightHand.transform.position.z);
                p.time = _video.getVideoTime();
				//p.time = Time.deltaTime;
                
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
                if(_myPoints.Count > 0)
                    _start = _myPoints[0].time;
                _duration = _video.getVideoTime() - _start + 0.5f;
                _hasBeenCreated = true;
            }
        } 
    }


    public override void play()
    {
        
        lineRenderer.positionCount = 0;
        int i =0;
        foreach (PositionFrame p in _myPoints)
        {
            if (p.time <= _video.getVideoTime())
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
        if (lineRendererGO != null)
            lineRendererGO.SetActive(false);
        if(_annotationIdGO != null)
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
            _duration += 0.5f;
            _annotationID.text = Convert.ToString(Convert.ToString(Math.Round(_duration, 1)));
        }
        //Debug.Log("duration = " + _duration);
    }

    public override void decreaseDuration()
    {
        if (lineRendererGO.activeSelf && _duration >= 0) { 
            _duration -= 0.5f;
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

    public override string serialize()
    {
        string res = getID() + "#" + getStart() + "#" + getDuration() + "#" + lineRenderer.startColor.r + "#" + lineRenderer.startColor.g + "#" + lineRenderer.startColor.b + "#";

        bool first = true;
        foreach (PositionFrame p in _myPoints)
        {
            if (first) first = false;
            else res += "$";
            res += p.position.x + "/" + p.position.y + "/" + p.position.z + "/" + p.time;
        }
        return res;
    }

    public override void deserialize(string s)
    {
        string[] tokens = s.Split('#');
        setID(int.Parse(tokens[0]));
        setStart(float.Parse(tokens[1]));
        setDuration(float.Parse(tokens[2]));
        Color c = new Color(float.Parse(tokens[3]), float.Parse(tokens[4]), float.Parse(tokens[5]));

        string[] positionsTimes = tokens[6].Split('$');
        foreach (string pt in positionsTimes)
        {
            string[] token = pt.Split('/');
            PositionFrame p;
            p.position = new Vector3(float.Parse(token[0]), float.Parse(token[1]), float.Parse(token[2]));
            p.time = float.Parse(token[3]);
            _myPoints.Add(p);
        }
        lineRenderer.startColor = c;
        lineRenderer.endColor = c;

        _hasBeenCreated = true;
    }
}
