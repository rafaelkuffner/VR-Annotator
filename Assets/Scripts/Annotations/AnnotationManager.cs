﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class AnnotationManager : MonoBehaviour {


    public bool IsAnnotationActive { get; set; }

    private GameObject currentAnimationGO;

    private bool bHighlightPoints;
    private bool bScribbler;
    private bool bSpeechToText;
    private bool bMark;

    public GameObject speechToTextGO;
    private Material speechMaterial;

    public GameObject scribblerGO;
    private Material scribblerMaterial;

    public GameObject highlightPointsGO;
    private Material highlightPointsMaterial;

    public GameObject markGO;
    private Material markMaterial;

    public GameObject deleteGO;
    private Texture deleteTexture;

    private SteamVR_Controller.Device _rightController;
    private GameObject _rightHand;
    private CloudVideoPlayer _video;

    private ScribblerAnnotation scribblerAnnotation;
    private HighlightPointsAnnotation highlightPointsAnnotation;
    private MarkAnnotation markAnnotation;
    private SpeechAnnotation speechAnnotation;

    private InputManager inputManager;

    public List<StaticAnnotation> staticAnnotationList;

    public void SetRightHand(GameObject rightHand)
    {
        _rightHand = rightHand;
    }

    public void SetCloudVideo(CloudVideoPlayer video)
    {
        _video = video;
    }

    public void SetRightHandController(SteamVR_Controller.Device rightController)
    {
        _rightController = rightController;
    }

    // Use this for initialization
	void Start () {

        IsAnnotationActive = false;

        currentAnimationGO = Instantiate(Resources.Load("Prefabs/CurrentAnnotation")) as GameObject;
        currentAnimationGO.SetActive(false);

        bHighlightPoints = false;
        bScribbler = false;
        bSpeechToText = false;
        bMark = false;

        //Load menu buttons materials
        speechMaterial = Resources.Load("Materials/speechToTextMat") as Material;

        scribblerMaterial = Resources.Load("Materials/scribblerMat") as Material;

        highlightPointsMaterial = Resources.Load("Materials/highlightPointsMat") as Material;

        markMaterial = Resources.Load("Materials/markMat") as Material;

        deleteTexture = Resources.Load("Textures/deleteActive") as Texture;

        scribblerAnnotation = null;
        highlightPointsAnnotation = null;
        markAnnotation = null;
        speechAnnotation = null;
        staticAnnotationList = new List<StaticAnnotation>();

        inputManager = GameObject.Find("InputManager").GetComponent<InputManager>();
    }

    
    public void HandleHighlightPointsAnnotation()
    {
        if (!bHighlightPoints)
        {
            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial = highlightPointsMaterial;
            renderers[1].sharedMaterial = highlightPointsMaterial;

            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", highlightPointsTexture);
            currentAnimationGO.SetActive(true);
            bHighlightPoints = true;
            highlightPointsAnnotation = new HighlightPointsAnnotation(_video, _rightHand, _rightController);
            highlightPointsAnnotation.IsActive = true;
            staticAnnotationList.Add(highlightPointsAnnotation);
        }
    }

    public void HandleScribblerAnnotation()
    {
        if (!bScribbler) {

            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial = scribblerMaterial;
            renderers[1].sharedMaterial = scribblerMaterial;
            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", scribblerTexture);
            currentAnimationGO.SetActive(true);
            bScribbler = true;
            scribblerAnnotation = new ScribblerAnnotation(_video, _rightHand, _rightController,inputManager.PointerColor);
            scribblerAnnotation.IsActive = true;
            staticAnnotationList.Add(scribblerAnnotation);
            
        }
    }

    public void HandleMarkAnnotation(GameObject _head, GameObject _rightPointer)
    {
        if (!bMark) {

            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial =  markMaterial;
            renderers[1].sharedMaterial = markMaterial;
            
            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", markTexture);
            currentAnimationGO.SetActive(true);
            bMark = true;
            markAnnotation = new MarkAnnotation(_video, _rightHand, _rightController, _head, _rightPointer);
            markAnnotation.IsActive = true;
            staticAnnotationList.Add(markAnnotation);
        }
    }

    public void HandleSpeechAnnotation()
    {
        if (!bSpeechToText) {

            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial = speechMaterial;
            renderers[1].sharedMaterial = speechMaterial;
            
            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", speechTexture);
            currentAnimationGO.SetActive(true);
            bSpeechToText = true;
            speechAnnotation = new SpeechAnnotation(_video, _rightHand, _rightController);
            speechAnnotation.IsActive = true;
            staticAnnotationList.Add(speechAnnotation);
        }
    }

	// Update is called once per frame
	void Update () {

        if (IsAnnotationActive) {
            Debug.Log("number of static annotation = " + staticAnnotationList.Count);
            currentAnimationGO.transform.position = new Vector3(_rightHand.transform.position.x,
                    _rightHand.transform.position.y + 0.075f, _rightHand.transform.position.z);
            Vector3 rot = Camera.main.transform.forward;
            rot.y = 0.0f;
            currentAnimationGO.transform.rotation = Quaternion.LookRotation(rot);
      
            if (bHighlightPoints)
            {
                Debug.Log("Start highlightPoint Annotation");
                highlightPointsAnnotation.annotate();

                if (!highlightPointsAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    bHighlightPoints = false;
                }
               
            }
            else if(bScribbler) 
            {
                Debug.Log("Start Scribbler Annotation");
                scribblerAnnotation.annotate();
                
                if (!scribblerAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    bScribbler = false;
                }
            }
            else if (bSpeechToText)
            {
                Debug.Log("Start SpeechToText Annotation");
                speechAnnotation.annotate();
                if (!speechAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    bSpeechToText = false;
                }
            }

            else if (bMark)
            {
                Debug.Log("Start Mark Annotation");
                markAnnotation.annotate();

                if (!markAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    bMark = false;
                }
            }
        }
        IsAnnotationActive = bHighlightPoints || bScribbler || bSpeechToText || bMark;

	}
}
