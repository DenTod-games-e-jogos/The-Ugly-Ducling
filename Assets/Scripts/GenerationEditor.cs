using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

public class GenerationEditor : EditorWindow
{
    [MenuItem("Window/Generation Editor")]

    public static void ShowWindow()
    {
        EditorWindow.GetWindow(typeof(GenerationEditor));
    }

    public int Tab;

    ///World settings
    public World world;
    public int segmentHeight = 16;
    public int segmentSize = 16;
    public int waterHeight = 50;

    ///Noise settings
    public Noise noise;
    public float frequencyOffset = 1;
    public float amplitudeOffset = 1;
    public int maxHeight = 150;
    public float mapOffset = 32000f;
    public int stoneHeight = -30;
    public int sandHeight = -70;
    

    private void OnGUI()
    {
        Tab = GUILayout.Toolbar(Tab, new string[] { "World", "Noise" });
        switch (Tab)
        {
            case 0:
                world = (World)EditorGUILayout.ObjectField(world, typeof(World), true);

                EditorGUI.BeginDisabledGroup(world == null);
                {                   
                    segmentHeight = EditorGUILayout.IntField("segmentHeight", segmentHeight);
                    segmentSize = EditorGUILayout.IntField("segmentSize", segmentSize);
                    waterHeight = EditorGUILayout.IntField("waterHeight", waterHeight);
                    if (GUILayout.Button("Apply"))
                    {
                        world.segmentHeight = segmentHeight;
                        world.segmentSize = segmentSize;
                        world.waterHeight = waterHeight;
                        SaveManager.Save();
                    }

                    if (GUILayout.Button("Default"))
                    {
                        world.segmentHeight = segmentHeight = 16;
                        world.segmentSize = segmentSize = 16;
                        world.waterHeight = waterHeight = 50;
                        SaveManager.Save();
                    }
                }
                EditorGUI.EndDisabledGroup();
                break;

            case 1:
                noise = (Noise)EditorGUILayout.ObjectField(noise, typeof(Noise), true);

                EditorGUI.BeginDisabledGroup(noise == null);
                {                   
                    frequencyOffset = EditorGUILayout.FloatField("frequencyOffset", frequencyOffset);
                    amplitudeOffset = EditorGUILayout.FloatField("amplitudeOffset", amplitudeOffset);
                    maxHeight = EditorGUILayout.IntField("maxHeight", maxHeight);
                    EditorGUILayout.BeginHorizontal();
                    mapOffset = EditorGUILayout.FloatField("mapOffset", mapOffset);
                    if (GUILayout.Button("Randomize"))
                    {
                        mapOffset = Random.Range(-99999,99999);
                    }
                    EditorGUILayout.EndHorizontal();
                    stoneHeight = EditorGUILayout.IntField("stoneHeight", stoneHeight);
                    sandHeight = EditorGUILayout.IntField("sandHeight", sandHeight);

                  
                    if (GUILayout.Button("Apply"))
                    {
                        noise.frequencyOffset = frequencyOffset;
                        noise.amplitudeOffset = amplitudeOffset;
                        noise.maxHeight = maxHeight;
                        noise.mapOffset = mapOffset;
                        noise.stoneHeight = stoneHeight;
                        noise.sandHeight = sandHeight;
                        SaveManager.Save();
                    }

                    if (GUILayout.Button("Default"))
                    {
                        noise.frequencyOffset = frequencyOffset = 1;
                        noise.amplitudeOffset = amplitudeOffset = 1;
                        noise.maxHeight = maxHeight = 150;
                        noise.mapOffset = mapOffset = 32000f;
                        noise.stoneHeight = stoneHeight = -30;
                        noise.sandHeight = sandHeight = -70;
                        SaveManager.Save();
                    }
                }
                EditorGUI.EndDisabledGroup();
                break;
        }
    }
    private void OnInspectorUpdate()
    {
        this.Repaint();
    }
}
