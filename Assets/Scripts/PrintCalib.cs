using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PrintCalib : MonoBehaviour
{


    // Update is called once per frame
    void Update()
    {
        if (Input.GetKeyUp(KeyCode.T))
            print(transform.position.x + ";" + transform.position.y + ";" + transform.position.z + ";" + transform.rotation.x + ";" + transform.rotation.y + ";" + transform.rotation.z + ";" + transform.rotation.w + ";");

        if (Input.GetKeyUp(KeyCode.O))
        {
            Matrix4x4 m = Matrix4x4.TRS(transform.localPosition, transform.localRotation, Vector3.one);
            print(m[0, 0] + ";" + m[0, 1] + ";" + m[0, 2] + ";" + m[0, 3] + ";"
                + m[1, 0] + ";" + m[1, 1] + ";" + m[1, 2] + ";" + m[1, 3] + ";"
                + m[2, 0] + ";" + m[2, 1] + ";" + m[2, 2] + ";" + m[2, 3] + ";"
                + m[3, 0] + ";" + m[3, 1] + ";" + m[3, 2] + ";" + m[3, 3] + ";");
        }

    }
}
