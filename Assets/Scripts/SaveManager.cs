using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;

public class SaveManager
{
    public static void Save()
    {
        string dataToSave;
        dataToSave = World.Instance.segmentHeight + ","
            + World.Instance.segmentSize + ","
            + World.Instance.waterHeight + ","
            + Noise.Instance.frequencyOffset + ","
            + Noise.Instance.amplitudeOffset + ","
            + Noise.Instance.maxHeight + ","
            + Noise.Instance.mapOffset + ","
            + Noise.Instance.stoneHeight + ","
             + Noise.Instance.sandHeight;
        File.WriteAllText(Application.persistentDataPath + "/Generation.setting", dataToSave);
    }
    public static void Load()
    {
        if (File.Exists(Application.persistentDataPath + "/Generation.setting"))
        {
            string path = Application.persistentDataPath + "/Generation.setting";
            StreamReader streamReader = new StreamReader(path);
            string data = streamReader.ReadLine();
            string[] splitData = data.Split(',');
            World.Instance.segmentHeight = int.Parse(splitData[0]);
            World.Instance.segmentSize = int.Parse(splitData[1]);
            World.Instance.waterHeight = int.Parse(splitData[2]);
            Noise.Instance.frequencyOffset = float.Parse(splitData[3]);
            Noise.Instance.amplitudeOffset = float.Parse(splitData[4]);
            Noise.Instance.maxHeight = int.Parse(splitData[5]);
            Noise.Instance.mapOffset = float.Parse(splitData[6]);
            Noise.Instance.stoneHeight = int.Parse(splitData[7]);
            Noise.Instance.sandHeight = int.Parse(splitData[8]);
        }
    }
}
