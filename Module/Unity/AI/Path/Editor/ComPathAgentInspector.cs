namespace Module.Unity.AI
{
    using System.Collections;
    using System.Collections.Generic;
    using System.IO;
    using UnityEditor;
    using UnityEngine;

    [CustomEditor(typeof(ComPathAgent))]
    public class ComPathAgentInspector : Editor
    {
        ComPathAgent script;
        public override void OnInspectorGUI()
        {
            if (script.PathInfo.Count != 0)
            {
                if (GUILayout.Button("Generate"))
                {
                    script.GenerateData();
                }
            }
            
            if (GUILayout.Button("Clear"))
            {
                script.ClearPath();
            }

            base.OnInspectorGUI();
        }


        private void OnEnable()
        {
            script = (ComPathAgent)target;
        }
    }

}
