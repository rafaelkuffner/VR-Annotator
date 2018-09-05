using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ScribblerAnnotation : StaticAnnotation {

    private GameObject lineRendererGO { get; set; }
    public LineRenderer lineRenderer { get; set; }
    public bool IsActive { get; set; }
    private List<Vector3> _myPoints;
    private bool triggerPressed;
    private BoxCollider boxCollider;
    
    public ScribblerAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController,Color c) :
        base(video, rightHand, rightController)
    {
        _myPoints = new List<Vector3>();
        IsActive = false;
        triggerPressed = false;
 
        lineRendererGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/LineRendererPrefab")) as GameObject;
        lineRenderer = lineRendererGO.GetComponent<LineRenderer>();
        boxCollider = lineRendererGO.GetComponent<BoxCollider>();
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
                int nPoints = _myPoints.Count;
                if (nPoints > 2)
                {
                    Vector3 startPoint = _myPoints[0];
                    Vector3 endPoint = _myPoints[nPoints - 1];
                    addColliderToLine(startPoint, endPoint);
                }
                triggerPressed = false;
                IsActive = false;
                //_duration = Time.deltaTime - _start;
                Debug.Log("duration = " + _duration);
            }
        } 
    }

    // TODO: improve collider accuracy.. this adds a box collider that encompasses the complete line renderer... maybe add several coliderers?
    private void addColliderToLine(Vector3 startPos, Vector3 endPos)
    {
        float lineLength = Vector3.Distance(startPos, endPos) + 0.5f; // length of line TODO check if this is right!!!
        boxCollider.size = new Vector3(lineLength, 0.1f, 1f); // size of collider is set where X is length of line, Y is width of line, Z will be set as per requirement
        Vector3 midPoint = (startPos + endPos) / 2;
        boxCollider.transform.position = midPoint; // setting position of collider object
        // Following lines calculate the angle between startPos and endPos
        float angle = Mathf.Atan2((endPos.z - startPos.z), (endPos.x - startPos.x));
        angle *= Mathf.Rad2Deg;
        angle *= -1;
        boxCollider.transform.Rotate(angle, angle, 0);
        /*float angle = (Mathf.Abs(startPos.y - endPos.y) / Mathf.Abs(startPos.x - endPos.x));
        if ((startPos.y < endPos.y && startPos.x > endPos.x) || (endPos.y < startPos.y && endPos.x > startPos.x))
        {
            angle *= -1;
        }
        angle = Mathf.Rad2Deg * Mathf.Atan(angle);
        boxCollider.transform.Rotate(0, 0, angle); */
    }

    public override void play()
    {
        lineRendererGO.SetActive(true);
    }

    public override void stop()
    {
        lineRendererGO.SetActive(false);
    }

}
