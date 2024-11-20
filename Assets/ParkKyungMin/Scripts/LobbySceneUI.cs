using BackEnd;
using System.Collections;
using System.Collections.Generic;
using System.Drawing;
using System.Runtime.InteropServices;
using TMPro;
using UnityEngine;
using UnityEngine.UI;

public class LobbySceneUI : BaseUI
{
    enum GameObjects
    {
        TouchUI,
        GameStartSceneUI
    }
    enum Texts
    {
        GuestText,
        GoogleText,
        TouchText,
        StartText,
        StoryText,
        QuitGameText
    }

    [SerializeField] GameObject loginUI;    // Login창
    [SerializeField] GameObject touchUI;    // 중간터치창
    [SerializeField] GameObject startUI;    // 게임시작창
    [SerializeField] UI_Notice notice;      // 구글로그인 안된다는 공지
    [SerializeField] PopUpUI waitLogin;     // 로그인 대기창

    [SerializeField] Button googleLoginID;  // 구글 로그인 버튼
    [SerializeField] Button guestLoginID;   // 게스트 로그인 버튼

    [SerializeField] TMP_Text ID1;
    [SerializeField] TMP_Text ID2;


    [SerializeField] Button deleteGuestID;      // 게스트 아이디 로컬데이터 삭제 버튼
    [SerializeField] TMP_Text deleteGuestIDText;    // 게스트 아이디 로컬데이터 삭제 버튼 텍스트

    bool isLoading; //정민 추가 ---> 버튼 연속 클릭으로 인한 로드 막기.

    private UI_Story storyUI; //새롬 추가 --> 스토리 UI 프리팹
    private UI_QuitGame quitGameUI; //새롬 추가 --> 게임종료 UI 프리팹

    private void Start()
    {
        LocalUpdate();
        GetUI(GameObjects.TouchUI.ToString()).SetActive(false);
        GetUI(GameObjects.GameStartSceneUI.ToString()).SetActive(false);
        if (Manager.Game.LanguageSet == Define.LanguageSet.EN)
        {
            deleteGuestIDText.text = "Delete Guest ID";
        }
        else
        {
            deleteGuestIDText.text = "게스트 ID 삭제";
        }
    }

    #region buttonSetting
    public void ButtonSet()
    {
        googleLoginID.onClick.AddListener(GoogleLogin);
        guestLoginID.onClick.AddListener(GuestLogin);
        deleteGuestID.onClick.AddListener(DeleteGuestID);
    }
    
    public void GoogleLogin()
    {
        Manager.Backend.GoogleLogin();
    }

    public void GuestLogin()
    {
        Manager.Backend.GuestLogin();
    }

    public void DeleteGuestID()
    {
        Manager.Backend.DeleteGuestID();
    }
    #endregion

    private void LateUpdate()
    {
        if (Backend.BMember.GetGuestID() == null || Backend.BMember.GetGuestID() == "")
        {
            return;
        }
        else
        {
            SetID();
        }

    }

    public override void LocalUpdate()
    {
        GetUI<TMP_Text>(Texts.GuestText.ToString()).text = LanguageSetting.GetLocaleText(102215);
        GetUI<TMP_Text>(Texts.GoogleText.ToString()).text = LanguageSetting.GetLocaleText(102214);
        GetUI<TMP_Text>(Texts.TouchText.ToString()).text = LanguageSetting.GetLocaleText(102216);
        GetUI<TMP_Text>(Texts.StartText.ToString()).text = LanguageSetting.GetLocaleText(102217);
        GetUI<TMP_Text>(Texts.StoryText.ToString()).text = LanguageSetting.GetLocaleText(102218);
        GetUI<TMP_Text>(Texts.QuitGameText.ToString()).text = LanguageSetting.GetLocaleText(102517);
    }

    public void OnLogin()
    {
        // Login창 비활성화, 게임시작창 활성화
        loginUI.SetActive(false);
        touchUI.SetActive(true);
        Manager.UI.ShowPopUpUI(waitLogin);
    }

    // 페더레이션 로그인이 현재 구현되어 있지 않으므로 임시로 띄울 공지를 위한 함수
    public void OnGoogleLogin()
    {
        UI_Notice tmp = Manager.UI.ShowPopUpUI(notice);

        if (Manager.Game.LanguageSet == Define.LanguageSet.EN)
        {
            tmp.NoticeText = "Notice";
            tmp.DescriptionText = "GoogleLogin is unlinked.";
        }
        else
        {
            tmp.NoticeText = "알림";
            tmp.DescriptionText = "구글로그인은 아직 연결 되지 않았습니다.";
        }
        tmp.Setting();
    }

    public void OnTouch()
    {
        touchUI.SetActive(false);
        startUI.SetActive(true);
    }

    public void ChapterSceneLoad()
    {
        Manager.Chapter.RefreshData();
        if (isLoading) { return; }
        // ChapterScene 불러오기
        Manager.Scene.LoadScene("ChapterScene");
        isLoading = true;  //정민 추가 --
    }

    public void SetID()
    {
        ID1.text = BackendManager.Instance.PrintGuestID();
        ID2.text = BackendManager.Instance.PrintGuestID();
    }

    // 서버의 유저데이터를 생성 혹은 불러오기
    public void SetUserInfo()
    {
        BackendManager.Instance.SetServerData();
    }

    //Method : 정민 추가. 재로드 시엔 isLoading 변수 초기화.
    private void OnDisable()
    {
        isLoading = false;
    }

    //권새롬 추가 --> 스토리 UI 띄우기
    public void OnClickedStory()
    {
        if (storyUI == null)
            storyUI = Manager.Resource.Load<UI_Story>("UI_Story");
        Manager.Chapter.RefreshData();
        Manager.UI.ShowPopUpUI(storyUI);

    }

    public void QuitGameLog()
    {
        if (quitGameUI == null)
            quitGameUI = Manager.Resource.Load<UI_QuitGame>("UI_QuitGame");
        Manager.UI.ShowPopUpUI(quitGameUI);
    }
}
