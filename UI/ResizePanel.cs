using UnityEngine;
using UnityEngine.EventSystems;

[RequireComponent(typeof(EventTrigger))]
public class ResizePanel : MonoBehaviour
{

	//EventTrigger eventTrigger;
	RectTransform cRT;
	public RectTransform target;
	float minHeight = 8;
	float maxHeight = 1600;

	void Start() {
		cRT = GetComponentInParent<Canvas>().GetComponent<RectTransform>();
		//eventTrigger = GetComponent<EventTrigger>();
		//eventTrigger.AddEventTrigger(OnDrag, EventTriggerType.Drag);
	}
	/*
	void OnDrag(BaseEventData data)
	{
		PointerEventData ped = (PointerEventData)data;

		float ratio = cRT.sizeDelta.y / Screen.height;
		float height = Mathf.Clamp(target.sizeDelta.y + ped.delta.y * ratio, minHeight, maxHeight);
		target.sizeDelta = new Vector2(target.sizeDelta.x, height);
	}
	*/

	public void DoTheThing(BaseEventData data) {
		PointerEventData ped = (PointerEventData)data;
		float ratio = cRT.sizeDelta.y / Screen.height;
		float height = Mathf.Clamp(target.sizeDelta.y + ped.delta.y * ratio, minHeight, maxHeight);
		target.sizeDelta = new Vector2(target.sizeDelta.x, height);
	}
}