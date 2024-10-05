// Copyright 2013-2022 AFI, INC. All rights reserved.

using System.Collections.Generic;
using UnityEngine;
using InGameScene.UI;

namespace InGameScene
{
    //===========================================================
    // �� ����, �������� �̵����� ���� �帧�� �����ϴ� ���μ���.
    //===========================================================
    public class ProcessManager
    {

        private Player _player;
        private UIManager _uiManager;
        private GameObject _enemyPrefab;
        private GameObject _dropItemPrefab;

        //private int _currentStageNum = 0;

        private int _enemyDeafultCount = 0;

        private EnemyObject _enemyItem;

        public void Init(Player player, UIManager uiManager)
        {
            _player = player;
            _uiManager = uiManager;
            //_currentStageNum = 0;
            //SetStage();

            _enemyPrefab = Resources.Load<GameObject>("Prefabs/InGameScene/EnemyObject");
            _dropItemPrefab = Resources.Load<GameObject>("Prefabs/InGameScene/DropItemObject");

        }

        public void StartGame()
        {
            StartNextStage();
        }

        public EnemyObject GetEnemy()
        {
            return _enemyItem;
        }

/*        // ���� �� ���� ������ �������� ���� ������ ���Ͽ� ���� �������� ������ 
        private void SetStage()
        {
            for (int i = 0; i < StaticManager.Backend.Chart.Stage.List.Count; i++)
            {
                if (StaticManager.Backend.GameData.UserData.Level > StaticManager.Backend.Chart.Stage.List[i].Level)
                {
                    _currentStageNum = i;
                }
                else
                {
                    break;
                }
            }
        }*/

        //���� óġ�ϰ� �� ���� �ٸ� ���� �����ϴ� �Լ�
        private void RespawnNextEnemy()
        {
            if(StaticManager.Backend.GameData.UserData.Stage - 1 > StaticManager.Backend.Chart.Stage.List.Count)
            {
                if (_enemyDeafultCount >= 3)
                {
                    _enemyDeafultCount = 0;
                    RespawnBossEnemy();
                }
                else
                {
                    RespawnRandomEnemy();
                }
                return;
            }

            if(_enemyDeafultCount >= 3)
            {
                _enemyDeafultCount = 0;
                RespawnBossEnemy();
            }
            else
            {
                RespawnRandomEnemy();
            }

/*            // ������ ������������ �Ѵ� �ϵ��ھ� ������ ������ ���, ������ �������� ������ ��� ����
            if (_currentStageNum > StaticManager.Backend.Chart.Stage.List.Count)
            {
                RespawnRandomEnemy();
                return;
            }

            // ������ ������������ �Ѵ� �ϵ��ھ� ������ ������ ���, ������ �������� ������ ��� ����
            if (StaticManager.Backend.GameData.UserData.Level > StaticManager.Backend.Chart.Stage.List[_currentStageNum].Level)
            {
                _currentStageNum++;
                GoNextStage();
            }
            else
            {
                RespawnRandomEnemy();
            }*/
        }

        //���� ���������� EnemyInfoList�� �ִ� ������ ���� �ҷ��� �����ϴ� �Լ�
        private void RespawnRandomEnemy()
        {
            int randomMin = 0;
            int randomMax = StaticManager.Backend.Chart.Stage.List[StaticManager.Backend.GameData.UserData.Stage - 1].EnemyInfoList.Count;
            int randomValue = Random.Range(randomMin, randomMax);

            BackendData.Chart.Stage.Item.EnemyInfo enemyInfo = StaticManager.Backend.Chart.Stage.List[StaticManager.Backend.GameData.UserData.Stage - 1].EnemyInfoList[randomValue];
            CreateEnemy(enemyInfo);
        }

        private void RespawnBossEnemy()
        {
            foreach (var item in StaticManager.Backend.Chart.Boss.Dictionary.Values)
            {
/*                Debug.Log(_currentStageNum + "�Դϴ�");
                Debug.Log(item.BossName);*/
                if(item.BossID == StaticManager.Backend.GameData.UserData.Stage)
                {
                    CreateBoss(item);
                    _uiManager.EnemyUI.OnBossAnimation();
                }
            }
        }

        // ���� ���������� ���� ������ �����ִ� �Լ� 1.(�÷��̾ ������ ȭ�� ������ ���� ���̵� �ƿ��Ǳ����)
        private void GoNextStage()
        {
            _player.SetMove(Player.MoveState.MoveToNextStage,
                () => {
                    StaticManager.UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToBlack,
                        () => StaticManager.UI.FadeUI.FadeStart(FadeUI.FadeType.ChangeToTransparent,
                            StartNextStage));
                });
        }

        // ���� ���������� ���� ������ �����ִ� �Լ� 2.(�÷��̾ ���� ȭ�鿡�� ���� ��ġ���� ����)
        private void StartNextStage()
        {
            _player.SetMove(Player.MoveState.MoveToAttack);
            _uiManager.StageUI.ShowTitleStage(StaticManager.Backend.Chart.Stage.List[StaticManager.Backend.GameData.UserData.Stage - 1].StageName);
            //RespawnRandomEnemy();
            RespawnNextEnemy();
        }

        // ���� �����ϴ� �Լ�
        // ������ ���� ȭ�� �ۿ��� enemy��ü�� ���� �Լ��� ���� ȭ�� ������ �̵��Ѵ�.
        private void CreateEnemy(BackendData.Chart.Stage.Item.EnemyInfo stageEnemyInfo)
        {
            Vector3 enemyRespawnPosition = new Vector3(11.39f, -0.91f, 0);
            Vector3 enemyStayPosition = new Vector3(5.078f, -0.91f, 0);

            // �� ��Ʈ�������� ������ �ҷ�����
            BackendData.Chart.Enemy.Item enemyInfo =
                StaticManager.Backend.Chart.Enemy.Dictionary[stageEnemyInfo.EnemyID];

            // �� ��ȭ����
            float multiStat = stageEnemyInfo.MultiStat;

            // ������ �ε� (��Ʈ�� Image �̸��� ������� �������� �ε�)
            string prefabPath = $"Prefabs/Monster/{enemyInfo.Image}";
            GameObject enemyPrefab = Resources.Load<GameObject>(prefabPath);

            if (enemyPrefab == null)
            {
                Debug.LogError($"Prefab not found at path: {prefabPath}");
                return;
            }

            // �� �������� �ν��Ͻ�ȭ
            GameObject enemyObject = GameObject.Instantiate(enemyPrefab);
            enemyObject.transform.localPosition = enemyRespawnPosition;
            //enemyObject.transform.localScale = new Vector3(1, 1, 1);

            EnemyObject enemy = enemyObject.GetComponent<EnemyObject>();
            enemy.Init(enemyInfo, multiStat, enemyStayPosition);
            _enemyItem = enemy;
            _player.SetNewEnemy(enemy);
            _uiManager.EnemyUI.SetEnemyInfo(enemy.Name, enemy.MaxHp);
            _uiManager.EnemyUI.ShowUI(true);
            _uiManager.EnemyUI.ShowBossUI(false);
        }

        private void CreateBoss(BackendData.Chart.Boss.Item stageBossInfo)
        {
            //���� ���� ���������� ���������ʴ´ٸ� ������ ������ ��ȯ�Ѵ�.
            if(StaticManager.Backend.GameData.UserData.Stage >= StaticManager.Backend.Chart.Stage.List.Count)
            {
                int randomMin = 0;
                int randomMax = StaticManager.Backend.Chart.Boss.Dictionary.Count;
                int randomValue = Random.Range(randomMin, randomMax);

                foreach (var item in StaticManager.Backend.Chart.Boss.Dictionary.Values)
                {
                    if (item.BossID == randomValue)
                        stageBossInfo = item;
                }
            }

            Vector3 bossRespawnPosition = new Vector3(11.39f, -0.91f, 0);
            Vector3 bossStayPosition = new Vector3(5.078f, -0.91f, 0);

            // ������ �ε� (��Ʈ�� Image �̸��� ������� �������� �ε�)
            string prefabPath = $"Prefabs/Monster/Boss/{stageBossInfo.Image}";
            GameObject enemyPrefab = Resources.Load<GameObject>(prefabPath);

            if (enemyPrefab == null)
            {
                Debug.LogError($"Prefab not found at path: {prefabPath}");
                return;
            }

            // �� �������� �ν��Ͻ�ȭ
            GameObject enemyObject = GameObject.Instantiate(enemyPrefab);
            enemyObject.transform.localPosition = bossRespawnPosition;
            enemyObject.transform.localScale = new Vector3(1, 1, 1);

            EnemyObject enemy = enemyObject.GetComponent<EnemyObject>();
            enemy.InitBoss(stageBossInfo, bossStayPosition);
            _enemyItem = enemy;
            _player.SetNewEnemy(enemy);
            _uiManager.EnemyUI.SetEnemyInfo(enemy.Name, enemy.MaxHp);
            _uiManager.EnemyUI.ShowUI(true);
            _uiManager.EnemyUI.ShowBossUI(true);
        }


        // ���� �� ���¸� ������Ʈ ���ִ� �Լ�
        // �� ��ü���� �ش� Ŭ������ ȣ���Ѵ�.
        // ���� ���������� ui �� �����͸� ������Ʈ ���ִ� ����� ����ϰ� �ִ�.
        public void UpdateEnemyStatus(EnemyObject enemyItem)
        {
            switch (enemyItem.CurrentEnemyState)
            {
                case EnemyObject.EnemyState.Init:
                    //_player.SetNewEnemy(enemyItem);
/*                    _uiManager.EnemyUI.SetEnemyInfo(enemyItem.Name, enemyItem.MaxHp);
                    _uiManager.EnemyUI.ShowUI(true);
                    _uiManager.EnemyUI.ShowBossUI(enemyItem.isBoss);*/
                    break;
                case EnemyObject.EnemyState.Normal:

                    _uiManager.EnemyUI.SetCurrentHp(enemyItem.Hp);
                    break;
                case EnemyObject.EnemyState.Dead:
                    
                    //TODO : ����üũ �ʿ�

                    float moneyToAdd = enemyItem.Money;

                    // Money ������ Ȱ��ȭ�Ǿ� �ִ��� Ȯ��
                    if (Managers.Buff.IsBuffActive(Buff.BuffStatType.Gold))
                    {
                        moneyToAdd *= 2; // ���� �� ��� ����
                    }

                    Managers.Game.UpdateUserData(moneyToAdd, enemyItem.Exp);

                    StaticManager.Backend.GameData.UserData.CountDefeatEnemy();
                    _uiManager.LeftUI.GetUI<InGameUI_Quest>().UpdateUI(BackendData.Chart.Quest.QuestType.DefeatEnemy);
                    _player.SetNewEnemy(null);
                    if (enemyItem.isBoss)
                    {
                        if (StaticManager.Backend.GameData.UserData.Stage >= StaticManager.Backend.Chart.Stage.List.Count)
                        {
                            RespawnNextEnemy();
                        }
                        else
                        {
                            StaticManager.Backend.GameData.UserData.UpdateStage(1);
                            GoNextStage();
                            _uiManager.EnemyUI.ShowUI(false);
                        }
                    }
                    else
                    {
                        _enemyDeafultCount++;
                        RespawnNextEnemy();
                    }
                    Debug.Log(StaticManager.Backend.GameData.UserData.Stage + "�Դϴ�");
                    Debug.Log(StaticManager.Backend.Chart.Stage.List.Count + "�Դϴ�2");
                    Debug.Log(_enemyDeafultCount + "�Դϴ�3");
                    //_uiManager.EnemyUI.ResetUI();
                    //_uiManager.EnemyUI.ShowBossUI(false);
                    //_uiManager.EnemyUI.ShowUI(false);
                    break;
            }
        }

        // �� ����� �������� ����ϴ� �Լ�.
        // �� ��ü���� Ȯ���� �����Ͽ� ����Ʈ����.
        public void DropItem(Vector3 enemyPosition, int itemID)
        {
            var dropItem = Object.Instantiate(_dropItemPrefab);
            dropItem.transform.position = enemyPosition;
            dropItem.transform.localScale = new Vector3(0.3f, 0.3f, 0.3f);

            dropItem.GetComponent<SpriteRenderer>().sprite =
                StaticManager.Backend.Chart.Item.Dictionary[itemID].ImageSprite;

            float randomX = Random.Range(50, 100);
            float randomY = Random.Range(100, 150);
            dropItem.GetComponent<Rigidbody2D>().AddForce(new Vector2(randomX % 2 == 0 ? randomX : -randomX, randomY), ForceMode2D.Force);
            Object.Destroy(dropItem, 4f);
        }
        
        public void AttackEffect(float damage)
        {
            _uiManager.EnemyUI.OnAttackAnimation(damage);
        }
    }
}