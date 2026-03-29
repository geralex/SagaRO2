using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

using SagaMap;

namespace SagaMap.Packets.Client
{
    public class UnregisterFriendlistChar : Packet
    {
        public UnregisterFriendlistChar()
        {
            //0x1202
            this.size = 38;
            this.offset = 4;
        }

        public string GetName()
        {
            return this.GetString(4);
        }

        public override SagaLib.Packet New()
        {
            return new UnregisterFriendlistChar();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)client).OnUnregisterFriendlistChar(this);
        }
    }
}
