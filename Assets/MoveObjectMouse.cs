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
		objRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
	}

	void OnMouseDrag()
	{
		Vector3 curScreenPoint = new Vector3 (Input.mousePosition.x, Input.mousePosition.y, screenPoint.z);
		Vector3 curPosition = Camera.main.ScreenToWorldPoint (curScreenPoint) + offset;
		transform.position = curPosition;
		objRigidBody.constraints = RigidbodyConstraints.FreezeRotation;
		objRigidBody.isKinematic = true;
	}
	void OnMouseUp(){
		objRigidBody.constraints = RigidbodyConstraints.None;
		objRigidBody.isKinematic = false;
	}

}