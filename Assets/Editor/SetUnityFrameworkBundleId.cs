#if UNITY_IOS
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEditor.iOS.Xcode;
using System.IO;

public class SetUnityFrameworkBundleId
{
    [PostProcessBuild]
    public static void OnPostProcessBuild(BuildTarget target, string pathToBuiltProject)
    {
        if (target != BuildTarget.iOS)
            return;

        string projectPath = PBXProject.GetPBXProjectPath(pathToBuiltProject);
        PBXProject project = new PBXProject();
        project.ReadFromFile(projectPath);

        // UnityFramework ÇÃ target ÇéÊìæ
        string unityFrameworkTarget = project.GetUnityFrameworkTargetGuid();

        // Bundle Identifier Çé©ìÆê›íË
        project.SetBuildProperty(
            unityFrameworkTarget,
            "PRODUCT_BUNDLE_IDENTIFIER",
            "com.keiya.pushermedalrush.framework"
        );

        project.WriteToFile(projectPath);
    }
}
#endif
