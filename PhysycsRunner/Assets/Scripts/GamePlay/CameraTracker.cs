using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraTracker : MonoBehaviour
{
    [SerializeField] private bool _X;
    [SerializeField] private bool _Y;
    [SerializeField] private bool _Z;

    [SerializeField] private Transform _camera;

    void Update()
    {
        transform.position = new Vector3(_X ? _camera.position.x : transform.position.x,
            _Y ? _camera.position.y : transform.position.y,
            _Z ? _camera.position.z : transform.position.z);
    }
}
