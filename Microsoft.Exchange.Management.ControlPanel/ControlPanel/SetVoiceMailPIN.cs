using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetVoiceMailPIN : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-UMMailboxPIN";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}

		[DataMember]
		public string PIN
		{
			get
			{
				return (string)base["Pin"];
			}
			set
			{
				base["Pin"] = value;
				base["SendEmail"] = false;
				base["PinExpired"] = false;
			}
		}
	}
}
