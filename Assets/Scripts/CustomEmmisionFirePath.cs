using System;
using UnityEngine;

public class CustomEmmisionFirePath : MonoBehaviour
{
    [SerializeField] private CharacterBase _characterBase;
    [SerializeField] private ParticleSystem[] _particleSystems;
    
    private Vector3 _targetPoint; // The target transform to measure the distance from
    public float rateOverDistance = 10f; // The rate of emission per unit of distance

    private float lastEmissionTime; // The time at which the last particle was emitted

    private bool _isMove;

    private void Awake()
    {
        _characterBase.OnStartMove += MoveToTarget;
        _characterBase.OnStopMove += StopMove;
    }

    public void MoveToTarget(Vector3 targetPoint)
    {
        _targetPoint = targetPoint;
        _isMove = true;
    }

    public void StopMove()
    {
        _isMove = false;
    }
    
    void Update()
    {
        if (!_isMove)
            return;
        // Calculate the distance between this object and the target object
        float distanceToTarget = Vector3.Distance(transform.position, _targetPoint);

        // Calculate the emission rate based on the distance
        float emissionRate = distanceToTarget * rateOverDistance;

        // Calculate the time since the last particle was emitted
        float timeSinceLastEmission = Time.time - lastEmissionTime;

        // If enough time has passed since the last emission, emit particles based on the emission rate
        if (timeSinceLastEmission >= 1f / emissionRate)
        {
            foreach (ParticleSystem system in _particleSystems)
                system.Emit(1);
            lastEmissionTime = Time.time;
        }
    }
}
