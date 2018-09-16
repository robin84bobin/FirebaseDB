using System.Collections;

public class BaseScrollItemsContainer : BaseScrollItem 
{
	public BaseScrollItem[] items;

	public override void UpdateView (object dataObject)
	{
		IList dataList = dataObject as IList;
		
		for (int i = 0; i < items.Length; i++) {
			if (i < dataList.Count){
				items[i].UpdateView(dataList[i]);
			}
			else{
				items[i].SetEmpty(true);
			}
			
		}
	}
	
	public override void SetSelected (object dataObject)
	{
		for (int i = 0; i < items.Length; i++) {
			items[i].SetSelected(dataObject);
		}
	}
	
	public override void SetEmpty (bool isEmpty)
	{
		for (int i = 0; i < items.Length; i++) {
			items[i].SetEmpty(isEmpty);
		}
	}

}
