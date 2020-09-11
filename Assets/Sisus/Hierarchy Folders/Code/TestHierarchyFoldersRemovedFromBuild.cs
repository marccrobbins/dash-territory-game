#define LOG_ZERO_COUNT
#define LOG_ROOT_OBJECTS

using UnityEngine;
using UnityEngine.SceneManagement;
using JetBrains.Annotations;

namespace Sisus.HierarchyFolders
{
	/// <summary>
	/// Class that can be used to verify that all hierarchy folders have been removed from scenes in builds.
	/// </summary>
	public class TestHierarchyFoldersRemovedFromBuild : MonoBehaviour
	{
		[UsedImplicitly]
		private void Start()
		{
			var hierarchyFolders = FindObjectsOfType<HierarchyFolder>();
			int count = hierarchyFolders.Length;
			var scene = SceneManager.GetActiveScene();

			#if UNITY_EDITOR
			if(count > 0)
			{
				var playModeBehaviour = HierarchyFolderPreferences.Get().playModeBehaviour;
				if(count > 0 && playModeBehaviour != StrippingType.None && playModeBehaviour != StrippingType.FlattenHierarchy)
				{
					Debug.LogError(string.Format("Number of Hierarchy Folders in scene: {0}\nScene name: \"{1}\"", count, scene.name));
				}
			}
			#if LOG_ZERO_COUNT
			else
			{
				Debug.Log(string.Format("<color=green>Number of Hierarchy Folders in scene: 0\nScene name: \"{0}\"</color>", scene.name));
			}
			#endif
			#else
			if(count > 0)
			{
				Debug.LogError(string.Format("Number of Hierarchy Folders in scene: {0}\nScene name: \"{1}\"", count, scene.name));
			}
			#if LOG_ZERO_COUNT
			else
			{
				Debug.Log(string.Format("Number of Hierarchy Folders in scene: 0\nScene name: \"{0}\"", scene.name));
			}
			#endif
			#endif

			#if LOG_ROOT_OBJECTS
			var sb = new System.Text.StringBuilder();
			foreach(var rootObject in scene.GetRootGameObjects())
			{
				sb.Append('\n');
				sb.Append(rootObject.name);
			}
			Debug.Log(string.Format("{0} root GameObjects: {1}", scene.name, sb.ToString()));
			#endif
		}
	}
}