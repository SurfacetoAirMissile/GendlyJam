using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CostComponent : MonoBehaviour
{
    public int price;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {

    }

    public void OnPlacement()
    {
        GameManager.Instance.DeductCredits(price);
    }
}
