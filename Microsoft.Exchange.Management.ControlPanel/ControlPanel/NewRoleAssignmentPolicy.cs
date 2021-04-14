using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewRoleAssignmentPolicy : SetObjectProperties
	{
		public override string AssociatedCmdlet
		{
			get
			{
				return "New-RoleAssignmentPolicy";
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
		public IEnumerable<Identity> Roles
		{
			get
			{
				return (IEnumerable<Identity>)base["Roles"];
			}
			set
			{
				if (value != null && value.Count<Identity>() > 0)
				{
					base["Roles"] = value;
				}
			}
		}
	}
}
