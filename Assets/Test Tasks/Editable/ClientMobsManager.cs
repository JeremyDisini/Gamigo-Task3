using System;
using TestTask.NonEditable;
using TMPro;
using UnityEngine;
using UnityEngine.UI;
using Random = System.Random;

namespace TestTask.Editable
{
    public class ClientMobsManager : MonoBehaviour
    {
        [SerializeField] private TextMeshProUGUI monsterNameField;
        [SerializeField] private Slider monsterHealthbar;
        [SerializeField] private Image monsterPortrait;
        [SerializeField] private Sprite[] monsterPortraitSprites;
        private int monsterId = -1;
        
        //whenever a monster is updated
        public void UpdateMonster(int id, MonsterNames type, string name, float maxHealth, float currentHealth)
        {
            monsterId = id;
            monsterNameField.text = name;
            monsterPortrait.sprite = monsterPortraitSprites[(int)type];
            monsterPortrait.color = Color.white;
            monsterHealthbar.value = currentHealth / maxHealth;
        }

        public void DamageMonster(int id, float healthPercent)
        {
            //error checking in case we happen to receive a packet from a different id (e.g. health update from a monster we already killed)
            if (id != monsterId)
            {
                Debug.LogWarning("Error: Received health update for a different monster ID#" + id + ". (expected monster ID# " + monsterId + ")");
                return;
            }
            else if (healthPercent > monsterHealthbar.value)
            {
                Debug.LogError("Error: Received Damage packets out of order.");
                return;
            }
            monsterHealthbar.value = healthPercent;
        }

        public void DamageMonster()
        {
            ClientPacketsHandler.SendDamageRequest(monsterId,50);
        }
    }
}
