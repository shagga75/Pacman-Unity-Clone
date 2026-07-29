using System.Collections.Generic;
using UnityEngine;

namespace BitManSatChase
{
    /// <summary>
    /// Movement, AI pattern and Halving Boost vulnerability for one altcoin. All 4 altcoins
    /// share this component and the same prefab; SpawnController assigns each instance's
    /// pattern via SetPattern() right after spawning it, based on its AltcoinType. BitMan is
    /// found through the "Player" tag, so no manual wiring is needed per pattern.
    /// </summary>
    public class AltcoinMovement : MonoBehaviour
    {
        public enum AIPattern { Chase, Ambush, Random, Flee }

        [SerializeField] private AIPattern aiPattern = AIPattern.Random;
        [SerializeField] private float moveSpeed = 4f;
        [SerializeField] private float turnDetectionDistance = 1f;
        [SerializeField] private float decisionInterval = 0.2f;
        [SerializeField] private float ambushLookAhead = 3f;
        [SerializeField] private float fleeTargetDistance = 10f;
        [SerializeField] private int liquidateValue = 200;
        [SerializeField] private Color vulnerableColor = new Color(0.35f, 0.55f, 1f, 1f);

        private static readonly Vector2[] Directions = { Vector2.up, Vector2.down, Vector2.left, Vector2.right };

        private Rigidbody2D rb;
        private BoxCollider2D boxCollider;
        private SpriteRenderer spriteRenderer;
        private Transform player;
        private BitManMovement playerMovement;
        private Vector2 currentDirection = Vector2.up;
        private Vector2 spawnPosition;
        private float nextDecisionTime;
        private bool isVulnerable;
        private float vulnerableUntil;

        private AIPattern EffectivePattern => isVulnerable ? AIPattern.Flee : aiPattern;

        void Start()
        {
            rb = GetComponent<Rigidbody2D>();
            boxCollider = GetComponent<BoxCollider2D>();
            spriteRenderer = GetComponent<SpriteRenderer>();
            spawnPosition = transform.position;

            GameObject playerObject = GameObject.FindGameObjectWithTag("Player");
            if (playerObject != null)
            {
                player = playerObject.transform;
                playerMovement = playerObject.GetComponent<BitManMovement>();
            }

            ChooseNewDirection();
        }

        void FixedUpdate()
        {
            if (isVulnerable && Time.time >= vulnerableUntil)
            {
                isVulnerable = false;
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
            }

            if (!CanMove(currentDirection) || Time.time >= nextDecisionTime)
            {
                ChooseNewDirection();
                nextDecisionTime = Time.time + decisionInterval;
            }
            rb.linearVelocity = currentDirection * moveSpeed;
        }

        void OnCollisionEnter2D(Collision2D collision)
        {
            if (collision.collider.GetComponentInParent<BitManMovement>() == null) return;

            if (isVulnerable)
            {
                if (ScoreController.Instance != null) ScoreController.Instance.AddScore(liquidateValue);
                isVulnerable = false;
                if (spriteRenderer != null) spriteRenderer.color = Color.white;
                transform.position = spawnPosition;
            }
            // TODO (step 6): a non-vulnerable altcoin catching BitMan should cost a life / end the run.
        }

        /// <summary>
        /// Sets which of the 4 altcoin AI patterns this instance follows.
        /// </summary>
        public void SetPattern(AIPattern pattern)
        {
            aiPattern = pattern;
        }

        /// <summary>
        /// Makes this altcoin flee and become liquidatable for the given duration.
        /// </summary>
        public void BecomeVulnerable(float duration)
        {
            isVulnerable = true;
            vulnerableUntil = Time.time + duration;
            if (spriteRenderer != null) spriteRenderer.color = vulnerableColor;
        }

        /// <summary>
        /// Called by a PowerPellet (Halving Boost) to make every altcoin in the scene vulnerable at once.
        /// </summary>
        public static void TriggerHalvingBoostOnAll(float duration)
        {
            foreach (AltcoinMovement altcoin in Object.FindObjectsByType<AltcoinMovement>(FindObjectsSortMode.None))
            {
                altcoin.BecomeVulnerable(duration);
            }
        }

        /// <summary>
        /// Picks a new direction: uniformly random for SOL, or whichever open direction gets
        /// closest to this pattern's target for the other 3 (a U-turn is only allowed when it's
        /// the only open direction, matching classic ghost behaviour). While vulnerable, every
        /// altcoin temporarily flees regardless of its normal pattern.
        /// </summary>
        private void ChooseNewDirection()
        {
            Vector2 reverse = -currentDirection;
            List<Vector2> openDirections = new List<Vector2>();

            foreach (Vector2 direction in Directions)
            {
                if (direction == reverse) continue;
                if (CanMove(direction)) openDirections.Add(direction);
            }

            if (openDirections.Count == 0 && CanMove(reverse))
            {
                openDirections.Add(reverse);
            }

            if (openDirections.Count == 0) return;

            currentDirection = EffectivePattern == AIPattern.Random || player == null
                ? openDirections[Random.Range(0, openDirections.Count)]
                : ClosestDirectionTo(openDirections, GetTargetPosition());
        }

        /// <summary>
        /// The point this altcoin steers towards this decision, based on its effective pattern.
        /// Flee reuses the "closest to target" logic by targeting a point far away from
        /// BitMan, in the opposite direction, instead of maximizing distance directly.
        /// </summary>
        private Vector2 GetTargetPosition()
        {
            Vector2 playerPosition = player.position;

            switch (EffectivePattern)
            {
                case AIPattern.Ambush:
                    Vector2 playerFacing = playerMovement != null ? playerMovement.FacingDirection : Vector2.zero;
                    return playerPosition + playerFacing * ambushLookAhead;
                case AIPattern.Flee:
                    Vector2 awayFromPlayer = ((Vector2)transform.position - playerPosition).normalized;
                    return (Vector2)transform.position + awayFromPlayer * fleeTargetDistance;
                case AIPattern.Chase:
                default:
                    return playerPosition;
            }
        }

        private Vector2 ClosestDirectionTo(List<Vector2> openDirections, Vector2 target)
        {
            Vector2 best = openDirections[0];
            float bestDistance = float.MaxValue;

            foreach (Vector2 direction in openDirections)
            {
                Vector2 nextPosition = (Vector2)transform.position + direction * turnDetectionDistance;
                float distance = Vector2.Distance(nextPosition, target);
                if (distance < bestDistance)
                {
                    bestDistance = distance;
                    best = direction;
                }
            }

            return best;
        }

        private bool CanMove(Vector2 direction)
        {
            RaycastHit2D hit = Physics2D.BoxCast(transform.position, boxCollider.size * 0.9f, 0, direction, turnDetectionDistance, LayerMask.GetMask("Obstacles"));
            return hit.collider == null;
        }
    }
}
