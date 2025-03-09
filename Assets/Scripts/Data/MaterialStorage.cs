using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[CreateAssetMenu(fileName = "MaterialStorage", menuName = "Storage/MaterialStorage")]
public class MaterialStorage : ScriptableObject
{
    const int materialCount = 12;
    public Material SDefaultMaterial;
    public Material[] SMaterials = new Material[materialCount]; 

    public Material GetMesh(int index)
    {
        if (SMaterials[index] != null)
            return SMaterials[index];
        else
            return SDefaultMaterial;
    }
}
