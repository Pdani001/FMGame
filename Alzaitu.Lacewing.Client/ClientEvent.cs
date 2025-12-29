using System;
using Alzaitu.Lacewing.Client.Packet.EventData;

namespace Alzaitu.Lacewing.Client
{
	public class ClientEvent
	{
		internal ClientEvent() { }

		public event EventHandler<EventError> Error;
		internal virtual void OnError(EventError e)
		{
			Error?.Invoke(this, e);
		}

		public event EventHandler<EventDisconnect> Disconnect;
		internal virtual void OnDisconnect(EventDisconnect e)
		{
			Disconnect?.Invoke(this, e);
		}

		public event EventHandler<EventResponseConnect> ResponseConnect;
		internal virtual void OnResponseConnect(EventResponseConnect e)
		{
			ResponseConnect?.Invoke(this, e);
		}

		public event EventHandler<EventResponseChannelList> ResponseChannelList;
		internal virtual void OnResponseChannelList(EventResponseChannelList e)
		{
			ResponseChannelList?.Invoke(this, e);
		}

		public event EventHandler<EventResponseJoinChannel> ResponseJoinChannel;
		internal virtual void OnResponseJoinChannel(EventResponseJoinChannel e)
		{
			ResponseJoinChannel?.Invoke(this, e);
		}

		public event EventHandler<EventResponseLeaveChannel> ResponseLeaveChannel;
		internal virtual void OnResponseLeaveChannel(EventResponseLeaveChannel e)
		{
			ResponseLeaveChannel?.Invoke(this, e);
		}

		public event EventHandler<EventResponseSetName> ResponseSetName;
		internal virtual void OnResponseSetName(EventResponseSetName e)
		{
			ResponseSetName?.Invoke(this, e);
		}

		public event EventHandler<EventBinaryMessage> BinaryMessage;
		internal virtual void OnBinaryMessage(EventBinaryMessage e)
		{
			BinaryMessage?.Invoke(this, e);
		}

		public event EventHandler<EventTextMessage> TextMessage;
		internal virtual void OnTextMessage(EventTextMessage e)
		{
			TextMessage?.Invoke(this, e);
		}

		public event EventHandler<EventNumberMessage> NumberMessage;
		internal virtual void OnNumberMessage(EventNumberMessage e)
		{
			NumberMessage?.Invoke(this, e);
		}

		public event EventHandler<EventPeer> Peer;
		internal virtual void OnPeer(EventPeer e)
		{
			Peer?.Invoke(this, e);
		}
		
		public event EventHandler<EventPing> Ping;
		internal virtual void OnPing(EventPing e)
		{
			Ping?.Invoke(this, e);
		}
	}
}
