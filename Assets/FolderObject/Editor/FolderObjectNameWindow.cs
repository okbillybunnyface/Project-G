/*
	Hierarchy Folder Object
	Unity Editor Extension

	Version: 1.0.4
	Date: 01 Apr 14

	By Bright Static Media
	http://www.brightstatic.co.uk
*/

using UnityEngine;
using UnityEditor;
using System.Collections.Generic;

public class FolderObjectNameWindow : EditorWindow
{
	string folderName = "";

	string notification = " ";

	void OnGUI()
	{
		folderName = EditorGUILayout.TextField("", folderName);

		if (Input.GetKey(KeyCode.Return) && EditorWindow.focusedWindow == this)
		{
			UseFolderName(folderName);
		}

		GUILayout.BeginHorizontal("box");
		
		if(GUILayout.Button("OK", GUILayout.Width(140)))
		{
			UseFolderName(folderName);
		}
		
		GUI.SetNextControlName("Cancel");
		if(GUILayout.Button("Cancel", GUILayout.Width(140)))
			this.Close();

		GUILayout.EndHorizontal();

		GUILayout.Label(notification);

	}


	void UseFolderName(string name)
	{
		
		if (name.Length < 1)
		{
			notification = EditorGUILayout.TextField("Please enter a folder name!");
			return;
		}
		
		if (name.Length > 100)
		{
			notification = EditorGUILayout.TextField("Folder name can't be over 100 characters.");
			return;
		}
		
		if (FolderObjectOptions.useFolderNamePrefix && FolderObjectOptions.folderNamePrefix != null)
			name = FolderObjectOptions.folderNamePrefix + name;
		
		GameObject newFolderObject = new GameObject(name);
		newFolderObject.AddComponent<FolderObject>();

		this.Close();
	}


}