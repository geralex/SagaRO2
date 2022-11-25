using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Client
{
    public class RefreshFriendlist : Packet
    {
        public RefreshFriendlist()
        {
            //0x1203
            this.size = 4;
            this.offset = 4;
        }

        public uint GetFriendID()
        {
            return this.GetUInt(4);
        }


        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.RefreshFriendlist();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnRefreshFriendlist(this);
        }

    }
}
