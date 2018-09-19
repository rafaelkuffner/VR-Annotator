using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using Valve.VR;

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
 	private GameObject _controlDataset;
    private GameObject _menuGO;
    private AnnotationManager _annotationManager;

    private Dictionary<CloudVideoPlayer, AnnotationManager> annotationManagerByVideo;

    public enum MenuOpened
    {
        DatasetSelect,ColorSelect,AnnotationSelect,AnnotationEdit,None
    }

    void setupRightPointer()
    {
        _rightHolder = new GameObject();
        _rightHolder.transform.parent = _rightHand.transform;
        _rightHolder.transform.localPosition = Vector3.zero;
        _rightHolder.transform.localRotation = Quaternion.identity;

        _rightPointer = GameObject.CreatePrimitive(PrimitiveType.Cube);
        _rightPointer.transform.parent = _rightHolder.transform;
        _rightPointer.transform.localScale = new Vector3(_pointerThickness, _pointerThickness, 100f);
        _rightPointer.transform.localPosition = new Vector3(0f, 0f, 50f);
        _rightPointer.transform.localRotation = Quaternion.identity;
        BoxCollider collider = _rightPointer.GetComponent<BoxCollider>();
        collider.isTrigger = true;
        Rigidbody rigidBody = _rightPointer.AddComponent<Rigidbody>();
        rigidBody.isKinematic = true;

        _pointerColor = new Color(0.2f, 0.2f, 0.2f);
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

	void Start () {

        _rightObj = _rightHand.GetComponent<SteamVR_TrackedObject>();
        _leftObj = _leftHand.GetComponent<SteamVR_TrackedObject>();
        setupRightPointer();
        setupLeftPointer();

        _annotationManager = null;

        //GameObject annotationManagerGO = GameObject.Find("AnnotationManager");
        //_annotationManager = annotationManagerGO.GetComponent<AnnotationManager>();
        //_annotationManager.SetRightHand(_rightHand);

        DisableRightPointer();
        DisableLeftPointer();
        _menu = MenuOpened.None;
        _video = null;
        _playSpeed = 1;
        _slider = GameObject.Find("Timeline");
        _controlDataset = Instantiate(Resources.Load("Prefabs/ControlDataset")) as GameObject;
        _controlDataset.SetActive(false);
        _representation = "Full";
        annotationManagerByVideo = new Dictionary<CloudVideoPlayer, AnnotationManager>();

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

    void InputOpenMenus()
    {

        if (_leftController.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            //GameObject menu;
            CloseAllMenus();

            if (_menu != MenuOpened.DatasetSelect)
            {
                _menuGO = Instantiate(Resources.Load("Prefabs/CloudMenu")) as GameObject;
                _menuGO.name = "DatasetSelect";
                _menuGO.transform.position = _head.transform.position + (_head.transform.forward * 2);
                Vector3 rot = Camera.main.transform.forward;
                rot.y = 0.0f;
                _menuGO.transform.rotation = Quaternion.LookRotation(rot);
                _menuGO.transform.position = new Vector3(_menuGO.transform.position.x, 1.4f, _menuGO.transform.position.z);

                _menu = MenuOpened.DatasetSelect;
                EnableLeftPointer();
            }
            else
            {
                _menu = MenuOpened.None;
                DisableLeftPointer();
            }
        }

        if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.ApplicationMenu))
        {
            //GameObject menu;
            CloseAllMenus();

            if (_menu != MenuOpened.ColorSelect)
            {
                _menuGO = Instantiate(Resources.Load("Prefabs/ColorMenu")) as GameObject;
                _menuGO.name = "ColorSelect";
                _menuGO.transform.position = _head.transform.position + (_head.transform.forward * 2);
                Vector3 rot = Camera.main.transform.forward;
                rot.y = 0.0f;
                _menuGO.transform.rotation = Quaternion.LookRotation(rot);
                _menuGO.transform.position = new Vector3(_menuGO.transform.position.x, 1.4f, _menuGO.transform.position.z);

                _menu = MenuOpened.ColorSelect;
                EnableRightPointer();
            }
            else
            {
                _menu = MenuOpened.None;
                DisableRightPointer();
            }
        }
        if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
        {
            //GameObject menu;
            CloseAllMenus();

            if (_menu != MenuOpened.AnnotationSelect && _menu != MenuOpened.AnnotationEdit && _annotationManager != null)
            {
                _annotationManager.Reset(); // cancel any active annotation
                _menuGO = Instantiate(Resources.Load("Prefabs/AnnotationMenu")) as GameObject;
                _menuGO.name = "AnnotationSelect";
                _menuGO.transform.position = new Vector3(_rightHand.transform.position.x,
                    _rightHand.transform.position.y + 0.08f, _rightHand.transform.position.z);
                //Vector3 rot = Camera.main.transform.forward;
                Vector3 rot = _head.transform.forward;
                rot.y = 0.0f;
                _menuGO.transform.rotation = Quaternion.LookRotation(rot);
                _menu = MenuOpened.AnnotationSelect;
            }
            else
            {
                _menu = MenuOpened.None;
            } 
        }
        if (_annotationManager != null &&_annotationManager.currentAnnotationSelected != -1)
        {
            if (_menu != MenuOpened.AnnotationEdit)
            {
                _menuGO = Instantiate(Resources.Load("Prefabs/EditAnnotationMenu")) as GameObject;
                _menuGO.name = "EditAnnotationMenu";
                _menuGO.transform.position = new Vector3(_rightHand.transform.position.x,
                        _rightHand.transform.position.y + 0.08f, _rightHand.transform.position.z);
                //Vector3 rot = Camera.main.transform.forward;
                Vector3 rot = _head.transform.forward;
                rot.y = 0.0f;
                _menuGO.transform.rotation = Quaternion.LookRotation(rot);
                _menu = MenuOpened.AnnotationEdit;
            }
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
            case "Full":
                _video.show();
                if (o != null) o.GetComponent<SkeletonRepresentation>().show();
                break;
            case "Skeleton":
                _video.hide();
                if (o != null) o.GetComponent<SkeletonRepresentation>().show();
                break;
            case "Cloud":
                _video.show();
                if (o != null) o.GetComponent<SkeletonRepresentation>().hide();
                break;
            case "None":
                _video.hide();
                if (o != null) o.GetComponent<SkeletonRepresentation>().hide();
                break;
        }
		_representation = representation;
    }

    void SelectDataset()
    {
        Ray raycast = new Ray(_leftHand.transform.position, _leftHand.transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);
        if (hit.transform != null) {
             Button b = hit.transform.gameObject.GetComponent<Button>();
             if (b != null) { 
                 b.Select();

                 if (_leftController.GetPress(SteamVR_Controller.ButtonMask.Trigger))
                 {
                     if (b.name == "PlaySpeed")
                     {
                         SetPlaybackSpeed(float.Parse(b.GetComponent<VRUIItem>().value));
                         return;
                     }
                     if(b.name == "Representation")
                    {
                        SetRepresentation(b.GetComponent<VRUIItem>().value);
						return;
                    }
                     if (_video != null && _video.configFile == b.name)
                     {
                         return;
                     }
                     else if (_video != null && _video.configFile != b.name){
                        _video.Close();
                     }
                     
                     _video = new CloudVideoPlayer(b.name,this);

                     if (!annotationManagerByVideo.ContainsKey(_video)) {
                         _annotationManager = new AnnotationManager();
                         _annotationManager.init();
                         _annotationManager.SetCloudVideo(_video);
                         annotationManagerByVideo.Add(_video, _annotationManager);
                     }
                     CloseAllMenus();
                     DisableLeftPointer();
                     _menu = MenuOpened.None;
                 }
             }
        }
    }

    void SelectColor()
    {
        Ray raycast = new Ray(_rightHand.transform.position, _rightHand.transform.forward);
        RaycastHit hit;
        bool bHit = Physics.Raycast(raycast, out hit);
        if (hit.transform != null && hit.transform.name == "ColorPalette")
        {
            Renderer rend = hit.transform.GetComponent<Renderer>();
            Texture2D tex = rend.material.mainTexture as Texture2D;
            Vector2 pixelUV = hit.textureCoord;
            pixelUV.x *= tex.width;
            pixelUV.y *= tex.height;
            Color p = tex.GetPixel((int)pixelUV.x, (int)pixelUV.y);
            if (_rightController.GetPress(SteamVR_Controller.ButtonMask.Trigger))
            {
				_rightPointer.GetComponent<MeshRenderer>().material.SetColor("_Color",p);
				_pointerColor = p;
                CloseAllMenus();
                DisableRightPointer();
                _menu = MenuOpened.None;
            }
        }
    }

    void SelectAnnotationType()
    {
     
       if (_rightController.GetPress(SteamVR_Controller.ButtonMask.Touchpad))
        {
            if (!_annotationManager.IsAnnotationActive)
            {
                Vector2 touchpad = _rightController.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);
                //Debug.Log("Axis x = " + touchpad.x + " | Axis y = " + touchpad.y);

                if (touchpad.y > 0.7f)
                {
                    print("HighlightPoints Annotation");
                    _annotationManager.HandleVisualEffectsAnnotation(_head, _rightPointer);
                }
                else if (touchpad.y < -0.7f)
                {
                    print("Mark Annotation");
                    _annotationManager.HandleMarkAnnotation(_head, _rightPointer);
                }
                else if (touchpad.x > 0.7f)
                {
                    print("Speech Annotation");
                    _annotationManager.HandleSpeechAnnotation();
                }
                else if (touchpad.x < -0.7f)
                {
                    print("Scribbler Annotation");
                    _annotationManager.HandleScribblerAnnotation();
                }
                /*else
                {
                    _annotationManager.IsAnnotationActive = false;
                    _annotationManager.DisableAnnotations();
                } */
            }
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
        _controlDataset.SetActive(false);
        return;
    }

	// Update is called once per frame
	void Update () {

        if (_rightObj.index == SteamVR_TrackedObject.EIndex.None || _leftObj.index == SteamVR_TrackedObject.EIndex.None) return;
        _rightController = SteamVR_Controller.Input((int)_rightObj.index);
        _leftController = SteamVR_Controller.Input((int)_leftObj.index);

        if (_annotationManager != null) { 
            _annotationManager.SetRightHand(_rightHand);
            _annotationManager.SetRightHandController(_rightController);
            _annotationManager.SetHead(_head);
            _annotationManager.Update();
        }

        InputOpenMenus();

        if (_video != null )
        {
            if (_video.getTime() != 0 && _video.getDuration() != 0) 
            {
                float ratio = _video.getTime() / _video.getDuration();

                //_slider.SetActive(true);
                _slider.GetComponentInChildren<Slider>().value = ratio;
                //_slider.transform.position = new Vector3(0, 2.5f, 0);
              //  _slider.transform.forward =  Camera.main.transform.forward;
            }

        }

        if (_menu == MenuOpened.DatasetSelect)
        {
            SelectDataset();
        }
        else if (_menu == MenuOpened.ColorSelect)
        {
            SelectColor();
        }
        else if (_menu == MenuOpened.AnnotationSelect)
        {
            _menuGO.transform.position = new Vector3(_rightHand.transform.position.x,
                     _rightHand.transform.position.y + 0.15f, _rightHand.transform.position.z);
            Vector3 rot = _head.transform.forward;
            rot.y = 0.0f;
            _menuGO.transform.rotation = Quaternion.LookRotation(rot);
            SelectAnnotationType();
        }
        else if (_menu == MenuOpened.AnnotationEdit)
        {
            _menuGO.transform.position = new Vector3(_rightHand.transform.position.x,
                     _rightHand.transform.position.y + 0.15f, _rightHand.transform.position.z);
            Vector3 rot = _head.transform.forward;
            rot.y = 0.0f;
            _menuGO.transform.rotation = Quaternion.LookRotation(rot);
            EditAnnotations();
        }

  
        //if no menu is opened, annotation input, and playback control
        else if (_menu == MenuOpened.None)
        {
            if (!_controlDataset.activeSelf && _video != null)
            {
                _controlDataset.SetActive(true);
            }
            else
            {
                _controlDataset.transform.position = new Vector3(_leftHand.transform.position.x,
                    _leftHand.transform.position.y + 0.15f, _leftHand.transform.position.z);
                Vector3 rot = Camera.main.transform.forward;
                rot.y = 0.0f;
                _controlDataset.transform.rotation = Quaternion.LookRotation(rot);
            } 

            if (_leftController.GetPressUp(SteamVR_Controller.ButtonMask.Touchpad))
            {
                //Vector2 touchpad = _leftController.GetAxis(EVRButtonId.k_EButton_Axis0);
                Vector2 touchpad = _leftController.GetAxis(EVRButtonId.k_EButton_SteamVR_Touchpad);

                if (touchpad.y > 0.7f)
                {
                    print("Pressed Stop");
                    _video.Stop();
                    _playing = false;
                    _controlDataset.SetActive(false);
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

                    if (_playing) {
                        _video.Play();
						SetRepresentation (_representation);
                        _annotationManager.IsPlayingVideo = true;
                    }
                    else { 
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


          /*  //Got controllers, now handle input.
            if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                _playing = !_playing;
             
                if (_playing)
                    _video.Play();
                else
                    _video.Pause();
            }
            */

            if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger) && _annotationManager != null && !_annotationManager.IsAnnotationActive)
            {
                EnableRightPointer();
                if (_annotationManager != null)
                {
                    _annotationManager.EditAnnotation();
                    Debug.Log("Annotation Selected = " + _annotationManager.currentAnnotationSelected);
                }
               
            }
            if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger) && _annotationManager != null && !_annotationManager.IsAnnotationActive)
            {
                Ray raycast = new Ray(_rightHand.transform.position, _rightHand.transform.forward);
                RaycastHit hit;
                bool bHit = Physics.Raycast(raycast, out hit);
                if (bHit && hit.collider.gameObject.name.Equals("VRPlane"))
                {
                    Debug.Log("colide with = " + hit.collider.gameObject.name);
                    GameObject camera = GameObject.Find("[CameraRig]");
                    camera.transform.position = new Vector3(hit.point.x, camera.transform.position.y, hit.point.z);
                }
                DisableRightPointer();
            }
        }
	}


}
