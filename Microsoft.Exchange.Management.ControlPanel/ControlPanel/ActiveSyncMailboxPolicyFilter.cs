using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ActiveSyncMailboxPolicyFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-MobileMailboxPolicy";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}
	}
}
