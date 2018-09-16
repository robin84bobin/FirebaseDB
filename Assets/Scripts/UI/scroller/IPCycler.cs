using UnityEngine;
using System.Collections;

[ExecuteInEditMode]
[RequireComponent(typeof(UIPanel))]
[RequireComponent(typeof(UIScrollView))]
public class IPCycler : MonoBehaviour
{
	public enum Direction { Vertical, Horizontal }

	public Direction direction;

	public float spacing = 30f;	// The horizontal or vertical space between cycled transforms ( positive values only )
	public float recenterSpeedThreshold = 0.2f; //The minimum movement of the cycler panel before centering on the centermost transform ( high values will cut momentum )
	public float recenterSpringStrength = 8f;
	public bool restrictDragToPicker;
	public bool inverted = false;
	//Read only properties
	/// <summary>
	/// Gets or sets a value indicating whether this instance is moving. 
	/// Use onCyclerStopped delegate to be notified when cycler has finished recentering, 
	/// as IsMoving can be false for a frame between momentum and UICenterOnChildManual.CenterOnChild().
	/// </summary>
	public bool IsMoving { get; private set; }
	public int CenterWidgetIndex { get; private set; }
	public int LeftWidgetIndex { get; private set; }
	public int RecenterTargetWidgetIndex { get; private set; }
	public int NbOfTransforms { get { return transform.childCount; } }
	public int TransformChildCount { get { return transform.childCount; } }

	public bool ClampIncrement { get; set; }
	public bool ClampDecrement { get; set; }

	public delegate void CyclerIndexChangeHandler(bool increase, int indexToUpdate);
	public CyclerIndexChangeHandler onCyclerIndexChange;

	public delegate void CyclerStoppedHandler();
	public CyclerStoppedHandler onCyclerStopped;

	public delegate void SelectionStartedHandler();
	public SelectionStartedHandler onCyclerSelectionStarted;

	public delegate void CenterOnChildStartedHandler();
	public CenterOnChildStartedHandler onCenterOnChildStarted;

	public Transform[] _cycledTransforms;

	UIScrollView _draggablePanel;
	public UIScrollView draggablePanel { get { return _draggablePanel;} }

	[HideInInspector]
	public IPDragScrollView dragPanelContents;
	[HideInInspector]
	public IPUserInteraction userInteraction;
	UIPanel _panel;
	UICenterOnChild _uiCenterOnChild;

	BoxCollider _pickerCollider;

	int _initFrame = -1;
	int _lastResetFrame = -1;

	float _decrementThreshold;
	float _incrementThreshold;
	float _transformJumpDelta;
	float _panelSignificantPosVector;
	float _panelPrevPos;
	float _deltaPos;

	Vector3 _resetPosition;
	bool _isInitialized;
	//bool _hasMovedSincePress;
	[SerializeField]
	bool _isHorizontal;
	[SerializeField]
	private float _trueSpacing; // Spacing adjusted according to cycler direction

	#region Player mode methods

	public void Init()
	{
		//Prevent multiple Init in the same frame - for compound pickers
		if (_initFrame == Time.frameCount) {
			return;
		}
		_initFrame = Time.frameCount;

		//Cache NGUI components
		_draggablePanel = gameObject.GetComponent(typeof(UIScrollView)) as UIScrollView;
		_panel = gameObject.GetComponent(typeof(UIPanel)) as UIPanel;

		//Make sure the parent is active before getting collider
		NGUITools.SetActive(transform.parent.gameObject, true);

		//Look for a collider, and if one is found, add user interaction scripts
		if (_pickerCollider == null) {
			_pickerCollider = transform.parent.GetComponentInChildren(typeof(BoxCollider)) as BoxCollider;
		}

		if (_pickerCollider != null) {
			dragPanelContents = _pickerCollider.gameObject.AddComponent(typeof(IPDragScrollView)) as IPDragScrollView;
			dragPanelContents.scrollView = _draggablePanel;

			userInteraction = _pickerCollider.gameObject.AddComponent(typeof(IPUserInteraction)) as IPUserInteraction;
			userInteraction.cycler = this;
			userInteraction.restrictWithinPicker = restrictDragToPicker;
		} else {
			Debug.Log("Could not get collider");
		}

		//Add and subscribe to the recenter component
		_uiCenterOnChild = gameObject.AddComponent(typeof(UICenterOnChild)) as UICenterOnChild;
		_uiCenterOnChild.enabled = false;
		_uiCenterOnChild.springStrength = recenterSpringStrength;
		_uiCenterOnChild.onFinished = PickerStopped;

		//Check if the ScrollWheelEvents singleton is in the scene
		ScrollWheelEvents.CheckInstance();
		
		_draggablePanel.movement = _isHorizontal ? UIScrollView.Movement.Horizontal : UIScrollView.Movement.Vertical;

		//Iniitialize a bunch of variables
		_resetPosition = _panel.cachedTransform.localPosition;

		//was clipRange
		_panelPrevPos = SignificantPosVector(_panel.cachedTransform);

		_transformJumpDelta = -_trueSpacing * NbOfTransforms; //how much should the cycled transforms move when re-cycling (incrementing) ? 

		CenterWidgetIndex = NbOfTransforms / 2;
		LeftWidgetIndex = 0;
		_decrementThreshold = _panelPrevPos;
		_incrementThreshold = _panelPrevPos + _trueSpacing;
		_deltaPos = 0f;

		RecenterTargetWidgetIndex = 0;

		_isInitialized = true;
	}

//	public void Reset()
//	{
//		draggablePanel.ResetPosition();// cachedTransform.localPosition = _resetPosition;
//		_panelPrevPos = SignificantPosVector(_panel.cachedTransform);
//		LeftWidgetIndex = 0;
//		_decrementThreshold = _panelPrevPos;
//		_incrementThreshold = _panelPrevPos + _trueSpacing;
//		_deltaPos = 0f;
//		
//		RecenterTargetWidgetIndex = 0;
//
//		UpdateTrueSpacing();
//	}

/// <summary>
/// Called by IPPickerBase to reset the cycler - not meant for public use.
/// </summary>
	public void ResetCycler (int itemCount)
	{
		if ( _lastResetFrame == Time.frameCount ) //prevent resetting twice in the same frame ( for compound pickers )
			return;
		_lastResetFrame = Time.frameCount;

		//Reset the panel's position and clipping
		_panel.cachedTransform.localPosition = _resetPosition; 
		_panel.clipOffset = new Vector2( 0f, 0f );
		_panelPrevPos = SignificantPosVector ( _panel.cachedTransform );
		
		//Space the transforms
		PlaceTransforms (itemCount);

		//Reset various members
		LeftWidgetIndex = 0;
		_decrementThreshold = 0;
		_incrementThreshold = _trueSpacing;
		_deltaPos = 0f;
	}

	
	void Update()
	{
		if (!_isInitialized) {
			return;
		}

		_panelSignificantPosVector = SignificantPosVector(_panel.cachedTransform);

		//Debug.LogWarning("_panelSignificantPosVector: " + _panelSignificantPosVector);
		//Debug.LogWarning("_decrementThreshold: " + _decrementThreshold);		

		//ClampIncrement = true;
		//ClampDecrement = true;

		if (Mathf.Approximately(_panelSignificantPosVector, _panelPrevPos) == false) {
			IsMoving = true;
			//_hasMovedSincePress = true;

			_deltaPos = _panelSignificantPosVector - _panelPrevPos;

			if (_isHorizontal) {
				_deltaPos = -_deltaPos;
			}

			if (_deltaPos > 0) {
				while (TryIncrement()) {

				}
			} else {
				while (TryDecrement()) {

				}
			}
		} else if (IsMoving) {
			IsMoving = false;
			_deltaPos = 0f;
		}

		_panelPrevPos = _panelSignificantPosVector;
	}

	//Called by UICenterOnChildAuto.onFinished ( forwarding delegate, notably to IPForwardPickerEvents )
	void PickerStopped()
	{
		if (onCyclerStopped != null) {
			onCyclerStopped();
		}
	}

	/// <summary>
	///	The following methods can be used to manually handle the cycler.
	/// See PickerScrollButton for an example.
	/// </summary>
	public void Scroll(float delta)
	{
		_draggablePanel.Scroll(delta);
	}

	public int GetDeltaIndexFromScreenPos(Vector2 pos)
	{
		Vector3 onClickTouchPosInWorld = UICamera.currentCamera.ScreenToWorldPoint(new Vector3(pos.x, pos.y, 0f));
		Vector3 onClickTouchPosInPicker = transform.parent.InverseTransformPoint(onClickTouchPosInWorld);

		float distanceFromCenter;

		if (direction == Direction.Horizontal) {
			distanceFromCenter = onClickTouchPosInPicker.x;
		} else {
			distanceFromCenter = onClickTouchPosInPicker.y;
		}

		distanceFromCenter = distanceFromCenter >= 0 ? distanceFromCenter + spacing / 2 : distanceFromCenter - spacing / 2;

		int deltaIndex = (int)distanceFromCenter / (int)spacing;

		if (direction == Direction.Vertical) //Thank you loverainstm ;-)
		{
			deltaIndex = -deltaIndex;
		}

		deltaIndex = Mathf.Clamp(deltaIndex, -NbOfTransforms / 2, NbOfTransforms / 2);

		return deltaIndex;
	}

	public int GetIndexFromScreenPos(Vector2 pos)
	{
		int deltaIndex = GetDeltaIndexFromScreenPos(pos);
		int index = (CenterWidgetIndex + deltaIndex) % NbOfTransforms;
		if (index < 0)
			index += NbOfTransforms;
		return index;
	}

	/// <summary>
	/// Called by IPUserInteraction to forward press events. Not for public use!
	/// </summary>
	public void OnPress(bool press)
	{
		if (press) {
			//_hasMovedSincePress = false;
			StopAllCoroutines(); // Stop Recenter coroutine on press
			//uiCenterOnChildManual legacy
			//_uiCenterOnChild.Abort ();
			if (onCyclerSelectionStarted != null)
				onCyclerSelectionStarted();
		} else {
			//if (_hasMovedSincePress) {
			//	StartCoroutine(RecenterOnThreshold()); // Launch Recenter coroutine on release
			//}
		}
	}

	/// <summary>
	/// Called by IPUserInteraction to forward scrollwheel events. Not for public use!
	/// </summary>
	public void ScrollWheelStartOrStop(bool start)
	{
		if (start) {
			StopAllCoroutines();

			if (onCyclerSelectionStarted != null)
				onCyclerSelectionStarted();
		} else {
			StartCoroutine(RecenterOnThreshold());
		}
	}

	/// <summary>
	/// Wait until the cycler has slowed down enough before recentering.
	/// </summary>
	IEnumerator RecenterOnThreshold()
	{
		while (Mathf.Abs(_deltaPos) > recenterSpeedThreshold) // let the momentum carry on 
		{
			yield return null;
		}
		//
		CenterOnTransformIndex(LeftWidgetIndex); //Recenter
	}

	void CenterOnTransformIndex(int index)
	{
		RecenterTargetWidgetIndex = index;
		_uiCenterOnChild.CenterOn(_cycledTransforms[index]);
		if (onCenterOnChildStarted != null)
			onCenterOnChildStarted();
	}

	float SignificantPosVector(Transform trans)
	{
		return _isHorizontal ? trans.localPosition.x : trans.localPosition.y;
	}

	void SetWidgetSignificantPos(Transform trans, float pos)
	{
		if (!_isHorizontal) {
			trans.localPosition = new Vector3(0f, pos, trans.localPosition.z);
		} else {
			trans.localPosition = new Vector3(pos, 0f, trans.localPosition.z);
		}
	}

	bool TryIncrement()
	{
		if (ClampIncrement) {
			return false;
		}

		if (_isHorizontal) {
			if (!(_panelSignificantPosVector < _incrementThreshold)) {
				return false;
			}
		} else {
			if (!(_panelSignificantPosVector > _incrementThreshold)) {
				return false;
			}
		}

		_incrementThreshold += _trueSpacing;
		_decrementThreshold += _trueSpacing;

		int firstWidgetIndex;
		Transform firstWidget = FirstWidget(out firstWidgetIndex);
		SetWidgetSignificantPos(firstWidget, SignificantPosVector(firstWidget) + _transformJumpDelta);
		CenterWidgetIndex = (CenterWidgetIndex + 1) % NbOfTransforms;
		LeftWidgetIndex = (LeftWidgetIndex + 1) % NbOfTransforms;

		if (onCyclerIndexChange != null) {
			onCyclerIndexChange(true, firstWidgetIndex);
		}

		return true;
	}

	bool TryDecrement()
	{
		if (ClampDecrement) {
			return false;
		}

		if (_isHorizontal) {
			if (!(_panelSignificantPosVector > _decrementThreshold)) {
				return false;
			}
		} else {
			if (!(_panelSignificantPosVector < _decrementThreshold)) {
				return false;
			}
		}

		_incrementThreshold -= _trueSpacing;
		_decrementThreshold -= _trueSpacing;

		int lastWidgetIndex;
		Transform lastWidget = LastWidget(out lastWidgetIndex);
		SetWidgetSignificantPos(lastWidget, SignificantPosVector(lastWidget) - _transformJumpDelta);
		LeftWidgetIndex = (LeftWidgetIndex - 1);

		if (LeftWidgetIndex < 0) {
			LeftWidgetIndex += NbOfTransforms;
		}

		if (onCyclerIndexChange != null) {
			onCyclerIndexChange(false, lastWidgetIndex);
		}

		return true;;
	}

	Transform FirstWidget(out int firstWidgetIndex)
	{
		firstWidgetIndex = LeftWidgetIndex;

		if (firstWidgetIndex < 0) {
			firstWidgetIndex += NbOfTransforms;
		}

		return _cycledTransforms[firstWidgetIndex];
	}

	Transform LastWidget(out int lastWidgetIndex)
	{
		lastWidgetIndex = (LeftWidgetIndex + NbOfTransforms - 1) % NbOfTransforms;

		return _cycledTransforms[lastWidgetIndex];
	}

	#endregion

	#region Editor mode methods

	public void EditorInit()
	{
		DestroyChildrenOfChildren();
		if (_cycledTransforms == null || _cycledTransforms.Length != TransformChildCount) {
			_cycledTransforms = new Transform[TransformChildCount];
		}

		for (int i = 0; i < TransformChildCount; ++i) {
			_cycledTransforms[i] = transform.GetChild(i);
		}

		UpdateTrueSpacing();
	}

	public void RebuildTransforms(int newTransformCount)
	{
		int initTransformCount = TransformChildCount;
		int i;

		if (initTransformCount != 0) {
			Transform[] tmp = new Transform[initTransformCount];

			for (i = 0; i < initTransformCount; ++i) {
				tmp[i] = transform.GetChild(i);
			}

			for (i = 0; i < initTransformCount; ++i) {
				DestroyImmediate(tmp[i].gameObject);
			}
		}

		_cycledTransforms = new Transform[newTransformCount];

		for (i = 0; i < newTransformCount; ++i) {
			GameObject go = new GameObject();
			go.transform.parent = transform;
			go.transform.localScale = Vector3.one;
			go.transform.localPosition = Vector3.zero;
			go.transform.localRotation = Quaternion.identity;
			go.name = i.ToString();
			go.layer = 8; // NGUI layer
			_cycledTransforms[i] = go.transform;
		}

		UpdateTrueSpacing();
	}
	
	public void UpdateTrueSpacing()
	{
		_isHorizontal = direction == Direction.Horizontal;

		_trueSpacing = Mathf.Abs(spacing);

		if (_isHorizontal) {
			// if horizontal, increasing panel's x position moves the cycler backwards,
			// a negative spacing allows for less duplicate code
			_trueSpacing = -_trueSpacing;
		}

		PlaceTransforms();
	}
	
	public void PlaceTransforms(int itemCount = 0)
	{
		int childCount = TransformChildCount;
		float offset = 0f;
		if (inverted && itemCount != 0 && GetComponent<UIPanel>().baseClipRegion.w/_trueSpacing >= itemCount) {
			offset = (GetComponent<UIPanel>().baseClipRegion.w/_trueSpacing - itemCount) * _trueSpacing;
		}
		float transformSignificantPos = _isHorizontal
			? -GetComponent<UIPanel>().baseClipRegion.z / 2 + GetComponent<UIPanel>().clipSoftness.x / 2 - _trueSpacing / 2
			: GetComponent<UIPanel>().baseClipRegion.w / 2 - GetComponent<UIPanel>().clipSoftness.y / 2 - _trueSpacing / 2;

		for (int i = 0; i < childCount; ++i) {
			SetWidgetSignificantPos (_cycledTransforms [i], transformSignificantPos - offset);
			transformSignificantPos -= _trueSpacing;
		}
	}

	private void DestroyChildrenOfChildren()
	{
		foreach (Transform t in transform) {
			Transform[] children = new Transform[t.childCount];
			for (int i = 0; i < t.childCount; ++i) {
				children[i] = t.GetChild(i);
			}

			for (int j = 0; j < children.Length; ++j) {
				DestroyImmediate(children[j].gameObject);
			}
		}
	}

	#endregion
}
