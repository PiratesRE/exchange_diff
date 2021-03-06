using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "RemoveRoleMembersRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	public class RemoveRoleMembersRequest : Request
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
		public Guid RoleObjectId
		{
			get
			{
				return this.RoleObjectIdField;
			}
			set
			{
				this.RoleObjectIdField = value;
			}
		}

		private RoleMember[] RoleMembersField;

		private Guid RoleObjectIdField;
	}
}
