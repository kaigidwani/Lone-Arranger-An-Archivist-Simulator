using UnityEngine;

[CreateAssetMenu(fileName = "New Item", menuName = "Scriptable Objects/Item")]
public class ItemInfo : ScriptableObject
{
    [SerializeField] public string _name;
    [SerializeField] public string _description;
    [SerializeField] public Vector2 _dimensions;
    [SerializeField] public Texture2D _texture;
}
