using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class RoleAssignmentPolicyFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-RoleAssignmentPolicy";
			}
		}

		public override string RbacScope
		{
			get
			{
				return "@R:Organization";
			}
		}

		[DataMember]
		public Identity Policy { get; set; }
	}
}
