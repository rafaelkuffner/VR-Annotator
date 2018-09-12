using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnnotationManager {

    private int currentAnnotationID;
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
        currentAnnotationID = 0;

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
            highlightPointsAnnotation = new HighlightPointsAnnotation(_video, _rightHand, _rightController);
            highlightPointsAnnotation.IsActive = true;
            highlightPointsAnnotation.setID(currentAnnotationID);
            staticAnnotationList.Add(highlightPointsAnnotation);
            currentAnnotationID++;
            bHighlightPoints = true;
        }
    }

    public void HandleScribblerAnnotation()
    {
        if (!bScribbler)
        {

            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial = scribblerMaterial;
            renderers[1].sharedMaterial = scribblerMaterial;
            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", scribblerTexture);
            currentAnimationGO.SetActive(true);
            scribblerAnnotation = new ScribblerAnnotation(_video, _rightHand, _rightController,inputManager.PointerColor);
            scribblerAnnotation.IsActive = true;
            scribblerAnnotation.setID(currentAnnotationID);           
            currentAnnotationID++;
            bScribbler = true;
        }
    }

    public void HandleMarkAnnotation(GameObject _head, GameObject _rightPointer)
    {
        if (!bMark)
        {

            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial =  markMaterial;
            renderers[1].sharedMaterial = markMaterial;
            
            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", markTexture);
            currentAnimationGO.SetActive(true);
            markAnnotation = new MarkAnnotation(_video, _rightHand, _rightController, _head, _rightPointer);
            markAnnotation.IsActive = true;
            markAnnotation.setID(currentAnnotationID);
            staticAnnotationList.Add(markAnnotation);
            currentAnnotationID++;
            bMark = true;
        }
    }

    public void HandleSpeechAnnotation()
    {
        if (!bSpeechToText)
        {

            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial = speechMaterial;
            renderers[1].sharedMaterial = speechMaterial;
            
            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", speechTexture);
            currentAnimationGO.SetActive(true);
            speechAnnotation = new SpeechAnnotation(_video, _rightHand, _rightController);
            speechAnnotation.IsActive = true;
            speechAnnotation.setID(currentAnnotationID);
            staticAnnotationList.Add(speechAnnotation);
            currentAnnotationID++;
            bSpeechToText = true;
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

    public void resetStaticAnnotationList()
    {
        foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
        {
            if (staticAnnotation.getHasBeenCreated())
                staticAnnotationList.Remove(staticAnnotation);
        }
    }


    public void EditAnnotation()
    {
        foreach(StaticAnnotation staticAnnotation in staticAnnotationList){
            staticAnnotation.edit();
        }
    }

	public void DrawAnnotationsOnTimeline()
	{
		GameObject annotationMarks = GameObject.Find ("AnnotationMarks");
		if (annotationMarks != null)
			GameObject.Destroy (annotationMarks);
		GameObject slider = GameObject.Find ("Slider");
		annotationMarks = new GameObject ("AnnotationMarks");
		annotationMarks.transform.SetParent (slider.transform);
		annotationMarks.transform.localPosition = Vector3.zero;
		annotationMarks.transform.localScale = Vector3.one;
		annotationMarks.transform.localRotation = Quaternion.identity;
		foreach (StaticAnnotation sa in staticAnnotationList) 
		{
			float start = sa.getStart ();
			float width = slider.GetComponent<RectTransform> ().rect.width;
			float ratio = start / _video.getDuration ();
			float xposition = (ratio * width) - (width / 2);
			Sprite p =(Sprite) Resources.Load("Textures/Annotation", typeof(Sprite));
			GameObject r = new GameObject ("Annotation");
			r.transform.SetParent (annotationMarks.transform);
			SpriteRenderer rend = r.AddComponent<SpriteRenderer>();
			rend.sprite = p;
			r.transform.localRotation = Quaternion.identity;
			r.transform.localPosition = new Vector3 (xposition, 0, 0);
			r.transform.localScale = Vector3.one;
		}
	}

    static bool RoughlyEqual(float a, float b)
    {
        float treshold = 1.2f; //how much roughly
        return (Math.Abs(a - b) < treshold);
    }

	// Update is called once per frame
	public void Update () {

		DrawAnnotationsOnTimeline ();
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
                    if (highlightPointsAnnotation.getHasBeenCreated())
                        staticAnnotationList.Add(highlightPointsAnnotation);
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
                    if (scribblerAnnotation.getHasBeenCreated())
                        staticAnnotationList.Add(scribblerAnnotation);

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
                    if (speechAnnotation.getHasBeenCreated())
                        staticAnnotationList.Add(speechAnnotation);
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
                    if (markAnnotation.getHasBeenCreated())
                        staticAnnotationList.Add(markAnnotation);
                    bMark = false;
                }
            }
        }
        IsAnnotationActive = bHighlightPoints || bScribbler || bSpeechToText || bMark;

       if (!IsAnnotationActive) {

           Debug.Log("Static annotation n = " + staticAnnotationList.Count);
           
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
