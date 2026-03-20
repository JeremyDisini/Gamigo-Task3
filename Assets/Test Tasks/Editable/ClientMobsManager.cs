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
        private int monsterId = -1;
        private float monsterHealthPercent;

        void Update()
        { 
            monsterHealthbar.value = Mathf.Lerp(monsterHealthbar.value, monsterHealthPercent, Time.deltaTime * 10);
        }
        
        //whenever a monster is updated
        public void UpdateMonster(int id, MonsterNames type, string name, float maxHealth, float currentHealth)
        {
            monsterId = id;
            monsterNameField.text = name;
            UpdateMonsterHealthPercentage(monsterId, currentHealth / maxHealth);
        }

        public void UpdateMonsterHealthPercentage(int id, float healthPercent)
        {
            //error checking in case we happen to receive a packet from a different id (e.g. health update from a monster we already killed)
            if (id != monsterId)
            {
                Debug.LogWarning("Error: Received health update for a different monster ID#" + id + ". (expected monster ID# " + monsterId + ")");
            }
            monsterHealthPercent = healthPercent;
        }

        public void DamageMonster()
        {
            ClientPacketsHandler.SendDamageRequest(monsterId,50);
        }
    }
}
