using UnityEngine;
using UnityEditor;
using System.IO;
using System.Linq;
using UnityEngine.U2D.Animation;
using System;

#if UNITY_EDITOR
public class SpriteAnimationGenerator : EditorWindow
{
    private float frameRate = 12f;
    private string animationName = "NewAnimation";
    private DefaultAsset spriteFolder;

    [MenuItem("Tools/Generate Sprite Animation")]
    public static void ShowWindow()
    {
        GetWindow<SpriteAnimationGenerator>("Animation Generator");
    }

    private void OnGUI()
    {
        GUILayout.Label("Sprite Animation Settings", EditorStyles.boldLabel);
        
        spriteFolder = (DefaultAsset)EditorGUILayout.ObjectField("Sprite Folder", spriteFolder, typeof(DefaultAsset), false);
        animationName = EditorGUILayout.TextField("Animation Name", animationName);
        frameRate = EditorGUILayout.FloatField("Frame Rate", frameRate);

        if (GUILayout.Button("Generate Animation"))
        {
            if (spriteFolder == null)
            {
                Debug.LogError("Please select a sprite folder");
                return;
            }

            GenerateAnimation();
        }
    }

    private void GenerateAnimation()
    {
        // 获取文件夹路径
        string folderPath = AssetDatabase.GetAssetPath(spriteFolder);
        
        // 加载所有精灵
        Sprite[] sprites = AssetDatabase.FindAssets("t:Sprite", new[] { folderPath })
            .Select(AssetDatabase.GUIDToAssetPath)
            .Select(AssetDatabase.LoadAssetAtPath<Sprite>)
            .OrderBy(s => s.name)  // 按名称排序
            .ToArray();

        if (sprites.Length == 0)
        {
            Debug.LogError("No sprites found in selected folder");
            return;
        }

        // 创建动画剪辑
        AnimationClip animationClip = new AnimationClip();
        animationClip.frameRate = frameRate;

        // 创建曲线绑定
        EditorCurveBinding spriteBinding = new EditorCurveBinding
        {
            type = typeof(SpriteRenderer),
            path = "",
            propertyName = "m_Sprite"
        };

        // 创建关键帧
        ObjectReferenceKeyframe[] keyframes = new ObjectReferenceKeyframe[sprites.Length];
        float frameTime = 1f / frameRate;
        
        for (int i = 0; i < sprites.Length; i++)
        {
            keyframes[i] = new ObjectReferenceKeyframe
            {
                time = i * frameTime,
                value = sprites[i]
            };
        }

        // 保存动画剪辑
        AnimationUtility.SetObjectReferenceCurve(animationClip, spriteBinding, keyframes);
        
        // 创建保存路径
        string savePath = Path.Combine("Assets", "Animations");
        if (!Directory.Exists(savePath)) Directory.CreateDirectory(savePath);
        
        string clipPath = Path.Combine(savePath, $"{animationName}.anim");
        AssetDatabase.CreateAsset(animationClip, clipPath);

        // 创建Animator Controller
        var controller = UnityEditor.Animations.AnimatorController.CreateAnimatorControllerAtPath(
            Path.Combine(savePath, $"{animationName}Controller.controller"));
        
        // 添加动画状态
        var state = controller.layers[0].stateMachine.AddState(animationName);
        state.motion = animationClip;

        AssetDatabase.SaveAssets();
        AssetDatabase.Refresh();

        Debug.Log($"Animation {animationName} generated successfully!");
    }
}
#endif


