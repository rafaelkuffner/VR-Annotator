using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleVelocity : MonoBehaviour {
    Vector3 lastPosition;

    ParticleSystem ps;
	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        float Speed = (((transform.position - lastPosition).magnitude) / Time.deltaTime);
        ParticleSystem.MainModule main = ps.main;
        main.startSpeed = Speed;
        if(Speed == 0)
        {
            main.startLifetime = 0;
        }
        else
        {
            main.startLifetime = 3;
        }

        ParticleSystem.ShapeModule shape = ps.shape;
        Vector3 direction = transform.position-lastPosition;
        shape.rotation = Quaternion.LookRotation(direction,Vector3.one).eulerAngles;
        lastPosition = transform.position;

    }
}
