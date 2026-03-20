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

        public static void MonsterDataReceived(Packet packet)
        {
            Debug.Log("Received monster data!");
            //unwrap monster data from packet
            int monsterId = packet.ReadInt();
            MonsterNames monsterType = (MonsterNames)packet.ReadByte();
            string monsterName = packet.ReadString();
            float monsterMaxHealth = packet.ReadFloat();
            float monsterCurrentHealth = packet.ReadFloat();
            //update client side data
            ClientManager.Instance.ClientMobsManager.UpdateMonsterName(monsterName);
        }
        #endregion

        #region Packet Senders
        public static void SendLoginRequest()
        {
            Packet packet = new Packet(1);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }
        #endregion
    }
}
