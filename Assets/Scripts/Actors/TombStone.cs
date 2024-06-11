using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TombStone : MonoBehaviour
{
    // Start is called before the first frame update
    void Start()
    {
        GameManager.Get.AddTombStone(this);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
