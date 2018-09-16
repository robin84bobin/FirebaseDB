using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class ScrollPicker : MSScrollViewDataSource 
{
	private IList _dataList;

    public void AddDataItem(object item, bool setFocusOnNewItem = true)
    {
        if (_dataList == null) _dataList = new List<object>();

        _dataList.Add(item);
        //Reset(_dataList.Count-1);
        if (_dataList != null && _widgets != null)
        {
            bool canDrag = (_itemCount >= _widgets.Length - 1);
            scrollView.draggablePanel.dragEnabled = canDrag;
        }

        if (setFocusOnNewItem)
        {
            SetPosition(_dataList.Count - 1);
        }
    }

	public void SetData(IList dataList, bool setFocusOnLastItem = true)
	{
		_dataList = dataList;
		Reset(_dataList.Count);

		if (_dataList != null && _widgets != null){
			bool canDrag = (_itemCount >= _widgets.Length - 1);
			scrollView.draggablePanel.dragEnabled = canDrag;
		}

        if (setFocusOnLastItem)
        {
            SetPosition(_dataList.Count - 1);
        }
    }

	public override void UpdateWidget(int widgetIndex, int contentIndex)
	{
		if (lineSize > 1) {
			UpdateLine(widgetIndex,contentIndex);
			return;
		}

		BaseScrollItem item = _widgets[widgetIndex].GetComponentInChildren<BaseScrollItem>();
		if (contentIndex < _itemCount) {
			item.SetEmpty(false);
			item.UpdateView(_dataList[contentIndex]);
		}
		else{
			item.SetEmpty(true);
		}

		base.UpdateWidget(widgetIndex,contentIndex);
	}

	ArrayList _containerDataList = new ArrayList();
	int startIndex; int endIndex; int size;
	protected void UpdateLine(int widgetIndex, int contentIndex)
	{
		BaseScrollItem item = _widgets[widgetIndex].GetComponentInChildren<BaseScrollItem>();

		_containerDataList.Clear();
		startIndex = contentIndex * lineSize;
		endIndex = Mathf.Min ((startIndex + lineSize), _dataList.Count);
		size = endIndex - startIndex;
		if (size > 0) {
			for (int i = startIndex; i < endIndex; ++i) {
				_containerDataList.Add( _dataList[i]);
			}
		}

#if UNITY_EDITOR
/*		Debug.Log (string.Format ("[{3}<-{4}] {0}..{1} : {2}", 
		                          startIndex.ToString(), 
		                          endIndex.ToString(), 
		                          _containerDataList.Count.ToString(),
		                          widgetIndex.ToString(),
		                          contentIndex.ToString()));*/
#endif

		if (contentIndex < _itemCount) {
			item.SetEmpty(false);
			if (lineSize ==1){
				item.UpdateView(_containerDataList[0]);
			}else{
				item.UpdateView(_containerDataList);
			}
		}
		else{
			item.SetEmpty(true);
		}
	}


	public new void SetPosition(int index)
	{
		ResetAtIndex(_dataList.Count,  Mathf.Clamp(index, 0, _dataList.Count - 1));
	}

	public void SetSelected(object dataObject)
	{
		for (int i = 0; i < _widgets.Length; ++i) {
			BaseScrollItem item = _widgets[i].GetComponentInChildren<BaseScrollItem>();
			if (item != null) {
				item.SetSelected(dataObject);
			}
		}
	}

}
