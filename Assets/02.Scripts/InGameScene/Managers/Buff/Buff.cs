// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace InGameScene
{
    //===============================================================
    // ���� ���� Ŭ����
    //===============================================================
    public class Buff
    {
        public enum BuffStatType
        {
            Atk,
            Delay,
            Gold,
            Exp
        }

        public enum BuffAdditionType
        {
            Plus,
            Multi
        }

        public bool IsBuffing { get; private set; } // ���� ����
        public BuffAdditionType BuffAddition { get; private set; } // ������ ���ϱ�������, ����������
        public float Stat { get; private set; } // ������ �����ϴ� �ۼ�������
        public float Time { get; set; } // ���� �ð�

        public void UpdateBuff(float stat, float time, BuffAdditionType buffAdditionType)
        {
            IsBuffing = true;
            Stat = stat;
            Time = time;
            BuffAddition = buffAdditionType;
        }

        public Buff()
        {
            UpdateBuff(0, 0, 0);
            IsBuffing = false;
        }

        public void TurnOffBuff()
        {
            Time = 0;
            IsBuffing = false;
        }
    }
}
