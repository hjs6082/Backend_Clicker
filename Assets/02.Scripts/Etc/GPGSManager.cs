using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using GooglePlayGames;
using GooglePlayGames.BasicApi;

public class GPGSManager : MonoBehaviour
{    // Start is called before the first frame update
    //�߰� �α��� Ȯ���ʿ�
    void Start()
    {
        PlayGamesPlatform.Activate();
        SecondLogin();
    }

    //TODO: �α��� �ý��� �������� Ȯ��, �߰� Ȯ�� �ʿ�
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