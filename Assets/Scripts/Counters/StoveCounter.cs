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
                        burningReceipeSO = GetBuriningReceipeSoForInput(GetKitchenObject().GetKitchenObjectSO());
                        OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                            state = state
                        });
                    }
                    break;
                case State.Fried:
                    burningTimer += Time.deltaTime;
                    OnProgressChanged?.Invoke(this, new IHasProgress.OnProgressChangedEventArgs {
                        progressNormalized = burningTimer / burningReceipeSO.buringTimerMax
                    });

                    if (burningTimer >= burningReceipeSO.buringTimerMax) {
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
            Debug.Log(state);
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
            if (!player.HasKitchenObject()) {
                GetKitchenObject().SetKitchenObjectParent(player);
                state = State.Idle;
                OnStateChanged?.Invoke(this, new OnStateChangedEventArgs {
                    state = state
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

    private BurningReceipeSO GetBuriningReceipeSoForInput(KitchenObjectSO kitchenObjectSO) {
        foreach (BurningReceipeSO burningReceipeSO in burningReceipeSOArray) {
            if (burningReceipeSO.input == kitchenObjectSO) {
                return burningReceipeSO;
            }
        }
        return null;
    }
}
