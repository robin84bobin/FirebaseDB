using System;
/// <summary>
/// RS toggle.
/// UIToggle with label.
/// </summary>



public class RSToggle : UIToggle
{
	public UILabel label;
	public Action<RSToggle,string> OnToggleOn;
	public Action<RSToggle,string> OnToggleOff;

	public bool LockSwitchOff = false;

	private string _dataString = string.Empty;

	public void SetText(string str)
	{
		_dataString = str;
		if (label != null){
			label.text = _dataString;
		}
	}

	public override void Set (bool state)
	{
		if (!LockSwitchOff && !state){
			base.Set (false);
		}

		if (state){
			base.Set(true);
		}

		if (state){
			if (OnToggleOn != null){
				OnToggleOn(this,_dataString);
			}
		}
		else{
			if (OnToggleOff != null){
				OnToggleOff(this,_dataString);
			}
		}
	}
}

