using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Tilemaps;

namespace BitManSatChase
{
    /// <summary>
    /// Fills every open (non-wall) cell of the maze with a sat, except for a small
    /// exclusion zone around the altcoin pen and 4 corner-ish cells that get a Halving
    /// Boost instead. Reads the actual walls Tilemap instead of hardcoded coordinates,
    /// so it stays correct if the maze layout changes.
    /// </summary>
    public class MazePelletSpawner : MonoBehaviour
    {
        [SerializeField] private Tilemap wallTilemap;
        [SerializeField] private GameObject pelletPrefab;
        [SerializeField] private GameObject powerPelletPrefab;
        [SerializeField] private Vector2 penExclusionCenter = new Vector2(-4.9f, 1.6f);
        [SerializeField] private float penExclusionRadius = 3f;

        void Start()
        {
            if (wallTilemap == null || pelletPrefab == null) return;

            List<Vector3Int> openCells = new List<Vector3Int>();
            BoundsInt bounds = wallTilemap.cellBounds;

            foreach (Vector3Int cell in bounds.allPositionsWithin)
            {
                if (wallTilemap.HasTile(cell)) continue;

                Vector3 worldPosition = wallTilemap.GetCellCenterWorld(cell);
                if (Vector2.Distance(worldPosition, penExclusionCenter) < penExclusionRadius) continue;

                openCells.Add(cell);
            }

            HashSet<Vector3Int> powerPelletCells = PickCornerCells(openCells);

            foreach (Vector3Int cell in openCells)
            {
                GameObject prefab = powerPelletCells.Contains(cell) && powerPelletPrefab != null ? powerPelletPrefab : pelletPrefab;
                Instantiate(prefab, wallTilemap.GetCellCenterWorld(cell), Quaternion.identity, transform);
            }
        }

        /// <summary>
        /// Picks the 4 cells farthest towards each diagonal extreme of the open area, as a
        /// stand-in for the classic 4-corner power pellet placement.
        /// </summary>
        private HashSet<Vector3Int> PickCornerCells(List<Vector3Int> openCells)
        {
            HashSet<Vector3Int> corners = new HashSet<Vector3Int>();
            if (openCells.Count == 0) return corners;

            Vector3Int maxSum = openCells[0], minSum = openCells[0], maxDiff = openCells[0], minDiff = openCells[0];

            foreach (Vector3Int cell in openCells)
            {
                int sum = cell.x + cell.y;
                int diff = cell.x - cell.y;
                if (sum > maxSum.x + maxSum.y) maxSum = cell;
                if (sum < minSum.x + minSum.y) minSum = cell;
                if (diff > maxDiff.x - maxDiff.y) maxDiff = cell;
                if (diff < minDiff.x - minDiff.y) minDiff = cell;
            }

            corners.Add(maxSum);
            corners.Add(minSum);
            corners.Add(maxDiff);
            corners.Add(minDiff);
            return corners;
        }
    }
}
