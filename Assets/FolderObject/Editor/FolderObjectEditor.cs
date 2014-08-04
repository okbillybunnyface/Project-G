/*
	Hierarchy Folder Object
	Unity Editor Extension

	Version: 1.0.4
	Date: 01 Apr 14

	By Bright Static Media
	http://www.brightstatic.co.uk
*/

using System.Collections;
using System.Collections.Generic;
using System.IO;
using UnityEngine;
using UnityEditor;

[CustomEditor (typeof(FolderObject))]
[CanEditMultipleObjects]
public class FolderObjectEditor : Editor
{
	private FolderObject folderObject;

	private bool lockFolder;
	private bool hideChildrenInHierarchy;

	Tool lastTool = Tool.None;

	void OnEnable()
	{
		lastTool = Tools.current;
		Tools.current = Tool.None;

		if (folderObject == null)
			folderObject = target as FolderObject;

		if (folderObject.hideChildrenInHierarchy)
		{
			foreach (Transform child in folderObject.transform)
				child.hideFlags |= HideFlags.HideInHierarchy;
		}
		else
		{
			foreach (Transform child in folderObject.transform)
				child.hideFlags &= ~HideFlags.HideInHierarchy;
		}

		EditorApplication.RepaintHierarchyWindow();

	}

	void OnDisable()
	{
		Tools.current = lastTool;
	}


	public void FolderComponentRemoved()
	{
		if (folderObject == null)
			folderObject = target as FolderObject;

		folderObject.gameObject.GetComponent<Transform>().hideFlags = HideFlags.None;

		lockFolder = false;
		hideChildrenInHierarchy = false;

		if (folderObject.lockFolder != lockFolder)
		{
			folderObject.gameObject.hideFlags &= ~HideFlags.NotEditable;

			foreach (Component component in folderObject.GetComponents(typeof(Component)))
				component.hideFlags &= ~HideFlags.NotEditable;

			Tools.current = Tool.Move;

			EditorUtility.SetDirty(folderObject);

			folderObject.lockFolder = lockFolder;
		}

		if (folderObject.hideChildrenInHierarchy != hideChildrenInHierarchy)
		{
			foreach (Transform child in folderObject.transform)
				child.hideFlags &= ~HideFlags.HideInHierarchy;

			EditorApplication.RepaintHierarchyWindow();

			folderObject.hideChildrenInHierarchy = hideChildrenInHierarchy;
		}

	}


	public override void OnInspectorGUI()
	{
		if (folderObject == null)
			folderObject = target as FolderObject;

		if (folderObject.gameObject.GetComponent<Transform>().hideFlags != HideFlags.HideInInspector)
			folderObject.gameObject.GetComponent<Transform>().hideFlags = HideFlags.HideInInspector;

		lockFolder = folderObject.lockFolder;
		hideChildrenInHierarchy = folderObject.hideChildrenInHierarchy;

		GUILayout.Space(15);

		lockFolder = EditorGUILayout.Toggle(" Lock Folder?", lockFolder);
		if (FolderObjectOptions.displayFolderToggleDescriptions)
			EditorGUILayout.HelpBox("This option prevents the Folder object from being deleted or changed (does not apply to children).", MessageType.None);

		GUILayout.Space(15);

		hideChildrenInHierarchy = EditorGUILayout.Toggle(" Hide Children?", hideChildrenInHierarchy);
		if (FolderObjectOptions.displayFolderToggleDescriptions)
			EditorGUILayout.HelpBox("This option hides all of the Folder's children in the Hierarchy window. This has no effect on child GameObject's active or visible states in the Scene.", MessageType.None);

		GUILayout.Space(20);

		if (GUI.changed)
		{

			if (folderObject.lockFolder != lockFolder)
			{
				if (lockFolder)
				{
					folderObject.gameObject.hideFlags |= HideFlags.NotEditable;

					foreach (Component component in folderObject.GetComponents(typeof(Component)))
					{
						if (component == folderObject)
						{
							component.hideFlags &= ~HideFlags.NotEditable;
						}
						else
						{
							component.hideFlags |= HideFlags.NotEditable;
						}
					}
					
					EditorUtility.SetDirty(folderObject);
				}
				else
				{
					folderObject.gameObject.hideFlags &= ~HideFlags.NotEditable;

					foreach (Component component in folderObject.GetComponents(typeof(Component)))
					{
						component.hideFlags &= ~HideFlags.NotEditable;
					}

					Tools.current = Tool.Move;

					EditorUtility.SetDirty(folderObject);
				}

				folderObject.lockFolder = lockFolder;
			}

			if (folderObject.hideChildrenInHierarchy != hideChildrenInHierarchy)
			{
				if (hideChildrenInHierarchy)
				{
					foreach (Transform child in folderObject.transform)
						child.hideFlags |= HideFlags.HideInHierarchy;
				}
				else
				{
					foreach (Transform child in folderObject.transform)
						child.hideFlags &= ~HideFlags.HideInHierarchy;
				}

				EditorApplication.RepaintHierarchyWindow();
				folderObject.hideChildrenInHierarchy = hideChildrenInHierarchy;
			}

			EditorUtility.SetDirty(target);
		}

	}


	[MenuItem("GameObject/Create Other/Folder", false, -20)]
	static void CreateFolderObject()
	{
		FolderObjectNameWindow nameFolderWindow = (FolderObjectNameWindow)ScriptableObject.CreateInstance(typeof(FolderObjectNameWindow));

		nameFolderWindow.ShowUtility();
		nameFolderWindow.title = "Enter Folder Name";

		nameFolderWindow.maxSize = new Vector2(300.0f, 80.0f);
		nameFolderWindow.minSize = nameFolderWindow.maxSize;

	}

}


[InitializeOnLoad]
public class FolderHierarchyIcon
{
	private const float folderIconSize = 16.0f;
	private static Texture2D folderIcon;
	private static Texture2D lockedIcon;
	private static Texture2D hiddenIcon;

	static FolderHierarchyIcon()
	{
		if (FolderObjectOptions.displayFolderIconInHierarchy)
		{
			EditorApplication.hierarchyWindowItemOnGUI += DrawFolderIconInHierarchy;
			EditorApplication.RepaintHierarchyWindow();
		}
	}

	private static void DrawFolderIconInHierarchy(int instanceID, Rect folderObjectRect)
	{
		GameObject folderGameObject = EditorUtility.InstanceIDToObject(instanceID) as GameObject;
		if (folderGameObject && folderGameObject.GetComponent<FolderObject>())
		{
			FolderObject folderObject = folderGameObject.GetComponent<FolderObject>();

			if (!folderObject.hideChildrenInHierarchy)
			{
				if (!folderIcon)
				{
					string folderIconPath = "Assets" + Directory.GetFiles(Application.dataPath, "FolderObjectIcon.png", SearchOption.AllDirectories)[0].Substring(Application.dataPath.Length).Replace('\\', '/');
					folderIcon = AssetDatabase.LoadAssetAtPath(folderIconPath, typeof(Texture2D)) as Texture2D;
				}
				GUI.Box(new Rect(folderObjectRect.xMax - folderIconSize - 2.0f, folderObjectRect.center.y - (folderIconSize/2.0f), folderIconSize, folderIconSize), folderIcon, GUIStyle.none);
			}
			else
			{
				if (!hiddenIcon)
				{
					string hiddenIconPath = "Assets" + Directory.GetFiles(Application.dataPath, "FolderHiddenIcon.png", SearchOption.AllDirectories)[0].Substring(Application.dataPath.Length).Replace('\\', '/');
					hiddenIcon = AssetDatabase.LoadAssetAtPath(hiddenIconPath, typeof(Texture2D)) as Texture2D;
				}
				GUI.Box(new Rect(folderObjectRect.xMax - folderIconSize - 2.0f, folderObjectRect.center.y - (folderIconSize/2.0f), folderIconSize, folderIconSize), hiddenIcon, GUIStyle.none);
			}

			if (folderObject.lockFolder)
			{
				if (!lockedIcon)
				{
					string lockedIconPath = "Assets" + Directory.GetFiles(Application.dataPath, "FolderLockedIcon.png", SearchOption.AllDirectories)[0].Substring(Application.dataPath.Length).Replace('\\', '/');
					lockedIcon = AssetDatabase.LoadAssetAtPath(lockedIconPath, typeof(Texture2D)) as Texture2D;
				}
				GUI.Box(new Rect(folderObjectRect.xMax - folderIconSize - 20.0f, folderObjectRect.center.y - (folderIconSize/2.0f), folderIconSize, folderIconSize), lockedIcon, GUIStyle.none);
			}

		}
	}
}