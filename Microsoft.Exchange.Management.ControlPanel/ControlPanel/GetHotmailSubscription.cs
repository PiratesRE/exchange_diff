using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class GetHotmailSubscription : SelfMailboxParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-HotmailSubscription";
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
