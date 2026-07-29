using UnityEngine;

namespace BitManSatChase
{
    /// <summary>
    /// The "Halving Boost": makes every altcoin vulnerable for a while so BitMan can liquidate them.
    /// </summary>
    public class PowerPellet : MonoBehaviour
    {
        [SerializeField] private int value = 50;
        [SerializeField] private float boostDuration = 8f;

        void OnTriggerEnter2D(Collider2D other)
        {
            if (other.GetComponentInParent<BitManMovement>() == null) return;

            if (ScoreController.Instance != null) ScoreController.Instance.AddScore(value);
            AltcoinMovement.TriggerHalvingBoostOnAll(boostDuration);
            Destroy(gameObject);
        }
    }
}
