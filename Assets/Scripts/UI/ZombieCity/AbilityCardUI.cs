using UnityEngine;
using UnityEngine.UI;
using TMPro;

namespace ZombieCity.Abilities
{
    public class AbilityCardUI : MonoBehaviour
    {
        public Image icon;
        public TMP_Text title;
        public Button pickButton;

        private AbilitySO _ability;
        //private System.Action<AbilitySO> _onPick;
        
        public void Bind(AbilitySO ability)
        {
            _ability = ability;
            icon.sprite = ability.icon;
            title.text  = ability.displayName;
        }
        
        // public void Bind(AbilitySO ability, System.Action<AbilitySO> onPick)
        // {
        //     _ability = ability; _onPick = onPick;
        //     icon.sprite = ability.icon;
        //     title.text  = ability.displayName;
        //     //desc.text   = ability.description;
        //     pickButton.onClick.RemoveAllListeners();
        //     pickButton.onClick.AddListener(() => _onPick?.Invoke(_ability));
        // }
    }
}