using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Conversion;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.QueueViewer;
using Microsoft.Exchange.Rpc.QueueViewer;
using Microsoft.Exchange.Transport.Common;

namespace Microsoft.Exchange.Transport.QueueViewer
{
	internal class QueueViewerClient<ObjectType> : VersionedQueueViewerClient where ObjectType : PagedDataObject
	{
		public QueueViewerClient(string serverName) : base(serverName)
		{
		}

		public ObjectType[] GetPropertyBagBasedQueueViewerObjectPageCustomSerialization(QueueViewerInputObject inputObject, ref int totalCount, ref int pageOffset)
		{
			QVObjectType qvobjectType;
			if (typeof(ObjectType) == typeof(ExtensibleMessageInfo))
			{
				qvobjectType = QVObjectType.MessageInfo;
			}
			else
			{
				qvobjectType = QVObjectType.QueueInfo;
			}
			byte[] inputObjectBytes = Serialization.InputObjectToBytes(inputObject);
			byte[] propertyBagBasedQueueViewerObjectPageCustomSerialization = base.GetPropertyBagBasedQueueViewerObjectPageCustomSerialization(qvobjectType, inputObjectBytes, ref totalCount, ref pageOffset);
			ObjectType[] result;
			try
			{
				if (qvobjectType == QVObjectType.MessageInfo)
				{
					ExtensibleMessageInfo[] array = Serialization.BytesToPagedMessageObject(propertyBagBasedQueueViewerObjectPageCustomSerialization);
					result = (ObjectType[])array;
				}
				else
				{
					ExtensibleQueueInfo[] array2 = Serialization.BytesToPagedQueueObject(propertyBagBasedQueueViewerObjectPageCustomSerialization);
					result = (ObjectType[])array2;
				}
			}
			catch (SerializationException)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_SERVER_DATA);
			}
			return result;
		}

		public ObjectType[] GetPropertyBagBasedQueueViewerObjectPage(QueueViewerInputObject inputObject, ref int totalCount, ref int pageOffset)
		{
			QVObjectType objectType;
			if (typeof(ObjectType) == typeof(ExtensibleMessageInfo))
			{
				objectType = QVObjectType.MessageInfo;
			}
			else
			{
				objectType = QVObjectType.QueueInfo;
			}
			byte[] inputObjectBytes = Serialization.ObjectToBytes(inputObject);
			byte[] propertyBagBasedQueueViewerObjectPage = base.GetPropertyBagBasedQueueViewerObjectPage(objectType, inputObjectBytes, ref totalCount, ref pageOffset);
			ObjectType[] result;
			try
			{
				result = (ObjectType[])Serialization.BytesToObject(propertyBagBasedQueueViewerObjectPage);
			}
			catch (SerializationException)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_SERVER_DATA);
			}
			return result;
		}

		public ObjectType[] GetQueueViewerObjectPage(QueryFilter queryFilter, SortOrderEntry[] sortOrder, bool searchForward, int pageSize, PagedDataObject bookmarkObject, int bookmarkIndex, bool includeBookmark, bool includeDetails, bool includeComponentLatencyInfo, ref int totalCount, ref int pageOffset)
		{
			byte[] queryFilterBytes = Serialization.ObjectToBytes(queryFilter);
			byte[] sortOrderBytes = Serialization.ObjectToBytes(sortOrder);
			byte[] bookmarkObjectBytes = Serialization.ObjectToBytes(bookmarkObject);
			QVObjectType objectType;
			if (typeof(ObjectType) == typeof(ExtensibleMessageInfo))
			{
				objectType = QVObjectType.MessageInfo;
			}
			else
			{
				objectType = QVObjectType.QueueInfo;
			}
			MdbefPropertyCollection mdbefPropertyCollection = new MdbefPropertyCollection();
			TransportHelpers.AttemptAddToDictionary<uint, object>(mdbefPropertyCollection, 2483093515U, includeComponentLatencyInfo, null);
			byte[] queueViewerObjectPage = base.GetQueueViewerObjectPage(objectType, queryFilterBytes, sortOrderBytes, searchForward, pageSize, bookmarkObjectBytes, bookmarkIndex, includeBookmark, includeDetails, mdbefPropertyCollection.GetBytes(), ref totalCount, ref pageOffset);
			ObjectType[] result;
			try
			{
				if (typeof(ObjectType) == typeof(ExtensibleQueueInfo))
				{
					result = (this.GetExtensibleQueueInfo((QueueInfo[])Serialization.BytesToObject(queueViewerObjectPage)) as ObjectType[]);
				}
				else
				{
					result = (this.GetExtensibleMessageInfo((MessageInfo[])Serialization.BytesToObject(queueViewerObjectPage)) as ObjectType[]);
				}
			}
			catch (SerializationException)
			{
				throw new QueueViewerException(QVErrorCode.QV_E_INVALID_SERVER_DATA);
			}
			return result;
		}

		public void FreezeQueue(QueueIdentity queueIdentity, QueryFilter queryFilter)
		{
			byte[] queueIdentityBytes = Serialization.ObjectToBytes(queueIdentity);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(queryFilter);
			base.FreezeQueue(queueIdentityBytes, queryFilterBytes);
		}

		public void UnfreezeQueue(QueueIdentity queueIdentity, QueryFilter queryFilter)
		{
			byte[] queueIdentityBytes = Serialization.ObjectToBytes(queueIdentity);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(queryFilter);
			base.UnfreezeQueue(queueIdentityBytes, queryFilterBytes);
		}

		public void RetryQueue(QueueIdentity queueIdentity, QueryFilter queryFilter, bool resubmit)
		{
			byte[] queueIdentityBytes = Serialization.ObjectToBytes(queueIdentity);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(queryFilter);
			base.RetryQueue(queueIdentityBytes, queryFilterBytes, resubmit);
		}

		public byte[] ReadMessageBody(MessageIdentity mailItemId, int position, int count)
		{
			byte[] mailItemIdBytes = Serialization.ObjectToBytes(mailItemId);
			return base.ReadMessageBody(mailItemIdBytes, position, count);
		}

		public void FreezeMessage(MessageIdentity mailItemId, QueryFilter queryFilter)
		{
			byte[] mailItemIdBytes = Serialization.ObjectToBytes(mailItemId);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(queryFilter);
			base.FreezeMessage(mailItemIdBytes, queryFilterBytes);
		}

		public void UnfreezeMessage(MessageIdentity mailItemId, QueryFilter queryFilter)
		{
			byte[] mailItemIdBytes = Serialization.ObjectToBytes(mailItemId);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(queryFilter);
			base.UnfreezeMessage(mailItemIdBytes, queryFilterBytes);
		}

		public void DeleteMessage(MessageIdentity mailItemId, QueryFilter queryFilter, bool withNDR)
		{
			byte[] mailItemIdBytes = Serialization.ObjectToBytes(mailItemId);
			byte[] queryFilterBytes = Serialization.ObjectToBytes(queryFilter);
			base.DeleteMessage(mailItemIdBytes, queryFilterBytes, withNDR);
		}

		public void RedirectMessage(MultiValuedProperty<Fqdn> targetServers)
		{
			byte[] targetServersBytes = Serialization.ObjectToBytes(targetServers);
			base.RedirectMessage(targetServersBytes);
		}

		public void SetMessage(MessageIdentity mailItemId, QueryFilter queryFilter, ExtensibleMessageInfo properties, bool resubmit)
		{
			byte[] mailItemIdBytes = Serialization.MessageIdToBytes(mailItemId, Serialization.Version);
			byte[] queryFilterBytes = Serialization.FilterObjectToBytes(queryFilter);
			byte[] inputPropertiesBytes = Serialization.SingleMessageObjectToBytes(properties, Serialization.Version);
			base.SetMessage(mailItemIdBytes, queryFilterBytes, inputPropertiesBytes, resubmit);
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

		internal const uint InArgComponentLatencyInfo = 2483093515U;
	}
}
