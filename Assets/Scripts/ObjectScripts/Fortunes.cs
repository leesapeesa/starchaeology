using System;
using System.Text;
using System.IO;
using System.Collections.Generic; 

public class Fortunes {

    List<string> fortunes;
    int index;

    public Fortunes() {
        fortunes = new List<string> ();
        if (!Load ("Assets/Misc/fortunes.txt")) {
            fortunes.Add ("File reading fucked up.");
        }
        index = 0;
    }

    public string Next() {
        index %= fortunes.Count;
        return fortunes [index++]; // I'm purposely using the post decremement operator
    }

    // Found from http://answers.unity3d.com/questions/279750/loading-data-from-a-txt-file-c.html
    // Loads text from file line by line into fortunes.
    private bool Load(string fileName)
    {
        // Handle any problems that might arise when reading the text
        try
        {
            string line;
            // Create a new StreamReader, tell it which file to read and what encoding the file
            // was saved as
            StreamReader theReader = new StreamReader(fileName, Encoding.Default);
            // Immediately clean up the reader after this block of code is done.
            // You generally use the "using" statement for potentially memory-intensive objects
            // instead of relying on garbage collection.
            // (Do not confuse this with the using directive for namespace at the 
            // beginning of a class!)
            using (theReader)
            {
                // While there's lines left in the text file, do this:
                do
                {
                    line = theReader.ReadLine();
                    
                    if (line != null)
                    {
                        // Add our fortunes from a file.
                        fortunes.Add (line);
                    }
                }
                while (line != null);
                // Done reading, close the reader and return true to broadcast success    
                theReader.Close();
                return true;
            }
        }
        // If anything broke in the try block, we throw an exception with information
        // on what didn't work
        catch (Exception e)
        {
            Console.WriteLine("{0}\n", e.Message);
            return false;
        }
    }
}