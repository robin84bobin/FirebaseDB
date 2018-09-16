using UnityEngine;
using System;

public class MSPopupListContainer : MonoBehaviour 
{
	public UIPopupList popupList;
	public UILabel headerLabel;
	public UILabel selectedItemLabel;
	public UISprite iconOpened;
	public UISprite iconClosed;

	public Action OnItemSelect;

	private bool _isOpened = false;

	void Start()
	{
		popupList.onClose.Add (new EventDelegate (OnClose));
		popupList.onOpen.Add (new EventDelegate (OnOpen));
		popupList.onChange.Add (new EventDelegate (OnChange));
		SetOpenCloseIcon ();
	}
	
	void OnOpen()
	{
		_isOpened = true;
		SetOpenCloseIcon ();
	}
	
	void OnClose()
	{
		_isOpened = false;
		SetOpenCloseIcon ();
	}

	void SetOpenCloseIcon ()
	{
		bool isOpen = UIPopupList.current == popupList && UIPopupList.isOpen;
		RSNGUITools.SetActive (iconOpened.gameObject, isOpen);
		RSNGUITools.SetActive (iconClosed.gameObject, !isOpen);
	}

	void OnChange()
	{
		if (selectedItemLabel != null) {
			selectedItemLabel.text = popupList.value;
		}

		if (_isOpened && OnItemSelect != null) {
			OnItemSelect();
		}
	}

}
