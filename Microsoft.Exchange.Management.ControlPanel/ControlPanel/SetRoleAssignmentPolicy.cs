using System;
using System.Collections.Generic;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class SetRoleAssignmentPolicy : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "Set-RoleAssignmentPolicy";
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
		public string Name
		{
			get
			{
				return (string)base[ADObjectSchema.Name];
			}
			set
			{
				base[ADObjectSchema.Name] = value;
			}
		}

		[DataMember]
		public string Description
		{
			get
			{
				return (string)base[RoleAssignmentPolicySchema.Description];
			}
			set
			{
				base[RoleAssignmentPolicySchema.Description] = value;
			}
		}

		[DataMember]
		public IEnumerable<Identity> AssignedEndUserRoles { get; set; }
	}
}
