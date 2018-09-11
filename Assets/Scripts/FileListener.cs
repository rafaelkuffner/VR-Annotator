using UnityEngine;
using System.Collections.Generic;
using System.IO;

public class FileListener : MonoBehaviour {

	public static string NoneMessage = "0";

	private string _fileLocation;
	StreamLineReader file = null;
    private List<string> _stringsToParse;
	void Awake()
	{
		_stringsToParse = new List<string>();
        gameObject.AddComponent<TrackerClientRecorded>();
	}

   public void Initialize(string f)
    {
        if (file != null) file.Dispose();
        _fileLocation = f;

        if (f == "")
        {
            file = null;
            return;
        }
        file= new StreamLineReader(File.OpenRead(_fileLocation));
        Debug.Log("Skeleton initialized, file = " +file);
    }

    public void Skip5Sec()
    {
        if (file != null)
            file.GoToLine(file.CurrentLine + 150);
    }

    public void Back5Sec()
    {
        if(file != null)
            file.GoToLine(file.CurrentLine - 150);
    }

    public void Reset()
    {
        if(file !=null)
        file.GoToLine(0);
	
    }

    public void Close()
    {

        if (file != null) { 
            file.Dispose();
            file = null;
        }
    }

    public bool HasSkeleton()
    {
        return file != null;
    }

    //chamar em um dos eventos. Ter um frame num, se for diferente muda, senão faz nada.
    public void ReadNextLine(int framen)
    {
        if (framen <= file.CurrentLine) return;

        string stringToParse = file.ReadLine();
        if (stringToParse == null || stringToParse == "")
        {
            file.GoToLine(0);
            return;
        }

        List<Body> bodies = new List<Body>();
        if (stringToParse.Length > 1)
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
		file.Dispose();
	}

	void OnQuit()
	{
		OnApplicationQuit();
	}
}
