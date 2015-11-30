using UnityEngine;

public class Rope : MonoBehaviour
{
    public LineRenderer LineRenderer;
    public RopeEnd RopeEnd;

    public void SetPosition(Vector3 start, Vector3 end)
    {
        LineRenderer.SetPosition(0, start);
        LineRenderer.SetPosition(1, end);
    }

    public void UpdateColor(Color newColor)
    {
        Color predictedColor = PlayerCharacter.Instance.PredicateColor(newColor);
        LineRenderer.material.color = new Color(predictedColor.r, 
            predictedColor.g, predictedColor.b, 0.5f);
        RopeEnd.MeshRenderer.material.color = new Color(predictedColor.r,
            predictedColor.g, predictedColor.b, 0.5f);
    }

    public void ResetColor()
    {
        RopeEnd.MeshRenderer.material.color = new Color(
            PlayerCharacter.Instance.TargetedPoolColor.r,
            PlayerCharacter.Instance.TargetedPoolColor.g,
            PlayerCharacter.Instance.TargetedPoolColor.b,
            0.5f);
        LineRenderer.material.color = new Color(
            PlayerCharacter.Instance.TargetedPoolColor.r,
            PlayerCharacter.Instance.TargetedPoolColor.g,
            PlayerCharacter.Instance.TargetedPoolColor.b,
            0.5f);
    }
}
