using UnityEngine;
using System.Collections;
using UnityEngine.EventSystems;

[RequireComponent(typeof(MeshCollider))]

public class MoveObjectMouse : MonoBehaviour , IPointerClickHandler
{
	Rigidbody objRigidBody;
	MeshRenderer objMeshRederer;
	private Color origColor;
	private Vector3 screenPoint;
	private Vector3 offset;
	private RigidbodyConstraints originalConstraints;
    private bool wasKinematic;

	void Start(){
		objRigidBody = GetComponent<Rigidbody> ();
		objMeshRederer = GetComponent<MeshRenderer> ();
	}
	void OnMouseEnter(){
		origColor = objMeshRederer.material.color;
		objMeshRederer.material.color = Color.blue;
	}
	void OnMouseExit(){
		objMeshRederer.material.color = origColor;
	}

	public void OnPointerClick(PointerEventData eventData){
		
		if (eventData.button == PointerEventData.InputButton.Right){
			// Remove object on right click
			Destroy(gameObject);			
		}
	}


	void OnMouseDown()
	{
		screenPoint = Camera.main.WorldToScreenPoint(gameObject.transform.position);
		offset = gameObject.transform.position - Camera.main.ScreenToWorldPoint(new Vector3(Input.mousePosition.x, Input.mousePosition.y, screenPoint.z));
		
		// Save original constraints and kinematic state
        originalConstraints = objRigidBody.constraints;
        wasKinematic = objRigidBody.isKinematic;

        // Set new constraints for dragging
        objRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
        objRigidBody.isKinematic = true;
	}

	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
		transform.position = curPosition;
	}
	void OnMouseUp(){
		// Restore original constraints and kinematic state
        objRigidBody.constraints = originalConstraints;
        objRigidBody.isKinematic = wasKinematic;
	}

}