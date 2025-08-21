using System.Collections.Generic;
using UnityEngine;

namespace ZombieCity.Abilities
{
    public class AbilityDraftPanel : MonoBehaviour
    {
        public AbilityCardUI cardPrefab;
        public Transform cardRoot;

        private PlayerAbilitySystem _system;

        private void Awake() => _system = FindObjectOfType<PlayerAbilitySystem>(true);

        public void ShowDraft()
        {
            gameObject.SetActive(true);
            Time.timeScale = 0f; // pause
            foreach (Transform c in cardRoot) Destroy(c.gameObject);

            List<AbilitySO> choices = _system.RollChoices();
            foreach (var a in choices)
            {
                var card = Instantiate(cardPrefab, cardRoot);
                card.Bind(a, OnPick);
            }
        }

        private void OnPick(AbilitySO so)
        {
            _system.Pick(so);
            Close();
        }

        public void Close()
        {
            foreach (Transform c in cardRoot) Destroy(c.gameObject);
            gameObject.SetActive(false);
            Time.timeScale = 1f;
        }
    }
}