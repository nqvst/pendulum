using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Oscillator : MonoBehaviour {

	[SerializeField] float frequency = 440.0f;
	private float increment;
	private float phase;
	private float samplingFrequency = 48000.0f;

	[SerializeField] float gain;
	[SerializeField] private float maxVolume = 0.1f;

	float[] scale;

	Rigidbody2D rb;

	[SerializeField] float influence = 1;
	[SerializeField] float glide = 1;

	float targetFrequency = 440;
	float targetGain = 0;

	float dist = 0;

	void OnEnable()
	{
		EventManager.StartListening(Events.PRESS, OnPress);
		EventManager.StartListening(Events.UNPRESS, OnUnPress);
	}

	void OnDisable()
	{
		EventManager.StopListening(Events.PRESS, OnPress);
		EventManager.StopListening(Events.UNPRESS, OnUnPress);
	}

	void Start () {
		rb = GetComponent<Rigidbody2D>();
		scale = new float[8];
		scale[0] = 440;
		scale[1] = 494;
		scale[2] = 554;
		scale[3] = 587;
		scale[4] = 659;
		scale[5] = 740;
		scale[6] = 831;
		scale[7] = 880;
	}

	public void SetDist(float d) {
		dist = d;
	}

	void OnAudioFilterRead(float[] data, int channels) {
		increment = frequency * 2.0f * Mathf.PI / samplingFrequency;

		for (int i = 0; i < data.Length; i += channels) {
			phase += increment;
			
			data[i] = gain * Mathf.Sin(phase);

			if (channels == 2) {
				data[i + 1] = data[i];
			}

			if (phase > (Mathf.PI * 2)) {
				phase = 0f;
			}
		}
	}


	float GetNextFreq() {
		return scale[Random.Range(0, scale.Length -1)];
	}

	void OnPress()
    {
		targetFrequency = GetNextFreq(); // 440 / (dist / 20); // scale[(int)dist%scale.Length];
		gain = maxVolume;                            //    gain = maxVolume;
	}

	void OnUnPress()
    {
		targetGain = 0.0f;
	}

	void FixedUpdate() {
		gain = Mathf.Lerp(gain, targetGain, Time.deltaTime);
		frequency += rb.velocity.y * influence * Time.deltaTime;
		frequency = Mathf.Lerp(frequency, targetFrequency, Time.deltaTime * glide);

	}
}
