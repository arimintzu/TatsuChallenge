// DisableInInlineEditorExampleComponent.cs
using Sirenix.OdinInspector;
using UnityEngine;

#if UNITY_EDITOR // Editor namespaces can only be used in the editor.
using Sirenix.OdinInspector.Editor.Examples;
#endif

public class DisableInInlineEditorExampleComponent : MonoBehaviour
{
#if UNITY_EDITOR // DisabledInInlineEditorScriptableObject is an example type and only exists in the editor
    [InfoBox("Click the pen icon to open a new inspector window for the InlineObject too see the difference this attribute makes.")]
    [InlineEditor(Expanded = true)]
    public DisabledInInlineEditorScriptableObject InlineObject;
#endif 
    
#if UNITY_EDITOR // Editor-related code must be excluded from builds
    [OnInspectorInit]
    private void CreateData()
    {
        InlineObject = ExampleHelper.GetScriptableObject<DisabledInInlineEditorScriptableObject>("Inline Object");
    }
    
    [OnInspectorDispose]
    private void CleanupData()
    {
        if (InlineObject != null) Object.DestroyImmediate(InlineObject);
    }
#endif 
}