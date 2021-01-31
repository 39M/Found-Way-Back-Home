using UnityEngine;
using System.Collections;

namespace WhaleFall
{
    public class RTSCamera : MonoBehaviour
    {
        public Terrain terrain;

        public float panSpeed = 15.0f;
        public float zoomSpeed = 100.0f;
        public float rotationSpeed = 50.0f;
        public float mousePanMultiplier = 0.1f;
        public float mouseRotationMultiplier = 0.2f;
        public float mouseZoomMultiplier = 5.0f;
        public float minZoomDistance = 20.0f;
        public float maxZoomDistance = 200.0f;
        public float smoothingFactor = 0.1f;
        public float goToSpeed = 0.1f;
        public bool useKeyboardInput = true;
        public bool useMouseInput = true;
        public bool adaptToTerrainHeight = true;
        public bool increaseSpeedWhenZoomedOut = true;
        public bool correctZoomingOutRatio = true;
        public bool smoothing = true;

        public GameObject objectToFollow;
        public Vector3 cameraTarget;

        private float currentCameraDistance;
        private Vector3 lastMousePos;
        private Vector3 lastPanSpeed = Vector3.zero;
        private Vector3 goingToCameraTarget = Vector3.zero;
        private bool doingAutoMovement = false;

        void Start()
        {
            currentCameraDistance = minZoomDistance + ((maxZoomDistance - minZoomDistance) / 2.0f);
            lastMousePos = Vector3.zero;
        }

        void Update()
        {
            UpdatePanning();
            UpdateZooming();
            UpdatePosition();
            UpdateAutoMovement();
            lastMousePos = Input.mousePosition;

        }



        void GoTo(Vector3 position)
        {
            doingAutoMovement = true;
            goingToCameraTarget = position;
            objectToFollow = null;
        }

        void Follow(GameObject gameObjectToFollow)
        {
            objectToFollow = gameObjectToFollow;
        }

        private void UpdatePanning()
        {
            Vector3 moveVector = new Vector3(0, 0, 0);
            if (useKeyboardInput)
            {
                if (Input.GetKey(KeyCode.A))
                {
                    moveVector += new Vector3(-1, 0, 0);
                }
                if (Input.GetKey(KeyCode.S))
                {
                    moveVector += new Vector3(0, 0, -1);
                }
                if (Input.GetKey(KeyCode.D))
                {
                    moveVector += new Vector3(1, 0, 0);
                }
                if (Input.GetKey(KeyCode.W))
                {
                    moveVector += new Vector3(0, 0, 1);
                }
            }

            if (useMouseInput)
            {
                if (Input.GetMouseButton(2) && Input.GetKey(KeyCode.LeftShift))
                {
                    Vector3 deltaMousePos = (Input.mousePosition - lastMousePos);
                    moveVector += new Vector3(-deltaMousePos.x, 0, -deltaMousePos.y) * mousePanMultiplier;
                }
            }

            if (moveVector != Vector3.zero)
            {
                objectToFollow = null;
                doingAutoMovement = false;
            }

            var effectivePanSpeed = moveVector;
            if (smoothing)
            {
                effectivePanSpeed = Vector3.Lerp(lastPanSpeed, moveVector, smoothingFactor);
                lastPanSpeed = effectivePanSpeed;
            }

            float oldRotation = transform.localEulerAngles.x;

            Vector3 _tmp = transform.localEulerAngles;
            _tmp.x = 0.0f;
            transform.localEulerAngles = _tmp;

            float panMultiplier = increaseSpeedWhenZoomedOut ? (Mathf.Sqrt(currentCameraDistance)) : 1.0f;
            cameraTarget = cameraTarget + transform.TransformDirection(effectivePanSpeed) * panSpeed * panMultiplier * Time.deltaTime;

            Vector3 _tmp1 = transform.localEulerAngles;
            _tmp1.x = oldRotation;
            transform.localEulerAngles = _tmp1;
        }

        private void UpdateRotation()
        {
            if (useKeyboardInput)
            {
                if (Input.GetKey(KeyCode.Q))
                {
                }
                if (Input.GetKey(KeyCode.E))
                {

                }
            }
        }

        private void UpdateZooming()
        {
            float deltaZoom = 0.0f;
            //if (useKeyboardInput)
            {
                if (Input.GetKey(KeyCode.F))
                {
                    deltaZoom = 1.0f;
                }
                if (Input.GetKey(KeyCode.R))
                {
                    deltaZoom = -1.0f;
                }
            }
            if (useMouseInput)
            {
                float scroll = Input.GetAxis("Mouse ScrollWheel");
                deltaZoom -= scroll * mouseZoomMultiplier;
            }
            float zoomedOutRatio = correctZoomingOutRatio ? (currentCameraDistance - minZoomDistance) / (maxZoomDistance - minZoomDistance) : 0.0f;
            currentCameraDistance = Mathf.Max(minZoomDistance, Mathf.Min(maxZoomDistance, currentCameraDistance + deltaZoom * Time.deltaTime * zoomSpeed * (zoomedOutRatio * 2.0f + 1.0f)));
        }

        private void UpdatePosition()
        {
            if (objectToFollow != null)
            {
                cameraTarget = Vector3.Lerp(cameraTarget, objectToFollow.transform.position, goToSpeed);
            }

            transform.position = cameraTarget;
            transform.Translate(Vector3.back * currentCameraDistance);

            if (adaptToTerrainHeight && terrain != null)
            {
                Vector3 _tmp2 = transform.position;
                _tmp2.y = Mathf.Max(terrain.SampleHeight(transform.position) + terrain.transform.position.y + 10.0f, transform.position.y);
                transform.position = _tmp2;
            }
        }

        private void UpdateAutoMovement()
        {
            if (doingAutoMovement)
            {
                cameraTarget = Vector3.Lerp(cameraTarget, goingToCameraTarget, goToSpeed);
                if (Vector3.Distance(goingToCameraTarget, cameraTarget) < 1.0f)
                {
                    doingAutoMovement = false;
                }
            }
        }
    }
}