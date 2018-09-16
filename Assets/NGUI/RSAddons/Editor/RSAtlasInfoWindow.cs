using UnityEngine;
using UnityEditor;
using System.Collections.Generic;
using System.IO;

public class RSAtlasInfoWindow : EditorWindow
{
	private class AtlasInfo
	{
		public string atlasName;

		public UIAtlas hdAtlas;
		public UIAtlas refAtlas;
		public UIAtlas sdAtlas;

		public string RefAtlasReferenceTo()
		{
			if (refAtlas.replacement == null) {
				return "NULL";
			}

			if (refAtlas.replacement == hdAtlas) {
				return "HD";
			}

			if (refAtlas.replacement == sdAtlas) {
				return "SD";
			}

			return null;
		}
	}

	private static Dictionary<RSAtlasHelper.Device, List<AtlasInfo>> _deviceToAtlasInfo;

	private Vector2 _scrollPosition;

	[MenuItem("NGUI/RS Atlas Info")]
	public static void OpenWindow()
	{
		EditorWindow.GetWindow<RSAtlasInfoWindow>(false, "Atlas Info", true);
		SetupDeviceToAtlasInfo();
	}

	void OnGUI()
	{
		GUILayout.BeginHorizontal();
		{
			if (GUILayout.Button("Refresh Inspector")) {
				SetupDeviceToAtlasInfo();
				Repaint();
			}
		}
		GUILayout.EndHorizontal();

		DrawAllAtlasesInfo();
	}

	private static void SetupDeviceToAtlasInfo()
	{
		_deviceToAtlasInfo = new Dictionary<RSAtlasHelper.Device, List<AtlasInfo>>();
	
		RSAtlasHelper.Device[] devices = { RSAtlasHelper.Device.Tablet, RSAtlasHelper.Device.Phone };
	
		for (int i = 0; i < devices.Length; ++i) {
			var device = devices[i];
			var dirInfo = new DirectoryInfo(string.Format("Assets/Resources/Atlases/{0}/", device));
			if (dirInfo == null || !dirInfo.Exists) {
				continue;
			}
			var fis = dirInfo.GetFiles("*.prefab");
			var atlasNames = new List<string>();
			foreach (var fi in fis) {
				atlasNames.Add(fi.Name.Split('@')[0]);
			}
			
			_deviceToAtlasInfo[device] = new List<AtlasInfo>();
			foreach (var atlasName in atlasNames) {
				var atlasInfo = new AtlasInfo();
				atlasInfo.atlasName = atlasName;
				if (device == RSAtlasHelper.Device.Tablet) {
					atlasInfo.hdAtlas = RSAtlasHelper.GetAtlas(atlasName, device, RSAtlasHelper.AtlasType.HD);
				}
				atlasInfo.refAtlas = RSAtlasHelper.GetAtlas(atlasName, device, RSAtlasHelper.AtlasType.Ref);
				atlasInfo.sdAtlas = RSAtlasHelper.GetAtlas(atlasName, device, RSAtlasHelper.AtlasType.SD);
				_deviceToAtlasInfo[device].Add(atlasInfo);
			}
		}
	}

	private void DrawAllAtlasesInfo()
	{
		_scrollPosition = EditorGUILayout.BeginScrollView(_scrollPosition);
		{
			if (_deviceToAtlasInfo != null) {
				GUILayout.BeginVertical();
				{
					DrawAtlasesInfo(RSAtlasHelper.Device.Tablet);
					DrawAtlasesInfo(RSAtlasHelper.Device.Phone);
				}
				GUILayout.EndVertical();
			}
		}

		EditorGUILayout.EndScrollView();
	}

	private void DrawAtlasesInfo(RSAtlasHelper.Device device)
	{
		GUILayout.BeginVertical();
		{
			List<string> atlasNames = new List<string>();
			if (_deviceToAtlasInfo.ContainsKey(device)) {
				foreach (AtlasInfo atlasInfo in _deviceToAtlasInfo[device]) {
					atlasNames.Add(atlasInfo.atlasName);
				}
			}

			GUILayout.BeginHorizontal();
			{
				GUILayout.Label(string.Format("Atlases ({0}):", device), EditorStyles.boldLabel);
				if (GUILayout.Button("Set to HD", GUILayout.Width(80f))) {
					RSAtlasHelper.RefreshAtlasesReferences(atlasNames, device, RSAtlasHelper.AtlasType.HD);
					AssetDatabase.SaveAssets();
					Repaint();
				}
				if (GUILayout.Button("Set to NULL", GUILayout.Width(80f))) {
					RSAtlasHelper.RefreshAtlasesReferences(atlasNames, device, RSAtlasHelper.AtlasType.Ref);
					AssetDatabase.SaveAssets();
					Repaint();
				}
				if (GUILayout.Button("Set to SD", GUILayout.Width(80f))) {
					RSAtlasHelper.RefreshAtlasesReferences(atlasNames, device, RSAtlasHelper.AtlasType.SD);
					AssetDatabase.SaveAssets();
					Repaint();
				}
			}
			GUILayout.EndHorizontal();

			if (_deviceToAtlasInfo.ContainsKey(device)) {
				foreach (AtlasInfo atlasInfo in _deviceToAtlasInfo[device]) {
					DrawAtlasInfo(atlasInfo);
				}
			}
		}
		GUILayout.EndVertical();
	}

	private void DrawAtlasInfo(AtlasInfo atlasInfo)
	{
		GUILayout.BeginVertical();
		{
			GUILayout.BeginHorizontal();
			{
				GUILayout.Space(16.0f);
				GUILayout.Label(atlasInfo.atlasName + " (" + atlasInfo.RefAtlasReferenceTo() + ")");
			}
			GUILayout.EndHorizontal();
		}
		GUILayout.EndVertical();
	}
}
