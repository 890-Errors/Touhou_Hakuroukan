using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
namespace ViridaxGameStudios.AI
{
    public class StartupWindow : EditorWindow
    {
        Rect headerRect;
        Rect reviewRect;
        Rect patreonRect;
        Rect closeRect;
        public bool shouldLoad = true;
        public StartupWindow()
        {

        }
        private void OnGUI()
        {
            float width = Screen.width/2 + Screen.width/ 4 + Screen.width / 32;
            headerRect = new Rect(0, 0, width, 520f);
            reviewRect = new Rect(0, headerRect.yMax - 100f, width, 100f);
            patreonRect = new Rect(0, reviewRect.yMax, width, 100f);
            closeRect = new Rect(0, patreonRect.yMax, width, 100f);
            GUIStyle style = new GUIStyle();
            GUIContent label = new GUIContent();
            Texture2D image = (Texture2D)Resources.Load("CandiceAI");

            GUILayout.BeginArea(headerRect);
            style.fixedHeight = 400f;
            label = new GUIContent(image);
            GUILayout.Label(label, style);
            GUILayout.EndArea();


            EditorGUILayout.BeginVertical();
            style = new GUIStyle();
            style.fontStyle = FontStyle.Bold;
            style.alignment = TextAnchor.MiddleCenter;
            GUILayout.BeginArea(reviewRect);
            label = new GUIContent("Creating this asset took a lot of effort and time. Id really appreciate it if you could leave a review for me on the Asset store.");
            style.wordWrap = true;
            GUILayout.Label(label, style);

            if (GUILayout.Button("Leave Review"))
            {
                Application.OpenURL("https://assetstore.unity.com/packages/slug/148441");
            }
            GUILayout.EndArea();
            GUILayout.BeginArea(patreonRect);
            label = new GUIContent("Also, please consider supporting me on Patreon. Your support is what will allow me to continue creating awesome development tools, and ultimately, keep this marvellous asset free.");
            style.wordWrap = true;
            GUILayout.Label(label,style);
            image = (Texture2D)Resources.Load("Patreon");
            
            label = new GUIContent("Become a Patron", image);
            if (GUILayout.Button(label))
            {
                Application.OpenURL("https://www.patreon.com/natubeast");
            }
            GUILayout.Space(16f);
            
            GUILayout.EndArea();

            GUILayout.BeginArea(closeRect);
            shouldLoad = EditorGUILayout.Toggle("Show on startup", shouldLoad);
            if (GUILayout.Button("Close"))
            {
                if (!shouldLoad)
                {
                    Autorun.SaveToFile("0");
                }
                else
                {
                    Autorun.SaveToFile("1");
                }
                Close();
            }
            GUILayout.EndArea();
            EditorGUILayout.EndVertical();
        }
    }
}

