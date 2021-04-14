using System;
using Microsoft.Exchange.Rpc.QueueViewer;

namespace Microsoft.Exchange.Data.QueueViewer
{
	internal sealed class QueueInfoSchema : PagedObjectSchema
	{
		internal override bool IsBasicField(int field)
		{
			return QueueInfoSchema.FieldDescriptors[field].isBasic;
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
				if (PagedObjectSchema.CompareString(QueueInfoSchema.FieldDescriptors[i].Name, fieldName) == 0)
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
			return QueueInfoSchema.FieldDescriptors[field].Type;
		}

		internal override string GetFieldName(int field)
		{
			return QueueInfoSchema.FieldDescriptors[field].Name;
		}

		internal override ProviderPropertyDefinition GetFieldByName(string fieldName)
		{
			int num;
			if (this.TryGetFieldIndex(fieldName, out num))
			{
				return QueueInfoSchema.FieldDescriptors[num];
			}
			return null;
		}

		internal override bool MatchField(int field, PagedDataObject pagedDataObject, object matchPattern, MatchOptions matchOptions)
		{
			return QueueInfoSchema.FieldDescriptors[field].matcher((QueueInfo)pagedDataObject, matchPattern, matchOptions);
		}

		internal override int CompareField(int field, PagedDataObject pagedDataObject, object value)
		{
			return QueueInfoSchema.FieldDescriptors[field].comparer1((QueueInfo)pagedDataObject, value);
		}

		internal override int CompareField(int field, PagedDataObject object1, PagedDataObject object2)
		{
			return QueueInfoSchema.FieldDescriptors[field].comparer2((QueueInfo)object1, (QueueInfo)object2);
		}

		public static readonly QueueViewerPropertyDefinition<QueueInfo> Identity = new QueueViewerPropertyDefinition<QueueInfo>("Identity", typeof(QueueIdentity), QueueIdentity.Empty, true, (QueueInfo qi, object value) => QueueIdentity.Compare(qi.Identity, (ObjectId)value), (QueueInfo qi1, QueueInfo qi2) => QueueIdentity.Compare(qi1.Identity, qi2.Identity), (QueueInfo qi, object matchPattern, MatchOptions matchOptions) => ((QueueIdentity)qi.Identity).Match((QueueIdentity)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> DeliveryType = new QueueViewerPropertyDefinition<QueueInfo>("DeliveryType", typeof(DeliveryType), Microsoft.Exchange.Data.DeliveryType.Undefined, true, (QueueInfo qi, object value) => qi.DeliveryType.CompareTo(value), (QueueInfo qi1, QueueInfo qi2) => qi1.DeliveryType.CompareTo(qi2.DeliveryType));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> NextHopDomain = new QueueViewerPropertyDefinition<QueueInfo>("NextHopDomain", typeof(string), string.Empty, true, (QueueInfo qi, object value) => PagedObjectSchema.CompareString(qi.NextHopDomain, (string)value), (QueueInfo qi1, QueueInfo qi2) => PagedObjectSchema.CompareString(qi1.NextHopDomain, qi2.NextHopDomain), (QueueInfo qi, object matchPattern, MatchOptions matchOptions) => PagedObjectSchema.MatchString(qi.NextHopDomain, (string)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> NextHopConnector = new QueueViewerPropertyDefinition<QueueInfo>("NextHopConnector", typeof(Guid), Guid.Empty, true, (QueueInfo qi, object value) => qi.NextHopConnector.CompareTo(value), (QueueInfo qi1, QueueInfo qi2) => qi1.NextHopConnector.CompareTo(qi2.NextHopConnector));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> Status = new QueueViewerPropertyDefinition<QueueInfo>("Status", typeof(QueueStatus), QueueStatus.None, true, (QueueInfo qi, object value) => qi.Status.CompareTo(value), (QueueInfo qi1, QueueInfo qi2) => qi1.Status.CompareTo(qi2.Status));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> MessageCount = new QueueViewerPropertyDefinition<QueueInfo>("MessageCount", typeof(int), 0, true, (QueueInfo qi, object value) => qi.MessageCount.CompareTo(value), (QueueInfo qi1, QueueInfo qi2) => qi1.MessageCount.CompareTo(qi2.MessageCount));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> LastError = new QueueViewerPropertyDefinition<QueueInfo>("LastError", typeof(string), string.Empty, true, (QueueInfo qi, object value) => PagedObjectSchema.CompareString(qi.LastError, (string)value), (QueueInfo qi1, QueueInfo qi2) => PagedObjectSchema.CompareString(qi1.LastError, qi2.LastError), (QueueInfo qi, object matchPattern, MatchOptions matchOptions) => PagedObjectSchema.MatchString(qi.LastError, (string)matchPattern, matchOptions));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> LastRetryTime = new QueueViewerPropertyDefinition<QueueInfo>("LastRetryTime", typeof(DateTime?), null, true, (QueueInfo qi, object value) => PagedObjectSchema.CompareDateTimeNullable(qi.LastRetryTime, (DateTime?)value), (QueueInfo qi1, QueueInfo qi2) => PagedObjectSchema.CompareDateTimeNullable(qi1.LastRetryTime, qi2.LastRetryTime));

		public static readonly QueueViewerPropertyDefinition<QueueInfo> NextRetryTime = new QueueViewerPropertyDefinition<QueueInfo>("NextRetryTime", typeof(DateTime?), null, true, (QueueInfo qi, object value) => PagedObjectSchema.CompareDateTimeNullable(qi.NextRetryTime, (DateTime?)value), (QueueInfo qi1, QueueInfo qi2) => PagedObjectSchema.CompareDateTimeNullable(qi1.NextRetryTime, qi2.NextRetryTime));

		private static QueueViewerPropertyDefinition<QueueInfo>[] FieldDescriptors = new QueueViewerPropertyDefinition<QueueInfo>[]
		{
			QueueInfoSchema.Identity,
			QueueInfoSchema.DeliveryType,
			QueueInfoSchema.NextHopDomain,
			QueueInfoSchema.NextHopConnector,
			QueueInfoSchema.Status,
			QueueInfoSchema.MessageCount,
			QueueInfoSchema.LastError,
			QueueInfoSchema.LastRetryTime,
			QueueInfoSchema.NextRetryTime
		};
	}
}
