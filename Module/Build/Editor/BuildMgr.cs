using System.Collections;
using System.Collections.Generic;
using UnityEngine;

using UnityEditor;
using UnityEditor.Build.Reporting;
using UnityEditor.AddressableAssets.Settings;
using UnityEditor.AddressableAssets;
using System;
using Codice.Utils;

public class BuildMgr
{
    [MenuItem("Tool/Build/AOS")]
    public static void Build_AOS()
    {
        BuildAddressable();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FindEnableEditorScenes();
        buildPlayerOptions.target = BuildTarget.Android;
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.locationPathName = "Builds/AOS/test.apk";
        StartBuild(buildPlayerOptions);
    }

    [MenuItem("Tool/Build/IOS")]
    public static void Build_IOS()
    {
        BuildAddressable();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FindEnableEditorScenes();
        buildPlayerOptions.target = BuildTarget.iOS;
        buildPlayerOptions.options = BuildOptions.None;
        buildPlayerOptions.locationPathName = "Builds/IOS/test.apk";
        StartBuild(buildPlayerOptions);
    }

    [MenuItem("Tool/Build/EXE")]
    public static void Build_EXE()
    {
        BuildAddressable();

        BuildPlayerOptions buildPlayerOptions = new BuildPlayerOptions();
        buildPlayerOptions.scenes = FindEnableEditorScenes();
        buildPlayerOptions.target = BuildTarget.StandaloneWindows;
        buildPlayerOptions.options = BuildOptions.Development;
        buildPlayerOptions.locationPathName = "Builds/EXE/ProjectJ.exe";
        StartBuild(buildPlayerOptions);
    }

    private static void StartBuild(BuildPlayerOptions buildPlayerOptions)
    {
        BuildReport report = BuildPipeline.BuildPlayer(buildPlayerOptions);
        BuildSummary summary = report.summary;

        if (summary.result == BuildResult.Succeeded)
        {
            Debug.Log("Build succeeded" + summary.totalSize + "bytes");
        }

        if (summary.result == BuildResult.Failed)
        {
            Debug.Log("Build Failed");
        }
    }
    

    private static string[] FindEnableEditorScenes()
    {
        List<string> EditorScenes = new List<string>();

        foreach (EditorBuildSettingsScene scene in EditorBuildSettings.scenes)
        {
            if (!scene.enabled)
                continue;
            EditorScenes.Add(scene.path);
        }

        return EditorScenes.ToArray();
    }

    public static void BuildAddressable()
    {
        AddressableAssetSettings.CleanPlayerContent(AddressableAssetSettingsDefaultObject.Settings.ActivePlayerDataBuilder);
        AddressableAssetSettings.BuildPlayerContent();
    }
}
