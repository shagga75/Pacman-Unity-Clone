using UnityEngine;

namespace BitManSatChase
{
    public class SpawnController : MonoBehaviour
    {
        [SerializeField] private GameObject playerPrefab;
        [SerializeField] private GameObject altcoinPrefab;
        [SerializeField] private Transform playerSpawnPoint;
        [SerializeField] private Transform[] altcoinSpawnPoints; // Array of altcoin spawn points: ETH - 0, ADA - 1, SOL - 2, DOGE - 3
        [SerializeField] private Sprite[] altcoinSprites; // Indexed the same as altcoinSpawnPoints: ETH - 0, ADA - 1, SOL - 2, DOGE - 3
        public enum AltcoinType { ETH, ADA, SOL, DOGE }

        /// <summary>
        /// Spawns the player at the specified spawn point.
        /// </summary>
        public void SpawnPlayer()
        {
            if (playerPrefab != null && playerSpawnPoint != null)
            {
                GameObject player = Instantiate(playerPrefab, playerSpawnPoint.position, playerSpawnPoint.rotation);
                player.name = "BitMan";
            }
        }

        /// <summary>
        /// Spawns an altcoin of the specified type at the corresponding spawn point.
        /// </summary>
        /// <param name="altcoinType">The type of altcoin to spawn.</param>
        public void SpawnAltcoin(AltcoinType altcoinType)
        {
            if (altcoinPrefab != null && altcoinSpawnPoints != null && altcoinSpawnPoints.Length > (int)altcoinType)
            {
                GameObject altcoin = Instantiate(altcoinPrefab, altcoinSpawnPoints[(int)altcoinType].position, altcoinSpawnPoints[(int)altcoinType].rotation);
                altcoin.name = altcoinType.ToString();

                if (altcoinSprites != null && altcoinSprites.Length > (int)altcoinType)
                {
                    SpriteRenderer spriteRenderer = altcoin.GetComponent<SpriteRenderer>();
                    if (spriteRenderer != null) spriteRenderer.sprite = altcoinSprites[(int)altcoinType];
                }

                AltcoinMovement movement = altcoin.GetComponent<AltcoinMovement>();
                if (movement != null) movement.SetPattern(PatternFor(altcoinType));
            }
        }

        /// <summary>
        /// ETH chases BitMan directly, ADA ambushes ahead of him, SOL wanders randomly and DOGE flees from him.
        /// </summary>
        private static AltcoinMovement.AIPattern PatternFor(AltcoinType altcoinType)
        {
            switch (altcoinType)
            {
                case AltcoinType.ETH: return AltcoinMovement.AIPattern.Chase;
                case AltcoinType.ADA: return AltcoinMovement.AIPattern.Ambush;
                case AltcoinType.DOGE: return AltcoinMovement.AIPattern.Flee;
                case AltcoinType.SOL:
                default: return AltcoinMovement.AIPattern.Random;
            }
        }
    }
}
