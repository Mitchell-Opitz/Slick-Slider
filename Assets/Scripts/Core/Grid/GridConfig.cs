using UnityEngine;

[CreateAssetMenu(fileName = "GridConfig", menuName = "SlickSlider/Grid Config")]
public class GridConfig : ScriptableObject
{
    [Min(0.1f)] public float cellSize = 1f;
    public Vector2 worldOffset = Vector2.zero;
}
