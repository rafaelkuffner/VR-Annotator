using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechAnnotation : StaticAnnotation
{
    private GameObject audioSourceGO { get; set; }
    private AudioSource audioSource;
    public bool IsActive { get; set; }
    private bool triggerPressed;
    private float _recordingTime;
    private GameObject audioVisualCueGO;
    string videoName;

    public SpeechAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController, GameObject head) :
        base(video, rightHand, rightController, head)
    {
        videoName = video.configFile;
        audioSourceGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/AudioSourcePrefab")) as GameObject;
        audioSource = audioSourceGO.GetComponent<AudioSource>();
        IsActive = false;
        triggerPressed = false;
        audioVisualCueGO = null;
    }

    public override void annotate()
    {
        if (!IsActive)
            audioSource = null;
        else
        {

            if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerPressed = true;
                Debug.Log("start = " + _start);
                _start = _video.getVideoTime();
                audioSource.clip = Microphone.Start("Microfone (HTC Vive)", true, 120, 44100); // lenght???
                _recordingTime = 0;
            }
            if (triggerPressed)
            {
              _recordingTime += Time.deltaTime; 
            }
            if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                Microphone.End("Microfone (HTC Vive)");
                audioVisualCueGO = new GameObject();
                audioVisualCueGO.transform.position = _rightHand.transform.position;
                audioVisualCueGO.transform.rotation = Quaternion.identity;
                audioVisualCueGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
                SpriteRenderer sr = audioVisualCueGO.AddComponent<SpriteRenderer>();
                sr.sprite = (Sprite)Resources.Load("Sprites/microphone", typeof(Sprite));

                triggerPressed = false;
                IsActive = false;
                _hasBeenCreated = true;
                _duration = _recordingTime;
            }
        }
    }

    public override void play()
    {
        if (!audioSource.isPlaying && AnnotationManager.RoughlyEqual(_start, _video.getVideoTime()))
        {
            audioVisualCueGO.SetActive(true);
            audioSource.Play();
        }
    }

    public override void stop()
    {
        audioSource.Stop();
        audioVisualCueGO.SetActive(false);
    }

    public override int edit()
    {
       
        if (audioSourceGO.activeSelf)
            return _id;
        else
            return -1;

    }

    public override void increaseDuration()
    {
    }

    public override void decreaseDuration()
    {
    }

    public override void disableDurationGO()
    {
        if(_annotationIdGO.activeSelf)
            _annotationIdGO.SetActive(false);
    }

    public override void reset()
    {
        if (audioSourceGO != null)
        {
            GameObject.Destroy(audioSourceGO);
        }
    }

    public override string serialize()
    {
        string audioName = "audioAnnotation"+videoName+getID();
       SavWav.Save(audioName, audioSource.clip);
      
        return getID() + "#" + getStart() + "#" + getDuration() + "#" + audioName + "#"
            + audioVisualCueGO.transform.position.x + "#" + audioVisualCueGO.transform.position.y + "#" + audioVisualCueGO.transform.position.z;
    }

    public override void deserialize(string s)
    {
        string[] tokens = s.Split('#');
        setID(int.Parse(tokens[0]));
        setStart(float.Parse(tokens[1]));
        setDuration(float.Parse(tokens[2]));
        string audioName = tokens[3];

        WAV wav = new WAV(audioName);
        AudioClip audioClip = AudioClip.Create(audioName, wav.SampleCount, 1, wav.Frequency,false);
        audioClip.SetData(wav.LeftChannel, 0);
        audioSource.clip = audioClip;

        Vector3 pos = new Vector3(float.Parse(tokens[4]), float.Parse(tokens[5]), float.Parse(tokens[6]));

        audioVisualCueGO = new GameObject();
        audioVisualCueGO.transform.position = pos;
        audioVisualCueGO.transform.localRotation = Quaternion.identity;
        audioVisualCueGO.transform.localScale = new Vector3(0.05f, 0.05f, 0.05f);
        SpriteRenderer sr = audioVisualCueGO.AddComponent<SpriteRenderer>();
        sr.sprite = (Sprite)Resources.Load("Sprites/microphone", typeof(Sprite));

        _hasBeenCreated = true;
    }
}
