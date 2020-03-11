using UnityEditor;
using UnityEngine;

namespace AnimationImporter
{
    [CustomEditor(typeof(AnimationImporterSharedConfig))]
    public class AnimationImporterSharedConfigEditor : Editor
    {
        public override void OnInspectorGUI()
        {
            GUI.enabled = false;
            base.OnInspectorGUI();
            GUI.enabled = true;
        }
    }
}