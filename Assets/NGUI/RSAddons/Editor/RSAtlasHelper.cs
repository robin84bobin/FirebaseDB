#define RSATLASHELPER_DEBUG

using UnityEngine;
using UnityEditor;
using System.IO;
using System.Collections.Generic;

public class RSAtlasHelper : ScriptableObject
{
	public enum Device
	{
		Standalone,
		Tablet,
		Phone
	}

	public enum AtlasType
	{
		HD,
		Ref,
		SD
	}

	public static void MakePixelPerfect(GameObject go)
	{
		UIWidget[] widgets = go.GetComponentsInChildren<UIWidget>(true);
		foreach (var widget in widgets) {
			widget.MakePixelPerfect();
		}
	}

	public static void RefreshAtlasesReferences(List<string> atlasNames, Device device, AtlasType refTargetType)
	{
		foreach (string atlasName in atlasNames) {
			RefreshAtlasReference(atlasName, device, refTargetType);
		}
	}

	public static void RefreshAtlasReference(string atlasName, Device device, AtlasType refTargetType)
	{
		UIAtlas targetAtlas = null;
		if (refTargetType != AtlasType.Ref) {
			string targetAtlasDirectory = "Assets" + GetAtlasResourceFolder(device, refTargetType);
			string targetAtlasFullName = targetAtlasDirectory + GetAtlasName(atlasName, device, refTargetType) + ".prefab";
			GameObject targetAtlasPrefab = AssetDatabase.LoadAssetAtPath(targetAtlasFullName, typeof(GameObject)) as GameObject;
			targetAtlas = targetAtlasPrefab.GetComponent<UIAtlas>();
		}

		string refAtlasFullName = "Assets" + GetAtlasResourceFolder(device, AtlasType.Ref)
			+ GetAtlasName(atlasName, device, AtlasType.Ref) + ".prefab";

		GameObject refAtlasPrefab = AssetDatabase.LoadAssetAtPath(refAtlasFullName, typeof(GameObject)) as GameObject;
		UIAtlas refAtlas = refAtlasPrefab.GetComponent<UIAtlas>();
		refAtlas.replacement = targetAtlas;
	}

	public static UIAtlas GetAtlas(string atlasName, Device device, AtlasType atlasType)
	{
		string atlasFullName = "Assets" + GetAtlasResourceFolder(device, atlasType)
			+ GetAtlasName(atlasName, device, atlasType) + ".prefab";

		GameObject atlasPrefab = AssetDatabase.LoadAssetAtPath(atlasFullName, typeof(GameObject)) as GameObject;
		UIAtlas atlas = atlasPrefab.GetComponent<UIAtlas>();
		return atlas;
	}

	public static void GetAtlasSpriteData(ref UIAtlas atlas, ref List<UISpriteData> sprites)
	{
		foreach (UISpriteData sprite in atlas.spriteList) {
			UISpriteData newSpriteData = new UISpriteData();
			newSpriteData.CopyFrom(sprite);
			sprites.Add(newSpriteData);
		}
	}

	public static void UpdateAtlasSpriteData(ref UIAtlas atlas, ref List<UISpriteData> sprites)
	{
		foreach (UISpriteData sprite in sprites) {
			foreach (UISpriteData newSprite in atlas.spriteList) {
				if (newSprite.name == sprite.name) {
					newSprite.borderLeft = sprite.borderLeft;
					newSprite.borderRight = sprite.borderRight;
					newSprite.borderTop = sprite.borderTop;
					newSprite.borderBottom = sprite.borderBottom;

					newSprite.paddingLeft = sprite.paddingLeft;
					newSprite.paddingRight = sprite.paddingRight;
					newSprite.paddingTop = sprite.paddingTop;
					newSprite.paddingBottom = sprite.paddingBottom;
				}
			}
		}
	}

	public static void DebugAtlasSpriteData(ref UIAtlas atlas)
	{
		Debug.Log("sprite count in atlas (" + atlas.name + ") = " + atlas.spriteList.Count);
		foreach (UISpriteData sprite in atlas.spriteList) {
			Debug.Log("name = " + sprite.name);
		}
	}

	public static string DeviceToDirectoryName(Device device)
	{
		switch (device) {
			case Device.Standalone:
				return "standalone";
			case Device.Tablet:
				return "tablet";
			case Device.Phone:
				return "phone";
			default:
				return string.Empty;
		}
	}

	public static string AtlasTypeToDirectoryName(AtlasType atlasType)
	{
		switch (atlasType) {
			case AtlasType.HD:
				return "hd";
			case AtlasType.SD:
				return "sd";
			default:
				return string.Empty;
		}
	}

	public static string GetAtlasTexturesDirectory(string atlasName, Device device, AtlasType atlasType)
	{
		if (atlasType == AtlasType.Ref) {
			Debug.LogError("Reference atlas haven't textures");
			return string.Empty;
		}

		return string.Format("/GameContent/Textures/Atlases/{0}/{1}/{2}/",
			DeviceToDirectoryName(device), AtlasTypeToDirectoryName(atlasType), atlasName);
	}

	public static string GetAtlasResourceFolder(Device device, AtlasType atlasType)
	{
		if (atlasType == AtlasType.Ref) {
			return string.Format("/GameContent/Resources/Atlases/{0}/", DeviceToDirectoryName(device));
		}

		return string.Format("/GameContent/Resources/Atlases/{0}/{1}/",
			DeviceToDirectoryName(device), AtlasTypeToDirectoryName(atlasType));
	}

	public static string GetAtlasName(string atlasName, Device device, AtlasType atlasType)
	{
		string newAtlasName;

		if (atlasType == AtlasType.Ref) {
			newAtlasName = string.Format("{0}@{1}", atlasName, DeviceToDirectoryName(device));
		} else {
			newAtlasName = string.Format("{0}@{1}-{2}", atlasName, DeviceToDirectoryName(device),
				AtlasTypeToDirectoryName(atlasType));
		}

		return newAtlasName;
	}

	private static UIAtlas CreateAtlasInternal(string atlasName, Device device, AtlasType atlasType, out List<UISpriteData> oldSpriteData)
	{
		oldSpriteData = new List<UISpriteData>();

#if RSATLASHELPER_DEBUG
		Debug.Log(string.Format("Create atlas; atlasName = {0}, device = {1}, atlasType = {2}", atlasName, device, atlasType));
#endif
		string resourceDir = GetAtlasResourceFolder(device, atlasType);

		string newAtlasName = GetAtlasName(atlasName, device, atlasType);
		string newAtlas = string.Format("Assets{0}{1}.prefab", resourceDir, newAtlasName);
		string newAtlasMaterial = string.Format("Assets{0}{1}.mat", resourceDir, newAtlasName);

		DirectoryInfo dirInfo = new DirectoryInfo("Assets" + resourceDir);
		if (!dirInfo.Exists) {
#if RSATLASHELPER_DEBUG
			Debug.Log("create directory: " + resourceDir);
#endif
			dirInfo.Create();
		}


#if RSATLASHELPER_DEBUG
		Debug.Log("creating atlas assets: " + newAtlas);
#endif
		Material mat = null;
		if (atlasType != AtlasType.Ref) {
			mat = AssetDatabase.LoadAssetAtPath(newAtlasMaterial, typeof(Material)) as Material;
			if (mat == null) {
				Shader shader = Shader.Find("Unlit/Transparent Colored");
				if (shader != null) {
					mat = new Material(shader);

					AssetDatabase.CreateAsset(mat, newAtlasMaterial);
					AssetDatabase.SaveAssets();

					mat = AssetDatabase.LoadAssetAtPath(newAtlasMaterial, typeof(Material)) as Material;
				}
			}

			if (mat == null) {
				Debug.LogWarning("Error; Could not create material for atlas = " + atlasName);
				return null;
			}
		}

		bool saveSpriteData = true;
		Object atlasPrefab = AssetDatabase.LoadAssetAtPath(newAtlas, typeof(GameObject));
		if (atlasPrefab == null) {
			atlasPrefab = PrefabUtility.CreateEmptyPrefab(newAtlas);
			saveSpriteData = false;
		}

		if (atlasPrefab == null) {
			Debug.LogWarning("Error; Could not create atlas prefab for atlas = " + atlasName);
			return null;
		}

		GameObject go = new GameObject(atlasName);
		
		UIAtlas atlas = go.AddComponent<UIAtlas>();
		if (atlasType != AtlasType.Ref) {
			atlas.spriteMaterial = mat;
		}
	
		if (saveSpriteData) {
			GameObject goOldAtlas = (GameObject)Instantiate(atlasPrefab);
			UIAtlas oldAtlas = goOldAtlas.GetComponent<UIAtlas>();
			GetAtlasSpriteData(ref oldAtlas, ref oldSpriteData);
			oldAtlas = null;
			DestroyImmediate(goOldAtlas);
		}

		PrefabUtility.ReplacePrefab(go, atlasPrefab, ReplacePrefabOptions.ReplaceNameBased);

		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		GameObject go2 = AssetDatabase.LoadAssetAtPath(newAtlas, typeof(GameObject)) as GameObject;

		DestroyImmediate(go);
		EditorUtility.UnloadUnusedAssetsImmediate();

		UIAtlas ret = go2.GetComponent<UIAtlas>();

#if RSATLASHELPER_DEBUG
		Debug.Log("Finished creating atlas = " + ret);
#endif
		return ret;
	}

	public static UIAtlas CreateAtlasRef(string atlasName, Device device)
	{
		List<UISpriteData> oldSpriteData;
	
		UIAtlas atlasRef = CreateAtlasInternal(atlasName, device, AtlasType.Ref, out oldSpriteData);
		if (atlasRef == null) {
			Debug.LogWarning("Could not create atlas reference for " + atlasName);
			return null;
		}
	
		// TODO: add special script for fixAtlas
		//retinaProParent parent = atlasRef.gameObject.GetComponent<retinaProParent>();
		//if (parent == null) {
		//	atlasRef.gameObject.AddComponent<retinaProParent>();
		//}

		EditorUtility.SetDirty(atlasRef.gameObject);

		return atlasRef;
	}

	public static UIAtlas CreateAtlas(string atlasName, Device device, AtlasType atlasType)
	{
		List<UISpriteData> oldSpriteData;

#if RSATLASHELPER_DEBUG
		Debug.Log(string.Format("AddNewAtlas; atlasName = {0}, device = {1}, atlasType = {2}", atlasName, device, atlasType));
#endif

		string texturesDir = GetAtlasTexturesDirectory(atlasName, device, atlasType);
	
		DirectoryInfo dirInfo = new DirectoryInfo("Assets/" + texturesDir);
		if (dirInfo == null || !dirInfo.Exists) {
			Debug.LogWarning("Directory does not exist; " + atlasName + ", for device: " + device + " " + texturesDir);
			return null;
		}
		List<FileInfo> fis;
		GetTextureAssets(dirInfo, out fis);

		if (fis == null && fis.Count == 0) {
			Debug.LogWarning("Directory empty; " + atlasName + ", for device: " + device);
			return null;
		}
		
		UIAtlas newAtlas = CreateAtlasInternal(atlasName, device, atlasType, out oldSpriteData);
		if (newAtlas == null) {
			Debug.LogWarning("Could not create atlas for " + atlasName + ", device = " + device);
			return null;
		}

		NGUISettings.atlas = newAtlas;
	    NGUISettings.atlasPadding = 2;
	    NGUISettings.atlasTrimming = false;
		NGUISettings.forceSquareAtlas = true;
		NGUISettings.allow4096 = (atlasType == AtlasType.HD);
		NGUISettings.fontTexture = null;
		NGUISettings.unityPacking = false;

		if (device == Device.Tablet || device == Device.Standalone) {
			newAtlas.pixelSize = (atlasType == AtlasType.HD) ? 1f : 2f;
		} else {
			newAtlas.pixelSize = 1f;
		}

		EditorUtility.SetDirty(newAtlas.gameObject);

		List<Texture> textures = new List<Texture>();

		foreach (FileInfo fi in fis) {
			string textureName = "Assets" + texturesDir + fi.Name;

			Texture2D tex = AssetDatabase.LoadAssetAtPath(textureName, typeof(Texture2D)) as Texture2D;

			TextureImporter texImporter = AssetImporter.GetAtPath(textureName) as TextureImporter;
			texImporter.textureType = TextureImporterType.Default;
			texImporter.npotScale = TextureImporterNPOTScale.None;
			texImporter.generateCubemap = TextureImporterGenerateCubemap.None;
			texImporter.normalmap = false;
			texImporter.linearTexture = true;
			texImporter.alphaIsTransparency = true;
			texImporter.convertToNormalmap = false;
			texImporter.grayscaleToAlpha = false;
			texImporter.lightmap = false;
			texImporter.npotScale = TextureImporterNPOTScale.None;
			texImporter.filterMode = FilterMode.Point;
			texImporter.wrapMode = TextureWrapMode.Clamp;
			texImporter.maxTextureSize = 4096;
			texImporter.mipmapEnabled = false;
			texImporter.textureFormat = TextureImporterFormat.AutomaticTruecolor;
			AssetDatabase.ImportAsset(textureName, ImportAssetOptions.ForceUpdate);

			tex.filterMode = FilterMode.Bilinear;
			tex.wrapMode = TextureWrapMode.Clamp;

			textures.Add(tex);

#if RSATLASHELPER_DEBUG
			Debug.Log("- added tex: " + textureName);
#endif
		}

		List<UIAtlasMaker.SpriteEntry> sprites = UIAtlasMaker.CreateSprites(textures);
		UIAtlasMaker.ExtractSprites(newAtlas, sprites);
		UIAtlasMaker.UpdateAtlas(newAtlas, sprites);
		AssetDatabase.SaveAssets();

		{
			string resourceDir = GetAtlasResourceFolder(device, atlasType);
			string atlasName2 = GetAtlasName(atlasName, device, atlasType);
			string newTex = "Assets" + resourceDir + atlasName2 + ".png";

			TextureImporter texImporter = AssetImporter.GetAtPath(newTex) as TextureImporter;
			int maxTextureSize = atlasType == AtlasType.HD ? 4096 : 2048;
			texImporter.maxTextureSize = maxTextureSize;
			texImporter.textureFormat = TextureImporterFormat.AutomaticCompressed;
			if (device == Device.Phone) {
				texImporter.mipmapEnabled = true;
				texImporter.borderMipmap = true;
				texImporter.mipmapFilter = TextureImporterMipFilter.BoxFilter;
			} else {
				texImporter.mipmapEnabled = false;
			}
			texImporter.alphaIsTransparency = true;
			texImporter.filterMode = FilterMode.Bilinear;
			texImporter.wrapMode = TextureWrapMode.Clamp;
			texImporter.anisoLevel = 1;
			texImporter.SetPlatformTextureSettings("iPhone", maxTextureSize, TextureImporterFormat.PVRTC_RGBA4);
			texImporter.SetPlatformTextureSettings("Android", maxTextureSize, TextureImporterFormat.PVRTC_RGBA4);
			AssetDatabase.ImportAsset(newTex, ImportAssetOptions.ForceUpdate);
		}
			
		UpdateAtlasSpriteData(ref newAtlas, ref oldSpriteData);

#if RSATLASHELPER_DEBUG
		DebugAtlasSpriteData(ref newAtlas);
#endif
				
		newAtlas.MarkAsChanged();
		AssetDatabase.SaveAssets();
		AssetDatabase.Refresh();

		return newAtlas;
	}

	public static void CreateAtlases(string atlasName, Device device)
	{
		CreateAtlasRef(atlasName, device);

		switch(device) {
			case Device.Phone:
				CreateAtlas(atlasName, device, AtlasType.SD);
				break;
			case Device.Tablet:
			case Device.Standalone:
				CreateAtlas(atlasName, device, AtlasType.HD);
				CreateAtlas(atlasName, device, AtlasType.SD);
				break;
		}
	}

	public static void GetTextureAssets(DirectoryInfo dinfo, out List<FileInfo> files)
	{
		files = new List<FileInfo>();
		
		FileInfo [] fis;
		
		fis = dinfo.GetFiles("*.png");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = dinfo.GetFiles("*.psd");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}

		fis = dinfo.GetFiles("*.tif");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = dinfo.GetFiles("*.jpg");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = dinfo.GetFiles("*.bmp");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}

		fis = dinfo.GetFiles("*.tga");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}

		fis = dinfo.GetFiles("*.gif");
		foreach(FileInfo fi in fis)
		{
			files.Add(fi);
		}
		
		fis = null;
	}
}
