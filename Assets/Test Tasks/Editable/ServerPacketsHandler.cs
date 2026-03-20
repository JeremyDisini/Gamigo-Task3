using TestTask.NonEditable;
using UnityEngine;

namespace TestTask.Editable
{
    public static class ServerPacketsHandler
    {
        #region Packet Handlers
        public static void LoginRequest(Packet packet)
        {
            var clientLogInResponse = ServerMock.Instance.TryConnectClient(out var clientId);
            SendLoginResponse(clientLogInResponse, clientId);
        }
        
        //damage monster in the server
        public static void MonsterDamageRequest(Packet packet)
        {
            int monsterId = packet.ReadInt();
            float damage = packet.ReadFloat();
            Debug.Log("Server: Received Request, Damage Monster ID#" + monsterId + " by " + damage);
            
            //deny the damage request if the monster IDs mismatch (e.g. trying to damage a monster that's already dead)
            if (monsterId != ServerMock.Instance.ServerMobsManager.MonsterData.MonsterId)
            {
                Debug.LogWarning("Server: Requested damage to a non-existing monster with ID#" + monsterId);
                return;
            }
            
            ServerMock.Instance.ServerMobsManager.MonsterData.TakeDamage(damage);
        }

        #endregion

        #region Packet Senders
        public static void SendLoginResponse(LoginResponse response, int clientId)
        {
            // send login packet back to client
            using (Packet packet = new Packet(1))
            {
                packet.Write((int)response);
                packet.Write(clientId);

                ServerMock.Instance.PacketSenderServer.SendToClient(packet);
            }
            
            // after a successful login attempt, send monster data back to the client as a packet
            if (response == LoginResponse.Success)
            {
                SendMonsterData(ServerMock.Instance.ServerMobsManager.MonsterData);
            }
        }
        
        //sending monster data back to client
        public static void SendMonsterData(MonsterData monsterData)
        {
            Packet packet = new Packet(2);
            //serialize monster data into a packet
            packet.Write(monsterData.MonsterId);
            packet.Write((byte)monsterData.MonsterType);
            packet.Write(monsterData.MonsterName);
            packet.Write(monsterData.MonsterMaxHealth);
            packet.Write(monsterData.MonsterCurrentHealth);
            
            //send packet to client
            ServerMock.Instance.PacketSenderServer.SendToClient(packet);
            Debug.Log("Server: sending monster data...");
        }

        public static void SendMonsterUpdatedHealthPercent(int monsterId, float health)
        {
            Debug.Log("Server: sending updated health percent for monster ID#" + monsterId + " back to client");
            Packet packet = new Packet(3);
            packet.Write(monsterId);
            packet.Write(health);
            ServerMock.Instance.PacketSenderServer.SendToClient(packet);
        }
        #endregion
    }
}

public enum LoginResponse
{
    Success = 0,
    Failure = 1,
}