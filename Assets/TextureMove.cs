using UnityEngine;
using System.Collections;

public class textureMove : MonoBehaviour {

	[Tooltip("A name of tag (defined in config.json)")]
	public string tagDirection = "Belt#Direction";
	[Tooltip("A name of tag (defined in config.json)")]
	public string tagMovement = "Belt#Movement";
	public float speed = 1.0f; 
	public bool inverseDimension;
	public bool inverseDirection;

	private float offset = 0;
	private Vector2 offsetOrig;
	private Vector2 offsetVec = new Vector2();
	private Renderer _myrenderer;
	private float x;
	private float y;
	private int dir;

	Communication com;
	// Use this for initialization
	void Start () {
		com = GameObject.Find("Communication").GetComponent<Communication>();
		_myrenderer = GetComponent<Renderer>();
		offsetOrig = _myrenderer.material.GetTextureOffset ("_MainTex");

	}
	
	// Update is called once per frame
	void FixedUpdate () {		

		if (com.GetTagValue(tagMovement))
		{	
			if (!com.GetTagValue(tagDirection))
			{
				dir = -1;
			}
            else
            {
				dir = 1;
            }

			if (inverseDirection)
			{
				dir *= -1;
			}

			offset += dir * (Time.fixedDeltaTime * speed);

			if (!inverseDimension)
			{
				//x = dir * offset + offsetOrig.x;
				x = offset + offsetOrig.x;
				y = 0;
			}
			else
			{
				x = 0;
				//y = dir * offset + offsetOrig.y;
				y = offset + offsetOrig.y;
			}
			offsetVec.x = x;
			offsetVec.y = y;
			_myrenderer.material.SetTextureOffset("_MainTex", offsetVec);
		}

		




	}
}
