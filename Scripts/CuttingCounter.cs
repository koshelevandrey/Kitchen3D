using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CuttingCounter : BaseCounter {
    [SerializeField] private KitchenObjectSO cutKitchenObjectSO;

    public override void Interact(Player player) {
        bool counterHasObject = HasKitchenObject();
        bool playerHasObject = player.HasKitchenObject();

        if (!counterHasObject && playerHasObject) {
            // Put player object on counter
            player.GetKitchenObject().SetKitchenObjectParent(this);
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
        bool counterHasObject = HasKitchenObject();

        if (counterHasObject) {
            GetKitchenObject().DestroySelf();

            KitchenObject.SpawnKitchenObject(cutKitchenObjectSO, this);
        }
    }
}