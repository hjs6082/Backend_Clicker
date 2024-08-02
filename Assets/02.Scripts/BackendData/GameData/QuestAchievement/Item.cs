// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace BackendData.GameData.QuestAchievement
{
    //===============================================================
    // QuestAchievement ���̺��� Dictionary�� ����� �� ����Ʈ ���� Ŭ����
    //===============================================================
    public class Item
    {
        public bool IsAchieve { get; private set; } // �޼� ����
        public int QuestId { get; private set; } // ����Ʈ ���̵�(��Ʈ�� ����)

        public Item(int questId, bool isAchieve)
        {
            IsAchieve = isAchieve;
            QuestId = questId;
        }

        // �ش� ����Ʈ �޼����� ����
        public void SetAchieve()
        {
            IsAchieve = true;
        }
    }
}