using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static Module.Automation.Generator.TableFilePathDatas;

namespace Module.Automation.Generator
{
    public class TableAssetManagement : EditorWindow
    {
        static string gmsPath = "C:/Program Files/Unity/Hub/Editor/2021.3.16f1/Editor/Data/MonoBleedingEdge/bin/mcs.bat";
        static UnityEngine.Object comTableAsset;
        [MenuItem("Tool/DesignTool/Setting")]
        static void OpenGeneratorSettingTool()
        {
            EditorWindow.GetWindow<TableAssetManagement>();
            TalbeSettingData.LoadData(Application.dataPath + TABLE_DATA_PATH, out gmsPath, out comTableAsset);
        }

        private void OnGUI()
        {
            EditorGUILayout.LabelField("Generator Setting");

            GUILayout.Label("GmsPath", EditorStyles.boldLabel);
            gmsPath = EditorGUILayout.TextField("GmsPath", gmsPath);
            comTableAsset = EditorGUILayout.ObjectField("ComTableAsset", comTableAsset, typeof(UnityEngine.Object), true);

            using (new EditorGUILayout.HorizontalScope())
            {
                if (GUILayout.Button("Save"))
                {
                    TalbeSettingData.SaveData(Application.dataPath + TABLE_DATA_PATH,gmsPath, AssetDatabase.GetAssetPath(comTableAsset));
                }

                if (GUILayout.Button("Load"))
                {
                    TalbeSettingData.LoadData(Application.dataPath + TABLE_DATA_PATH, out gmsPath, out comTableAsset);
                }
            }
        }

    }
}

