using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class particleVelocityColor : MonoBehaviour {
    Vector3 lastPosition;
    public float maxSpeed = 30;
    ParticleSystem ps;
	// Use this for initialization
	void Start () {
        ps = GetComponent<ParticleSystem>();
	}
	
	// Update is called once per frame
	void Update () {
        float Speed = (((transform.position - lastPosition).magnitude) / Time.deltaTime);
        ParticleSystem.MainModule main = ps.main;
        main.startColor = convert_to_rgb(0, maxSpeed, Speed);
        lastPosition = transform.position;
    }

    Color convert_to_rgb(float minimum, float maximum, float value) {
        float ratio = 2 * (value - minimum) / (maximum - minimum);
        float b = Mathf.Max(0, (1 - ratio));
        float r = Mathf.Max(0, (ratio - 1));
        float g = 1 - b - r;
        return new Color(r, g, b);
    }
}
