// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace BackendData.GameData.WeaponEquip
{
    //===============================================================
    // WeaponEquip ���̺��� Dictionary�� ����� �� ���� ���� Ŭ����
    //===============================================================
    public class Item
    {
        public int Position { get; private set; } // ���� ���� ������ ��ġ
        public string MyWeaponId { get; private set; } // �� ���� ���� ���̵�

        public Item(int position, string myWeaponId)
        {
            Position = position;
            MyWeaponId = myWeaponId;
        }
    }
}