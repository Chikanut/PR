using UnityEngine;

public class CameraController : MonoBehaviour, Resetable
{

    [SerializeField] float _rotationDamping;
    [Range(-1,1)]
    [SerializeField] float _verticalDirection;
    [SerializeField] Progression _speedProgression;

    bool _movementEnabled = false;
    
    private void LateUpdate()
    {
        transform.rotation = Quaternion.Lerp(transform.rotation,
            Quaternion.LookRotation( Vector3.forward +
                Vector3.Lerp(Vector3.down, Vector3.up, (_verticalDirection + 1f) / 2f),
                Vector3.forward),
            _rotationDamping * Time.deltaTime);
        
        if(!_movementEnabled)
            return;
        
        _speedProgression.Update(Time.deltaTime);
        
        var speed = _speedProgression.Evaluate(); 

        transform.position += Vector3.forward * (speed * Time.fixedDeltaTime);
    }

    public void OnReset(GameState state)
    {
        _movementEnabled = state == GameState.game;
    }
}
