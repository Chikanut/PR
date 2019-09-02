using UnityEngine;

public class Player : MonoBehaviour
{
    [Header("Test")] [SerializeField] bool _canDie;
    
    [Header("Components")]
    [SerializeField] private MeshRenderer _renderer;
    [SerializeField] private Material _material;

    private bool _dead = false;

    Color _color;

    private void OnCollisionStay(Collision other)
    {
        OnCollisionEnter(other);
    }
    
    private void OnCollisionEnter(Collision other)
    {
        var cell = other.gameObject.GetComponent<CellPoolObject>();

        if (cell && !cell.OnSameColor(_color))
        {
            GameManager.Instance.OnDeath(); 
        }
    }

    public void SetColor(Color c)
    {
        if (!Application.isPlaying)
        {
            var tempMaterial = new Material(_material) {color = c};
            _renderer.sharedMaterial = tempMaterial;
        }
        else
            _renderer.material.color = c;
        
        _color = c;
    }

    void OnDeath()
    {
//        _dead = true;
//        var particels = GameManager.instance._objectsPool.GetObjectOfType<Particles>(ObjectType.ClusterExplosionRed);
//        particels.transform.position = transform.position;
//        _view.gameObject.SetActive(false);
    }
}
