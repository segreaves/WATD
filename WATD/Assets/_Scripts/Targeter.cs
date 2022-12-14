using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Targeter : MonoBehaviour
{
    [field: SerializeField] public SphereCollider TargetCollider { get; private set; }
    public List<Target> targets = new List<Target>();
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject == transform.root.gameObject) { return; }
        if (!other.gameObject.CompareTag(tag)) { return; }
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Add(target);
        target.OnRemoveTarget += RemoveTarget;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Remove(target);
    }

    public void RemoveTarget(Target target)
    {
        target.OnRemoveTarget -= RemoveTarget;
        targets.Remove(target);
    }
}
