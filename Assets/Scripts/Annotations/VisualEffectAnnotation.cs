using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class VisualEffectAnnotation : StaticAnnotation {


    public bool IsActive { get; set; }
    private Color effectColor;

    private int _mpPos;
    private bool triggerPressed;
    private GameObject _effectGO;
    GameObject effectsMenu;
	string _effectName;
    public bool annotationAdded;

    private string parentName;

    public VisualEffectAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,
       GameObject head) :
       base(video, rightHand, rightController, head)
    {
        effectColor = Color.white;
        IsActive = false;
        triggerPressed = false;
        _head = head;
        _effectName = null;
        _effectGO = null;

    }

    public VisualEffectAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,
		GameObject head,Color c, string effectName) :
        base(video, rightHand, rightController, head)
    {
        effectColor = c;
        IsActive = false;
        triggerPressed = false;
        _head = head;
		_effectName = effectName;
        _effectGO = null;
       
    }

    public override void annotate()
    {
        if (IsActive)
        {
            if (_effectGO == null)
            {
				//Debug.Log("EFFECT = " + _effectName);
         		_effectGO = GameObject.Instantiate(Resources.Load("EffectPrefabs/"+_effectName)) as GameObject;
                _effectGO.transform.parent = _rightHand.transform;
                _effectGO.transform.localPosition = new Vector3(0, 0, 0.1f);
                _effectGO.transform.localRotation = Quaternion.identity;
         //       _effectGO.transform.localScale = Vector3.one;
                Renderer r = _effectGO.GetComponent<Renderer>();
                r.material.SetColor("_TintColor", effectColor);

            }
            else
            {
                if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    _start = _video.getVideoTime();
                    IsActive = false;
                    GameObject o = GameObject.Find("Avatar");
                    if (o != null) { 
                        Transform joint = o.GetComponent<SkeletonRepresentation>().getTBR().findNearestBone(_rightHand.transform.position);
                        if (joint != null) {
                            Vector3 scale = _effectGO.transform.localScale;
                            _effectGO.transform.parent = joint;
                            parentName = joint.name;
                            _effectGO.transform.localPosition = Vector3.zero;
                            _effectGO.transform.localRotation = Quaternion.identity;
                            _effectGO.transform.localScale = scale;
                            _hasBeenCreated = true;
                        }
                        else
                        {
                            GameObject.Destroy(_effectGO);
                            _hasBeenCreated = false;

                        }
                    }
                    else
                    {
                        Vector3 scale = _effectGO.transform.localScale;
                        _effectGO.transform.parent = null;
                        _effectGO.transform.localPosition = Vector3.zero;
                        _effectGO.transform.localRotation = Quaternion.identity;
                        _effectGO.transform.localScale = scale;
                    }
                }



            }
        }
    }

    public override void play()
    {
        _effectGO.SetActive(true);
    }

    public override void stop()
    {
        _effectGO.SetActive(false);
    }

    public override int edit()
    {
       
        if (_effectGO.activeSelf){
            _annotationIdGO.SetActive(true);
            Vector3 rot = _head.transform.forward;
            rot.y = 0.0f;
            _annotationIdGO.transform.rotation = Quaternion.LookRotation(rot);
            _annotationIdGO.transform.position = new Vector3(_effectGO.transform.position.x, _effectGO.transform.position.y + 0.15f, _effectGO.transform.position.z);
            return _id;
        }
        else
            return -1;

    }

    public override void increaseDuration()
    {
        if (_effectGO.activeSelf) { 
            _duration += 0.5f;
            _annotationID.text = Convert.ToString(Convert.ToString(Math.Round(_duration, 1)));
        }
        
    }

    public override void decreaseDuration()
    {

        if (_effectGO.activeSelf && _duration >= 0) { 
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
        if (effectsMenu != null) GameObject.Destroy(effectsMenu);
        if (_effectGO != null) GameObject.Destroy(_effectGO);
    }

    public override string serialize()
    {
        return getID() + "#" + getStart() + "#" + getDuration () + "#"+ _effectName + "#" + effectColor.r + "#"+ effectColor.g + "#"+ effectColor.b + "#" + parentName;
    }

    public override void deserialize(string s)
    {
        string[] tokens = s.Split('#');
        setID(int.Parse(tokens[0]));
        setStart(float.Parse(tokens[1]));
        setDuration(float.Parse(tokens[2]));

        _effectName = tokens[3];
        effectColor = new Color(float.Parse(tokens[4]), float.Parse(tokens[5]), float.Parse(tokens[6]));
        parentName = tokens[7];

        Vector3 scale = _effectGO.transform.localScale;
        _effectGO = GameObject.Instantiate(Resources.Load("EffectPrefabs/" + _effectName)) as GameObject;
        _effectGO.transform.parent = GameObject.Find(parentName).transform;
        _effectGO.transform.localPosition = new Vector3(0, 0, 0.1f);
        _effectGO.transform.localRotation = Quaternion.identity;
        _effectGO.transform.localScale = scale;

        Renderer r = _effectGO.GetComponent<Renderer>();
        r.material.SetColor("_TintColor", effectColor);
        _hasBeenCreated = true;
    }
}
