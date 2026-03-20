using System;
using System.Collections.Generic;
using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public static class ClientPacketsHandler
    {
        #region Packet Handlers
        public static void LoginDataReceived(Packet packet)
        {
            int responseCode = packet.ReadInt();
            int clientId = packet.ReadInt();
            ClientManager.Instance.SetClientLogInStatus(responseCode, clientId);
        }

        public static void NewMonsterDataReceived(Packet packet)
        {
            Debug.Log("Client: Received monster data!");
            //unwrap monster data from packet
            int monsterId = packet.ReadInt();
            MonsterNames monsterType = (MonsterNames)packet.ReadByte();
            string monsterName = packet.ReadString();
            float monsterMaxHealth = packet.ReadFloat();
            float monsterCurrentHealth = packet.ReadFloat();
            
            //send unwrapped data to client
            ClientManager.Instance.ClientMobsManager.UpdateMonster(monsterId, monsterType, monsterName, monsterMaxHealth, monsterCurrentHealth);
        }

        public static void MonsterHealthPercentUpdateReceived(Packet packet)
        {
            int monsterId = packet.ReadInt();
            float monsterHealthPercent = packet.ReadFloat();
            Debug.Log("Client: received new health percent of " + monsterHealthPercent * 100 + "% for monster ID#" + monsterId);
            ClientManager.Instance.ClientMobsManager.DamageMonster(monsterId, monsterHealthPercent);
        }
        #endregion

        #region Packet Senders
        public static void SendLoginRequest()
        {
            Packet packet = new Packet(1);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }
        public static void SendDamageRequest(int monsterId, float damage)
        {
            Packet packet = new Packet(2);
            packet.Write(monsterId);
            packet.Write(damage);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
            Debug.Log("Client: sending request to damage monster ID#" + monsterId + " by " + damage);
        }
        #endregion
    }
}
