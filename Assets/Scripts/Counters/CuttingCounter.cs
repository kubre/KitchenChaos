using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter, IHasProgress {

    public event EventHandler OnCut;

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    [SerializeField] private CuttingReceipeSO[] cuttingReceipeSOArray;

    private int cuttingProgress;

    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                if (HasReceipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);
                    cuttingProgress = 0;

                    CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSoForInput(GetKitchenObject().GetKitchenObjectSO());
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = (float)cuttingProgress / cuttingReceipeSO.maxCuttingRequired
                    });
                }
            }
        } else {
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
            }
        }
    }

    public override void InteractAlternate(Player player) {
        if (HasKitchenObject() && HasReceipeWithInput(GetKitchenObject().GetKitchenObjectSO())) {
            cuttingProgress++;

            OnCut?.Invoke(this, EventArgs.Empty);

            CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSoForInput(GetKitchenObject().GetKitchenObjectSO());
            OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                progressNormalized = ((float)cuttingProgress % cuttingReceipeSO.maxCuttingRequired) / cuttingReceipeSO.maxCuttingRequired
            });


            if (cuttingProgress >= cuttingReceipeSO.maxCuttingRequired) {
                KitchenObjectSO outputObjectSO = GetOutputForInput(GetKitchenObject().GetKitchenObjectSO());
                GetKitchenObject().DestroySelf();

                KitchenObject.SpawnKitchenObject(outputObjectSO, this);
            }

        }
    }

    private bool HasReceipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSoForInput(inputKitchenObjectSO);
        return cuttingReceipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        CuttingReceipeSO cuttingReceipeSO = GetCuttingReceipeSoForInput(inputKitchenObjectSO);
        if (cuttingReceipeSO != null) {
            return cuttingReceipeSO.output;
        }
        return null;
    }

    private CuttingReceipeSO GetCuttingReceipeSoForInput(KitchenObjectSO kitchenObjectSO) {
        foreach (CuttingReceipeSO cuttingReceipeSO in cuttingReceipeSOArray) {
            if (cuttingReceipeSO.input == kitchenObjectSO) {
                return cuttingReceipeSO;
            }
        }
        return null;
    }
}
