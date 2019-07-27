using System.Collections.Generic;
using System.Linq;
using UnityEngine;

public class GlowObjectCmd : MonoBehaviour
{
	public Color GlowColor;

	public Renderer[] IgnoreRenderers;

	public Renderer[] Renderers { get; private set; }

	public Color CurrentColor { get; private set; }
	public float FadeInTime = 0.25f;
	public float FadeOutTime = 1;

	public float progress = 0;
	public float goal = 0;

	void Start()
	{
		Renderers = GetComponentsInChildren<Renderer>().Where(r => !IgnoreRenderers.Contains(r)).ToArray();
		GlowController.RegisterObject(this);
	}

	public void EnableGlow(GameObject subject)
	{
		goal = 1;
	}

	public void DisableGlow(GameObject subject)
	{
		goal = 0;
	}

	private void OnDisable()
	{
		goal = 0;
		progress = 0;
		CurrentColor = Color.black;
	}

	private void Update()
	{
		if(goal == 0)
		{
			progress = Mathf.Max(progress - (Time.deltaTime / FadeOutTime), 0);
		}
		else
		{
			progress = Mathf.Min(progress + (Time.deltaTime / FadeInTime), 1);
		}

		CurrentColor = Color.Lerp(Color.black, GlowColor, progress);
	}
}
