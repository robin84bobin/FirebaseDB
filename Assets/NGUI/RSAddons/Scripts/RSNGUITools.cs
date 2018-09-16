using UnityEngine;
using System.Collections.Generic;

public static class RSNGUITools
{
	public static void SetActive(GameObject go, bool state)
	{
		NGUITools.SetActive(go, state, false);
	}
	
	//aligns a set of rects, without relative changes, around point
	public static void AlignCenter(List<UIRect> elements, Transform relativeTo, bool horizontal = true)
	{
		float minPos = float.MaxValue;
		float maxPos = float.MinValue;
		for (int i = 0; i < elements.Count; ++i) {
			Vector3[] corners = elements[i].GetSides(elements[i].parent.transform); //ltrb
			//Vector3[] corners = elements[i].worldCorners;
			float firstPos = horizontal ? corners[0].x : corners[3].y;
			float secondPos = horizontal ? corners[2].x : corners[1].y;
			minPos = firstPos < minPos ? firstPos : minPos;
			maxPos = secondPos > maxPos ? secondPos : maxPos;
		}
		float halfSize = (maxPos - minPos) / 2;
		float moveBy = -(minPos + halfSize);

		for (int i = 0; i < elements.Count; ++i) {
			Vector3 pos = elements[i].transform.localPosition;
			if (horizontal) {
				pos.x += moveBy;
			} else {
				pos.y += moveBy;
			}
			elements[i].transform.localPosition = pos;
		}
	}
}
