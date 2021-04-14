using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Data.Serialization;
using Microsoft.Exchange.Data.Transport.Smtp;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal class QueueClient : QueueViewerRpcClient
	{
		public QueueClient(string serverName, DirectoryContext directoryContext) : base(serverName)
		{
			this.serverName = serverName;
			this.directoryContext = directoryContext;
		}

		public bool GetRecipientStatus(long internalId, string messageId, string recipientAddress, out RecipientStatus? status, out string lastError, out DateTime? lastRetry, out DateTime? nextRetry, out string queueName, out bool isUnreachable)
		{
			status = null;
			lastError = null;
			lastRetry = null;
			nextRetry = null;
			queueName = null;
			isUnreachable = true;
			ExtensibleMessageInfo[] messageInfo = this.GetMessageInfo(internalId);
			if (messageInfo == null || messageInfo.Length == 0)
			{
				TraceWrapper.SearchLibraryTracer.TraceDebug<long>(this.GetHashCode(), "No results found for message internal ID: {0}", internalId);
				return false;
			}
			ExtensibleMessageInfo extensibleMessageInfo = null;
			for (int i = 0; i < messageInfo.Length; i++)
			{
				string internetMessageId = messageInfo[i].InternetMessageId;
				if (string.IsNullOrEmpty(internetMessageId) || !internetMessageId.Equals(messageId) || messageInfo[i].Recipients == null)
				{
					TraceWrapper.SearchLibraryTracer.TraceError<string, string>(this.GetHashCode(), "Message-IDs did not match, Task: {0}, Queues: {1}", messageId, internetMessageId);
				}
				else
				{
					foreach (RecipientInfo recipientInfo in messageInfo[i].Recipients)
					{
						if (recipientInfo.Address.Equals(recipientAddress))
						{
							lastError = recipientInfo.LastError;
							status = new RecipientStatus?(recipientInfo.Status);
							extensibleMessageInfo = messageInfo[i];
							break;
						}
					}
				}
			}
			if (extensibleMessageInfo == null)
			{
				TraceWrapper.SearchLibraryTracer.TraceError(this.GetHashCode(), "Message did not match", new object[0]);
				return false;
			}
			if (string.IsNullOrEmpty(lastError))
			{
				lastError = extensibleMessageInfo.LastError;
			}
			QueueIdentity queue = extensibleMessageInfo.Queue;
			ExtensibleQueueInfo[] queueInfo = this.GetQueueInfo(queue);
			if (queueInfo == null || queueInfo.Length != 1)
			{
				TraceWrapper.SearchLibraryTracer.TraceError<QueueIdentity>(this.GetHashCode(), "No queue found with ID: {0}", queue);
				return false;
			}
			queueName = queueInfo[0].Identity.ToString();
			isUnreachable = (queueInfo[0].DeliveryType == DeliveryType.Unreachable);
			SmtpResponse smtpResponse;
			if (string.IsNullOrEmpty(lastError) || (SmtpResponse.TryParse(lastError, out smtpResponse) && smtpResponse.SmtpResponseType == SmtpResponseType.Success))
			{
				lastError = queueInfo[0].LastError;
			}
			if (status == null && (queueInfo[0].Status == QueueStatus.Retry || queueInfo[0].Status == QueueStatus.Suspended))
			{
				status = new RecipientStatus?(RecipientStatus.Retry);
			}
			lastRetry = queueInfo[0].LastRetryTime;
			nextRetry = queueInfo[0].NextRetryTime;
			return true;
		}

		private ExtensibleMessageInfo[] GetMessageInfo(long internalId)
		{
			TextFilter obj = new TextFilter(MessageInfoSchema.Identity, new MessageIdentity(internalId).ToString(), MatchOptions.FullString, MatchFlags.Default);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(obj);
			byte[] sortOrder = Serialization.ObjectToBytes(QueueClient.emptySortOrder);
			int num = 0;
			int num2 = 0;
			Exception ex = null;
			try
			{
				byte[] queueViewerObjectPage = base.GetQueueViewerObjectPage(QVObjectType.MessageInfo, queryFilterBytes, sortOrder, true, int.MaxValue, null, -1, false, true, true, null, ref num, ref num2);
				return this.GetExtensibleMessageInfo((MessageInfo[])Serialization.BytesToObject(queueViewerObjectPage));
			}
			catch (RpcException ex2)
			{
				ex = ex2;
			}
			catch (SerializationException ex3)
			{
				ex = ex3;
			}
			TraceWrapper.SearchLibraryTracer.TraceError<Exception>(this.GetHashCode(), "RPC or Deserialize exception: {0}", ex);
			TrackingTransientException.AddAndRaiseETX(this.directoryContext.Errors, ErrorCode.QueueViewerConnectionFailure, this.serverName, ex.ToString());
			return null;
		}

		private ExtensibleQueueInfo[] GetQueueInfo(QueueIdentity queueId)
		{
			ComparisonFilter obj = new ComparisonFilter(ComparisonOperator.Equal, QueueInfoSchema.Identity, queueId);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(obj);
			byte[] sortOrder = Serialization.ObjectToBytes(QueueClient.emptySortOrder);
			int num = 0;
			int num2 = 0;
			Exception ex = null;
			try
			{
				byte[] queueViewerObjectPage = base.GetQueueViewerObjectPage(QVObjectType.QueueInfo, queryFilterBytes, sortOrder, true, int.MaxValue, null, -1, false, false, true, null, ref num, ref num2);
				return this.GetExtensibleQueueInfo((QueueInfo[])Serialization.BytesToObject(queueViewerObjectPage));
			}
			catch (RpcException ex2)
			{
				ex = ex2;
			}
			catch (SerializationException ex3)
			{
				ex = ex3;
			}
			TraceWrapper.SearchLibraryTracer.TraceError<Exception>(this.GetHashCode(), "RPC or Deserialize exception: {0}", ex);
			TrackingTransientException.AddAndRaiseETX(this.directoryContext.Errors, ErrorCode.QueueViewerConnectionFailure, this.serverName, ex.ToString());
			return null;
		}

		private ExtensibleMessageInfo[] GetExtensibleMessageInfo(MessageInfo[] legacyMessageInfoCollection)
		{
			List<ExtensibleMessageInfo> list = new List<ExtensibleMessageInfo>();
			foreach (MessageInfo messageInfo in legacyMessageInfoCollection)
			{
				MessageIdentity messageIdentity = (MessageIdentity)messageInfo.Identity;
				list.Add(new PropertyBagBasedMessageInfo(messageIdentity.InternalId, messageIdentity.QueueIdentity)
				{
					Subject = messageInfo.Subject,
					InternetMessageId = messageInfo.InternetMessageId,
					FromAddress = messageInfo.FromAddress,
					Status = messageInfo.Status,
					Size = messageInfo.Size,
					MessageSourceName = messageInfo.MessageSourceName,
					SourceIP = messageInfo.SourceIP,
					SCL = messageInfo.SCL,
					DateReceived = messageInfo.DateReceived,
					ExpirationTime = messageInfo.ExpirationTime,
					LastErrorCode = messageInfo.LastErrorCode,
					LastError = messageInfo.LastError,
					RetryCount = messageInfo.RetryCount,
					Recipients = messageInfo.Recipients,
					ComponentLatency = messageInfo.ComponentLatency,
					MessageLatency = messageInfo.MessageLatency
				});
			}
			return list.ToArray();
		}

		private ExtensibleQueueInfo[] GetExtensibleQueueInfo(QueueInfo[] legacyQueueInfoCollection)
		{
			List<ExtensibleQueueInfo> list = new List<ExtensibleQueueInfo>();
			foreach (QueueInfo queueInfo in legacyQueueInfoCollection)
			{
				list.Add(new PropertyBagBasedQueueInfo((QueueIdentity)queueInfo.Identity)
				{
					DeliveryType = queueInfo.DeliveryType,
					NextHopConnector = queueInfo.NextHopConnector,
					Status = queueInfo.Status,
					MessageCount = queueInfo.MessageCount,
					LastError = queueInfo.LastError,
					LastRetryTime = queueInfo.LastRetryTime,
					NextRetryTime = queueInfo.NextRetryTime
				});
			}
			return list.ToArray();
		}

		private static SortOrderEntry[] emptySortOrder = new SortOrderEntry[0];

		private string serverName;

		private DirectoryContext directoryContext;
	}
}
