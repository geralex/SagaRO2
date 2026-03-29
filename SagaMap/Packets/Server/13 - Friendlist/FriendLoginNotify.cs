using SagaLib;

namespace SagaMap.Packets.Server
{
    /// <summary>0x1307 Ч друг вошЄл в игру (saga-revised SMSG_FRIENDSLIST_NOTIFYLOGIN).</summary>
    public class FriendLoginNotify : Packet
    {
        public FriendLoginNotify()
        {
            this.data = new byte[68];
            this.ID = 0x1307;
            this.offset = 4;
        }

        public void SetName(string name)
        {
            PutString(name, 4);
        }
    }
}
