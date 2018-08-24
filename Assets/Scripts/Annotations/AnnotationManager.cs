using System.Collections;
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
    private Texture speechToTextTextureActive;
  //  private Texture speechToTextTextureInactive;

    public GameObject scribblerGO;
    private Texture scribblerTextureActive;
 //   private Texture scribblerTextureInactive;

    public GameObject highlightPointsGO;
    private Texture highlightPointsTextureActive;
 //   private Texture highlightPointsTextureInactive;

    public GameObject markGO;
    private Texture markTextureActive;
 //   private Texture markTextureInactive;

    public GameObject deleteGO;
    private Texture deleteTextureActive;
 //   private Texture deleteTextureInactive;

    private SteamVR_Controller.Device _rightController;
    private GameObject _rightHand;
    private CloudVideoPlayer _video;

    private ScribblerAnnotation scribblerAnnotation;
    private HighlightPointsAnnotation highlightPointsAnnotation;
    private MarkAnnotation markAnnotation;
    private SpeechToTextAnnotation speechToTextAnnotation;

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

        //Load menu buttons textures
        speechToTextTextureActive = Resources.Load("Textures/speechToTextActive") as Texture;
      //  speechToTextTextureInactive = Resources.Load("Textures/speechToText") as Texture;

        scribblerTextureActive = Resources.Load("Textures/scribblerActive") as Texture;
   //     scribblerTextureInactive = Resources.Load("Textures/scribbler") as Texture;

        highlightPointsTextureActive = Resources.Load("Textures/highlightPointsActive") as Texture;
   //     highlightPointsTextureInactive = Resources.Load("Textures/highlightPoints") as Texture;

        markTextureActive = Resources.Load("Textures/markActive") as Texture;
     //   markTextureInactive = Resources.Load("Textures/mark") as Texture;

        deleteTextureActive = Resources.Load("Textures/deleteActive") as Texture;
   //     deleteTextureInactive = Resources.Load("Textures/delete") as Texture;

        scribblerAnnotation = null;
        highlightPointsAnnotation = null;
        markAnnotation = null;
        speechToTextAnnotation = null;
        staticAnnotationList = new List<StaticAnnotation>();
      //  DisableAnnotations();

    }

    
    public void HandleHighlightPointsAnnotation()
    {
        if (!bHighlightPoints)
        {
            //DisableAnnotations();
            currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", highlightPointsTextureActive);
            currentAnimationGO.SetActive(true);
            //highlightPointsGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", highlightPointsTextureActive);
            bHighlightPoints = true;
            highlightPointsAnnotation = new HighlightPointsAnnotation(_video, _rightHand, _rightController);
            highlightPointsAnnotation.IsActive = true;
            staticAnnotationList.Add(highlightPointsAnnotation);
        }
    }

    public void HandleScribblerAnnotation()
    {
        if (!bScribbler) { 

           // DisableAnnotations();
            currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", scribblerTextureActive);
            currentAnimationGO.SetActive(true);
            //scribblerGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", scribblerTextureActive);
            bScribbler = true;
            scribblerAnnotation = new ScribblerAnnotation(_video, _rightHand, _rightController);
            scribblerAnnotation.IsActive = true;
            staticAnnotationList.Add(scribblerAnnotation);
            
        }
    }

    public void HandleMarkAnnotation(GameObject _head, GameObject _rightPointer)
    {
        if (!bMark) { 
           // DisableAnnotations();
            currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", markTextureActive);
            currentAnimationGO.SetActive(true);
            //markGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", markTextureActive);
            bMark = true;
            markAnnotation = new MarkAnnotation(_video, _rightHand, _rightController, _head, _rightPointer);
            markAnnotation.IsActive = true;
            staticAnnotationList.Add(markAnnotation);
        }
    }

   

    public void HandleSpeechToTextAnnotation()
    {
        if (!bSpeechToText) { 
           // DisableAnnotations();
            currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", speechToTextTextureActive);
            currentAnimationGO.SetActive(true);
            //speechToTextGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", speechToTextTextureActive);
            bSpeechToText = true;
            speechToTextAnnotation = new SpeechToTextAnnotation(_video, _rightHand, _rightController);
            speechToTextAnnotation.IsActive = true;
            staticAnnotationList.Add(speechToTextAnnotation);
        }
    }

 /*   public void DisableAnnotations()
    {
        bHighlightPoints = false;
        highlightPointsGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", highlightPointsTextureInactive);
        bScribbler = false;
        scribblerGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", scribblerTextureInactive);
        bSpeechToText = false;
        speechToTextGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", speechToTextTextureInactive);
        bMark = false;
        markGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", markTextureInactive);
    }
*/
   
	// Update is called once per frame
	void Update () {

        if (IsAnnotationActive) {
            Debug.Log("number of static annotation = " + staticAnnotationList.Count);
            currentAnimationGO.transform.position = new Vector3(_rightHand.transform.position.x,
                    _rightHand.transform.position.y + 0.15f, _rightHand.transform.position.z);
            Vector3 rot = Camera.main.transform.forward;
            rot.y = 0.0f;
            rot.z = 90.0f;
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
                speechToTextAnnotation.annotate();
                if (!speechToTextAnnotation.IsActive)
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
