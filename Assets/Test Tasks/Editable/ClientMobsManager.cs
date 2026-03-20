using TestTask.NonEditable;
using TMPro;
using UnityEngine;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI monsterNameField;

        public void Awake()
        {
            
        }
        
        public void UpdateMonsterName(string monsterName)
        {
            if (monsterName != monsterNameField.text)
            {
                monsterNameField.text = monsterName;
            }
        }
    }
}
