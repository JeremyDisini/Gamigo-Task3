using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public class ServerMobsManager
    {
        [field: SerializeField] public MonsterData MonsterData { get; private set; }
        
        //stored index for packets (used to ensure latest packet in the client)
        private byte packetIndex = 0;

        public ServerMobsManager()
        {
            MonsterData = SpawnMonster();
        }

        //spawns a new monster
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
            //increment packet index to ensure each packet has a unique ID corresponding to it's relative time
            packetIndex++;
            ServerPacketsHandler.SendMonsterHealthPercent(packetIndex, MonsterData.MonsterId, updatedHealth);
        }
        
        public void OnMonsterDied()
        {
            MonsterData.MonsterDeath -= OnMonsterDied;
            
            //spawn a new monster after death
            MonsterData = SpawnMonster();
            
            //send request to send over new monster information to client
            ServerPacketsHandler.SendMonsterData(MonsterData);
        }
    }
}  
