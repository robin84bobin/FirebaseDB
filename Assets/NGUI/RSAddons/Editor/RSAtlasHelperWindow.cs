using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class RSAtlasHelperWindow : EditorWindow
{
	private static Dictionary<RSAtlasHelper.Device, List<string>> deviceAtlasNames;

	private Vector2 scrollPosition;

	//private bool waiting;
	//private string progressInfo;
	//private float progressValue;
	
	[MenuItem("NGUI/RS Atlas Helper")]
	public static void OpenWindow()
	{
		EditorWindow.GetWindow<RSAtlasHelperWindow>(false, "Atlas Helper", true);
		SetupDeviceAtlasNames();
	}

	void OnGUI()
	{
		//if (waiting) {
		//	EditorUtility.DisplayProgressBar("Atlas in processing...", progressInfo, progressValue);	
		//	return;
		//}

		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Refresh Inspector")) {
				SetupDeviceAtlasNames();
				Repaint();
			}
			if (GUILayout.Button("Refresh Atlases")) {
				Repaint();
			}
			if (GUILayout.Button("Set to HD")) {
				Repaint();
			}
			if (GUILayout.Button("Set to NULL")) {
				Repaint();
			}
			if (GUILayout.Button("Set to SD")) {
				Repaint();
			}
		}
		GUILayout.EndHorizontal();

		DrawAllAtlases();
	}
	
	private static void SetupDeviceAtlasNames()
	{
		deviceAtlasNames = new Dictionary<RSAtlasHelper.Device, List<string>>();

		RSAtlasHelper.Device[] devices = { RSAtlasHelper.Device.Standalone, RSAtlasHelper.Device.Tablet, RSAtlasHelper.Device.Phone };
		RSAtlasHelper.AtlasType[] baseTypeForDevice = { RSAtlasHelper.AtlasType.HD, RSAtlasHelper.AtlasType.HD, RSAtlasHelper.AtlasType.SD };

		for (int i = 0; i < devices.Length; ++i) {
			var device = devices[i];
			var atlasType = baseTypeForDevice[i];
			var dirInfo = new DirectoryInfo(string.Format("Assets/GameContent/Textures/Atlases/{0}/{1}/",
				RSAtlasHelper.DeviceToDirectoryName(device),
				RSAtlasHelper.AtlasTypeToDirectoryName(atlasType)));
			if (dirInfo == null || !dirInfo.Exists) {
				continue;
			}
			var atlasDirs = dirInfo.GetDirectories();
			deviceAtlasNames[device] = new List<string>();
			foreach (var atlasDir in atlasDirs) {
				deviceAtlasNames[device].Add(atlasDir.Name);
			}
		}
	}

	private void DrawErrors()
	{
		//EditorGUILayout.HelpBox("Tablet atlas SD/HD not equals", MessageType.Error);
		//EditorGUILayout.HelpBox("Tablet atlas empty", MessageType.Error);
	}
	
	private void DrawAllAtlases()
	{
		scrollPosition = EditorGUILayout.BeginScrollView(scrollPosition);
		{
			DrawErrors();

			if (deviceAtlasNames != null) {
				GUILayout.BeginVertical();
				{
					DrawAtlases(RSAtlasHelper.Device.Standalone);
					DrawAtlases(RSAtlasHelper.Device.Tablet);
					DrawAtlases(RSAtlasHelper.Device.Phone);
				}
				GUILayout.EndVertical();
			}
		}

		EditorGUILayout.EndScrollView();
	}

	private void DrawAtlases(RSAtlasHelper.Device device)
	{
		GUILayout.BeginVertical();
		{
			GUILayout.Label(string.Format("Atlases ({0}):", device), EditorStyles.boldLabel);
			if (deviceAtlasNames.ContainsKey(device)) {
				var atlasNames = deviceAtlasNames[device];
				foreach (var atlasName in atlasNames) {
					DrawAtlas(atlasName, device);
				}
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawAtlas(string atlasName, RSAtlasHelper.Device device)
	{
		GUILayout.BeginVertical();
		{
			DrawAtlasHeader(atlasName, device);
		}
		GUILayout.EndVertical();
	}

	private void DrawAtlasHeader(string atlasName, RSAtlasHelper.Device device)
	{
		GUILayout.BeginHorizontal();
		{
			GUILayout.Space(16.0f);
			GUILayout.Label(atlasName);
			if (GUILayout.Button("Refresh", GUILayout.Width(80f))) {
				RSAtlasHelper.CreateAtlases(atlasName, device);
			}
			if (GUILayout.Button("Set to HD", GUILayout.Width(80f))) {
				RSAtlasHelper.RefreshAtlasReference(atlasName, device, RSAtlasHelper.AtlasType.HD);
			}
			if (GUILayout.Button("Set to NULL", GUILayout.Width(80f))) {
				RSAtlasHelper.RefreshAtlasReference(atlasName, device, RSAtlasHelper.AtlasType.Ref);
			}
			if (GUILayout.Button("Set to SD", GUILayout.Width(80f))) {
				RSAtlasHelper.RefreshAtlasReference(atlasName, device, RSAtlasHelper.AtlasType.SD);
			}
		}
		GUILayout.EndHorizontal();
	}
}
