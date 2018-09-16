using UnityEngine;
using System.Collections.Generic;

public class ScrollButtons : MonoBehaviour 
{
	public UIScrollView scrollView;

	public UIGrid grid;

	public UIButton backwardButton;
	public UIButton forwardButton;

	public List<EventDelegate> onScroll = new List<EventDelegate>();

//	private float _spacing;

	void Start()
	{
		Init ();
	}

	public void Init()
	{
		if (backwardButton != null){
			backwardButton.onClick.Add(new EventDelegate(() => {
				Backward();
			}));
		}
		
		if (forwardButton!= null){
			forwardButton.onClick.Add(new EventDelegate(() => {
				Forward();
			}));
		}

//		_spacing = scrollView.movement == UIScrollView.Movement.Horizontal ? grid.cellWidth : grid.cellHeight;
	}

//	Vector3 offsetPosition;
//	float offset;

	protected void Forward()
	{
		/*
		if (!scrollView.shouldMove) return;

		offsetPosition = Vector3.zero;
		
		if (scrollView.movement == UIScrollView.Movement.Horizontal) 
		{
			offset = scrollView.panel.baseClipRegion.x;
			offsetPosition = new Vector3(-_spacing - offset, 0, 0);
		} 
		else if (scrollView.movement == UIScrollView.Movement.Vertical) 
		{
			offset = scrollView.panel.baseClipRegion.y;
			offsetPosition = new Vector3(0,_spacing - offset, 0);
		}
		*/
		scrollView.Scroll(-1);
		//scrollView.Scroll(offsetPosition);
		EventDelegate.Execute(onScroll);
	}
	
	protected void Backward()
	{
		/*
		if (!scrollView.shouldMove) return;

		offsetPosition = Vector3.zero;
		
		if (scrollView.movement == UIScrollView.Movement.Horizontal) 
		{
			offset = scrollView.panel.baseClipRegion.x;
			offsetPosition = new Vector3(_spacing - offset, 0, 0);
		} 
		else if (scrollView.movement == UIScrollView.Movement.Vertical) 
		{
			offset = scrollView.panel.baseClipRegion.y;
			offsetPosition = new Vector3(0,-_spacing - offset, 0);
		}
		*/
		scrollView.Scroll(1);
		//scrollView.MoveRelative(offsetPosition);
		EventDelegate.Execute(onScroll);
	}
}
