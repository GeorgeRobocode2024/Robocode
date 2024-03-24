using UnityEngine;
using Photon.Pun;

public class Bullet : MonoBehaviour
{
    [SerializeField] private BulletInfo info;
    private Rigidbody _rb;
    private PhotonView _pv;

    private void Awake()
    {
        _rb = GetComponent<Rigidbody>();
        _pv = GetComponent<PhotonView>();
        info.render = gameObject; 
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!_pv.IsMine) return;
        if(other.gameObject.CompareTag("Player"))
        {
            other.gameObject.GetComponentInParent<PlayerSetting>().TakeDamage(info.damage);
            PhotonNetwork.Destroy(gameObject);
        }
    }

    public void StartMove(Vector3 dir)
    {
        _rb.velocity = dir * info.speed;
    }

}
