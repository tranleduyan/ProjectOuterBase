public enum SceneName
{
    HomeLand,
    HomeHouse,
    AuraCity,
    GonasTown,
    KhakanyCity,
    GobbaFamilyHouseGroundFloor,
    GobbaFamilyHouseCandyStoreBasement
}

public enum Season
{
    Hazy,
    Sandy,
    Storm,
    Snowy,
    none,
    count
}

public enum Weather
{
     cloudy, 
     raining, 
     snowing,
     none, 
     count
}

public enum GridBoolProperty
{
    diggable,
    canDropItem,
    canPlaceFurniture,
    isPath,
    isNPCObstacle, 
    isExtendFurnitureWall
}
public enum InventoryLocation
{
    player,
    chest,
    count
}

public enum Direction
{
    Up,
    Down, 
    Left,
    Right,
    none,
}

public enum ToolEffect
{
    none,
    watering
}

public enum HarvestActionEffect
{
    eoliaLeavesFalling,
    viliaLeavesFalling,
    obionyLeavesFalling,
    sillesLeavesFalling,
    gossiaLeavesFalling,
    choppingTreeTrunk,
    goraBreakingStone,
    berusBreakingStone,
    leonBreakingStone,
    larsBreakingStone,
    reaping,
    none
}

public enum ItemType
{
    Seed,
    Tree_Seed,
    Commodity,
    Watering_tool,
    Hoeing_tool,
    Chopping_tool,
    Breaking_tool,
    Reaping_tool,
    Collecting_tool,
    Reapable_scenary,
    Furniture,
    none,
    count
}

public enum Facing
{
    none, 
    down,
    up,
    right,
    left
}