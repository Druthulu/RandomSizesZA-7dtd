using System;
using System.Collections.Generic;
using System.Threading.Tasks;
using XMLData.Parsers;

public static class RZA_Utils
{
	public static bool AllowedEntityTypes(EntityAlive entity)
	{
        List<string> entityTagList = entity.EntityTags.GetTagNames();
        if (entityTagList.Contains("trader") || entityTagList.Contains("player"))
        {
            return false;
        }
        if (entityTagList.Contains("animal"))
        {
            return Init.randomAnimalSizes;
        }
        else if (entityTagList.Contains("zombie"))
        {
            return Init.randomZombieSizes;
        }
        //LOD($"entity tags does not contains animal or zombie :{string.Join(", ", entityTagList)}");
        return false;
	}

    public static float ServerDictSearch(int entityId, float min, float max)
    {
        float scale = 1f;
        if (Init.entityScaleDict.ContainsKey(entityId))
        {
            scale = Init.entityScaleDict[entityId];
            //LOD($"DictSearch. Found existing scale {scale}");
        }
        else
        {
            System.Random random = new System.Random();
            scale = (float)(random.NextDouble() * (max - min) + min);
            //LOD($"DictSearch. Generating new scale {scale}");
            Init.entityScaleDict.Add(entityId, scale);
            //LOD($"DictSearch. Added entityId:{entityId} and scale:{scale} to dictionary");
        }
        return scale;
    }

    public static (float,float, string) MinMax(EntityAlive e)
    {
        float min = 1f;
        float max = 1f;
        string eType = "o";
        List<string> entityTagList = e.EntityTags.GetTagNames();
        if (entityTagList.Contains("zombie"))
        {
            min = Init.zombieMin;
            max = Init.zombieMax;
            eType = "Z"; // so server can override
        }
        else if (entityTagList.Contains("animal"))
        {
            min = Init.animalMin;
            max = Init.animalMax;
            eType = "A";
        }
        return (min, max, eType);
    }

    public static void LOD(string s)
    {
        if (Init.debugMode)
        {
            Log.Out("[RandomSizesZA Debug] {0}", s);
        }
    }
    public static void LO(string s)
    {
        Log.Out("[RandomSizesZA] {0}", s);
    }

    // Notes:

    // .EntityClass.classname        =   EntityZombie or EntityAnimal
    // .EntityClass.entityClassName  =   zombieBusinessMan
    // .entityId                     =   176 
}
