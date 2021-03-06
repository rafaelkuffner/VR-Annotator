﻿using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;
using System.IO;

public class PopulateEffectsMenu : MonoBehaviour {

    // Use this for initialization
    string filesPath = "Assets\\Resources\\EffectPrefabs";
    List<string> files;
    void Start()
    {
        string[] f = System.IO.Directory.GetFiles(filesPath);
        files = new List<string>(f);
        GameObject butPrefab = Resources.Load("Prefabs/Button") as GameObject;
        int count = 0;
        foreach (string s in files)
        {
            if (s.Contains(".meta")) continue;
            GameObject g = Instantiate(butPrefab);
            g.name = s;
            char[] sep = { '\\' };
            string[] tokens = s.Split(sep);
            string b = tokens[tokens.Length - 1];
            b = b.Replace(".prefab", "");
            g.GetComponentInChildren<Text>().text = b;
            g.transform.SetParent(this.transform, false);
            g.name = b;
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
