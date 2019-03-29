using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PopulateMarkMenu : MonoBehaviour {

    // Use this for initialization
    string filesPath = "Assets\\Resources\\Sprites";
    List<string> files;
    float spacex = 4f;
    float spacey = 4f;
    int line = 0;
    void Start()
    {
        string[] f = System.IO.Directory.GetFiles(filesPath);
        files = new List<string>(f);
        GameObject butPrefab = Resources.Load("Prefabs/MarkButton") as GameObject;
        int count = 0; float scalex = butPrefab.transform.localScale.x;
        float scaley = butPrefab.transform.localScale.y;
        float height = butPrefab.GetComponent<RectTransform>().rect.height * scaley;
        float width = butPrefab.GetComponent<RectTransform>().rect.width * scalex;
        foreach (string s in files)
        {
            if (s.Contains(".meta")) continue;
            GameObject g = Instantiate(butPrefab);
            char[] sep = { '\\' };
            string[] tokens = s.Split(sep);
            string b = tokens[tokens.Length - 1];
            b = b.Replace(".png", "");
            b = b.Replace(".jpg", "");
            b = b.Replace(".bmp", "");
            b = "Sprites/" + b;
            Sprite p = (Sprite)Resources.Load(b, typeof(Sprite));
            Image[] img = g.GetComponentsInChildren<Image>();
            for (int i = 0; i < img.Length; i++)
            {
                if (img[i].gameObject != g)
                    img[i].sprite = p;
            }
            g.name = b;
            //g.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
            Vector3 buttonPos = Vector3.zero;
            if (count < 5 && count != 0)
            {
                buttonPos = new Vector3(g.transform.position.x + (spacex * count) + (width * count), g.transform.position.y - (spacey * line) - (height * line), g.transform.position.z);
                g.transform.position = buttonPos;
            }
            else if (count == 0)
            {
                buttonPos = new Vector3(g.transform.position.x, g.transform.position.y - (spacey * line) - (height * line), g.transform.position.z);
                g.transform.position = buttonPos;
            }
            else
            {
                count = 0;
                line++;
                continue;
            }

            g.transform.SetParent(this.transform, false);
            count++;
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
