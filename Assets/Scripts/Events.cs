using System;
public class Events
{
    // player states
    public static String START = "START";
    public static String PAUSE = "PAUSE";
    public static String UNPAUSE = "UNPAUSE";

    // trigger when the player reaches the goal on the map
    public static String GOAL = "GOAL";

    // trigger when the player presses space or touches the screen
    public static String PRESS = "PRESS";
    public static String UNPRESS = "UNPRESS";

    // trigger when the player dies
    public static String DIE = "DIE";

    // after all grab points have been created
    public static String POINTS_CREATED = "POINTS_CREATED";

    // after the game manager has generated a new set of colors
    public static String COLOR_CHANGE = "COLOR_CHANGE";

    // Timer for sppedruns
    public static String START_TIMER = "START_TIMER";
    public static String STOP_TIMER = "STOP_TIMER";

    // blip blip blip blipp!
    public static String START_COUNTDOWN = "START_COUNTDOWN";

    //sound control
    public static string MUTE = "MUTE";
    public static string UNMUTE = "UNMUTE";

}
