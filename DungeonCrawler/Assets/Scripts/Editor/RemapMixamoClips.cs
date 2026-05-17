using UnityEngine;
using UnityEditor;

// Run this once via Tools > Remap Mixamo Strafe Clips
// It fixes the bone path mismatch between Mixamo exports and Ellen's hierarchy.
// Mixamo exports paths starting at "Ellen_Root" but Ellen's Animator expects
// "Ellen_Skeleton/Ellen_Root" (the skeleton sits one level deeper in the hierarchy).
public class RemapMixamoClips : MonoBehaviour
{
    [MenuItem("Tools/Remap Mixamo Strafe Clips")]
    static void Remap()
    {
        string[] fbxPaths = {
            "Assets/Art/Animations/Ellen/Ellen@Jog Strafe Left.fbx",
            "Assets/Art/Animations/Ellen/Ellen@Jog Strafe Right.fbx"
        };

        foreach (var fbxPath in fbxPaths)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            AnimationClip sourceClip = null;
            foreach (var a in assets)
            {
                if (a is AnimationClip c && !c.name.Contains("__preview__"))
                {
                    sourceClip = c;
                    break;
                }
            }

            if (sourceClip == null)
            {
                Debug.LogWarning("No clip found in " + fbxPath);
                continue;
            }

            string savePath = fbxPath.Replace(".fbx", "_Remapped.anim");
            var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(savePath);
            AnimationClip targetClip = existing != null ? existing : new AnimationClip();

            targetClip.ClearCurves();
            targetClip.name = sourceClip.name + "_Remapped";
            targetClip.frameRate = sourceClip.frameRate;
            var clipSettings = AnimationUtility.GetAnimationClipSettings(sourceClip);
            clipSettings.loopTime = true;
            AnimationUtility.SetAnimationClipSettings(targetClip, clipSettings);

            foreach (var binding in AnimationUtility.GetCurveBindings(sourceClip))
            {
                // Skip root transform curves (empty path) — these are root motion
                // and would incorrectly move Ellen_Skeleton as a child bone
                if (string.IsNullOrEmpty(binding.path))
                    continue;

                var newBinding = binding;
                newBinding.path = "Ellen_Skeleton/" + binding.path;

                var curve = AnimationUtility.GetEditorCurve(sourceClip, binding);
                AnimationUtility.SetEditorCurve(targetClip, newBinding, curve);
            }

            if (existing == null)
                AssetDatabase.CreateAsset(targetClip, savePath);
            else
                EditorUtility.SetDirty(targetClip);

            Debug.Log("Saved remapped clip: " + savePath);
        }

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();
        Debug.Log("Done! Use the _Remapped.anim files in your blend tree.");
    }
}
