//Copyright 2013-2022 AFI, INC. All right reserved

using UnityEngine;
using UnityEngine.SceneManagement;

public class StaticManager : MonoBehaviour
{
    public static StaticManager Instance { get; private set; }

    public static BackendManager Backend { get; private set; }
    public static UIManager UI { get; private set; }


    // ��� ������ ���Ǵ� ��ɵ��� ��Ƴ��� Ŭ����.
    // �����޴����� ���� �ڿ� �����ϴ��� Ȯ�� �� �����Ѵ�.
    void Awake()
    {
        Init();
    }

    void Init()
    {
        if (Instance != null)
        {
            Destroy(gameObject);
            return;
        }

        Instance = this;
        DontDestroyOnLoad(this.gameObject);

        Backend = GetComponentInChildren<BackendManager>();
        UI = GetComponentInChildren<UIManager>();

        UI.Init();
        Backend.Init();


    }

    // �� ���� �� ���̵�ƿ��Ǹ鼭 �� ��ȯ
    public void ChangeScene(string sceneName)
    {
        UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToBlack, () => SceneManager.LoadScene(sceneName));
    }
}