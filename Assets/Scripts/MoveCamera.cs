// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
// for using the mouse displacement for calculating the amount of camera movement and panning code.

using UnityEngine;
using System.Collections;

public class MoveCamera : MonoBehaviour
{
	//
	// VARIABLES
	//

	public float turnSpeed = 4.0f;      // Speed of camera turning when mouse moves in along an axis
	public float panSpeed = 4.0f;       // Speed of the camera when being panned
	public float zoomSpeed = 4.0f;      // Speed of the camera going back and forth
	// RR: 1, 10, 0
	// L2N: 0.5f, 10, 1
	public Vector3 cameraPositionTop = new Vector3(0.5f, 10, 1);
	public float cameraOrtographicSizeTop = 4f;
	// RR: 15, 3, 0
	// L2N: 15, 2, 0.9f
	public Vector3 cameraPositionSide = new Vector3(15, 2, 0.9f);
	// RR: 3.5f
	// L2N: 2.7f
	public float cameraOrtographicSizeSide = 2.7f;
	
	private Vector3 mouseOrigin;    // Position of cursor when mouse dragging starts
	private bool isPanning;     // Is the camera being panned?
	private bool isRotating;    // Is the camera being rotated?
	private bool isZooming;     // Is the camera zooming?
	private bool isShiftPressed;

	// Starting point
	Vector3 cameraStartPoint = Vector3.zero;
	Quaternion cameraStartRotation;

	Quaternion cameraRotationTop = Quaternion.Euler(new Vector3(90, 0, 0));
	Quaternion cameraRotationSide = Quaternion.Euler(new Vector3(0, 270, 0));


	void Start()
	{
		cameraStartPoint = transform.position;
		cameraStartRotation = transform.rotation;
	}

	void OnGUI()
	{
		isShiftPressed = Event.current.shift;
	}

	//
	// UPDATE
	//
	void Update()
	{
		float scrollWheel = 0;

		// If holding shift, camera move is disabled
		if (isShiftPressed)
		{
			// Get the left mouse button
			if (Input.GetMouseButtonDown(0))
			{
				// Get mouse origin
				mouseOrigin = Input.mousePosition;
				isRotating = true;
			}

			// Get the right mouse button
			if (Input.GetMouseButtonDown(1))
			{
				// Get mouse origin
				mouseOrigin = Input.mousePosition;
				isPanning = true;
			}

			// Get the middle mouse button
			if (Input.GetMouseButtonDown(2))
			{
				// Get mouse origin
				mouseOrigin = Input.mousePosition;
				isZooming = true;
			}

			scrollWheel = Input.GetAxis("Mouse ScrollWheel");

			// Disable movements on button release
			if (!Input.GetMouseButton(0))
				isRotating = false;
			if (!Input.GetMouseButton(1))
				isPanning = false;
			if (!Input.GetMouseButton(2))
				isZooming = false;
		}
		// Rotate camera along X and Y axis
		if (isRotating)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
			transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
		}

		// Move the camera on it's XY plane
		if (isPanning)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
			transform.Translate(move, Space.Self);
		}

		// Move the camera linearly along Z axis
		if (isZooming)
		{
			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

			if (Camera.main.orthographic)
			{
				Camera.main.orthographicSize = Camera.main.orthographicSize - pos.y * zoomSpeed;
			}
			else
			{
				Vector3 move = pos.y * zoomSpeed * transform.forward;
				transform.Translate(move, Space.World);
			}
		}
		// Scrollwheel
		if (scrollWheel != 0)
		{
			if (Camera.main.orthographic)
			{
				Camera.main.orthographicSize = Camera.main.orthographicSize - scrollWheel;
			}
			else
			{
				Vector3 moveScroll = scrollWheel * zoomSpeed * transform.forward;
				transform.Translate(moveScroll, Space.World);
			}
		}
	}

	public void setCameraTop()
	{
		transform.position = cameraPositionTop;
		transform.rotation = cameraRotationTop;
		Camera.main.orthographic = true;
		Camera.main.orthographicSize = cameraOrtographicSizeTop;
	}
	public void setCameraSide()
	{
		transform.position = cameraPositionSide;
		transform.rotation = cameraRotationSide;
		Camera.main.orthographic = true;
		Camera.main.orthographicSize = cameraOrtographicSizeSide;
	}

	public void resetCamera()
	{
		transform.position = cameraStartPoint;
		transform.rotation = cameraStartRotation;
		Camera.main.orthographic = false;
	}
}

//// Credit to damien_oconnell from http://forum.unity3d.com/threads/39513-Click-drag-camera-movement
//// for using the mouse displacement for calculating the amount of camera movement and panning code.

//using UnityEngine;
//using System.Collections;

//public class MoveCamera : MonoBehaviour 
//{
//	//
//	// VARIABLES
//	//

//	public float turnSpeed = 4.0f;		// Speed of camera turning when mouse moves in along an axis
//	public float panSpeed = 4.0f;		// Speed of the camera when being panned
//	public float zoomSpeed = 4.0f;		// Speed of the camera going back and forth

//	private Vector3 mouseOrigin;	// Position of cursor when mouse dragging starts
//	private bool isPanning;		// Is the camera being panned?
//	private bool isRotating;	// Is the camera being rotated?
//	private bool isZooming;		// Is the camera zooming?

//	// Starting point
//	public Vector3 cameraStartPoint = Vector3.zero;
//	public Quaternion cameraStartRotation;

//	void Start()
//	{
//		cameraStartPoint = transform.position;
//		cameraStartRotation = transform.rotation;
//	}
//	//
//	// UPDATE
//	//
//	void Update () 
//	{
//		// Get the left mouse button
//		if(Input.GetMouseButtonDown(0))
//		{
//			// Get mouse origin
//			mouseOrigin = Input.mousePosition;
//			isRotating = true;
//		}

//		// Get the right mouse button
//		if(Input.GetMouseButtonDown(1))
//		{
//			// Get mouse origin
//			mouseOrigin = Input.mousePosition;
//			isPanning = true;
//		}

//		// Get the middle mouse button
//		if(Input.GetMouseButtonDown(2))
//		{
//			// Get mouse origin
//			mouseOrigin = Input.mousePosition;
//			isZooming = true;
//		}

//		// Disable movements on button release
//		if (!Input.GetMouseButton(0)) isRotating=false;
//		if (!Input.GetMouseButton(1)) isPanning=false;
//		if (!Input.GetMouseButton(2)) isZooming=false;

//		// Rotate camera along X and Y axis
//		if (isRotating)
//		{
//			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

//			transform.RotateAround(transform.position, transform.right, -pos.y * turnSpeed);
//			transform.RotateAround(transform.position, Vector3.up, pos.x * turnSpeed);
//		}

//		// Move the camera on it's XY plane
//		if (isPanning)
//		{
//			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);

//			Vector3 move = new Vector3(pos.x * panSpeed, pos.y * panSpeed, 0);
//			transform.Translate(move, Space.Self);
//		}

//		// Move the camera linearly along Z axis
//		if (isZooming)
//		{
//			Vector3 pos = Camera.main.ScreenToViewportPoint(Input.mousePosition - mouseOrigin);
//			Vector3 move = pos.y * zoomSpeed * transform.forward; 
//			transform.Translate(move, Space.World);
//		}
//		// Scrollwheel
//		Vector3 moveScroll = Input.GetAxis ("Mouse ScrollWheel") * zoomSpeed * transform.forward;
//		transform.Translate(moveScroll, Space.World);

//	}

//	public void resetCamera(){
//		transform.position = cameraStartPoint;
//		transform.rotation = cameraStartRotation;
//	}
//	public void infoCamera(){
//		if (GameObject.FindWithTag ("Dialog_error_PLCSIM") == null && GameObject.FindWithTag ("Dialog_camera_info") == null) {
//			Dialog.MessageBox (
//				"Dialog_camera_info", 
//				"Control and navigation", 
//				"Use your mouse buttons to control the camera:\n" +
//				"    left - rotation,\n" +
//				"    middle/scroll wheel - zoom,\n" +
//				"    right - translation.\n\n" +
//				"Use keys to control the buttons:\n" +
//				"    Q - key,\n" +
//				"    W - high-left,\n" +
//				"    E - high-right,\n" +
//				"    S - low-left,\n" +
//				"    D - low-right,\n" +
//				"    A - green.\n", 
//				"Close", 
//				() => {;},
//				heightMax: 250,
//				pos_y: 20);
//		}
//	}
//}