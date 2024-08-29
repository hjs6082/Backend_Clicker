using BackndChat;

using System;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class UIChatManager : MonoBehaviour, BackndChat.IChatClientListener
{
    //public GameObject ChannelContent = null;

    public GameObject ChatContent = null;

    public GameObject UserContent = null;

    public Text ChannelUserCount = null;

    public InputField ChatInput = null;

    public Button SendButton = null;

    public GameObject ReportPopup = null;

    public GameObject channelInfoObject;

    //public Button JoinChannelButton = null;

    //public GameObject JoinChannelPopup = null;
    
    private string CurrentChannelGroup = string.Empty;

    private string CurrentChannelName = string.Empty;

    private UInt64 CurrentChannelNumber = 0;

    private List<string> SelectMessageKey = new List<string>();

    private ChatClient ChatClient = null;

    private Dictionary<string, Dictionary<string, Dictionary<UInt64, ChannelInfo>>> ChannelList = new Dictionary<string, Dictionary<string, Dictionary<UInt64, ChannelInfo>>>();

    // Start is called before the first frame update
    void Start()
    {
        if (SendButton != null)
        {
            SendButton.onClick.AddListener(SendChatMessage);
        }

        //if (JoinChannelButton != null)
        //{
        //    JoinChannelButton.onClick.AddListener(OnClickJoinChannel);
        //}
        
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

        List<string> avatars = new List<string>
        {
            "Boy_1",
            "Boy_2",
            "Boy_3",
            "Boy_4",
            "Girl_1",
            "Girl_2",
            "Girl_3",
            "Girl_4"
        };

        string avatar = avatars[UnityEngine.Random.Range(0, avatars.Count)];

        ChatClient = new ChatClient(this, new ChatClientArguments
        {
            Avatar = avatar,
        });

        Debug.Log(ChatClient);
        
    }

    // Update is called once per frame
    void Update()
    {
        ChatClient?.Update();
    }

    private void SendChatMessage()
    {
        if (ChatClient == null) return;

        if (ChatInput == null) return;

        if (ChatInput.text.Length == 0) return;

        if (CurrentChannelName == string.Empty) return;

        if (!ChannelList.ContainsKey(CurrentChannelGroup)) return;

        if (!ChannelList[CurrentChannelGroup].ContainsKey(CurrentChannelName)) return;

        if (!ChannelList[CurrentChannelGroup][CurrentChannelName].ContainsKey(CurrentChannelNumber)) return;

        ChannelInfo channelInfo = ChannelList[CurrentChannelGroup][CurrentChannelName][CurrentChannelNumber];
        if (channelInfo == null) return;

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
        else if (ChatInput.text.IndexOf("/translate") == 0)
        {
            if (SelectMessageKey.Count == 0) return;

            List<string> langaues = new List<string>();

            string[] translate = ChatInput.text.Split(' ');
            for (int i = 1; i < translate.Length; ++i)
            {
                langaues.Add(translate[i]);
            }

            List<MessageInfo> messages = new List<MessageInfo>();
            foreach (var key in SelectMessageKey)
            {
                string[] keys = key.Split(',');
                if (keys.Length < 2) continue;

                string tag = keys[0];
                UInt64 index = Convert.ToUInt64(keys[1]);

                foreach (var message in channelInfo.Messages)
                {
                    if (message.Index == index && message.Tag == tag)
                    {
                        messages.Add(message);
                        break;
                    }
                }
            }

            for (int i = 0; i < messages.Count; ++i)
            {
                ChatClient.SendTranslateChatMessage(messages[i], langaues);
            }
        }
        else
        {
            ChatClient.SendChatMessage(channelInfo.ChannelGroup, channelInfo.ChannelName, channelInfo.ChannelNumber, ChatInput.text);
        }

        ChatInput.text = string.Empty;
    }

    private void OnChannelSelected(string channelGroup, string channelName, UInt64 channelNumber)
    {
        if (!ChannelList.ContainsKey(channelGroup)) return;

        if (!ChannelList[channelGroup].ContainsKey(channelName)) return;

        if (!ChannelList[channelGroup][channelName].ContainsKey(channelNumber)) return;

        ChannelInfo channelInfo = ChannelList[channelGroup][channelName][channelNumber];
        if (channelInfo == null) return;

        SelectMessageKey.Clear();

        if (ChatContent != null)
        {
            foreach (Transform child in ChatContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        if (UserContent != null)
        {
            foreach (Transform child in UserContent.transform)
            {
                Destroy(child.gameObject);
            }
        }

        if (ChannelUserCount != null)
        {
            ChannelUserCount.text = string.Format("{0} / {1}", channelInfo.Players.Count, channelInfo.MaxCount);
        }

        BackEnd.Backend.BackendChatSettings settings = BackEnd.Backend.GetBackendChatSettings();

        if (UserContent != null)
        {
            foreach (var player in channelInfo.Players)
            {
                GameObject userList = Instantiate(Resources.Load<GameObject>("Prefabs/UserList"), UserContent.transform);
                userList.name = player.Value.GamerName;

                if (player.Value.GamerName == settings.nickname)
                {
                    userList.GetComponent<UIUserList>().SetData(player.Value.Avatar, player.Value.GamerName, true);
                }
                else
                {
                    userList.GetComponent<UIUserList>().SetData(player.Value.Avatar, player.Value.GamerName);
                }
            }
        }

        if (ChatContent != null)
        {
            foreach (var message in channelInfo.Messages)
            {
                GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);

                if (message.GamerName == settings.nickname)
                {
                    chatList.GetComponent<UIChatList>().SetData(message.Index, message.Avatar, message.GamerName, message.Message, message.Time, message.Tag, OnReportButton, OnTranslateCheckButton, true);
                }
                else
                {
                    chatList.GetComponent<UIChatList>().SetData(message.Index, message.Avatar, message.GamerName, message.Message, message.Time, message.Tag, OnReportButton, OnTranslateCheckButton);
                }
            }
        }

        CurrentChannelGroup = channelGroup;
        CurrentChannelName = channelName;
        CurrentChannelNumber = channelNumber;
    }

    private void SendReportChat(UInt64 index, string tag, string keyword, string reason)
    {
        if (ChatClient == null) return;

        ChatClient.SendReportChatMessage(index, tag, keyword, reason);
    }

    private void OnReportButton(UInt64 index, string tag)
    {
        if (ReportPopup)
        {
            ReportPopup.GetComponent<UIReportManager>().SetData(index, tag, ChatClient?.GetReportReasons(), SendReportChat);
        }
    }

    private void OnTranslateCheckButton(bool isOn, string messageKey)
    {
        if (isOn)
        {
            for (int i = 0; i < SelectMessageKey.Count; ++i)
            {
                if (SelectMessageKey[i] == messageKey) return;
            }

            SelectMessageKey.Add(messageKey);
        }
        else
        {
            for (int i = 0; i < SelectMessageKey.Count; ++i)
            {
                if (SelectMessageKey[i] == messageKey)
                {
                    SelectMessageKey.RemoveAt(i);
                    break;
                }
            }
        }
    }

    private void SendCreatePrivateChannel(string channelGroup, UInt64 channelNumber, string channelName, uint maxCount, string password)
    {
        if (ChatClient == null) return;

        ChatClient.SendCreatePrivateChannel(channelGroup, channelNumber, channelName, maxCount, password);
    }

    private void SendJoinOpenChannel(string channelGroup, string channelName)
    {
        if (ChatClient == null) return;

        ChatClient.SendJoinOpenChannel(channelGroup, channelName);
    }

    private void SendJoinPrivateChannel(string channelGroup, UInt64 channelNumber, string password)
    {
        if (ChatClient == null) return;

        ChatClient.SendJoinPrivateChannel(channelGroup, channelNumber, password);
    }

/*    public void OnClickJoinChannel()
    {
        if (JoinChannelPopup)
        {
            JoinChannelPopup.GetComponent<UIJoinChannelManager>().SetData(SendCreatePrivateChannel, SendJoinOpenChannel, SendJoinPrivateChannel);
        }
    }*/

    public void OnJoinChannel(ChannelInfo channelInfo)
    {
        if (ChannelList.ContainsKey(channelInfo.ChannelGroup))
        {
            if (ChannelList[channelInfo.ChannelGroup].ContainsKey(channelInfo.ChannelName))
            {
                if (ChannelList[channelInfo.ChannelGroup][channelInfo.ChannelName].ContainsKey(channelInfo.ChannelNumber)) return;
            }
        }

        if (!ChannelList.ContainsKey(channelInfo.ChannelGroup))
        {
            ChannelList.Add(channelInfo.ChannelGroup, new Dictionary<string, Dictionary<UInt64, ChannelInfo>>());
            ChannelList[channelInfo.ChannelGroup].Add(channelInfo.ChannelName, new Dictionary<UInt64, ChannelInfo>());
        }
        else
        {
            if (!ChannelList[channelInfo.ChannelGroup].ContainsKey(channelInfo.ChannelName))
            {
                ChannelList[channelInfo.ChannelGroup].Add(channelInfo.ChannelName, new Dictionary<UInt64, ChannelInfo>());
            }
        }

        ChannelList[channelInfo.ChannelGroup][channelInfo.ChannelName].Add(channelInfo.ChannelNumber, channelInfo);

        OnChannelSelected(channelInfo.ChannelGroup, channelInfo.ChannelName, channelInfo.ChannelNumber);

        GameObject channelInfoObj = Instantiate(Resources.Load<GameObject>("Prefabs/ChannelInfo"), ChatContent.transform);
        channelInfoObj.GetComponent<TMPro.TextMeshProUGUI>().text = channelInfo.ChannelName + "채널에 입장하셨습니다 " + "채널 현재 인원 : " + channelInfo.Players.Count;
        /*
        GameObject channelList = Instantiate(Resources.Load<GameObject>("Prefabs/ChannelList"), ChannelContent.transform);
        channelList.name = channelInfo.ChannelGroup + "_" + channelInfo.ChannelName + "_" + channelInfo.ChannelNumber.ToString();
        channelList.GetComponent<UIChannelList>().AddChannel(channelInfo.ChannelGroup, channelInfo.ChannelName, channelInfo.ChannelNumber, OnChannelSelected);

        if (ChannelList.Count == 1)
        {
            OnChannelSelected(channelInfo.ChannelGroup, channelInfo.ChannelName, channelInfo.ChannelNumber);
        }*/
    }

    
    public void OnLeaveChannel(ChannelInfo channelInfo)
    {
        if (!ChannelList.ContainsKey(channelInfo.ChannelGroup)) return;

        if (!ChannelList[channelInfo.ChannelGroup].ContainsKey(channelInfo.ChannelName)) return;

        if (!ChannelList[channelInfo.ChannelGroup][channelInfo.ChannelName].ContainsKey(channelInfo.ChannelNumber)) return;

        ChannelList[channelInfo.ChannelGroup][channelInfo.ChannelName].Remove(channelInfo.ChannelNumber);

        if (ChannelList[channelInfo.ChannelGroup][channelInfo.ChannelName].Count == 0)
        {
            ChannelList[channelInfo.ChannelGroup].Remove(channelInfo.ChannelName);
        }

        if (ChannelList[channelInfo.ChannelGroup].Count == 0)
        {
            ChannelList.Remove(channelInfo.ChannelGroup);
        }

/*        if (ChannelContent != null)
        {
            foreach (Transform child in ChannelContent.transform)
            {
                if (child.name == channelInfo.ChannelGroup + "_" + channelInfo.ChannelName + "_" + channelInfo.ChannelNumber.ToString())
                {
                    Destroy(child.gameObject);
                    break;
                }
            }
        }*/

        if (CurrentChannelGroup == channelInfo.ChannelGroup && CurrentChannelName == channelInfo.ChannelName && CurrentChannelNumber == channelInfo.ChannelNumber)
        {
            if (ChannelList.Count > 0)
            {
                foreach (var channel in ChannelList)
                {
                    foreach (var channelName in channel.Value)
                    {
                        foreach (var channelNumber in channelName.Value)
                        {
                            OnChannelSelected(channel.Key, channelName.Key, channelNumber.Key);
                            return;
                        }
                    }
                }
            }
            else
            {

                if (ChatContent != null)
                {
                    foreach (Transform child in ChatContent.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }

                if (UserContent != null)
                {
                    foreach (Transform child in UserContent.transform)
                    {
                        Destroy(child.gameObject);
                    }
                }

                if (ChannelUserCount != null)
                {
                    ChannelUserCount.text = "0 / 0";
                }
            }
        }
    }

    public void OnJoinChannelPlayer(string channelGroup, string channelName, UInt64 channelNumber, string gamerName, string avatar)
    {
        if (!ChannelList.ContainsKey(channelGroup)) return;

        if (!ChannelList[channelGroup].ContainsKey(channelName)) return;

        if (!ChannelList[channelGroup][channelName].ContainsKey(channelNumber)) return;

        ChannelInfo channelInfo = ChannelList[channelGroup][channelName][channelNumber];
        if (channelInfo == null) return;

        if (channelInfo.Players.ContainsKey(gamerName)) return;

        PlayerInfo playerInfo = new PlayerInfo()
        {
            GamerName = gamerName,
            Avatar = avatar
        };

        channelInfo.Players.Add(gamerName, playerInfo);

        if (CurrentChannelGroup == channelGroup && CurrentChannelName == channelName && CurrentChannelNumber == channelNumber)
        {
            if (ChannelUserCount != null)
            {
                ChannelUserCount.text = string.Format("{0} / {1}", channelInfo.Players.Count, channelInfo.MaxCount);
            }

            if (UserContent != null)
            {
                GameObject userList = Instantiate(Resources.Load<GameObject>("Prefabs/UserList"), UserContent.transform);
                userList.name = playerInfo.GamerName;

                if (playerInfo.GamerName == BackEnd.Backend.GetBackendChatSettings().nickname)
                {
                    userList.GetComponent<UIUserList>().SetData(playerInfo.Avatar, playerInfo.GamerName, true);
                }
                else
                {
                    userList.GetComponent<UIUserList>().SetData(playerInfo.Avatar, playerInfo.GamerName);
                }
            }
        }
    }

    public void OnLeaveChannelPlayer(string channelGroup, string channelName, UInt64 channelNumber, string gamerName, string avatar)
    {
        if (!ChannelList.ContainsKey(channelGroup)) return;

        if (!ChannelList[channelGroup].ContainsKey(channelName)) return;

        if (!ChannelList[channelGroup][channelName].ContainsKey(channelNumber)) return;

        ChannelInfo channelInfo = ChannelList[channelGroup][channelName][channelNumber];
        if (channelInfo == null) return;

        if (!channelInfo.Players.ContainsKey(gamerName)) return;

        channelInfo.Players.Remove(gamerName);

        if (CurrentChannelGroup == channelGroup && CurrentChannelName == channelName && CurrentChannelNumber == channelNumber)
        {
            if (ChannelUserCount != null)
            {
                ChannelUserCount.text = string.Format("{0} / {1}", channelInfo.Players.Count, channelInfo.MaxCount);
            }

            if (UserContent != null)
            {
                foreach (Transform child in UserContent.transform)
                {
                    if (child.name == gamerName)
                    {
                        Destroy(child.gameObject);
                        break;
                    }
                }
            }
        }
    }

    public void OnChatMessage(MessageInfo messageInfo)
    {
        if (!ChannelList.ContainsKey(messageInfo.ChannelGroup)) return;

        if (!ChannelList[messageInfo.ChannelGroup].ContainsKey(messageInfo.ChannelName)) return;

        if (!ChannelList[messageInfo.ChannelGroup][messageInfo.ChannelName].ContainsKey(messageInfo.ChannelNumber)) return;

        ChannelInfo channelInfo = ChannelList[messageInfo.ChannelGroup][messageInfo.ChannelName][messageInfo.ChannelNumber];
        if (channelInfo == null) return;

        channelInfo.Messages.Add(messageInfo);

        if (CurrentChannelGroup == messageInfo.ChannelGroup && CurrentChannelName == messageInfo.ChannelName && CurrentChannelNumber == messageInfo.ChannelNumber)
        {
            if (ChatContent != null)
            {
                GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);

                if (messageInfo.GamerName == BackEnd.Backend.GetBackendChatSettings().nickname)
                {
                    chatList.GetComponent<UIChatList>().SetData(messageInfo.Index, messageInfo.Avatar, messageInfo.GamerName, messageInfo.Message, messageInfo.Time, messageInfo.Tag, OnReportButton, OnTranslateCheckButton, true);
                }
                else
                {
                    chatList.GetComponent<UIChatList>().SetData(messageInfo.Index, messageInfo.Avatar, messageInfo.GamerName, messageInfo.Message, messageInfo.Time, messageInfo.Tag, OnReportButton, OnTranslateCheckButton);
                }
            }
        }
    }

    public void OnWhisperMessage(WhisperMessageInfo messageInfo)
    {
        if (!ChannelList.ContainsKey(CurrentChannelGroup)) return;

        if (!ChannelList[CurrentChannelGroup].ContainsKey(CurrentChannelName)) return;

        if (!ChannelList[CurrentChannelGroup][CurrentChannelName].ContainsKey(CurrentChannelNumber)) return;

        ChannelInfo channelInfo = ChannelList[CurrentChannelGroup][CurrentChannelName][CurrentChannelNumber];
        if (channelInfo == null) return;

        MessageInfo add_messageInfo = new MessageInfo()
        {
            ChannelGroup = channelInfo.ChannelGroup,
            ChannelName = channelInfo.ChannelName,
            ChannelNumber = channelInfo.ChannelNumber,
            Index = messageInfo.Index,
            GamerName = messageInfo.FromGamerName,
            Avatar = messageInfo.FromAvatar,
            Message = "[귓속말] " + messageInfo.Message,
            Time = messageInfo.Time,
            Tag = messageInfo.Tag
        };

        channelInfo.Messages.Add(add_messageInfo);

        if (ChatContent != null)
        {
            GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);

            if (messageInfo.FromGamerName == BackEnd.Backend.GetBackendChatSettings().nickname)
            {
                chatList.GetComponent<UIChatList>().SetData(add_messageInfo.Index, add_messageInfo.Avatar, add_messageInfo.GamerName, add_messageInfo.Message, add_messageInfo.Time, add_messageInfo.Tag, OnReportButton, OnTranslateCheckButton, true);
            }
            else
            {
                chatList.GetComponent<UIChatList>().SetData(add_messageInfo.Index, add_messageInfo.Avatar, add_messageInfo.GamerName, add_messageInfo.Message, add_messageInfo.Time, add_messageInfo.Tag, OnReportButton, OnTranslateCheckButton);
            }
        }
    }

    public void OnTranslateMessage(List<MessageInfo> messages)
    {
        if (!ChannelList.ContainsKey(CurrentChannelGroup)) return;

        if (!ChannelList[CurrentChannelGroup].ContainsKey(CurrentChannelName)) return;

        if (!ChannelList[CurrentChannelGroup][CurrentChannelName].ContainsKey(CurrentChannelNumber)) return;

        ChannelInfo channelInfo = ChannelList[CurrentChannelGroup][CurrentChannelName][CurrentChannelNumber];
        if (channelInfo == null) return;

        if (ChatContent != null)
        {
            foreach (var message in messages)
            {
                message.GamerName = message.GamerName + " (번역)";

                GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);

                if (message.GamerName == BackEnd.Backend.GetBackendChatSettings().nickname)
                {
                    chatList.GetComponent<UIChatList>().SetData(message.Index, message.Avatar, message.GamerName, message.Message, message.Time, message.Tag, null, null, true);
                }
                else
                {
                    chatList.GetComponent<UIChatList>().SetData(message.Index, message.Avatar, message.GamerName, message.Message, message.Time, message.Tag, null, null);
                }
            }
        }
    }

    public void OnHideMessage(MessageInfo messageInfo)
    {
        if (!ChannelList.ContainsKey(messageInfo.ChannelGroup)) return;

        if (!ChannelList[messageInfo.ChannelGroup].ContainsKey(messageInfo.ChannelName)) return;

        if (!ChannelList[messageInfo.ChannelGroup][messageInfo.ChannelName].ContainsKey(messageInfo.ChannelNumber)) return;

        ChannelInfo channelInfo = ChannelList[messageInfo.ChannelGroup][messageInfo.ChannelName][messageInfo.ChannelNumber];
        if (channelInfo == null) return;

        if (CurrentChannelGroup == messageInfo.ChannelGroup && CurrentChannelName == messageInfo.ChannelName && CurrentChannelNumber == messageInfo.ChannelNumber)
        {
            if (ChatContent != null)
            {
                foreach (Transform child in ChatContent.transform)
                {
                    UIChatList chatList = child.GetComponent<UIChatList>();
                    if (chatList != null)
                    {
                        if (chatList.IsEqual(messageInfo.Index, messageInfo.Tag))
                        {
                            chatList.SetMessage(messageInfo.Message);
                            break;
                        }
                    }
                }
            }
        }

        foreach (var message in channelInfo.Messages)
        {
            if (message.Index == messageInfo.Index && message.Tag == messageInfo.Tag)
            {
                message.Message = messageInfo.Message;
                break;
            }
        }
    }

    public void OnDeleteMessage(MessageInfo messageInfo)
    {
        if (!ChannelList.ContainsKey(messageInfo.ChannelGroup)) return;

        if (!ChannelList[messageInfo.ChannelGroup].ContainsKey(messageInfo.ChannelName)) return;

        if (!ChannelList[messageInfo.ChannelGroup][messageInfo.ChannelName].ContainsKey(messageInfo.ChannelNumber)) return;

        ChannelInfo channelInfo = ChannelList[messageInfo.ChannelGroup][messageInfo.ChannelName][messageInfo.ChannelNumber];
        if (channelInfo == null) return;

        if (CurrentChannelGroup == messageInfo.ChannelGroup && CurrentChannelName == messageInfo.ChannelName && CurrentChannelNumber == messageInfo.ChannelNumber)
        {
            if (ChatContent != null)
            {
                foreach (Transform child in ChatContent.transform)
                {
                    UIChatList chatList = child.GetComponent<UIChatList>();
                    if (chatList != null)
                    {
                        if (chatList.IsEqual(messageInfo.Index, messageInfo.Tag))
                        {
                            Destroy(child.gameObject);
                            break;
                        }
                    }
                }
            }
        }

        foreach (var message in channelInfo.Messages)
        {
            if (message.Index == messageInfo.Index && message.Tag == messageInfo.Tag)
            {
                channelInfo.Messages.Remove(message);
                break;
            }
        }
    }

    public void OnSuccess(SUCCESS_MESSAGE success, object param)
    {
        switch (success)
        {
            case SUCCESS_MESSAGE.REPORT:
                {
                    MessageInfo messageInfo = new MessageInfo
                    {
                        Index = 0,
                        GamerName = "SYSTEM",
                        Avatar = "Girl_5",
                        Message = "신고 처리가 완료되었습니다.",
                        Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
                        Tag = ""
                    };

                    if (ChannelList.ContainsKey(CurrentChannelGroup))
                    {
                        if (ChannelList[CurrentChannelGroup].ContainsKey(CurrentChannelName))
                        {
                            if (ChannelList[CurrentChannelGroup][CurrentChannelName].ContainsKey(CurrentChannelNumber))
                            {
                                ChannelInfo channelInfo = ChannelList[CurrentChannelGroup][CurrentChannelName][CurrentChannelNumber];
                                if (channelInfo != null)
                                {
                                    messageInfo.ChannelGroup = channelInfo.ChannelGroup;
                                    messageInfo.ChannelName = channelInfo.ChannelName;
                                    messageInfo.ChannelNumber = channelInfo.ChannelNumber;

                                    channelInfo.Messages.Add(messageInfo);
                                }
                            }
                        }
                    }

                    if (ChatContent != null)
                    {
                        GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);
                        chatList.GetComponent<UIChatList>().SetData(messageInfo.Index, messageInfo.Avatar, messageInfo.GamerName, messageInfo.Message, messageInfo.Time, messageInfo.Tag, OnReportButton, OnTranslateCheckButton);
                    }
                }
                break;
        }
    }

    public void OnError(ERROR_MESSAGE error, object param)
    {
        MessageInfo messageInfo = new MessageInfo
        {
            Index = 0,
            GamerName = "SYSTEM",
            Avatar = "Girl_5",
            Time = DateTime.Now.ToString("yyyy-MM-dd HH:mm:ss"),
            Tag = ""
        };

        if (error == ERROR_MESSAGE.CHAT_BAN)
        {
            ErrorMessageChatBanParam errorMessageChatBanParam = (ErrorMessageChatBanParam)param;
            if (errorMessageChatBanParam == null) return;

            var banTime = DateTime.Now.AddSeconds(errorMessageChatBanParam.RemainSeconds);

            messageInfo.Message = error.ToString() + " : " + banTime.ToString("yyyy-MM-dd HH:mm:ss") + " 까지";
        }
        else if (error == ERROR_MESSAGE.CHANNEL_FULL ||
            error == ERROR_MESSAGE.INVALID_PASSWORD ||
            error == ERROR_MESSAGE.ALREADY_CREATED_CHANNEL ||
            error == ERROR_MESSAGE.CHANNEL_GROUP_TOO_SHORT ||
            error == ERROR_MESSAGE.CHANNEL_GROUP_TOO_LONG ||
            error == ERROR_MESSAGE.CHANNEL_NAME_TOO_SHORT ||
            error == ERROR_MESSAGE.CHANNEL_NAME_TOO_LONG ||
            error == ERROR_MESSAGE.DUPLICATE_CHANNEL_GROUP ||
            error == ERROR_MESSAGE.PASSWORD_TOO_LONG ||
            error == ERROR_MESSAGE.CHANNEL_GROUP_FILTERED ||
            error == ERROR_MESSAGE.CHANNEL_NAME_FILTERED)
        {
            ErrorMessageChannelParam errorMessageChannelParam = (ErrorMessageChannelParam)param;
            if (errorMessageChannelParam == null) return;

            messageInfo.Message = error.ToString() + " : " + errorMessageChannelParam.ChannelGroup + " / " + errorMessageChannelParam.ChannelName + " / " + errorMessageChannelParam.ChannelNumber;
        }
        else
        {
            messageInfo.Message = error.ToString();
        }

        if (ChannelList.ContainsKey(CurrentChannelGroup))
        {
            if (ChannelList[CurrentChannelGroup].ContainsKey(CurrentChannelName))
            {
                if (ChannelList[CurrentChannelGroup][CurrentChannelName].ContainsKey(CurrentChannelNumber))
                {
                    ChannelInfo channelInfo = ChannelList[CurrentChannelGroup][CurrentChannelName][CurrentChannelNumber];
                    if (channelInfo != null)
                    {
                        messageInfo.ChannelGroup = channelInfo.ChannelGroup;
                        messageInfo.ChannelName = channelInfo.ChannelName;
                        messageInfo.ChannelNumber = channelInfo.ChannelNumber;

                        channelInfo.Messages.Add(messageInfo);
                    }
                }
            }
        }

        if (ChatContent != null)
        {
            GameObject chatList = Instantiate(Resources.Load<GameObject>("Prefabs/ChatList"), ChatContent.transform);
            chatList.GetComponent<UIChatList>().SetData(messageInfo.Index, messageInfo.Avatar, messageInfo.GamerName, messageInfo.Message, messageInfo.Time, messageInfo.Tag, OnReportButton, OnTranslateCheckButton);
        }
    }

    private void OnApplicationQuit()
    {
        ChatClient?.Dispose();
    }
}
