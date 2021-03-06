﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;
using System.Xml;

public class InputManager : MonoBehaviour {

    public GameObject _rightHand;
    public GameObject _leftHand;
    public GameObject _head;
    private SteamVR_TrackedObject _rightObj = null;
    private SteamVR_TrackedObject _leftObj = null;
    private SteamVR_Controller.Device _rightController;
    private SteamVR_Controller.Device _leftController;
    private SteamVR_Controller.Device device;
    private MenuOpened _menu;
    private CloudVideoPlayer _video;
    private float _playSpeed;

    //Laser Pointer Variables
    private GameObject _rightHolder;
    private GameObject _rightPointer;
    private float _pointerThickness = 0.005f;
    private Color _pointerColor;

	private string _representation;

    public Color PointerColor
    {
        get { return _pointerColor; }
        set { _pointerColor = value; }
    }

    private GameObject _leftHolder;
    private GameObject _leftPointer;

    public bool _playing { get; set; }

    private GameObject _slider;
    private GameObject _menuGO;
    private AnnotationManager _annotationManager;

    private Sprite spriteButtonSelected;
    private Sprite spriteButton;
    private Button buttonSelected;


    private Dictionary<CloudVideoPlayer, AnnotationManager> annotationManagerByVideo;

    public enum MenuOpened
    {
        DatasetSelect,ColorSelect,AnnotationSelect,AnnotationEdit,None
    }

	private bool itemSelected;
	private Transform currentItem;
    private bool isChoosingColor;

    void setupRightPointer()
    {
        _rightHolder = new GameObject();
        _rightHolder.transform.parent = _rightHand.transform;
        _rightHolder.transform.localPosition = Vector3.zero;
        _rightHolder.transform.localRotation = Quaternion.identity;

        _rightPointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _rightPointer.transform.parent = _rightHolder.transform;
        _rightPointer.transform.localScale = new Vector3(_pointerThickness, _pointerThickness, 100f);
        _rightPointer.transform.localPosition = new Vector3(0f, -0.046f, 50f);
        _rightPointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = _rightPointer.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        Rigidbody rigidBody = _rightPointer.AddComponent<Rigidbody>();
        rigidBody.isKinematic = true;

        _pointerColor = new Color(0.0f, 0.6f, 0.6f);
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", _pointerColor);
        _rightPointer.GetComponent<MeshRenderer>().material = newMaterial;
    }

    void setupLeftPointer()
    {
        _leftHolder = new GameObject();
        _leftHolder.transform.parent = _leftHand.transform;
        _leftHolder.transform.localPosition = Vector3.zero;
        _leftHolder.transform.localRotation = Quaternion.identity;

        _leftPointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _leftPointer.transform.parent = _leftHolder.transform;
        _leftPointer.transform.localScale = new Vector3(_pointerThickness, _pointerThickness, 100f);
        _leftPointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        _leftPointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = _leftPointer.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        Rigidbody rigidBody = _leftPointer.AddComponent<Rigidbody>();
        rigidBody.isKinematic = true;

        _pointerColor = new Color(0.5f, 0.5f, 0.5f);
        Material newMaterial = new Material(Shader.Find("Unlit/Color"));
        newMaterial.SetColor("_Color", _pointerColor);
        _leftPointer.GetComponent<MeshRenderer>().material = newMaterial;
    }
    // Use this for initialization

    void Start()
    {

        _rightObj = _rightHand.GetComponent<SteamVR_TrackedObject>();
        _leftObj = _leftHand.GetComponent<SteamVR_TrackedObject>();
        setupRightPointer();
        setupLeftPointer();

        _annotationManager = null;

        DisableRightPointer();
        DisableLeftPointer();
        _menu = MenuOpened.None;
        _video = null;
        _playSpeed = 1;
        _slider = GameObject.Find("Timeline");
        _representation = "Full";
        annotationManagerByVideo = new Dictionary<CloudVideoPlayer, AnnotationManager>();

        _annotationManager = new AnnotationManager();
        _annotationManager.init();
        itemSelected = false;

        spriteButtonSelected = (Sprite)Resources.Load("Textures/white", typeof(Sprite));
        spriteButton = (Sprite)Resources.Load("Textures/border2", typeof(Sprite));

       
       
   

    }

    void EnableRightPointer()
    {
        _rightPointer.SetActive(true);
    }

    void EnableLeftPointer()
    {
        _leftPointer.SetActive(true);
    }
    void DisableRightPointer()
    {
        _rightPointer.SetActive(false);
    }
    void DisableLeftPointer()
    {
        _leftPointer.SetActive(false);
    }

    // select menu
    void OnCollisionStay(Collision collision)
    {
        // only annotates when a dataset is selected
        if (_video != null)
        {
            if (collision.transform.name.Contains("annotation") && !itemSelected && _rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                //collision.transform.position = new Vector3 (collision.transform.position.x - 0.05f, collision.transform.position.y , collision.transform.position.z);
                Renderer renderer = collision.transform.gameObject.GetComponent<Renderer>();
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", Color.cyan);

                itemSelected = true;
                currentItem = collision.transform;

                string AnnotationMenuItem = collision.transform.name;
                string[] AnnotationMenuItemElements = AnnotationMenuItem.Split('.');
                if (AnnotationMenuItemElements.Length == 2)
                {
                    string annotationType = AnnotationMenuItemElements[1];
                    SelectAnnotationType(annotationType);
                }

            }
            else if (collision.transform.name.Contains("visualeffect") && !itemSelected && _rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                //hit.transform.position = new Vector3 (hit.transform.position.x - 0.05f, hit.transform.position.y , hit.transform.position.z);
                Renderer renderer = collision.transform.gameObject.GetComponent<Renderer>();
                renderer.material.EnableKeyword("_EMISSION");
                renderer.material.SetColor("_EmissionColor", Color.cyan);
                itemSelected = true;
                currentItem = collision.transform;

                string visualEffectMenuItem = collision.transform.name;
                string[] visualEffectMenuItemElements = visualEffectMenuItem.Split('.');
                if (visualEffectMenuItemElements.Length == 2)
                {
                    string visualEffect = visualEffectMenuItemElements[1];
                    SelectVisualEffectType(visualEffect);
                }
            }

            else if (collision.transform.name.Contains("representation")  && _rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                string representationMenuItem = collision.transform.name;
                string[] representationMenuItemElements = representationMenuItem.Split('.');
                if (representationMenuItemElements.Length == 2)
                {
                    string dataRepresentation = representationMenuItemElements[1];
                    SetRepresentation(dataRepresentation);
                }
            }

            else if (collision.transform.name.Contains("ColorPalette") && _rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                isChoosingColor = true;
                SelectColor(collision);
                
            }
        }

        if(collision.transform != null && collision.transform.name.Contains(".ini") && _rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
        {
            SelectDataset(collision.gameObject);
        }
        isChoosingColor = false;
    }

    // deactivates the selected menu
    private void OnCollisionEnter(Collision collision)
    {
        if (currentItem != null && collision.transform.name.Equals(currentItem.name) && itemSelected)
        {
            //currentItem.position = new Vector3 (currentItem.position.x + 0.05f, currentItem.position.y , currentItem.position.z);
            Renderer renderer = collision.transform.gameObject.GetComponent<Renderer>();
            renderer.material.SetColor("_EmissionColor", Color.clear);

            if (currentItem.name.Contains("mark"))
            {
                GameObject markMenu = GameObject.FindGameObjectWithTag("MarkMenu");
                Transform panel = markMenu.transform.Find("Panel");
                panel.gameObject.SetActive(false);
                DisableRightPointer();
            }

            itemSelected = false;
            currentItem = null;

            _annotationManager.IsAnnotationActive = false;
            _annotationManager.Reset();
        }
    }

    public void SetPlaybackSpeed(float speed)
    {
        _playSpeed = speed;
        print("Changing video speed to " + speed);
        _video.setSpeed(speed);
    }

    void SetRepresentation(string representation)
    {
        GameObject o = GameObject.Find("Avatar");
        switch (representation)
        {
            case "full":
                _video.setRendering(true);
                if (o != null) o.GetComponent<SkeletonRepresentation>().show();
                break;
            case "skeleton":
                _video.setRendering(false);
                if (o != null) o.GetComponent<SkeletonRepresentation>().show();
                break;
            case "cloud":
                _video.setRendering(true);
                if (o != null) o.GetComponent<SkeletonRepresentation>().hide();
                break;
            case "none":
                _video.setRendering(false);
                if (o != null) o.GetComponent<SkeletonRepresentation>().hide();
                break;
        }
		_representation = representation;
    }
	    
	void SelectDataset(GameObject hit)
	{
        if(buttonSelected != null)
        {
            Image buttonImg = buttonSelected.GetComponent<Image>();
            buttonImg.sprite = spriteButton;
        }

        if (_video != null)
        {
            _video.Close();
            SaveAnnotations();
        }

		_video = new CloudVideoPlayer(hit.name,this);
        buttonSelected = hit.transform.gameObject.GetComponent<Button>();
        Image img = buttonSelected.GetComponent<Image>();
        img.sprite = spriteButtonSelected;
        LoadAnnotations();
        _annotationManager.SetCloudVideo(_video);

	}

    void LoadAnnotations()
    {
        _annotationManager.resetStaticAnnotationList();
        string name = _video.configFile;
        name = name.Replace(".ini", ".xml");
        if (System.IO.File.Exists(name))
        {
            XmlDocument d = new XmlDocument();
            d.Load(XmlReader.Create(name));
            XmlNodeList l = d.SelectNodes("/Annotations/Annotation");
            foreach(XmlNode n in l)
            {
                string type = n.Attributes["type"].Value;
                System.Type t = System.Type.GetType(type);
                StaticAnnotation a = System.Activator.CreateInstance(t, _video, _rightHand, _rightController, _head) as StaticAnnotation;
                a.deserialize(n.InnerText);
                _annotationManager.staticAnnotationList.Add(a);
            }
        }
    }
    void SaveAnnotations ()
    {
        string name = _video.configFile;
        name = name.Replace(".ini", ".xml");
        using (XmlWriter writer = XmlWriter.Create(name))
        {
            writer.WriteStartDocument();
            writer.WriteStartElement("Annotations");
            foreach (StaticAnnotation a in _annotationManager.staticAnnotationList)
            {
                writer.WriteStartElement("Annotation");
                writer.WriteAttributeString("type", a.GetType().ToString());
                writer.WriteRaw(a.serialize());
                writer.WriteEndElement();
            }
            writer.WriteEndElement();
            writer.WriteEndDocument();
            writer.Close();
        }
    }

	void SelectColor(Collision collision)
    {
        if (currentItem != null) return; // so it doesnt annotate at the same time is picking a color

        Ray raycast = new Ray(transform.position, _rightPointer.transform.forward);
        //Debug.DrawRay (_rightHand.transform.position, _rightHand.transform.forward * 100f, Color.green, 20, true);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);
        Renderer rend = hit.transform.GetComponent<Renderer>();
        Texture2D tex = rend.material.mainTexture as Texture2D;
        Vector2 pixelUV = hit.textureCoord;
        pixelUV.x *= tex.width;
        pixelUV.y *= tex.height;
        Color p = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
        //if (_rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger))
        // {
        this.transform.GetComponent<Renderer>().material.SetColor("_Color", p);
        _rightPointer.GetComponent<MeshRenderer>().material.SetColor("_Color", p);
        _pointerColor = p;  
    }

    void SelectAnnotationType(string annotationType)
    {
		switch (annotationType)
		{
			case "scribble":
				_annotationManager.HandleScribblerAnnotation ();
				break;

			case "voice":
				_annotationManager.HandleSpeechAnnotation ();
				break;

            case "floor":
                _annotationManager.HandleFloorAnnotation();
                break;

            case "mark":
                GameObject markMenu = GameObject.FindGameObjectWithTag("MarkMenu");
                Transform panel = markMenu.transform.Find("Panel");
                panel.gameObject.SetActive(true);
                _annotationManager.HandleMarkAnnotation(_head, _rightPointer);
				break;

			default:
				Debug.Log ("Invalid Annotation Type");
				break;
		}
    }

	void SelectVisualEffectType(string effectType)
	{
		switch (effectType)
		{
		case "trail":
			_annotationManager.HandleVisualEffectsAnnotation (_head, _rightPointer, effectType);
			break;

		case "particlesSpeed":
			_annotationManager.HandleVisualEffectsAnnotation (_head, _rightPointer, effectType);
			break;

		case "highlightSpeed": 
			_annotationManager.HandleVisualEffectsAnnotation (_head, _rightPointer, effectType);
			break;

		default:
			Debug.Log ("Invalid Annotation Type");
			break;
		}
	}


    void EditAnnotations()
    {
        if (_rightController.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (!_annotationManager.IsAnnotationActive)
            {
                Vector2 touchpad = _rightController.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
                //Debug.Log("Axis x = " + touchpad.x + " | Axis y = " + touchpad.y);

                if (touchpad.y > 0.7f)
                {
                    _annotationManager.DisableDurationGO();
                    _annotationManager.DeleteAnnotation();
                    _annotationManager.currentAnnotationSelected = -1;
                    _menu = MenuOpened.None;
                    CloseAllMenus();
                }
                else if (touchpad.y < -0.7f)
                {
                    _annotationManager.currentAnnotationSelected = -1;
                    _annotationManager.DisableDurationGO();
                    _menu = MenuOpened.None;
                    CloseAllMenus();
                }
                else if (touchpad.x > 0.7f)
                {
                    _annotationManager.IncrementDuration();
                }
                else if (touchpad.x < -0.7f)
                {
                    _annotationManager.DecreaseDuration();
                }
                /*else
                {
                    _annotationManager.IsAnnotationActive = false;
                    _annotationManager.DisableAnnotations();
                } */
            }
        }  
    }

    void CloseAllMenus()
    {
        GameObject o = GameObject.Find("DatasetSelect");
        if (o != null) Destroy(o);
        o = GameObject.Find("ColorSelect");
        if (o != null) Destroy(o);
        o = GameObject.Find("AnnotationSelect");
        if (o != null) Destroy(o);
        o = GameObject.Find("EditAnnotationMenu");
        if (o != null) Destroy(o);
        DisableLeftPointer();
        DisableRightPointer();
        return;
    }

    void HandleAnnotations()
    {
        // continue to add annotations while a menu item is still selected
        if (currentItem != null && !isChoosingColor)
        {

            string name = currentItem.name;
            string[] tmp = name.Split('.');
            if (tmp.Length == 2)
            {
                string menuType = tmp[0];

                switch (menuType)
                {

                    case "annotation":
                        if (!_annotationManager.IsAnnotationActive)
                            SelectAnnotationType(tmp[1]);
                        //Debug.Log("annotation");
                        break;

                    case "visualeffect":
                        if (!_annotationManager.IsAnnotationActive)
                            SelectVisualEffectType(tmp[1]);
                        //Debug.Log("visualeffects");
                        break;

                    default:
                        //Debug.Log("Invalid Annotation Type");
                        break;
                }
            }
        }
    }

    void HandleTeleport()
    {
        if (_rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger) && _annotationManager != null && !_annotationManager.IsAnnotationActive)
        {
            Ray raycast = new Ray(_rightHand.transform.position, _rightHand.transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit);
            if (bHit && hit.collider.gameObject.name.Equals("VRPlane"))
            {
                EnableRightPointer();
            }
            else
            {
                DisableRightPointer();
            }

        }else if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && currentItem == null)
        {
            Ray raycast = new Ray(_rightHand.transform.position, _rightHand.transform.forward);
            RaycastHit hit;
            bool bHit = Physics.Raycast(raycast, out hit);
            if (bHit && hit.collider.gameObject.name.Equals("VRPlane"))
            {
                //Debug.Log("colide with = " + hit.collider.gameObject.name);
                GameObject rig = GameObject.Find("[CameraRig]");
                Vector3 camerapos = Camera.main.transform.localPosition;
                rig.transform.position = new Vector3(hit.point.x-camerapos.x, rig.transform.position.y, hit.point.z-camerapos.z);
            }
        }
        else
        {
            DisableRightPointer();
        }
    }

    void HandleVideoPlayback()
    {
        if (_leftController.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && _video != null)
        {
            Vector2 touchpad = _leftController.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            if (touchpad.y > 0.7f)
            {
                print("Pressed Stop");
                _video.Stop();
                _playing = false;
                _annotationManager.DisableAnnotations();
                _annotationManager.currentTime = 0.0f;
                _annotationManager.IsPlayingVideo = false;
                GameObject o = GameObject.Find("Avatar");
                if (o != null) o.GetComponent<SkeletonRepresentation>().hide();
            }
            else if (touchpad.y < -0.7f)
            {
                print("Pressed Play");
                _playing = !_playing;

                if (_playing)
                {
                    _video.Play();
                    SetRepresentation(_representation);
                    _annotationManager.IsPlayingVideo = true;
                }
                else
                {
                    _video.Pause();
                    _annotationManager.IsPlayingVideo = false;
                }
            }
            else if (touchpad.x > 0.7f)
            {
                print("Pressed Foward");
                _video.Skip5Sec();

            }
            else if (touchpad.x < -0.7f)
            {
                print("Pressed Backward");
                _video.Back5Sec();
            }
        }

        if (_video != null && _video.getVideoTime() != 0 && _video.getVideoDuration() != 0)
        {
            float ratio = _video.getVideoTime() / _video.getVideoDuration();

            //_slider.SetActive(true);
            _slider.GetComponentInChildren<Slider>().value = ratio;
            //_slider.transform.position = new Vector3(0, 2.5f, 0);
            //  _slider.transform.forward =  Camera.main.transform.forward;

        }

        if(_video != null)
        {
            _video.Update();
        }
    }

    void HandleCancelAnnotation()
    {
        if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu) && _video != null && _annotationManager.staticAnnotationList.Count > 0)
        {
            int numberOfAnnotations = _annotationManager.staticAnnotationList.Count;
            StaticAnnotation lastAnnotation = _annotationManager.staticAnnotationList[numberOfAnnotations - 1];
            lastAnnotation.reset();
            _annotationManager.staticAnnotationList.Remove(lastAnnotation);
            _annotationManager.DrawAnnotationsOnTimeline();

            //Debug.Log("pressed cancel annotation");
        }
    }

    void HandleDuration()
    {
        if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad) && _video != null && currentItem != null)
        {
            Vector2 touchpad = _rightController.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

            if (touchpad.x > 0.7f)
            {
                //print("Increase Duration");
                _annotationManager.IncrementDuration();
               
            }
            else if (touchpad.x < -0.7f)
            {
                //print("Decrease Duration");
                _annotationManager.DecreaseDuration();

            }
        }
    }

    // Update is called once per frame
    void Update () {


        if (_rightObj.index == SteamVR_TrackedObject.EIndex.None || _leftObj.index == SteamVR_TrackedObject.EIndex.None) return;
        _rightController = SteamVR_Controller.Input((int)_rightObj.index);
        _leftController = SteamVR_Controller.Input((int)_leftObj.index);

        if (_annotationManager != null) { 
            _annotationManager.SetRightHand(_rightHand);
            _annotationManager.SetRightHandController(_rightController);
            _annotationManager.SetRightPointer(_rightPointer);
            _annotationManager.SetHead(_head);
            _annotationManager.SetAnnotationDurationGO();
            _annotationManager.Update();
        }

    /*    if (_menu == MenuOpened.AnnotationEdit)
        {
            _menuGO.transform.position = new Vector3(_rightHand.transform.position.x,
                     _rightHand.transform.position.y + 0.15f, _rightHand.transform.position.z);
            Vector3 rot = _head.transform.forward;
            rot.y = 0.0f;
            _menuGO.transform.rotation = Quaternion.LookRotation(rot);
            EditAnnotations();
        } */

        // handle Annotations
        HandleAnnotations();

        // handle video playback
        HandleVideoPlayback();

        // handle Teleport
        HandleTeleport();

        // handle Cancel Annotation
        HandleCancelAnnotation();

        // handle Duration
        HandleDuration();


    }

    private void OnApplicationQuit()
    {
        if(_video != null)
        {
            _video.Close();
        }
        SaveAnnotations();
    }
}


