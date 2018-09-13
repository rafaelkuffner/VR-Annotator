using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class MarkAnnotation : StaticAnnotation
{
    public bool IsActive { get; set; }
    private bool triggerPressed;
    GameObject markMenu;
    GameObject _rightPointer;
    GameObject _head;
    GameObject _markGO;
    

    public MarkAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,
        GameObject head, GameObject rightPointer) :
        base(video, rightHand, rightController) 
    {
        IsActive = false;
        triggerPressed = false;
        _rightPointer = rightPointer;
        _head = head;

        markMenu = MonoBehaviour.Instantiate(Resources.Load("Prefabs/MarkMenu")) as GameObject;
        markMenu.transform.position = _head.transform.position + (_head.transform.forward * 2);
        Vector3 rot = Camera.main.transform.forward;
        rot.y = 0.0f;
        markMenu.transform.rotation = Quaternion.LookRotation(rot);
        markMenu.name = "MarkSelect";
        markMenu.SetActive(false);
        _markGO = null;
    }

    public override void annotate()
    {

        if (IsActive)
        {
            if (_markGO == null) {

                markMenu.SetActive(true);
                markMenu.transform.position = new Vector3(markMenu.transform.position.x, 1.4f, markMenu.transform.position.z);
                _rightPointer.SetActive(true);
                Ray raycast = new Ray(_rightHand.transform.position, _rightHand.transform.forward);
                RaycastHit hit;
                bool bHit = Physics.Raycast(raycast, out hit);
                if (hit.transform != null)
                {
                    Button b = hit.transform.gameObject.GetComponent<Button>();
                    if (b != null)
                    {
                        b.Select();

                        if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                        {
                            if (b != null)
                            {
                                Debug.Log("MARK = " + b.name);
                                MonoBehaviour.Destroy(markMenu);
                                _rightPointer.SetActive(false);
                                _markGO = new GameObject();
                                _markGO.transform.parent = _rightHand.transform;
                                _markGO.transform.localPosition = new Vector3(0, 0, 0.1f);
                                _markGO.transform.localRotation = Quaternion.identity; 
                                _markGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                                SpriteRenderer sr = _markGO.AddComponent<SpriteRenderer>();
                                sr.sprite = (Sprite)Resources.Load(b.name, typeof(Sprite));
                            
                            }
                        }
                    }
                }
            }
            else
            {
                if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
                {
                    _start = _video.getTime();
                    IsActive = false;
                    _hasBeenCreated = true;
                    _markGO.transform.parent = null;

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

    public override void edit()
    {
        throw new System.NotImplementedException();
    }

    public override void reset()
    {
        if(markMenu != null)
            GameObject.Destroy(markMenu);
        if(_markGO != null)
            GameObject.Destroy(_markGO);
    }
}
