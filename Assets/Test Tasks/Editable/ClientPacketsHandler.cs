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

        //whenever a health update is received from the server
        public static void MonsterHealthPercentReceived(Packet packet)
        {
            byte packetId = packet.ReadByte();
            int monsterId = packet.ReadInt();
            float monsterHealthPercent = packet.ReadFloat();
            Debug.Log("Client: received new health percent of " + monsterHealthPercent * 100 + "% for monster ID#" + monsterId);

            //Try to register the health packet to the client 
            bool success = ClientManager.Instance.ClientMobsManager.TryRegisterHealthPacket(packetId, monsterId);
            if(success)
            {
                ClientManager.Instance.ClientMobsManager.UpdateHealthbar(monsterHealthPercent);
            }
        }

        
        public static void ColorsReceived(Packet packet)
        {
            int colorCount = packet.ReadInt();
            ClientManager.Instance.ClientColorManager.OnReceivedColors(packet.ReadBytes(colorCount * 3));
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

        public static void SendColorRequest(int count)
        {
            if(count <= 0)
            {
                Debug.LogError("Error: Requested 0 or negative colors.");
                return;
            }

            Packet packet = new Packet(3);
            packet.Write(count);
            ClientManager.Instance.PacketSenderClient.SendToServer(packet);
        }
        #endregion
    }
}
