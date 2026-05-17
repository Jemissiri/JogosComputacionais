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
            "Assets/Art/Animations/Ellen/Ellen@Jog Strafe Right.fbx",
            "Assets/Art/Animations/Ellen/Ellen@Standing Dodge Forward.fbx",
            "Assets/Art/Animations/Ellen/Ellen@Standing Dodge Backward.fbx"
        };

        foreach (var fbxPath in fbxPaths)
        {
            var assets = AssetDatabase.LoadAllAssetsAtPath(fbxPath);
            Debug.Log(fbxPath + " — total assets loaded: " + assets.Length);
            foreach (var a in assets)
                Debug.Log("  asset: " + a.GetType().Name + " name='" + a.name + "'");

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
                Debug.LogWarning("No clip found in " + fbxPath + " — try right-clicking the FBX in the Project window and choosing Reimport, then run this again.");
                continue;
            }
            Debug.Log(fbxPath + " — using clip: '" + sourceClip.name + "'");

            string savePath = fbxPath.Replace(".fbx", "_Remapped.anim");
            var existing = AssetDatabase.LoadAssetAtPath<AnimationClip>(savePath);
            AnimationClip targetClip = existing != null ? existing : new AnimationClip();

            targetClip.ClearCurves();
            targetClip.name = sourceClip.name + "_Remapped";
            targetClip.frameRate = sourceClip.frameRate;
            var clipSettings = AnimationUtility.GetAnimationClipSettings(sourceClip);
            clipSettings.loopTime = fbxPath.Contains("Strafe");
            AnimationUtility.SetAnimationClipSettings(targetClip, clipSettings);

            var bindings = AnimationUtility.GetCurveBindings(sourceClip);
            Debug.Log(fbxPath + " — source curves: " + bindings.Length);

            foreach (var binding in bindings)
            {
                if (string.IsNullOrEmpty(binding.path)) continue;

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
