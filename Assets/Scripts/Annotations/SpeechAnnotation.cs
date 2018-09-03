using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SpeechAnnotation : StaticAnnotation
{
    private GameObject audioSourceGO { get; set; }
    private AudioSource audioSource;
    public bool IsActive { get; set; }
    private AudioClip audioClip;
    private bool triggerPressed;

    public SpeechAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController) :
        base(video, rightHand, rightController)
    {
        audioSourceGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/AudioSourcePrefab")) as GameObject;
        audioSource = audioSourceGO.GetComponent<AudioSource>();
        IsActive = false;
        audioClip = null;
        triggerPressed = false;
    }

    public override void annotate()
    {
        Debug.Log("annotate method scribbler");

        if (!IsActive)
            audioClip = null;
        else
        {

            if (_rightController.GetPressDown(SteamVR_Controller.ButtonMask.Trigger))
            {
                triggerPressed = true;
                _start = Time.time;
                Debug.Log("start = " + _start);
            }

            if (triggerPressed)
            {
                audioSource.clip = Microphone.Start("2- HTC Vive", true, 120, 44100); // lenght????
                Debug.Log("Recorded audoclip lenght = " + audioSource.clip.length); 
                Debug.Log("Recording audio...."); 
            }

            if (_rightController.GetPressUp(SteamVR_Controller.ButtonMask.Trigger))
            {
                audioClip = audioSource.clip;
                triggerPressed = false;
                IsActive = false;
                _duration = Time.time - _start;
                Debug.Log("duration = " + _duration);
            }
        }
    }

    public override void play()
    {
        throw new System.NotImplementedException();
    }

    public override void stop()
    {
        throw new System.NotImplementedException();
    }
}
