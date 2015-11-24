using System.Collections.Generic;
using UnityEngine;

public class Fortunes {

    List<string> fortunes;

    public Fortunes(TextAsset text) {
        fortunes = new List<string> ();
        if (!LoadFortunesFromText(text)) {
            fortunes.Add ("File reading fucked up.");
        }
    }

    /// <summary>
    /// Pick a random fortune from the list
    /// </summary>
    public string GetRandom() {
        int index = Random.Range(0, fortunes.Count);
        return fortunes [index]; 
    }

    /// <summary>
    /// Decompose the given text asset into its individual fortunes.
    /// </summary>
    private bool LoadFortunesFromText(TextAsset text)
    {
        string rawText = text.text; //The raw contents of the file, newlines included.
        fortunes = new List<string>(rawText.Split('\n'));
        //check to see if we successfully loaded the fortunes
        return fortunes.Count > 0;
    }
}