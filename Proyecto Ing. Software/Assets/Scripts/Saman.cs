using UnityEngine;

public class Saman : MonoBehaviour
{
    private bool jugadorDentro = false;
    private Coroutine restarConocimientoCoroutine;

    private void OnTriggerEnter2D(Collider2D other)
    {
        if (other.CompareTag("Player"))
        {
            jugadorDentro = true;
            if (restarConocimientoCoroutine == null)
                restarConocimientoCoroutine = StartCoroutine(RestarConocimientoCadaMinuto());
        }
    }

    private System.Collections.IEnumerator RestarConocimientoCadaMinuto()
    {
        while (jugadorDentro)
        {
            PlayerStatsManager.Instance.AddConocimiento(-1);
            yield return new WaitForSeconds(30f);
        }
    }
}
