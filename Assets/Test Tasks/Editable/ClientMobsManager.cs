using System;
using TestTask.NonEditable;
using TMPro;
using UnityEditor.Build.Reporting;
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

        private byte healthPercentagePacketId = 0;

        //storing current and max health locally so we can manipulate client side data 

        //whenever a monster is updated
        public void UpdateMonster(int id, MonsterNames type, string name, float maxHealth, float currentHealth)
        {
            monsterId = id;
            monsterNameField.text = name;
            monsterPortrait.sprite = monsterPortraitSprites[(int)type];
            monsterPortrait.color = Color.white;
            monsterHealthbar.value = currentHealth / maxHealth;
        }

        //Updates health percentage according to what's happening in the server
        public void UpdateHealthPercentage(byte packetId, int monsterId, float healthPercent)
        {
            //error checking in case we happen to receive a packet from a different id (e.g. health update from a monster we already killed)
            if (monsterId != this.monsterId)
            {
                Debug.LogWarning("Received health update for a different monster ID#" + monsterId + ". (expected monster ID# " + this.monsterId + ")");
                return;
            }

            //if the packet is from the past relative to the current latest packet, then ignore it
            //Add an exception for if the packetId overflows (only accept packets below 127 to prevent REALLY old packets from being accepted)
            if ((packetId <= healthPercentagePacketId) &&
                !(healthPercentagePacketId == byte.MaxValue && packetId < byte.MaxValue/2))
            {
                Debug.LogError("Dropped outdated packet #" + (int)packetId + " (current latest packet ID: #" + (int)healthPercentagePacketId + ")");
                return;
            }

            //record the packet index and set monster health to proper value
            healthPercentagePacketId = packetId;
            monsterHealthbar.value = healthPercent;
        }

        //send a damage request to the server
        public void DamageMonster(float damage)
        {
            ClientPacketsHandler.SendDamageRequest(monsterId,damage);
        }
    }
}
