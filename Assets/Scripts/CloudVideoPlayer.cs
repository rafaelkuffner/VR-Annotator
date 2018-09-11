using UnityEngine;
using System.Collections.Generic;
using System.Collections;
using System;
using System.Linq;
using System.Net.Sockets;
using System.Net;
using System.Text;

public class CloudVideoPlayer{


    private Dictionary<string, PointCloudDepth> _clouds;

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

    public CloudVideoPlayer(string path,InputManager manager)
    {
        configFile = path;
        //Debug.Log("VideoObject loading from " + configFile);
        _clouds = new Dictionary<string, PointCloudDepth>();
        loadConfig();
        _inputManager = manager;
    }



    private void loadConfig()
    { 
         Dictionary<string, string> config = ConfigProperties.load(configFile);
        _videosDir = config["videosDir"];
        _colorStreamName = config["colorStreamName"];
        _depthStreamName = config["depthStreamName"];
        _normalStreamName = config["normalStreamName"];
        _vidWidth =int.Parse(config["vidWidth"]);
        _vidHeight =int.Parse(config["vidHeight"]);
        _layerNum =int.Parse(config["numLayers"]);

       
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

        GameObject papi = GameObject.Find("Data");
        for (int i = 0; i < _layerNum; i++)
        {
            string s = "";
            s = s + i;
            string calib = config[s];
            string[] chunks = calib.Split(';');
            Matrix4x4 mat = new Matrix4x4(new Vector4(float.Parse(chunks[0]), float.Parse(chunks[4]), float.Parse(chunks[8]), float.Parse(chunks[12])),
                new Vector4(float.Parse(chunks[1]), float.Parse(chunks[5]), float.Parse(chunks[9]), float.Parse(chunks[13])),
                new Vector4(float.Parse(chunks[2]), float.Parse(chunks[6]), float.Parse(chunks[10]), float.Parse(chunks[14])),
                new Vector4(float.Parse(chunks[3]), float.Parse(chunks[7]), float.Parse(chunks[11]), float.Parse(chunks[15])));

            GameObject cloudobj = new GameObject(s);
            cloudobj.transform.SetParent(papi.transform);
            cloudobj.transform.localPosition = new Vector3(mat[0, 3], mat[1, 3],mat[2,3]);
            cloudobj.transform.localRotation = mat.rotation;
            cloudobj.transform.localScale = new Vector3(-1, 1, 1);
            cloudobj.AddComponent<PointCloudDepth>();

            PointCloudDepth cloud = cloudobj.GetComponent<PointCloudDepth>();

            //play from url
            string colorvideo = _videosDir + "\\" + s + _colorStreamName;
            //char[] sep = { '\\' };
            //string[] folderName = _videosDir.Split(sep);
            //string colorvideo = "CloudData\\" + folderName[folderName.Length - 1] + "\\" + s + _colorStreamName;
            string depthvideo = _videosDir + "\\" + s + _depthStreamName;
            //string normalvideo = _videosDir + "\\" + s + _normalStreamName;

            cloud.initStructs((uint)i,colorvideo, depthvideo,cloudobj,_skeletonPlayer,this);

            _clouds.Add(s, cloud);               
        }
        Debug.Log("has skele" +_skeletonPlayer.HasSkeleton());
    }

    public float getDuration()
    {
        return _clouds.Values.ElementAt<PointCloudDepth>(0).getDuration();
    }

    public float getTime()
    {
        return _clouds.Values.ElementAt<PointCloudDepth>(0).getTime();
    }

    public void hide()
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.hideRenderer();
        }
    }

    public void show()
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.showRenderer();
        }
    }

    public void Skip5Sec(){
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.Skip5Sec();
        }
        _skeletonPlayer.Skip5Sec();
    }

    public void Back5Sec()
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.Back5Sec();
        }
        _skeletonPlayer.Back5Sec();

    }

    public void Play()
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.PlayCloudVideo();
        }
    }

    public void Pause()
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.PauseCloudVideo();
        }
    }

    public void Stop()
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.StopCloudVideo();
        }
        _skeletonPlayer.Reset();
        _inputManager._playing = false;
        Debug.Log("Stop!");

    }
    public void Close()
    {
        foreach (PointCloudDepth pcd in _clouds.Values)
        {
            pcd.destroy();
            GameObject.Destroy(pcd.gameObject);
        }
        _skeletonPlayer.Close();
    }

    public void setSpeed(float speed)
    {
        foreach (PointCloudDepth d in _clouds.Values)
        {
            d.SetSpeed(speed);
        }
    }

}
