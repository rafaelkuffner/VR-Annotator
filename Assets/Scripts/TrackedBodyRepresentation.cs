using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using MyTechnic;

public class TrackedBodyRepresentation
{

    public const float ballSize = 0.07f;
    public const float boneSize = 0.025f;
    // Body transforms and joints

    public List<Transform> bodyTransforms;
    // Spine
    public Transform spineBase;
    public Transform spineShoulder;
    public Transform head;

    public Vector3 spineBaseJoint;
    public Vector3 spineShoulderJoint;
    public Vector3 headJoint;

    // Left arm
    public Transform leftShoulder;
    public Transform leftElbow;
    public Transform leftArm;

    public Vector3 leftShoulderJoint;
    public Vector3 leftElbowJoint;
    public Vector3 leftWristJoint;

    // Left leg
    public Transform leftHip;
    public Transform leftKnee;
    public Transform leftAnkle;

    public Vector3 leftHipJoint;
    public Vector3 leftKneeJoint;
    public Vector3 leftAnkleJoint;

    // Right arm
    public Transform rightShoulder;
    public Transform rightElbow;
    public Transform rightArm;

    public Vector3 rightShoulderJoint;
    public Vector3 rightElbowJoint;
    public Vector3 rightWristJoint;

    // Right leg
    public Transform rightHip;
    public Transform rightKnee;
    public Transform rightAnkle;


    public Vector3 rightHipJoint;
    public Vector3 rightKneeJoint;
    public Vector3 rightAnkleJoint;

    // Bones
    public Transform boneNeck, boneSpine;
    public Transform boneLeftShoulder, boneLeftArm, boneLeftForearm;
    public Transform boneRightShoulder, boneRightArm, boneRightForearm;
    public Transform boneLeftHip, boneLeftThigh, boneLeftCalf;
    public Transform boneRightHip, boneRightThigh, boneRightCalf;

    // Use this for initialization
    public TrackedBodyRepresentation()
    {
        bodyTransforms = new List<Transform>();

        spineBaseJoint = new Vector3();
        spineShoulderJoint = new Vector3();
        headJoint = new Vector3();

        leftShoulderJoint = new Vector3();
        leftElbowJoint = new Vector3();
        leftWristJoint = new Vector3();
        leftHipJoint = new Vector3();
        leftKneeJoint = new Vector3();
        leftAnkleJoint = new Vector3();

        rightShoulderJoint = new Vector3();
        rightElbowJoint = new Vector3();
        rightWristJoint = new Vector3();
        rightHipJoint = new Vector3();
        rightKneeJoint = new Vector3();
        rightAnkleJoint = new Vector3();

        GameObject avatarGo = new GameObject("Avatar");
        SkeletonRepresentation sk = avatarGo.AddComponent<SkeletonRepresentation>();
        sk.setTBR(this);
        avatarGo.transform.parent = GameObject.Find("Data").transform;

        spineBase = createAvatarJoint(avatarGo.transform, "spineBase");
        spineShoulder = createAvatarJoint(avatarGo.transform, "spineShoulder");
        head = createAvatarJoint(avatarGo.transform, "head", 0.20f);

        leftShoulder = createAvatarJoint(avatarGo.transform, "leftShoulder");
        leftElbow = createAvatarJoint(avatarGo.transform, "leftElbow");
        leftArm = createAvatarJoint(avatarGo.transform, "leftArm");
        leftHip = createAvatarJoint(avatarGo.transform, "leftHip");
        leftKnee = createAvatarJoint(avatarGo.transform, "leftKnee");
        leftAnkle = createAvatarJoint(avatarGo.transform, "leftAnkle");

        rightShoulder = createAvatarJoint(avatarGo.transform, "rightShoulder");
        rightElbow = createAvatarJoint(avatarGo.transform, "rightElbow");
        rightArm = createAvatarJoint(avatarGo.transform, "rightArm");
        rightHip = createAvatarJoint(avatarGo.transform, "rightHip");
        rightKnee = createAvatarJoint(avatarGo.transform, "rightKnee");
        rightAnkle = createAvatarJoint(avatarGo.transform, "rightAnkle");

        boneNeck = createAvatarBone(avatarGo.transform);
        boneSpine = createAvatarBone(avatarGo.transform);
        boneLeftShoulder = createAvatarBone(avatarGo.transform);
        boneLeftArm = createAvatarBone(avatarGo.transform);
        boneLeftForearm = createAvatarBone(avatarGo.transform);
        boneRightShoulder = createAvatarBone(avatarGo.transform);
        boneRightArm = createAvatarBone(avatarGo.transform);
        boneRightForearm = createAvatarBone(avatarGo.transform);
        boneLeftHip = createAvatarBone(avatarGo.transform);
        boneLeftThigh = createAvatarBone(avatarGo.transform);
        boneLeftCalf = createAvatarBone(avatarGo.transform);
        boneRightHip = createAvatarBone(avatarGo.transform);
        boneRightThigh = createAvatarBone(avatarGo.transform);
        boneRightCalf = createAvatarBone(avatarGo.transform);

    }

    public void show()
    {
        foreach (Transform t in bodyTransforms)
        {
            t.GetComponent<Renderer>().enabled = true;
        }
    }

    public void hide()
    {
        foreach(Transform t in bodyTransforms)
        {
            t.GetComponent<Renderer>().enabled = false;
        }
    }

    Transform createAvatarJoint(Transform parent, string name, float scale = ballSize)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Sphere);
        Material mat = Resources.Load("Materials/SkeletonMaterial") as Material;
        gameObject.GetComponent<Renderer>().material = mat;
        gameObject.name = name;
        Rigidbody r = gameObject.AddComponent<Rigidbody>();
        r.isKinematic = true;
        r.useGravity = false;
        Transform transform = gameObject.transform;
        transform.parent = parent;
        transform.localScale *= scale;
        bodyTransforms.Add(transform);
        return transform;
    }

    Transform createAvatarBone(Transform parent, float scale = boneSize)
    {
        GameObject gameObject = GameObject.CreatePrimitive(PrimitiveType.Cylinder);
        Material mat = Resources.Load("Materials/SkeletonMaterial") as Material;
        gameObject.GetComponent<Renderer>().material = mat;
        Transform transform = gameObject.transform;
        transform.parent = parent;
        bodyTransforms.Add(transform);
        return transform;
    }
    // Update is called once per frame

    void updateAvatarBone(Transform bone, Vector3 joint1, Vector3 joint2)
    {
        if (bone == null)
            return;
        bone.localScale = new Vector3(boneSize, 0.5f * Vector3.Distance(joint1, joint2), boneSize);
        bone.position = (joint1 + joint2) * 0.5f;
        bone.up = joint2 - joint1;
    }

    public Transform findNearestBone(Vector3 position, Vector3 handPosition)
    {
        float minDist = float.MaxValue;
        float maxDist = 1f;
        Transform minTransf = null;
        foreach (Transform jointTransform in bodyTransforms)
        {
            float dist = (handPosition - jointTransform.position).magnitude;
            if (dist < minDist && dist < maxDist)
            {
                minDist = dist;
                minTransf = jointTransform;
            }
        }
        return minTransf;
    }

    public void updateAvatarBody()
    {
        Vector3 spineUp = Utils.GetBoneDirection(spineShoulderJoint, spineBaseJoint);
        Vector3 spineRight = Utils.GetBoneDirection(rightShoulderJoint, leftShoulderJoint);
        Vector3 spineForward = Vector3.Cross(spineRight, spineUp);
        // Spine
        spineBase.position = spineBaseJoint;
        spineShoulder.position = spineShoulderJoint;
        head.position = headJoint;
        head.rotation = Quaternion.LookRotation(spineForward, spineUp);

        // Left Arm
        leftShoulder.position = leftShoulderJoint;
        leftArm.position = leftElbowJoint;
        leftElbow.position = leftWristJoint;

        // Left Leg
        leftHip.position = leftHipJoint;
        leftKnee.position = leftKneeJoint;
        leftAnkle.position = leftAnkleJoint;

        // Right Arm
        rightShoulder.position = rightShoulderJoint;
        rightArm.position = rightElbowJoint;
        rightElbow.position = rightWristJoint;

        // Right Leg
        rightHip.position = rightHipJoint;
        rightKnee.position = rightKneeJoint;
        rightAnkle.position = rightAnkleJoint;

        // Bones
        updateAvatarBone(boneNeck, head.position, spineShoulder.position);
        updateAvatarBone(boneSpine, spineBase.position, spineShoulder.position);

        updateAvatarBone(boneLeftShoulder, leftShoulder.position, spineShoulder.position);
        updateAvatarBone(boneLeftArm, leftShoulder.position, leftArm.position);
        updateAvatarBone(boneLeftForearm, leftArm.position, leftElbow.position);

        updateAvatarBone(boneRightShoulder, rightShoulder.position, spineShoulder.position);
        updateAvatarBone(boneRightArm, rightShoulder.position, rightArm.position);
        updateAvatarBone(boneRightForearm, rightArm.position, rightElbow.position);

        updateAvatarBone(boneLeftHip, spineBase.position, leftHip.position);
        updateAvatarBone(boneLeftThigh, leftHip.position, leftKnee.position);
        updateAvatarBone(boneLeftCalf, leftKnee.position, leftAnkle.position);

        updateAvatarBone(boneRightHip, spineBase.position, rightHip.position);
        updateAvatarBone(boneRightThigh, rightHip.position, rightKnee.position);
        updateAvatarBone(boneRightCalf, rightKnee.position, rightAnkle.position);
    }


}