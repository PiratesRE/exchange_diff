using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "AddGroupMembersRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class AddGroupMembersRequest : Request
	{
		[DataMember]
		public GroupMember[] GroupMembers
		{
			get
			{
				return this.GroupMembersField;
			}
			set
			{
				this.GroupMembersField = value;
			}
		}

		[DataMember]
		public Guid GroupObjectId
		{
			get
			{
				return this.GroupObjectIdField;
			}
			set
			{
				this.GroupObjectIdField = value;
			}
		}

		private GroupMember[] GroupMembersField;

		private Guid GroupObjectIdField;
	}
}
