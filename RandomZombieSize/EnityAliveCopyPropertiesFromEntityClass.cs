using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using System.Threading.Tasks;
using HarmonyLib;
using UnityEngine;
//using static EntityVehicle.RemoteData;

[HarmonyPatch(typeof(EntityAlive))]
[HarmonyPatch("CopyPropertiesFromEntityClass")]
public class EnityAliveCopyPropertiesFromEntityClass
{
    public static async void Postfix(EntityAlive __instance)
    {
        if (RZA_Utils.AllowedEntityTypes(__instance))
        {
            EntityScaleHandler scaleHandler = __instance.GetComponent<EntityScaleHandler>();
            if (scaleHandler == null)
            {
                // Attach the EntityScaleHandler MonoBehaviour to the GameObject if not already attached
                scaleHandler = __instance.gameObject.AddComponent<EntityScaleHandler>();
            }

            (float min, float max, string eType) = RZA_Utils.MinMax(__instance);
            bool entityFoundInDict = false;
            int checkDictAttempts = 0;
            int maxRetries = 200; // max of 2 seconds

            if (SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                // Server checks dict and generates scale as needed
                RZA_Utils.ServerDictSearch(__instance.entityId, min, max);
                entityFoundInDict = true;
            }
            else if (!SingletonMonoBehaviour<ConnectionManager>.Instance.IsServer)
            {
                // Client search local dict and skip net pkg if possible
                if (!Init.entityScaleDict.ContainsKey(__instance.entityId)) //!
                {
                    Init.PendingScaleCallbacks[__instance.entityId] = (scale) =>
                    {
                        if (scaleHandler != null)
                            scaleHandler.SetScale(scale);
                    };

                    SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(
                        NetPackageManager.GetPackage<NetPkgRZA_Scale>()
                            .ToServer(__instance.entityId, eType));
                    return; // Exit early, callback will handle scale application

                    //RZA_Utils.LOD($"Main Postfix client found key in dict, no net pkg needed");
                    //entityFoundInDict = true;
                }
                /*
                else
                {
                    // if key not in dict, send net pkg
                    //RZA_Utils.LOD($"Main Postfix. client failed to find key in dict, sent data to server {__instance.entityId} {eType}");
                    SingletonMonoBehaviour<ConnectionManager>.Instance.SendToServer(
                        NetPackageManager.GetPackage<NetPkgRZA_Scale>()
                            .ToServer(__instance.entityId, eType));
                }*/
            }
            /* race condition
            while (checkDictAttempts < maxRetries && !entityFoundInDict && __instance != null) //added null reff check
            {
                checkDictAttempts++;
                //RZA_Utils.LOD($"Postfix awaiting NetOkg update Attempt #{checkDictAttempts}");

                if (__instance != null) // added null reff check
                {
                    entityFoundInDict = Init.entityScaleDict.ContainsKey(__instance.entityId);
                }
                if (entityFoundInDict)
                {
                    //RZA_Utils.LOD($"Postfix awaiting NetOkg update Found:{entityFoundInDict}");
                    break;
                }
                if (checkDictAttempts >= maxRetries)
                {
                    //RZA_Utils.LOD($"Postfix awaiting NetOkg update, failed on attempt #{checkDictAttempts}");
                    break;
                }
                await Task.Delay(10); // Delay for 1/1000 seconds before retrying
            }
            */
            // this should never get ran since the net pkg callback should apply the scale immediately, but just in case, this will apply the scale if the dict update comes through after the timeout or if the callback fails for some reason.
            if (entityFoundInDict && __instance != null)
            {
                try
                {
                    float entityScale = Init.entityScaleDict[__instance.entityId];

                    scaleHandler.SetScale(entityScale);

                    //RZA_Utils.LOD($"Applied localScale {entityScale} to entityId: {__instance.entityId} entityName: {__instance.EntityClass.entityClassName}");
                    //RZA_Utils.LOD($"Reading localScale: {__instance.transform.localScale} from entityId: {__instance.entityId} entityName: {__instance.EntityClass.entityClassName}");
                }
                catch
                {
                    //RZA_Utils.LOD($"Error applying localScale, __instance is null?:{(__instance == null)}");
                }
            }
        }
    }
}
