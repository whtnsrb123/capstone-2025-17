#if UNITY_EDITOR
using UnityEngine;
using UnityEditor;
using System.IO;

public class MeshSaver
{
    [MenuItem("Tools/Save Combined Mesh")]
    public static void SaveSelectedMesh()
    {
        GameObject selected = Selection.activeGameObject;

        if (selected == null)
        {
            Debug.LogWarning("아무 오브젝트도 선택되지 않았어요.");
            return;
        }

        MeshFilter mf = selected.GetComponent<MeshFilter>();
        if (mf == null || mf.sharedMesh == null)
        {
            Debug.LogWarning("MeshFilter 또는 메시가 없어요.");
            return;
        }

        string path = "Assets/SavedMeshes";
        if (!Directory.Exists(path))
            Directory.CreateDirectory(path);

        string assetPath = $"{path}/{selected.name}_CombinedMesh.asset";

        AssetDatabase.CreateAsset(Object.Instantiate(mf.sharedMesh), assetPath);
        AssetDatabase.SaveAssets();

        Debug.Log("메시 저장 완료: " + assetPath);
    }
}
#endif
