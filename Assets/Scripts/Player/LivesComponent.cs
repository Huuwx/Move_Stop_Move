using UnityEngine;

namespace ZombieCity.Abilities
{
    public class LivesComponent : MonoBehaviour
    {
        [SerializeField] int lives = 1;
        public void AddLife(int v) => lives += v;
        public int GetLives() => lives;

        public void Kill()
        {
            lives--;
            if (lives > 0)
            {
                // revive
                PlayerController.RaiseOnPlayerRevived();
                // hồi HP, v.v.
            }
            else
            {
                PlayerController.RaiseOnPlayerDeath();
                // Game over
            }
        }
    }
}