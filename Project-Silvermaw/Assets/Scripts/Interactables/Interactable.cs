using System;
using System.Collections.Generic;
using System.Linq;
using UnityEngine;
using UnityEngine.Events;

[RequireComponent(typeof(Collider))]
public sealed class Interactable : MonoBehaviour
{
	[Serializable]
	public class UnityInteractableEvent : UnityEvent<GameObject> { }

	[Serializable]
	public class UnityInteractHoldEvent : UnityEvent<GameObject, float> { }

	//Events to add to in unity editor
	[SerializeField]
	private UnityInteractableEvent
		onHoverEnter = new UnityInteractableEvent(),
		onHover = new UnityInteractableEvent(),
		onHoverExit = new UnityInteractableEvent(), 
		onInteract = new UnityInteractableEvent();
	[SerializeField]
	private UnityInteractHoldEvent onInteractHold = new UnityInteractHoldEvent();

	//Use these to add events during runtime to corresponding unity events
	public event UnityAction<GameObject> OnHoverEnter, OnHover, OnHoverExit, OnInteract;
	public event UnityAction<GameObject, float> OnInteractHold;


	//Used to determine which unity event to invoke on a given GameObject
	private enum HoverState : byte { Entering, Holding, Exiting }

	private class SubjectState
	{
		public GameObject Subject;
		public HoverState State;
	}

	private class InteractInfo
	{
		public GameObject Subject;
		public float InteractStartTime;
		public bool Dirty;
	}

	//Holds GameObject.GetInstanceID() for all subjects that are hovering
	Dictionary<int, SubjectState> hoveringSubjects = new Dictionary<int, SubjectState>();

	//Holds GameObject.GetInstanceID() for all subjects that are interacting
	Dictionary<int, InteractInfo> interactingSubjects = new Dictionary<int, InteractInfo>();


	public void Hover(GameObject self)
	{
		if(!hoveringSubjects.ContainsKey(self.GetInstanceID()))
		{
			hoveringSubjects[self.GetInstanceID()] = new SubjectState { Subject=self, State=HoverState.Entering };
		}
		else
		{
			hoveringSubjects[self.GetInstanceID()].State = HoverState.Holding;
		}
	}

	public void Interact(GameObject self, bool allowNewInteractions = true)
	{
		if (!interactingSubjects.ContainsKey(self.GetInstanceID()))
		{
			if(allowNewInteractions)
			{
				interactingSubjects[self.GetInstanceID()] = new InteractInfo { Subject = self, InteractStartTime = Time.time, Dirty = false };
			}
		}
		else
		{
			interactingSubjects[self.GetInstanceID()].Dirty = false;
		}
	}

	private void Start()
	{
		//Set up public C# events to be called alongside private UnityEvents
		OnHoverEnter = OnHoverEnter ?? (go => { });
		onHoverEnter.AddListener(OnHoverEnter);

		OnHover = OnHover ?? (go => { });
		onHover.AddListener(OnHover);

		OnHoverExit = OnHoverExit ?? (go => { });
		onHoverExit.AddListener(OnHoverExit);

		OnInteract = OnInteract ?? (go => { });
		onInteract.AddListener(OnInteract);

		OnInteractHold = OnInteractHold ?? ((go, time) => { });
		onInteractHold.AddListener(OnInteractHold);
	}

	private void LateUpdate()
	{
		IEnumerable<SubjectState> exitingSubjects = hoveringSubjects.Values.Where(ss => ss.State == HoverState.Exiting);

		foreach (SubjectState subjectState in exitingSubjects.ToList())
		{
			onHoverExit.Invoke(subjectState.Subject);
			hoveringSubjects.Remove(subjectState.Subject.GetInstanceID());
		}

		foreach (var keyValuePair in hoveringSubjects)
		{
			HoverState state = keyValuePair.Value.State;
			GameObject subject = keyValuePair.Value.Subject;

			//Run either onHoverEnter or onHover, depending on the state
			switch (state)
			{
				case HoverState.Entering:
					onHoverEnter.Invoke(subject);
					break;
				case HoverState.Holding:
					onHover.Invoke(subject);
					break;
				//Default case should never happen; by this point, all Exiting states should have been removed
				default:
					break;
			}

			//Increment the state
			keyValuePair.Value.State = (HoverState)((byte)state + 1);
		}

		IEnumerable<InteractInfo> finishingInteractions = interactingSubjects.Values.Where(ii => ii.Dirty);

		foreach (InteractInfo interactInfo in finishingInteractions.ToList())
		{
			interactingSubjects.Remove(interactInfo.Subject.GetInstanceID());
		}

		foreach (var keyValuePair in interactingSubjects)
		{
			float InteractTime = Time.time - keyValuePair.Value.InteractStartTime;
			GameObject subject = keyValuePair.Value.Subject;

			if (InteractTime == 0f)
			{
				onInteract.Invoke(subject);
			}
			onInteractHold.Invoke(subject, InteractTime);
			keyValuePair.Value.Dirty = true;
		}
	}
}