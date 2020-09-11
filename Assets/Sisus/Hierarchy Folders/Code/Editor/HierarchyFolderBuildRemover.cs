using JetBrains.Annotations;
using UnityEditor.Callbacks;
using UnityEngine;
using UnityEditor;

namespace Sisus.HierarchyFolders
{
	public static class HierarchyFolderBuildRemover
	{
		private static bool warnedAboutRemoveFromBuildDisabled;

		[PostProcessScene(1), UsedImplicitly]
		private static void OnPostProcessScene()
		{
			if(!Application.isPlaying)
			{
				var preferences = HierarchyFolderPreferences.Get();
				if(!preferences.removeFromBuild)
				{
					if(!preferences.warnWhenNotRemovedFromBuild || warnedAboutRemoveFromBuildDisabled)
					{
						return;
					}

					warnedAboutRemoveFromBuildDisabled = true;
					if(EditorUtility.DisplayDialog("Warning: Hierarchy Folder Stripping Disabled", "This is a reminder that you have disabled stripping of hierarchy folders from builds. This will result in suboptimal performance and is not recommended when making a release build.", "Continue Anyway", "Enable Stripping"))
					{
						return;
					}
				}

				HierarchyFolderUtility.ApplyStrippingTypeToAllLoadedScenes(StrippingType.FlattenHierarchyAndRemoveGameObject);
			}
		}
	}
}