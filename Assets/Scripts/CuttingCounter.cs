using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {

    [SerializeField] private CuttingReceipeSO[] cuttingReceipeSOArray;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                bool hasCuttingReceipe = GetCuttingReceipeOutput(player.GetKitchenObject().GetKitchenObjectSO()) != null;
                if (hasCuttingReceipe) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                }
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject()) {
            KitchenObjectSO outputObjectSO = GetCuttingReceipeOutput(GetKitchenObject().GetKitchenObjectSO());
            bool hasCuttingReceipe = outputObjectSO != null;

            if (hasCuttingReceipe) {
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputObjectSO, this);
            }
        }
    }

    private KitchenObjectSO GetCuttingReceipeOutput(KitchenObjectSO kitchenObjectSO) {
        foreach (CuttingReceipeSO cuttingReceipeSO in cuttingReceipeSOArray) {
            if (cuttingReceipeSO.input == kitchenObjectSO) {
                return cuttingReceipeSO.output;
            }
        }
        return null;
    }
}
