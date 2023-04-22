using System.Collections;
using System.Collections.Generic;
using UnityEngine;

/// <summary>
/// 視陽角、視野半径、障害物に遮られていないもの
/// </summary>
public class FieldOfView : MonoBehaviour
    {
        #region [Var]
        [Header("Sight Settings")] public float viewRadius = 5f;
        [Range(0, 360)] public float viewAngle = 90f;
        [Header("Find Settings")] public float delay = 0.2f;

        public LayerMask targetMask;
        public LayerMask obstacleMask;

        private List<Transform> _visibleTargets = new List<Transform>();
        private Transform _nearestTarget;
        private float _distanceToTarget = 0.0f;
        #endregion

        #region [Properties]
        public List<Transform> VisibleTargets => _visibleTargets;
        public Transform NearestTarget => _nearestTarget;
        public float DistanceToTarget => _distanceToTarget;
        #endregion

        #region [Unity Methods]
        void Start() => StartCoroutine("FindTargetsWithDelay", delay);

        #endregion

        #region [Logic Methods]
        private IEnumerator FindTargetsWithDelay(float delay)
        {
            while (true)
            {
                yield return new WaitForSeconds(delay);
                FindVisibleTargets();
            }
        }

        private void FindVisibleTargets()
        {
            _distanceToTarget = 0.0f;
            _nearestTarget = null;
            _visibleTargets.Clear();

            Collider[] targetsInViewRadius = Physics.OverlapSphere(transform.position, viewRadius, targetMask);

            for (int i = 0; i < targetsInViewRadius.Length; i++)
            {
                Transform target = targetsInViewRadius[i].transform;

                Vector3 dirToTarget = (target.position - transform.position).normalized;
                if (Vector3.Angle(transform.forward, dirToTarget) < viewAngle / 2)
                {
                    float dstToTarget = Vector3.Distance(transform.position, target.position);

                    if (!Physics.Raycast(transform.position, dirToTarget, dstToTarget, obstacleMask))
                    {
                        _visibleTargets.Add(target);

                        if (_nearestTarget == null || (_distanceToTarget > dstToTarget))
                            _nearestTarget = target;

                        _distanceToTarget = dstToTarget;
                    }
                }
            }
        }

        public Vector3 DirFromAngle(float angleInDegrees, bool angleIsGlobal)
        {
            if (!angleIsGlobal)
            {
                angleInDegrees += transform.eulerAngles.y;
            }
            return new Vector3(Mathf.Sin(angleInDegrees * Mathf.Deg2Rad), 0, Mathf.Cos(angleInDegrees * Mathf.Deg2Rad));
        }
        #endregion
    }
