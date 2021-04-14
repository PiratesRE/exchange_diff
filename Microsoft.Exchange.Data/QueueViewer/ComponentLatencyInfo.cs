using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Diagnostics.Components.Data;

namespace Microsoft.Exchange.Data.QueueViewer
{
	[Serializable]
	public class ComponentLatencyInfo
	{
		public ComponentLatencyInfo(LocalizedString componentName, EnhancedTimeSpan componentLatency, int componentSequenceNumber, bool isPending)
		{
			this.componentName = componentName;
			this.componentLatency = componentLatency;
			this.isPending = isPending;
			this.componentSequenceNumber = componentSequenceNumber;
		}

		private ComponentLatencyInfo(PropertyStreamReader reader)
		{
			KeyValuePair<string, object> item;
			reader.Read(out item);
			if (!string.Equals("NumProperties", item.Key, StringComparison.OrdinalIgnoreCase))
			{
				throw new SerializationException(string.Format("Cannot deserialize ComponentLatencyInfo. Expected property NumProperties, but found property '{0}'", item.Key));
			}
			int value = PropertyStreamReader.GetValue<int>(item);
			for (int i = 0; i < value; i++)
			{
				reader.Read(out item);
				if (string.Equals("Name", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.componentName = new LocalizedString(PropertyStreamReader.GetValue<string>(item));
				}
				else if (string.Equals("Latency", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.componentLatency = EnhancedTimeSpan.Parse(PropertyStreamReader.GetValue<string>(item));
				}
				else if (string.Equals("SequenceNumber", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.componentSequenceNumber = PropertyStreamReader.GetValue<int>(item);
				}
				else if (string.Equals("Pending", item.Key, StringComparison.OrdinalIgnoreCase))
				{
					this.isPending = PropertyStreamReader.GetValue<bool>(item);
				}
				else
				{
					ExTraceGlobals.SerializationTracer.TraceWarning<string>(0L, "Ignoring unknown property '{0} in ComponentLatencyInfo", item.Key);
				}
			}
		}

		public int ComponentSequenceNumber
		{
			get
			{
				return this.componentSequenceNumber;
			}
		}

		public LocalizedString ComponentName
		{
			get
			{
				return this.componentName;
			}
		}

		public bool IsPending
		{
			get
			{
				return this.isPending;
			}
		}

		public EnhancedTimeSpan ComponentLatency
		{
			get
			{
				return this.componentLatency;
			}
		}

		public override string ToString()
		{
			return string.Concat(new object[]
			{
				this.componentSequenceNumber,
				";",
				this.componentName,
				";",
				this.isPending,
				";",
				this.componentLatency
			});
		}

		internal static ComponentLatencyInfo Create(PropertyStreamReader reader)
		{
			return new ComponentLatencyInfo(reader);
		}

		internal void ToByteArray(ref byte[] bytes, ref int offset)
		{
			int num = 0;
			PropertyStreamWriter.WritePropertyKeyValue("NumProperties", StreamPropertyType.Int32, 4, ref bytes, ref offset);
			PropertyStreamWriter.WritePropertyKeyValue("Name", StreamPropertyType.String, this.componentName.ToString(), ref bytes, ref offset);
			num++;
			PropertyStreamWriter.WritePropertyKeyValue("Latency", StreamPropertyType.String, this.componentLatency.ToString(), ref bytes, ref offset);
			num++;
			PropertyStreamWriter.WritePropertyKeyValue("SequenceNumber", StreamPropertyType.Int32, this.componentSequenceNumber, ref bytes, ref offset);
			num++;
			PropertyStreamWriter.WritePropertyKeyValue("Pending", StreamPropertyType.Bool, this.isPending, ref bytes, ref offset);
			num++;
		}

		private const string NumPropertiesKey = "NumProperties";

		private const string ComponentSequenceNumberKey = "SequenceNumber";

		private const string ComponentNameKey = "Name";

		private const string ComponentLatencyKey = "Latency";

		private const string PendingKey = "Pending";

		private int componentSequenceNumber;

		private LocalizedString componentName;

		private EnhancedTimeSpan componentLatency;

		private bool isPending;
	}
}
