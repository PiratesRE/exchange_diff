using System;
using System.Net;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Data.QueueViewer
{
	internal sealed class MessageInfoSchema : PagedObjectSchema
	{
		internal override bool IsBasicField(int field)
		{
			return MessageInfoSchema.FieldDescriptors[field].isBasic;
		}

		internal override int GetFieldIndex(string fieldName)
		{
			int result;
			if (this.TryGetFieldIndex(fieldName, out result))
			{
				return result;
			}
			throw new QueueViewerException(QVErrorCode.QV_E_INVALID_FIELD_NAME);
		}

		internal override bool TryGetFieldIndex(string fieldName, out int index)
		{
			for (int i = 0; i < MessageInfoSchema.FieldDescriptors.Length; i++)
			{
				if (PagedObjectSchema.CompareString(MessageInfoSchema.FieldDescriptors[i].Name, fieldName) == 0)
				{
					index = i;
					return true;
				}
			}
			index = -1;
			return false;
		}

		internal override Type GetFieldType(int field)
		{
			return MessageInfoSchema.FieldDescriptors[field].Type;
		}

		internal override string GetFieldName(int field)
		{
			return MessageInfoSchema.FieldDescriptors[field].Name;
		}

		internal override ProviderPropertyDefinition GetFieldByName(string fieldName)
		{
			int num;
			if (this.TryGetFieldIndex(fieldName, out num))
			{
				return MessageInfoSchema.FieldDescriptors[num];
			}
			return null;
		}

		internal override bool MatchField(int field, PagedDataObject pagedDataObject, object matchPattern, MatchOptions matchOptions)
		{
			return MessageInfoSchema.FieldDescriptors[field].matcher((MessageInfo)pagedDataObject, matchPattern, matchOptions);
		}

		internal override int CompareField(int field, PagedDataObject pagedDataObject, object value)
		{
			return MessageInfoSchema.FieldDescriptors[field].comparer1((MessageInfo)pagedDataObject, value);
		}

		internal override int CompareField(int field, PagedDataObject object1, PagedDataObject object2)
		{
			return MessageInfoSchema.FieldDescriptors[field].comparer2((MessageInfo)object1, (MessageInfo)object2);
		}

		public static readonly QueueViewerPropertyDefinition<MessageInfo> Identity = new QueueViewerPropertyDefinition<MessageInfo>("Identity", typeof(MessageIdentity), MessageIdentity.Empty, false, (MessageInfo mi, object value) => MessageIdentity.Compare(mi.Identity, (ObjectId)value), (MessageInfo mi1, MessageInfo mi2) => MessageIdentity.Compare(mi1.Identity, mi2.Identity), (MessageInfo mi, object matchPattern, MatchOptions matchOptions) => ((MessageIdentity)mi.Identity).Match((MessageIdentity)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> Subject = new QueueViewerPropertyDefinition<MessageInfo>("Subject", typeof(string), string.Empty, true, (MessageInfo mi, object value) => PagedObjectSchema.CompareString(mi.Subject, (string)value), (MessageInfo mi1, MessageInfo mi2) => PagedObjectSchema.CompareString(mi1.Subject, mi2.Subject), (MessageInfo mi, object matchPattern, MatchOptions matchOptions) => PagedObjectSchema.MatchString(mi.Subject, (string)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> InternetMessageId = new QueueViewerPropertyDefinition<MessageInfo>("InternetMessageId", typeof(string), string.Empty, true, (MessageInfo mi, object value) => PagedObjectSchema.CompareString(mi.InternetMessageId, (string)value), (MessageInfo mi1, MessageInfo mi2) => PagedObjectSchema.CompareString(mi1.InternetMessageId, mi2.InternetMessageId), (MessageInfo mi, object matchPattern, MatchOptions matchOptions) => PagedObjectSchema.MatchString(mi.InternetMessageId, (string)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> FromAddress = new QueueViewerPropertyDefinition<MessageInfo>("FromAddress", typeof(string), string.Empty, true, (MessageInfo mi, object value) => PagedObjectSchema.CompareString(mi.FromAddress, (string)value), (MessageInfo mi1, MessageInfo mi2) => PagedObjectSchema.CompareString(mi1.FromAddress, mi2.FromAddress), (MessageInfo mi, object matchPattern, MatchOptions matchOptions) => PagedObjectSchema.MatchString(mi.FromAddress, (string)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> Status = new QueueViewerPropertyDefinition<MessageInfo>("Status", typeof(MessageStatus), MessageStatus.None, false, (MessageInfo mi, object value) => mi.Status.CompareTo(value), (MessageInfo mi1, MessageInfo mi2) => mi1.Status.CompareTo(mi2.Status));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> Size = new QueueViewerPropertyDefinition<MessageInfo>("Size", typeof(ByteQuantifiedSize), ByteQuantifiedSize.FromBytes(0UL), true, (MessageInfo mi, object value) => ((IComparable)mi.Size).CompareTo(value), (MessageInfo mi1, MessageInfo mi2) => mi1.Size.CompareTo(mi2.Size));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> MessageSourceName = new QueueViewerPropertyDefinition<MessageInfo>("MessageSourceName", typeof(string), string.Empty, true, (MessageInfo mi, object value) => PagedObjectSchema.CompareString(mi.MessageSourceName, (string)value), (MessageInfo mi1, MessageInfo mi2) => PagedObjectSchema.CompareString(mi1.MessageSourceName, mi2.MessageSourceName), (MessageInfo mi, object matchPattern, MatchOptions matchOptions) => PagedObjectSchema.MatchString(mi.MessageSourceName, (string)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> SourceIP = new QueueViewerPropertyDefinition<MessageInfo>("SourceIP", typeof(IPAddress), IPAddress.None, true, (MessageInfo mi, object value) => PagedObjectSchema.CompareIPAddress(mi.SourceIP, (IPAddress)value), (MessageInfo mi1, MessageInfo mi2) => PagedObjectSchema.CompareIPAddress(mi1.SourceIP, mi2.SourceIP));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> SCL = new QueueViewerPropertyDefinition<MessageInfo>("SCL", typeof(int), 0, true, (MessageInfo mi, object value) => mi.SCL.CompareTo(value), (MessageInfo mi1, MessageInfo mi2) => mi1.SCL.CompareTo(mi2.SCL));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> DateReceived = new QueueViewerPropertyDefinition<MessageInfo>("DateReceived", typeof(DateTime), DateTime.MinValue, true, (MessageInfo mi, object value) => mi.DateReceived.CompareTo(value), (MessageInfo mi1, MessageInfo mi2) => mi1.DateReceived.CompareTo(mi2.DateReceived));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> ExpirationTime = new QueueViewerPropertyDefinition<MessageInfo>("ExpirationTime", typeof(DateTime?), null, true, (MessageInfo mi, object value) => PagedObjectSchema.CompareDateTimeNullable(mi.ExpirationTime, (DateTime?)value), (MessageInfo mi1, MessageInfo mi2) => PagedObjectSchema.CompareDateTimeNullable(mi1.ExpirationTime, mi2.ExpirationTime));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> LastError = new QueueViewerPropertyDefinition<MessageInfo>("LastError", typeof(string), string.Empty, false, (MessageInfo mi, object value) => PagedObjectSchema.CompareString(mi.LastError, (string)value), (MessageInfo mi1, MessageInfo mi2) => PagedObjectSchema.CompareString(mi1.LastError, mi2.LastError), (MessageInfo mi, object matchPattern, MatchOptions matchOptions) => PagedObjectSchema.MatchString(mi.LastError, (string)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> RetryCount = new QueueViewerPropertyDefinition<MessageInfo>("RetryCount", typeof(int), 0, false, (MessageInfo mi, object value) => mi.RetryCount.CompareTo(value), (MessageInfo mi1, MessageInfo mi2) => mi1.RetryCount.CompareTo(mi2.RetryCount));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> Queue = new QueueViewerPropertyDefinition<MessageInfo>("Queue", typeof(QueueIdentity), QueueIdentity.Empty, false, (MessageInfo mi, object value) => QueueIdentity.Compare(mi.Queue, (ObjectId)value), (MessageInfo mi1, MessageInfo mi2) => QueueIdentity.Compare(mi1.Queue, mi2.Queue), (MessageInfo mi, object matchPattern, MatchOptions matchOptions) => mi.Queue.Match((QueueIdentity)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<MessageInfo> Recipients = new QueueViewerPropertyDefinition<MessageInfo>("Recipients", typeof(RecipientInfo[]), null, false, delegate(MessageInfo mi, object value)
		{
			throw new QueueViewerException(QVErrorCode.QV_E_COMPARISON_NOT_SUPPORTED);
		}, delegate(MessageInfo mi1, MessageInfo mi2)
		{
			throw new QueueViewerException(QVErrorCode.QV_E_COMPARISON_NOT_SUPPORTED);
		});

		internal static QueueViewerPropertyDefinition<MessageInfo>[] FieldDescriptors = new QueueViewerPropertyDefinition<MessageInfo>[]
		{
			MessageInfoSchema.Identity,
			MessageInfoSchema.Subject,
			MessageInfoSchema.InternetMessageId,
			MessageInfoSchema.FromAddress,
			MessageInfoSchema.Status,
			MessageInfoSchema.Size,
			MessageInfoSchema.MessageSourceName,
			MessageInfoSchema.SourceIP,
			MessageInfoSchema.SCL,
			MessageInfoSchema.DateReceived,
			MessageInfoSchema.ExpirationTime,
			MessageInfoSchema.LastError,
			MessageInfoSchema.RetryCount,
			MessageInfoSchema.Queue,
			MessageInfoSchema.Recipients
		};
	}
}
