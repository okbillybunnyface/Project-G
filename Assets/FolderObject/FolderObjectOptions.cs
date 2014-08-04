using System.Collections;
using UnityEngine;

public static class FolderObjectOptions
{

	// Edit FolderObject behaviour options here

	public static bool displayFolderIconInHierarchy = true;			// Display the Folder icons to the right of all Folder objects in the Hierarchy
	
	public static bool useFolderNamePrefix = false;					// Automatically prefix all Folder object names with the "folderNamePrefix" value on creation
	public static string folderNamePrefix = "FOLDER_";				// Prefix to use if the above option is set to true

	public static bool displayFolderToggleDescriptions = true;		// Set to false to hide descriptions beneath "Lock Folder" and "Hide Children" tools in Folder Object Inspector window
}