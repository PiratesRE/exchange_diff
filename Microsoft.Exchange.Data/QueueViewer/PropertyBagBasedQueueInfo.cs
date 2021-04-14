using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class PropertyBagBasedQueueInfo : ExtensibleQueueInfo
	{
		internal PropertyBagBasedQueueInfo(QueueIdentity identity) : base(new QueueInfoPropertyBag())
		{
			this[this.propertyBag.ObjectIdentityPropertyDefinition] = identity;
			this.NextHopDomain = identity.NextHopDomain;
			this.propertyBag[ExtensibleQueueInfoSchema.PriorityDescriptions] = base.PriorityDescriptions;
		}

		private PropertyBagBasedQueueInfo(PropertyStreamReader reader) : base(new QueueInfoPropertyBag())
		{
			KeyValuePair<string, object> item;
			reader.Read(out item);
			if (!string.Equals("NumProperties", item.Key, StringComparison.OrdinalIgnoreCase))
			{
				throw new SerializationException(string.Format("Cannot deserialize PropertyBagBasedQueueInfo. Expected property NumProperties, but found '{0}'", item.Key));
			}
			int value = PropertyStreamReader.GetValue<int>(item);
			for (int i = 0; i < value; i++)
			{
				reader.Read(out item);
				if (string.Equals(ExtensibleQueueInfoSchema.Identity.Name, item.Key, StringComparison.OrdinalIgnoreCase))
				{
					QueueIdentity value2 = QueueIdentity.Parse(PropertyStreamReader.GetValue<string>(item));
					this[this.propertyBag.ObjectIdentityPropertyDefinition] = value2;
				}
				else if (string.Equals(ExtensibleQueueInfoSchema.DeliveryType.Name, item.Key, StringComparison.OrdinalIgnoreCase))
				{
					DeliveryType deliveryType = (DeliveryType)PropertyStreamReader.GetValue<int>(item);
					this.propertyBag[ExtensibleQueueInfoSchema.DeliveryType] = deliveryType;
				}
				else if (string.Equals(ExtensibleQueueInfoSchema.Status.Name, item.Key, StringComparison.OrdinalIgnoreCase))
				{
					QueueStatus value3 = (QueueStatus)PropertyStreamReader.GetValue<int>(item);
					this.propertyBag[ExtensibleQueueInfoSchema.Status] = value3;
				}
				else if (string.Equals(ExtensibleQueueInfoSchema.RiskLevel.Name, item.Key, StringComparison.OrdinalIgnoreCase))
				{
					RiskLevel value4 = (RiskLevel)PropertyStreamReader.GetValue<int>(item);
					this.propertyBag[ExtensibleQueueInfoSchema.RiskLevel] = value4;
				}
				else if (string.Equals(ExtensibleQueueInfoSchema.NextHopCategory.Name, item.Key, StringComparison.OrdinalIgnoreCase))
				{
					NextHopCategory value5 = (NextHopCategory)PropertyStreamReader.GetValue<int>(item);
					this.propertyBag[ExtensibleQueueInfoSchema.NextHopCategory] = value5;
				}
				else
				{
					PropertyDefinition fieldByName = PropertyBagBasedQueueInfo.schema.GetFieldByName(item.Key);
					if (fieldByName != null)
					{
						this.propertyBag.SetField((QueueViewerPropertyDefinition<ExtensibleQueueInfo>)fieldByName, item.Value);
					}
					else
					{
						ExTraceGlobals.SerializationTracer.TraceWarning<string>(0L, "Cannot convert key index '{0}' into a property in the ExtensibleQueueInfo schema", item.Key);
					}
				}
			}
			if (this.propertyBag[ExtensibleQueueInfoSchema.NextHopDomain] != null)
			{
				QueueIdentity queueIdentity = (QueueIdentity)this[this.propertyBag.ObjectIdentityPropertyDefinition];
				queueIdentity.NextHopDomain = (string)this.propertyBag[ExtensibleQueueInfoSchema.NextHopDomain];
			}
		}

		public override bool IsDeliveryQueue()
		{
			return base.QueueIdentity.Type == QueueType.Delivery;
		}

		public override bool IsSubmissionQueue()
		{
			return base.QueueIdentity.Type == QueueType.Submission;
		}

		public override bool IsPoisonQueue()
		{
			return base.QueueIdentity.Type == QueueType.Poison;
		}

		public override bool IsShadowQueue()
		{
			return base.QueueIdentity.Type == QueueType.Shadow;
		}

		public override DeliveryType DeliveryType
		{
			get
			{
				return (DeliveryType)this.propertyBag[ExtensibleQueueInfoSchema.DeliveryType];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.DeliveryType] = value;
			}
		}

		public override string NextHopDomain
		{
			get
			{
				return (string)this.propertyBag[ExtensibleQueueInfoSchema.NextHopDomain];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.NextHopDomain] = value;
			}
		}

		public override string TlsDomain
		{
			get
			{
				return (string)this.propertyBag[ExtensibleQueueInfoSchema.TlsDomain];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.TlsDomain] = value;
			}
		}

		public override Guid NextHopConnector
		{
			get
			{
				return (Guid)this.propertyBag[ExtensibleQueueInfoSchema.NextHopConnector];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.NextHopConnector] = value;
			}
		}

		public override QueueStatus Status
		{
			get
			{
				return (QueueStatus)this.propertyBag[ExtensibleQueueInfoSchema.Status];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.Status] = value;
			}
		}

		public override int MessageCount
		{
			get
			{
				return (int)this.propertyBag[ExtensibleQueueInfoSchema.MessageCount];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.MessageCount] = value;
			}
		}

		public override string LastError
		{
			get
			{
				return (string)this.propertyBag[ExtensibleQueueInfoSchema.LastError];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.LastError] = value;
			}
		}

		public override int RetryCount
		{
			get
			{
				return (int)this.propertyBag[ExtensibleQueueInfoSchema.RetryCount];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.RetryCount] = value;
			}
		}

		public override DateTime? LastRetryTime
		{
			get
			{
				return (DateTime?)this.propertyBag[ExtensibleQueueInfoSchema.LastRetryTime];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.LastRetryTime] = value;
			}
		}

		public override DateTime? NextRetryTime
		{
			get
			{
				return (DateTime?)this.propertyBag[ExtensibleQueueInfoSchema.NextRetryTime];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.NextRetryTime] = value;
			}
		}

		public override DateTime? FirstRetryTime
		{
			get
			{
				return (DateTime?)this.propertyBag[ExtensibleQueueInfoSchema.FirstRetryTime];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.FirstRetryTime] = value;
			}
		}

		public override int DeferredMessageCount
		{
			get
			{
				return (int)this.propertyBag[ExtensibleQueueInfoSchema.DeferredMessageCount];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.DeferredMessageCount] = value;
			}
		}

		public override int LockedMessageCount
		{
			get
			{
				return (int)this.propertyBag[ExtensibleQueueInfoSchema.LockedMessageCount];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.LockedMessageCount] = value;
			}
		}

		public override int[] MessageCountsPerPriority
		{
			get
			{
				return (int[])this.propertyBag[ExtensibleQueueInfoSchema.MessageCountsPerPriority];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.MessageCountsPerPriority] = value;
			}
		}

		public override int[] DeferredMessageCountsPerPriority
		{
			get
			{
				return (int[])this.propertyBag[ExtensibleQueueInfoSchema.DeferredMessageCountsPerPriority];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.DeferredMessageCountsPerPriority] = value;
			}
		}

		public override RiskLevel RiskLevel
		{
			get
			{
				return (RiskLevel)this.propertyBag[ExtensibleQueueInfoSchema.RiskLevel];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.RiskLevel] = value;
			}
		}

		public override int OutboundIPPool
		{
			get
			{
				return (int)this.propertyBag[ExtensibleQueueInfoSchema.OutboundIPPool];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.OutboundIPPool] = value;
			}
		}

		public override NextHopCategory NextHopCategory
		{
			get
			{
				return (NextHopCategory)this.propertyBag[ExtensibleQueueInfoSchema.NextHopCategory];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.NextHopCategory] = value;
			}
		}

		public override double IncomingRate
		{
			get
			{
				return (double)this.propertyBag[ExtensibleQueueInfoSchema.IncomingRate];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.IncomingRate] = value;
			}
		}

		public override double OutgoingRate
		{
			get
			{
				return (double)this.propertyBag[ExtensibleQueueInfoSchema.OutgoingRate];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.OutgoingRate] = value;
			}
		}

		public override double Velocity
		{
			get
			{
				return (double)this.propertyBag[ExtensibleQueueInfoSchema.Velocity];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.Velocity] = value;
			}
		}

		public override string OverrideSource
		{
			get
			{
				return (string)this.propertyBag[ExtensibleQueueInfoSchema.OverrideSource];
			}
			internal set
			{
				this.propertyBag[ExtensibleQueueInfoSchema.OverrideSource] = value;
			}
		}

		internal static PropertyBagBasedQueueInfo CreateFromByteStream(PropertyStreamReader reader)
		{
			return new PropertyBagBasedQueueInfo(reader);
		}

		internal void ToByteArray(ref byte[] bytes, ref int offset)
		{
			bool flag = this.DeferredMessageCountsPerPriority != null && this.DeferredMessageCountsPerPriority.Length > 0;
			bool flag2 = this.MessageCountsPerPriority != null && this.MessageCountsPerPriority.Length > 0;
			int num = 17 + ((this.LastRetryTime != null) ? 1 : 0) + ((this.NextRetryTime != null) ? 1 : 0) + ((this.FirstRetryTime != null) ? 1 : 0) + (flag ? 1 : 0) + (flag2 ? 1 : 0) + ((flag2 || flag) ? 1 : 0) + (string.IsNullOrEmpty(this.OverrideSource) ? 0 : 1);
			int num2 = 0;
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, num, ref bytes, ref offset);
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.Identity.Name, StreamPropertyType.String, this.Identity.ToString(), ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.DeliveryType.Name, StreamPropertyType.Int32, (int)this.DeliveryType, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.NextHopDomain.Name, StreamPropertyType.String, this.NextHopDomain, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.NextHopConnector.Name, StreamPropertyType.Guid, this.NextHopConnector, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.Status.Name, StreamPropertyType.Int32, (int)this.Status, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.MessageCount.Name, StreamPropertyType.Int32, this.MessageCount, ref bytes, ref offset);
			num2++;
			if (flag2)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.MessageCountsPerPriority.Name, StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array, this.MessageCountsPerPriority, ref bytes, ref offset);
				num2++;
			}
			if (flag || flag2)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.PriorityDescriptions.Name, StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.UInt32 | StreamPropertyType.Array, base.PriorityDescriptions, ref bytes, ref offset);
				num2++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.LastError.Name, StreamPropertyType.String, this.LastError, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.RetryCount.Name, StreamPropertyType.Int32, this.RetryCount, ref bytes, ref offset);
			num2++;
			if (this.LastRetryTime != null)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.LastRetryTime.Name, StreamPropertyType.DateTime, this.LastRetryTime.Value, ref bytes, ref offset);
				num2++;
			}
			if (this.NextRetryTime != null)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.NextRetryTime.Name, StreamPropertyType.DateTime, this.NextRetryTime.Value, ref bytes, ref offset);
				num2++;
			}
			if (this.FirstRetryTime != null)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.FirstRetryTime.Name, StreamPropertyType.DateTime, this.FirstRetryTime.Value, ref bytes, ref offset);
				num2++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.TlsDomain.Name, StreamPropertyType.String, this.TlsDomain, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.DeferredMessageCount.Name, StreamPropertyType.Int32, this.DeferredMessageCount, ref bytes, ref offset);
			num2++;
			if (flag)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.DeferredMessageCountsPerPriority.Name, StreamPropertyType.Null | StreamPropertyType.Bool | StreamPropertyType.SByte | StreamPropertyType.Array, this.DeferredMessageCountsPerPriority, ref bytes, ref offset);
				num2++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.LockedMessageCount.Name, StreamPropertyType.Int32, this.LockedMessageCount, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.RiskLevel.Name, StreamPropertyType.Int32, (int)this.RiskLevel, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.OutboundIPPool.Name, StreamPropertyType.Int32, this.OutboundIPPool, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.NextHopCategory.Name, StreamPropertyType.Int32, (int)this.NextHopCategory, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.IncomingRate.Name, StreamPropertyType.Double, this.IncomingRate, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.OutgoingRate.Name, StreamPropertyType.Double, this.OutgoingRate, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.Velocity.Name, StreamPropertyType.Double, this.Velocity, ref bytes, ref offset);
			num2++;
			if (!string.IsNullOrEmpty(this.OverrideSource))
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleQueueInfoSchema.OverrideSource.Name, StreamPropertyType.String, this.OverrideSource, ref bytes, ref offset);
				num2++;
			}
		}

		private const string NumPropertiesKey = "NumProperties";

		private static ExtensibleQueueInfoSchema schema = ObjectSchema.GetInstance<ExtensibleQueueInfoSchema>();
	}
}
