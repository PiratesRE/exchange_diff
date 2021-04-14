using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "RemoveGroupMembersRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	public class RemoveGroupMembersRequest : Request
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
