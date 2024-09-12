using BackndChat;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using TMPro;
using UnityEngine.UI;

public class ChatManager : MonoBehaviour, BackndChat.IChatClientListener
{
    private BackndChat.ChatClient ChatClient = null;

    [SerializeField]
    private TMP_Text quickChat;

    [SerializeField]
    private GameObject ChatContent = null;

    [SerializeField]
    private TMP_InputField ChatInput = null;

    [SerializeField]
    private Button SendButton = null;

    private ChannelInfo _channelInfo;

    private RectTransform rectTransform;

    private bool isMiniMize = true;

    void Start()
    {
        ChatClient = new ChatClient(this, new ChatClientArguments
        {
            Avatar = "default"
        });

        if (SendButton != null)
        {
            SendButton.onClick.AddListener(SendChatMessage);
        }

        if (ChatInput != null)
        {
            ChatInput.onEndEdit.AddListener((string text) =>
            {
                if (Input.GetKeyDown(KeyCode.Return))
                {
                    SendChatMessage();
                }
            });
        }

        rectTransform = this.gameObject.GetComponent<RectTransform>();

        ChatInput.gameObject.SetActive(false);
        SendButton.gameObject.SetActive(false);
    }

    void Update()
    {
        ChatClient?.Update();
    }

    private void SendChatMessage()
    {
        if (ChatClient == null) return;

        if (ChatInput == null) return;

        if (ChatInput.text.Length == 0) return;

        if (ChatInput.text.IndexOf("/w") == 0)
        {
            string[] whisper = ChatInput.text.Split(' ');

            if (whisper.Length < 3)
            {
                return;
            }

            string message = string.Empty;
            for (int i = 2; i < whisper.Length; ++i)
            {
                if (i == whisper.Length - 1)
                {
                    message += whisper[i];
                    break;
                }

                message += whisper[i] + " ";
            }

            ChatClient.SendWhisperMessage(whisper[1], message);
        }
        else
        {
            ChatClient.SendChatMessage(_channelInfo.ChannelGroup, _channelInfo.ChannelName, _channelInfo.ChannelNumber, ChatInput.text);
        }

        ChatInput.text = string.Empty;
        ContentFit();
    }


    private void OnApplicationQuit()
    {
        ChatClient?.Dispose();
    }

    public void OnJoinChannel(ChannelInfo channelInfo)
    {
        _channelInfo = channelInfo;
        if (isMiniMize)
        {
            string quickMessage = channelInfo.ChannelName + "채널에 입장하셨습니다.";
            if (quickMessage.Length > 19)
            {
                // 19자를 초과할 경우 앞 16글자만 보여주고 "..." 추가
                quickChat.text = quickMessage.Substring(0, 19 - 3) + "...";
            }
            else
            {
                // 19자 이하일 경우 그대로 출력
                quickChat.text = quickMessage;
            }
        }
     
        GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);
        chatList.GetComponent<UIChatList>().SetData(0, "관리자", channelInfo.ChannelName + "채널에 입장하셨습니다.");
    }

    public void OnLeaveChannel(ChannelInfo channelInfo)
    {
        //throw new System.NotImplementedException();
    }

    public void OnJoinChannelPlayer(string channelGroup, string channelName, ulong channelNumber, string gamerName, string avatar)
    {
        //throw new System.NotImplementedException();
    }

    public void OnLeaveChannelPlayer(string channelGroup, string channelName, ulong channelNumber, string gamerName, string avatar)
    {
        throw new System.NotImplementedException();
    }

    public void OnChatMessage(MessageInfo messageInfo)
    {
        if (isMiniMize)
        {
            string quickMessage = messageInfo.GamerName + " : " + messageInfo.Message;
            if (quickMessage.Length > 19)
            {
                // 19자를 초과할 경우 앞 16글자만 보여주고 "..." 추가
                quickChat.text = quickMessage.Substring(0, 19 - 3) + "...";
            }
            else
            {
                // 19자 이하일 경우 그대로 출력
                quickChat.text = quickMessage;
            }
        }

        GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);

        if (messageInfo.GamerName == BackEnd.Backend.GetBackendChatSettings().nickname)
        {
            chatList.GetComponent<UIChatList>().SetData(messageInfo.Index, messageInfo.GamerName, messageInfo.Message, true);
        }
        else
        {
            chatList.GetComponent<UIChatList>().SetData(messageInfo.Index, messageInfo.GamerName, messageInfo.Message);
        }
        ContentFit();
    }

    public void OnWhisperMessage(WhisperMessageInfo messageInfo)
    {
        throw new System.NotImplementedException();
    }

    public void OnTranslateMessage(List<MessageInfo> messages)
    {
        throw new System.NotImplementedException();
    }

    public void OnHideMessage(MessageInfo messageInfo)
    {
        throw new System.NotImplementedException();
    }

    public void OnDeleteMessage(MessageInfo messageInfo)
    {
        throw new System.NotImplementedException();
    }

    public void OnSuccess(SUCCESS_MESSAGE success, object param)
    {
        switch (success)
        {
            default:
                break;
        }
    }

    public void OnError(ERROR_MESSAGE error, object param)
    {
        switch (error)
        {
            default:
                break;
        }
    }

    public void OnClickMiniMize()
    {
        rectTransform.sizeDelta = new Vector2(626.9099f, 100f);
        int childCount = ChatContent.transform.childCount;
        Transform lastChild = ChatContent.transform.GetChild(childCount - 1);

        TMP_Text lastChildText = lastChild.GetComponent<TMP_Text>();

        if (lastChildText.text.Length > 19)
        {
            // 19자를 초과할 경우 앞 16글자만 보여주고 "..." 추가
            quickChat.text = lastChildText.text.Substring(0, 19 - 3) + "...";
        }
        else
        {
            // 19자 이하일 경우 그대로 출력
            quickChat.text = lastChildText.text;
        }
        SizeSet(true);
        ContentFit();
    }

    public void OnClickMaximize()
    {
        rectTransform.sizeDelta = new Vector2(967.48f, 231.67f);
        SizeSet(false);
        ContentFit();
    }

    private void SizeSet(bool isMinimize)
    {
        quickChat.gameObject.SetActive(isMinimize);
        isMiniMize = isMinimize;
        ChatInput.gameObject.SetActive(!isMinimize);
        SendButton.gameObject.SetActive(!isMinimize);
    }

    private void ContentFit()
    {
        Canvas.ForceUpdateCanvases();
        this.gameObject.GetComponent<ScrollRect>().verticalNormalizedPosition = isMiniMize == true ? 1f : 0f;
    }


}
