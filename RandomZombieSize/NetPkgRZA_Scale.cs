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
        _bw.Write(this.EntityId);
        _bw.Write(this.Scale);
        _bw.Write(this.EType);
        //RZA_Utils.LOD("NetPkgRZA_Scale Write");
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
            // Client recieves scale from server and adds to local dict so it can be applied in EntityScaleHandler
            if(Init.PendingScaleCallbacks.TryGetValue(this.EntityId, out var callback))
            {
                callback(this.Scale);
                Init.PendingScaleCallbacks.Remove(this.EntityId);
                //RZA_Utils.LOD($"NetPkgRZA_Scale Client ProcessPackge. Callback found and invoked for eID:{this.EntityId} scale:{this.Scale}");
            }

            // add key for later if needed

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
