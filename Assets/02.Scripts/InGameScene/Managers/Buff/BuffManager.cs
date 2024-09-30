using System;
using System.Collections;
using UnityEngine;
using TMPro;

namespace InGameScene
{
    //===============================================================
    // ������ �����ϴ� Ŭ����
    //===============================================================
    public class BuffManager : MonoBehaviour
    {
        private Buff[] _buffArray;
        private WaitForSeconds _buffCountSeconds = new WaitForSeconds(1);
        [SerializeField] private TMP_Text remainingTimeText; // UI �ؽ�Ʈ ���

        public void Init()
        {
            int buffTypeCount = Enum.GetValues(typeof(Buff.BuffStatType)).Length;
            _buffArray = new Buff[buffTypeCount];

            for (int i = 0; i < buffTypeCount; i++)
            {
                _buffArray[i] = new Buff();
            }
        }

        public bool StartBuff(Buff.BuffStatType buffStatType, float stat, float time,
            Buff.BuffAdditionType buffAdditionType)
        {
            int buffCase = (int)buffStatType;

            if (_buffArray[buffCase].Time > 0)
            {
                // �̹� Ȱ��ȭ�� ����
                return false;
            }

            _buffArray[buffCase].UpdateBuff(stat, time, buffAdditionType);
            StartCoroutine(StartBuffCoroutine(_buffArray[buffCase]));
            remainingTimeText.gameObject.SetActive(true);
            return true;
        }

        IEnumerator StartBuffCoroutine(Buff buff)
        {
            while (buff.Time > 0)
            {
                UpdateRemainingTimeText(buff.Time); // ���� �ð� ������Ʈ
                yield return _buffCountSeconds;
                buff.Time -= 1;
            }

            buff.TurnOffBuff();
            UpdateRemainingTimeText(0); // �ð��� ������ �� UI �ؽ�Ʈ �ʱ�ȭ
        }

        private void UpdateRemainingTimeText(float time)
        {
            if (time <= 0)
                remainingTimeText.gameObject.SetActive(false);
            remainingTimeText.text = $"���� ���� �ð�: {time}��"; // UI �ؽ�Ʈ ������Ʈ
        }

        public float GetBuffedStat(Buff.BuffStatType buffStatType, float originalStat)
        {
            if (!_buffArray[(int)buffStatType].IsBuffing)
            {
                return originalStat;
            }

            if (_buffArray[(int)buffStatType].BuffAddition == Buff.BuffAdditionType.Plus)
            {
                return originalStat + _buffArray[(int)buffStatType].Stat;
            }
            else
            {
                return originalStat * _buffArray[(int)buffStatType].Stat;
            }
        }

        public bool IsBuffActive(Buff.BuffStatType buffStatType)
        {
            return _buffArray[(int)buffStatType].IsBuffing;
        }

        public bool IsAnyBuffActive()
        {
            foreach (var buff in _buffArray)
            {
                if (buff.IsBuffing)
                {
                    return true; // Ȱ��ȭ�� ������ �ϳ��� ����
                }
            }
            return false; // ��� ������ ��Ȱ��ȭ��
        }
    }
}
