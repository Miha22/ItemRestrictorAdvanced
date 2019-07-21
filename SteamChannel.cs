//// Decompiled with JetBrains decompiler
//// Type: SDG.Unturned.SteamChannel
//// Assembly: Assembly-CSharp, Version=0.0.0.0, Culture=neutral, PublicKeyToken=null
//// MVID: A3A01109-C0EF-426B-A010-1DBD430D99C6
//// Assembly location: E:\Users\Deniel\Documents\GitHub\ItemRestrictorAdvanced\lib\Assembly-CSharp.dll

//using Steamworks;
//using System;
//using System.Collections.Generic;
//using System.Reflection;
//using UnityEngine;

//namespace SDG.Unturned
//{
//    public class SteamChannel : MonoBehaviour
//    {
//        private static object[] voiceParameters = new object[1];
//        public int id;
//        public SteamPlayer owner;
//        public bool isOwner;
//        public static TriggerReceive onTriggerReceive;
//        public static TriggerSend onTriggerSend;

//        public SteamChannelMethod[] calls { get; protected set; }

//        public bool checkServer(CSteamID steamID)
//        {
//            return steamID == Provider.server;
//        }

//        public bool checkOwner(CSteamID steamID)
//        {
//            if (this.owner == null)
//                return false;
//            return steamID == this.owner.playerID.steamID;
//        }

//        public bool receive(CSteamID steamID, byte[] packet, int offset, int size)
//        {
//            if (SteamChannel.onTriggerReceive != null)
//            {
//                try
//                {
//                    SteamChannel.onTriggerReceive(this, steamID, packet, offset, size);
//                }
//                catch (Exception ex)
//                {
//                    Debug.LogWarning((object)"Plugin raised an exception from SteamChannel.onTriggerReceive (deprecated, if you really have to use Provider.onServerReadingPacket instead):");
//                    Debug.LogException(ex);
//                }
//            }
//            if (size < 2)
//                return true;
//            int index = (int)packet[offset + 1];
//            if (index < 0 || index >= this.calls.Length)
//                return true;
//            ESteamPacket esteamPacket = (ESteamPacket)packet[offset];
//            if (esteamPacket == ESteamPacket.UPDATE_VOICE && size < 5)
//                return true;
//            bool flag1;
//            switch (this.calls[index].attribute.validation)
//            {
//                case ESteamCallValidation.NONE:
//                    flag1 = true;
//                    break;
//                case ESteamCallValidation.ONLY_FROM_SERVER:
//                    flag1 = this.checkServer(steamID);
//                    break;
//                case ESteamCallValidation.SERVERSIDE:
//                    flag1 = Provider.isServer;
//                    break;
//                case ESteamCallValidation.ONLY_FROM_OWNER:
//                    flag1 = this.checkOwner(steamID);
//                    break;
//                default:
//                    flag1 = false;
//                    Debug.LogWarning((object)("Unhandled RPC validation type on method: " + this.calls[index].method.Name));
//                    break;
//            }
//            if (!flag1)
//                return true;
//            bool flag2 = false;
//            switch (this.calls[index].attribute.frequency)
//            {
//                case ESteamCallFrequency.DEFAULT:
//                    flag2 = true;
//                    break;
//                case ESteamCallFrequency.ONCE_PER_PLAYER:
//                    string name = this.calls[index].method.Name;
//                    SteamPlayer steamPlayer = PlayerTool.getSteamPlayer(steamID);
//                    if (steamPlayer == null)
//                    {
//                        Debug.Log((object)("RPC " + name + " on channel " + (object)this.id + " called without player sender, so we're ignoring it"));
//                        return true;
//                    }
//                    if (!steamPlayer.oncePerChannelRPCs.ContainsKey(this.id))
//                        steamPlayer.oncePerChannelRPCs.Add(this.id, new HashSet<string>());
//                    HashSet<string> oncePerChannelRpC = steamPlayer.oncePerChannelRPCs[this.id];
//                    if (oncePerChannelRpC.Contains(name))
//                    {
//                        flag2 = false;
//                        break;
//                    }
//                    oncePerChannelRpC.Add(name);
//                    flag2 = true;
//                    break;
//            }
//            if (!flag2)
//                return true;
//            try
//            {
//                if (esteamPacket == ESteamPacket.UPDATE_UNRELIABLE_CHUNK_BUFFER || esteamPacket == ESteamPacket.UPDATE_RELIABLE_CHUNK_BUFFER)
//                {
//                    SteamPacker.openRead(offset + 2, packet);
//                    this.calls[index].method.Invoke((object)this.calls[index].component, new object[1]
//                    {
//            (object) steamID
//                    });
//                    SteamPacker.closeRead();
//                }
//                else if (this.calls[index].types.Length > 0)
//                {
//                    if (esteamPacket == ESteamPacket.UPDATE_VOICE)
//                    {
//                        SteamChannel.voiceParameters[0] = (object)packet;
//                        this.calls[index].method.Invoke((object)this.calls[index].component, SteamChannel.voiceParameters);
//                    }
//                    else
//                    {
//                        object[] objects = SteamPacker.getObjects(steamID, offset, 2, packet, this.calls[index].types);
//                        if (objects != null)
//                            this.calls[index].method.Invoke((object)this.calls[index].component, objects);
//                    }
//                }
//                else
//                    this.calls[index].method.Invoke((object)this.calls[index].component, (object[])null);
//            }
//            catch (Exception ex)
//            {
//                Debug.LogFormat("Exception raised when RPC invoked {0}:", (object)this.calls[index].method.Name);
//                Debug.LogException(ex);
//            }
//            return true;
//        }

//        public object read(Type type)
//        {
//            return SteamPacker.read(type);
//        }

//        public object[] read(Type type_0, Type type_1)
//        {
//            return SteamPacker.read(type_0, type_1);
//        }

//        public object[] read(Type type_0, Type type_1, Type type_2)
//        {
//            return SteamPacker.read(type_0, type_1, type_2);
//        }

//        public object[] read(Type type_0, Type type_1, Type type_2, Type type_3)
//        {
//            return SteamPacker.read(type_0, type_1, type_2, type_3);
//        }

//        public object[] read(Type type_0, Type type_1, Type type_2, Type type_3, Type type_4)
//        {
//            return SteamPacker.read(type_0, type_1, type_2, type_3, type_4);
//        }

//        public object[] read(
//          Type type_0,
//          Type type_1,
//          Type type_2,
//          Type type_3,
//          Type type_4,
//          Type type_5)
//        {
//            return SteamPacker.read(type_0, type_1, type_2, type_3, type_4, type_5);
//        }

//        public object[] read(
//          Type type_0,
//          Type type_1,
//          Type type_2,
//          Type type_3,
//          Type type_4,
//          Type type_5,
//          Type type_6)
//        {
//            return SteamPacker.read(type_0, type_1, type_2, type_3, type_4, type_5, type_6);
//        }

//        public object[] read(params Type[] types)
//        {
//            return SteamPacker.read(types);
//        }

//        public void write(object objects)
//        {
//            SteamPacker.write(objects);
//        }

//        public void write(object object_0, object object_1)
//        {
//            SteamPacker.write(object_0, object_1);
//        }

//        public void write(object object_0, object object_1, object object_2)
//        {
//            SteamPacker.write(object_0, object_1, object_2);
//        }

//        public void write(object object_0, object object_1, object object_2, object object_3)
//        {
//            SteamPacker.write(object_0, object_1, object_2, object_3);
//        }

//        public void write(
//          object object_0,
//          object object_1,
//          object object_2,
//          object object_3,
//          object object_4)
//        {
//            SteamPacker.write(object_0, object_1, object_2, object_3, object_4);
//        }

//        public void write(
//          object object_0,
//          object object_1,
//          object object_2,
//          object object_3,
//          object object_4,
//          object object_5)
//        {
//            SteamPacker.write(object_0, object_1, object_2, object_3, object_4, object_5);
//        }

//        public void write(
//          object object_0,
//          object object_1,
//          object object_2,
//          object object_3,
//          object object_4,
//          object object_5,
//          object object_6)
//        {
//            SteamPacker.write(object_0, object_1, object_2, object_3, object_4, object_5, object_6);
//        }

//        public void write(params object[] objects)
//        {
//            SteamPacker.write(objects);
//        }

//        public int step
//        {
//            get
//            {
//                return SteamPacker.step;
//            }
//            set
//            {
//                SteamPacker.step = value;
//            }
//        }

//        public bool useCompression
//        {
//            get
//            {
//                return SteamPacker.useCompression;
//            }
//            set
//            {
//                SteamPacker.useCompression = value;
//            }
//        }

//        public bool longBinaryData
//        {
//            get
//            {
//                return SteamPacker.longBinaryData;
//            }
//            set
//            {
//                SteamPacker.longBinaryData = value;
//            }
//        }

//        public void openWrite()
//        {
//            SteamPacker.openWrite(2);
//        }

//        public void closeWrite(string name, CSteamID steamID, ESteamPacket type)
//        {
//            if (!Provider.isChunk(type))
//            {
//                Debug.LogError((object)"Failed to stream non chunk.");
//            }
//            else
//            {
//                int call = this.getCall(name);
//                if (call == -1)
//                    return;
//                int size;
//                byte[] packet;
//                this.getPacket(type, call, out size, out packet);
//                if (this.isOwner && steamID == Provider.client)
//                    this.receive(Provider.client, packet, 0, size);
//                else if (Provider.isServer && steamID == Provider.server)
//                    this.receive(Provider.server, packet, 0, size);
//                else
//                    Provider.send(steamID, type, packet, size, this.id);
//            }
//        }

//        public void closeWrite(string name, ESteamCall mode, byte bound, ESteamPacket type)
//        {
//            if (!Provider.isChunk(type))
//            {
//                Debug.LogError((object)"Failed to stream non chunk.");
//            }
//            else
//            {
//                int call = this.getCall(name);
//                if (call == -1)
//                    return;
//                int size;
//                byte[] packet;
//                this.getPacket(type, call, out size, out packet);
//                this.send(mode, bound, type, size, packet);
//            }
//        }

//        public void closeWrite(
//          string name,
//          ESteamCall mode,
//          byte x,
//          byte y,
//          byte area,
//          ESteamPacket type)
//        {
//            if (!Provider.isChunk(type))
//            {
//                Debug.LogError((object)"Failed to stream non chunk.");
//            }
//            else
//            {
//                int call = this.getCall(name);
//                if (call == -1)
//                    return;
//                int size;
//                byte[] packet;
//                this.getPacket(type, call, out size, out packet);
//                this.send(mode, x, y, area, type, size, packet);
//            }
//        }

//        public void closeWrite(string name, ESteamCall mode, ESteamPacket type)
//        {
//            if (!Provider.isChunk(type))
//            {
//                Debug.LogError((object)"Failed to stream non chunk.");
//            }
//            else
//            {
//                int call = this.getCall(name);
//                if (call == -1)
//                    return;
//                int size;
//                byte[] packet;
//                this.getPacket(type, call, out size, out packet);
//                this.send(mode, type, size, packet);
//            }
//        }

//        public void send(string name, CSteamID steamID, ESteamPacket type, params object[] arguments)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, arguments);
//            if (this.isOwner && steamID == Provider.client)
//                this.receive(Provider.client, packet, 0, size);
//            else if (Provider.isServer && steamID == Provider.server)
//                this.receive(Provider.server, packet, 0, size);
//            else
//                Provider.send(steamID, type, packet, size, this.id);
//        }

//        public void sendAside(
//          string name,
//          CSteamID steamID,
//          ESteamPacket type,
//          params object[] arguments)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, arguments);
//            for (int index = 0; index < Provider.clients.Count; ++index)
//            {
//                if (Provider.clients[index].playerID.steamID != steamID)
//                    Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//            }
//        }

//        public void send(ESteamCall mode, byte bound, ESteamPacket type, int size, byte[] packet)
//        {
//            switch (mode)
//            {
//                case ESteamCall.SERVER:
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(Provider.server, type, packet, size, this.id);
//                    break;
//                case ESteamCall.ALL:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (int)Provider.clients[index].player.movement.bound == (int)bound)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.OTHERS:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (int)Provider.clients[index].player.movement.bound == (int)bound)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.OWNER:
//                    if (this.isOwner)
//                    {
//                        this.receive(this.owner.playerID.steamID, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(this.owner.playerID.steamID, type, packet, size, this.id);
//                    break;
//                case ESteamCall.NOT_OWNER:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != this.owner.playerID.steamID && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (int)Provider.clients[index].player.movement.bound == (int)bound)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.CLIENTS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (int)Provider.clients[index].player.movement.bound == (int)bound)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (!Provider.isClient)
//                        break;
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.PEERS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (int)Provider.clients[index].player.movement.bound == (int)bound)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//            }
//        }

//        public void send(
//          string name,
//          ESteamCall mode,
//          byte bound,
//          ESteamPacket type,
//          byte[] bytes,
//          int length)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, bytes, length);
//            this.send(mode, bound, type, size, packet);
//        }

//        public void send(
//          string name,
//          ESteamCall mode,
//          byte bound,
//          ESteamPacket type,
//          params object[] arguments)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, arguments);
//            this.send(mode, bound, type, size, packet);
//        }

//        public void send(
//          ESteamCall mode,
//          byte x,
//          byte y,
//          byte area,
//          ESteamPacket type,
//          int size,
//          byte[] packet)
//        {
//            switch (mode)
//            {
//                case ESteamCall.SERVER:
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(Provider.server, type, packet, size, this.id);
//                    break;
//                case ESteamCall.ALL:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && Regions.checkArea(x, y, Provider.clients[index].player.movement.region_x, Provider.clients[index].player.movement.region_y, area))
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.OTHERS:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && Regions.checkArea(x, y, Provider.clients[index].player.movement.region_x, Provider.clients[index].player.movement.region_y, area))
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.OWNER:
//                    if (this.isOwner)
//                    {
//                        this.receive(this.owner.playerID.steamID, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(this.owner.playerID.steamID, type, packet, size, this.id);
//                    break;
//                case ESteamCall.NOT_OWNER:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != this.owner.playerID.steamID && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && Regions.checkArea(x, y, Provider.clients[index].player.movement.region_x, Provider.clients[index].player.movement.region_y, area))
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.CLIENTS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && Regions.checkArea(x, y, Provider.clients[index].player.movement.region_x, Provider.clients[index].player.movement.region_y, area))
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (!Provider.isClient)
//                        break;
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.PEERS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && Regions.checkArea(x, y, Provider.clients[index].player.movement.region_x, Provider.clients[index].player.movement.region_y, area))
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//            }
//        }

//        public void send(
//          string name,
//          ESteamCall mode,
//          byte x,
//          byte y,
//          byte area,
//          ESteamPacket type,
//          byte[] bytes,
//          int length)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, bytes, length);
//            this.send(mode, x, y, area, type, size, packet);
//        }

//        public void send(
//          string name,
//          ESteamCall mode,
//          byte x,
//          byte y,
//          byte area,
//          ESteamPacket type,
//          params object[] arguments)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, arguments);
//            this.send(mode, x, y, area, type, size, packet);
//        }

//        public void send(ESteamCall mode, ESteamPacket type, int size, byte[] packet)
//        {
//            switch (mode)
//            {
//                case ESteamCall.SERVER:
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(Provider.server, type, packet, size, this.id);
//                    break;
//                case ESteamCall.ALL:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.OTHERS:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.OWNER:
//                    if (this.isOwner)
//                    {
//                        this.receive(this.owner.playerID.steamID, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(this.owner.playerID.steamID, type, packet, size, this.id);
//                    break;
//                case ESteamCall.NOT_OWNER:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != this.owner.playerID.steamID)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.CLIENTS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (!Provider.isClient)
//                        break;
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.PEERS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//            }
//        }

//        public void send(string name, ESteamCall mode, ESteamPacket type, params object[] arguments)
//        {
//            if (SteamChannel.onTriggerSend != null)
//            {
//                try
//                {
//                    SteamChannel.onTriggerSend(this.owner, name, mode, type, arguments);
//                }
//                catch (Exception ex)
//                {
//                    Debug.LogWarning((object)"Plugin raised an exception from SteamChannel.onTriggerSend (deprecated, if you really have to use Provider.onServerWritingPacket instead):");
//                    Debug.LogException(ex);
//                }
//            }
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, arguments);
//            this.send(mode, type, size, packet);
//        }

//        public void send(string name, ESteamCall mode, ESteamPacket type, byte[] bytes, int length)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, bytes, length);
//            this.send(mode, type, size, packet);
//        }

//        public void send(
//          ESteamCall mode,
//          Vector3 point,
//          float radius,
//          ESteamPacket type,
//          int size,
//          byte[] packet)
//        {
//            radius *= radius;
//            switch (mode)
//            {
//                case ESteamCall.SERVER:
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(Provider.server, type, packet, size, this.id);
//                    break;
//                case ESteamCall.ALL:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (double)(Provider.clients[index].player.transform.position - point).sqrMagnitude < (double)radius)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (Provider.isServer)
//                    {
//                        this.receive(Provider.server, packet, 0, size);
//                        break;
//                    }
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.OTHERS:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (double)(Provider.clients[index].player.transform.position - point).sqrMagnitude < (double)radius)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.OWNER:
//                    if (this.isOwner)
//                    {
//                        this.receive(this.owner.playerID.steamID, packet, 0, size);
//                        break;
//                    }
//                    Provider.send(this.owner.playerID.steamID, type, packet, size, this.id);
//                    break;
//                case ESteamCall.NOT_OWNER:
//                    if (!Provider.isServer)
//                        Provider.send(Provider.server, type, packet, size, this.id);
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != this.owner.playerID.steamID && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (double)(Provider.clients[index].player.transform.position - point).sqrMagnitude < (double)radius)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//                case ESteamCall.CLIENTS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (double)(Provider.clients[index].player.transform.position - point).sqrMagnitude < (double)radius)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    if (!Provider.isClient)
//                        break;
//                    this.receive(Provider.client, packet, 0, size);
//                    break;
//                case ESteamCall.PEERS:
//                    for (int index = 0; index < Provider.clients.Count; ++index)
//                    {
//                        if (Provider.clients[index].playerID.steamID != Provider.client && (UnityEngine.Object)Provider.clients[index].player != (UnityEngine.Object)null && (double)(Provider.clients[index].player.transform.position - point).sqrMagnitude < (double)radius)
//                            Provider.send(Provider.clients[index].playerID.steamID, type, packet, size, this.id);
//                    }
//                    break;
//            }
//        }

//        public void send(
//          string name,
//          ESteamCall mode,
//          Vector3 point,
//          float radius,
//          ESteamPacket type,
//          params object[] arguments)
//        {
//            int call = this.getCall(name);
//            if (call == -1)
//                return;
//            int size;
//            byte[] packet;
//            this.getPacket(type, call, out size, out packet, arguments);
//            this.send(mode, point, radius, type, size, packet);
//        }

//        public void build()
//        {
//            List<SteamChannelMethod> steamChannelMethodList = new List<SteamChannelMethod>();
//            Component[] components = this.GetComponents(typeof(Component));
//            for (int index1 = 0; index1 < components.Length; ++index1)
//            {
//                MemberInfo[] members = components[index1].GetType().GetMembers();
//                for (int index2 = 0; index2 < members.Length; ++index2)
//                {
//                    if (members[index2].MemberType == MemberTypes.Method)
//                    {
//                        MethodInfo newMethod = (MethodInfo)members[index2];
//                        object[] customAttributes = newMethod.GetCustomAttributes(typeof(SteamCall), true);
//                        if (customAttributes.Length > 0)
//                        {
//                            SteamCall attribute = customAttributes[0] as SteamCall;
//                            if (attribute != null)
//                            {
//                                ParameterInfo[] parameters = newMethod.GetParameters();
//                                Type[] newTypes = new Type[parameters.Length];
//                                for (int index3 = 0; index3 < parameters.Length; ++index3)
//                                    newTypes[index3] = parameters[index3].ParameterType;
//                                steamChannelMethodList.Add(new SteamChannelMethod(components[index1], newMethod, newTypes, attribute));
//                            }
//                        }
//                    }
//                }
//            }
//            this.calls = steamChannelMethodList.ToArray();
//            if (this.calls.Length <= 235)
//                return;
//            CommandWindow.LogError((object)(this.name + " approaching 255 methods!"));
//        }

//        public void setup()
//        {
//            Provider.openChannel(this);
//        }

//        public void getPacket(ESteamPacket type, int index, out int size, out byte[] packet)
//        {
//            packet = SteamPacker.closeWrite(out size);
//            packet[0] = (byte)type;
//            packet[1] = (byte)index;
//        }

//        public void getPacket(
//          ESteamPacket type,
//          int index,
//          out int size,
//          out byte[] packet,
//          byte[] bytes,
//          int length)
//        {
//            size = 4 + length;
//            packet = bytes;
//            packet[0] = (byte)type;
//            packet[1] = (byte)index;
//            byte[] bytes1 = BitConverter.GetBytes((ushort)length);
//            packet[2] = bytes1[0];
//            packet[3] = bytes1[1];
//        }

//        public void encodeVoicePacket(
//          byte callIndex,
//          out int size,
//          out byte[] packet,
//          byte[] bytes,
//          ushort length,
//          bool usingWalkieTalkie)
//        {
//            size = 5 + (int)length;
//            packet = bytes;
//            packet[0] = (byte)23;
//            packet[1] = callIndex;
//            byte[] bytes1 = BitConverter.GetBytes(length);
//            packet[2] = bytes1[0];
//            packet[3] = bytes1[1];
//            packet[4] = !usingWalkieTalkie ? (byte)0 : (byte)1;
//        }

//        public void decodeVoicePacket(
//          byte[] packet,
//          out uint compressedSize,
//          out bool usingWalkieTalkie)
//        {
//            compressedSize = (uint)BitConverter.ToUInt16(packet, 2);
//            usingWalkieTalkie = packet[4] == (byte)1;
//        }

//        public void sendVoicePacket(CSteamID steamID, byte[] packet, int packetSize)
//        {
//            if (Dedicator.isDedicated)
//                Provider.send(steamID, ESteamPacket.UPDATE_VOICE, packet, packetSize, this.id);
//            else
//                this.receive(steamID, packet, 0, packetSize);
//        }

//        public void getPacket(
//          ESteamPacket type,
//          int index,
//          out int size,
//          out byte[] packet,
//          params object[] arguments)
//        {
//            packet = SteamPacker.getBytes(2, out size, arguments);
//            packet[0] = (byte)type;
//            packet[1] = (byte)index;
//        }

//        public int getCall(string name)
//        {
//            for (int index = 0; index < this.calls.Length; ++index)
//            {
//                if (this.calls[index].method.Name == name)
//                    return index;
//            }
//            CommandWindow.LogError((object)("Failed to find a method named: " + name));
//            return -1;
//        }

//        private void Awake()
//        {
//            this.build();
//        }

//        private void OnDestroy()
//        {
//            if (this.id == 0)
//                return;
//            Provider.closeChannel(this);
//        }
//    }
//}
