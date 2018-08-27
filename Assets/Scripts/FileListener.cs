using UnityEngine;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

public class FileListener : MonoBehaviour {

	public static string NoneMessage = "0";

	private string _fileLocation;
	StreamReader file;
	private List<string> _stringsToParse;

	void Start()
	{
        file = null;
		_stringsToParse = new List<string>();
        gameObject.AddComponent<TrackerClientRecorded>();
	}

   public void Initialize(string f)
    {
        if (file != null) file.Close();
        _fileLocation = f;
        file= new StreamReader(_fileLocation);
    }

    public void Skip5Sec()
    {
        //implement
    }

    public void Back5Sec()
    {
        //implement
    }

    public void Reset()
    {
        //implement
    }

    public void Close()
    {
        file.Close();
    }

    //chamar em um dos eventos. Ter um frame num, se for diferente muda, senão faz nada.
    public void ReadNextLine()
    {
       
        string stringToParse = file.ReadLine();
        if (stringToParse == null)
        {
            file.BaseStream.Position = 0;
            file.DiscardBufferedData();
            stringToParse = file.ReadLine();
        }

        List<Body> bodies = new List<Body>();
        if (stringToParse.Length != 1)
        {
            List<string> bstrings = new List<string>(stringToParse.Split(MessageSeparators.L1));
            bstrings.RemoveAt(0); // first statement is not a body
            foreach (string b in bstrings)
            {
                if (b != NoneMessage) bodies.Add(new Body(b));
            }
        }
        gameObject.GetComponent<TrackerClientRecorded>().SetNewFrame(bodies.ToArray());
       
    }

	void OnApplicationQuit()
	{
		file.Close ();
	}

	void OnQuit()
	{
		OnApplicationQuit();
	}
}
