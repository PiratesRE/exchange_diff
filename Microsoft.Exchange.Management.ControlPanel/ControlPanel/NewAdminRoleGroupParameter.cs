using System;
using System.Runtime.Serialization;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Management.RbacTasks;

namespace Microsoft.Exchange.Management.ControlPanel
{
	[DataContract]
	public class NewAdminRoleGroupParameter : BaseRoleGroupParameters
	{
		[DataMember]
		public Identity[] Members
		{
			get
			{
				return (Identity[])base[ADGroupSchema.Members];
			}
			set
			{
				base[ADGroupSchema.Members] = value.ToIdParameters();
			}
		}

		[DataMember]
		public Identity[] Roles
		{
			get
			{
				return (Identity[])base[ADGroupSchema.Roles];
			}
			set
			{
				base[ADGroupSchema.Roles] = value.ToIdParameters();
			}
		}

		[DataMember]
		internal Identity RecipientWriteScope
		{
			get
			{
				return (Identity)base[RbacCommonParameters.ParameterCustomRecipientWriteScope];
			}
			set
			{
				base[RbacCommonParameters.ParameterCustomRecipientWriteScope] = value.ToIdParameter();
			}
		}

		[DataMember]
		internal Identity ConfigWriteScope
		{
			get
			{
				return (Identity)base[RbacCommonParameters.ParameterCustomConfigWriteScope];
			}
			set
			{
				base[RbacCommonParameters.ParameterCustomConfigWriteScope] = value.ToIdParameter();
			}
		}

		[DataMember]
		internal Identity RecipientOrganizationalUnitScope
		{
			get
			{
				return (Identity)base[RbacCommonParameters.ParameterRecipientOrganizationalUnitScope];
			}
			set
			{
				base[RbacCommonParameters.ParameterRecipientOrganizationalUnitScope] = value.ToIdParameter();
			}
		}

		public bool IsRolesModified
		{
			get
			{
				return this.Roles != null;
			}
		}

		public override string AssociatedCmdlet
		{
			get
			{
				return "New-RoleGroup";
			}
		}

		[OnDeserialized]
		private void OnDeserialized(StreamingContext context)
		{
			if (this.Roles != null && this.Roles.Length < 1)
			{
				base[ADGroupSchema.Roles] = null;
			}
			if (this.Members != null && this.Members.Length < 1)
			{
				base[ADGroupSchema.Members] = null;
			}
		}
	}
}
