using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DataContract(Name = "ListGroupsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class ListGroupsRequest : Request
	{
		[DataMember]
		public GroupSearchDefinition GroupSearchDefinition
		{
			get
			{
				return this.GroupSearchDefinitionField;
			}
			set
			{
				this.GroupSearchDefinitionField = value;
			}
		}

		private GroupSearchDefinition GroupSearchDefinitionField;
	}
}
