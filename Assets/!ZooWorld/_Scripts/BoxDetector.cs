using System;
using UnityEngine;

public class BoxDetector : MonoBehaviour
{
    [SerializeField] private Vector3 boxSize = new Vector3(1, 1, 1); // Size of the box
    [SerializeField] private Vector3 boxOffset = Vector3.zero; // Local offset of the box
    public LayerMask layerMask = Physics.DefaultRaycastLayers; // Detect all by default
    public bool showInEditor = true; // Toggle to enable/disable visualization in edit mode
    private Action<Collider> onDetect;
    [SerializeField] private Collider[] ignoreColliders; // Colliders to ignore

    public void Initialize(Action<Collider> onDetectTrigger)
    {
        onDetect = onDetectTrigger;
    }

    void Update()
    {
        Collider collider = CheckForObstacle();

        if (collider != null)
        {
            onDetect?.Invoke(collider);
        }
    }

    private Collider CheckForObstacle()
    {
        Vector3 boxCenter = transform.position + transform.TransformVector(boxOffset);

        // Get all colliders within the box
        Collider[] hits = Physics.OverlapBox(boxCenter, boxSize / 2, transform.rotation, layerMask);

        // Filter out ignored colliders
        foreach (var hit in hits)
        {
            if (!ShouldIgnoreCollider(hit))
            {
                return hit; // Return the first valid collider
            }
        }

        return null; // No valid collider found
    }

    private bool ShouldIgnoreCollider(Collider collider)
    {
        // Check if the collider is in the ignore list
        foreach (var ignored in ignoreColliders)
        {
            if (collider == ignored)
            {
                return true;
            }
        }

        return false;
    }

    private void OnDrawGizmos()
    {
        if (!showInEditor) return;

        Vector3 boxCenter = transform.position + transform.TransformVector(boxOffset);

        DrawWireBox(boxCenter, transform.rotation, boxSize, Color.yellow);
    }

    private void DrawWireBox(Vector3 center, Quaternion rotation, Vector3 size, Color color)
    {
        Gizmos.color = color;
        Gizmos.matrix = Matrix4x4.TRS(center, rotation, Vector3.one);
        Gizmos.DrawWireCube(Vector3.zero, size);
    }
}
