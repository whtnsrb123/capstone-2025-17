using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer))]
public class CombineMeshes : MonoBehaviour
{
    void Start()
    {
        CombineAllMeshes();
    }

    void CombineAllMeshes()
    {
        MeshFilter[] meshFilters = GetComponentsInChildren<MeshFilter>();
        CombineInstance[] combine = new CombineInstance[meshFilters.Length];

        int i = 0;
        foreach (MeshFilter mf in meshFilters)
        {
            if (mf == GetComponent<MeshFilter>()) continue; // 자기 자신은 제외

            combine[i].mesh = mf.sharedMesh;
            combine[i].transform = mf.transform.localToWorldMatrix;
            i++;

            mf.gameObject.SetActive(false); // 기존 오브젝트는 비활성화
        }

        Mesh combinedMesh = new Mesh();
        combinedMesh.CombineMeshes(combine);

        MeshFilter meshFilter = GetComponent<MeshFilter>();
        meshFilter.mesh = combinedMesh;
        GetComponent<MeshRenderer>().enabled = true;
    }
}
