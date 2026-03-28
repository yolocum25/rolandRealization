using UnityEngine;

namespace DefaultNamespace
{
    public class PlayerData
    {
        private Vector3 savedPosition;
        private Vector3 savedOrientation;
        private int score;
        private int health;
        private int emotion;

        public PlayerData(Vector3 savedPosition, Vector3 savedOrientation, int score, int health, int emotion)
        {
            this.savedPosition = savedPosition;
            this.savedOrientation = savedOrientation;
            this.score = score;
            this.health = health;
            this.emotion = emotion;
        }
    }
}