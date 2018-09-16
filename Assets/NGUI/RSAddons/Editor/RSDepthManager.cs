using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class RSDepthManager : ScriptableWizard
{
	public GameObject NGUIRootObject = null;
	public int numberOfLevelsToSearch = 10;

	private Dictionary<string, Dictionary<string, Dictionary<int, List<UIWidget>>>>
		widgetHierarchy = new Dictionary<string, Dictionary<string, Dictionary<int, List<UIWidget>>>>();

	private Vector2 scrollPosition = Vector2.zero;

	[MenuItem("NGUI/RS Depth Manager")]
	static void CreateWizard()
	{
		ScriptableWizard.DisplayWizard("RS Depth Manager", typeof(RSDepthManager));
	}

	void OnGUI()
	{
		EditorGUIUtility.labelWidth = 80f;

		this.NGUIRootObject = (GameObject)
			EditorGUILayout.ObjectField("Root Object", this.NGUIRootObject, typeof(GameObject), true, GUILayout.ExpandWidth(true));

		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Find Widgets Under Root")) {
				this.findObjs();
				this.Repaint();
			}
			if (GUILayout.Button("Root Object Clear")) {
				this.NGUIRootObject = null;
				this.widgetHierarchy.Clear();
				this.Repaint();
			}
		}
		GUILayout.EndHorizontal();

		if (this.widgetHierarchy.Count == 0) {
			GUILayout.Label("Use 'Find Widgets Under Root' to get the widgets and their depths and Z Pos.");
		} else {
			if (this.NGUIRootObject == null) {
				this.NGUIRootObject = null;
				this.widgetHierarchy.Clear();
				this.Repaint();
			} else {
				this.drawWidgetsScrollView();
				GUILayout.Label("If RenderQueue value is equal, Atlas Z position value of the widget closer look at the smaller screen.");
			}

		}

	}

	protected void drawWidgetsScrollView()
	{
		this.scrollPosition =
			EditorGUILayout.BeginScrollView(this.scrollPosition);

		List<string> sortedPanelPaths = new List<string>(
			this.widgetHierarchy.Keys);
		sortedPanelPaths.Sort();

		foreach (string panelPath in sortedPanelPaths) {
			Vector3 tPanelPos;
			GUILayout.Label(string.Format("Panel: {0}", panelPath), EditorStyles.boldLabel);

			GUILayout.BeginHorizontal();
			{
				EditorGUILayout.ObjectField(
					this.objectAtTransPathRoot(panelPath),
					typeof(GameObject), false);
				GameObject tPanelObj = this.objectAtTransPathRoot(panelPath);
				tPanelPos = tPanelObj.transform.localPosition;
				EditorGUILayout.LabelField("Z Pos:", GUILayout.Width(40), GUILayout.MaxWidth(40));
				tPanelPos.z = EditorGUILayout.FloatField(tPanelObj.transform.localPosition.z, GUILayout.MaxWidth(60));
				tPanelObj.transform.localPosition = tPanelPos;
			}
			GUILayout.EndHorizontal();

			Dictionary<string, Dictionary<int, List<UIWidget>>>
				hierarchyUnderPanel = this.widgetHierarchy[panelPath];

			List<string> sortedAtlasNames = new List<string>(
				hierarchyUnderPanel.Keys);
			sortedAtlasNames.Sort();

			int tAtlasCnt = 0;
			foreach (string atlasName in sortedAtlasNames) {
				tAtlasCnt++;
				Dictionary<int, List<UIWidget>> hierarchyUnderAtlas =
						hierarchyUnderPanel[atlasName];

				List<int> sortedDepths = new List<int>(
					hierarchyUnderAtlas.Keys);
				sortedDepths.Sort();

				//float tZPos = 0f;
				float tMinZPos = float.MaxValue;
				float tMaxZpos = -float.MaxValue;
				int tTotWidgetCnt = 0;
				int i = 0;
				//UIWidget tSetWidget = null;

				GUILayout.BeginVertical();
				{
					GUILayout.Label(string.Format("{0}. Atlas: {1}", tAtlasCnt, atlasName), EditorStyles.boldLabel);

					foreach (int depth in sortedDepths) {
						List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];

						foreach (UIWidget widget in widgetsAtDepth) {
							Vector3 tPos = widget.transform.localPosition;
							tMinZPos = Mathf.Min(tMinZPos, tPos.z);
							tMaxZpos = Mathf.Max(tMaxZpos, tPos.z);
							tTotWidgetCnt++;
						}
					}

					foreach (int depth in sortedDepths) {
						List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];

						foreach (UIWidget widget in widgetsAtDepth) {
							i++;
							Vector3 tPos = widget.transform.localPosition;

							if (i == tTotWidgetCnt) {
								tPos.z = tMaxZpos;
								widget.transform.localPosition = tPos;
								//tSetWidget = widget;
							} else {
								tPos.z = tMinZPos;
								widget.transform.localPosition = tPos;
							}
						}

					}

					//float tSumPos = tMinZPos + tPanelPos.z;
					//int tDiv;
					//if (tTotWidgetCnt > 1) {
					//	tDiv = 2;
					//	tZPos = tSumPos + (tMaxZpos - tMinZPos) / tDiv;
					//} else {
					//	tDiv = tTotWidgetCnt;
					//	tZPos = tSumPos;
					//}

					/*
					Vector3 tAtlasPos = tSetWidget.transform.localPosition;

					GUILayout.BeginHorizontal();
					{
						int t_QueNum;
						GUILayout.Space(30f);
						EditorGUILayout.LabelField("1) RenderQueue:", GUILayout.Width(105), GUILayout.MaxWidth(105));

						tSetWidget.material.renderQueue = EditorGUILayout.IntField(tSetWidget.material.renderQueue, GUILayout.Width(50), GUILayout.MinWidth(50));
						//NGUISetRenderQueue t_Ques = (NGUISetRenderQueue)tSetWidget.GetComponent(typeof(NGUISetRenderQueue));

						//if (t_Ques != null) 
						//{								
						//	t_Ques.m_RenderQueueNum = tSetWidget.material.renderQueue;
						//}

						if (GUILayout.Button("Set", GUILayout.Width(50), GUILayout.MaxWidth(50))) {
							t_QueNum = tSetWidget.material.renderQueue;
							//if (t_Ques == null) 
							//{								
							//	t_Ques = (NGUISetRenderQueue)tSetWidget.gameObject.AddComponent("NGUISetRenderQueue");
							//}

							//t_Ques.m_RenderQueueNum = t_QueNum;
							tSetWidget.material.renderQueue = t_QueNum;
							//Selection.activeObject = t_Ques.gameObject;
							EditorApplication.SaveScene();
							GUIUtility.keyboardControl = 0;
						}
						EditorGUILayout.LabelField(", 2) Atlas Z Pos:", GUILayout.Width(100), GUILayout.MaxWidth(100));
						tAtlasPos.z = (EditorGUILayout.FloatField(tZPos, GUILayout.Width(50), GUILayout.MinWidth(50)) - tSumPos) * tDiv + tMinZPos;
						tSetWidget.transform.localPosition = tAtlasPos;

						if (GUILayout.Button("Atlas Z Pos Clear", GUILayout.Width(120))) {
							foreach (int depth in sortedDepths) {
								List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];

								foreach (UIWidget widget in widgetsAtDepth) {
									Vector3 tPos = widget.transform.localPosition;
									tPos.z = 0;
									widget.transform.localPosition = tPos;
								}
							}
							GUIUtility.keyboardControl = 0;
						}

					}
					GUILayout.EndHorizontal();
					*/

				}
				GUILayout.EndVertical();

				foreach (int depth in sortedDepths) {
					List<UIWidget> widgetsAtDepth = hierarchyUnderAtlas[depth];

					foreach (UIWidget widget in widgetsAtDepth) {
						this.drawDepthsWidget(depth, widget, tPanelPos.z);
					}
				}
			}

		}

		EditorGUILayout.EndScrollView();
	}

	protected void drawDepthsWidget(int depth, UIWidget widget, float tPanelZPos)
	{
		GUILayout.BeginHorizontal();
		{
			GUILayout.Space(64.0f);
			EditorGUILayout.ObjectField(widget, typeof(UIWidget), true, GUILayout.ExpandWidth(true));

			if (GUILayout.Button("Select", GUILayout.Width(50))) {
				Selection.activeObject = widget.gameObject;
			}

			GUILayout.Space(10.0f);
			widget.depth = EditorGUILayout.IntField("Depth:", widget.depth);

		}
		GUILayout.EndHorizontal();
	}

	protected void findObjs()
	{
		if (this.NGUIRootObject == null) {
			return;
		}

		this.widgetHierarchy.Clear();

		this.findObjsUnderTransformTree(0, this.numberOfLevelsToSearch - 1, this.NGUIRootObject.transform, "", ref this.widgetHierarchy);
	}

	protected void findObjsUnderTransformTree(int recursionLevel,
		int maxRecursionLevel, Transform currentTransform,
		string parentPanelPath,
		ref Dictionary<string, Dictionary<string, Dictionary<int, List<UIWidget>>>>
			widgetHierarchy)
	{
		if (recursionLevel > maxRecursionLevel) {
			return;
		}

		UIPanel currentPanel = currentTransform.gameObject.GetComponent<UIPanel>();

		if (currentPanel != null) {
			string underPath = AnimationUtility.CalculateTransformPath(currentTransform, this.NGUIRootObject.transform);
			if (underPath.Length > 0) {
				parentPanelPath = string.Format("{0}/{1}", this.NGUIRootObject.name, underPath);
			} else {
				parentPanelPath = this.NGUIRootObject.name;
			}
		}

		foreach (Transform child in currentTransform) {
			UIWidget widget = child.gameObject.GetComponent<UIWidget>();
			if (widget != null) {
				string atlasName =
					RSDepthManager.atlasNameFromObj(widget);

				int depth = widget.depth;

				Dictionary<string, Dictionary<int, List<UIWidget>>>
					hierarchyUnderPanel = null;

				if (!widgetHierarchy.TryGetValue(parentPanelPath, out hierarchyUnderPanel)) {
					hierarchyUnderPanel =
						new Dictionary<string, Dictionary<int, List<UIWidget>>>();
					widgetHierarchy[parentPanelPath] = hierarchyUnderPanel;
				}

				Dictionary<int, List<UIWidget>> hierarchyUnderAtlas = null;

				if (!hierarchyUnderPanel.TryGetValue(atlasName, out hierarchyUnderAtlas)) {
					hierarchyUnderAtlas = new Dictionary<int, List<UIWidget>>();
					hierarchyUnderPanel[atlasName] = hierarchyUnderAtlas;
				}

				List<UIWidget> widgetsAtDepth = null;

				if (!hierarchyUnderAtlas.TryGetValue(depth, out widgetsAtDepth)) {
					widgetsAtDepth = new List<UIWidget>();
					hierarchyUnderAtlas[depth] = widgetsAtDepth;
				}
				widgetsAtDepth.Add(widget);
			}

			this.findObjsUnderTransformTree(recursionLevel + 1,
				maxRecursionLevel, child, parentPanelPath, ref widgetHierarchy);
		}
	}

	public GameObject objectAtTransPathRoot(string path)
	{
		GameObject objectAtPath = null;
		string subPath = path.Substring(this.NGUIRootObject.name.Length);
		if (subPath.Length < 1) {
			objectAtPath = this.NGUIRootObject;
		} else {
			subPath = subPath.Substring(1);
			Transform transformAtPath = this.NGUIRootObject.transform.Find(subPath);
			if (transformAtPath != null) {
				objectAtPath = transformAtPath.gameObject;
			}
		}
		return objectAtPath;

	}

	static public string atlasNameFromObj(UIWidget widget)
	{
		string atlasName = "Not Atlas";
		UIAtlas atlas = null;

		if (widget is UISprite) {
			atlas = ((UISprite)widget).atlas;
		} else if (widget is UILabel) {
			UIFont font = ((UILabel)widget).bitmapFont;
			if (font != null && font.atlas != null) {
				atlas = font.atlas;
			}
		}

		if (atlas != null) {
			atlasName = atlas.gameObject.name;
		}
		return atlasName;
	}
}
