using UnityEngine;
using System.Collections;

public class Follow : MonoBehaviour
{

	public Transform target;
	public GameObject finish;
	Vector3 targetPosition;
	[SerializeField] float smoothness = 2f;
	[SerializeField] float radius = 2;
	
	Vector3 startPosition = new Vector3(0,0,-10);

	private bool hasStarted = false;

	void Start ()
	{
		target = GameObject.FindGameObjectWithTag("Player").transform;
		targetPosition = startPosition;
	}

	void OnEnable()
    {
		EventManager.StartListening(Events.START, OnStart);
    }

    private void OnDisable()
    {
		EventManager.StopListening(Events.START, OnStart);
    }

	private void OnStart()
    {
		hasStarted = true;
	}



	void FixedUpdate ()
	{
		if (!hasStarted) return;

		if (target != null) {

			if(target.gameObject == null){
				targetPosition = startPosition;
			} else{
				targetPosition = new Vector3 (target.transform.position.x, target.transform.position.y, -10);
			}

		}

		Camera.main.WorldToScreenPoint(transform.position);


		//Debug.Log(Camera.main.WorldToScreenPoint(targetPosition) + " - " + Screen.height);

		float distance = Vector2.Distance(targetPosition, transform.position);
		if (distance > radius) {
			transform.position = Vector3.Lerp (transform.position, targetPosition, smoothness * Time.fixedDeltaTime * distance);
		}

	}
}
