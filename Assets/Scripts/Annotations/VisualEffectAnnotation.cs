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
    GameObject _rightPointer;
    GameObject _head;
    GameObject effectsMenu;
	string _effectName;


    public VisualEffectAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,
		GameObject head, GameObject rightPointer,Color c, string effectName) :
        base(video, rightHand, rightController, head)
    {
        effectColor = c;
        IsActive = false;
        triggerPressed = false;
        _rightPointer = rightPointer;
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
				Debug.Log("EFFECT = " + _effectName);
         		_effectGO = GameObject.Instantiate(Resources.Load("EffectPrefabs/"+_effectName)) as GameObject;
                _effectGO.transform.parent = _rightHand.transform;
                _effectGO.transform.localPosition = new Vector3(0, 0, 0.1f);
                _effectGO.transform.localRotation = Quaternion.identity;
         //       _effectGO.transform.localScale = Vector3.one;
                Renderer r = _effectGO.GetComponent<Renderer>();
                r.sharedMaterial.SetColor("_TintColor", effectColor);

            }
            else
            {
                if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    _start = _video.getVideoTime();
                    IsActive = false;
                    _hasBeenCreated = true;
                    GameObject o = GameObject.Find("Avatar");
                    if (o != null) { 
                        Transform joint = o.GetComponent<SkeletonRepresentation>().getTBR().findNearestBone(_rightHand.transform.position);
                        if (joint != null) {
                            Vector3 scale = _effectGO.transform.localScale;
                            _effectGO.transform.parent = joint;
                            _effectGO.transform.localPosition = Vector3.zero;
                            _effectGO.transform.localRotation = Quaternion.identity;
                            _effectGO.transform.localScale = scale;
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
            _duration += 0.1f;
            _annotationID.text = Convert.ToString(Convert.ToString(Math.Round(_duration, 1)));
        }
        
    }

    public override void decreaseDuration()
    {

        if (_effectGO.activeSelf && _duration >= 0) { 
            _duration -= 0.1f;
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
}
