﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System;

public class AnnotationManager
{

    private int currentAnnotationID;
    public bool IsAnnotationActive { get; set; }
    public bool IsPlayingVideo { get; set; }

    private GameObject currentAnimationGO;

    private bool bVisualEffect;
    private bool bScribbler;
    private bool bSpeechToText;
    private bool bMark;
    private bool bFloor;

    private Material speechMaterial;
    private Material scribblerMaterial;
    private Material visualEffectsMaterial;
    private Material markMaterial;

    private Texture deleteTexture;

    private SteamVR_Controller.Device _rightController;
    private GameObject _rightHand;
    private CloudVideoPlayer _video;
    private GameObject _head;
    private GameObject _rightPointer;
    private GameObject _annotationDurationGO;

    private ScribblerAnnotation scribblerAnnotation;
    private VisualEffectAnnotation visualEffectAnnotation;
    private MarkAnnotation markAnnotation;
    private SpeechAnnotation speechAnnotation;
    private FloorAnnotation floorAnnotation;
    private StaticAnnotation currentAnnotation;

    private InputManager inputManager;

    public int currentAnnotationSelected { get; set; }

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

    public void SetRightPointer(GameObject rightPointer)
    {
        _rightPointer = rightPointer;
    }

    public void SetHead(GameObject head)
    {
        _head = head;
    }

    public void SetAnnotationDurationGO()
    {
        //_annotationIdGO = GameObject.Instantiate(Resources.Load("Prefabs/Duration")) as GameObject;
        _annotationDurationGO = GameObject.Find("Duration");
        //_annotationIdGO.transform.localScale = new Vector3(0.01f, 0.01f, 0.01f);
        //_annotationID = _annotationDurationGO.GetComponent<TextMesh>();
        //_annotationID.text = Convert.ToString(_duration);
        //_annotationIdGO.SetActive(false);
    }

    // Use this for initialization
    public void init()
    {

        IsAnnotationActive = false;

        currentAnimationGO = GameObject.Instantiate(Resources.Load("Prefabs/CurrentAnnotation")) as GameObject;
        currentAnimationGO.SetActive(false);

        bVisualEffect = false;
        bScribbler = false;
        bSpeechToText = false;
        bMark = false;
        IsPlayingVideo = false;

        //Load menu buttons materials
        speechMaterial = Resources.Load("Materials/speechToTextMat") as Material;

        scribblerMaterial = Resources.Load("Materials/scribblerMat") as Material;

        visualEffectsMaterial = Resources.Load("Materials/highlightPointsMat") as Material;

        markMaterial = Resources.Load("Materials/markMat") as Material;

        deleteTexture = Resources.Load("Textures/deleteActive") as Texture;

        scribblerAnnotation = null;
        visualEffectAnnotation = null;
        markAnnotation = null;
        speechAnnotation = null;
        staticAnnotationList = new List<StaticAnnotation>();

        currentTime = 0.0f;
        currentAnnotationID = 0;
        currentAnnotation = null;

        inputManager = GameObject.Find("Controller (right)").GetComponentInChildren<InputManager>();
        currentAnnotationSelected = -1;
        DrawAnnotationsOnTimeline();
    }


    public void HandleVisualEffectsAnnotation(GameObject _head, GameObject _rightPointer, string effectType)
    {
        if (!bVisualEffect)
        {
            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial = visualEffectsMaterial;
            renderers[1].sharedMaterial = visualEffectsMaterial;

            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", highlightPointsTexture);
            currentAnimationGO.SetActive(true);
            currentAnnotation = visualEffectAnnotation = new VisualEffectAnnotation(_video, _rightHand, _rightController, _head, inputManager.PointerColor, effectType);
            visualEffectAnnotation.IsActive = true;
            visualEffectAnnotation.setAnnotationDurationTextMesh(_annotationDurationGO.GetComponent<TextMesh>());
            visualEffectAnnotation.setID(currentAnnotationID);
            currentAnnotationID++;
            bVisualEffect = true;
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
            currentAnnotation = scribblerAnnotation = new ScribblerAnnotation(_video, _rightHand, _rightController, inputManager.PointerColor, _head);
            scribblerAnnotation.IsActive = true;
            scribblerAnnotation.setAnnotationDurationTextMesh(_annotationDurationGO.GetComponent<TextMesh>());
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
            renderers[0].sharedMaterial = markMaterial;
            renderers[1].sharedMaterial = markMaterial;

            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", markTexture);
            currentAnimationGO.SetActive(true);
            currentAnnotation = markAnnotation = new MarkAnnotation(_video, _rightHand, _rightController, _head);
            markAnnotation.IsActive = true;
            markAnnotation.setAnnotationDurationTextMesh(_annotationDurationGO.GetComponent<TextMesh>());
            markAnnotation.setID(currentAnnotationID);
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
            currentAnnotation = speechAnnotation = new SpeechAnnotation(_video, _rightHand, _rightController, _head);
            speechAnnotation.IsActive = true;
            speechAnnotation.setAnnotationDurationTextMesh(_annotationDurationGO.GetComponent<TextMesh>());
            speechAnnotation.setID(currentAnnotationID);
            currentAnnotationID++;
            bSpeechToText = true;
        }
    }

    public void HandleFloorAnnotation()
    {
        if (!bFloor)
        {
            Renderer[] renderers = currentAnimationGO.GetComponentsInChildren<Renderer>();
            renderers[0].sharedMaterial = scribblerMaterial; // TODO
            renderers[1].sharedMaterial = scribblerMaterial;
            //currentAnimationGO.GetComponent<Renderer>().sharedMaterial.SetTexture("_MainTex", scribblerTexture);
            currentAnimationGO.SetActive(true);
            currentAnnotation = floorAnnotation = new FloorAnnotation(_video, _rightHand, _rightController, inputManager.PointerColor, _head);
            floorAnnotation.IsActive = true;
            floorAnnotation.setAnnotationDurationTextMesh(_annotationDurationGO.GetComponent<TextMesh>());
            floorAnnotation.setID(currentAnnotationID);
            currentAnnotationID++;
            bFloor = true;
        }
    }

    public void DisableAnnotations()
    {
        foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
        {
            staticAnnotation.stop();
        }
    }

    public void Reset()
    {
        if (currentAnimationGO != null) currentAnimationGO.SetActive(false);
        if (bMark && markAnnotation != null) markAnnotation.reset();
        if (bVisualEffect && visualEffectAnnotation != null) visualEffectAnnotation.reset();
        if (bScribbler && scribblerAnnotation != null) scribblerAnnotation.reset();
        if (bSpeechToText && speechAnnotation != null) speechAnnotation.reset();
        if (bFloor && floorAnnotation != null) floorAnnotation.reset();

        bVisualEffect = false;
        bScribbler = false;
        bSpeechToText = false;
        bMark = false;
        bFloor = false;

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
        foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
        {
            currentAnnotationSelected = staticAnnotation.edit();
        }
    }

    public void IncrementDuration()
    {
        currentAnnotation.increaseDuration();
       /* foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
        {
            staticAnnotation.increaseDuration();
        } */
    }

    public void DisableDurationGO()
    {
        foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
        {
            staticAnnotation.disableDurationGO();
        }
    }

    public void DecreaseDuration()
    {
        currentAnnotation.decreaseDuration();
        /*foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
        {
            staticAnnotation.decreaseDuration();
        }*/
    }

    public void DeleteAnnotation()
    {
        if (currentAnnotationSelected != -1)
        {
            foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
            {
                if (staticAnnotation.getID() == currentAnnotationSelected)
                {
                    staticAnnotation.reset();
                    staticAnnotationList.Remove(staticAnnotation);
                    currentAnnotationSelected = -1;
                    DrawAnnotationsOnTimeline();
                    return;
                }
            }

        }
    }

    public void DrawAnnotationsOnTimeline()
    {
        GameObject annotationMarks = GameObject.Find("AnnotationMarks");
        if (annotationMarks != null)
            GameObject.Destroy(annotationMarks);
        GameObject slider = GameObject.Find("Slider");
        annotationMarks = new GameObject("AnnotationMarks");
        annotationMarks.transform.SetParent(slider.transform);
        annotationMarks.transform.localPosition = Vector3.zero;
        annotationMarks.transform.localScale = Vector3.one;
        annotationMarks.transform.localRotation = Quaternion.identity;
        foreach (StaticAnnotation sa in staticAnnotationList)
        {
            float start = sa.getStart();
            float width = slider.GetComponent<RectTransform>().rect.width;
            float ratio = start / _video.getVideoDuration();
            float xposition = (ratio * width) - (width / 2);
            Sprite p = (Sprite)Resources.Load("Textures/Annotation", typeof(Sprite));
            GameObject r = new GameObject("Annotation");
            r.transform.SetParent(annotationMarks.transform);
            SpriteRenderer rend = r.AddComponent<SpriteRenderer>();
            rend.sprite = p;
            r.transform.localRotation = Quaternion.identity;
            r.transform.localPosition = new Vector3(xposition, 0, 0);
            r.transform.localScale = Vector3.one;

        }
    }

    public static bool RoughlyEqual(float a, float b)
    {
        float treshold = 0.2f; //how much roughly
        return (Math.Abs(a - b) < treshold);
    }

    // Update is called once per frame
    public void Update()
    {


        if (IsAnnotationActive)
        {

            currentAnimationGO.transform.position = new Vector3(_rightHand.transform.position.x,
                    _rightHand.transform.position.y + 0.075f, _rightHand.transform.position.z);
            Vector3 rot = Camera.main.transform.forward;
            rot.y = 0.0f;
            currentAnimationGO.transform.rotation = Quaternion.LookRotation(rot);

            if (bVisualEffect)
            {
                //Debug.Log("Start highlightPoint Annotation");
                visualEffectAnnotation.annotate();

                if (!visualEffectAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    if (visualEffectAnnotation.getHasBeenCreated())
                    {
                        staticAnnotationList.Add(visualEffectAnnotation);
                        DrawAnnotationsOnTimeline();
                    }
                    bVisualEffect = false;
                }

            }
            else if (bScribbler)
            {
                //Debug.Log("Start Scribbler Annotation");
                scribblerAnnotation.annotate();

                if (!scribblerAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    if (scribblerAnnotation.getHasBeenCreated())
                    {
                        staticAnnotationList.Add(scribblerAnnotation);
                        DrawAnnotationsOnTimeline();
                    }

                    bScribbler = false;

                }
            }
            else if (bSpeechToText)
            {
                //Debug.Log("Start SpeechToText Annotation");
                speechAnnotation.annotate();
                if (!speechAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    if (speechAnnotation.getHasBeenCreated())
                    {
                        staticAnnotationList.Add(speechAnnotation);
                        DrawAnnotationsOnTimeline();
                    }
                    bSpeechToText = false;
                }
            }

            else if (bMark)
            {
                //Debug.Log("Start Mark Annotation");
                markAnnotation.annotate();

                if (!markAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    if (markAnnotation.getHasBeenCreated())
                    {
                        staticAnnotationList.Add(markAnnotation);
                        DrawAnnotationsOnTimeline();
                    }
                    _rightPointer.SetActive(false);
                    bMark = false;
                }
            }
            else if (bFloor)
            {
                //Debug.Log("Start Floor Annotation");
                floorAnnotation.annotate();

                if (!floorAnnotation.IsActive)
                {
                    currentAnimationGO.SetActive(false);
                    if (floorAnnotation.getHasBeenCreated())
                    {
                        staticAnnotationList.Add(floorAnnotation);
                        DrawAnnotationsOnTimeline();
                    }

                    bFloor = false;

                }
            }
        }
        IsAnnotationActive = bVisualEffect || bScribbler || bSpeechToText || bMark || bFloor;

        if (_video != null && staticAnnotationList.Count > 0)
        {
            currentTime = _video.getVideoTime();
            Debug.Log("number of annotations = " + staticAnnotationList.Count);
            foreach (StaticAnnotation staticAnnotation in staticAnnotationList)
            {
                Debug.Log("duration = " + staticAnnotation.getDuration());
                if (currentTime >= staticAnnotation.getStart() && currentTime < staticAnnotation.getStart() + staticAnnotation.getDuration())
                {
                    staticAnnotation.play();
                    //_video.Play();
                }
                else
                {
                    staticAnnotation.stop();
                }

            }
        }
    }
}
