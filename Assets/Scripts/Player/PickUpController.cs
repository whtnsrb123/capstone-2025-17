using Photon.Pun;
using UnityEngine;
using TMPro;
using System.Collections;
using Photon.Pun;

public class PickUpController : MonoBehaviourPun
{
    private float defaultMass; // 물체의 원래 질량
    private float defaultDrag; // 물체의 원래 저항력
    private float defaultAngularDrag;

    public Transform raycastPosition;
    public Transform pickPosition;
    public GameObject heldObject;
    private Rigidbody heldObjectRb;
    public GameObject detectedObject;

    private float detectionRange = 2f;        // 감지 거리
    private float pickUpOffset = 0.5f;        // 손과 물체 사이 거리
    [SerializeField] private TMP_Text pickUpUI;
    [SerializeField] private float throwForce = 10f;

    [SerializeField] private LineRenderer trajectoryLine;
    private int trajectoryPoints = 50;       
    private float timeBetweenPoints = 0.03f; 
    [SerializeField] private float dashLength = 0.1f; // 점 길이
    [SerializeField] private float dashGap = 0.05f; 
    private float crosshairSize = 50f;        // 에임 크기

    [SerializeField] private float holdFollowSpeed = 60f; 
    [SerializeField] private float holdRotateSpeed = 10f; 

    private Quaternion originalRotation; // 처음 잡았던 회전값 저장
    private bool isTouching = false;
    private Quaternion heldRotationOffset = Quaternion.identity; // R키로 회전한 상태 누적 저장
    
    private GameObject recentlyThrownObject;
    private float throwCooldownTime = 1.0f;
    private float throwTimer = 0f;

    void Start()
    {
        Camera mainCamera = Camera.main;
        raycastPosition = mainCamera.transform;

        if (pickUpUI != null) pickUpUI.enabled = false;

        InitializeTrajectoryLine();
    }
    private void InitializeTrajectoryLine() // 궤적
    {
        trajectoryLine = GetComponent<LineRenderer>();
        trajectoryLine.enabled = true;
        trajectoryLine.positionCount = 0;
        trajectoryLine.startWidth = 0.05f;
        trajectoryLine.endWidth = 0.05f;
        trajectoryLine.numCornerVertices = 5;
        trajectoryLine.numCapVertices = 0;
        trajectoryLine.textureMode = LineTextureMode.Tile;

        trajectoryLine.material = new Material(Shader.Find("Sprites/Default"))
        {
            mainTexture = CreateDashedTexture()
        };
        trajectoryLine.startColor = trajectoryLine.endColor = Color.yellow;
        trajectoryLine.material.mainTextureScale = new Vector2(1f / (dashLength + dashGap), 1f);
    }

    private Texture2D CreateDashedTexture()
    {
        Texture2D dashTexture = new Texture2D(4, 1);
        dashTexture.SetPixel(0, 0, Color.white);
        dashTexture.SetPixel(1, 0, Color.white);
        dashTexture.SetPixel(2, 0, Color.clear);
        dashTexture.SetPixel(3, 0, Color.clear);
        dashTexture.Apply();
        return dashTexture;
    }

    void Update()
    {

        if (GameStateManager.isServerTest && !photonView.IsMine) return;
        if (throwTimer > 0f)
            throwTimer -= Time.deltaTime;
        Debug.DrawRay(raycastPosition.position, raycastPosition.forward * detectionRange, Color.red);

        if (heldObject != null)
        {
            if (pickUpUI != null)
            {
                pickUpUI.enabled = true;
                pickUpUI.text = "Press F to Drop";
            }
        }
        else if (trajectoryLine != null)
        {
            trajectoryLine.positionCount = 0;
        }
    }

    void FixedUpdate()
    {
        if (GameStateManager.isServerTest && !photonView.IsMine) return;
        // 궤적 그리기
        DisplayTrajectory();

        // 들고 있는 물체 손 위치로 이동
        if (heldObject != null && heldObjectRb != null && pickPosition != null)
        {
            // 위치 이동
            Vector3 targetPosition = pickPosition.position + pickPosition.forward * pickUpOffset;
            Vector3 moveDirection = (targetPosition - heldObjectRb.position);
            heldObjectRb.MovePosition(heldObjectRb.position + moveDirection * holdFollowSpeed * Time.fixedDeltaTime);

            // 충돌 감지
            Vector3 checkSize = heldObject.transform.localScale * 0.5f;
            isTouching = Physics.CheckBox(heldObjectRb.position, checkSize, heldObjectRb.rotation, ~LayerMask.GetMask("HeldObject"));

            // 회전: pickPosition의 회전에 R 키 누적 회전 추가
            Quaternion targetRotation = isTouching
                ? pickPosition.rotation * heldRotationOffset
                : originalRotation * heldRotationOffset;

            heldObjectRb.MoveRotation(Quaternion.Slerp(heldObjectRb.rotation, targetRotation, holdRotateSpeed * Time.fixedDeltaTime));
        }
    }

    void OnGUI()
    {
        float screenCenterX = Screen.width / 2;
        float screenCenterY = Screen.height / 2;
        float thickness = 2f;

        GUI.color = Color.red;
        GUI.DrawTexture(new Rect(screenCenterX - crosshairSize / 2, screenCenterY - thickness / 2, crosshairSize, thickness), Texture2D.whiteTexture);
        GUI.DrawTexture(new Rect(screenCenterX - thickness / 2, screenCenterY - crosshairSize / 2, thickness, crosshairSize), Texture2D.whiteTexture);
    }

    public void HandlePickUpOrDrop()
    {
        if (detectedObject != null && heldObject == null)
        {
            TryPickUp(); // 줍기
        }
        else if (heldObject != null)
        {
            DropObject(); // 놓기
        }
    }
    /*private void TryPickUp()
    {
        if (detectedObject == null) return;
        if (detectedObject == recentlyThrownObject && throwTimer > 0f)
        {
            Debug.Log("감지 금지 : 최근 던진 오브젝트");
            return;
        }

        heldObject = detectedObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();
        if (heldObjectRb != null)
        {
            // 원래 물리 속성 저장
            defaultMass = heldObjectRb.mass;
            defaultDrag = heldObjectRb.drag;
            defaultAngularDrag = heldObjectRb.angularDrag;

            // 들고 있을 때 가벼운 속성 적용
            heldObjectRb.mass = 0.1f; // 매우 가벼운 질량
            heldObjectRb.drag = 5f;   // 높은 저항력
            heldObjectRb.angularDrag = 10f; // 높은 회전 저항력

            heldObjectRb.isKinematic = false;
            heldObjectRb.useGravity = false;
            heldObject.layer = LayerMask.NameToLayer("HeldObject");
            originalRotation = heldObject.transform.rotation;
        }
    }
*/
    private bool isPickingUp = false; // 중복 줍기 방지용

    public void TryPickUp()
    {
        if (GameStateManager.isServerTest)
        {
            photonView.RPC(nameof(RPC_TryPickUp), RpcTarget.All);
        }
        else
        {
            RPC_TryPickUp();
        }
    }
    [PunRPC]
    public void RPC_TryPickUp()
    {
        if (isPickingUp) return; // 이미 줍는 중이면 무시
        if (detectedObject == null) return; // 감지된 오브젝트 없으면 무시
    
        StartCoroutine(PickUpWithDelay(0.8f));
    }

    private IEnumerator PickUpWithDelay(float delay)
    {
        isPickingUp = true;
        yield return new WaitForSeconds(delay);

        if (detectedObject == null || heldObject != null)
        {
            isPickingUp = false;
            yield break;
        }

        heldObject = detectedObject;
        heldObjectRb = heldObject.GetComponent<Rigidbody>();
        if (heldObjectRb != null)
        {
            heldObjectRb.isKinematic = false;
            heldObjectRb.useGravity = false;

            heldObject.layer = LayerMask.NameToLayer("HeldObject");
            originalRotation = heldObject.transform.rotation;
            PhotonView objectView = detectedObject.GetComponent<PhotonView>();
            if (objectView != null && !objectView.IsMine)
            {
                objectView.RequestOwnership();
            }
            Debug.Log("물체 잡기 성공: " + heldObject.name);
            int objectViewID = objectView.ViewID;
            int playerViewID = photonView.ViewID;

            photonView.RPC(nameof(RPC_SetParent), RpcTarget.All, objectViewID, playerViewID);
        }
        else
        {
            heldObject = null;
        }
        isPickingUp = false;
    }
    
    [PunRPC]
    void RPC_SetParent(int objectViewID, int playerViewID)
    {
        PhotonView objView = PhotonView.Find(objectViewID);
        PhotonView playerView = PhotonView.Find(playerViewID);

        if (objView == null || playerView == null) return;

        Transform pickPos = playerView.transform.Find("pickPosition");
        
        if (pickPos != null)
        {
            objView.transform.SetParent(pickPos);
            objView.transform.localPosition = Vector3.forward * pickUpOffset;
            objView.transform.localRotation = Quaternion.identity;
        }

    }
    
    [PunRPC]
    void RPC_ClearParent(int objectViewID)
    {
        PhotonView objView = PhotonView.Find(objectViewID);

        if (objView == null) return;

        Transform objTransform = objView.transform;
        objTransform.SetParent(null); // 부모 해제
    }



    /*public void DropObject()
    {
        if (heldObject != null)
        {
            // 원래 물리 속성 복구
            heldObjectRb.mass = defaultMass;
            heldObjectRb.drag = defaultDrag;
            heldObjectRb.angularDrag = defaultAngularDrag;

            heldObjectRb.useGravity = true;
            heldObjectRb.isKinematic = false;
            heldObject.layer = LayerMask.NameToLayer("Default");
        }
        ResetHeldObject();
    }
*/
    private bool isDropping = false; // 중복 방지용

    public void DropObject()
    {
        if (GameStateManager.isServerTest)
        {
            photonView.RPC(nameof(RPC_DropObject), RpcTarget.All);
        }
        else
        {
            RPC_DropObject();
        }
    }

    [PunRPC]
    public void RPC_DropObject()
    {
        if (isDropping) return;      // 이미 내려놓는 중이면 무시
        if (heldObject == null) return; // 들고 있는 물체 없으면 무시
        int objectViewID = heldObject.GetPhotonView().ViewID;
        StartCoroutine(DropWithDelay(0.8f, objectViewID));
    }

    private IEnumerator DropWithDelay(float delay, int objectViewID)
    {
        PhotonView objView = PhotonView.Find(objectViewID);
        
        isDropping = true;
        yield return new WaitForSeconds(delay);

        // 딜레이 중에 heldObject가 사라졌으면 중단
        if (heldObject == null)
        {
            isDropping = false;
            yield break;
        }

        // 기존 DropObject 처리
        heldObjectRb.useGravity = true;
        heldObjectRb.isKinematic = false;
        heldObjectRb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
        heldObject.layer = LayerMask.NameToLayer("Default");
        Debug.Log("물체 놓기: " + heldObject.name);
        photonView.RPC(nameof(RPC_ClearParent), RpcTarget.All, objectViewID);
        //heldObject.transform.parent = null;
        heldObject = null;
        heldObjectRb = null;

        isDropping = false;
    }

    public void ThrowObject()
    {
        if (heldObject == null || heldObjectRb == null) return;

        Vector3 throwDirection = (pickPosition.forward + Vector3.up * 0.5f).normalized;
        int objectViewID = heldObject.GetPhotonView().ViewID;

        if (GameStateManager.isServerTest)
        {
            photonView.RPC(nameof(RPC_StartThrowWithDelay), RpcTarget.All, objectViewID, throwDirection);
        }
        else
        {
            StartCoroutine(ThrowWithDelay(0.7f, objectViewID, throwDirection));
        }

        recentlyThrownObject = heldObject;
        throwTimer = throwCooldownTime;
        ResetHeldObject(); // 로컬 heldObject는 즉시 null 처리
    }
    
    [PunRPC]
    public void RPC_StartThrowWithDelay(int objectViewID, Vector3 throwDirection)
    {
        StartCoroutine(ThrowWithDelay(0.7f, objectViewID, throwDirection));
    }
    
    private IEnumerator ThrowWithDelay(float delay, int objectViewID, Vector3 throwDirection)
    {
        yield return new WaitForSeconds(delay);

        PhotonView objView = PhotonView.Find(objectViewID);
        if (objView == null) yield break;

        GameObject obj = objView.gameObject;
        Rigidbody rb = obj.GetComponent<Rigidbody>();
        Collider col = obj.GetComponent<Collider>();

        if (rb != null)
        {
            rb.useGravity = true;
            rb.isKinematic = false;
            rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
            rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
        }

        if (col != null)
            col.isTrigger = false;

        obj.transform.parent = null;
        obj.transform.position += Vector3.up * 0.1f;

        Debug.Log("ThrowWithDelay 던지기 완료");
    }

    private void ResetHeldObject()
    {
        photonView.RPC(nameof(RPC_ResetHeldObject), RpcTarget.All);
    }

    [PunRPC]
    void RPC_ResetHeldObject()
    {
        if (heldObject != null)
            heldObject.layer = LayerMask.NameToLayer("Default");

        heldObject = null;
        heldObjectRb = null;

        if (TryGetComponent(out InteractManager interact))
            interact.ResetDetection();
    }


    [PunRPC]
    void RPC_ThrowObject(int objectViewID, Vector3 throwDirection)
    {
       PhotonView objView = PhotonView.Find(objectViewID);
       if (objView == null) return;
       
       Transform objTransform = objView.transform;
       Rigidbody rb = objTransform.GetComponent<Rigidbody>();
       Collider col = objTransform.GetComponent<Collider>();
       Collider heldObjectCollider = objView.gameObject.GetComponent<Collider>();

       if (col != null)
       {
           col.isTrigger = false;
       }
       
       if (rb != null)
       {
           rb.isKinematic = false;
           rb.collisionDetectionMode = CollisionDetectionMode.ContinuousDynamic;
           rb.AddForce(throwDirection * throwForce, ForceMode.VelocityChange);
       }
       objTransform.parent = null;
       objTransform.position += Vector3.up * 0.1f;

       Debug.Log("RPC 던지기 완료");
    }

    private void DisplayTrajectory()
    {
        if (heldObject == null || heldObjectRb == null || pickPosition == null) return;

        trajectoryLine.positionCount = trajectoryPoints;

        Vector3 currentPosition = heldObject.transform.position;
        Vector3 initialVelocity = (pickPosition.forward + Vector3.up * 0.5f).normalized * throwForce;

        float totalLength = 0f;
        Vector3 previousPoint = currentPosition;

        // 들고 있는 물체 콜라이더
        Collider heldCollider = heldObject.GetComponent<Collider>();

        for (int i = 0; i < trajectoryPoints; i++)
        {
            float t = i * timeBetweenPoints;
            Vector3 displacement = (initialVelocity * t) + (0.5f * Physics.gravity * t * t);
            Vector3 pointPosition = currentPosition + displacement;

            trajectoryLine.SetPosition(i, pointPosition);

            // Ray를 현재 궤적 구간 방향으로 발사
            Ray ray = new Ray(previousPoint, (pointPosition - previousPoint).normalized);
            float distance = Vector3.Distance(previousPoint, pointPosition) * 0.9f;

            // 모든 충돌체 감지
            RaycastHit[] hits = Physics.RaycastAll(ray, distance, ~0, QueryTriggerInteraction.Ignore);

            foreach (var hit in hits)
            {
                // 들고 있는 물체는 무시
                if (heldCollider != null && hit.collider == heldCollider)
                    continue;

                // 다른 물체와 충돌 시 궤적 종료
                trajectoryLine.positionCount = i + 1;
                return;
            }

            if (i > 0)
            {
                totalLength += Vector3.Distance(previousPoint, pointPosition);
            }
            previousPoint = pointPosition;
        }

        // 궤적 길이에 따라 텍스처 스케일 조정
        trajectoryLine.material.mainTextureScale = new Vector2(totalLength / (dashLength + dashGap), 1f);
    }

    public void RotateHeldObject()
    {
        if (GameStateManager.isServerTest)
        {
            photonView.RPC(nameof(RPC_RotateHeldObject), RpcTarget.All);
        }
        else
        {
            RPC_RotateHeldObject();
        }
    }
    [PunRPC]
    public void RPC_RotateHeldObject()
    {
        heldRotationOffset *= Quaternion.Euler(0, 0, -90);
        Debug.Log("물체 회전");
    }

    public bool IsHoldingObject()
    {
        return heldObject != null;
    }
}