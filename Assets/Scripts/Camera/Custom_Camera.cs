using UnityEngine;
using System.Collections;

public class Custom_Camera : MonoBehaviour {

    private static float halfFieldOfView;
    private static float planeAspect;
    private static float halfPlaneHeight;
    private static float halfPlaneWidth;


    public Transform cameraPivot;
    public Camera camera;
    public float distance = 5f;
    public float distanceMax = 30f;
    public float camDistanceSpeed = 0.7f;
    public float speed = 0.8f;
    public float mouseScroll = 15f;

    private Vector3 desiredPosition;
    private float desiredDistance;
    private float lastDistance;
    private bool constraint;
    private float distanceVel;

    private float rotateX = 0f;
    private float rotateY = 15f;
    private float rotateXMin = -89.5f;
    private float rotateYMax = 89.5f;


    
    void Start () {
        GameObject playerCam = GameObject.FindGameObjectWithTag("PlayerCamera");
        camera = playerCam.GetComponent<Camera>();

        distance = Mathf.Clamp(distance, 0.05f, distanceMax);
        desiredDistance = distance;

        
    }
	
	void LateUpdate () {
        if (cameraPivot == null)
        {
            Debug.Log("Error: No cameraPivot found!");
            return;
        }

        GetInput();

        GetDesiredPosition();

        PositionUpdate();
	}

    void GetInput()
    {
        if (Input.GetKey(KeyCode.Q))
        {
            rotateX += speed;
        }
        else if (Input.GetKey(KeyCode.E))
        {
            rotateX -= speed;
        }

        desiredDistance = desiredDistance - Input.GetAxis("Mouse ScrollWheel") * mouseScroll;

        if (desiredDistance > distanceMax)
            desiredDistance = distanceMax;

        if (desiredDistance < 0.05)
            desiredDistance = 0.05f;

    }
    
    void GetDesiredPosition()
    {
        distance = desiredDistance;
        desiredPosition = GetCameraPosition(rotateX, rotateY, distance);

        float closestDistance;
        constraint = false;

        closestDistance = CheckCameraClipPlane(cameraPivot.position, desiredPosition);

        if (closestDistance != -1)
        {
            distance = closestDistance;
            desiredPosition = GetCameraPosition(rotateY, rotateX, distance);

            constraint = true;
        }


        distance -= camera.nearClipPlane;

        if (lastDistance < distance || !constraint)
            distance = Mathf.SmoothDamp(lastDistance, distance, ref distanceVel, camDistanceSpeed);

        if (distance < 0.05)
            distance = 0.05f;

        lastDistance = distance;

        desiredPosition = GetCameraPosition(rotateY, rotateX, distance); // if the camera view was blocked, then this is the new "forced" position
    }

    void PositionUpdate()
    {
        transform.position = desiredPosition;

        if (distance > 0.05)
            transform.LookAt(cameraPivot);
    }

    float CheckCameraClipPlane(Vector3 from, Vector3 to)
    {
        float nearDistance = -1f;

        RaycastHit hit;

        ClipPlanePoints clipPlanePoints = GetClipPlaneAt(to);

        Debug.DrawLine(from, clipPlanePoints.UpperLeft);
        Debug.DrawLine(from, clipPlanePoints.LowerLeft);
        Debug.DrawLine(from, clipPlanePoints.UpperRight);
        Debug.DrawLine(from, clipPlanePoints.LowerRight);

        Debug.DrawLine(from - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlanePoints.UpperLeft, Color.cyan);
        Debug.DrawLine(from + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlanePoints.UpperRight, Color.cyan);
        Debug.DrawLine(from - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlanePoints.LowerLeft, Color.cyan);
        Debug.DrawLine(from + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlanePoints.LowerRight, Color.cyan);

        Debug.DrawLine(from, to, Color.red);

        Debug.DrawLine(clipPlanePoints.UpperLeft, clipPlanePoints.UpperRight);
        Debug.DrawLine(clipPlanePoints.UpperRight, clipPlanePoints.LowerRight);
        Debug.DrawLine(clipPlanePoints.LowerRight, clipPlanePoints.LowerLeft);
        Debug.DrawLine(clipPlanePoints.LowerLeft, clipPlanePoints.UpperLeft);


        if (Physics.Linecast(from, to, out hit) && hit.collider.tag != "Player")
            nearDistance = hit.distance - camera.nearClipPlane;

        if (Physics.Linecast(from - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlanePoints.UpperLeft, out hit) && hit.collider.tag != "Player")
            if (hit.distance < nearDistance || nearDistance == -1)
                nearDistance = Vector3.Distance(hit.point + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, from);

        if (Physics.Linecast(from + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, clipPlanePoints.UpperRight, out hit) && hit.collider.tag != "Player")
            if (hit.distance < nearDistance || nearDistance == -1)
                nearDistance = Vector3.Distance(hit.point - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, from);

        if (Physics.Linecast(from - transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlanePoints.LowerLeft, out hit) && hit.collider.tag != "Player")
            if (hit.distance < nearDistance || nearDistance == -1)
                nearDistance = Vector3.Distance(hit.point + transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, from);

        if (Physics.Linecast(from + transform.right * halfPlaneWidth - transform.up * halfPlaneHeight, clipPlanePoints.LowerRight, out hit) && hit.collider.tag != "Player")
            if (hit.distance < nearDistance || nearDistance == -1)
                nearDistance = Vector3.Distance(hit.point - transform.right * halfPlaneWidth + transform.up * halfPlaneHeight, from);
        
        return nearDistance;
    }

    Vector3 GetCameraPosition(float xAxis, float yAxis, float distance)
    {
        Vector3 offset = new Vector3(0, 0, -distance);
        Quaternion rotation = Quaternion.Euler(xAxis, yAxis, 0);
        return cameraPivot.position + rotation * offset;
    }

    float ClampAngle(float angle, float min, float max)
    {
        while (angle < -360 || angle > 360)
        {
            if (angle < -360)
                angle += 360;
            if (angle > 360)
                angle -= 360;
        }

        return Mathf.Clamp(angle, min, max);
    }

    public struct ClipPlanePoints
    {
        public Vector3 UpperLeft;
        public Vector3 UpperRight;
        public Vector3 LowerLeft;
        public Vector3 LowerRight;
    }

    public static ClipPlanePoints GetClipPlaneAt(Vector3 pos)
    {
        var clipPlane = new ClipPlanePoints();

        GameObject playerCam = GameObject.FindGameObjectWithTag("PlayerCamera");
        Camera camera = playerCam.GetComponent<Camera>();

        if (camera == null)
            return clipPlane;

        Transform transform = camera.transform;
        float offset = camera.nearClipPlane;
        halfFieldOfView = (camera.fieldOfView / 2) * Mathf.Deg2Rad;
        planeAspect = camera.aspect;
        halfPlaneHeight = camera.nearClipPlane * Mathf.Tan(halfFieldOfView);
        halfPlaneWidth = halfPlaneHeight * planeAspect;


        clipPlane.UpperLeft = pos - transform.right * halfPlaneWidth;
        clipPlane.UpperLeft += transform.up * halfPlaneHeight;
        clipPlane.UpperLeft += transform.forward * offset;

        clipPlane.UpperRight = pos + transform.right * halfPlaneWidth;
        clipPlane.UpperRight += transform.up * halfPlaneHeight;
        clipPlane.UpperRight += transform.forward * offset;

        clipPlane.LowerLeft = pos - transform.right * halfPlaneWidth;
        clipPlane.LowerLeft -= transform.up * halfPlaneHeight;
        clipPlane.LowerLeft += transform.forward * offset;

        clipPlane.LowerRight = pos + transform.right * halfPlaneWidth;
        clipPlane.LowerRight -= transform.up * halfPlaneHeight;
        clipPlane.LowerRight += transform.forward * offset;


        return clipPlane;
    }
}
