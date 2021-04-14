using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Services.OnlineMeetings.ResourceContract
{
	[DataContract(Name = "Telemetry")]
	internal class Telemetry : Resource
	{
		public Telemetry(string selfUri) : base(selfUri)
		{
		}

		[DataMember(Name = "StartTime", EmitDefaultValue = false)]
		public DateTime StartTime
		{
			get
			{
				return base.GetValue<DateTime>("StartTime");
			}
			set
			{
				base.SetValue<DateTime>("StartTime", value);
			}
		}

		[DataMember(Name = "EndTime", EmitDefaultValue = false)]
		public DateTime EndTime
		{
			get
			{
				return base.GetValue<DateTime>("EndTime");
			}
			set
			{
				base.SetValue<DateTime>("EndTime", value);
			}
		}
	}
}
