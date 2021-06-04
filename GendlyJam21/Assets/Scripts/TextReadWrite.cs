using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;
using SimpleJSON;

public class HandleTextFile
{
    [MenuItem("Tools/Write file")]
    static void WriteString(string _write)
    {
        string path = "Assets/Resources/highscores.txt";

        //Write some text to the test.txt file
        StreamWriter writer = new StreamWriter(path, true);
        writer.WriteLine(_write);
        writer.Close();

        //Re-import the file to update the reference in the editor
        //AssetDatabase.ImportAsset(path);
        //TextAsset asset = Resources.Load<TextAsset>("test");

        //Print the text from the file
        //Debug.Log(asset.text);

    }

    [MenuItem("Tools/Read file")]
    static string ReadString()
    {
        string path = "Assets/Resources/highscores.txt";

        //Read the text from directly from the test.txt file
        StreamReader reader = new StreamReader(path);
        return reader.ReadToEnd();
    }
}

public class TextReadWrite : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {

    }

    // Update is called once per frame
    void Update()
    {

    }
}
