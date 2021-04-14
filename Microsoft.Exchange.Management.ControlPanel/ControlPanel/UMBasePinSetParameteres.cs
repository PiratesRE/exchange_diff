using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[KnownType(typeof(NewUMMailboxParameters))]
	[DataContract]
	[KnownType(typeof(SetUMMailboxPinParameters))]
	public abstract class UMBasePinSetParameteres : SetObjectProperties
	{
		[DataMember]
		public string AutoPin { get; set; }

		[DataMember]
		public string ManualPin { get; set; }

		[DataMember]
		public bool? PinExpired
		{
			get
			{
				return (bool?)base["PinExpired"];
			}
			set
			{
				base["PinExpired"] = value;
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			bool flag;
			if (bool.TryParse(this.AutoPin, out flag))
			{
				if (flag)
				{
					base["Pin"] = null;
					return;
				}
				this.ManualPin.FaultIfNullOrEmpty(Strings.UMManualPin);
				base["Pin"] = this.ManualPin;
			}
		}
	}
}
