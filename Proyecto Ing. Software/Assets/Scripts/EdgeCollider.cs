using UnityEngine;

public class EdgeCollider : MonoBehaviour
{
    public Invaders invadersGroup;
    public Vector2 direction; // (-1,0) para izquierda, (1,0) para derecha

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.GetComponent<Invader>())
        {
            invadersGroup.OnEdgeCollision(direction);
        }
    }
}