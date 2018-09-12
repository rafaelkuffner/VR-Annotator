using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TrailAnnotation : DynamicAnnotation {

    private GameObject trailGO;
    private TrailRenderer trailRenderer;
	   
    public TrailAnnotation(GameObject rightHand, SteamVR_Controller.Device rightController) :
        base(rightHand, rightController) 
    {

        trailGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/Trail")) as GameObject;
        trailRenderer = trailGO.GetComponent<TrailRenderer>();
        trailGO.SetActive(false);
    
    }
    

    public override void annotate()
    {
        //TODO: test
        // get TrackedBodyRepresentation instance
        // use findNeartestJoint to attach trail

        throw new System.NotImplementedException();
    }
}
