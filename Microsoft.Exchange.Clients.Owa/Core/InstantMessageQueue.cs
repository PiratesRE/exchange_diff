using System;
using System.Collections.Generic;
using System.Globalization;
using System.Threading;
using Microsoft.Exchange.Diagnostics.Components.Clients;
using Microsoft.Exchange.InstantMessaging;

namespace Microsoft.Exchange.Clients.Owa.Core
{
	internal sealed class InstantMessageQueue
	{
		internal InstantMessageQueue(UserContext userContext, IConversation conversation, InstantMessagePayload payload)
		{
			this.userContext = userContext;
			this.conversation = conversation;
			this.payload = payload;
		}

		public IConversation Conversation
		{
			get
			{
				return this.conversation;
			}
			set
			{
				this.conversation = value;
			}
		}

		public List<InstantMessageChat> MessageList
		{
			get
			{
				return this.messageList;
			}
		}

		public void AddMessage(string contentType, string message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.AddMessage");
			Interlocked.CompareExchange<List<InstantMessageChat>>(ref this.messageList, new List<InstantMessageChat>(), null);
			bool flag = false;
			lock (this.lockObject)
			{
				if (this.messageList.Count < 20)
				{
					this.messageList.Add(new InstantMessageChat("text/plain;charset=utf-8", message));
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError<int>((long)this.GetHashCode(), "InstantMessageQueue.AddMessage. Message queued count: {0}", this.messageList.Count);
				InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.payload, this.conversation.Cid.ToString(CultureInfo.InvariantCulture));
			}
		}

		public void Clear()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.Clear");
			lock (this.lockObject)
			{
				if (this.messageList != null && this.messageList.Count > 0)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageQueue.Clear. Message list count: {0}", this.messageList.Count);
					this.messageList.Clear();
					this.messageList = null;
				}
			}
		}

		public void SendAndClearMessageList()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList");
			if (this.conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList. Conversation is null.");
				return;
			}
			if (this.messageList != null && this.messageList.Count > 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug<int>((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList. Message list count: {0}", this.messageList.Count);
				IIMModality iimmodality = this.conversation.GetModality(1) as IIMModality;
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList. IIMModality is null.");
					return;
				}
				lock (this.lockObject)
				{
					foreach (InstantMessageChat instantMessageChat in this.messageList)
					{
						iimmodality.BeginSendMessage(instantMessageChat.ContentType, instantMessageChat.Message, new AsyncCallback(this.SendMessageCallback), iimmodality);
					}
					this.messageList.Clear();
					this.messageList = null;
				}
			}
		}

		private void SendMessageCallback(IAsyncResult result)
		{
			IIMModality iimmodality = null;
			try
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.SendMessageCallback");
				iimmodality = (result.AsyncState as IIMModality);
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageQueue.SendMessageCallback. Instant Messaging Modality is null.");
				}
				else
				{
					iimmodality.EndSendMessage(result);
				}
			}
			catch (InstantMessagingException ex)
			{
				InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.payload, "InstantMessageQueue.SendMessageCallback", (iimmodality == null || iimmodality.Conversation == null) ? 0 : iimmodality.Conversation.Cid, ex);
				InstantMessagingError code = ex.Code;
				if (code <= 18102)
				{
					if (code == 0)
					{
						goto IL_DB;
					}
					if (code == 18102)
					{
						InstantMessagingErrorSubCode subCode = ex.SubCode;
						if (subCode != 9)
						{
							InstantMessageUtilities.SendWatsonReport("InstantMessageQueue.SendMessageCallback", this.userContext, ex);
							goto IL_DB;
						}
						goto IL_DB;
					}
				}
				else if (code == 18201 || code == 18204)
				{
					goto IL_DB;
				}
				InstantMessageUtilities.SendWatsonReport("InstantMessageQueue.SendMessageCallback", this.userContext, ex);
				IL_DB:;
			}
			catch (Exception exception)
			{
				InstantMessageUtilities.SendWatsonReport("InstantMessageQueue.SendMessageCallback", this.userContext, exception);
			}
		}

		private const int MaxMessageCount = 20;

		private UserContext userContext;

		private IConversation conversation;

		private InstantMessagePayload payload;

		private List<InstantMessageChat> messageList;

		private object lockObject = new object();
	}
}
