using UnityEditor;
using UnityEngine;

[CustomEditor(typeof(TopDownCamera))]
public class TopDownCamera_Editor : Editor
{
    #region [Var]
    private TopDownCamera _targetCamera;
    #endregion
    
    public override void OnInspectorGUI()
    {
        _targetCamera = (TopDownCamera)target;
        base.OnInspectorGUI();
    }
    
    private void OnSceneGUI()
    {
        // Make sure we have a target first!
        if (!_targetCamera || !_targetCamera.target)
            return;

        // Storing target reference
        Transform camTarget = _targetCamera.target;
        Vector3 targetPosition = camTarget.position;
        targetPosition.y += _targetCamera.lookAtHeight;

        // Draw distance circle
        Handles.color = new Color(1f, 0f, 0f, 0.15f);
        Handles.DrawSolidDisc(targetPosition, Vector3.up, _targetCamera.distance);

        Handles.color = new Color(0f, 1f, 0f, 0.75f);
        Handles.DrawWireDisc(targetPosition, Vector3.up, _targetCamera.distance);

        // Create slider handles to adjust camera properties
        Handles.color = new Color(1f, 0f, 0f, 0.5f);
        _targetCamera.distance = Handles.ScaleSlider(_targetCamera.distance, targetPosition, 
            -camTarget.forward, Quaternion.identity, _targetCamera.distance, 0.1f);
        _targetCamera.distance = Mathf.Clamp(_targetCamera.distance, 2f, float.MaxValue);

        Handles.color = new Color(0f, 0f, 1f, 0.5f);
        _targetCamera.height = Handles.ScaleSlider(_targetCamera.height, targetPosition, 
            Vector3.up, Quaternion.identity, _targetCamera.height, 0.1f);
        _targetCamera.height = Mathf.Clamp(_targetCamera.height, 2f, float.MaxValue);

        // Create Labels
        GUIStyle labelStyle = new GUIStyle();
        labelStyle.fontSize = 15;
        labelStyle.normal.textColor = Color.white;
        labelStyle.alignment = TextAnchor.UpperCenter;

        Handles.Label(targetPosition + (-camTarget.forward * _targetCamera.distance), "Distance", labelStyle);

        labelStyle.alignment = TextAnchor.MiddleRight;
        Handles.Label(targetPosition + (Vector3.up * _targetCamera.height), "Height", labelStyle);

        _targetCamera.HandleCamera();
    }
}
