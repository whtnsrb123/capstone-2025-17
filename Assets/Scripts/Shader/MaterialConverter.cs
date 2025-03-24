using UnityEngine;
using UnityEditor;

public class MaterialConverter : EditorWindow
{
    private Material newMaterial;
    private int newLayer;

    [MenuItem("Tools/Change Materials (Keep BaseMap)")]
    private static void ShowWindow()
    {
        GetWindow<MaterialConverter>("Change Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("변경할 오브젝트", EditorStyles.boldLabel);
        newMaterial = (Material)EditorGUILayout.ObjectField("New Material", newMaterial, typeof(Material), false);

        GUILayout.Label("변경할 Layer", EditorStyles.boldLabel);
        newLayer = EditorGUILayout.LayerField("New Layer", newLayer);

        // 선택한 오브젝트만 변경
        GUILayout.Label("선택된 오브젝트의 모든 머테리얼 변경 (BaseMap 유지)", EditorStyles.boldLabel);
        if (GUILayout.Button("Change Materials") && newMaterial != null)
        {
            ChangeSelectedMaterials();
        }

        // 모든 Material이 적용된 오브젝트 변경 
        GUILayout.Label("배치된 모든 오브젝트의 모든 머테리얼 변경 (BaseMap 유지)", EditorStyles.boldLabel);
        if (GUILayout.Button("Change All Materials") && newMaterial != null)
        {
            ChangeAllMaterials();
        }
    }

    private void ChangeSelectedMaterials()
    {
        foreach (GameObject obj in Selection.gameObjects)
        {
            Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] oldMaterials = renderer.sharedMaterials; // 모든 머테리얼 가져오기
                Material[] newMaterials = new Material[oldMaterials.Length]; // 변경할 머테리얼 배열 생성

                for (int i = 0; i < oldMaterials.Length; i++)
                {
                    Material oldMaterial = oldMaterials[i];
                    if (oldMaterial != null)
                    {
                        // 기존 BaseMap 텍스처 가져오기
                        Texture baseMapTexture = oldMaterial.GetTexture("_BaseMap");

                        Undo.RecordObject(renderer, "Change Material");
                        Material newMatInstance = new Material(newMaterial);

                        // 기존 BaseMap 텍스처가 있으면 유지
                        if (baseMapTexture != null)
                        {
                            newMatInstance.SetTexture("_BaseMap", baseMapTexture);
                        }

                        newMaterials[i] = newMatInstance;
                    }
                }

                renderer.sharedMaterials = newMaterials; // 모든 머테리얼 교체
                EditorUtility.SetDirty(renderer);
            }

            obj.layer = newLayer;
        }

        Debug.Log("선택된 오브젝트의 모든 머테리얼을 변경하였습니다.");
    }

    private void ChangeAllMaterials()
    {
        foreach (Renderer renderer in FindObjectsOfType<Renderer>())
        {
            //Renderer renderer = obj.GetComponent<Renderer>();
            if (renderer != null)
            {
                Material[] oldMaterials = renderer.sharedMaterials; // 모든 머테리얼 가져오기
                Material[] newMaterials = new Material[oldMaterials.Length]; // 변경할 머테리얼 배열 생성

                for (int i = 0; i < oldMaterials.Length; i++)
                {
                    Material oldMaterial = oldMaterials[i];
                    if (oldMaterial != null)
                    {
                        // 기존 BaseMap 텍스처 가져오기
                        Texture baseMapTexture = oldMaterial.GetTexture("_BaseMap");

                        Undo.RecordObject(renderer, "Change Material");
                        Material newMatInstance = new Material(newMaterial);

                        // 기존 BaseMap 텍스처가 있으면 유지
                        if (baseMapTexture != null)
                        {
                            newMatInstance.SetTexture("_BaseMap", baseMapTexture);
                        }

                        newMaterials[i] = newMatInstance;
                    }
                }

                renderer.sharedMaterials = newMaterials; // 모든 머테리얼 교체
                EditorUtility.SetDirty(renderer);
            }
            renderer.gameObject.layer = newLayer;
        }

        Debug.Log("선택된 오브젝트의 모든 머테리얼을 변경하였습니다.");
    }
}