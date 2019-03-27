using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;
public class PopulateCloudMenu : MonoBehaviour {

    // Use this for initialization
    string filesPath = "Assets\\Data Files";
    float spacex = 0.5f;
    float spacey = 0.25f;
    List<string> files;
	void Start () {
		string[] f =  System.IO.Directory.GetFiles(filesPath);
        files = new List<string>(f);
        GameObject butPrefab = Resources.Load("Prefabs/ButtonSmall") as GameObject;
        float scalex = butPrefab.transform.localScale.x;
        float scaley = butPrefab.transform.localScale.y;
        float height = butPrefab.GetComponent<RectTransform>().rect.height * scaley;
        float width = butPrefab.GetComponent<RectTransform>().rect.width * scalex;
        int count = 0;
        foreach (string s in files)
        {
            if (s.Contains(".meta")) continue;
            if (s.Contains(".ini"))
            { 
                GameObject g = Instantiate(butPrefab);
                g.name = s;
                char[] sep = {'\\'};
                string[] tokens = s.Split(sep);
                string b = tokens[tokens.Length - 1];
                g.GetComponentInChildren<Text>().text = b;
                
                Vector3 buttonPos = Vector3.zero;
                if (count % 2 == 0 && count != 0)
                {
                    buttonPos = new Vector3(g.transform.position.x, g.transform.position.y - spacey - (height * count), g.transform.position.z);
                    g.transform.position = buttonPos;
                }

                else if (count % 2 == 1 && count != 0 && count != 1)
                {
                    buttonPos = new Vector3(g.transform.position.x - spacex - width, g.transform.position.y - spacey - (height  * (count - 1)), g.transform.position.z);
                    g.transform.position = buttonPos;
                }
                else if(count == 1)
                {
                    buttonPos = new Vector3(g.transform.position.x - spacex - width, g.transform.position.y, g.transform.position.z);
                    g.transform.position = buttonPos;
                }

                g.transform.SetParent(this.transform, false);
                count++;

            }
        }
     /*   if (count > 24) {
            GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(grid.cellSize.x,grid.cellSize.y/2.0f);
        } */
	}
	
	// Update is called once per frame
	void Update () {
		
	}
}
