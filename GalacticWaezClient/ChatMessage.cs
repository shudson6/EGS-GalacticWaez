﻿using Eleon;
using Eleon.Modding;

namespace GalacticWaez
{
    public class ChatMessage: MessageData
    {
        public ChatMessage(string message, IPlayer player, 
            MsgChannel channel = MsgChannel.Global)
        {
            SenderType = SenderType.System;
            SenderNameOverride = "Waez";
            Channel = channel;
            RecipientEntityId = player.Id;
            RecipientFaction = player.Faction;
            Text = message;
            IsTextLocaKey = false;
        }
    }
}
