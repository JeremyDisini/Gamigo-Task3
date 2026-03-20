using System;
using TestTask.Editable;
using TestTask.NonEditable;
using UnityEditor.PackageManager;
using UnityEngine;
using Random = UnityEngine.Random;

namespace TestTask.Editable
{
    public class ServerMobsManager
    {
        [field: SerializeField] public MonsterData MonsterData { get; private set; }

        public ServerMobsManager()
        {
            MonsterData = SpawnMonster();
        }

        public MonsterData SpawnMonster()
        {
            var monsterId = Random.Range(1, 1000);
            var monsterType = MonsterNameExtensions.MonsterTypeFromId(monsterId);
            var monsterMaxHealth = Random.Range(50, 201);
            var monsterCurrentHealth = monsterMaxHealth;

            MonsterData = new MonsterData(monsterId, monsterType, monsterMaxHealth, monsterCurrentHealth);
            MonsterData.MonsterDeath += OnMonsterDied;
            
            MonsterData.MonsterDamaged += OnMonsterDamaged;

            return MonsterData;
        }

        //whenever the monster is damaged, send the updated health back to the client
        private void OnMonsterDamaged(float updatedHealth)
        {
            ServerPacketsHandler.SendMonsterUpdatedHealthPercent(MonsterData.MonsterId, updatedHealth);
        }
        
        public void OnMonsterDied()
        {
            MonsterData.MonsterDeath -= OnMonsterDied;
            MonsterData = SpawnMonster();
            
            //send request to send over new monster information to client
            ServerPacketsHandler.SendMonsterData(MonsterData);
        }
    }
}  
