using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ListUsersRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ListUsersRequest : Request
	{
		[DataMember]
		public UserSearchDefinition UserSearchDefinition
		{
			get
			{
				return this.UserSearchDefinitionField;
			}
			set
			{
				this.UserSearchDefinitionField = value;
			}
		}

		private UserSearchDefinition UserSearchDefinitionField;
	}
}
