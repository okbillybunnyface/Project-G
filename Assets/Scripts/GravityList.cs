using UnityEngine;
using System.Collections;

public class GravityList
{
	private GravityNode first, current;

	public GravityList()
	{
		first = null;
		current = null;
	}

	public void Enqueue(GameObject item)
	{
		GravityNode temp = new GravityNode(item);
		if(!this.IsEmpty())
		{
			temp.SetNext(first);
			first.SetPrevious(temp);
		}

		first = temp;
	}

	//Removes item from the list, along with its script. Returns false if list doesn't contain item, true otherwise.
	public bool Dequeue(GameObject item)
	{
		if(this.Contains(item))
		{
			if(current.HasNext())
			{
				if(current.HasPrevious())
				{
					//Set the previous node's next node to be the current node's next node.
					current.GetPrevious().SetNext(current.GetNext());
					//Set the next node's previous node to be the current node's previous node.
					current.GetNext().SetPrevious(current.GetPrevious());
					//Set the current node to be the previous node.
					current = current.GetPrevious();

					return true;
				}
				else//it must be the first node if there isn't a previous node
				{
					//Set the second node's previous node to null.
					current.GetNext().SetPrevious(null);
					//Set the second node to now be the first node.
					first = current.GetNext();
					//Set current to the new first node.
					current = first;

					return true;
				}
			}
			else//it must be the last node if there is no next node
			{
				if(current.HasPrevious())
				{
					//Sets the previous node's next node to null.
					current.GetPrevious().SetNext(null);
					//Sets current to the previous node.
					current = current.GetPrevious();

					return true;
				}
				else//it must be the only node
				{
					current = null;
					first = null;

					return true;
				}
			}
		}

		return false;
	}

	//Returns true is the list contains the item and sets current to be that item, otherwise returns false and sets current to be the first.
	public bool Contains(GameObject item)
	{
		if(this.IsEmpty())
		{
			return false;
		}

		current = first;

		if(current.GetItem() == item)
		{
			return true;
		}
		
		while(current.HasNext())
		{
			current = current.GetNext();
			if(current.GetItem() == item)
			{
				return true;
			}
		}

		current = first;
		return false;
	}

	public void Next()
	{
		if(current.HasNext())
		{
			current = current.GetNext();
		}
	}

	public void Previous()
	{
		if(current.HasPrevious())
		{
			current = current.GetPrevious();
		}
	}

	public bool IsEmpty()
	{
		return (first == null);
	}

	public GravityNode GetCurrent()
	{
		return current;
	}

	public void Reset()
	{
		current = first;
	}
}