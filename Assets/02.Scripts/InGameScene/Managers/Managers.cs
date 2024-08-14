// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Reflection;
using BackEnd;
using Unity.VisualScripting;
using UnityEngine;
using InGameScene.UI;
using UnityEngine.SceneManagement;

namespace InGameScene
{
    //===========================================================
    // �ΰ��Ӿ��� ��� Manager Ŭ������ �����ϴ� ��ǥ Ŭ����
    //===========================================================
    public class Managers : MonoBehaviour
    {


        [Header("UI")]
        [SerializeField] private InGameUI_User _userUI;
        [SerializeField] private InGameUI_Enemy _enemyUI;
        [SerializeField] private InGameUI_Stage _stageUI;
        [SerializeField] private InGameUI_LeftUI _leftUI;
        [SerializeField] private InGameUI_ItemInventory _itemUI;

        public static readonly ItemManager Item = new();
        public static readonly ProcessManager Process = new();
        public static readonly GameManager Game = new();
        private readonly UIManager _uiManager = new();

        public static BuffManager Buff;

        private Player _player;
        //private CloudManager _cloudManager;
        private GameObject _bulletPrefab;

        private void Awake()
        {
            // ������ ���� inGameScene�� ��쿡�� LoadScene���� ���� �����͸� �ε�
            if (Backend.IsInitialized == false)
            {
                SceneManager.LoadScene("LoginScene");
            }
        }

        // �� �Ŵ����� �ʱ�ȭ
        void Start()
        {
            try
            {
                //_cloudManager = FindObjectOfType<CloudManager>();
                _player = FindObjectOfType<Player>();

                _bulletPrefab = Resources.Load<GameObject>("Prefabs/InGameScene/BulletObject");
                
                //_cloudManager.Init();
                _player.Init(_bulletPrefab);

                //Item.Init(_bulletPrefab);

                _uiManager.Init(_userUI, _leftUI, _enemyUI, _stageUI, _itemUI);
                Game.Init(_player, _uiManager);

                Process.Init(_player, _uiManager);

                var buffObject = new GameObject();
                buffObject.transform.SetParent(this.transform);
                Buff = buffObject.GetOrAddComponent<BuffManager>();
                Buff.Init();

                //���̵���
                StaticManager.UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToTransparent, Process.StartGame);
                // �ڷ�ƾ�� ���� ���� ������ ������Ʈ ����
                StaticManager.Backend.StartUpdate();

                // ������ ������ 1�� �̻��̶�� ���� ������ ǥ��
                if (StaticManager.Backend.Post.Dictionary.Count > 0)
                {
                    _uiManager.LeftUI.GetUI<InGameUI_Post>().SetPostIconAlert(true);
                }
                else
                {
                    _uiManager.LeftUI.GetUI<InGameUI_Post>().SetPostIconAlert(false);
                }
            }
            catch (Exception e)
            {
                StaticManager.UI.AlertUI.OpenErrorUI(GetType().Name, MethodBase.GetCurrentMethod()?.ToString(), e.ToString());
            }
        }


    }
}