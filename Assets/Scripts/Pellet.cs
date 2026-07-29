using UnityEngine;

namespace BitManSatChase
{
    public class Pellet : MonoBehaviour
    {
        [SerializeField] private int value = 10;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<BitManMovement>() == null) return;

            if (ScoreController.Instance != null) ScoreController.Instance.AddScore(value);
            Destroy(gameObject);
        }
    }
}
