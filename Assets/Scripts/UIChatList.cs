using System;

using UnityEngine;
using UnityEngine.UI;

public class UIChatList : MonoBehaviour
{/*
    public Toggle CheckBox = null;

    public Image Avatar = null;
*/
    //public Text Name = null;

    public TMPro.TMP_Text Message = null;

  /*  public Text Time = null;

    public Button ReportButton = null;
*/
    private UInt64 Index = 0;

    public void SetData(UInt64 index, string name, string message, bool is_my = false)
    {
        Index = index;
/*
        if (avatar == string.Empty || avatar == "default")
        {
            Avatar.sprite = Resources.Load<Sprite>("Images/Girl_5");
        } 
        else
        {
            Avatar.sprite = Resources.Load<Sprite>("Images/" + avatar);
        }*/

        if (is_my)
        {
            Message.text = name + " (You) : " + message;
        }
        else
        {
            Message.text = name + " : " + message;
        }
        

/*        Time.text = time;

        ReportButton.onClick.RemoveAllListeners();
        ReportButton.onClick.AddListener(() =>
        {
            if (Index > 0 && Tag != string.Empty)
            {
                if (report != null)
                {
                    report(Index, Tag);
                }
            }
        });

        CheckBox.onValueChanged.RemoveAllListeners();
        CheckBox.onValueChanged.AddListener((bool isOn) =>
        {
            if (Index > 0 && Tag != string.Empty)
            {
                if (translate != null)
                {
                    string key = tag + "," + index.ToString();
                    translate(isOn, key);
                }
            }
        });*/
    }

/*    public bool IsEqual(UInt64 index, string tag)
    {
        return Index == index && Tag == tag;
    }*/

    public void SetMessage(string message)
    {
        Message.text = message;
    }
}
