// Copyright 2013-2022 AFI, INC. All rights reserved.

using InGameScene.UI;

namespace InGameScene
{

    //===========================================================
    //UI를 관리하는 클래스, Manager간 접근을 위한 인자값으로만 사용된다.
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