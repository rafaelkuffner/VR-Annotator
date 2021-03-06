﻿using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Runtime.InteropServices;


public class CloudVideoPlayer{

    [DllImport("RavatarPlugin")]
    private static extern IntPtr initLocal(string configLocation);
    [DllImport("RavatarPlugin")]
    private static extern bool getFrameAndNormal(string cloudID, byte[] colorFrame, byte[] depthFrame, byte[] normalFrame);
    [DllImport("RavatarPlugin")]
    private static extern void stopClouds();
    [DllImport("RavatarPlugin")]
    private static extern void skip5seconds();
    [DllImport("RavatarPlugin")]
    private static extern void back5seconds();
    [DllImport("RavatarPlugin")]
    private static extern void resetStreams();
    [DllImport("RavatarPlugin")]
    private static extern float getTime();
    [DllImport("RavatarPlugin")]
    private static extern float getDuration();

    private Dictionary<string, PointCloudDepth> _clouds;
    private Dictionary<string, GameObject> _cloudGameObjects;

    public string configFile;
    private string _videosDir;
    private string _colorStreamName;
    private string _depthStreamName;
    private string _normalStreamName;
    private int _vidWidth;
    private int _vidHeight;
    private int _layerNum;

    //things to play the skeleton 
    private string _skeletonFileName;
    private GameObject _skeletonPlayerGO;
    private FileListener _skeletonPlayer;
    private InputManager _inputManager;

    public int BUFFER = 868352;
    public int DBUFFER = 868352;
    internal byte[] _colorData;
    internal byte[] _depthData;

    float _speed;
    bool _playing;
    bool _rendering; 

    float _lastDelta;

    public CloudVideoPlayer(string path,InputManager manager)
    {
        Application.targetFrameRate = 30;
        _clouds = new Dictionary<string, PointCloudDepth>();
        _cloudGameObjects = new Dictionary<string, GameObject>();
        _colorData = new byte[BUFFER];
        _depthData = new byte[DBUFFER];
        configFile = path;
        //Debug.Log("VideoObject loading from " + configFile);
        LoadSkeletonData();
        IntPtr output = initLocal(path);
        string calib = Marshal.PtrToStringAnsi(output);
        processCalibrationMatrix(calib);
        resetStreams();
        _inputManager = manager;
        _playing = false;
        _speed = 1;
        _lastDelta = 0;
        _rendering = true;
    }

    public void setRendering(bool r)
    {
        _rendering = r;
        if (!_rendering) hide();
        else show();
    }

    public void Update()
    {
        float delta = Time.deltaTime;
        _lastDelta += delta;

        if (!_playing) return;


        if (_lastDelta > 1.0f / 30.0f) { 
            foreach (KeyValuePair<string, PointCloudDepth> p in _clouds)
            {
                if (getFrameAndNormal(p.Key, _colorData, _depthData, null))
                {
                    _clouds[p.Key].setPointsUncompressed(_colorData, _depthData);
                }
            }
            show();
            _lastDelta = 0;
            if(_skeletonPlayer != null)
                _skeletonPlayer.ReadNextLine();
        }
    }
    public void processCalibrationMatrix(string calibration)
    {
        GameObject papi = GameObject.Find("Data");
        string[] tokens = calibration.Split(MessageSeparators.L1);
        foreach (string s in tokens)
        {
            if (s == "") break;
            string[] chunks = s.Split(';');
            string id = chunks[0];

            Matrix4x4 mat = new Matrix4x4(new Vector4(float.Parse(chunks[1]), float.Parse(chunks[5]), float.Parse(chunks[9]), float.Parse(chunks[13])),
           new Vector4(float.Parse(chunks[2]), float.Parse(chunks[6]), float.Parse(chunks[10]), float.Parse(chunks[14])),
           new Vector4(float.Parse(chunks[3]), float.Parse(chunks[7]), float.Parse(chunks[11]), float.Parse(chunks[15])),
           new Vector4(float.Parse(chunks[4]), float.Parse(chunks[8]), float.Parse(chunks[12]), float.Parse(chunks[16])));

            GameObject cloudobj = new GameObject(id);
            cloudobj.transform.parent = papi.transform;
            cloudobj.transform.localPosition = new Vector3(mat[0, 3], mat[1, 3], mat[2, 3]);
            cloudobj.transform.localRotation = mat.rotation;
            cloudobj.transform.localScale = new Vector3(-1, 1, 1);
            cloudobj.AddComponent<PointCloudDepth>();

            PointCloudDepth cloud = cloudobj.GetComponent<PointCloudDepth>();
            _clouds.Add(id, cloud);
            _cloudGameObjects.Add(id, cloudobj);
        }
    }

    private void LoadSkeletonData()
    { 
         Dictionary<string, string> config = ConfigProperties.load(configFile);
       
        if (_skeletonPlayerGO == null)
        {
            _skeletonPlayerGO = new GameObject("SkeletonPlayer");
            _skeletonPlayerGO.transform.position = Vector3.zero;
            _skeletonPlayerGO.transform.rotation = Quaternion.identity;
            _skeletonPlayerGO.transform.localScale = Vector3.one;
            _skeletonPlayer = _skeletonPlayerGO.AddComponent<FileListener>();
        }
        _skeletonFileName = "";
        if (config.ContainsKey("skeletonFileName"))
        {
            _skeletonFileName = config["skeletonFileName"];
        }
        _skeletonPlayer.Initialize(_skeletonFileName);
        Debug.Log("has skele" + _skeletonPlayer.HasSkeleton());
   
    }

    public float getVideoDuration()
    {
        return getDuration();
    }

    public float getVideoTime()
    {
        return getTime();
    }

    public void hide()
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.hide();
        }
    }

    public void show()
    {
        if (!_rendering) return;
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.show();
        }
    }

    public void Skip5Sec(){
        skip5seconds();
        _skeletonPlayer.Skip5Sec();
    }

    public void Back5Sec()
    {
        back5seconds();
        _skeletonPlayer.Back5Sec();

    }

    public void Play()
    {
        _playing = true;
        show();
    }

    public void Pause()
    {
        _playing = false;
    }

    public void Stop()
    {
        resetStreams();
        _playing = false;
        _skeletonPlayer.Reset();
        _inputManager._playing = false;
        Debug.Log("Stop!");
        hide();
    }

    public bool Playing()
    {
        return _inputManager._playing;
    }
    public void Close()
    {
        stopClouds();
        _skeletonPlayer.Close();
        GameObject.Destroy(_skeletonPlayerGO);
        foreach (GameObject go in _cloudGameObjects.Values)
        {
            GameObject.Destroy(go);
        }

    }

    public void setSpeed(float speed)
    {
        _speed = speed;
    }

}
