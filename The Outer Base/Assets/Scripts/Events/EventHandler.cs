using System;
using System.Collections.Generic;
using UnityEngine;

public delegate void MovementDelegate(float inputX, float inputY, bool isWalking, bool isIdle
    /*, bool isCarrying*/, ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight);

public static class EventHandler
{

    //Drop Selected Item event
    public static event Action DropSelectedItemEvent;

    public static void CallDropSelectedItemEvent()
    {
        if(DropSelectedItemEvent != null)
        {
            DropSelectedItemEvent();
        }
    }

    //remove selected item from inventory
    public static event Action RemoveSelectedItemFromInventoryEvent;

    public static void CallRemoveSelectedItemFromInventoryEvent()
    {
        if(RemoveSelectedItemFromInventoryEvent != null)
        {
            RemoveSelectedItemFromInventoryEvent();
        }
    }

    //harvest action effect event
    public static event Action<Vector3, HarvestActionEffect> HarvestActionEffectEvent;

    public static void CallHarvestActionEffectEvent(Vector3 effectPosition, HarvestActionEffect harvestActionEffect)
    {
        if(HarvestActionEffectEvent != null)
        {
            HarvestActionEffectEvent(effectPosition, harvestActionEffect);
        }
    }

    //Inventory Updated Event
    public static event Action<InventoryLocation, List<InventoryItem>> InventoryUpdatedEvent;

    public static void CallInventoryUpdatedEvent(InventoryLocation inventoryLocation, List<InventoryItem> inventoryList)
    {
        if (InventoryUpdatedEvent != null)
        {
            InventoryUpdatedEvent(inventoryLocation, inventoryList);
        }
    }

    //Instantiate crop prefabs
    public static event Action InstantiateCropPrefabsEvent;

    public static void CallInstantiateCropPrefabsEvent()
    {
        if(InstantiateCropPrefabsEvent != null)
        {
            InstantiateCropPrefabsEvent();
        }
    }

    //movement event 
    public static event MovementDelegate MovementEvent;

    //call movement event from player 
    public static void CallMovementEvent(float inputX, float inputY, bool isWalking, bool isIdle,
    ToolEffect toolEffect,
    bool isUsingToolRight, bool isUsingToolLeft, bool isUsingToolUp, bool isUsingToolDown,
    bool isSwingingToolRight, bool isSwingingToolLeft, bool isSwingingToolUp, bool isSwingingToolDown,
    bool idleUp, bool idleDown, bool idleLeft, bool idleRight)
    {
        if (MovementEvent != null)
        {
            MovementEvent(inputX, inputY, isWalking, isIdle,
                toolEffect,
                isUsingToolRight, isUsingToolLeft, isUsingToolUp, isUsingToolDown,
                isSwingingToolRight, isSwingingToolLeft, isSwingingToolUp, isSwingingToolDown,
                idleUp, idleDown, idleLeft, idleRight);
        }
    }

    //Time Events

    //Advance game minute
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameMinuteEvent;

    public static void CallAdvanceGameMinuteEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameMinuteEvent != null)
        {
            AdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game hour

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameHourEvent;

    public static void CallAdvanceGameHourEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameHourEvent != null)
        {
            AdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
        }
    }

    //Advance game Day

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameDayEvent;

    public static void CallAdvanceGameDayEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameDayEvent != null)
        {
            AdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

        }
    }

    //Advance game season

    public static event Action<int, Season, int, string, int, int, int> AdvanceGameSeasonEvent;

    public static void CallAdvanceGameSeasonEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameSeasonEvent != null)
        {
            AdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

        }
    }

    //Advance game year
    public static event Action<int, Season, int, string, int, int, int> AdvanceGameYearEvent;

    public static void CallAdvanceGameYearEvent(int gameYear, Season gameSeason, int gameDay, string gameDayOfWeek, int gameHour, int gameMinute, int gameSecond)
    {
        if (AdvanceGameYearEvent != null)
        {
            AdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);

        }
    }

    //Scene Load Events - in the order they happen
    //before scene unload fadeout event

    public static event Action BeforeSceneUnloadFadeOutEvent;

    public static void CallBeforeSceneUnloadFadeOutEvent()
    {
        if(BeforeSceneUnloadFadeOutEvent != null)
        {
            BeforeSceneUnloadFadeOutEvent();
        }
    }

    //before scene unload event
    public static event Action BeforeSceneUnloadEvent;

    public static void CallBeforeSceneUnloadEvent()
    {
        if(BeforeSceneUnloadEvent != null)
        {
            BeforeSceneUnloadEvent();
        }
    }

    //after scene load event
    public static event Action AfterSceneLoadEvent;

    public static void CallAfterSceneLoadEvent()
    {
        if (AfterSceneLoadEvent != null)
        {
            AfterSceneLoadEvent();
        }
    }

    //after scene load fade in event
    public static event Action AfterSceneLoadFadeInEvent;

    public static void CallAfterSceneLoadFadeInEvent()
    {
        if(AfterSceneLoadFadeInEvent != null)
        {
            AfterSceneLoadFadeInEvent();
        }
    }
}

