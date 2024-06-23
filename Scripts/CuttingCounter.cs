using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {
    [SerializeField] private CuttingRecipeSO[] cuttingRecipeSOArray;

    public override void Interact(Player player) {
        bool counterHasObject = HasKitchenObject();
        bool playerHasObject = player.HasKitchenObject();

        if (!counterHasObject && playerHasObject) {
            // Put player object on counter if it can be sliced
            KitchenObject playerKitchenObject = player.GetKitchenObject();
            KitchenObjectSO playerKitchenObjectSO = playerKitchenObject.GetKitchenObjectSO();
            if (HasRecipeWithInput(playerKitchenObjectSO)) {
                playerKitchenObject.SetKitchenObjectParent(this);
            }
        }
        else if (counterHasObject && !playerHasObject) {
            // Player takes object from counter
            GetKitchenObject().SetKitchenObjectParent(player);
        }
        else if (counterHasObject && playerHasObject) {
            // Swap objects between player and counter
            KitchenObject counterKitchenObject = GetKitchenObject();
            KitchenObject playerKitchenObject = player.GetKitchenObject();

            counterKitchenObject.SetKitchenObjectParent(player);
            playerKitchenObject.SetKitchenObjectParent(this);

            // This shouldn't be needed but doesn't work without it
            SetKitchenObject(playerKitchenObject);
            player.SetKitchenObject(counterKitchenObject);
        }
    }

    public override void InteractAlternate(Player player) {
        if (!HasKitchenObject()) return;

        KitchenObject counterKitchenObject = GetKitchenObject();
        KitchenObjectSO outputKitchenObjectSO = GetOutputForInput(counterKitchenObject.GetKitchenObjectSO());
        if (outputKitchenObjectSO is null) return;

        counterKitchenObject.DestroySelf();

        KitchenObject.SpawnKitchenObject(outputKitchenObjectSO, this);
    }

    private bool HasRecipeWithInput(KitchenObjectSO inputKitchenObjectSO) {
        return GetOutputForInput(inputKitchenObjectSO) is not null;
    }

    private KitchenObjectSO GetOutputForInput(KitchenObjectSO inputKitchenObjectSO) {
        foreach (CuttingRecipeSO cuttingRecipeSO in cuttingRecipeSOArray) {
            if (cuttingRecipeSO.input == inputKitchenObjectSO) {
                return cuttingRecipeSO.output;
            }
        }

        return null;
    }
}