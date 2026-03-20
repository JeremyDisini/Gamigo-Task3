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
            Debug.Log("Server sending monster data...");
        }
        #endregion
    }
}

public enum LoginResponse
{
    Success = 0,
    Failure = 1,
}