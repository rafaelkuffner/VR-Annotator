using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScribblerAnnotation : StaticAnnotation {

    private GameObject lineRendererGO { get; set; }
    public LineRenderer lineRenderer { get; set; }
    public bool IsActive { get; set; }
    private List<Vector3> _myPoints;
    private bool triggerPressed;

   

    public ScribblerAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController) :
        base(video, rightHand, rightController)
    {
        _myPoints = new List<Vector3>();
        IsActive = false;
        triggerPressed = false;

        lineRendererGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/LineRendererPrefab")) as GameObject;
        lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        lineRenderer.material = new Material(Shader.Find("Particles/Additive"));
       // lineRenderer.material = Resources.Load("Materials/ParticleAfterburner") as Material;
        Color color = new Color(Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f), Random.Range(0.0f, 1.0f));
        lineRenderer.startColor = color;
        Debug.Log("Start Color = " + color);
        //lineRenderer.endColor = new Color(color.r / 0.2f, color.g / 0.2f, color.b / 0.2f);
        //Debug.Log("End Color = " + lineRenderer.endColor);
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
                _start = Time.time;
                Debug.Log("start = " + _start);
            }
            
            if(triggerPressed)
            {
                _myPoints.Add(_rightHand.transform.position);

                if (_myPoints != null)
                {
                    lineRenderer.positionCount = _myPoints.Count;
                   // Debug.Log("MyPoints Count B = " + _myPoints.Count);
                    for (int i = 0; i < _myPoints.Count; i++) { 
                        lineRenderer.SetPosition(i, _myPoints[i]);
                    }
                }
            }

            if(_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerPressed = false;
                IsActive = false;
                _duration = Time.time - _start;
                Debug.Log("duration = " + _duration);
            }
        } 
    }

}
