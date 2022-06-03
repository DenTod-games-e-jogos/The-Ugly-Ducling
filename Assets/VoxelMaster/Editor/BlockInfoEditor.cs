using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

namespace VoxelMaster
{
    [CustomEditor(typeof(BlockInfo))]
    public class BlockInfoEditor : Editor
    {
        private static VoxelTerrain _terrain;
        private static VoxelTerrain terrain
        {
            get
            {
                if (_terrain == null)
                    _terrain = FindObjectOfType<VoxelTerrain>();
                return _terrain;
            }
        }

        private static Mesh cubeMesh;
        private static Material defaultMaterial
        {
            get
            {
                if (terrain == null) return null;

                return terrain.material;
            }
        }

        private static PreviewRenderUtility previewRender;
        private Material previewMaterial;
        private int tiling;

        private Vector2 drag = new Vector2(30, -30);

        private void ValidateData()
        {
            if (previewRender == null)
            {
                previewRender = new PreviewRenderUtility();
            }
        }

        public override bool HasPreviewGUI()
        {
            ValidateData();
            return true;
        }

        public override void OnInspectorGUI()
        {
            DrawDefaultInspector();

            EditorGUILayout.Space();

            EditorGUILayout.LabelField("Preview Settings", EditorStyles.boldLabel);
            EditorGUILayout.BeginHorizontal();
            previewMaterial = (Material)EditorGUILayout.ObjectField(previewMaterial, typeof(Material), false);
            tiling = EditorGUILayout.IntField(tiling);
            EditorGUILayout.EndHorizontal();
        }

        public override void OnPreviewGUI(Rect r, GUIStyle background)
        {
            drag = Drag2D(drag, r);

            if (Event.current.type == EventType.Repaint)
            {
                RefreshBlockObject();
                if (cubeMesh == null || previewMaterial == null) return;

                previewRender.BeginPreview(r, background);
                previewRender.DrawMesh(cubeMesh, new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity, previewMaterial, 0);

                previewRender.camera.transform.position = Vector2.zero;
                previewRender.camera.transform.rotation = Quaternion.Euler(new Vector3(-drag.y, -drag.x, 0));
                previewRender.camera.transform.position = previewRender.camera.transform.forward * -6f;
                previewRender.camera.Render();

                Texture resultRender = previewRender.EndPreview();
                GUI.DrawTexture(r, resultRender, ScaleMode.StretchToFill, true);
            }
        }

        public override Texture2D RenderStaticPreview(string assetPath, Object[] subAssets, int width, int height)
        {
            RefreshBlockObject();

            previewRender.BeginPreview(new Rect(0, 0, width / 2, height / 2), GUIStyle.none);
            previewRender.DrawMesh(cubeMesh, new Vector3(-0.5f, -0.5f, -0.5f), Quaternion.identity, previewMaterial, 0);

            previewRender.camera.transform.position = Vector2.zero;
            previewRender.camera.transform.rotation = Quaternion.Euler(new Vector3(-drag.y, -drag.x, 0));
            previewRender.camera.transform.position = previewRender.camera.transform.forward * -6f;
            previewRender.camera.Render();

            Texture resultRender = previewRender.EndPreview();
            Texture2D thumbnail = new Texture2D(width, height);
            RenderTexture.active = (RenderTexture)resultRender;
            thumbnail.ReadPixels(new Rect(0, 0, width, height), 0, 0);
            thumbnail.Apply();
            RenderTexture.active = null;
            return thumbnail;
        }

        private static Vector2 Drag2D(Vector2 scrollPosition, Rect position)
        {
            int controlID = GUIUtility.GetControlID("Slider".GetHashCode(), FocusType.Passive);
            Event current = Event.current;
            switch (current.GetTypeForControl(controlID))
            {
                case EventType.MouseDown:
                    if (position.Contains(current.mousePosition) && position.width > 50f)
                    {
                        GUIUtility.hotControl = controlID;
                        current.Use();
                        EditorGUIUtility.SetWantsMouseJumping(1);
                    }
                    break;
                case EventType.MouseUp:
                    if (GUIUtility.hotControl == controlID)
                    {
                        GUIUtility.hotControl = 0;
                    }
                    EditorGUIUtility.SetWantsMouseJumping(0);
                    break;
                case EventType.MouseDrag:
                    if (GUIUtility.hotControl == controlID)
                    {
                        scrollPosition -= current.delta * (float)((!current.shift) ? 1 : 3) / Mathf.Min(position.width, position.height) * 140f;
                        scrollPosition.y = Mathf.Clamp(scrollPosition.y, -90f, 90f);
                        current.Use();
                        GUI.changed = true;
                    }
                    break;
            }
            return scrollPosition;
        }

        void RefreshBlockObject()
        {
            BlockInfo blockInfo = (BlockInfo)target;

            if (cubeMesh == null)
            {
                cubeMesh = new Mesh();

                cubeMesh.Clear();

                // VERTICES
                cubeMesh.vertices = new Vector3[24]
                {
                //FORWARD FACE
                new Vector3(0, 1, 0),
                new Vector3(1, 1, 0),
                new Vector3(1, 0, 0),
                new Vector3(0, 0, 0),

                //LEFT FACE
                new Vector3(0, 1, 1),
                new Vector3(0, 1, 0),
                new Vector3(0, 0, 0),
                new Vector3(0, 0, 1),

                //BACK FACE
                new Vector3(1, 1, 1),
                new Vector3(0, 1, 1),
                new Vector3(0, 0, 1),
                new Vector3(1, 0, 1),

                //RIGHT FACE
                new Vector3(1, 1, 0),
                new Vector3(1, 1, 1),
                new Vector3(1, 0, 1),
                new Vector3(1, 0, 0),

                //TOP FACE
                new Vector3(0, 1, 1),
                new Vector3(1, 1, 1),
                new Vector3(1, 1, 0),
                new Vector3(0, 1, 0),

                //BOTTOM FACE
                new Vector3(0, 0, 0),
                new Vector3(1, 0, 0),
                new Vector3(1, 0, 1),
                new Vector3(0, 0, 1)
                };

                // TRIANGLES
                List<int> triangles = new List<int>();
                for (int i = 0; i < 6; i++)
                {
                    int o = i * 4;
                    triangles.AddRange(new int[]{
                    0+o, 1+o, 2+o,
                    2+o, 3+o, 0+o,
                    });
                }
                cubeMesh.triangles = triangles.ToArray();

                cubeMesh.RecalculateNormals();
            }

            // UVS
            List<Vector2> uv = new List<Vector2>();

            uv.AddRange(GetUVs(blockInfo.blockTexture.front));
            uv.AddRange(GetUVs(blockInfo.blockTexture.left));
            uv.AddRange(GetUVs(blockInfo.blockTexture.back));
            uv.AddRange(GetUVs(blockInfo.blockTexture.right));
            uv.AddRange(GetUVs(blockInfo.blockTexture.top));
            uv.AddRange(GetUVs(blockInfo.blockTexture.bottom));

            cubeMesh.uv = uv.ToArray();
        }

        void Reset()
        {
            previewMaterial = defaultMaterial;
            tiling = (terrain != null ? terrain.tiling : 16);
        }

        void OnDestroy()
        {
            DestroyImmediate(cubeMesh);
        }

        private Vector2[] GetUVs(int id)
        {
            Vector2[] uv = new Vector2[4];

            int id2 = id + 1;
            float o = 1f / tiling;
            int i = 0;

            for (int y = 0; y < tiling; y++)
            {
                for (int x = 0; x < tiling; x++)
                {
                    i++;

                    if (i == id2)
                    {
                        float padding = 0.01f / tiling; // Adding a little padding to prevent UV bleeding (to fix)
                        uv[0] = new Vector2(x / (float)tiling + padding, 1f - (y / (float)tiling) - padding);
                        uv[1] = new Vector2(x / (float)tiling + o - padding, 1f - (y / (float)tiling) - padding);
                        uv[2] = new Vector2(x / (float)tiling + o - padding, 1f - (y / (float)tiling + o) + padding);
                        uv[3] = new Vector2(x / (float)tiling + padding, 1f - (y / (float)tiling + o) + padding);

                        return uv;
                    }
                }
            }

            uv[0] = Vector3.zero;
            uv[1] = Vector3.zero;
            uv[2] = Vector3.zero;
            uv[3] = Vector3.zero;

            return uv;
        }
    }
}