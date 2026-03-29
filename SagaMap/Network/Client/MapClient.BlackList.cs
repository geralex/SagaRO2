using System;
using System.Collections.Generic;

using SagaLib;
using SagaMap.Manager;

namespace SagaMap
{
    public partial class MapClient
    {
        public void OnRegisterBlacklistChar(Packets.Client.RegisterBlacklistChar p)
        {
            if (this.Char == null || this.state == SESSION_STATE.NOT_IDENTIFIED || this.state == SESSION_STATE.LOGGEDOFF) return;

            byte result = 0;
            string nname = p.GetName();
            if (string.IsNullOrEmpty(nname)) return;

            byte reason = p.GetReason();

            if (string.Equals(this.Char.name, nname, StringComparison.OrdinalIgnoreCase))
                result = 4;
            else if (this.Char.Blacklist != null && this.Char.Blacklist.Exists(delegate(KeyValuePair<string, byte> pair) { return string.Equals(pair.Key, nname, StringComparison.OrdinalIgnoreCase); }))
                result = 3;
            else if (this.Char.Blacklist != null && this.Char.Blacklist.Count >= MaxBlacklist)
                result = 2;
            else if (!MapServer.charDB.CharExists(this.Char.worldID, nname))
                result = 1;
            else
            {
                if (!MapServer.charDB.InsertBlacklist(this.Char, nname, reason))
                    result = 1;
                else
                {
                    if (this.Char.Blacklist == null) this.Char.Blacklist = new List<KeyValuePair<string, byte>>();
                    this.Char.Blacklist.RemoveAll(delegate(KeyValuePair<string, byte> pair) { return string.Equals(pair.Key, nname, StringComparison.OrdinalIgnoreCase); });
                    this.Char.Blacklist.Add(new KeyValuePair<string, byte>(nname, reason));
                    if (this.Char.Friends != null) this.Char.Friends.Remove(nname);
                }
            }

            Packets.Server.RegisterBlacklistChar sp = new SagaMap.Packets.Server.RegisterBlacklistChar();
            sp.SetName(nname);
            sp.SetReasonForBlacklist(reason);
            sp.SetReason(result);
            this.netIO.SendPacket(sp, this.SessionID);
        }

        public void OnUnregisterBlacklistChar(Packets.Client.UnregisterBlacklistChar p)
        {
            if (this.Char == null || this.state == SESSION_STATE.NOT_IDENTIFIED || this.state == SESSION_STATE.LOGGEDOFF) return;

            byte result = 0;
            string nname = p.GetName();
            if (string.IsNullOrEmpty(nname)) return;

            if (this.Char.Blacklist == null || this.Char.Blacklist.RemoveAll(delegate(KeyValuePair<string, byte> pair) { return string.Equals(pair.Key, nname, StringComparison.OrdinalIgnoreCase); }) <= 0)
                result = 5;
            else
                MapServer.charDB.DeleteBlacklist(this.Char, nname);

            Packets.Server.UnregisterBlacklistChar sp = new SagaMap.Packets.Server.UnregisterBlacklistChar();
            sp.SetName(nname);
            sp.SetReason(result);
            this.netIO.SendPacket(sp, this.SessionID);
        }

        public void OnRefreshBlacklist(Packets.Client.RefreshBlacklist p)
        {
            if (this.Char == null || this.state == SESSION_STATE.NOT_IDENTIFIED || this.state == SESSION_STATE.LOGGEDOFF) return;

            Packets.Server.RefreshBlacklist sp = new SagaMap.Packets.Server.RefreshBlacklist();
            if (this.Char.Blacklist != null)
            {
                foreach (KeyValuePair<string, byte> e in this.Char.Blacklist)
                    sp.Add(e.Key, e.Value);
            }
            this.netIO.SendPacket(sp, this.SessionID);
        }
    }
}
