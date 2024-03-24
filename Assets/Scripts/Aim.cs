using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Photon.Pun;
using UnityEngine.InputSystem;
using System.IO;

public class Aim : MonoBehaviour
{
    [SerializeField] private Transform spawnPosition;
    [SerializeField] private List<GameObject> allTarget;
    [SerializeField] private GameObject targetCylinder;
    [SerializeField] private float range;
    private PlayerInput inputs;
    private PhotonView pv;
    private CharacterController controller;
    private GameObject targetObj;
    private bool canSearch = true;
    private int targetCount;

    private void Awake()
    {
        inputs = new PlayerInput();
        controller = GetComponent<CharacterController>();
        pv = GetComponentInParent<PhotonView>();
    }

    private void Start()
    {
        if (!pv.IsMine) return;
        targetCylinder.SetActive(false);
        inputs.CharacterControls.Fire.started += OnFire;
        inputs.CharacterControls.ChangeTarget.started += SelectNewTarget;
    }

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, range);
    }

    public void SetTargetStatus(bool isTarget)
    {
        targetCylinder.SetActive(isTarget);
    }

    private void SelectTarget()
    {
        if(controller.velocity == Vector3.zero)
        {
            if (canSearch) InvokeRepeating("Calculate", 0f, 0.5f);
        }
        else
        {
            if(targetObj != null)
            {
                targetObj.GetComponent<Aim>().SetTargetStatus(false);
                targetObj = null;
            }
            canSearch = true;
            CancelInvoke();
        }
    }

    private void Calculate()
    {
        canSearch = false;
        allTarget.Clear();
        RaycastHit[] hits = Physics.SphereCastAll(transform.position, range,
            transform.position, range);
        foreach (RaycastHit hit in hits)
        {
            GameObject tempObj = hit.collider.gameObject;
            if (tempObj.GetComponent<CharacterController>() && !tempObj.GetComponentInParent<PhotonView>().IsMine)
            {
                allTarget.Add(tempObj);
            }
            else continue;
        }
        if(allTarget.Count == 0) return;
        SelectNewTarget();
    }

    private void SelectNewTarget()
    {
        foreach (GameObject obj in allTarget)
        {
            obj.GetComponent<Aim>().SetTargetStatus(false);
        }
        if(targetCount >= allTarget.Count)
        {
            targetCount = 0;
        }
        targetObj = allTarget[targetCount];
        allTarget[targetCount].GetComponent<Aim>().SetTargetStatus(true);
    }

    private void SelectNewTarget(InputAction.CallbackContext context)
    {
        targetCount++;
        foreach (GameObject obj in allTarget)
        {
            obj.GetComponent<Aim>().SetTargetStatus(false);
        }
        if (targetCount >= allTarget.Count)
        {
            targetCount = 0;
        }
        targetObj = allTarget[targetCount];
        allTarget[targetCount].GetComponent<Aim>().SetTargetStatus(true);
    }

    private void FixedUpdate()
    {
        if (!pv.IsMine) return;
        SelectTarget();
    }

    private void OnFire(InputAction.CallbackContext context)
    {
        if(targetObj != null)
        {
            Vector3 dir = (targetObj.transform.position - transform.position).normalized;
            GameObject temp = PhotonNetwork.Instantiate(Path.Combine("Fireball"), 
                spawnPosition.position, Quaternion.identity);
            temp.GetComponent<Bullet>().StartMove(dir);
            Physics.IgnoreCollision(temp.GetComponent<Collider>(), transform.GetComponent<Collider>());
        }
    }

}
