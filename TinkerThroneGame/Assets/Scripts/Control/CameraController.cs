using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CameraController : MonoBehaviour
{
	[SerializeField] private float movementSpeed = 1.0f;
	[SerializeField] private float zoomSpeed = 1.0f;
	[SerializeField] private float boostFactor = 5.0f;
	[SerializeField] private float rotationSpeed = 1.0f;
	[SerializeField] private Slider movementSpeedSlider = null;
	[SerializeField] private Slider zoomSpeedSlider = null;
	[SerializeField] private Slider rotationSpeedSlider = null;
	[SerializeField] private MinMax xRestriction = new MinMax(-1.0f, 1.0f);
	[SerializeField] private MinMax yRestriction = new MinMax(-1.0f, 1.0f);
	[SerializeField] private MinMax zRestriction = new MinMax(-1.0f, 1.0f);

	private new Transform transform = null;
	private float pitch = 0.0f;
	private float yaw = 0.0f;
	private float roll = 0.0f;

	private void Start()
	{
		transform = gameObject.GetComponent<Transform>();

		Vector3 startrotation = transform.rotation.eulerAngles;
		pitch = startrotation.x;
		yaw = startrotation.y;
		roll = startrotation.z;

		UpdateMovementSpeed();
		UpdateZoomSpeed();
		UpdateRotationSpeed();
	}

	private void Update()
	{
		// Boost on Shift
		float boost = 1.0f;
		if(Input.GetAxis("Boost") > 0.0f)
		{
			boost = boostFactor;
		}

		// Hide and lock Cursor when the Rotation Button is pressed and save Mouse Rotation
		if(Input.GetButton("Rotate Camera"))
		{
			Cursor.visible = false;
			Cursor.lockState = CursorLockMode.Locked;

			yaw += Input.GetAxis("Mouse X") * rotationSpeed;
			pitch -= Input.GetAxis("Mouse Y") * rotationSpeed;
		}
		// Show and unlock Cursor on MMB Release
		else
		{
			Cursor.visible = true;
			Cursor.lockState = CursorLockMode.None;
		}

		// Get Input Directions
		Vector3 direction = new Vector3();
		if(Input.GetAxis("Vertical") > 0.0f)
		{
			direction += Vector3.forward;
		}
		if(Input.GetAxis("Horizontal") < 0.0f)
		{
			direction += Vector3.left;
		}
		if(Input.GetAxis("Vertical") < 0.0f)
		{
			direction += Vector3.back;
		}
		if(Input.GetAxis("Horizontal") > 0.0f)
		{
			direction += Vector3.right;
		}

		// Rotate and translate Camera
		transform.rotation = Quaternion.Euler(pitch, yaw, roll);
		Vector3 forward = transform.forward;
		transform.position += Quaternion.LookRotation(new Vector3(forward.x, 0.0f, forward.z)) * direction.normalized * movementSpeed * boost;
		transform.position += forward * Input.GetAxis("Mouse ScrollWheel") * zoomSpeed * transform.position.y * boost;

		// Enforce Constraints
		if(transform.position.x < xRestriction.min)
		{
			transform.position = new Vector3(xRestriction.min, transform.position.y, transform.position.z);
		}
		else if(transform.position.x > xRestriction.max)
		{
			transform.position = new Vector3(xRestriction.max, transform.position.y, transform.position.z);
		}
		if(transform.position.y < yRestriction.min)
		{
			transform.position = new Vector3(transform.position.x, yRestriction.min, transform.position.z);
		}
		else if(transform.position.y > yRestriction.max)
		{
			transform.position = new Vector3(transform.position.x, yRestriction.max, transform.position.z);
		}
		if(transform.position.z < zRestriction.min)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, zRestriction.min);
		}
		else if(transform.position.z > zRestriction.max)
		{
			transform.position = new Vector3(transform.position.x, transform.position.y, zRestriction.max);
		}
	}

	public void UpdateMovementSpeed()
	{
		if(movementSpeedSlider != null && movementSpeedSlider.gameObject.activeSelf)
		{
			movementSpeed = movementSpeedSlider.value;
		}
	}

	public void UpdateZoomSpeed()
	{
		if(zoomSpeedSlider != null && zoomSpeedSlider.gameObject.activeSelf)
		{
			zoomSpeed = zoomSpeedSlider.value;
		}
	}

	public void UpdateRotationSpeed()
	{
		if(rotationSpeedSlider != null && rotationSpeedSlider.gameObject.activeSelf)
		{
			rotationSpeed = rotationSpeedSlider.value;
		}
	}
}
