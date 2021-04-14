using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class ManagementRoleFilter : WebServiceParameters
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Get-ManagementRole";
			}
		}

		[DataMember]
		public string RoleType
		{
			get
			{
				return (string)base["RoleType"];
			}
			set
			{
				base["RoleType"] = value;
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

		internal const string GetCmdlet = "Get-ManagementRole";
	}
}
