using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StoveCounter : BaseCounter, IHasProgress {

    public event EventHandler<IHasProgress.OnProgressChangedEventArgs> OnProgressChanged;

    public event EventHandler<OnStateChangedEventArgs> OnStateChanged;
    public class OnStateChangedEventArgs : EventArgs {
        public State state;
    }

    public enum State {
        Idle,
        Frying,
        Fried,
        Burned,
    }

    [SerializeField] private FryingReceipeSO[] fryingReceipeSOArray;
    [SerializeField] private BurningReceipeSO[] burningReceipeSOArray;

    private State state;
    private float fryingTimer = 0f;
    private float burningTimer = 0f;
    private FryingReceipeSO fryingReceipeSO;
    private BurningReceipeSO burningReceipeSO;


    private void Start() {
        state = State.Idle;
    }

    private void Update() {

        if (HasKitchenObject()) {
            switch (state) {
                case State.Idle:
                    break;
                case State.Frying:
                    fryingTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = fryingTimer / fryingReceipeSO.fryingTimerMax
                    });

                    if (fryingTimer >= fryingReceipeSO.fryingTimerMax) {
                        GetKitchenObject().DestroySelf();

                        KitchenObject.SpawnKitchenObject(fryingReceipeSO.output, this);

                        state = State.Fried;
                        burningTimer = 0f;
                        burningReceipeSO = GetBurningReceipeSoForInput(GetKitchenObject().GetKitchenObjectSO());
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = burningTimer / burningReceipeSO.burningTimerMax
                    });

                    if (burningTimer >= burningReceipeSO.burningTimerMax) {
                        GetKitchenObject().DestroySelf();
                        KitchenObject.SpawnKitchenObject(burningReceipeSO.output, this);
                        state = State.Burned;
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });

                        OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                            progressNormalized = 0f
                        });
                    }
                    break;
                case State.Burned:
                    break;
            }
        }
    }


    public override void Interact(Player player) {
        if (!HasKitchenObject()) {
            if (player.HasKitchenObject()) {
                if (HasReceipeWithInput(player.GetKitchenObject().GetKitchenObjectSO())) {
                    player.GetKitchenObject().SetKitchenObjectParent(this);

                    fryingReceipeSO = GetFryingReceipeSoForInput(GetKitchenObject().GetKitchenObjectSO());
                    state = State.Frying;
                    fryingTimer = 0f;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = fryingTimer / fryingReceipeSO.fryingTimerMax
                    });
                }
            }
        } else {
            // There is a kitchen object on the stove
            if (player.HasKitchenObject()) {
                if (player.GetKitchenObject().TryGetPlate(out PlateKitchenObject plateKitchenObject)) {
                    // player is holding a plate
                    if (plateKitchenObject.TryAddIngredient(GetKitchenObject().GetKitchenObjectSO())) {
                        GetKitchenObject().DestroySelf();
                    }
                    state = State.Idle;
                    OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                        state = state
                    });

                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = 0f
                    });
                }
            } else {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = state
                });

                OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                    progressNormalized = 0f
                });
            }
        }
    }

    private bool HasReceipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSoForInput(inputKitchenObjectSO);
        return fryingReceipeSO != null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        FryingReceipeSO fryingReceipeSO = GetFryingReceipeSoForInput(inputKitchenObjectSO);
        if (fryingReceipeSO != null) {
            return fryingReceipeSO.output;
        }
        return null;
    }

    private FryingReceipeSO GetFryingReceipeSoForInput(KitchenObjectSO kitchenObjectSO) {
        foreach (FryingReceipeSO fryingReceipeSO in fryingReceipeSOArray) {
            if (fryingReceipeSO.input == kitchenObjectSO) {
                return fryingReceipeSO;
            }
        }
        return null;
    }

    private BurningReceipeSO GetBurningReceipeSoForInput(KitchenObjectSO kitchenObjectSO) {
        foreach (BurningReceipeSO burningReceipeSO in burningReceipeSOArray) {
            if (burningReceipeSO.input == kitchenObjectSO) {
                return burningReceipeSO;
            }
        }
        return null;
    }
}
