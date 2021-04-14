using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetHotmailSubscription : PimSubscriptionParameter
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-HotmailSubscription";
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
