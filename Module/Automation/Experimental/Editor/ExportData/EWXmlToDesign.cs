using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Module.Automation.Generator;
using System.Net;
using System.Text;
using static Module.Automation.Generator.TableFilePathDatas;

public class EWXmlToDesign : EditorWindow
{
    //string DATAMGR_OUTPUT_PATH = "/Automation/Output";
    string tableName;
    static TalbeSettingData settingData;

    [MenuItem("Tool/DesignTool/Generator")]
    static void OpenGeneratorTool()
    {
        GetWindow<EWXmlToDesign>();
        settingData = TalbeSettingData.LoadSettingData(Application.dataPath + TABLE_DATA_PATH);

    }

    void LoadEnum()
    {
        EnumGenerator enumGenerator = new EnumGenerator();
        enumGenerator.Load(ENUM_PATH, ENUM_OUTPUT_PATH);
    }

    void LoadTable(string address)
    {
        TableGenerator tableGenerator = new TableGenerator();
        tableGenerator.Load(TABLE_PATH + address + ".xml", TABLE_OUTPUT_PATH, DATAMGR_OUTPUT_PATH, TABLE_PATH, settingData.comTableAssetPath, DATAMESSAGESERIALIZER_OUTPUT_PATH);

        //ExportDllDesignMessages(DLL_OUTPUT_PTAH, DATAMESSAGES_DLLFILE_NAME, settingData.gmsPath, Application.dataPath + TABLE_OUTPUT_PATH);
        ExportDllDesignMessages(DLL_OUTPUT_PTAH, DATAMGR_DLLFILE_NAME, settingData.gmsPath, Application.dataPath + DATAMGR_OUTPUT_PATH);
        tableGenerator.ExportDataByteOneFile(address.Replace("_data_",""), DESIGNBYTEFILE_OUTPUT_PATH);
    }

    void LoadAllTable()
    {
        TableGenerator tableGenerator = new TableGenerator();
        tableGenerator.LoadAll(TABLE_PATH, TABLE_OUTPUT_PATH, DATAMGR_OUTPUT_PATH, settingData.comTableAssetPath, DATAMESSAGESERIALIZER_OUTPUT_PATH);
        //ExportDllDesignMessages(DLL_OUTPUT_PTAH, DATAMESSAGES_DLLFILE_NAME, settingData.gmsPath, Application.dataPath + TABLE_OUTPUT_PATH);
        ExportDllDesignMessages(DLL_OUTPUT_PTAH, DATAMGR_DLLFILE_NAME, settingData.gmsPath, Application.dataPath + DATAMGR_OUTPUT_PATH);
        tableGenerator.ExportDataByteFile(DESIGNBYTEFILE_OUTPUT_PATH);
    }


    public void ExportDllDesignMessages(string outPath,string dllFileName, string gmsFileName, string readPath)
    {
        System.Diagnostics.ProcessStartInfo startInfo = new System.Diagnostics.ProcessStartInfo();
        StringBuilder sb = new StringBuilder();

        sb.AppendFormat(" -target:library -unsafe+ ");
        sb.AppendFormat(" -out:{0}/{1}.dll", outPath, dllFileName);
        sb.AppendFormat(" /r:{0}/protobuf-net.dll", readPath);
        sb.AppendFormat(" {0}/*.cs", readPath);

        Debug.Log(sb.ToString());
        startInfo.FileName = gmsFileName;
        startInfo.Arguments = sb.ToString();

        using (System.Diagnostics.Process exeProcess = System.Diagnostics.Process.Start(startInfo))
        {
            exeProcess.WaitForExit();
        }
    }

    private void OnGUI()
    {
        EditorGUILayout.LabelField("Design Generator");

        GUILayout.Label("enum", EditorStyles.boldLabel);
        if (GUILayout.Button("Generate"))
        {
            LoadEnum();
            EditorUtility.DisplayDialog("Complete", "Complete!", "OK");
        }


        tableName = EditorGUILayout.TextField("TalbleName", tableName);
        using (new EditorGUILayout.HorizontalScope())
        {
            if (GUILayout.Button("Generate"))
            {
                if (tableName == null)
                {
                    EditorUtility.DisplayDialog("Error", "테이블 이름이 없습니다 ","OK");
                }
                else
                {
                    LoadTable(tableName);
                    EditorUtility.DisplayDialog("Complete", "Complete!", "OK");

                }

            }

            if (GUILayout.Button("AllGenerate"))
            {
                LoadAllTable();
                EditorUtility.DisplayDialog("Complete", "Complete!", "OK");
            }
        }
    }


}
