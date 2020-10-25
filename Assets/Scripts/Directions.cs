using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Directions : MonoBehaviour {

	Transform goal;
	new Transform camera;

	LineRenderer lineRenderer;

	void Start () {
		camera = Camera.main.transform;
		lineRenderer = GetComponent<LineRenderer>();
		//lineRenderer.SetPosition(1, (Vector2) transform.position);
	}


	void LateUpdate () {
		Vector2 dir = (camera.position - transform.position).normalized;
		lineRenderer.SetPosition(0, (Vector2)camera.position - dir * 20);
		lineRenderer.SetPosition(1, (Vector2)lineRenderer.GetPosition(0) - dir * 20);
	}
}
