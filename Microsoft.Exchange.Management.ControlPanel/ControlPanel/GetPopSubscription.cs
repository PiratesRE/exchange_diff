using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class GetPopSubscription : SelfMailboxParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-PopSubscription";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Self";
			}
		}
	}
}
