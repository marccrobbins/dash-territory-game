using UnityEngine;
using UnityEditor;
 
namespace Sisus.HierarchyFolders
{
	public class DisallowHierarchyFolderPrefabs : AssetPostprocessor
	{
		void OnPreprocessAsset()
		{
			if(!string.Equals(assetImporter.GetType().Name, "PrefabImporter"))
			{
				return;
			}

			// Can't use AssetDatabase.LoadAssetAtPath without waiting a bit first
			EditorApplication.delayCall += ConvertToGameObjectIfHierarchyFolder;
		}

		private void ConvertToGameObjectIfHierarchyFolder()
		{
			var gameObject = AssetDatabase.LoadAssetAtPath<GameObject>(assetImporter.assetPath);
			if(gameObject == null)
			{
				return;
			}

			if(!gameObject.IsHierarchyFolder())
			{
				return;
			}

			Debug.LogWarning("Hierarchy Folders can only exist in the scene and as such can't be prefabs. Converting into a normal GameObject.", gameObject);
			HierarchyFolderUtility.UnmakeHierarchyFolder(gameObject, gameObject.GetComponent<HierarchyFolder>());
		}
	}
}