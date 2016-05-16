using UnityEngine;
using System.Collections;

public class Grid {

	public int x;
	public int y;
	public int objectNumberOnGrid;
	public bool isOccupied { get { return objectNumberOnGrid > 0; } } 

	public Grid(int x, int y)
	{
		this.x = x;
		this.y = y;
		objectNumberOnGrid = 0;
	}

}
