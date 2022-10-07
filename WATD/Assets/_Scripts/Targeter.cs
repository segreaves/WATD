using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.AI;

public class Targeter : MonoBehaviour
{
    [SerializeField] private string Tag;
    [field: SerializeField] public SphereCollider TargetCollider { get; private set; }
    public List<Target> targets = new List<Target>();
    

    private void OnTriggerEnter(Collider other)
    {
        if (other.transform.root.gameObject == transform.root.gameObject) { return; }
        if (!other.transform.root.gameObject.CompareTag(Tag)) { return; }
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Add(target);
        target.OnDestroyedEvent += RemoveTarget;
    }

    private void OnTriggerExit(Collider other)
    {
        if (!other.TryGetComponent<Target>(out Target target)) { return; }
        targets.Remove(target);
    }

    public void RemoveTarget(Target target)
    {
        target.OnDestroyedEvent -= RemoveTarget;
        targets.Remove(target);
    }
}
