using UnityEngine;

public class Invaders : MonoBehaviour
{
    public Invader[] prefabs;
    public int rows = 5;
    public int columns = 11;
    public AnimationCurve speed = AnimationCurve.Linear(0, 1, 1, 1);

    public int amountKilled { get; private set; }
    public int totalInvaders => this.rows * this.columns;
    public float percentKilled => (float)this.amountKilled / (float)this.totalInvaders;

    private Vector3 _direction = Vector2.right;

    // Agrega referencia al jugador
    public Player player;

    private void Awake()
    {
        for (int row = 0; row < this.rows; row++)
        {
            float width = 2.0f * (this.columns - 1);
            float height = 2.0f * (this.rows - 1);
            Vector2 centering = new Vector2(-width / 2, -height / 2);
            Vector3 rowPosition = new Vector3(centering.x, centering.y + (row * 2.0f), 0.0f);
            for (int col = 0; col < this.columns; col++)
            {
                Invader invader = Instantiate(this.prefabs[row], this.transform);
                invader.killed += InvaderKilled;
                invader.invadersGroup = this;
                Vector3 position = rowPosition;
                position.x += col * 2.0f;
                invader.transform.localPosition = position;
            }
        }
    }

    private void Update()
    {
        this.transform.position += _direction * 2.0f * Time.deltaTime;
    }

    public void OnEdgeCollision(Vector2 edgeDirection)
    {
        if ((_direction.x > 0 && edgeDirection.x > 0) || (_direction.x < 0 && edgeDirection.x < 0))
        {
            AdvanceRow();
        }
    }

    private void AdvanceRow()
    {
        _direction.x *= -1.0f;

        Vector3 position = this.transform.position;
        position.y -= 1.0f;
        this.transform.position = position;
    }

    private void InvaderKilled()
    {
        amountKilled++;
        ScoreManager.Instance.AddPoint();

        if (amountKilled >= totalInvaders)
        {
            // Llama al método de victoria del SpaceManager
            SpaceManager sm = FindObjectOfType<SpaceManager>();
            if (sm != null)
            {
                sm.WinGame();
            }
        }
    }
}
