using System;
using System.Collections.Generic;

namespace GrapplingGame.GameObjectsComponentsLevels.Helpers;

public struct GLOBALS
{
    public static int GRAVITY = 6;
    public static int FPS = 15;
    public static double TIMEBETWEENFRAMES = 0.1;
}

public enum TARGETTYPE
{
    swing,
    pull
}

public enum CAMERAMODE
{
    playerCenter, // Note: the player doesn't actually go this center in this mode until they have passed the middle of the screen
    playerLeft,
    playerRight,
    playerBottom,
    playerTop
}