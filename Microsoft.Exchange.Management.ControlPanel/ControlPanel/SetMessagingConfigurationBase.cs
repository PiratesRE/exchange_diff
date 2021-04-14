using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetMessagingConfigurationBase : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-MailboxMessageConfiguration";
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
