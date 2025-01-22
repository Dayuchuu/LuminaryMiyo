using UnityEngine;
using System.Collections;

[RequireComponent(typeof(MeshFilter))]
[RequireComponent(typeof(MeshRenderer))]
public class MeshCombiner : MonoBehaviour
{
    [ContextMenu("Combine Meshes")]
    void CombineMeshes()
    {
        //Combines a meshes children to one. 
        CombineInstance[] combine = new CombineInstance[transform.childCount];

        int index = 0;
        foreach (Transform child in gameObject.transform)
        {
            MeshFilter filter = child.GetComponent<MeshFilter>();
            combine[index].mesh = filter.sharedMesh;
            combine[index].transform = child.localToWorldMatrix;
            child.gameObject.SetActive(false);
            index++;
        }
        

        Mesh mesh = new Mesh();
        mesh.CombineMeshes(combine);
        transform.GetComponent<MeshFilter>().sharedMesh = mesh;
        transform.gameObject.SetActive(true);
    }
}
