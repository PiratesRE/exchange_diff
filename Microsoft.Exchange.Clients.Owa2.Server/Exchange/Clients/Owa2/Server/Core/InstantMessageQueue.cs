using System;
using System.Collections.Generic;
using System.Threading;
using Microsoft.Exchange.Clients.Owa.Server.LyncIMLogging;
using Microsoft.Exchange.InstantMessaging;

namespace Microsoft.Exchange.Clients.Owa2.Server.Core
{
	internal sealed class InstantMessageQueue
	{
		internal InstantMessageQueue(IUserContext userContext, IConversation conversation, InstantMessageNotifier notifier)
		{
			this.userContext = userContext;
			this.Conversation = conversation;
			this.notifier = notifier;
		}

		public IConversation Conversation { get; set; }

		public List<Tuple<string, string>> MessageList
		{
			get
			{
				return this.messageList;
			}
		}

		public void AddMessage(string contentType, string message)
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.AddMessage");
			Interlocked.CompareExchange<List<Tuple<string, string>>>(ref this.messageList, new List<Tuple<string, string>>(), null);
			bool flag = false;
			lock (this.lockObject)
			{
				if (this.messageList.Count < 20)
				{
					if (string.IsNullOrWhiteSpace(contentType))
					{
						contentType = "text/plain;charset=utf-8";
					}
					this.messageList.Add(new Tuple<string, string>(contentType, message));
				}
				else
				{
					flag = true;
				}
			}
			if (flag)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageQueue.AddMessage. Message queued count: {0}", new object[]
				{
					this.messageList.Count
				});
				InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.notifier, "InstantMessageQueue.AddMessage", this.Conversation.Cid, UserActivityType.FailedDelivery);
			}
		}

		public void Clear()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.Clear");
			lock (this.lockObject)
			{
				if (this.messageList != null && this.messageList.Count > 0)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.Clear. Message list count: {0}", new object[]
					{
						this.messageList.Count
					});
					this.messageList.Clear();
					this.messageList = null;
				}
			}
		}

		public void SendAndClearMessageList()
		{
			ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList");
			if (this.Conversation == null)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList. Conversation is null.");
				return;
			}
			if (this.messageList != null && this.messageList.Count > 0)
			{
				ExTraceGlobals.InstantMessagingTracer.TraceDebug((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList. Message list count: {0}", new object[]
				{
					this.messageList.Count
				});
				IIMModality iimmodality = this.Conversation.GetModality(1) as IIMModality;
				if (iimmodality == null)
				{
					ExTraceGlobals.InstantMessagingTracer.TraceError((long)this.GetHashCode(), "InstantMessageQueue.SendAndClearMessageList. IIMModality is null.");
					return;
				}
				lock (this.lockObject)
				{
					foreach (Tuple<string, string> tuple in this.messageList)
					{
						iimmodality.BeginSendMessage(tuple.Item1, tuple.Item2, new AsyncCallback(this.SendMessageCallback), iimmodality);
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
				InstantMessagePayloadUtilities.GenerateMessageNotDeliveredPayload(this.notifier, "InstantMessageQueue.SendMessageCallback", (iimmodality == null || iimmodality.Conversation == null) ? 0 : iimmodality.Conversation.Cid, ex);
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

		private IUserContext userContext;

		private InstantMessageNotifier notifier;

		private List<Tuple<string, string>> messageList;

		private object lockObject = new object();
	}
}
