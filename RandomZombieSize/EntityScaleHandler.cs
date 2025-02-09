using UnityEngine;

public class EntityScaleHandler : MonoBehaviour
{
    private EntityAlive entityAlive;
    private bool isScaleApplied = false;
    private float currentScale = 0f;

    void Start()
    {
        // Ensure the MonoBehaviour is attached to an EntityAlive instance
        entityAlive = GetComponent<EntityAlive>();
        if (entityAlive == null)
        {
            RZA_Utils.LOD("EntityScaleHandler requires an EntityAlive component.");
        }
    }

    public void SetScale(float scale)
    {
        currentScale = scale;
        isScaleApplied = false;  // Reset flag to allow scale application
        RZA_Utils.LOD("EntityScaleHandler setScale method.");
    }

    void LateUpdate()
    {
        // Apply scale once after everything else has updated
        if (!isScaleApplied && currentScale != 0f && entityAlive != null)
        {
            if (entityAlive.transform.localScale != new Vector3(currentScale, currentScale, currentScale))
            {
                entityAlive.transform.localScale = new Vector3(currentScale, currentScale, currentScale);
                isScaleApplied = true;  // Flag to prevent re-applying the scale
                RZA_Utils.LOD($"Applied scale {currentScale} to entityId: {entityAlive.entityId}");
            }
        }
    }
}