using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ClearCounter : MonoBehaviour {

    [SerializeField] private Transform tomatoPerfab;
    [SerializeField] private Transform counterTopPoint;

    public void Interact() {
        
        Transform tomatoTransform = Instantiate(tomatoPerfab, counterTopPoint);
        tomatoTransform.localPosition = Vector3.zero;
    }
}
