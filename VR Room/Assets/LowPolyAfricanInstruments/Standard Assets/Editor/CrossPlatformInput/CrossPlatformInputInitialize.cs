using System;
using System.Collections.Generic;
using UnityEditor;

namespace UnityStandardAssets.CrossPlatformInput.Inspector
{
    [InitializeOnLoad]
    public class CrossPlatformInitialize
    {
        // Custom compiler defines:
        //
        // CROSS_PLATFORM_INPUT : denotes that cross platform input package exists, so that other packages can use their CrossPlatformInput functions.
        // EDITOR_MOBILE_INPUT : denotes that mobile input should be used in editor, if a mobile build target is selected. (i.e. using Unity Remote app).
        // MOBILE_INPUT : denotes that mobile input should be used right now!

        static CrossPlatformInitialize()
        {
            // 过滤掉非法平台（如 PSM）
            BuildTargetGroup validGroup = BuildTargetGroup.Standalone; // 默认使用 Standalone

            foreach (var group in buildTargetGroups)
            {
                try
                {
                // 尝试访问，判断是否合法
                var defines = GetDefinesList(group);
                validGroup = group;
                break;
                }
                catch (ArgumentException)
                {
                // 忽略非法平台
                }
            }   

            var validDefines = GetDefinesList(validGroup);
            if (!validDefines.Contains("CROSS_PLATFORM_INPUT"))
            {
                SetEnabled("CROSS_PLATFORM_INPUT", true, false);
                SetEnabled("MOBILE_INPUT", true, true);
            }
        }



        [MenuItem("Mobile Input/Enable")]
        private static void Enable()
        {
            SetEnabled("MOBILE_INPUT", true, true);
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                case BuildTarget.PSM: 
                case BuildTarget.Tizen: 
                case BuildTarget.WSAPlayer: 
                    EditorUtility.DisplayDialog("Mobile Input",
                                                "You have enabled Mobile Input. You'll need to use the Unity Remote app on a connected device to control your game in the Editor.",
                                                "OK");
                    break;

                default:
                    EditorUtility.DisplayDialog("Mobile Input",
                                                "You have enabled Mobile Input, but you have a non-mobile build target selected in your build settings. The mobile control rigs won't be active or visible on-screen until you switch the build target to a mobile platform.",
                                                "OK");
                    break;
            }
        }


        [MenuItem("Mobile Input/Enable", true)]
        private static bool EnableValidate()
        {
            var defines = GetDefinesList(mobileBuildTargetGroups[0]);
            return !defines.Contains("MOBILE_INPUT");
        }


        [MenuItem("Mobile Input/Disable")]
        private static void Disable()
        {
            SetEnabled("MOBILE_INPUT", false, true);
            switch (EditorUserBuildSettings.activeBuildTarget)
            {
                case BuildTarget.Android:
                case BuildTarget.iOS:
                    EditorUtility.DisplayDialog("Mobile Input",
                                                "You have disabled Mobile Input. Mobile control rigs won't be visible, and the Cross Platform Input functions will always return standalone controls.",
                                                "OK");
                    break;
            }
        }


        [MenuItem("Mobile Input/Disable", true)]
        private static bool DisableValidate()
        {
            var defines = GetDefinesList(mobileBuildTargetGroups[0]);
            return defines.Contains("MOBILE_INPUT");
        }


        private static BuildTargetGroup[] buildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Standalone,
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS
            };

        private static BuildTargetGroup[] mobileBuildTargetGroups = new BuildTargetGroup[]
            {
                BuildTargetGroup.Android,
                BuildTargetGroup.iOS,
                BuildTargetGroup.PSM, 
                BuildTargetGroup.SamsungTV,
                BuildTargetGroup.Tizen,
                BuildTargetGroup.WSA 
            };


        private static void SetEnabled(string defineName, bool enable, bool mobile)
        {
            foreach (var group in mobile ? mobileBuildTargetGroups : buildTargetGroups)
            {
                try
                {
                    var defines = GetDefinesList(group);

                    if (enable)
                    {
                        if (defines.Contains(defineName))
                            return;

                        defines.Add(defineName);
                    }
                    else
                    {
                        if (!defines.Contains(defineName))
                            return;

                        while (defines.Contains(defineName))
                            defines.Remove(defineName);
                    }

                    string definesString = string.Join(";", defines.ToArray());
                    PlayerSettings.SetScriptingDefineSymbolsForGroup(group, definesString);
                }
                catch (ArgumentException)
                {
                    // 跳过不合法的平台（如 PSM）
                    continue;
                }
            }
        }



        private static List<string> GetDefinesList(BuildTargetGroup group)
        {
            try
            {
                string definesString = PlayerSettings.GetScriptingDefineSymbolsForGroup(group);
                return new List<string>(definesString.Split(';'));
            }
            catch (ArgumentException)
            {
                // 返回空列表，代表该平台不可用
                return new List<string>();
            }
        }

    }
}
