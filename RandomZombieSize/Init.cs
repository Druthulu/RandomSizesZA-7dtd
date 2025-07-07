using System;
using System.Collections.Generic;
using System.Reflection;
using System.Linq;
using System.Xml;
using HarmonyLib;
using static vp_Message;
using static EAIBlockIf;


public class Init : IModApi
{
    public static string modsFolderPath;
    public static bool randomZombieSizes = true;
    public static bool randomAnimalSizes = true;
    public static float zombieMin = 0.75f;
    public static float zombieMax = 1.25f;
    public static float animalMin = 0.75f;
    public static float animalMax = 1.25f;
    public static Dictionary<int, float> entityScaleDict = new Dictionary<int, float>();
    public static bool debugMode = false;

    public void InitMod(Mod _modInstance)
	{
        modsFolderPath = _modInstance.Path;
        ReadXML();
        Log.Out(" Loading Patch: " + base.GetType().ToString());
		Harmony harmony = new Harmony(base.GetType().ToString());
		harmony.PatchAll(Assembly.GetExecutingAssembly());
        ModEvents.EntityKilled.RegisterHandler(this.EntityKilled);
        ModEvents.PlayerSpawnedInWorld.RegisterHandler(this.PlayerSpawnedInWorld);
    }

    public void ReadXML()
    {
        RZA_Utils.LO($"Reading prefs in {Init.modsFolderPath}\\settings.xml");
        using (XmlReader xmlReader = XmlReader.Create(Init.modsFolderPath + "\\settings.xml"))
        {
            while (xmlReader.Read())
            {
                if (xmlReader.NodeType == XmlNodeType.Element)
                {
                    if (xmlReader.Name.ToString() == "randomZombieSizes")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!bool.TryParse(temp, out randomZombieSizes))
                        {
                            RZA_Utils.LO($"failed to parse randomZombieSizes, using default value of {randomZombieSizes}");
                        }
                    }
                    if (xmlReader.Name.ToString() == "randomAnimalSizes")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!bool.TryParse(temp, out randomAnimalSizes))
                        {
                            RZA_Utils.LO($"failed to parse randomAnimalSizes, using default value of {randomAnimalSizes}");
                        }
                    }
                    if (xmlReader.Name.ToString() == "zombieMin")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!float.TryParse(temp, out zombieMin))
                        {
                            RZA_Utils.LO($"failed to parse zombieMin, using default value of {zombieMin}");
                        }
                    }
                    if (xmlReader.Name.ToString() == "zombieMax")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!float.TryParse(temp, out zombieMax))
                        {
                            RZA_Utils.LO($"failed to parse zombieMax, using default value of {zombieMax}");
                        }
                    }
                    if (xmlReader.Name.ToString() == "animalMin")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!float.TryParse(temp, out animalMin))
                        {
                            RZA_Utils.LO($"failed to parse animalMin, using default value of {animalMin}");
                        }   
                    }
                    if (xmlReader.Name.ToString() == "animalMax")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!float.TryParse(temp, out animalMax))
                        {
                            RZA_Utils.LO($"failed to parse animalMax, using default value of {animalMax}");
                        }
                    }
                    if (xmlReader.Name.ToString() == "debugMode")
                    {
                        string temp = xmlReader.ReadElementContentAsString();
                        if (!bool.TryParse(temp, out debugMode))
                        {
                            RZA_Utils.LO($"failed to parse debugMode, using default value of {debugMode}");
                        }
                    }
                }
            }
        }
        if (animalMin > animalMax)
        {
            RZA_Utils.LO("ERR: animalMin is greater than animalMax, using defaults of 0.5 to 1.5");
            animalMin = 0.75f;
            animalMax = 1.25f;
        }
        if (zombieMin > zombieMax)
        {
            RZA_Utils.LO("ERR: zombieMin is greater than zombieMax, using defaults of 0.5 to 1.5");
            zombieMin = 0.75f;
            zombieMax = 1.25f;
        }
        if (animalMin < 0f || animalMax < 0f)
        {
            RZA_Utils.LO("ERR: animalMin|animalMax must be greater than 0, using defaults of 0.5 to 1.5");
            animalMin = 0.75f;
            animalMax = 1.25f;
        }
        if (zombieMin < 0f || zombieMax < 0f)
        {
            RZA_Utils.LO("ERR: zombieMin|zombieMax must be greater than 0, using defaults of 0.5 to 1.5");
            zombieMin = 0.75f;
            zombieMax = 1.25f;
        }
    }

    public void EntityKilled(ref ModEvents.SEntityKilledData data)
    {
        var entityKilled = data.KilledEntitiy;
        // remove entity from dictionary since it wont be needed anymore
        entityScaleDict.Remove(entityKilled.entityId);
        //RZA_Utils.LOD($"Entity Killed Event. eID:{entityKilled.entityId} Entity removed from Dict");


        // send net pkg to client to remove from dictionary as well
        SingletonMonoBehaviour<ConnectionManager>.Instance.SendToClientsOrServer(
                        NetPackageManager.GetPackage<NetPkgRZA_DictUpdate>()
                            .ToClient(entityKilled.entityId));

    }

    public void PlayerSpawnedInWorld(ref ModEvents.SPlayerSpawnedInWorldData data)
    //public void PlayerSpawnedInWorld(ClientInfo cInfo, RespawnType respawnReason, Vector3i pos)
    {
        var respawnReason = data.RespawnType;

        //RZA_Utils.LOD("Event PlayerSpawnedInWorld - Syncing Dictionary");
        if (respawnReason == RespawnType.EnterMultiplayer || respawnReason == RespawnType.JoinMultiplayer)
        {
            // send net pkg of current server dictionary to sync client side dictionary
            SingletonMonoBehaviour<ConnectionManager>.Instance.SendToClientsOrServer(
                        NetPackageManager.GetPackage<NetPkgRZA_DictSync>()
                            .ToClient(entityScaleDict));
        }
    }
}
