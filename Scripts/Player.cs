using System;
using UnityEngine;
using UnityEngine.Serialization;

public class Player : MonoBehaviour, IKitchenObjectParent {
    public static Player Instance { get; private set; }

    public event EventHandler<OnSelectedCounterChangedEventArgs> OnSelectedCounterChanged;

    public class OnSelectedCounterChangedEventArgs : EventArgs {
        public BaseCounter selectedCounter;
    }

    [SerializeField] private float moveSpeed = 7.0f;
    [SerializeField] private float rotateSpeed = 10.0f;
    [SerializeField] private GameInput gameInput;
    [SerializeField] private LayerMask countersLayerMask;
    [SerializeField] private Transform kitchenObjectHoldPoint;

    private bool isWalking;
    private Vector3 lastInteractDirection;
    private BaseCounter selectedCounter;
    private KitchenObject kitchenObject;

    private void Awake() {
        if (Instance != null) {
            Debug.LogError("There is more than one Player instance");
        }

        Instance = this;
    }

    private void Start() {
        // Subscribe to interact event
        gameInput.OnInteractAction += GameInputOnInteractAction;
        gameInput.OnInteractAlternateAction += GameInputOnInteractAlternateAction;
    }

    private void Update() {
        HandleMovement();
        HandleInteractions();
    }

    private void GameInputOnInteractAction(object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.Interact(this);
        }
    }

    private void GameInputOnInteractAlternateAction(object sender, EventArgs e) {
        if (selectedCounter != null) {
            selectedCounter.InteractAlternate(this);
        }
    }

    public bool IsWalking() {
        return isWalking;
    }

    private void HandleInteractions() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        if (moveDirection != Vector3.zero) {
            lastInteractDirection = moveDirection;
        }

        const float interactDistance = 2f;
        if (!Physics.Raycast(transform.position, lastInteractDirection, out RaycastHit raycastHit, interactDistance,
                countersLayerMask)) {
            SetSelectedCounter(null);
        }
        else if (raycastHit.transform.TryGetComponent(out BaseCounter baseCounter)) {
            // Found clear counter
            if (selectedCounter != baseCounter) {
                SetSelectedCounter(baseCounter);
            }
        }
        else {
            SetSelectedCounter(null);
        }
    }

    private void HandleMovement() {
        Vector2 inputVector = gameInput.GetMovementVectorNormalized();

        Vector3 moveDirection = new Vector3(inputVector.x, 0f, inputVector.y);
        isWalking = moveDirection != Vector3.zero;

        // Check if player hits wall or other obstacle
        float moveDistance = moveSpeed * Time.deltaTime;
        float playerRadius = 0.7f;
        float playerHeight = 2.0f;

        bool canMove = !Physics.CapsuleCast(transform.position,
            transform.position + Vector3.up * playerHeight,
            playerRadius, moveDirection, moveDistance);

        // Check when can move only in X or Z axis
        if (!canMove) {
            // Can move in X?
            Vector3 moveDirectionX = new Vector3(moveDirection.x, 0f, 0f).normalized;
            canMove = moveDirection.x != 0 && !Physics.CapsuleCast(transform.position,
                transform.position + Vector3.up * playerHeight,
                playerRadius, moveDirectionX, moveDistance);

            if (canMove) {
                moveDirection = moveDirectionX;
            }
            else {
                // Can move in Z?
                Vector3 moveDirectionZ = new Vector3(0f, 0f, moveDirection.z).normalized;
                canMove = moveDirection.z != 0 && !Physics.CapsuleCast(transform.position,
                    transform.position + Vector3.up * playerHeight,
                    playerRadius, moveDirectionZ, moveDistance);

                if (canMove) {
                    moveDirection = moveDirectionZ;
                }
            }
        }

        // Move
        if (canMove) {
            transform.position += moveDirection * moveDistance;
        }

        // Rotate
        transform.forward = Vector3.Slerp(transform.forward, moveDirection, Time.deltaTime * rotateSpeed);
    }

    private void SetSelectedCounter(BaseCounter counter) {
        selectedCounter = counter;

        OnSelectedCounterChanged?.Invoke(this, new OnSelectedCounterChangedEventArgs {
            selectedCounter = counter
        });
    }

    public Transform GetKitchenObjectFollowTransform() {
        return kitchenObjectHoldPoint;
    }

    public void SetKitchenObject(KitchenObject kitchenObject) {
        this.kitchenObject = kitchenObject;
    }

    public KitchenObject GetKitchenObject() {
        return kitchenObject;
    }

    public void ClearKitchenObject() {
        kitchenObject = null;
    }

    public bool HasKitchenObject() {
        return kitchenObject != null;
    }
}