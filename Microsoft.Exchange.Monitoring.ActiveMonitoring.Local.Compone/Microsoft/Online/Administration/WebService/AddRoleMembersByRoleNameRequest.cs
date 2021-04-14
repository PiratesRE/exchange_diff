using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "AddRoleMembersByRoleNameRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class AddRoleMembersByRoleNameRequest : Request
	{
		[DataMember]
		public RoleMember[] RoleMembers
		{
			get
			{
				return this.RoleMembersField;
			}
			set
			{
				this.RoleMembersField = value;
			}
		}

		[DataMember]
		public string RoleName
		{
			get
			{
				return this.RoleNameField;
			}
			set
			{
				this.RoleNameField = value;
			}
		}

		private RoleMember[] RoleMembersField;

		private string RoleNameField;
	}
}
