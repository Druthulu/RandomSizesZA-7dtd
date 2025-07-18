﻿using System;
using System.Collections.Generic;
using static EAIBlockIf;

public class NetPkgRZA_Scale : NetPackage
{
    public int EntityId = 0;
    public float Scale = 0f;
    public string EType = "o";

    public NetPkgRZA_Scale ToServer(int entityId, string eType)
    {
        this.EntityId = entityId;
        this.Scale = 1f;
        this.EType = eType;
        //RZA_Utils.LOD($"NetPkgRZA_Scale ToServer eID:{entityId} eType:{eType}");
        return this;
    }

    public NetPkgRZA_Scale ToClient(int entityId, float scale)
    {
        this.EntityId = entityId;
        this.Scale = scale;
        //RZA_Utils.LOD($"NetPkgRZA_Scale ToClient eID:{entityId} scale:{scale} ");
        return this;
    }

    public override void read(PooledBinaryReader _br)
    {
        this.EntityId = _br.ReadInt32();
        this.Scale = _br.ReadSingle();
        this. EType = _br.ReadString();
        //RZA_Utils.LOD("NetPkgRZA_Scale Read");
    }

    public override void write(PooledBinaryWriter _bw)
    {
        base.write(_bw);
        WriteInt(_bw, this.EntityId);
        WriteFloat(_bw, this.Scale);
        WriteString(_bw, this.EType);
        //RZA_Utils.LOD("NetPkgRZA_Scale Write");
    }
    private void WriteInt(PooledBinaryWriter bw, int value)
    {
        byte[] buf = BitConverter.GetBytes(value);
        bw.Write(buf, 0, buf.Length); // Make sure PooledBinaryWriter has this overload!
    }
    private void WriteFloat(PooledBinaryWriter bw, float value)
    {
        byte[] buf = BitConverter.GetBytes(value);
        bw.Write(buf, 0, buf.Length);
    }
    private void WriteString(PooledBinaryWriter bw, string value)
    {
        if (value == null)
        {
            WriteInt(bw, -1); // Indicate null string
            return;
        }

        byte[] strBytes = System.Text.Encoding.UTF8.GetBytes(value);
        WriteInt(bw, strBytes.Length); // Write string length as prefix
        bw.Write(strBytes, 0, strBytes.Length); // Write actual string bytes
    }


    public override void ProcessPackage(World _world, GameManager _callbacks)
    {
        //RZA_Utils.LOD("NetPkgRZA_Scale ProcessPackge start");
        if (_world == null) return;
        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            float min = 0; float max = 0;
            // Override client side choise of sizes with server sizes
            if (this.EType == "Z")
            {
                min = Init.zombieMin;
                max = Init.zombieMax;
            }
            if (this.EType == "A")
            {
                min = Init.animalMin;
                max = Init.animalMax;
            }
            //RZA_Utils.LOD($"NetPkgRZA_Scale Server ProcessPackge. Searching Dict");
            float scale = RZA_Utils.ServerDictSearch(this.EntityId, min, max);
            //RZA_Utils.LOD($"NetPkgRZA_Scale Server ProcessPackge eID:{this.EntityId} scale:{scale}");
            Sender.SendPackage(NetPackageManager.GetPackage<NetPkgRZA_Scale>().ToClient(this.EntityId, scale));
        }
        else
        {
            //RZA_Utils.LOD($"NetPkgRZA_Scale Client ProcessPackge. start. eID:{this.EntityId} scale:{this.Scale}");
            if (this.Scale >0 && !Init.entityScaleDict.ContainsKey(this.EntityId))
            {
                //RZA_Utils.LOD($"PKG ProcessPackge. client eID:{this.EntityId} scale:{this.Scale}");
                Init.entityScaleDict.Add(this.EntityId, this.Scale);
                //RZA_Utils.LOD($"NetPkgRZA_Scale Client ProcessPackge. done. eID:{this.EntityId} scale:{this.Scale}");
            }
        }
    }

    public override int GetLength() => 9; // (4byte Int32) + (4byte float) + (1byte string)
}
