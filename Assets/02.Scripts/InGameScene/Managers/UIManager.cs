// Copyright 2013-2022 AFI, INC. All rights reserved.

using InGameScene.UI;

namespace InGameScene
{

    //===========================================================
    //UI�� �����ϴ� Ŭ����, Manager�� ������ ���� ���ڰ����θ� ���ȴ�.
    //===========================================================
    public class UIManager
    {
        public InGameUI_User UserUI;
        public InGameUI_LeftUI LeftUI;
        public InGameUI_Enemy EnemyUI;
        public InGameUI_Stage StageUI;
        public InGameUI_ItemInventory ItemUI;

        public void Init(InGameUI_User userUI, InGameUI_LeftUI leftUI, InGameUI_Enemy enemyUI,
            InGameUI_Stage stageUI, InGameUI_ItemInventory itemUI)
        {
            UserUI = userUI;
            LeftUI = leftUI;
            EnemyUI = enemyUI;
            StageUI = stageUI;
            ItemUI = itemUI;

            ItemUI.Init();
            UserUI.Init();
            LeftUI.Init();
        }
    }
}