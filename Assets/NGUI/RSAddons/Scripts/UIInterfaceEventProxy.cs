using UnityEngine;
using System.Collections.Generic;

public class UIInterfaceEventProxy : MonoBehaviour 
{
	public List<EventDelegate> onClick = new List<EventDelegate> ();
	public List<EventDelegate> onTooltipOn = new List<EventDelegate> ();
	public List<EventDelegate> onTooltipOff = new List<EventDelegate> ();
	public List<EventDelegate> onPressOn = new List<EventDelegate> ();
	public List<EventDelegate> onPressOff = new List<EventDelegate> ();

	void OnClick()
	{
		for (int i = 0; i < onClick.Count; i++) {
			onClick[i].Execute();
		}
	}

	void OnTooltip(bool show)
	{
		if (show) {
			for (int i = 0; i < onTooltipOn.Count; i++) {
				onTooltipOn[i].Execute();
			}
		} else {
			for (int i = 0; i < onTooltipOff.Count; i++) {
				onTooltipOff[i].Execute();
			}
		}
	}

	void OnPress(bool press)
	{
		if (press) {
			for (int i = 0; i < onPressOn.Count; i++) {
				onPressOn[i].Execute();
			}
		} else {
			for (int i = 0; i < onPressOff.Count; i++) {
				onPressOff[i].Execute();
			}
		}
	}

}
