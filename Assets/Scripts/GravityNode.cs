using UnityEngine;
using System.Collections;

public class GravityNode
{
	private GravityNode next, previous;
	private GameObject item;
	private GravityScript itemScript;

	public GravityNode(GameObject item)
	{
		this.item = item;
		this.SetScript();
		next = null;
		previous = null;
	}

	public void SetNext(GravityNode next)
	{
		this.next = next;
	}

	//Returns the next object in the list
	public GravityNode GetNext()
	{
		return next;
	}

	public bool HasNext()
	{
		return (next != null);
	}

	public void SetPrevious(GravityNode previous)
	{
		this.previous = previous;
	}

	public GravityNode GetPrevious()
	{
		return previous;
	}

	public bool HasPrevious()
	{
		return (previous != null);
	}

	public void SetItem(GameObject item)
	{
		this.item = item;
	}

	public GameObject GetItem()
	{
		return item;
	}

	public void SetScript()
	{
		itemScript = item.GetComponent<GravityScript>(); 
	}

	public GravityScript GetScript()
	{
		return itemScript;
	}
}