using UnityEditor;
using UnityEngine;
namespace ViridaxGameStudios.AI
{
    public class CandiceAIManager_Menu : EditorWindow
    {
        [MenuItem("Window/Viridax Game Studios/AI/"+CandiceConfig.MANAGER_NAME)]
        public static void AddManager()
        {
            if(!ManagerExists())
            {
                GameObject go = GameObject.Find(CandiceConfig.MANAGER_NAME);
                if(go != null)
                {

                }
                else
                {
                    go = new GameObject(CandiceConfig.MANAGER_NAME);
                    go.AddComponent<CandiceAIManager>();
                    go.AddComponent<Grid>();
                }

            }
            else
            {
                EditorUtility.DisplayDialog(CandiceConfig.APP_NAME, CandiceConfig.MANAGER_NAME + " already exists.", "OK");
            }
        }

        static bool ManagerExists()
        {
            bool isExists = false;
            GameObject[] arrGO = FindObjectsOfType<GameObject>();
            int count = 0;
            while(!isExists && count < arrGO.Length)
            {
                GameObject go = arrGO[count];
                CandiceAIManager aiManager = go.GetComponent<CandiceAIManager>();
                if(aiManager != null)
                {
                    isExists = true;
                }
                count++;
            }
            return isExists;
        }
    }
}

