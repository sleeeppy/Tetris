using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CSParent : MonoBehaviour
{
    public int csValue = 300;
    // Start is called before the first frame update
    void Start()
    {
        int theValue1 = GetComponentInChildren<CSParent>().csValue;
        int theValue2 = GetComponentInChildren<CSChild>().csValue;
        int theValue3 = GetComponentInChildren<CSGrandChild>().csValue;
        Debug.Log($"value1 = {theValue1} / value2 = {theValue2} / value3 = {theValue3}");
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
