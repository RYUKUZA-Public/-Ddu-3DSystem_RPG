using UnityEngine;

public class BaseCamera : MonoBehaviour
{
    #region [Var]
    public Transform target;
    #endregion

    #region [Main Methods]
    void Start() => HandleCamera();

    private void LateUpdate() => HandleCamera();
    #endregion

    public virtual void HandleCamera() { }
}
