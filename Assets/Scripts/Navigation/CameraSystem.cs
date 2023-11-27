using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using Cinemachine;
using UnityEngine.UIElements;
using Unity.VisualScripting;
using System;
using UnityEngine.Events;

namespace CodeMonkey.CameraSystem {

    public class CameraSystem : MonoBehaviour {

        [SerializeField] public UnityEvent ResetCamera;

        [SerializeField] private CinemachineVirtualCamera cinemachineVirtualCamera;
        [SerializeField] private float fieldOfViewMax = 50;
        [SerializeField] private float fieldOfViewMin = 10;
        [SerializeField] private float followOffsetMin = 5f;
        [SerializeField] private float followOffsetMax = 50f;
        [SerializeField] private float followOffsetMinY = 10f;
        [SerializeField] private float followOffsetMaxY = 50f;
        [SerializeField] private float dragPanSpeed = .1f;
        [SerializeField] private float zoomSpeed = .1f;
        [SerializeField] private float movementSpeed;
        [SerializeField] private float normalSpeed = 10f;
        [SerializeField] private float fastSpeed = 30f;
        [SerializeField] private float movementTime = 12f;

        private float targetFieldOfView = 50;
        private Vector3 followOffset;

        private Vector3 newPosition;
        private Vector3 dragStartPosition;
        private Vector3 dragCurrentPosition;

        private void Awake() {
            followOffset = cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset;
        }

        private void Start()
        {
            newPosition = transform.position;
        }

        private void Update() {
            HandleCameraMovementDragPan();
            HandleReset();
            HandleCameraMovement();
            HandleCameraRotation();

            HandleCameraZoom_FieldOfView();
            HandleCameraZoom_MoveForward();
            HandleCameraZoom_LowerY();

        }

        private void HandleReset()
        {
            if (Input.GetKey(KeyCode.R))
            {
                newPosition = new Vector3(GridManager.Instance.gridSize.x/2f, transform.position.y, GridManager.Instance.gridSize.y / 2f);
            }

        }

        private void HandleCameraMovement() {
            Vector3 inputDir = new Vector3(0, 0, 0);

            if (Input.GetKey(KeyCode.LeftShift))
            {
                movementSpeed = fastSpeed;
            }
            else
            {
                movementSpeed = normalSpeed;
            }

            if (Input.GetKey(KeyCode.W)) inputDir.z = +1f;
            if (Input.GetKey(KeyCode.S)) inputDir.z = -1f;
            if (Input.GetKey(KeyCode.A)) inputDir.x = -1f;
            if (Input.GetKey(KeyCode.D)) inputDir.x = +1f;

            newPosition += transform.forward * inputDir.z * movementSpeed + transform.right * inputDir.x * movementSpeed;
            transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        }


        private void HandleCameraMovementDragPan() {

            if (Input.GetMouseButtonDown(1)) {
                
                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;

                if (plane.Raycast(ray, out entry))
                {
                    dragStartPosition = ray.GetPoint(entry);
                }
            }

            if (Input.GetMouseButton(1)) {

                Plane plane = new Plane(Vector3.up, Vector3.zero);
                Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);

                float entry;

                if(plane.Raycast(ray, out entry))
                {
                    dragCurrentPosition = ray.GetPoint(entry);
                    newPosition = transform.position + dragStartPosition - dragCurrentPosition;
                }
            }
        }

        private void HandleCameraRotation() {
            float rotateDir = 0f;
            if (Input.GetKey(KeyCode.Q)) rotateDir = +1f;
            if (Input.GetKey(KeyCode.E)) rotateDir = -1f;

            float rotateSpeed = 100f;
            transform.eulerAngles += new Vector3(0, rotateDir * rotateSpeed * Time.deltaTime, 0);
        }

        private void HandleCameraZoom_FieldOfView() {
            if (Input.mouseScrollDelta.y > 0) {
                targetFieldOfView -= 5;
            }
            if (Input.mouseScrollDelta.y < 0) {
                targetFieldOfView += 5;
            }

            targetFieldOfView = Mathf.Clamp(targetFieldOfView, fieldOfViewMin, fieldOfViewMax);

            cinemachineVirtualCamera.m_Lens.FieldOfView =
                Mathf.Lerp(cinemachineVirtualCamera.m_Lens.FieldOfView, targetFieldOfView, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_MoveForward() {
            Vector3 zoomDir = followOffset.normalized;

            float zoomAmount = 3f;
            if (Input.mouseScrollDelta.y > 0) {
                followOffset -= zoomDir * zoomAmount;
            }
            if (Input.mouseScrollDelta.y < 0) {
                followOffset += zoomDir * zoomAmount;
            }

            if (followOffset.magnitude < followOffsetMin) {
                followOffset = zoomDir * followOffsetMin;
            }

            if (followOffset.magnitude > followOffsetMax) {
                followOffset = zoomDir * followOffsetMax;
            }

        
            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);
        }

        private void HandleCameraZoom_LowerY() {
            float zoomAmount = 3f;
            if (Input.mouseScrollDelta.y > 0) {
                followOffset.y -= zoomAmount;
            }
            if (Input.mouseScrollDelta.y < 0) {
                followOffset.y += zoomAmount;
            }

            followOffset.y = Mathf.Clamp(followOffset.y, followOffsetMinY, followOffsetMaxY);

            cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset =
                Vector3.Lerp(cinemachineVirtualCamera.GetCinemachineComponent<CinemachineTransposer>().m_FollowOffset, followOffset, Time.deltaTime * zoomSpeed);

        }

    }

}