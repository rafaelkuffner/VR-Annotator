using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ParticleAnnotation : DynamicAnnotation {

    public ParticleAnnotation(GameObject rightHand, SteamVR_Controller.Device rightController) :
        base(rightHand, rightController) 
    {
        //TODO
        
    }

    public override void annotate()
    {
        throw new System.NotImplementedException();
    }
}
