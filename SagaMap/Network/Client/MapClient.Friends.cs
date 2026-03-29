using System;
using System.Collections.Generic;

using SagaLib;
using SagaMap.Manager;

namespace SagaMap
{
    public partial class MapClient
    {
        const int MaxFriends = 50;
        const int MaxBlacklist = 10;

        public void OnRegisterFriendlistChar(Packets.Client.RegisterFriendlistChar p)
        {
            if (this.Char == null || this.state == SESSION_STATE.NOT_IDENTIFIED || this.state == SESSION_STATE.LOGGEDOFF) return;

            byte result = 0;
            byte clvl = 0, jlvl = 0, map = 0, job = 0;
            string nname = p.GetName();
            if (string.IsNullOrEmpty(nname)) return;

            if (string.Equals(this.Char.name, nname, StringComparison.OrdinalIgnoreCase))
                result = 4;
            else if (this.Char.Friends != null && this.Char.Friends.Contains(nname))
                result = 3;
            else if (this.Char.Friends != null && this.Char.Friends.Count >= MaxFriends)
                result = 2;
            else if (!MapServer.charDB.CharExists(this.Char.worldID, nname))
                result = 1;
            else
            {
                if (!MapServer.charDB.InsertFriend(this.Char, nname))
                    result = 1;
                else
                {
                    if (this.Char.Friends == null) this.Char.Friends = new List<string>();
                    this.Char.Friends.Add(nname);
                    MapClient online = MapClientManager.Instance.GetClientByNameCaseInsensitive(nname);
                    if (online != null && online.Char != null)
                    {
                        clvl = (byte)Math.Min(online.Char.cLevel, 255);
                        jlvl = (byte)Math.Min(online.Char.jLevel, 255);
                        map = online.Char.mapID;
                        job = (byte)online.Char.job;
                    }
                }
            }

            Packets.Server.RegisterFriendlistChar sp = new SagaMap.Packets.Server.RegisterFriendlistChar();
            sp.SetName(nname);
            sp.SetJob(job);
            sp.SetClvl(clvl);
            sp.SetJlvl(jlvl);
            sp.SetMap(map);
            sp.SetReason(result);
            this.netIO.SendPacket(sp, this.SessionID);
        }

        public void OnUnregisterFriendlistChar(Packets.Client.UnregisterFriendlistChar p)
        {
            if (this.Char == null || this.state == SESSION_STATE.NOT_IDENTIFIED || this.state == SESSION_STATE.LOGGEDOFF) return;

            byte result = 0;
            string nname = p.GetName();
            if (string.IsNullOrEmpty(nname)) return;

            if (this.Char.Friends == null || !this.Char.Friends.Remove(nname))
                result = 5;
            else
                MapServer.charDB.DeleteFriend(this.Char, nname);

            Packets.Server.UnregisterFriendlistChar sp = new SagaMap.Packets.Server.UnregisterFriendlistChar();
            sp.SetName(nname);
            sp.SetReason(result);
            this.netIO.SendPacket(sp, this.SessionID);
        }

        public void OnRefreshFriendlist(Packets.Client.RefreshFriendlist p)
        {
            if (this.Char == null || this.state == SESSION_STATE.NOT_IDENTIFIED || this.state == SESSION_STATE.LOGGEDOFF) return;

            Packets.Server.RefreshFriendlist sp = new SagaMap.Packets.Server.RefreshFriendlist();
            if (this.Char.Friends != null)
            {
                foreach (string s in this.Char.Friends)
                {
                    MapClient target = MapClientManager.Instance.GetClientByNameCaseInsensitive(s);
                    if (target != null && target.Char != null)
                    {
                        sp.Add(s, (byte)target.Char.job,
                            (byte)Math.Min(target.Char.cLevel, 255),
                            (byte)Math.Min(target.Char.jLevel, 255),
                            target.Char.mapID);
                    }
                    else
                        sp.Add(s, 0, 0, 0, 0);
                }
            }
            this.netIO.SendPacket(sp, this.SessionID);
        }

        /// <summary>????????? ?????? ? ????, ??? ???? ???????? ????? (saga-revised).</summary>
        internal static void BroadcastFriendLogin(MapClient who)
        {
            if (who == null || who.Char == null || who.Char.Friends == null) return;
            foreach (MapClient c in MapClientManager.Instance.Players)
            {
                if (c == null || c.Char == null || c.Char.Friends == null) continue;
                if (c.state == SESSION_STATE.NOT_IDENTIFIED || c.state == SESSION_STATE.LOGGEDOFF) continue;
                foreach (string fname in c.Char.Friends)
                {
                    if (string.Equals(fname, who.Char.name, StringComparison.OrdinalIgnoreCase))
                    {
                        Packets.Server.FriendLoginNotify n = new SagaMap.Packets.Server.FriendLoginNotify();
                        n.SetName(who.Char.name);
                        c.netIO.SendPacket(n, c.SessionID);
                        break;
                    }
                }
            }
        }
    }
}
