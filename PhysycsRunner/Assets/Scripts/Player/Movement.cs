using UnityEngine;

public class Movement : MonoBehaviour
{
    private bool _enabliPriv = true;

    public bool _enable
    {
        get { return _enabliPriv; }
        set
        {
            if (value != _enabliPriv)
            {
                if (Input.touchCount > 0)
                    _prevInputPos = Input.GetTouch(0).position;
                else
                    _prevInputPos = Input.mousePosition;
            }

            _enabliPriv = value;
        }
    }

    [Header("Components")]
    [SerializeField] Transform _anchor;

    [Header("Settings")]
    [SerializeField] Vector2 _movementZoneSize;
    [SerializeField] Vector3 _movementZoneOffset;
    [SerializeField] private bool _invert;
    [SerializeField] private float _inputSense;
    [SerializeField] private float _inputDamp;

    private Vector2 _prevInputPos;
    private float _speedX;
    float _speedY;
    private float velocityHorizontal;
    float velocityVertical;

    private bool _pressed = false;
    
    void Update()
    {
        if(!_enable)
            return;
        
        OnFingerInput();
    }

    void OnFingerInput()
    {
        if (Input.GetMouseButtonDown(0) || (Input.touchCount > 0 && !_pressed))
        {
            _prevInputPos = Input.mousePosition;
            _pressed = true;
        }
#if UNITY_EDITOR
        if (Input.GetMouseButton(0))
            CalculateTouch(Input.mousePosition);
        else
            Damping();
#elif UNITY_ANDROID
        
        if (Input.touchCount == 0)
            _pressed = false;

        if (_pressed)
        {
            var touchPos = Input.touches[0].position;

            CalculateTouch(touchPos);
        }
        else
            Damping();
#endif
    }

    void CalculateTouch(Vector2 touch)
    {
        var distanceX = (touch.x - _prevInputPos.x) / Screen.width;
        var distanceY = (touch.y - _prevInputPos.y) / Screen.height;
            
        _speedX = Mathf.SmoothDamp(_speedX, distanceX * _inputSense, ref velocityHorizontal,
            _inputDamp * Time.deltaTime);
        _speedY = Mathf.SmoothDamp(_speedY, distanceY * _inputSense, ref velocityVertical,
            _inputDamp * Time.deltaTime);
            
        var speed = new Vector3(_speedX,0,_speedY);
            
        Move(speed);

        _prevInputPos = touch;
    }

    void Damping()
    {
        _speedX = Mathf.SmoothDamp(_speedX, 0, ref velocityHorizontal,
            _inputDamp * Time.deltaTime);
        _speedY = Mathf.SmoothDamp(_speedY, 0, ref velocityVertical,
            _inputDamp * Time.deltaTime);
            
        var speed = new Vector3(_speedX,0,_speedY);
            
        Move(speed);
    }

    void Move(Vector3 speed)
    {
        if (_invert)
            speed *= -1;
        
        var newPos = _anchor.position;
        newPos += speed;

        newPos.x = Mathf.Clamp(newPos.x, 
            transform.position.x + _movementZoneOffset.x - _movementZoneSize.x / 2,
            transform.position.x + _movementZoneOffset.x + _movementZoneSize.x / 2);
        newPos.z = Mathf.Clamp(newPos.z, 
            transform.position.z + _movementZoneOffset.z - _movementZoneSize.y / 2,
            transform.position.z + _movementZoneOffset.z + _movementZoneSize.y / 2);

        _anchor.position = newPos;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = Color.red;

        Gizmos.DrawCube(transform.TransformPoint(_movementZoneOffset),
            new Vector3(_movementZoneSize.x, 0, _movementZoneSize.y));
    }
}
