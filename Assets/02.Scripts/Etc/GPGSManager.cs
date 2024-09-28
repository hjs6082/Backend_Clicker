using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSManager : MonoBehaviour
{    // Start is called before the first frame update
    //추가 로그인 확인필요
    void Start()
    {
        PlayGamesPlatform.Activate();
        SecondLogin();
    }

    //TODO: 로그인 시스템 연동까진 확인, 추가 확인 필요
    public void SecondLogin()
    {
        if (PlayGamesPlatform.Instance.localUser.authenticated == false)
        {
            Social.localUser.Authenticate((bool sucess) =>
            {
                if (sucess)
                {

                }
                else
                {
                }
            });
        }
    }
}