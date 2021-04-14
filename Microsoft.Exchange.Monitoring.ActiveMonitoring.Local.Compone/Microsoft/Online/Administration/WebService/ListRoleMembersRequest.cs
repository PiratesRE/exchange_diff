using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ListRoleMembersRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ListRoleMembersRequest : Request
	{
		[DataMember]
		public RoleMemberSearchDefinition RoleMemberSearchDefinition
		{
			get
			{
				return this.RoleMemberSearchDefinitionField;
			}
			set
			{
				this.RoleMemberSearchDefinitionField = value;
			}
		}

		private RoleMemberSearchDefinition RoleMemberSearchDefinitionField;
	}
}
