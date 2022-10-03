using UnityEngine;

public static class Settings
{
    //scenes
    public const string PreLoadScene = "PreLoadScene";

    //obscuring Item Fading - obscuring item fader

    public const float fadeInSeconds = 0.25f;
    public const float fadeOutSeconds = 0.35f;
    public const float targetAlpha = 0.45f;

    //TileMap
    public const float gridCellSize = 1f; //grid cell size in unity units
    public const float gridCellDiagonalSize = 1.41f; //diagonal distance between unity cell centers
    public static Vector2 cursorSize = Vector2.one;
    public const int maxGridHeight = 99999;
    public const int maxGridWidth = 99999;

    //Player Center point
    public const float playerCenterYOffSet = 0.875f;

    //Player Tag String
    public const string PlayerTag = "Player";


    //Inventory
    public static int playerInitialInventoryCapacity = 24;
    public static int playerMaximumInventoryCapacity = 48;

    //Tools
    public const string HoeingTool = "Hoe";
    public const string ChoppingTool = "Axe";
    public const string BreakingTool = "Pickaxe";
    public const string ReapingTool = "Scythe";
    public const string WateringTool = "Water Bottle";
    public const string CollectingTool = "Basket";

    //Reaping
    public const int maxCollidersToTestPerReapSwing = 15;
    public const int maxTargetComponentsToDestroyPerReapSwing = 2;

    //Time System
    public const float secondsPerGameSecond = 0.012f;

    //Player Tool Usage
    public static float useToolAnimationPause = 0.25f;
    public static float afterUseToolAnimationPause = 0.2f;
    public static float liftToolAnimationPause = 0.4f;
    public static float afterLiftToolAnimationPause = 0.4f;
    public static float pickAnimationPause = 1f;
    public static float afterPickAnimationPause = 0.2f;

    //NPC Movement
    public static float pixelSize = 0.0625f; // for movement code moving NPC to test whether npc is in a pixel size away from the target position before we end the loop

    //NPC animation parameters
    public static int walkUp;
    public static int walkDown;
    public static int walkLeft;
    public static int walkRight;
    public static int eventAnimation;
    public static int idleUp;
    public static int idleDown;
    public static int idleLeft;
    public static int idleRight;

    static Settings()
    {
        //NPC animation parameters
        idleUp = Animator.StringToHash("idleUp");
        idleDown = Animator.StringToHash("idleDown");
        idleLeft = Animator.StringToHash("idleLeft");
        idleRight = Animator.StringToHash("idleRight");
        walkUp = Animator.StringToHash("walkUp");
        walkDown = Animator.StringToHash("walkDown");
        walkLeft = Animator.StringToHash("walkLeft");
        walkRight = Animator.StringToHash("walkRight");
        eventAnimation = Animator.StringToHash("eventAnimation");
    }
}
