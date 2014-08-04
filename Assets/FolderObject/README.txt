HIERARCHY FOLDER OBJECT
Unity Editor Extension

By Bright Static Media
http://www.brightstatic.co.uk

- Coded by Robbie Cargill
- Folder icons provided by Yusuke Kamiyamane. Licensed under a Creative Commons Attribution 3.0 License.

- For instructions of use see Instructions.pdf

- Next anticipated release will be major update Version 1.1 in late April, which will add additional functionality when locking Folders to prevent child objects from being edited.


CHANGE LOG

Version: 1.0.4
Date: 01 Apr 14
	- Bug fix: Wrapped FolderObject OnDestroy function in additional "#if UNITY_EDITOR" check.

Version: 1.0.3
Date: 24 Jan 14
	- Bug fix: Safely returns a Folder GameObject and its children to its previous when the Folder component is removed in the Inspector
		- Uses OnDestroy function to unlock the folder and show children (if locked/hidden)
		- Sets Transform components HideFlags back to None in order to reshow once Folder component removed

Version: 1.0.2
Date: 02 Oct 13
	- Instructions PDF added for clearer visual instructions of use