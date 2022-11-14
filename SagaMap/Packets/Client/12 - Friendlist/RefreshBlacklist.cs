using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

namespace SagaMap.Packets.Client
{
    public class RefreshBlacklist : Packet
    {
        public RefreshBlacklist() 
        {
            //0x1206 
            this.size = 4;
            this.offset = 4;
        }

        public uint GetQuestID()
        {
            return this.GetUInt(4);
        }

        public override SagaLib.Packet New()
        {
            return (SagaLib.Packet)new SagaMap.Packets.Client.RefreshBlacklist();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)(client)).OnRefreshBlacklist(this);
        }
    }
}
