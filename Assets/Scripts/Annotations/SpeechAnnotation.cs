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

    public SpeechAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController) :
        base(video, rightHand, rightController)
    {
        audioSourceGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/AudioSourcePrefab")) as GameObject;
        audioSource = audioSourceGO.GetComponent<AudioSource>();
        IsActive = false;
        triggerPressed = false;
    }

    public override void annotate()
    {
        Debug.Log("annotate method scribbler");

        if (!IsActive)
            audioSource = null;
        else
        {

            if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerPressed = true;
                Debug.Log("start = " + _start);
                _start = _video.getTime();
                audioSource.clip = Microphone.Start("2- HTC Vive", true, 120, 44100); // lenght???
                _recordingTime = 0;
            }
            if (triggerPressed)
            {
              _recordingTime += Time.deltaTime; 
            }
            if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                Microphone.End("2- HTC Vive");
                triggerPressed = false;
                IsActive = false;
                _hasBeenCreated = true;
                _duration = _recordingTime;
                Debug.Log("Duration is " + _duration);
            }
        }
    }

    public override void play()
    {
        if(!audioSource.isPlaying && AnnotationManager.RoughlyEqual(_start,_video.getTime()))
            audioSource.Play();
    }

    public override void stop()
    {
        audioSource.Stop();
    }

    public override void edit()
    {
        throw new System.NotImplementedException();
    }
    public override void reset()
    {
        if (audioSourceGO != null)
        {
            GameObject.Destroy(audioSourceGO);
        }
    }
}
