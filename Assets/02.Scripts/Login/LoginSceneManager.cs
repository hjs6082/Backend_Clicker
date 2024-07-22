// Copyright 2013-2022 AFI, INC. All rights reserved.

using BackEnd;
using UnityEngine;
using UnityEngine.UI;

public class LoginSceneManager : MonoBehaviour
{
    private static LoginSceneManager _instance;

    public static LoginSceneManager Instance
    {
        get
        {
            return _instance;
        }
    }

    [SerializeField] private Canvas _loginUICanvas;
    [SerializeField] private GameObject _loginButtonGroup;
    [SerializeField] private GameObject _touchStartButton;

    public Canvas GetLoginUICanvas()
    {
        return _loginUICanvas;
    }

    void Awake()
    {
        if (_instance == null)
        {
            _instance = this;
        }

        // StaticManager�� ���� ��� ���� ����
        if (FindObjectOfType(typeof(StaticManager)) == null)
        {
            var obj = Resources.Load<GameObject>("Prefabs/StaticManager");
            Instantiate(obj);
        }
    }

    void Start()
    {
        _loginButtonGroup.SetActive(false); // �α��� ��ư ��Ȱ��ȭ
        SetTouchStartButton(); // ��ư ��Ȱ��ȭ
    }

    // ��ġ�Ͽ� ���� ��ư Ȱ��ȭ
    // ��ġ ��, �ش� UI�� ������� �ڵ� �α��� �Լ��� ȣ���Ѵ�.
    private void SetTouchStartButton()
    {
        _touchStartButton.GetComponent<Button>().onClick.AddListener(() => {
            Destroy(_touchStartButton);
            _touchStartButton = null;
            LoginWithBackendToken();
        });
    }

    // �α��� ��ư ������ �����ϱ�
    private void SetButton()
    {
        if (_loginButtonGroup.activeSelf)
        {
            return;
        }
        _loginButtonGroup.SetActive(true);

        Button[] buttons = _loginButtonGroup.GetComponentsInChildren<Button>();

        for (int i = 0; i < buttons.Length; i++)
        {
            buttons[i].onClick.RemoveAllListeners();
        }

        buttons[0].onClick.AddListener(FederationLogin);
        buttons[1].onClick.AddListener(CustomLogin);
        buttons[2].onClick.AddListener(GuestLogin);

        // �䵥���̼� �α��� ��� �̱���
        buttons[0].gameObject.SetActive(false);

    }

    // �ڵ��α��� �Լ� ȣ��. ���� ó���� AuthorizeProcess ����
    private void LoginWithBackendToken()
    {
        SendQueue.Enqueue(Backend.BMember.LoginWithTheBackendToken, callback => {
            Debug.Log($"Backend.BMember.LoginWithTheBackendToken : {callback}");

            if (callback.IsSuccess())
            {
                // �г����� ���� ���
                if (string.IsNullOrEmpty(Backend.UserNickName))
                {
                    StaticManager.UI.OpenUI<LoginUI_Nickname>("Prefabs/LoginScene/UI", GetLoginUICanvas().transform);
                }
                else
                {
                    GoNextScene();
                }
            }
            else
            {
                SetButton();
            }
        });
    }

    // �Խ�Ʈ�α��� �Լ� ȣ��. ���� ó���� AuthorizeProcess ����
    private void GuestLogin()
    {
        SendQueue.Enqueue(Backend.BMember.GuestLogin, AuthorizeProcess);
    }

    // �α��� UI ����
    private void CustomLogin()
    {
        StaticManager.UI.OpenUI<LoginUI_CustomLogin>("Prefabs/LoginScene/UI", GetLoginUICanvas().transform);
    }
    private void FederationLogin()
    {

    }

    // �α��� �Լ� �� ó�� �Լ�
    private void AuthorizeProcess(BackendReturnObject callback)
    {
        Debug.Log($"Backend.BMember.AuthorizeProcess : {callback}");

        // ������ �߻��� ��� ����
        // �α��� ��ư Ȱ��ȭ
        if (callback.IsSuccess() == false)
        {
            SetButton();
            return;
        }

        // ���� ������ ��쿡�� statusCode�� 201, ���� �α����� ��쿡�� 200�� ���ϵȴ�.
        if (callback.GetStatusCode() == "201")
        {
            GetPolicy();
        }
        else
        {
            GoNextScene();
        }
    }

    // ����������ȣ��å UI ����
    private void GetPolicy()
    {
        StaticManager.UI.OpenUI<LoginUI_Policy>("Prefabs/LoginScene/UI", GetLoginUICanvas().transform);
    }

    public void GoNextScene()
    {
        StaticManager.Instance.ChangeScene("LoadingScene");

    }
}