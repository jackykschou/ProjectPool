using UnityEngine;

public class RopeEnd : MonoBehaviour
{
    public MeshRenderer MeshRenderer;
    public Innocent Innocent;

    void Start()
    {
        MeshRenderer = GetComponent<MeshRenderer>();
    }

    void OnTriggerEnter(Collider col)
    {
        if (Innocent != null)
        {
            return;
        }
        Innocent = col.gameObject.GetComponent<Innocent>();
        if (Innocent != null && !Innocent.Linked)
        {
            PlayerCharacter.Instance.Rope.UpdateColor(Innocent.MeshRenderer.material.color);
        }
        else
        {
            Innocent = null;
        }
    }

    void OnTriggerExit(Collider col)
    {
        if (Innocent == null)
        {
            return;
        }
        var innocent = col.gameObject.GetComponent<Innocent>();
        if (Innocent != null &&
            Innocent == innocent)
        {
            PlayerCharacter.Instance.Rope.ResetColor();
            Innocent = null;
        }
    }
}
