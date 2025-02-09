using System.Collections.Generic;
using static EAIBlockIf;

public class NetPkgRZA_DictUpdate : NetPackage
{
    public int EntityId = 0;

    public NetPkgRZA_DictUpdate ToClient(int entityId)
    {
        this.EntityId = entityId;
        RZA_Utils.LOD($"NetPkgRZA_DictUpdate ToClient eID:{entityId}");
        return this;
    }

    public override void read(PooledBinaryReader _br)
    {
        this.EntityId = _br.ReadInt32();
        RZA_Utils.LOD("NetPkgRZA_DictUpdate Read");
    }

    public override void write(PooledBinaryWriter _bw)
    {
        base.write(_bw);
        _bw.Write(this.EntityId);
        RZA_Utils.LOD("NetPkgRZA_DictUpdate Write");
    }

    public override void ProcessPackage(World _world, GameManager _callbacks)
    {
        RZA_Utils.LOD("NetPkgRZA_DictUpdate ProcessPackge start");
        if (_world == null) return;
        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            // server wont see this I suppose
            RZA_Utils.LOD("NetPkgRZA_DictUpdate serverside, nothing to do");
        }
        else
        {
            RZA_Utils.LOD($"NetPkgRZA_DictUpdate Client ProcessPackge. start. eID:{this.EntityId}");
            if (Init.entityScaleDict.ContainsKey(this.EntityId))
            {
                //RZA_Utils.LOD($"PKG ProcessPackge. client eID:{this.EntityId} scale:{this.Scale}");
                Init.entityScaleDict.Remove(this.EntityId);
                RZA_Utils.LOD($"NetPkgRZA_DictUpdate Client ProcessPackge. done. eID:{this.EntityId}");
            }
        }
    }

    public override int GetLength() => 4; // (4byte Int32)
}
