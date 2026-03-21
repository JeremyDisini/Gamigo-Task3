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
        //UI fields
        [SerializeField] private TextMeshProUGUI monsterNameField;
        [SerializeField] private Slider monsterHealthbar;
        [SerializeField] private Image monsterPortrait;
        [SerializeField] private Sprite[] monsterPortraitSprites;
        
        //variables kept in client side to help sync with the server
        private int monsterId = -1;
        private byte healthPercentageLatestPacketId = 0;

        //called whenever the focused mob has to be updated (e.g. when a monster dies and a new one is spawned)
        public void UpdateMonster(int id, MonsterNames type, string name, float maxHealth, float currentHealth)
        {
            monsterId = id;
            monsterNameField.text = name;
            monsterPortrait.sprite = monsterPortraitSprites[(int)type];
            monsterPortrait.color = Color.white;
            monsterHealthbar.value = currentHealth / maxHealth;
        }

        //Updates health bar according to what's happening in the server
        public void UpdateHealthbar(byte packetId, int monsterId, float healthPercent)
        {
            //ignore the packet if monster IDs mismatch (e.g. health update from a monster we already killed)
            if (monsterId != this.monsterId)
            {
                Debug.LogWarning("Received health update for a different monster ID#" + monsterId + ". (expected monster ID# " + this.monsterId + ")");
                return;
            }

            //ignore packet if it is from the past relative to the current latest packet
            //Add an exception for if the packetId overflows (only accept packets below 127 to prevent recent out of order packets from making it through)
            if ((packetId <= healthPercentageLatestPacketId) &&
                !(healthPercentageLatestPacketId == byte.MaxValue && packetId < byte.MaxValue/2))
            {
                Debug.LogError("Dropped outdated packet #" + (int)packetId + " (current latest packet ID: #" + (int)healthPercentageLatestPacketId + ")");
                return;
            }

            //Update the UI and record the current latest packet id
            healthPercentageLatestPacketId = packetId;
            monsterHealthbar.value = healthPercent;
        }

        //send a damage request to the server
        public void DamageMonster(float damage)
        {
            ClientPacketsHandler.SendDamageRequest(monsterId,damage);
        }
    }
}
