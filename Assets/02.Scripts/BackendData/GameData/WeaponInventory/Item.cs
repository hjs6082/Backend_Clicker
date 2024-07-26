// Copyright 2013-2022 AFI, INC. All rights reserved.

namespace BackendData.GameData.WeaponInventory
{
    //===============================================================
    // WeaponInventory ���̺��� Dictionary�� ����� �� ���� ���� Ŭ����
    //===============================================================
    public class Item
    {

        // ���� ���� ����
        public class CurrentStat
        {
            public float Atk { get; private set; }
            public float Spd { get; private set; }
            public float Delay { get; private set; }
            public long UpgradePrice { get; private set; }

            public CurrentStat(int level, Chart.Weapon.Item item)
            {
                LevelUp(level, item);
            }

            // ��Ʈ���� ���� ���ݸ�ŭ level�� ���Ѵ�
            public void LevelUp(int level, Chart.Weapon.Item weaponChartChart)
            {
                Atk = weaponChartChart.Atk + (level * weaponChartChart.GrowingAtk);
                Spd = weaponChartChart.Spd + (level * weaponChartChart.GrowingSpd);
                Delay = weaponChartChart.Delay + (level * weaponChartChart.GrowingDelay);
                UpgradePrice = weaponChartChart.Price;
            }
        }

        // ������ ������ ���Ű� �����ϱ� ������ itemId�� �ƴ� �����ϱ� ���� ������ ������ ���� ���̵�
        public string MyWeaponId { get; private set; }
        // ���� ����
        public int WeaponLevel { get; private set; }
        // ������ ��Ʈ ���̵�(weaponId)
        public int WeaponChartId { get; private set; }

        // �⺻ ������ <- ����� ������ �������ݿ� ������ �޴´�
        private CurrentStat _normalStat;

        //Param�� Ŭ������ ���� ���, public �Ǿ��ִ� ������ �ڵ����� ���Եȴ�.
        //�ڳ� Dictionary<class> ���� ��, �����͸� ���ܽ�Ű���� �Լ��� �����ؾ��Ѵ�.
        private Chart.Weapon.Item _weaponChartData;

        // ������ ���� �� ������ �����͸� �ڳ��� �°� �Ľ��ϴ� �Լ�
        public Item(string myWeaponId, int weaponLevel,
            Chart.Weapon.Item weaponData)
        {
            MyWeaponId = myWeaponId;
            WeaponLevel = weaponLevel;
            WeaponChartId = weaponData.WeaponID;
            _weaponChartData = weaponData;
            _normalStat = new CurrentStat(weaponLevel, weaponData);
        }

        // �ش� ������ ��Ʈ ����
        public Chart.Weapon.Item GetWeaponChartData()
        {
            return _weaponChartData;
        }

        // ���� ���� ����
        public CurrentStat GetCurrentWeaponStat()
        {
            return _normalStat;
        }

        // ������
        public void LevelUp()
        {
            WeaponLevel++;
            _normalStat.LevelUp(WeaponLevel, _weaponChartData);
        }
    }
}