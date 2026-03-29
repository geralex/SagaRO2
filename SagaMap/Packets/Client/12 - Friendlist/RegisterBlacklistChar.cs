using System;
using System.Collections.Generic;
using System.Text;

using SagaLib;

using SagaMap;

namespace SagaMap.Packets.Client
{
    public class RegisterBlacklistChar : Packet
    {
        public RegisterBlacklistChar()
        {
            //0x1204 
            this.size = 39;
            this.offset = 4;
        }

        public string GetName()
        {
            return this.GetString(4);
        }

        /// <summary>Reason byte after fixed 16-char Unicode name field (32 bytes from offset 4).</summary>
        public byte GetReason()
        {
            return this.GetByte(32);
        }

        public override SagaLib.Packet New()
        {
            return new RegisterBlacklistChar();
        }

        public override void Parse(SagaLib.Client client)
        {
            ((MapClient)client).OnRegisterBlacklistChar(this);
        }
    }
}
