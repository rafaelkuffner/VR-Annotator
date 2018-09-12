using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class EditAnnotations : MonoBehaviour {

    private InputManager inputManager;
    public GameObject _rightHand;

	// Use this for initialization
	void Start () {
        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
	}

    private void OnTriggerEnter(Collider other)
    {
        Debug.Log("entered");
    }

    private void OnTriggerExit(Collider other)
    {
        Debug.Log("exit");
    }

	// Update is called once per frame
	void Update () {
		
	}
}
