using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedCounterVisual : MonoBehaviour {

    [SerializeField] private BaseCounter counter;
    [SerializeField] private GameObject[] visualGameObjects;

    private void Start() {
        Player.Instance.OnSelectedCounterChanged += Player_OnSelectedCounterChanged;
    }

    private void Player_OnSelectedCounterChanged(object sender, Player.OnSelectedCounterChangedEventArgs e) {
        bool isSelectedCounter = e.selectedCounter == counter;
        foreach (GameObject visualGameObject in visualGameObjects) {
            visualGameObject.SetActive(isSelectedCounter);
        }
    }
}
