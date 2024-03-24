using Photon.Pun;
using UnityEngine;
using UnityEngine.UI;

public class PlayerSetting : MonoBehaviourPunCallbacks
{
    [SerializeField] private int health;
    [SerializeField] private int maxHealth;
    [SerializeField] private Slider healthBar;
    private PhotonView _pv;

    private void Awake()
    {
        _pv = GetComponent<PhotonView>();
    }

    private void Start()
    {
        health = maxHealth;
        healthBar.value = health;
    }

    public void TakeDamage(int value)
    {
        _pv.RPC("UpdateHealth", RpcTarget.All, value);
    }

    [PunRPC]
    public void UpdateHealth(int value)
    {
        health -= value;
        if(health <= 0)
        {
            health = maxHealth;
            transform.GetComponentInChildren<PlayerController>().Respawn();
        }
        healthBar.value = health;
    }

}
