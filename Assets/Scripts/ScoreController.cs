using UnityEngine;

namespace BitManSatChase
{
    /// <summary>
    /// Tracks accumulated sats and shows them with a minimal OnGUI label. This is a
    /// placeholder HUD: swap it for a proper Canvas/TMP display once in the Editor.
    /// </summary>
    public class ScoreController : MonoBehaviour
    {
        public static ScoreController Instance { get; private set; }

        private int sats;

        void Awake()
        {
            Instance = this;
        }

        public void AddScore(int amount)
        {
            sats += amount;
        }

        void OnGUI()
        {
            GUI.skin.label.fontSize = 24;
            GUI.color = Color.white;
            GUI.Label(new Rect(16, 12, 300, 40), $"Sats: {sats}");
        }
    }
}
