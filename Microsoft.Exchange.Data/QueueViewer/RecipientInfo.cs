using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class RecipientInfo
	{
		internal RecipientInfo()
		{
		}

		private RecipientInfo(PropertyStreamReader reader)
		{
			KeyValuePair<string, object> item;
			reader.Read(out item);
			if (!string.Equals("NumProperties", item.Key, StringComparison.OrdinalIgnoreCase))
			{
				throw new SerializationException(string.Format("Cannot deserialize RecipientInfo. Expected property NumProperties, but found property '{0}'", item.Key));
			}
			int value = PropertyStreamReader.GetValue<int>(item);
			for (int i = 0; i < value; i++)
			{
				reader.Read(out item);
				if (string.Equals("Address", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.address = PropertyStreamReader.GetValue<string>(item);
				}
				else if (string.Equals("Status", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.status = (RecipientStatus)PropertyStreamReader.GetValue<int>(item);
				}
				else if (string.Equals("LastError", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.lastError = PropertyStreamReader.GetValue<string>(item);
				}
				else if (string.Equals("Type", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.type = (MailRecipientType)PropertyStreamReader.GetValue<int>(item);
				}
				else if (string.Equals("LastErrorCode", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.lastErrorCode = PropertyStreamReader.GetValue<int>(item);
				}
				else if (string.Equals("FinalDestination", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.finalDestination = PropertyStreamReader.GetValue<string>(item);
				}
				else if (string.Equals("OutboundIPPool", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.outboundIPPool = PropertyStreamReader.GetValue<int>(item);
				}
				else
				{
					ExTraceGlobals.SerializationTracer.TraceWarning<string>(0L, "Ignoring unknown property '{0} in recipientInfo", item.Key);
				}
			}
		}

		public string Address
		{
			get
			{
				return this.address;
			}
			internal set
			{
				this.address = value;
			}
		}

		public int OutboundIPPool
		{
			get
			{
				return this.outboundIPPool;
			}
			internal set
			{
				this.outboundIPPool = value;
			}
		}

		public MailRecipientType Type
		{
			get
			{
				return this.type;
			}
			internal set
			{
				this.type = value;
			}
		}

		public string FinalDestination
		{
			get
			{
				return this.finalDestination;
			}
			internal set
			{
				this.finalDestination = value;
			}
		}

		public RecipientStatus Status
		{
			get
			{
				return this.status;
			}
			internal set
			{
				this.status = value;
			}
		}

		internal int LastErrorCode
		{
			get
			{
				return this.lastErrorCode;
			}
			set
			{
				this.lastErrorCode = value;
			}
		}

		public string LastError
		{
			get
			{
				if (this.lastError != null)
				{
					return this.lastError;
				}
				if (this.lastErrorCode != 0)
				{
					return StatusCodeConverter.UnreachableReasonToString((UnreachableReason)this.lastErrorCode);
				}
				return null;
			}
			internal set
			{
				this.lastError = value;
			}
		}

		public override string ToString()
		{
			return string.Format("{0};{1};{2};{3};{4};{5};{6}", new object[]
			{
				this.address,
				(int)this.status,
				(int)this.type,
				this.lastError,
				this.lastErrorCode,
				this.finalDestination,
				this.outboundIPPool
			});
		}

		internal static RecipientInfo Create(PropertyStreamReader reader)
		{
			return new RecipientInfo(reader);
		}

		internal void ToByteArray(ref byte[] bytes, ref int offset)
		{
			int num = 6 + ((this.OutboundIPPool > 0) ? 1 : 0);
			int num2 = 0;
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, num, ref bytes, ref offset);
			PropertyStreamWriter.WritePropertyKeyValue("Address", StreamPropertyType.String, this.address, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue("Status", StreamPropertyType.Int32, (int)this.Status, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue("Type", StreamPropertyType.Int32, (int)this.Type, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue("LastError", StreamPropertyType.String, this.LastError, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue("LastErrorCode", StreamPropertyType.Int32, this.LastErrorCode, ref bytes, ref offset);
			num2++;
			PropertyStreamWriter.WritePropertyKeyValue("FinalDestination", StreamPropertyType.String, this.FinalDestination, ref bytes, ref offset);
			num2++;
			if (this.outboundIPPool > 0)
			{
				PropertyStreamWriter.WritePropertyKeyValue("OutboundIPPool", StreamPropertyType.Int32, this.outboundIPPool, ref bytes, ref offset);
				num2++;
			}
		}

		private const string NumPropertiesKey = "NumProperties";

		private const string AddressKey = "Address";

		private const string StatusKey = "Status";

		private const string TypeKey = "Type";

		private const string LastErrorKey = "LastError";

		private const string LastErrorCodeKey = "LastErrorCode";

		private const string FinalDestinationKey = "FinalDestination";

		private const string OutboundIPPoolKey = "OutboundIPPool";

		private string address;

		private RecipientStatus status;

		private MailRecipientType type;

		private string finalDestination;

		private string lastError;

		private int lastErrorCode;

		private int outboundIPPool;
	}
}
