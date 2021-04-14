using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class UMEnableSelectedPolicyParameters : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Enable-UMMailbox";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@W:Organization";
			}
		}

		[DataMember]
		public Identity UMMailboxPolicy
		{
			get
			{
				return (Identity)base["UMMailboxPolicy"];
			}
			set
			{
				base["UMMailboxPolicy"] = value;
			}
		}
	}
}
