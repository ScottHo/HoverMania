using UnityEngine;

public class Camera : MonoBehaviour
{
    public Transform obstrution;
    public Transform target;
    Color lastColor;

    private void LateUpdate()
    {
        ViewObstructed();
    }
    void ViewObstructed()
    {
        RaycastHit hit;
        if (Physics.Raycast(transform.position, target.position - transform.position, out hit, 10f))
        {
            int terrainLayer = 6;
            if (hit.collider.gameObject.layer == terrainLayer)
            {
                if (obstrution != null && obstrution != hit.transform)
                {
                    // Obstruction is a continuous hit on terrain layer, revert old obstruction first
                    SetObstructionOpaque();
                    obstrution = hit.collider.transform;
                    SetObstructionTransparent();
                    return;
                }
                else if (obstrution == null)
                {
                    // Obstruction is a new hit on terrain layer
                    obstrution = hit.collider.transform;
                    SetObstructionTransparent();
                    return;
                }
                else if (obstrution == hit.transform)
                {
                    // Obstruction is same object, continue
                    return;
                } 
            }
        }
        SetObstructionOpaque();
    }
    void SetObstructionOpaque()
    {
        if (obstrution != null)
        {
            obstrution.gameObject.GetComponent<MeshRenderer>().material.color = lastColor;
            Utils.SetupMaterialWithBlendMode(obstrution.gameObject.GetComponent<MeshRenderer>().material, Utils.BlendMode.Opaque, true);
            obstrution = null;
        }
    }
    void SetObstructionTransparent()
    {
        if (obstrution != null)
        {
            // Store the previous color before modifying it's transparency
            Color color = obstrution.gameObject.GetComponent<MeshRenderer>().material.color;
            lastColor = color;
            color.a = .5f;
            obstrution.gameObject.GetComponent<MeshRenderer>().material.color = color;
            Utils.SetupMaterialWithBlendMode(obstrution.gameObject.GetComponent<MeshRenderer>().material, Utils.BlendMode.Transparent, true);
        }
    }
}
