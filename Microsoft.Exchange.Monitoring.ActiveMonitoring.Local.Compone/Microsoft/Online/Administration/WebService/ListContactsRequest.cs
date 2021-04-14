using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ListContactsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ListContactsRequest : Request
	{
		[DataMember]
		public ContactSearchDefinition ContactSearchDefinition
		{
			get
			{
				return this.ContactSearchDefinitionField;
			}
			set
			{
				this.ContactSearchDefinitionField = value;
			}
		}

		private ContactSearchDefinition ContactSearchDefinitionField;
	}
}
