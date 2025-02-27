using System.Collections.Generic;
using static EAIBlockIf;

public class NetPkgRZA_DictSync : NetPackage
{
    public int Count = 0;
    public Dictionary<int, float> EntityScaleDict = new Dictionary<int, float>();

    public NetPkgRZA_DictSync ToClient(Dictionary<int, float> entityScaleDict2)
    {
        this.Count = entityScaleDict2.Count;
        this.EntityScaleDict = entityScaleDict2;
        //RZA_Utils.LOD($"NetPkgRZA_DictSync ToClient count:{this.Count} expected size:{this.Count *4 + this.Count *4} bytes");
        return this;
    }

    public override void read(PooledBinaryReader _br)
    {
        this.Count = _br.ReadInt32();
        for (int i = 0; i < this.Count; i++)
        {
            int key = _br.ReadInt32();
            float value = _br.ReadSingle();
            this.EntityScaleDict.Add(key, value);
        }
        //RZA_Utils.LOD($"NetPkgRZA_DictSync Read count:{this.Count} result size:{this.Count * 4 + this.Count * 4} bytes");
    }

    public override void write(PooledBinaryWriter _bw)
    {
        base.write(_bw);
        _bw.Write(this.Count);
        foreach (var kvp in this.EntityScaleDict)
        {
            _bw.Write(kvp.Key);
            _bw.Write(kvp.Value);
        }
        //RZA_Utils.LOD($"NetPkgRZA_DictSync Write count:{this.Count} result size:{this.Count * 4 + this.Count * 4} bytes");
    }

    public override void ProcessPackage(World _world, GameManager _callbacks)
    {
        //RZA_Utils.LOD("NetPkgRZA_DictSync ProcessPackage start");
        if (_world == null) return;
        if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
        {
            // server wont see this I suppose
            //RZA_Utils.LOD("NetPkgRZA_DictSync serverside, nothing to do");
        }
        else
        {
            //RZA_Utils.LOD($"NetPkgRZA_DictSync Client ProcessPackge. recieved count:{this.Count}");
            Init.entityScaleDict.Clear();
            foreach (var kvp in this.EntityScaleDict)
            {
                Init.entityScaleDict.Add(kvp.Key, kvp.Value);
            }
            //RZA_Utils.LOD($"NetPkgRZA_DictSync Client ProcessPackge. Synced Dictionary count:{Init.entityScaleDict.Count}");
        }
    }

    public override int GetLength() => (4 + (this.EntityScaleDict.Count * 8)); // (4byte Int32 of count) + ((4byte Int32 + 4byte Single) *8)
}
