using System.Collections.Generic;
using UnityEngine;

public class Fortunes {

    List<string> fortunes;
    int index;

    public Fortunes(TextAsset text) {
        fortunes = new List<string> ();
        if (!LoadFortunesFromText(text)) {
            fortunes.Add ("File reading fucked up.");
        }
        index = 0;
    }

    // Gets the next fortune in the list.
    // We can make it so that it is a random fortune.
    public string Next() {
        index %= fortunes.Count;
        return fortunes [index++]; // I'm purposely using the post decremement operator
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