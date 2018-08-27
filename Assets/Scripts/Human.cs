using UnityEngine;
using System;
using System.Collections;

public class Human 
{
	private string _id;
	public string id { get { return _id; } }

	private Body _body;
	public Body body { get { return _body; } }

	private DateTime _lastUpdated;
	public DateTime lastUpdated { get { return _lastUpdated; } }

	private TrackedBodyRepresentation _tbrepresentation;
	public TrackedBodyRepresentation tbr { get { return _tbrepresentation; } }

	public Human(bool createTBR)
	{
		_body = null;
		_id = null;
		if (createTBR) {
			_tbrepresentation = new TrackedBodyRepresentation ();
		}
	}


	public void UpdateAvatarBody(bool isNewFrame, DateTime frameTime)
	{
		ApplyFilterToJoints(isNewFrame,frameTime);
		tbr.updateAvatarBody ();
	}

	/// <summary>
	/// Applies the noise filter to joints.
	/// </summary>
	private void ApplyFilterToJoints(bool isNewFrame, DateTime frameTime)
	{
		// Spine
		tbr.spineBaseJoint = body.Joints[BodyJointType.spineBase];
		tbr.spineShoulderJoint= body.Joints[BodyJointType.spineShoulder];
		tbr.headJoint= body.Joints[BodyJointType.head];

        // Left arm
        tbr.leftShoulderJoint = body.Joints[BodyJointType.leftShoulder];
		tbr.leftElbowJoint = body.Joints[BodyJointType.leftElbow];
		tbr.leftWristJoint = body.Joints[BodyJointType.leftWrist];

		// Left leg
		tbr.leftHipJoint = body.Joints[BodyJointType.leftHip];
		tbr.leftKneeJoint = body.Joints[BodyJointType.leftKnee];
		tbr.leftAnkleJoint = body.Joints[BodyJointType.leftFoot];

		// Right arm
		tbr.rightShoulderJoint = body.Joints[BodyJointType.rightShoulder];
		tbr.rightElbowJoint = body.Joints[BodyJointType.rightElbow];
		tbr.rightWristJoint = body.Joints[BodyJointType.rightWrist];

		// Right leg
		tbr.rightHipJoint = body.Joints[BodyJointType.rightHip];
		tbr.rightKneeJoint = body.Joints[BodyJointType.rightKnee];
		tbr.rightAnkleJoint = body.Joints[BodyJointType.rightFoot];
	}





	public void Update(Body newBody)
	{
		_body = newBody;
		_id = _body.Properties[BodyPropertiesType.UID];
		_lastUpdated = DateTime.Now;
	}
}