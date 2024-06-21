using System;
using System.Collections.Generic;

namespace GrapplingGame.GameObjectsComponentsLevels.Helpers;
// A Vector2-esque struct for ints instead of floats(use for pixel values)
public struct Vector2Int
{
	public int x;
	public int y;

	public Vector2Int(int x, int y)
	{
		this.x = x;
		this.y = y;
	}
}

public struct GLOBALS
{
	public static int GRAVITY = 6;
	public static int FPS = 15;
	public static double TIMEBETWEENFRAMES = 0.1;
}