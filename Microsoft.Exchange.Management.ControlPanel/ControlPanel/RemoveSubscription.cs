using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RemoveSubscription : SelfMailboxParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Remove-Subscription";
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
