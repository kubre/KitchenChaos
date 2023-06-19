using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PlatesCounter : BaseCounter {

    public event EventHandler OnPlateSpawned;
    public event EventHandler OnPlateRemoved;

    [SerializeField] private KitchenObjectSO kitchenObjectSO;
    private float spawnPlateTimer = 0f;
    private float spawnPlateTimerMax = 4;
    private int platesSpawnedAmount = 0;
    private int platesSpawnedAmountMax = 4;

    private void Update() {
        spawnPlateTimer += Time.deltaTime;

        if (spawnPlateTimer > 4f) {
            spawnPlateTimer = 0f;
            if (platesSpawnedAmount < platesSpawnedAmountMax) {
                platesSpawnedAmount++;
                OnPlateSpawned?.Invoke(this, EventArgs.Empty);
            }
        }
    }

    public override void Interact(Player player) {
        Debug.Log(player.name + " Interacted with PlatesCounter");
        Debug.Log(player.HasKitchenObject());
        if (!player.HasKitchenObject()) {
            if (platesSpawnedAmount > 0) {
                platesSpawnedAmount--;
                OnPlateRemoved?.Invoke(this, EventArgs.Empty);
                KitchenObject.SpawnKitchenObject(kitchenObjectSO, player);
            }
        }
    }
}
