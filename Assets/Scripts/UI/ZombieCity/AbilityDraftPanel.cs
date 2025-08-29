using System.Collections.Generic;
using UnityEngine;

namespace ZombieCity.Abilities
{
    public class AbilityDraftPanel : MonoBehaviour
    {
        public AbilityCardUI cardPrefab;
        public Transform cardRoot;

        private PlayerAbilitySystem _system;
        
        private AbilitySO currentChoice;

        private void Awake() => _system = FindObjectOfType<PlayerAbilitySystem>(true);

        public void ShowDraft()
        {
            gameObject.SetActive(true);
            GameController.Instance.isPlaying = false;
            //Time.timeScale = 0f; // pause
            
            RollChoices();
        }

        public void RollChoices()
        {
            foreach (Transform c in cardRoot) Destroy(c.gameObject);
            
            List<AbilitySO> choices = _system.RollChoices();
            foreach (var a in choices)
            {
                var card = Instantiate(cardPrefab, cardRoot);
                currentChoice = a;
                card.Bind(a);
            }
        }

        public void OnPick()
        {
            _system.Pick(currentChoice);
            Close();
        }
        public void Close()
        {
            foreach (Transform c in cardRoot) Destroy(c.gameObject);
            gameObject.SetActive(false);
            //Time.timeScale = 1f;
            GameController.Instance.isPlaying = true;
            GameController.Instance.GetUIController().SetActiveTimeCounterPanel(true);
            GameController.Instance.GetUIController().SetActiveCoinBG(false);
        }
    }
}