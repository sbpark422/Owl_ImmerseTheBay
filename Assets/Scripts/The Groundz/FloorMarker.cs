using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FloorMarker : MonoBehaviour {

	// Use this for initialization
	public Sprite floorImage;
	 GameObject floorMarker;
	public GameObject ground;
	public float maxDistance = 5f;

	float scaleWeight = 10f;

	void Start () {

		//shadowImage = gameObject.GetComponent<SpriteRenderer> ();  // playerConfig
		floorMarker = new GameObject ("Floor Marker");
		floorMarker.AddComponent<SpriteRenderer> ();
		floorMarker.GetComponent<SpriteRenderer> ().sprite = floorImage;
		floorMarker.GetComponent<SpriteRenderer> ().color = new Color (Random.value, Random.value, Random.value);         //also scene dependent
		floorMarker.GetComponent<SpriteRenderer> ().sortingOrder = 1;
		floorMarker.transform.parent = this.transform;
		floorMarker.transform.localPosition = Vector3.zero;
		floorMarker.transform.localEulerAngles = Vector3.zero;
	}
	
	
	void LateUpdate ()
	{
		//also scene dependent
		if (ground)
        {
            floorMarker.GetComponent<SpriteRenderer>().sprite = floorImage;
			floorMarker.transform.localScale = transform.parent.localScale * Mathf.Clamp(GetDistance(),1.5f,3f);
			floorMarker.transform.position = new Vector3(transform.parent.position.x, ground.transform.position.y, transform.parent.position.z);
			floorMarker.GetComponent<SpriteRenderer>().color = new Color(Random.value, Random.value, Random.value, Mathf.Clamp( GetDistance(), 0.5f, 1f));
			floorMarker.transform.rotation = Quaternion.Euler(90,0,0);

		}

	}

	public float GetDistance() {
		float delta =  (transform.parent.position.y - ground.transform.position.y)/maxDistance;
		return  Mathf.Clamp(delta,0,1);
      

    }

    public void SetGroundObject(GameObject x)
    {
		ground = x;
    }
}
