using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MaterialsAlpha : MonoBehaviour
{

    MeshRenderer meshRenderer;
    public Material[] materials;
    Shader shader1;
    



    void Start()
    {
        meshRenderer = GetComponent<MeshRenderer>();
        materials = meshRenderer.materials;
  /*      
        foreach (Material mat in materials)
        {           
            mat.SetOverrideTag("RenderType", "Transparent");
        }*/
        foreach (Material mat in materials)
        {
            Color tempColor = mat.color;
            tempColor.a = 0.5f;
            mat.color = tempColor;
        }
    }


    void Update()
    {

    }
}
