using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TowerWeaponSlave : MonoBehaviour
{
    [SerializeField]
    private TowerWeapon m_towerWeapon;
    public TowerWeapon towerWeapon => m_towerWeapon;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    private void OnTriggerEnter2D(Collider2D collision)
    {
        towerWeapon.OnTriggerEnter2DAction(collision);
    }

    private void OnTriggerExit2D(Collider2D collision)
    {
        towerWeapon.OnTriggerExit2DAction(collision);
    }
}
