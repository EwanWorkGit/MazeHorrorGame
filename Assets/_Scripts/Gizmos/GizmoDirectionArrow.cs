using UnityEngine;

#if UNITY_EDITOR
using UnityEditor;
#endif

public class GizmoDirectionArrow : MonoBehaviour
{
    [SerializeField] float Length = 20f;

#if UNITY_EDITOR
    private void OnDrawGizmos()
    {
        Handles.color = Color.red;
        Vector3 startPosition = transform.position;
        Vector3 direction = transform.forward;

        Handles.ArrowHandleCap(0, startPosition, Quaternion.LookRotation(direction), Length, EventType.Repaint);
    }
#endif
}
