using System;
using System.Collections.Generic;
using System.Net;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class PropertyBagBasedMessageInfo : ExtensibleMessageInfo
	{
		internal PropertyBagBasedMessageInfo(long identity, QueueIdentity queueIdentity) : base(identity, queueIdentity, new MessageInfoPropertyBag())
		{
		}

		private PropertyBagBasedMessageInfo(PropertyStreamReader reader, Version sourceVersion) : base(new MessageInfoPropertyBag())
		{
			KeyValuePair<string, object> keyValuePair;
			reader.Read(out keyValuePair);
			if (!string.Equals("NumProperties", keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
			{
				throw new SerializationException(string.Format("Cannot deserialize PropertyBagBasedMessageInfo. Expected property NumProperties, but found property '{0}'", keyValuePair.Key));
			}
			int value = PropertyStreamReader.GetValue<int>(keyValuePair);
			for (int i = 0; i < value; i++)
			{
				reader.Read(out keyValuePair);
				if (string.Equals(ExtensibleMessageInfoSchema.Identity.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					MessageIdentity value2 = MessageIdentity.Create(sourceVersion, keyValuePair, reader);
					this.propertyBag[ExtensibleMessageInfoSchema.Identity] = value2;
				}
				else if (string.Equals(ExtensibleMessageInfoSchema.Status.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					MessageStatus value3 = (MessageStatus)PropertyStreamReader.GetValue<int>(keyValuePair);
					this.propertyBag[ExtensibleMessageInfoSchema.Status] = value3;
				}
				else if (string.Equals(ExtensibleMessageInfoSchema.Size.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					ByteQuantifiedSize byteQuantifiedSize = new ByteQuantifiedSize(PropertyStreamReader.GetValue<ulong>(keyValuePair));
					this.propertyBag[ExtensibleMessageInfoSchema.Size] = byteQuantifiedSize;
				}
				else if (string.Equals(ExtensibleMessageInfoSchema.MessageLatency.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					EnhancedTimeSpan enhancedTimeSpan = EnhancedTimeSpan.Parse(PropertyStreamReader.GetValue<string>(keyValuePair));
					this.propertyBag[ExtensibleMessageInfoSchema.MessageLatency] = enhancedTimeSpan;
				}
				else if (string.Equals(ExtensibleMessageInfoSchema.ExternalDirectoryOrganizationId.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					Guid value4 = PropertyStreamReader.GetValue<Guid>(keyValuePair);
					this.propertyBag[ExtensibleMessageInfoSchema.ExternalDirectoryOrganizationId] = value4;
				}
				else if (string.Equals(ExtensibleMessageInfoSchema.Directionality.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					MailDirectionality value5 = (MailDirectionality)PropertyStreamReader.GetValue<int>(keyValuePair);
					this.propertyBag[ExtensibleMessageInfoSchema.Directionality] = value5;
				}
				else if (string.Equals(ExtensibleMessageInfoSchema.Recipients.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					int value6 = PropertyStreamReader.GetValue<int>(keyValuePair);
					RecipientInfo[] array = new RecipientInfo[value6];
					for (int j = 0; j < value6; j++)
					{
						RecipientInfo recipientInfo = RecipientInfo.Create(reader);
						array[j] = recipientInfo;
					}
					this.propertyBag[ExtensibleMessageInfoSchema.Recipients] = array;
				}
				else if (string.Equals(ExtensibleMessageInfoSchema.ComponentLatency.Name, keyValuePair.Key, StringComparison.OrdinalIgnoreCase))
				{
					int value7 = PropertyStreamReader.GetValue<int>(keyValuePair);
					ComponentLatencyInfo[] array2 = new ComponentLatencyInfo[value7];
					for (int k = 0; k < value7; k++)
					{
						ComponentLatencyInfo componentLatencyInfo = ComponentLatencyInfo.Create(reader);
						array2[k] = componentLatencyInfo;
					}
					this.propertyBag[ExtensibleMessageInfoSchema.ComponentLatency] = array2;
				}
				else
				{
					PropertyDefinition fieldByName = PropertyBagBasedMessageInfo.schema.GetFieldByName(keyValuePair.Key);
					if (fieldByName != null)
					{
						this.propertyBag.SetField((QueueViewerPropertyDefinition<ExtensibleMessageInfo>)fieldByName, keyValuePair.Value);
					}
					else
					{
						ExTraceGlobals.SerializationTracer.TraceWarning<string>(0L, "Cannot convert key index '{0}' into a property in the ExtensibleMessageInfo schema", keyValuePair.Key);
					}
				}
			}
		}

		public override string Subject
		{
			get
			{
				return (string)this[ExtensibleMessageInfoSchema.Subject];
			}
			internal set
			{
				this[ExtensibleMessageInfoSchema.Subject] = value;
			}
		}

		public override string InternetMessageId
		{
			get
			{
				return (string)this.propertyBag[ExtensibleMessageInfoSchema.InternetMessageId];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.InternetMessageId] = value;
			}
		}

		public override string FromAddress
		{
			get
			{
				return (string)this[ExtensibleMessageInfoSchema.FromAddress];
			}
			internal set
			{
				this[ExtensibleMessageInfoSchema.FromAddress] = value;
			}
		}

		public override MessageStatus Status
		{
			get
			{
				return (MessageStatus)this.propertyBag[ExtensibleMessageInfoSchema.Status];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.Status] = value;
			}
		}

		public override ByteQuantifiedSize Size
		{
			get
			{
				return (ByteQuantifiedSize)this.propertyBag[ExtensibleMessageInfoSchema.Size];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.Size] = value;
			}
		}

		public override string MessageSourceName
		{
			get
			{
				return (string)this.propertyBag[ExtensibleMessageInfoSchema.MessageSourceName];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.MessageSourceName] = value;
			}
		}

		public override IPAddress SourceIP
		{
			get
			{
				return (IPAddress)this.propertyBag[ExtensibleMessageInfoSchema.SourceIP];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.SourceIP] = value;
			}
		}

		public override int SCL
		{
			get
			{
				return (int)this.propertyBag[ExtensibleMessageInfoSchema.SCL];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.SCL] = value;
			}
		}

		public override DateTime DateReceived
		{
			get
			{
				return (DateTime)this.propertyBag[ExtensibleMessageInfoSchema.DateReceived];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.DateReceived] = value;
			}
		}

		public override DateTime? ExpirationTime
		{
			get
			{
				return (DateTime?)this.propertyBag[ExtensibleMessageInfoSchema.ExpirationTime];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.ExpirationTime] = value;
			}
		}

		internal override int LastErrorCode
		{
			get
			{
				return (int)this.propertyBag[ExtensibleMessageInfoSchema.LastErrorCode];
			}
			set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.LastErrorCode] = value;
			}
		}

		public override string LastError
		{
			get
			{
				string text = (string)this.propertyBag[ExtensibleMessageInfoSchema.LastError];
				if (text != null)
				{
					return text;
				}
				if (base.Queue.Type == QueueType.Unreachable)
				{
					return StatusCodeConverter.UnreachableReasonToString((UnreachableReason)this.LastErrorCode);
				}
				if (base.Queue.Type == QueueType.Submission)
				{
					return StatusCodeConverter.DeferReasonToString((DeferReason)this.LastErrorCode);
				}
				return null;
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.LastError] = value;
			}
		}

		public override int RetryCount
		{
			get
			{
				return (int)this.propertyBag[ExtensibleMessageInfoSchema.RetryCount];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.RetryCount] = value;
			}
		}

		public override RecipientInfo[] Recipients
		{
			get
			{
				return (RecipientInfo[])this[ExtensibleMessageInfoSchema.Recipients];
			}
			internal set
			{
				this[ExtensibleMessageInfoSchema.Recipients] = value;
			}
		}

		public override ComponentLatencyInfo[] ComponentLatency
		{
			get
			{
				return (ComponentLatencyInfo[])this.propertyBag[ExtensibleMessageInfoSchema.ComponentLatency];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.ComponentLatency] = value;
			}
		}

		public override EnhancedTimeSpan MessageLatency
		{
			get
			{
				return (EnhancedTimeSpan)this.propertyBag[ExtensibleMessageInfoSchema.MessageLatency];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.MessageLatency] = value;
			}
		}

		public override string DeferReason
		{
			get
			{
				return (string)this.propertyBag[ExtensibleMessageInfoSchema.DeferReason];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.DeferReason] = value;
			}
		}

		public override string LockReason
		{
			get
			{
				return (string)this.propertyBag[ExtensibleMessageInfoSchema.LockReason];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.LockReason] = value;
			}
		}

		internal override bool IsProbeMessage
		{
			get
			{
				return (bool)this.propertyBag[ExtensibleMessageInfoSchema.IsProbeMessage];
			}
			set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.IsProbeMessage] = value;
			}
		}

		public override string Priority
		{
			get
			{
				return (string)this.propertyBag[ExtensibleMessageInfoSchema.Priority];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.Priority] = value;
			}
		}

		public override Guid ExternalDirectoryOrganizationId
		{
			get
			{
				return (Guid)this.propertyBag[ExtensibleMessageInfoSchema.ExternalDirectoryOrganizationId];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.ExternalDirectoryOrganizationId] = value;
			}
		}

		public override MailDirectionality Directionality
		{
			get
			{
				return (MailDirectionality)this.propertyBag[ExtensibleMessageInfoSchema.Directionality];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.Directionality] = value;
			}
		}

		public override string OriginalFromAddress
		{
			get
			{
				return (string)this.propertyBag[ExtensibleMessageInfoSchema.OriginalFromAddress];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.OriginalFromAddress] = value;
			}
		}

		public override string AccountForest
		{
			get
			{
				return (string)this.propertyBag[ExtensibleMessageInfoSchema.AccountForest];
			}
			internal set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.AccountForest] = value;
			}
		}

		internal override int OutboundIPPool
		{
			get
			{
				return (int)this.propertyBag[ExtensibleMessageInfoSchema.OutboundIPPool];
			}
			set
			{
				this.propertyBag[ExtensibleMessageInfoSchema.OutboundIPPool] = value;
			}
		}

		internal static PropertyBagBasedMessageInfo CreateFromByteStream(PropertyStreamReader reader, Version sourceVersion)
		{
			return new PropertyBagBasedMessageInfo(reader, sourceVersion);
		}

		internal void ToByteArray(Version targetVersion, ref byte[] bytes, ref int offset)
		{
			int num = 17 + ((!string.IsNullOrEmpty(this.Subject)) ? 1 : 0) + ((this.SourceIP != null) ? 1 : 0) + ((this.ExpirationTime != null) ? 1 : 0) + ((this.DateReceived != DateTime.MinValue) ? 1 : 0);
			int num2;
			if (this.propertyBag.Contains(ExtensibleMessageInfoSchema.MessageLatency))
			{
				EnhancedTimeSpan messageLatency = this.MessageLatency;
				num2 = 1;
			}
			else
			{
				num2 = 0;
			}
			int num3 = num + num2 + ((!string.IsNullOrEmpty(this.LastError)) ? 1 : 0) + ((this.Recipients != null && this.Recipients.Length > 0) ? 1 : 0) + ((this.ComponentLatency != null && this.ComponentLatency.Length > 0) ? 1 : 0) + ((this.OutboundIPPool > 0) ? 1 : 0) + ((this.ExternalDirectoryOrganizationId != Guid.Empty) ? 1 : 0);
			int num4 = 0;
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, num3, ref bytes, ref offset);
			num4++;
			base.MessageIdentity.ToByteArray(targetVersion, ref bytes, ref offset);
			num4++;
			if (!string.IsNullOrEmpty(this.Subject))
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Subject.Name, StreamPropertyType.String, this.Subject, ref bytes, ref offset);
				num4++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.InternetMessageId.Name, StreamPropertyType.String, this.InternetMessageId, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.FromAddress.Name, StreamPropertyType.String, this.FromAddress, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Status.Name, StreamPropertyType.Int32, (int)this.Status, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Size.Name, StreamPropertyType.UInt64, this.Size.ToBytes(), ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.MessageSourceName.Name, StreamPropertyType.String, this.MessageSourceName, ref bytes, ref offset);
			num4++;
			if (this.SourceIP != null)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.SourceIP.Name, StreamPropertyType.IPAddress, this.SourceIP, ref bytes, ref offset);
				num4++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.SCL.Name, StreamPropertyType.Int32, this.SCL, ref bytes, ref offset);
			num4++;
			if (this.DateReceived != DateTime.MinValue)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.DateReceived.Name, StreamPropertyType.DateTime, this.DateReceived, ref bytes, ref offset);
				num4++;
			}
			if (this.ExpirationTime != null)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.ExpirationTime.Name, StreamPropertyType.DateTime, this.ExpirationTime, ref bytes, ref offset);
				num4++;
			}
			if (!string.IsNullOrEmpty(this.LastError))
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.LastError.Name, StreamPropertyType.String, this.LastError, ref bytes, ref offset);
				num4++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.LastErrorCode.Name, StreamPropertyType.Int32, this.LastErrorCode, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.RetryCount.Name, StreamPropertyType.Int32, this.RetryCount, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.MessageSourceName.Name, StreamPropertyType.String, this.MessageSourceName, ref bytes, ref offset);
			num4++;
			if (this.propertyBag.Contains(ExtensibleMessageInfoSchema.MessageLatency))
			{
				EnhancedTimeSpan messageLatency2 = this.MessageLatency;
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.MessageLatency.Name, StreamPropertyType.String, this.MessageLatency.ToString(), ref bytes, ref offset);
				num4++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.LockReason.Name, StreamPropertyType.String, this.LockReason, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.IsProbeMessage.Name, StreamPropertyType.Bool, this.IsProbeMessage, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.DeferReason.Name, StreamPropertyType.String, this.DeferReason, ref bytes, ref offset);
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Priority.Name, StreamPropertyType.String, this.Priority, ref bytes, ref offset);
			num4++;
			if (this.OutboundIPPool > 0)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.OutboundIPPool.Name, StreamPropertyType.Int32, this.OutboundIPPool, ref bytes, ref offset);
				num4++;
			}
			if (this.ExternalDirectoryOrganizationId != Guid.Empty)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.ExternalDirectoryOrganizationId.Name, StreamPropertyType.Guid, this.ExternalDirectoryOrganizationId, ref bytes, ref offset);
				num4++;
			}
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Directionality.Name, StreamPropertyType.Int32, this.Directionality, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.OriginalFromAddress.Name, StreamPropertyType.String, this.OriginalFromAddress, ref bytes, ref offset);
			num4++;
			PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.AccountForest.Name, StreamPropertyType.String, this.AccountForest, ref bytes, ref offset);
			num4++;
			if (this.Recipients != null && this.Recipients.Length > 0)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.Recipients.Name, StreamPropertyType.Int32, this.Recipients.Length, ref bytes, ref offset);
				foreach (RecipientInfo recipientInfo in this.Recipients)
				{
					recipientInfo.ToByteArray(ref bytes, ref offset);
				}
				num4++;
			}
			if (this.ComponentLatency != null && this.ComponentLatency.Length > 0)
			{
				PropertyStreamWriter.WritePropertyKeyValue(ExtensibleMessageInfoSchema.ComponentLatency.Name, StreamPropertyType.Int32, this.ComponentLatency.Length, ref bytes, ref offset);
				foreach (ComponentLatencyInfo componentLatencyInfo in this.ComponentLatency)
				{
					componentLatencyInfo.ToByteArray(ref bytes, ref offset);
				}
				num4++;
			}
		}

		private const string NumPropertiesKey = "NumProperties";

		private static ExtensibleMessageInfoSchema schema = ObjectSchema.GetInstance<ExtensibleMessageInfoSchema>();
	}
}
