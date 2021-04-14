using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class GetImapSubscription : SelfMailboxParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-ImapSubscription";
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
