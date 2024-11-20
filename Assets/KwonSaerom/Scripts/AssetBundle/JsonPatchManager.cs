using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Text;
using TMPro;
using Unity.VisualScripting;
using UnityEngine;
using UnityEngine.Networking;
using UnityEngine.Purchasing.MiniJSON;
using static UnityEngine.Rendering.DebugUI.Table;

public class JsonPatchManager : Singleton<JsonPatchManager>
{
    enum VersionTableColumn // 버전 테이블 Column(열) 정보
    {
        fileName,
        version,
        downloadLink,
    }

#if UNITY_EDITOR
    private string path => Path.Combine(Application.dataPath, $"Resources/Data");
#else
    private string path => Path.Combine(Application.persistentDataPath, $"Resources/Data");
#endif
    private string serverVersionTableURL; // 서버 버전 테이블 접속 URL
    private string localVersionTablePath; // 로컬 버전 테이블 경로

    private string[,] serverVersionTable; // 서버 버전 테이블
    private string[,] localVersionTable; // 로컬 버전 테이블

    private List<string[]> patchListInfo = new List<string[]>(); // 패치가 필요한 데이터 정보
    private bool isLoad = false;

    [Header("Patch Progress UI")]
    [SerializeField] private Transform patchScreen;
    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private TMP_Text loadingSubText;

    
    public void PatchJson()
    {
        if (isLoad == false)
        {
            isLoad = true;
            serverVersionTableURL = "https://docs.google.com/spreadsheets/d/1NrSTAuSwUuLQM4Y6tpL9mh-LJjOM-Ef5/export?format=csv";
            localVersionTablePath = $"{path}/Json/JsonVersionTable.csv";
            StartCoroutine(LoadJsonProgress()); // 패치 시작
        }
    }

    IEnumerator LoadJsonProgress()
    {
        patchScreen.gameObject.SetActive(true);

        loadingText.text = "버전 정보를 받아오는 중";
        yield return CheckJsonVersion();

        loadingText.text = "패치를 진행하는 중";
        yield return SerialLoadJson(); // 순차 방식 패치

        patchScreen.gameObject.SetActive(false);
        Manager.Data.LoadAllTable();
    }


    /// <summary>
    /// 서버 버전 테이블 저장
    /// </summary>
    IEnumerator CheckJsonVersion()
    {
        loadingSubText.text = "인터넷에 접속합니다.";

        // # 서버 버전 테이블 load
        using (UnityWebRequest v = UnityWebRequest.Get(serverVersionTableURL))
        {
            loadingSubText.text = "서버 버전 테이블을 조회합니다.";

            yield return v.SendWebRequest();

            string[] rows = v.downloadHandler.text.Split('\n');
            serverVersionTable = new string[rows.Length, rows[0].Split(',').Length];
            for (int r = 0; r < serverVersionTable.GetLength(0); r++)
            {
                string[] cols = rows[r].Split(',');
                for (int c = 0; c < serverVersionTable.GetLength(1); c++)
                {
                    serverVersionTable[r, c] = cols[c];
                }
            }

            // # 2. 로컬 버전 테이블 load
            loadingSubText.text = "로컬 버전 테이블을 조회합니다.";

            bool isInit = false;
            StreamReader sr;
            try
            {
                sr = new StreamReader(localVersionTablePath);
            }
            catch(IOException ex)
            {
                if(Directory.Exists($"{path}/Json") == false)
                {
                    Directory.CreateDirectory($"{path}/Json");
                }

                byte[] data = v.downloadHandler.data;
                FileStream fs = new FileStream(localVersionTablePath, FileMode.Create);
                fs.Write(data, 0, data.Length);
                fs.Dispose();
                isInit = true;
                sr = new StreamReader(localVersionTablePath);
            }


            // TODO 함수로 하나하나 빼서, 최초일때는 바로 패치 넘어가게.
            rows = sr.ReadToEnd().Split('\n');
            localVersionTable = new string[rows.Length, rows[0].Split(',').Length];
            for (int r = 0; r < localVersionTable.GetLength(0); r++)
            {
                string[] cols = rows[r].Split(',');
                for (int c = 0; c < localVersionTable.GetLength(1); c++)
                {
                    localVersionTable[r, c] = cols[c];
                }
            }
            sr.Dispose();

            //// # 3. 비교 및 패치 진행할 번들 정보 추출
            //// # 3-1. 패치 정보 로컬 버전 테이블에 기록
            ////   >>>  그냥 서버 버전 테이블로 덮어쓰기?
            loadingSubText.text = "패치 정보를 비교하는 중입니다.";
            for (int r = 1; r < serverVersionTable.GetLength(0); r++)
            {
                // 버전 비교. 패치가 필요한 번들 발견
                if (isInit || string.Compare(serverVersionTable[r, (int)VersionTableColumn.version], localVersionTable[r, (int)VersionTableColumn.version]) != 0)
                {
                    string[] s = new string[3];
                    s[(int)VersionTableColumn.fileName] = serverVersionTable[r, (int)VersionTableColumn.fileName];
                    s[(int)VersionTableColumn.version] = serverVersionTable[r, (int)VersionTableColumn.version];
                    s[(int)VersionTableColumn.downloadLink] = serverVersionTable[r, (int)VersionTableColumn.downloadLink];

                    patchListInfo.Add(s);
                }
            }

            // 패치가 필요하다면. 서버 버전 테이블을 내려받아 로컬 버전테이블에 덮어쓰기
            if (patchListInfo.Count != 0)
            {
                byte[] data = v.downloadHandler.data;
                FileStream fs = new FileStream(localVersionTablePath, FileMode.Create);
                fs.Write(data, 0, data.Length);
                fs.Dispose();
            }
        }
    }


    IEnumerator SerialLoadJson()
    {
        for (int row = 0; row < patchListInfo.Count; row++)
        {
            loadingSubText.text = $"패치를 진행하고 있습니다. ({row+1}/{patchListInfo.Count})";

            string fileName = patchListInfo[row][(int)VersionTableColumn.fileName];
            string version = patchListInfo[row][(int)VersionTableColumn.version];
            string downloadURL = patchListInfo[row][(int)VersionTableColumn.downloadLink];

            UnityWebRequest v = UnityWebRequest.Get(downloadURL);

            // 통신 진행 정도 확인
            v.SendWebRequest();
            while (!v.isDone) // 통신 끝날 때까지 진행
            {
                StringBuilder comment = new StringBuilder();
                comment.Append($"데이터를 불러오고 있습니다. ({row+1}/{patchListInfo.Count})");
                int progress = (int)(v.downloadProgress * 100);
                if (progress != 0)
                    comment.Append($" {progress}%");
                loadingSubText.text = comment.ToString();
                yield return null;
            }

            foreach (Define.JsonFile json in System.Enum.GetValues(typeof(Define.JsonFile)))
            {
                if (string.Compare(fileName, json.ToString()) == 0)
                {
                    Manager.Data.SaveJson(json, v.downloadHandler.text);
                    break;
                }
            }
            Manager.Data.LoadTable(fileName);
        }

        loadingSubText.text = "곧 게임이 시작됩니다.";
    }


}
