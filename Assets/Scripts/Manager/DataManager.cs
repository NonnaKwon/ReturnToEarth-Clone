using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using UnityEngine;
using static UnityEngine.Rendering.DebugUI;
/// <summary>
/// 제작 : 찬규
/// 각종 데이터테이블을 관리하는 매니저
/// </summary>
public class DataManager : Singleton<DataManager>
{
    #region data
    GameData gameData;

    IDData iDData;
    ResourceData resourceData;
    TextData textData;
    TextBundleData textBundleData;
    UIData uIData;
    PuzzleData puzzleData;
    ItemData itemData;
    InteractionObjectData interactionObjectData;
    StageData stageData;
    ChapterData chapterData;
    UserInfoData userInfoData;
    UserItemData userItemData;
    InventoryData inventoryData;
    StageProcessData stageProcessData;
    UserLogData userLogData;

    Dictionary<int, IDTable> iDTables;
    Dictionary<int, ResourceTable> resourceTables;
    Dictionary<int, TextTable> textTables;
    Dictionary<int, TextBundleTable> textBundleTables;
    Dictionary<int, UITable> uITables;
    Dictionary<int, PuzzleTable> puzzleTables;
    Dictionary<int, ItemTable> itemTables;
    Dictionary<int, InteractionObject> interactionObjects;
    Dictionary<int, StageTable> stageTables;
    Dictionary<int, ChapterTable> chapterTables;
    Dictionary<int, UserInfoTable> userInfoTables;
    Dictionary<int, UserItemTable> userItemTables;
    Dictionary<int, InventoryTable> inventoryTables;
    Dictionary<int, StageProcessTable> stageProcessTables;
    Dictionary<int, UserLogTable> userLogTables;

    public GameData GameData { get { return gameData; } }

    public IDData IDData { get { return iDData; } }
    public ResourceData ResourceData { get { return resourceData; } }
    public TextData TextData { get { return textData; } }
    public TextBundleData TextBundleData { get { return textBundleData; } }
    public UIData UIData { get { return uIData; } }
    public PuzzleData PuzzleData { get { return puzzleData; } }
    public ItemData ItemData { get { return itemData; } }
    public InteractionObjectData InteractionOjectData { get { return interactionObjectData; } }
    public StageData StageData { get { return stageData; } }
    public ChapterData ChapterData { get { return chapterData; } }
    public UserInfoData UserInfoData { get { return userInfoData; } }
    public UserItemData UserItemData { get { return userItemData; } }
    public InventoryData InventoryData { get { return inventoryData; } }
    public StageProcessData StageProcessData { get { return stageProcessData; } }
    public UserLogData UserLogData { get { return userLogData; } }

    public Dictionary<int, IDTable> IDTables { get { return iDTables; } }
    public Dictionary<int, ResourceTable> ResourceTables { get { return resourceTables; } }
    public Dictionary<int, TextTable> TextTables { get { return textTables; } }
    public Dictionary<int, TextBundleTable> TextBundleTables { get { return textBundleTables; } }
    public Dictionary<int, UITable> UITables { get { return uITables; } }
    public Dictionary<int, PuzzleTable> PuzzleTables { get { return puzzleTables; } }
    public Dictionary<int, ItemTable> ItemTables { get { return itemTables; } }
    public Dictionary<int, InteractionObject> InteractionObjects { get { return interactionObjects; } }
    public Dictionary<int, StageTable> StageTables { get { return stageTables; } }
    public Dictionary<int, ChapterTable> ChapterTables { get { return chapterTables; } }
    public Dictionary<int, UserInfoTable> UserInfoTables { get { return userInfoTables; } }
    public Dictionary<int, UserItemTable> UserItemTables { get { return userItemTables; } }
    public Dictionary<int, InventoryTable> InventoryTables { get { return inventoryTables; } }
    public Dictionary<int, StageProcessTable> StageProcessTables { get { return stageProcessTables; } }
    public Dictionary<int, UserLogTable> UserLogTables { get { return userLogTables; } }


    TestCaseData testCaseData;
    public TestCaseData TestCaseData { get { return testCaseData; } }
    Dictionary<int, TestCase> testCaseTables;
    public Dictionary<int, TestCase> TestCaseTables { get { return testCaseTables; } }
    #endregion

#if UNITY_EDITOR
    private string path => Path.Combine(Application.dataPath, $"Resources/Data");
#else
    private string path => Path.Combine(Application.persistentDataPath, $"Resources/Data");
#endif

    #region DataMethod

    public void LoadAllTable()
    {
        LoadTable("IDTable");
        LoadTable("ResourceTable");
        LoadTable("TextTable");
        LoadTable("TextBundleTable");
        LoadTable("UITable");
        LoadTable("PuzzleTable");
        LoadTable("ItemTable");
        LoadTable("InteractionObjectTable");
        LoadTable("StageTable");
        LoadTable("ChapterTable");
        LoadTable("UserInfoTable");
        LoadTable("UserItemTable");
        LoadTable("InventoryTable");
        LoadTable("StageProcessTable");
        LoadTable("UserLogTable");
    }

    public void NewData()
    {
        gameData = new GameData();
    }

    public void SaveData(int index)
    {
        if (Directory.Exists($"{path}/SaveLoad") == false)
        {
            Directory.CreateDirectory($"{path}/SaveLoad");
        }

        string json = JsonUtility.ToJson(gameData, true);
        File.WriteAllText($"{path}/SaveLoad/{index}.txt", json);
    }

    public void LoadData(int index)
    {
        if (File.Exists($"{path}/SaveLoad/{index}.txt") == false)
        {
            NewData();
            return;
        }

        string json = File.ReadAllText($"{path}/SaveLoad/{index}.txt");
        try
        {
            gameData = JsonUtility.FromJson<GameData>(json);
        }
        catch (Exception ex)
        {
            Debug.LogWarning($"Load data fail : {ex.Message}");
            NewData();
        }
    }

    public bool ExistData(int index)
    {
        return File.Exists($"{path}/{index}.txt");
    }

    #endregion


    #region DataTableMethod

    /// <summary>
    /// 변동되는 데이터테이블을 json으로 다시 저장하는 함수
    /// </summary>
    /// <param name="fileName"></param>
    public void SaveTable(string fileName)
    {
        if (Directory.Exists($"{path}/Json") == false)
        {
            Directory.CreateDirectory($"{path}/Json");
        }

        // 파일 이름 검사 후
        // 각 데이터별 dictionary의 값을 List로 언팩한다
        // 언팩한 리스트를 하나하나 Json으로 파싱하여 붙인다
        // 앞 뒤에 배열 임을 확인 할 수 있게 대괄호를 붙여준다
        // 해당 Json을 해당하는 위치에 저장한다
        switch (fileName)
        {
            case "UserInfoTable":
                List<UserInfoTable> _userInfoTable = new List<UserInfoTable>();
                string userInfoTable = "";

                foreach (UserInfoTable i in UserInfoTables.Values)
                {
                    _userInfoTable.Add(i);
                }

                for (int i = 0; i < _userInfoTable.Count; i++)
                {
                    if (i == 0)
                    {
                        string _json = JsonUtility.ToJson(_userInfoTable[i], true);
                        userInfoTable += _json;
                    }
                    else
                    {
                        string _json = "," + JsonUtility.ToJson(_userInfoTable[i], true);
                        userInfoTable += _json;
                    }
                }
                userInfoTable = "[" + userInfoTable + "]";
                File.WriteAllText($"{path}/Json/{fileName}.json", userInfoTable);
                Debug.Log("저장 성공");
                break;
            case "UserItemTable":
                List<UserItemTable> _userItemTable = new List<UserItemTable>();
                string userItemTable = "";

                foreach (UserItemTable i in UserItemTables.Values)
                {
                    _userItemTable.Add(i);
                }

                for (int i = 0; i < _userItemTable.Count; i++)
                {
                    if (i == 0)
                    {
                        string _json = JsonUtility.ToJson(_userItemTable[i], true);
                        userItemTable += _json;
                    }
                    else
                    {
                        string _json = "," + JsonUtility.ToJson(_userItemTable[i], true);
                        userItemTable += _json;
                    }
                }
                userItemTable = "[" + userItemTable + "]";
                File.WriteAllText($"{path}/Json/{fileName}.json", userItemTable);
                Debug.Log("저장 성공");
                break;
            case "InventoryTable":
                List<InventoryTable> _inventoryTable = new List<InventoryTable>();
                string inventoryTable = "";

                foreach (InventoryTable i in InventoryTables.Values)
                {
                    _inventoryTable.Add(i);
                }

                for (int i = 0; i < _inventoryTable.Count; i++)
                {
                    if (i == 0)
                    {
                        string _json = JsonUtility.ToJson(_inventoryTable[i], true);
                        inventoryTable += _json;
                    }
                    else
                    {
                        string _json = "," + JsonUtility.ToJson(_inventoryTable[i], true);
                        inventoryTable += _json;
                    }
                }
                inventoryTable = "[" + inventoryTable + "]";
                File.WriteAllText($"{path}/Json/{fileName}.json", inventoryTable);
                Debug.Log("저장 성공");
                break;
            case "StageProcessTable":
                List<StageProcessTable> _stageProcessTable = new List<StageProcessTable>();
                string stageProcessTable = "";

                foreach (StageProcessTable i in StageProcessTables.Values)
                {
                    _stageProcessTable.Add(i);
                }

                for (int i = 0; i < _stageProcessTable.Count; i++)
                {
                    if (i == 0)
                    {
                        string _json = JsonUtility.ToJson(_stageProcessTable[i], true);
                        stageProcessTable += _json;
                    }
                    else
                    {
                        string _json = "," + JsonUtility.ToJson(_stageProcessTable[i], true);
                        stageProcessTable += _json;
                    }
                }
                stageProcessTable = "[" + stageProcessTable + "]";
                File.WriteAllText($"{path}/Json/{fileName}.json", stageProcessTable);
                Debug.Log("저장 성공");
                break;
            case "UserLogTable":
                List<UserLogTable> _userLogTable = new List<UserLogTable>();
                string userLogTable = "";

                foreach (UserLogTable i in UserLogTables.Values)
                {
                    _userLogTable.Add(i);
                }

                for (int i = 0; i < _userLogTable.Count; i++)
                {
                    if (i == 0)
                    {
                        string _json = JsonUtility.ToJson(_userLogTable[i], true);
                        userLogTable += _json;
                    }
                    else
                    {
                        string _json = "," + JsonUtility.ToJson(_userLogTable[i], true);
                        userLogTable += _json;
                    }
                }
                userLogTable = "[" + userLogTable + "]";
                File.WriteAllText($"{path}/Json/{fileName}.json", userLogTable);
                Debug.Log("저장 성공");
                break;
            default:
                List<TestCase> _testCase = new List<TestCase>();
                string testCaseTable = "";

                foreach (TestCase testCase in TestCaseTables.Values)
                {
                    _testCase.Add(testCase);
                }

                for (int i = 0; i < _testCase.Count; i++)
                {
                    if (i == 0)
                    {
                        string _json = JsonUtility.ToJson(_testCase[i], true);
                        testCaseTable += _json;
                    }
                    else
                    {
                        string _json = "," + JsonUtility.ToJson(_testCase[i], true);
                        testCaseTable += _json;
                    }
                }
                testCaseTable = "[" + testCaseTable + "]";
                File.WriteAllText($"{path}/Json/{fileName}.json", testCaseTable);
                Debug.Log("저장 성공");
                break;
        }
    }

    //---> 권새롬 추가 : json을 받아왔을때 파일에 로컬 저장하는 함수
    public void SaveJson(Define.JsonFile jsonFileName,string json)
    {
        if (Directory.Exists($"{path}/Json") == false)
        {
            Directory.CreateDirectory($"{path}/Json");
            Debug.Log("sss");
            Debug.Log(Directory.Exists($"{path}/Json"));
        }

        File.WriteAllText($"{path}/Json/{jsonFileName}.json", json);
    }

    ///---------------------------------

    /// <summary>
    /// 씬 전환 시 데이터테이블을 불러와서 저장해놓는 함수
    /// </summary>
    /// <param name="fileName"></param>
    public void LoadTable(string fileName)
    {
        if (Directory.Exists($"{path}/Json") == false)
        {
            Debug.Log("Json 폴더 없음");
            return;
        }

        string json = File.ReadAllText($"{path}/Json/{fileName}.json");
        json = "{\"tables\":" + json + "}";

        switch (fileName)
        {
            case "IDTable":
                iDData = JsonUtility.FromJson<IDData>(json);
                iDTables = iDData.MakeDic();
                break;
            case "ResourceTable":
                resourceData = JsonUtility.FromJson<ResourceData>(json);
                resourceTables = resourceData.MakeDic();
                break;
            case "TextTable":
                textData = JsonUtility.FromJson<TextData>(json);
                textTables = textData.MakeDic();
                break;
            case "TextBundleTable":
                textBundleData = JsonUtility.FromJson<TextBundleData>(json);
                textBundleTables = textBundleData.MakeDic();
                break;
            case "UITable":
                uIData = JsonUtility.FromJson<UIData>(json);
                uITables = uIData.MakeDic();
                break;
            case "PuzzleTable":
                puzzleData = JsonUtility.FromJson<PuzzleData>(json);
                puzzleTables = puzzleData.MakeDic();
                break;
            case "ItemTable":
                itemData = JsonUtility.FromJson<ItemData>(json);
                itemTables = itemData.MakeDic();
                break;
            case "InteractionObjectTable":
                interactionObjectData = JsonUtility.FromJson<InteractionObjectData>(json);
                interactionObjects = interactionObjectData.MakeDic();
                break;
            case "StageTable":
                stageData = JsonUtility.FromJson<StageData>(json);
                stageTables = stageData.MakeDic();
                break;
            case "ChapterTable":
                chapterData = JsonUtility.FromJson<ChapterData>(json);
                chapterTables = chapterData.MakeDic();
                break;
            case "UserInfoTable":
                userInfoData = JsonUtility.FromJson<UserInfoData>(json);
                userInfoTables = userInfoData.MakeDic();
                break;
            case "UserItemTable":
                userItemData = JsonUtility.FromJson<UserItemData>(json);
                userInfoTables = userInfoData.MakeDic();
                break;
            case "InventoryTable":
                inventoryData = JsonUtility.FromJson<InventoryData>(json);
                inventoryTables = inventoryData.MakeDic();
                break;
            case "StageProcessTable":
                stageProcessData = JsonUtility.FromJson<StageProcessData>(json);
                stageProcessTables = stageProcessData.MakeDic();
                break;
            case "UserLogTable":
                userLogData = JsonUtility.FromJson<UserLogData>(json);
                userLogTables = userLogData.MakeDic();
                break;
            default:
                testCaseData = JsonUtility.FromJson<TestCaseData>(json);
                testCaseTables = testCaseData.MakeDic();
                break;
        }
    }

    // 각 테이블의 데이터를 불러오는 함수

    public IDTable IDLoad(int id)
    {
        IDTable result = IDTables[id];
        return result;
    }

    public ResourceTable ResourceLoad(int id)
    {
        ResourceTable result = ResourceTables[id];
        return result;
    }

    public TextTable TextLoad(int id)
    {
        TextTable result = TextTables[id];
        return result;
    }

    public TextBundleTable TextBundleLoad(int id)
    {
        TextBundleTable result = TextBundleTables[id];
        return result;
    }

    public UITable UILoad(int id)
    {
        UITable result = UITables[id];
        return result;
    }

    public PuzzleTable PuzzleLoad(int id)
    {
        PuzzleTable result = PuzzleTables[id];
        return result;
    }

    public ItemTable ItemLoad(int id)
    {
        ItemTable result = ItemTables[id];
        return result;
    }

    public InteractionObject InteractionObjectLoad(int id)
    {
        InteractionObject result = InteractionObjects[id];
        return result;
    }

    public StageTable StageLoad(int id)
    {
        StageTable result = StageTables[id];
        return result;
    }

    public ChapterTable ChapterLoad(int id)
    {
        ChapterTable result = ChapterTables[id];
        return result;
    }

    public UserInfoTable UserInfoLoad(int id)
    {
        UserInfoTable result = UserInfoTables[id];
        return result;
    }

    public UserItemTable UserItemLoad(int id)
    {
        UserItemTable result = UserItemTables[id];
        return result;
    }

    public InventoryTable InventoryLoad(int id)
    {
        InventoryTable result = InventoryTables[id];
        return result;
    }

    public StageProcessTable StageProcessLoad(int id)
    {
        StageProcessTable result = StageProcessTables[id];
        return result;
    }

    public UserLogTable UserLogLoad(int id)
    {
        UserLogTable result = UserLogTables[id];
        return result;
    }

    public TestCase TestCaseLoad(int id)
    {
        TestCase result = TestCaseTables[id];
        return result;
    }

    public bool ExistTable(string fileName)
    {
        return File.Exists($"{path}/Json/{fileName}.json");
    }

    #endregion
}
