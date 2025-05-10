/*using UnityEngine;
using UnityEditor;
using System.Linq;

public class MaterialConverter : EditorWindow
{
    private Material newMaterial;
    private int newLayer = 6;

    [MenuItem("Tools/Change Materials (Keep BaseMap)")]
    private static void ShowWindow()
    {
        GetWindow<MaterialConverter>("Change Materials");
    }

    private void OnGUI()
    {
        GUILayout.Label("������ ������Ʈ", EditorStyles.boldLabel);
        newMaterial = (Material)EditorGUILayout.ObjectField("New Material", newMaterial, typeof(Material), false);

        GUILayout.Label("������ Layer", EditorStyles.boldLabel);
        newLayer = EditorGUILayout.LayerField("New Layer", newLayer);

        // ������ ������Ʈ�� ����
        GUILayout.Label("���õ� ������Ʈ�� ��� ���׸��� ���� (BaseMap ����)", EditorStyles.boldLabel);
        if (GUILayout.Button("Change Materials") && newMaterial != null)
        {
            ChangeSelectedMaterials();
        }

        // ��� Material�� ����� ������Ʈ ���� 
        GUILayout.Label("��ġ�� ��� ������Ʈ�� ��� ���׸��� ���� (BaseMap ����)", EditorStyles.boldLabel);
        if (GUILayout.Button("Change All Materials") && newMaterial != null)
        {
            ChangeAllMaterials();
        }
    }

    private void ChangeSelectedMaterials()
    {
        Renderer[] renderers;
        renderers = Selection.gameObjects
           .Select(go => go.GetComponent<Renderer>())
           .Where(r => r != null) // Renderer�� ���� GameObject�� ����
           .ToArray();
        ChangeMaterials(renderers);

        Debug.Log("���õ� ������Ʈ�� ��� ���׸����� �����Ͽ����ϴ�.");
    }

    private void ChangeAllMaterials()
    {
        ChangeMaterials(FindObjectsOfType<Renderer>());

        Debug.Log("��� ������Ʈ�� ��� ���׸����� �����Ͽ����ϴ�.");
    }

    private void ChangeMaterials(Renderer[] renderers)
    {
        foreach (Renderer renderer in renderers)
        {
            if (renderer != null)
            {
                Material[] oldMaterials = renderer.sharedMaterials; // ��� ���׸��� ��������
                Material[] newMaterials = new Material[oldMaterials.Length]; // ������ ���׸��� �迭 ����

                for (int i = 0; i < oldMaterials.Length; i++)
                {
                    Material oldMaterial = oldMaterials[i];
                    if (oldMaterial != null)
                    {
                        // ���� BaseMap �ؽ�ó ��������
                        Texture baseMapTexture = oldMaterial.GetTexture("_BaseMap");
                        Color color = oldMaterial.GetColor("_BaseColor");

                        Undo.RecordObject(renderer, "Change Material");
                        Material newMatInstance = new Material(newMaterial);

                        // ���� BaseMap �ؽ�ó�� ������ ����
                        if (baseMapTexture != null)
                        {
                            newMatInstance.SetTexture("_BaseMap", baseMapTexture);
                        }
                        newMatInstance.SetColor("_BaseColor", color);

                        newMaterials[i] = newMatInstance;
                    }
                }

                renderer.sharedMaterials = newMaterials; // ��� ���׸��� ��ü
                EditorUtility.SetDirty(renderer);
            }
            renderer.gameObject.layer = newLayer;
        }
    }
}*/