using UnityEngine;
using System.Collections;
using System.Collections.Generic;

public class PathFinder : MonoBehaviour {

	void Start () {

	}

	public static Int2[] getPath(Int2 current, Int2 final, GridSystem grid) {
		int width = grid.gridSizeX;
		int height = grid.gridSizeZ;
		if (width <= current._x || width <= final._x) {
			return null;
		}
		if (height <= current._z || height <= final._z) {
			return null;
		}
		if (grid[current._x, current._z] == null || grid[final._x,final._z] == null)
		{
			return null;
		}
		int[,] distanceGrid = new int[width, height];
		for (int i = 0; i < width; i++) {
			for (int j = 0; j < height; j++) {
				distanceGrid[i,j] = -1;
			}
		}

		distanceGrid [current._x, current._z] = 0;
		Int2[] path =  DFS (current, final, grid, distanceGrid);
		Debug.Log ("Path");
		for (int i = 0; i < path.Length; i++) {
			Debug.Log (path[i]);
		}
		return path;

	}

	private static Int2[] DFS(Int2 from, Int2 destination, GridSystem grid, int[,] distanceGrid) {
		Int2[][] best = new Int2[4][];

		if (from._x > 0 && grid[from._x - 1,from._z] != null) {
			if (distanceGrid[from._x - 1,from._z] == -1 || distanceGrid[from._x - 1,from._z] > (distanceGrid[from._x,from._z] + 1)) {
				distanceGrid[from._x - 1,from._z] = distanceGrid[from._x,from._z] + 1;
				Int2 next = new Int2 (from._x - 1, from._z);
				best[0] = DFS(next, destination, grid, distanceGrid);
			}
		} 

		if (from._x < (grid.gridSizeX - 1) && grid[from._x + 1,from._z] != null) {
			if (distanceGrid[from._x + 1,from._z] == -1 || distanceGrid[from._x + 1,from._z] > (distanceGrid[from._x,from._z] + 1)) {
				distanceGrid[from._x + 1,from._z] = distanceGrid[from._x,from._z] + 1;
				Int2 next = new Int2 (from._x + 1, from._z);
				best[1] = DFS(next,  destination, grid, distanceGrid);
			}
		}
		
		if (from._z > 0 && grid[from._x, from._z - 1] != null) {
			if (distanceGrid[from._x,from._z - 1] == -1 || distanceGrid[from._x, from._z - 1] > (distanceGrid[from._x, from._z] + 1)) {
				distanceGrid[from._x,from._z - 1] = distanceGrid[from._x, from._z] + 1;
				Int2 next = new Int2 (from._x, from._z - 1);
				best[2] = DFS(next, destination, grid, distanceGrid);
			}
		} 

		if (from._z < (grid.gridSizeZ - 1) && grid[from._x, from._z + 1] != null) {
			if (distanceGrid[from._x, from._z + 1] == -1 || distanceGrid[from._x, from._z + 1] > (distanceGrid[from._x, from._z] + 1)) {
				distanceGrid[from._x, from._z + 1] = distanceGrid[from._x, from._z] + 1;
				Int2 next = new Int2 (from._x, from._z + 1);
				best[3] = DFS(next, destination, grid, distanceGrid);
			}
		}

		int nearestIdx = -1;
		float curDist = (destination - from).magnitude;
		int bestLength = 1000;
		float nextDist;
		for (int i = 0; i < 4; i++) {
			if (best[i] != null && best[i].Length > 0) {
				nextDist = (destination - best[i][0]).magnitude;
				if (nextDist < curDist || (nextDist == curDist && bestLength > best[i].Length)) {
					nearestIdx = i;
					curDist = nextDist;
					bestLength = best[i].Length;
				}
			}
		}

		if (nearestIdx == -1) {
			return new Int2[]{from};
		} else {
			Int2[] currentChain = new Int2[best[nearestIdx].Length + 1];
			for(int i = 0; i < best[nearestIdx].Length; i++) {
				currentChain[i] = best[nearestIdx][i]; 
				currentChain[currentChain.Length - 1] = from;
			}
			return currentChain;
		}
	}


}
