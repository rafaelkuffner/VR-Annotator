using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;
public class MarkAnnotation : StaticAnnotation
{
    public bool IsActive { get; set; }
    private bool triggerPressed;
    GameObject markMenu;
  //  GameObject _rightPointer;
    GameObject _head;
    GameObject _markGO;
    private bool markNotPlaced;
    private string markName;

    public MarkAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,
        GameObject head): //, GameObject rightPointer) :
        base(video, rightHand, rightController, head) 
    {
        IsActive = false;
        triggerPressed = false;
        //_rightPointer = rightPointer;
        _head = head;
        _markGO = null;
        markNotPlaced = false;
        
    }

    public override void annotate()
    {

        if (IsActive)
        {
            if (_markGO == null)
            {

              //  _rightPointer.SetActive(true);
                Ray raycast = new Ray(_rightHand.transform.position, _rightHand.transform.forward);
                RaycastHit hit;
                bool bHit = Physics.Raycast(raycast, out hit);
                if (hit.transform != null)
                {
                    Button b = hit.transform.gameObject.GetComponent<Button>();
                    if (b != null)
                    {
                        b.Select();

                        if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
                        {
                            if (b != null)
                            {
                                //_rightPointer.SetActive(false);
                                _markGO = new GameObject();
                                _markGO.transform.parent = _rightHand.transform;
                                _markGO.transform.localPosition = new Vector3(0, 0, 0.1f);
                                _markGO.transform.localRotation = Quaternion.identity;
                                _markGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                                SpriteRenderer sr = _markGO.AddComponent<SpriteRenderer>();
                                markName = b.name;
                                sr.sprite = (Sprite)Resources.Load(b.name, typeof(Sprite));
                            }
                        }
                    }
                }
            }
            else
            {
                if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && !markNotPlaced)
                {
                    _start = _video.getVideoTime();
                    IsActive = false;
                    _hasBeenCreated = true;
                    _markGO.transform.parent = null;
                    markNotPlaced = true;

                }
            }
        }
    }

    public override void play()
    {
        _markGO.SetActive(true);
    }

    public override void stop()
    {
        _markGO.SetActive(false);
    }

    public override int edit()
    {
       
        if (_markGO.activeSelf){
             _annotationIdGO.SetActive(true);
            _annotationIdGO.transform.position = new Vector3(_markGO.transform.position.x, _markGO.transform.position.y + 0.15f, _markGO.transform.position.z);

            Vector3 rot = _head.transform.forward;
            rot.y = 0.0f;
            _annotationIdGO.transform.rotation = Quaternion.LookRotation(rot);
            return _id;
         }
        else
            return -1;
    }



    public override void increaseDuration()
    {
        if (_markGO.activeSelf) { 
            _duration += 0.5f;
            _annotationID.text = Convert.ToString(Math.Round(_duration, 1));
        }

       
    }

    public override void decreaseDuration()
    {
        if (_markGO.activeSelf && _duration >= 0) { 
            _duration -= 0.5f;
            _annotationID.text = Convert.ToString(Math.Round(_duration, 1));
        }
    }

    public override void disableDurationGO()
    {
        if(_annotationIdGO)
            _annotationIdGO.SetActive(false);
    }


    public override void reset()
    {
        if(markMenu != null)
            GameObject.Destroy(markMenu);
        if(_markGO != null)
            GameObject.Destroy(_markGO);
    }

    public override string serialize()
    {
        return getID() + "#" + getStart() + "#" + getDuration() + "#" + markName + "#" 
            + _markGO.transform.position.x + "#" + _markGO.transform.position.y + "#" + _markGO.transform.position.z;
    }

    public override void deserialize(string s)
    {
        string[] tokens = s.Split('#');
        setID(int.Parse(tokens[0]));
        setStart(float.Parse(tokens[1]));
        setDuration(float.Parse(tokens[2]));
        markName = tokens[3];

        Vector3 pos = new Vector3(float.Parse(tokens[4]), float.Parse(tokens[5]), float.Parse(tokens[6]));

        _markGO = new GameObject();
        _markGO.transform.position = pos;
        _markGO.transform.localRotation = Quaternion.identity;
        _markGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        SpriteRenderer sr = _markGO.AddComponent<SpriteRenderer>();
        sr.sprite = (Sprite)Resources.Load(markName, typeof(Sprite));
    
        _hasBeenCreated = true;
    }
    

}