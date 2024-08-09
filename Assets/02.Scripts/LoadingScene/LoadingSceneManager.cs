// Copyright 2013-2022 AFI, INC. All rights reserved.


using System;
using System.Collections.Generic;
using System.Reflection;
using BackEnd;
using LitJson;
using TMPro;
using UnityEngine;
using UnityEngine.SceneManagement;
using UnityEngine.UI;

public class LoadingSceneManager : MonoBehaviour
{

    [SerializeField] private TMP_Text loadingText;
    [SerializeField] private Slider loadingSlider;

    private int _maxLoadingCount; // �� �ڳ� �Լ��� ȣ���� ����

    private int _currentLoadingCount; // ���� �ڳ� �Լ��� ȣ���� ����

    private delegate void BackendLoadStep();
    private readonly Queue<BackendLoadStep> _initializeStep = new Queue<BackendLoadStep>();

    void Awake()
    {
        if (Backend.IsInitialized == false)
        {
            SceneManager.LoadScene("LoginScene");
        }
    }

    void Start()
    {
        //Queue�� �Լ� Insert
        Init();

        // �ڳ� ������ �ʱ�ȭ
        StaticManager.Backend.InitInGameData();

        //Queue�� ����� �Լ� ���������� ����
        NextStep(true, string.Empty, string.Empty, string.Empty);
    }

    void Init()
    {
        _initializeStep.Clear();
        // Ʈ��������� �ҷ��� ��, �Ⱥҷ��� ��� ���� Get �Լ��� �ҷ����� �Լ� *�߿�*
        _initializeStep.Enqueue(() => { ShowDataName("Ʈ����� �õ� �Լ�"); TransactionRead(NextStep); });

        // ��Ʈ���� �ҷ����� �Լ� Insert
        _initializeStep.Enqueue(() => { ShowDataName("��� ��Ʈ ����"); StaticManager.Backend.Chart.ChartInfo.BackendLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("���� ����"); StaticManager.Backend.Chart.Weapon.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("�� ����"); StaticManager.Backend.Chart.Enemy.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("�������� ����"); StaticManager.Backend.Chart.Stage.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("������ ����"); StaticManager.Backend.Chart.Item.BackendChartDataLoad(NextStep); });
        //_initializeStep.Enqueue(() => { ShowDataName("���� ����"); StaticManager.Backend.Chart.Shop.BackendChartDataLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("����Ʈ ����"); StaticManager.Backend.Chart.Quest.BackendChartDataLoad(NextStep); });

        // ��ŷ ���� �ҷ����� �Լ� Insert
        _initializeStep.Enqueue(() => { ShowDataName("��ŷ ���� �ҷ�����"); StaticManager.Backend.Rank.BackendLoad(NextStep); });
        // ���� ���� �ҷ����� �Լ� Insert
        _initializeStep.Enqueue(() => { ShowDataName("������ ���� ���� �ҷ�����"); StaticManager.Backend.Post.BackendLoad(NextStep); });
        _initializeStep.Enqueue(() => { ShowDataName("��ŷ ���� ���� �ҷ�����"); StaticManager.Backend.Post.BackendLoadForRank(NextStep); });

        //���� ������ �Ѿ�� �Լ� Insert


        //������ �� ����
        _maxLoadingCount = _initializeStep.Count;
        loadingSlider.maxValue = _maxLoadingCount;

        _currentLoadingCount = 0;
        loadingSlider.value = _currentLoadingCount;

        // �ε������� Ȱ��ȭ
        StaticManager.UI.SetLoadingIcon(true);
    }

    private void ShowDataName(string text)
    {
        string info = $"{text} �ҷ����� ��...({_currentLoadingCount}/{_maxLoadingCount})";
        loadingText.text = info;
        Debug.Log(info);
    }

    // �� �ڳ� �Լ��� ȣ���ϴ� BackendGameDataLoad���� ������ ����� ó���ϴ� �Լ�
    // �����ϸ� ���� �������� �̵�, �����ϸ� ���� UI�� ����.
    private void NextStep(bool isSuccess, string className, string funcName, string errorInfo)
    {
        if (isSuccess)
        {
            _currentLoadingCount++;
            loadingSlider.value = _currentLoadingCount;

            if (_initializeStep.Count > 0)
            {
                _initializeStep.Dequeue().Invoke();
            }
            else
            {
                InGameStart();
            }
        }
        else
        {
            StaticManager.UI.AlertUI.OpenErrorUI(className, funcName, errorInfo);
        }
    }

    // Ʈ����� �б� ȣ�� �Լ�
    private void TransactionRead(BackendData.Base.Normal.AfterBackendLoadFunc func)
    {
        bool isSuccess = false;
        string className = GetType().Name;
        string functionName = MethodBase.GetCurrentMethod()?.Name;
        string errorInfo = string.Empty;

        //Ʈ����� ����Ʈ ����
        List<TransactionValue> transactionList = new List<TransactionValue>();

        // ���� ���̺� �����͸�ŭ Ʈ����� �ҷ�����
        foreach (var gameData in StaticManager.Backend.GameData.GameDataList)
        {
            transactionList.Add(gameData.Value.GetTransactionGetValue());
        }

        // [�ڳ�] Ʈ����� �б� �Լ�
        SendQueue.Enqueue(Backend.GameData.TransactionReadV2, transactionList, callback => {
            try
            {
                Debug.Log($"Backend.GameData.TransactionReadV2 : {callback}");

                // �����͸� ��� �ҷ����� ���
                if (callback.IsSuccess())
                {
                    JsonData gameDataJson = callback.GetFlattenJSON()["Responses"];

                    int index = 0;

                    foreach (var gameData in StaticManager.Backend.GameData.GameDataList)
                    {

                        _initializeStep.Enqueue(() => {
                            ShowDataName(gameData.Key);
                            // �ҷ��� �����͸� ���ÿ��� �Ľ�
                            gameData.Value.BackendGameDataLoadByTransaction(gameDataJson[index++], NextStep);
                        });
                        _maxLoadingCount++;

                    }
                    // �ִ� �۾� ���� ����
                    loadingSlider.maxValue = _maxLoadingCount;
                    isSuccess = true;
                }
                else
                {
                    // Ʈ��������� �����͸� ã�� ���Ͽ� ������ �߻��Ѵٸ� ������ GetMyData�� ȣ��
                    foreach (var gameData in StaticManager.Backend.GameData.GameDataList)
                    {
                        _initializeStep.Enqueue(() => {
                            ShowDataName(gameData.Key);
                            // GetMyData ȣ��
                            gameData.Value.BackendGameDataLoad(NextStep);
                        });
                        _maxLoadingCount++;
                    }
                    // �ִ� �۾� ���� ����
                    loadingSlider.maxValue = _maxLoadingCount;
                    isSuccess = true;
                }
            }
            catch (Exception e)
            {
                errorInfo = e.ToString();
            }
            finally
            {
                func.Invoke(isSuccess, className, functionName, errorInfo);
            }
        });
    }

    // �ΰ��Ӿ����� �̵����� �Լ�
    private void InGameStart()
    {
        StaticManager.UI.SetLoadingIcon(false);
        loadingText.text = "���� �����ϴ� ��";
        _initializeStep.Clear();
        StaticManager.Instance.ChangeScene("InGameScene");

    }
}