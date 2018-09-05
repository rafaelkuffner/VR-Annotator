using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnnotationManager {


    public bool IsAnnotationActive { get; set; }
    public bool IsPlayingVideo { get; set; }

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

    public float currentTime { get; set; }

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
	public void init () {

        IsAnnotationActive = false;

        currentAnimationGO = GameObject.Instantiate(Resources.Load("Prefabs/CurrentAnnotation")) as GameObject;
        currentAnimationGO.SetActive(false);

        bHighlightPoints = false;
        bScribbler = false;
        bSpeechToText = false;
        bMark = false;
        IsPlayingVideo = false;

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

        currentTime = 0.0f;

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

    public void DisableAnnotations()
    {
        foreach (StaticAnnotation staticAnnotation in staticAnnotationList) {
            staticAnnotation.stop();
        }
    }

    public void Reset()
    {
        currentAnimationGO.SetActive(false);
        bHighlightPoints = false;
        bScribbler = false;
        bSpeechToText = false;
        bMark = false;
    }


    static bool RoughlyEqual(float a, float b)
    {
        float treshold = 1.2f; //how much roughly
        return (Math.Abs(a - b) < treshold);
    }

	// Update is called once per frame
	public void Update () {

        currentTime += Time.deltaTime;

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
                highlightPointsAnnotation.setStart(currentTime);
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
                scribblerAnnotation.setStart(currentTime);
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
                speechAnnotation.setStart(currentTime);
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
                markAnnotation.setStart(currentTime);
                markAnnotation.annotate();

                if (!markAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    bMark = false;
                }
            }
        }
        IsAnnotationActive = bHighlightPoints || bScribbler || bSpeechToText || bMark;

       if (!IsAnnotationActive) { 
            foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
            {
                if (RoughlyEqual(staticAnnotation.getStart(), currentTime)) {
                    _video.Pause();
                    staticAnnotation.play();
                }

                if (currentTime > staticAnnotation.getStart() && RoughlyEqual(staticAnnotation.getStart() + staticAnnotation.getDuration(), currentTime)) 
                {
                    staticAnnotation.stop();
                    _video.Play();
                }
            }
        }

       if (IsPlayingVideo)
       {
           Time.timeScale = 1.0f;
       }
       else
       {
           Time.timeScale = 0.0f;
       }

	}
}
