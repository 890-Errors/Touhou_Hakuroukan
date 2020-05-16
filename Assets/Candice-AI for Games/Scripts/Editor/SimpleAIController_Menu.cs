using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;

namespace ViridaxGameStudios.AI
{
    public class SimpleAIController_Menu : MonoBehaviour
    {
        [MenuItem("Window/Viridax Game Studios/AI/Simple AI Controller")]
        public static void CreateAIScript()
        {
            GameObject[] selectedGO = Selection.gameObjects;
            if (selectedGO.Length > 0)
            {
                foreach(GameObject obj in selectedGO)
                {
                    AttachAIControllerScript(obj);
                }
                
            }
            else
            {
                EditorUtility.DisplayDialog("AI Tools", "You need to select at least 1 GameObject", "OK");
            }

        }


        static void AttachAIControllerScript(GameObject obj)
        {
            //Assign AI Script to the GameObject
            SimpleAIController AIscript = null;
            if (obj)
            {
                AIscript = obj.AddComponent<SimpleAIController>();
                Selection.activeGameObject = obj;
            }
        }
    }
}

