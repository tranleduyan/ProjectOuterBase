using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TimeManager : Singleton<TimeManager>, ISaveable
{
    private int gameYear = 1;
    private Season gameSeason = Season.Hazy;
    private int gameDay = 1;
    private int gameHour = 6;
    private int gameMinute = 30;
    private int gameSecond = 0;
    private string gameDayOfWeek = "Mon";

    private bool gameClockPaused = false;

    private float gameTick = 0f;
    private int gameSecondInDay = 23400;

    private string _iSaveableUniqueID;

    public string ISaveableUniqueID { get { return _iSaveableUniqueID; } set { _iSaveableUniqueID = value; } }

    private GameObjectSave _gameObjectSave;

    public GameObjectSave GameObjectSave { get { return _gameObjectSave; } set { _gameObjectSave = value; } }

    protected override void Awake()
    {
        base.Awake();

        ISaveableUniqueID = GetComponent<GenerateGUID>().GUID;

        GameObjectSave = new GameObjectSave();
    }

    void Start()
    {
        EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
    }

    private void OnEnable()
    {
        ISaveableRegister();

        EventHandler.BeforeSceneUnloadEvent += BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent += AfterSceneLoadFadeIn;
    }

    private void OnDisable()
    {
        ISaveableDeregister();

        EventHandler.BeforeSceneUnloadEvent -= BeforeSceneUnloadFadeOut;
        EventHandler.AfterSceneLoadEvent -= AfterSceneLoadFadeIn;
    }

    private void BeforeSceneUnloadFadeOut()
    {
        gameClockPaused = true;
    }

    private void AfterSceneLoadFadeIn()
    {
        gameClockPaused = false;
    }

    void Update()
    {
        if (!gameClockPaused)
        {
            GameTick();
        }
    }

    void GameTick()
    {
        gameTick += Time.deltaTime;
        if(gameTick >= Settings.secondsPerGameSecond)
        {
            gameTick -= Settings.secondsPerGameSecond;

            UpdateGameSecond();
        }
    }

    void UpdateGameSecond()
    {
        gameSecond++;
        gameSecondInDay++;
        if (gameSecond > 59)
        {
            gameSecond = 0;
            gameMinute++;

            if(gameMinute > 59)
            {
                gameMinute = 0;
                gameHour++;

                if(gameHour > 23)
                {
                    gameHour = 0;
                    gameSecondInDay = 0;
                    gameDay++;
                    if(gameDay > 30)
                    {
                        gameDay = 1;

                        int gseason = (int)gameSeason;
                        gseason++;

                        gameSeason = (Season)gseason;

                        if (gseason > 3) {

                            gseason = 0;
                            gameSeason = (Season)gseason;
                            gameYear++;

                            if(gameYear > 9999)
                            {
                                gameYear = 1;
                            }
                            EventHandler.CallAdvanceGameYearEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                        }
                        EventHandler.CallAdvanceGameSeasonEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);
                    }
                    gameDayOfWeek = GetDayOfWeek();
                    EventHandler.CallAdvanceGameDayEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); ;
                }
                EventHandler.CallAdvanceGameHourEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); 
            }
            EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond); ;
        }

        //call to advance game second event would go here if required
    }

    string GetDayOfWeek()
    {
        int totalDays = ((int)gameSeason * 30) + gameDay;
        int dayOfWeek = totalDays % 7;

        switch (dayOfWeek)
        {
            case 1:
                return "Mon";
            case 2:
                return "Tue";
            case 3:
                return "Wed";
            case 4:
                return "Thu";
            case 5:
                return "Fri";
            case 6:
                return "Sat";
            case 0: 
                return "Sun";
            default:
                return "";
        }
    }

    //return current gametime
    public TimeSpan GetGameTime()
    {
        TimeSpan gameTime = new TimeSpan(gameHour, gameMinute, gameSecond);

        return gameTime;
    }

    //Advance 1 game Minute for test purpose
    public void TestAdvanceGameMinute()
    {
        for(int i = 0; i < 60; i++)
        {
            UpdateGameSecond();
        }
    }

    //ADvance 1 gameDay for test purpose
    public void TestAdvanceGameDay()
    {
        for(int i= 0; i < 86400; i++)
        {
            UpdateGameSecond();
        }
    }
    //for sleeping to next day purpose
    public void AdvanceGameDay()
    {
        for (int i = gameSecondInDay; i < 109800; i++) //86400 + 23400 = 109800
        {
            UpdateGameSecond();

        }
        SaveLoadManager.Instance.SaveDataToFile();
        SaveLoadManager.Instance.LoadDataFromFile();
    }

    public void ISaveableRegister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Add(this);
    }

    public void ISaveableDeregister()
    {
        SaveLoadManager.Instance.iSaveableObjectList.Remove(this);
    }

    public GameObjectSave ISaveableSave()
    {
        //Delete existing scene save if exist
        GameObjectSave.sceneData.Remove(Settings.PreLoadScene);

        //Create new scene save
        SceneSave sceneSave = new SceneSave();

        //Create new int dictionary
        sceneSave.intDictionary = new Dictionary<string, int>();

        //Create new stirng dictionary
        sceneSave.stringDictionary = new Dictionary<string, string>();

        //add values to the int dictionary
        sceneSave.intDictionary.Add("gameYear", gameYear);
        sceneSave.intDictionary.Add("gameDay", gameDay);
        sceneSave.intDictionary.Add("gameHour", gameHour);
        sceneSave.intDictionary.Add("gameMinute", gameMinute);
        sceneSave.intDictionary.Add("gameSecond", gameSecond);

        //Add values to the string dictionary
        sceneSave.stringDictionary.Add("gameDayOfWeek", gameDayOfWeek);
        sceneSave.stringDictionary.Add("gameSeason", gameSeason.ToString());

        //Add scnesave to game object for preload scene
        GameObjectSave.sceneData.Add(Settings.PreLoadScene, sceneSave);

        return GameObjectSave;
    }

    public void ISaveableLoad(GameSave gameSave)
    {
        //Get Saved gameObject from gameSave data
        if(gameSave.gameObjectData.TryGetValue(ISaveableUniqueID, out GameObjectSave gameObjectSave))
        {
            GameObjectSave = gameObjectSave;

            //Get savedscene data for gameObject
            if(GameObjectSave.sceneData.TryGetValue(Settings.PreLoadScene, out SceneSave sceneSave))
            {
                //if int and string dictioanries are found
                if(sceneSave.intDictionary != null && sceneSave.stringDictionary != null)
                {
                    //populate saved int values
                    if(sceneSave.intDictionary.TryGetValue("gameYear", out int savedGameYear))
                    {
                        gameYear = savedGameYear;
                    }
                    if(sceneSave.intDictionary.TryGetValue("gameDay", out int savedGameDay))
                    {
                        gameDay = savedGameDay;
                    }
                    if(sceneSave.intDictionary.TryGetValue("gameHour", out int savedGameHour))
                    {
                        gameHour = savedGameHour;
                    }
                    if(sceneSave.intDictionary.TryGetValue("gameMinute", out int savedGameMinute))
                    {
                        gameMinute = savedGameMinute;
                    }
                    if(sceneSave.intDictionary.TryGetValue("gameSecond", out int savedGameSecond))
                    {
                        gameSecond = savedGameSecond;  
                    }

                    //populate string saved values
                    if(sceneSave.stringDictionary.TryGetValue("gameDayOfWeek", out string savedGameDayOfWeek))
                    {
                        gameDayOfWeek = savedGameDayOfWeek;
                    }
                    
                    if(sceneSave.stringDictionary.TryGetValue("gameSeason", out string savedGameSeason))
                    {
                        if(Enum.TryParse<Season>(savedGameSeason, out Season season))
                        {
                            gameSeason = season;
                        }
                    }

                    //zero gametick
                    gameTick = 0f;

                    //trigger advance minute event aka refresh gameclock
                    EventHandler.CallAdvanceGameMinuteEvent(gameYear, gameSeason, gameDay, gameDayOfWeek, gameHour, gameMinute, gameSecond);


                }
            }
        }
    }

    public void ISaveableStoreScene(string sceneName)
    {
        //Nothing in here since time manager is in preload scene
    }

    public void ISaveableRestoreScene(string sceneName)
    {
        //Nothing in here since time manager is in preload scene
    }
}
