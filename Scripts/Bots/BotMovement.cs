using System;
using UnityEngine;

public class BotMovement : MonoBehaviour
{
    [SerializeField] private float _speed;

    private Transform _target;
    private bool _canMove;

    public event Action OnReachTarget;

    private void Update()
    {
        if (_canMove)
            Move();
    }

    public void Stop()
    {
        _canMove = false;
    }

    public void Move()
    {
        if (_target == null) return;

        transform.position = Vector3.MoveTowards(transform.position, _target.position, _speed * Time.deltaTime);

        if(Vector3.Distance(transform.position, _target.position) < 0.1f)
        {
            Stop();
            OnReachTarget?.Invoke();
        }
    }

    public void SetTarget(Transform target)
    {
        _canMove = true;
        _target = target;
        transform.LookAt(target);
        transform.rotation = Quaternion.Euler(0, transform.rotation.eulerAngles.y, 0);
    }
}