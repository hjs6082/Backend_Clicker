// Copyright 2013-2022 AFI, INC. All rights reserved.

using System;
using System.Collections;
using UnityEngine;

using static InGameScene.Buff;

namespace InGameScene
{
    //===============================================================
    // ������ �����ϴ� Ŭ����
    //===============================================================
    public class BuffManager : MonoBehaviour
    {

        // ���� �迭
        private Buff[] _buffArray;
        private WaitForSeconds _buffCountSeconds = new WaitForSeconds(1);

        public void Init()
        {
            // ������ ������ŭ ���� �迭 �Ҵ�
            int buffTypeCount = Enum.GetValues(typeof(BuffStatType)).Length;
            _buffArray = new Buff[buffTypeCount];

            for (int i = 0; i < buffTypeCount; i++)
            {
                _buffArray[i] = new Buff();
            }
        }

        public bool StartBuff(BuffStatType buffStatType, float stat, float time,
            BuffAdditionType buffAdditionType)
        {

            // ���� ����
            int buffCase = (int)buffStatType;

            float remainTime = _buffArray[buffCase].Time;

            if (remainTime > 0)
            {
                StaticManager.UI.AlertUI.OpenWarningUI("���Ұ� �ȳ�", $"���� ������Դϴ�.\n�����ð� : {remainTime}");
                return false;
            }

            _buffArray[buffCase].UpdateBuff(stat, time, buffAdditionType);
            StartCoroutine(StartBuffCoroutine(_buffArray[buffCase]));
            return true;
        }

        // 1�ʸ��� �ݺ��ϸ鼭 �ð��� ����ϴ� �ڷ�ƾ�Լ�.
        IEnumerator StartBuffCoroutine(Buff buff)
        {
            while (buff.Time > 0)
            {
                yield return _buffCountSeconds;
                buff.Time -= 1;
            }

            buff.TurnOffBuff();
        }

        // �⺻  ���ݿ��� ������ ���ݸ�ŭ ����Ͽ� �����ϴ� �Լ�.
        public float GetBuffedStat(BuffStatType buffStateType, float originalStat)
        {
            try
            {
                if (_buffArray[(int)buffStateType].IsBuffing == false)
                {
                    return originalStat;
                }

                // ���� ���°� ������ ��� �׳� ���ϱ�, ����
                if (_buffArray[(int)buffStateType].BuffAddition == BuffAdditionType.Plus)
                {
                    return originalStat + _buffArray[(int)buffStateType].Stat;
                }
                else
                {
                    // ������ ���, ���ϱ�
                    return originalStat * _buffArray[(int)buffStateType].Stat;
                }
            }
            catch (Exception e)
            {
                throw new Exception($"GetBuffedStat {buffStateType} {originalStat} ����\n" + e.ToString());
            }
        }
    }
}