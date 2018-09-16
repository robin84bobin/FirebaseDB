using UnityEngine;


public class BaseScrollItem : MonoBehaviour 
{
	public virtual void UpdateView (object dataObject){}
	public virtual void SetSelected (object dataObject){}
	public virtual void SetEmpty(bool isEmpty){}
	public virtual void SetItemIndex (int contentIndex){}
}
