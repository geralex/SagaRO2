using System;
using System.Collections.Generic;
using System.Text;

namespace SagaMap
{
    public partial class MapClient
    {

        #region "0x12"
        // 0x12 Packets =========================================

        // 12 01
        public void OnRefreshFriendlist(Packets.Client.RefreshFriendlist p)
        {
            SendMessage("Saga", "!OnRefreshFriendlist!");

            Packets.Server.RefreshFriendlist p1 = new SagaMap.Packets.Server.RefreshFriendlist();
            //p1.Add("test",1,1,1,0);
            this.netIO.SendPacket(p1, this.SessionID);

            
        }

        #endregion

    }
}
