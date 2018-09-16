using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class MSScrollViewDataSource : MonoBehaviour
{
	public IPCycler scrollView;
	public GameObject itemPrefab;

	public UIWidget[] items;
	[Range (1,100)]
	public int lineSize = 1;

	public UIButton backwardButton;
	public UIButton forwardButton;

	protected UIWidget[] _widgets;
	protected int _itemCount;

	protected int _leftIndex;

	//widget indexes for first and last _messageViewData item
	private int _firstIndex = -1;
	private int _lastIndex = -1;

	public virtual void Init()
	{
		scrollView.Init();
		scrollView.onCyclerIndexChange += CyclerIndexChange;
        RSNGUITools.SetActive(itemPrefab, false);
		
		//if (WidgetsNeedRebuild()) {
		CreateWidgets();
		//}
		StartCoroutine(InitScrollPositions());
		InitButtons();

		scrollView.draggablePanel.onDrag.Add( new EventDelegate(UpdateButtons));
	}

	private float _backwardPos;
	private float _forwardPos;
	IEnumerator InitScrollPositions()
	{
		yield return null;
		
		Vector3 leftBottomCorner = scrollView.draggablePanel.panel.localCorners[0];
		Vector3 leftBottomCornerGlobal = transform.TransformPoint(leftBottomCorner);
		float leftPos = UICamera.mainCamera.WorldToScreenPoint(leftBottomCornerGlobal).x;
		float bottomPos = UICamera.mainCamera.WorldToScreenPoint(leftBottomCornerGlobal).y;
		
		Vector3 rigthTopCorner = scrollView.draggablePanel.panel.localCorners[2];
		Vector3 rigthTopCornerGlobal = transform.TransformPoint(rigthTopCorner);
		float rightPos = UICamera.mainCamera.WorldToScreenPoint(rigthTopCornerGlobal).x;
		float topPos   = UICamera.mainCamera.WorldToScreenPoint(rigthTopCornerGlobal).y;
		
		if (scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal){
			_backwardPos = leftPos;
			_forwardPos  = rightPos;
		}
		else if (scrollView.draggablePanel.movement == UIScrollView.Movement.Vertical){
			_backwardPos = topPos;
			_forwardPos  = bottomPos;
		}
		UpdateButtons();
	}


	public virtual void Setup(int itemCount)
	{	
		scrollView.Init();
		scrollView.onCyclerIndexChange += CyclerIndexChange;

		//if (WidgetsNeedRebuild()) {
		CreateWidgets();
		//}

		SetItemsCount(itemCount);
		_leftIndex = 0;

		//if (_itemCount == 0) {
		//	EnableWidgets(false);
		//	return;
		//}

		UpdateWidgets();
		UpdateButtons();
	}

	public void SetPosition(int index)
	{
		ResetAtIndex(_itemCount,  Mathf.Clamp(index, 0, _itemCount - 1));
	}

	protected void InitButtons()
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
	}

	protected void Forward()
	{
		Vector3 offsetPosition = Vector3.zero;
		float offset;
		float itemLenght = scrollView.spacing;

		if (scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal) 
		{
			offset = scrollView.draggablePanel.panel.baseClipRegion.x;
			offsetPosition = new Vector3(-itemLenght - offset, 0, 0);
		} 
		else if (scrollView.draggablePanel.movement == UIScrollView.Movement.Vertical) 
		{
			offset = scrollView.draggablePanel.panel.baseClipRegion.y;
			offsetPosition = new Vector3(0,itemLenght - offset,0);
		}

		scrollView.draggablePanel.MoveRelative(offsetPosition);
		scrollView.draggablePanel.RestrictWithinBounds (false);
		UpdateButtons();
	}

	protected void Backward()
	{
		Vector3 offsetPosition = Vector3.zero;
		float itemLenght = scrollView.spacing;
		float offset;

		if (scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal) 
		{
			offset = scrollView.draggablePanel.panel.baseClipRegion.x;
			offsetPosition = new Vector3(itemLenght - offset, 0, 0);
		} 
		else if (scrollView.draggablePanel.movement == UIScrollView.Movement.Vertical) 
		{
			offset = scrollView.draggablePanel.panel.baseClipRegion.x;
			offsetPosition = new Vector3(0,-itemLenght - offset, 0);
		}

		scrollView.draggablePanel.MoveRelative(offsetPosition);
		scrollView.draggablePanel.RestrictWithinBounds (false);
		UpdateButtons();
	}

	protected void ResetAtIndex(int itemCount, int mustSeeElement)
	{
		SetItemsCount(itemCount);

		_leftIndex = mustSeeElement;
		if (_widgets.Length >= _itemCount) {
			_leftIndex = 0;
		} else if ((_leftIndex + _widgets.Length) > (_itemCount)) {
			_leftIndex = _itemCount - _widgets.Length;
		}

		//Debug.Log("### ResetAtIndex > _leftIndex: " + _leftIndex + " mustSee: " + mustSeeElement);

		scrollView.draggablePanel.ResetPosition();
		scrollView.ResetCycler(itemCount);
		UpdateWidgets();

		float lenght = scrollView.spacing;
		int layout = mustSeeElement - _leftIndex;
		Vector3 newPosition = Vector3.zero;
		if (scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal) {
			float x = scrollView.draggablePanel.panel.baseClipRegion.x;
			float z = scrollView.draggablePanel.panel.baseClipRegion.z;
			float items = z / lenght;

			//Debug.Log("### ResetAtIndex > x: " + x + " z: " + z + " items: " + items + " lenght: " + lenght + " layout: " + layout);

			if (layout >= (int)items) {
				newPosition = new Vector3(-lenght * (Mathf.Clamp((layout - items), 0, float.MaxValue) + 1) - x, 0, 0);
			}
		} else if (scrollView.draggablePanel.movement == UIScrollView.Movement.Vertical) {
			float w = scrollView.draggablePanel.panel.baseClipRegion.w;
			float y = scrollView.draggablePanel.panel.baseClipRegion.y;
			float items = w / lenght;

			if (layout >= (int)items) {
				newPosition = new Vector3(0, lenght * (Mathf.Clamp((layout - items), 0, float.MaxValue) + 1) - y + lenght/2, 0);
			}
		}
		scrollView.draggablePanel.MoveRelative(newPosition);
	}

	protected void Reset(int itemCount, int mustSeeElement = 0)
	{
		ResetAtIndex(itemCount, mustSeeElement);
		
		float totalItemsCount = Mathf.Max(_widgets.Length, _itemCount);
		float contentSize = totalItemsCount * scrollView.spacing;
		scrollView.draggablePanel.SetContentCustomSize(contentSize);
	}

	protected void SetItemsCount(int itemCount)
	{
		float c = (float)itemCount / lineSize;
		_itemCount = Mathf.CeilToInt(c);
	}

	void OnDestroy()
	{
        if (scrollView != null)
		    scrollView.onCyclerIndexChange -= CyclerIndexChange;
	}

//	protected void ResetPickerAtIndex(int index)
//	{
//		_selectedIndex = index;
//
//		int prevNbOfElements = _nbOfVirtualElements;
//
//		UpdateVirtualElementsCount();
//
//		if (prevNbOfElements == 0) {
//			if (_nbOfVirtualElements > 0) {
//				EnableWidgets(true);
//			}
//		} else if (_nbOfVirtualElements == 0) {
//			EnableWidgets(false);
//			return;
//		}
//
//		UpdateCurrentValue();
//
//		cycler.ResetCycler();
//
//		ResetWidgetsContent();
//	}

	//public void ResetPicker()
	//{
	//	ResetPickerAtIndex(_selectedIndex);
	//}

	void CyclerIndexChange(bool increment, int widgetIndex)
	{
		if (_itemCount == 0) {
			return;
		}

		if (increment) {
			++_leftIndex;
			if (_widgets.Length >= _itemCount) {
				_leftIndex = 0;
			} else if ((_leftIndex + _widgets.Length) > (_itemCount)) {
				_leftIndex = _itemCount - _widgets.Length;
			}
		} else {
			--_leftIndex;
			if (_leftIndex < 0) {
				_leftIndex = 0;
			}
		}

		CycleWidgets(increment, widgetIndex);

		scrollView.ClampDecrement = _leftIndex == 0;
		scrollView.ClampIncrement = _leftIndex == (_itemCount - 1 - (_widgets.Length - 1));
	}

	void Update()
	{
		if (scrollView != null && _widgets != null) {
			if (_widgets.Length - 1 >= _itemCount) {
				scrollView.ClampIncrement = true;
			} else {
				scrollView.ClampIncrement = _leftIndex == (_itemCount - _widgets.Length);
			}
			scrollView.ClampDecrement = _leftIndex == 0;
		}
	}

	void CycleWidgets(bool indexIncremented, int widgetIndex)
	{
		int contentIndex;

		if (indexIncremented) {
			contentIndex = _leftIndex + _widgets.Length - 1;
		} else {
			contentIndex = _leftIndex;
		}
	
		UpdateWidget(widgetIndex, contentIndex);
	}

	float _firstItemPos; 
	float _lastItemPos;

	bool backScroll = true;
	bool forwScroll = true;

	protected void UpdateButtons()
	{
		if (backwardButton != null){
			if( _firstIndex >= 0){
				_firstItemPos =  scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal ?
					UICamera.mainCamera.WorldToScreenPoint( _widgets[_firstIndex].transform.position).x :
					UICamera.mainCamera.WorldToScreenPoint( _widgets[_firstIndex].transform.position).y	;

				backwardButton.isEnabled = scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal ?
					_firstItemPos < _backwardPos + scrollView.spacing * 0.1f :
					_firstItemPos > _backwardPos - scrollView.spacing * 0.1f;

				if (!backwardButton.isEnabled && backScroll) {
					backScroll = false;
					scrollView.Scroll(-0.01f);
				}
			}
			else{
				backwardButton.isEnabled = true;
				backScroll = true;
			}
		}

		if (forwardButton != null){
			if(_lastIndex >= 0){
				_lastItemPos = scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal ?
					UICamera.mainCamera.WorldToScreenPoint( _widgets[_lastIndex].transform.position).x :
					UICamera.mainCamera.WorldToScreenPoint( _widgets[_lastIndex].transform.position).y ;
				forwardButton.isEnabled =
					scrollView.draggablePanel.movement == UIScrollView.Movement.Horizontal ?
					_lastItemPos > _forwardPos - scrollView.spacing * 0.1f :
					_lastItemPos < _forwardPos + scrollView.spacing * 0.1f;

				if (!forwardButton.isEnabled && forwScroll) {
					forwScroll = false;
					scrollView.Scroll(0.01f);
				}
			}
			else{
				forwardButton.isEnabled = true;
				forwScroll = true;
			}
		}
	}

	private Dictionary<int,int> _contentMap = new Dictionary<int, int>();
	private Dictionary<int,int> _widgetMap = new Dictionary<int, int>();
	public virtual void UpdateWidget(int widgetIndex, int contentIndex)
	{
		_widgetMap[widgetIndex] = contentIndex;
		_contentMap[contentIndex] = widgetIndex;
		_firstIndex = _widgetMap.ContainsValue(0) ? _contentMap[0] : -1;
		_lastIndex  = _widgetMap.ContainsValue(_itemCount-1) ? _contentMap[_itemCount-1] : -1;

	}

	public void CreateWidgets()
	{
		_widgets = new UIWidget[scrollView.NbOfTransforms];

		for (int i = 0; i < scrollView.NbOfTransforms; ++i) {
			Transform parent = scrollView._cycledTransforms[i];
			
			while (parent.childCount > 0) {
				Transform child = parent.GetChild(0);
				child.parent = null;
				Destroy(child.gameObject);
			}

			GameObject newGO = NGUITools.AddChild(parent.gameObject, itemPrefab);
            RSNGUITools.SetActive(newGO, true);
			_widgets[i] = newGO.GetComponentInChildren<UIWidget>();
			_widgets[i].cachedTransform.localPosition = Vector3.zero;
		}
	}

	public void UpdateWidgets()
	{
		for (int i = 0; i < _widgets.Length; ++i) {
			UpdateWidget(i, _leftIndex + i);
		}
	}

}
