using UnityEngine;
using System.Collections;


/***
 * 
 *   Implementation of a butter worth filter.
 * 
 * */


public class BWFilter {
	
	public readonly float resonance;
	
	public readonly float frequency;
	public readonly int sampleRate;
	public readonly PassType passType;
	
	public readonly float c, a1, a2, a3, b1, b2;

	public float[] inputHistory = new float[2];

	public float[] outputHistory = new float[3];
	
	public BWFilter(float frequency, int sampleRate, PassType passType, float resonance)
	{
		this.resonance = resonance;
		this.frequency = frequency;
		this.sampleRate = sampleRate;
		this.passType = passType;
		
		switch (passType)
		{
			case PassType.Lowpass:
				c = 1.0f / (float)Mathf.Tan(Mathf.PI * frequency / sampleRate);
				a1 = 1.0f / (1.0f + resonance * c + c * c);
				a2 = 2f * a1;
				a3 = a1;
				b1 = 2.0f * (1.0f - c * c) * a1;
				b2 = (1.0f - resonance * c + c * c) * a1;
				break;
			case PassType.Highpass:
				c = (float)Mathf.Tan(Mathf.PI * frequency / sampleRate);
				a1 = 1.0f / (1.0f + resonance * c + c * c);
				a2 = -2f * a1;
				a3 = a1;
				b1 = 2.0f * (c * c - 1.0f) * a1;
				b2 = (1.0f - resonance * c + c * c) * a1;
				break;
			}
	}
	
	public enum PassType
	{
		Highpass,
		Lowpass,
	}
	
	public void Update(float newInput)
	{
		float newOutput = a1 * newInput + a2 * this.inputHistory[0] + a3 * this.inputHistory[1] - b1 * this.outputHistory[0] - b2 * this.outputHistory[1];
		
		this.inputHistory[1] = this.inputHistory[0];
		this.inputHistory[0] = newInput;
		
		this.outputHistory[2] = this.outputHistory[1];
		this.outputHistory[1] = this.outputHistory[0];
		this.outputHistory[0] = newOutput;
	}
	
	public float Value
	{
		get { return this.outputHistory[0]; }
	}
}
