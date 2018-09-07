using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SkeletonRepresentation : MonoBehaviour {

    TrackedBodyRepresentation _tbr;

    public void setTBR(TrackedBodyRepresentation tbr)
    {
        _tbr = tbr;
    }

    public void show()
    {
        _tbr.show();
    }

    public void hide()
    {
        _tbr.hide();
    }
}
