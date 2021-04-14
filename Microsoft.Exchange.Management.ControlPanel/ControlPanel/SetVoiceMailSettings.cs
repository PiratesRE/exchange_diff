using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetVoiceMailSettings : SetVoiceMailBase
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-UMMailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Self";
			}
		}
	}
}
