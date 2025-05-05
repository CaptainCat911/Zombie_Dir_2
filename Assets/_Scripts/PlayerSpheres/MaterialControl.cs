using UnityEngine;
[AddComponentMenu("Rendering/Material Control")]
public class MaterialControl : MonoBehaviour
{
    private MeshRenderer meshRenderer;
    //public Material transparent;
    private Material[] materials;
    private Material[] materialsNew;
    //private Color color = Color.black;

    void OnValidate()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.sharedMaterials;
    }

    public void SetMask(bool value)
    {
/*        if (value)
        {
            foreach (Material mat in materials)
            {
                //mat.color = Color.yellow;           
            }                   
        }
        else
        {
            //meshRenderer.materials = materials;
        }*/
    }
}