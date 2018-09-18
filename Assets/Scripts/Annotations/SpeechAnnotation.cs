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

    public SpeechAnnotation(CloudVideoPlayer video, GameObject rightHand, SteamVR_Controller.Device rightController) :
        base(video, rightHand, rightController)
    {
        audioSourceGO = MonoBehaviour.Instantiate(Resources.Load("Prefabs/AudioSourcePrefab")) as GameObject;
        audioSource = audioSourceGO.GetComponent<AudioSource>();
        IsActive = false;
        triggerPressed = false;
        audioVisualCueGO = null;
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
                Debug.Log("Duration is " + _duration);
            }
        }
    }

    public override void play()
    {
        if (!audioSource.isPlaying && AnnotationManager.RoughlyEqual(_start, _video.getTime()))
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
    public override void reset()
    {
        if (audioSourceGO != null)
        {
            GameObject.Destroy(audioSourceGO);
        }
    }
}
