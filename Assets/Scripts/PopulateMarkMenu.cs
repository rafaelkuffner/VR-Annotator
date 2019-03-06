﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PopulateMarkMenu : MonoBehaviour {

    // Use this for initialization
    string filesPath = "Assets\\Resources\\Sprites";
    List<string> files;
    void Start()
    {
        string[] f = System.IO.Directory.GetFiles(filesPath);
        files = new List<string>(f);
        GameObject butPrefab = Resources.Load("Prefabs/MarkButton") as GameObject;
        int count = 0;
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
            Sprite p = (Sprite) Resources.Load(b, typeof(Sprite));
            Image[] img = g.GetComponentsInChildren<Image>();
            for(int i = 0; i < img.Length; i++) { 
                if(img[i].gameObject!=g)
                    img[i].sprite = p;
            }
            g.name = b; 
			g.transform.localScale = new Vector3 (0.2f, 0.2f, 0.2f);
			g.transform.SetParent(this.transform, false);
			//g.GetComponentInChildren<Text>().text = b;
            count++;
            
        }
        if (count > 30)
        {
            GridLayoutGroup grid = GetComponent<GridLayoutGroup>();
            grid.cellSize = new Vector2(grid.cellSize.x, grid.cellSize.y / 2.0f);
        }
    }
	
	// Update is called once per frame
	void Update () {
		
	}
}
