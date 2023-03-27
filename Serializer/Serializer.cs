using System.Collections.Generic;
using System.IO;

namespace IOSerializer;

public static class Serializer
{
    public static void Serialize(FileStream stream, Dictionary<string, string> sPairs, Dictionary<string, float> fPairs)
    {
        string output = "";

        foreach (var pair in sPairs)
        {
            if (pair.Key.Contains('\n'))
                pair.Key.Replace("\n", "\\n");
            if (pair.Value.Contains('\n'))
                pair.Value.Replace("\n", "\\n");

            output += $"{pair.Key}={pair.Value}\0";
        }

        output += '\n';

        foreach (var pair in fPairs)
            output += $"{pair.Key}={pair.Value}\0";

        StreamWriter streamWriter = new StreamWriter(stream);
        streamWriter.WriteAsync(output);
        streamWriter.Flush();
        stream.Flush();
    }

    public static void Deserialize(FileStream stream, out Dictionary<string, string> sPairs, out Dictionary<string, float> fPairs)
    {
        sPairs = new Dictionary<string, string>();
        fPairs = new Dictionary<string, float>();

        string[] strings = new StreamReader(stream).ReadToEnd().Replace(" r0", "").Split('\n');

        if (strings.Length < 1)
            return;

        if (strings[0].Length > 2)
        {
            string[] sStrings = strings[0].Split('\0');
            foreach (string sString in sStrings)
                if (!string.IsNullOrWhiteSpace(sString))
                    sPairs.Add(sString.Split('=')[0], sString.Split('=')[1]);
        }

        if (strings.Length < 2)
            return;

        if (strings[1].Length > 2)
        {
            string[] fStrings = strings[1].Split('\0');
            foreach (string fString in fStrings)
                if (!string.IsNullOrWhiteSpace(fString))
                    fPairs.Add(fString.Split('=')[0], float.Parse(fString.Split('=')[1]));
        }
    }
}