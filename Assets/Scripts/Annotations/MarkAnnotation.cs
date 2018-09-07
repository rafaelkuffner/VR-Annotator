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
    string markName;

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
        markName = "";

    }

    public override void annotate()
    {

        if (!IsActive)
            Debug.Log("NOT MARKING ANNOTATING");

        else
        {
            markMenu.SetActive(true);
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
                            _start = Time.time;
                            Debug.Log("MARK = " + b.name);
                            IsActive = false;
                            markName = b.name;
                            MonoBehaviour.Destroy(markMenu);
                            _rightPointer.SetActive(false);
                            
                        }
                    }
                }
            }
        }
    }

    public override void play()
    {
        throw new System.NotImplementedException();
    }

    public override void stop()
    {
        throw new System.NotImplementedException();
    }

    public override void edit()
    {
        throw new System.NotImplementedException();
    }
}
