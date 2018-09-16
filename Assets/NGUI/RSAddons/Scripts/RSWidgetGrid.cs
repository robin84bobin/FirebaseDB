using UnityEngine;
using System.Collections.Generic;

public class RSWidgetGrid : UIGrid 
{

	public Rect GetContentSizeRect()
	{
		Rect resultRect = new Rect();

		var list = GetChildList();
		for (int i = 0; i < list.Count; i++) {
			UIWidget widget = list[i].GetComponent<UIWidget>();
			if (arrangement == Arrangement.Horizontal){
				if (widget.height > resultRect.height){
					resultRect.height = widget.height;
				}
				resultRect.width += widget.width;
			}
			if (arrangement == Arrangement.Vertical){
				if(widget.width > resultRect.width){
					resultRect.width = widget.width;
				}
				resultRect.height += widget.height;
			}
		}

		return resultRect;
	}

	override protected void ResetPosition(List<Transform> list)
	{
		//base.ResetPosition(list);
		//return;

		mReposition = false;
		
		int x = 0;
		int y = 0;
		int maxX = 0;
		int maxY = 0;
		Transform myTrans = transform;
		
		// Re-add the children in the same order we have them in and position them accordingly
		Vector3 prevPos = Vector3.zero;
		for (int i = 0, imax = list.Count; i < imax; ++i)
		{
			UIWidget rect = list[i].GetComponent<UIWidget>();
			UIWidget prevRect = i > 0 ? list[i-1].GetComponent<UIWidget>() : null;

			cellWidth = rect.width * .5f + (prevRect != null ? prevRect.width *.5f : 0);
			cellHeight = rect.height * .5f + (prevRect != null ? prevRect.height *.5f : 0);

			Transform t = list[i];
			float depth = t.localPosition.z;
			Vector3 pos = (arrangement == Arrangement.Horizontal) ?
				new Vector3(cellWidth + prevPos.x, y, depth) :
					new Vector3(x, -cellHeight + prevPos.y, depth);
			
			if (animateSmoothly && Application.isPlaying)
			{
				SpringPosition.Begin(t.gameObject, pos, 15f).updateScrollView = true;
			}
			else t.localPosition = pos;

			prevPos = pos;

			maxX = Mathf.Max(maxX, x);
			maxY = Mathf.Max(maxY, y);
			
			if (++x >= maxPerLine && maxPerLine > 0)
			{
				x = 0;
				++y;
			}
		}
		
		// Apply the origin offset
		if (pivot != UIWidget.Pivot.TopLeft)
		{
			Vector2 po = NGUIMath.GetPivotOffset(pivot);
			
			float fx, fy;
			
			if (arrangement == Arrangement.Horizontal)
			{
				fx = Mathf.Lerp(0f, maxX * cellWidth, po.x);
				fy = Mathf.Lerp(-maxY * cellHeight, 0f, po.y);
			}
			else
			{
				fx = Mathf.Lerp(0f, maxY * cellWidth, po.x);
				fy = Mathf.Lerp(-maxX * cellHeight, 0f, po.y);
			}
			
			for (int i = 0; i < myTrans.childCount; ++i)
			{
				Transform t = myTrans.GetChild(i);
				SpringPosition sp = t.GetComponent<SpringPosition>();
				
				if (sp != null)
				{
					sp.target.x -= fx;
					sp.target.y -= fy;
				}
				else
				{
					Vector3 pos = t.localPosition;
					pos.x -= fx;
					pos.y -= fy;
					t.localPosition = pos;
				}
			}
		}
	}
}
