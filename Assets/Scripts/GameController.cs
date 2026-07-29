using UnityEngine;

namespace BitManSatChase
{
    public class GameController : MonoBehaviour
    {
        [SerializeField] private SpawnController spawnController; // Control spawning of all game objects
        void Start()
        {
            spawnController.SpawnPlayer();
            spawnController.SpawnAltcoin(SpawnController.AltcoinType.ETH);
            spawnController.SpawnAltcoin(SpawnController.AltcoinType.ADA);
            spawnController.SpawnAltcoin(SpawnController.AltcoinType.SOL);
            spawnController.SpawnAltcoin(SpawnController.AltcoinType.DOGE);
        }

    }
}
