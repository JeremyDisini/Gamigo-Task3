using System.Collections.Generic;
using TestTask.NonEditable;

namespace TestTask.Editable
{
    public static class PacketHandlerLookup
    {
        public delegate void PacketHandler(Packet packet);
        //client packet receivers
        public static Dictionary<int, PacketHandler> OnClientPacketHandlers = new Dictionary<int, PacketHandler>()
        {
            //receive login
            {1, ClientPacketsHandler.LoginDataReceived},
            //receive new monster
            {2, ClientPacketsHandler.NewMonsterDataReceived},
            //receive updated monster health (e.g. after damage or heal)
            {3, ClientPacketsHandler.MonsterHealthPercentUpdateReceived}
        };
        
        //server packet receivers
        public static Dictionary<int, PacketHandler> OnServerPacketHandlers = new Dictionary<int, PacketHandler>()
        {
            //request login
            {1, ServerPacketsHandler.LoginRequest},
            //damage monster
            {2, ServerPacketsHandler.MonsterDamageRequest},
        };
    }
}
