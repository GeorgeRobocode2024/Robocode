using UnityEngine;

[CreateAssetMenu(menuName = "Bullet/Add bullet type")]
public class BulletInfo : ScriptableObject
{
    public GameObject render;
    public int damage;
    public float speed;
}
