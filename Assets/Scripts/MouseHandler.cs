using System;
using System.Collections;
using System.Collections.Generic;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.InputSystem;
using UnityEngine.InputSystem.XR;

public class MouseHandler : MonoBehaviour
{
    [SerializeField]
    public GameObject m_XRRig;

    [SerializeField]
    public GameObject m_XRCamera;

    [SerializeField]
    public GameObject m_SceneManagerObject;
    private SceneManager m_SceneManager;
    
    [SerializeField]
    public InputActionReference m_mouseLeftClickInputAction = null;

    [SerializeField]
    public InputActionReference m_mouseDragInputAction = null;

    [SerializeField]
    public float m_cameraRotationSpeed = 2;

    [SerializeField]
    public float m_maxCameraPitchAngle = 60;

    private Vector2 m_mouseDelta;

    private float m_cameraYaw, m_cameraPitch;

    private Vector3 m_lastCameraRotation;

    private TrackedPoseDriver m_cameraTrackedPosedDriver;

    // flag is required since canceled event gets called twice, probably a bug?
    private bool m_cancelEventHandled = false;

    private void Awake()
    {

    }

    private void OnEnable()
    {
        m_mouseLeftClickInputAction.action.Enable();
        m_mouseLeftClickInputAction.action.started += OnLeftClick;

        m_mouseDragInputAction.action.Enable();
        m_mouseDragInputAction.action.started += OnRightClickStart;
        m_mouseDragInputAction.action.canceled += OnRightClickCancel;
    }

    void Start()
    {
        Init();        
    }

    private void Init()
    {
        m_cameraYaw = m_cameraPitch = 0;
        m_cameraTrackedPosedDriver = m_XRCamera.GetComponent<TrackedPoseDriver>();

        m_SceneManager = m_SceneManagerObject.GetComponent<SceneManager>();
    }

    private void OnLeftClick(InputAction.CallbackContext context)
    {
        float ZDistance = 1;
        bool IsWidgetClick = false;

        Vector2 clickPos = Mouse.current.position.ReadValue();
        Vector3 screenCoord = new Vector3(clickPos.x, clickPos.y, ZDistance);
        Vector3 worldCoord = Camera.main.ScreenToWorldPoint(screenCoord);

        Vector3 cameraPos = Camera.main.transform.position;
        Vector3 direction = (worldCoord - cameraPos).normalized;
        Ray ray = new Ray(cameraPos, direction);

        RaycastHit hitInfo;
        if(Physics.Raycast(ray, out hitInfo, ZDistance + 1))
        {
            GameObject o = hitInfo.collider.gameObject;
            Widget w = o.GetComponent<Widget>();
            if(w != null)
            {
                IsWidgetClick = true;
            }
        }
        if(!IsWidgetClick)
        {
            m_SceneManager.CreateAnchor(worldCoord);
        }
    }

    private void OnRightClickStart(InputAction.CallbackContext context)
    {
        m_cancelEventHandled = false;
        m_cameraTrackedPosedDriver.enabled = false;
        m_lastCameraRotation = GetXRCameraForwardProjectedVector();
    }

    private void OnRightClickCancel(InputAction.CallbackContext context)
    {
        if(!m_cancelEventHandled)
        {
            InverseXRRigRotation();
            m_cameraTrackedPosedDriver.enabled = true;
            m_cameraYaw = m_cameraPitch = 0;
            m_cancelEventHandled = true;
        }
    }

    private Vector3 GetXRCameraForwardProjectedVector()
    {
        return Vector3.ProjectOnPlane(m_XRCamera.transform.forward, Vector3.up).normalized;
    }

    private void RotateCamera()
    {
        m_mouseDelta = m_mouseDragInputAction.action.ReadValue<Vector2>();

        if (m_mouseDelta.magnitude > 0)
        {
            m_cameraYaw += m_cameraRotationSpeed * m_mouseDelta.x * Time.deltaTime;
            m_cameraPitch -= m_cameraRotationSpeed * m_mouseDelta.y * Time.deltaTime;

            m_cameraPitch = Mathf.Clamp(m_cameraPitch, -m_maxCameraPitchAngle, m_maxCameraPitchAngle);

            m_XRCamera.transform.localEulerAngles = new Vector3(m_cameraPitch, m_cameraYaw, 0.0f);
        }
    }

    private void InverseXRRigRotation()
    {        
        Vector3 camera_forward = GetXRCameraForwardProjectedVector();

        float angle = Vector3.SignedAngle(camera_forward, m_lastCameraRotation, Vector3.up);
        Vector3 targetAngle = new Vector3(0, -angle, 0);

        m_XRRig.transform.eulerAngles += targetAngle;
    }

    void Update()
    {
        RotateCamera();        
    }

    private void OnDisable()
    {
        m_mouseLeftClickInputAction.action.started -= OnLeftClick;
        m_mouseDragInputAction.action.Disable();

        m_mouseDragInputAction.action.started -= OnRightClickStart;
        m_mouseDragInputAction.action.canceled -= OnRightClickCancel;
        m_mouseDragInputAction.action.Disable();
    }
}
