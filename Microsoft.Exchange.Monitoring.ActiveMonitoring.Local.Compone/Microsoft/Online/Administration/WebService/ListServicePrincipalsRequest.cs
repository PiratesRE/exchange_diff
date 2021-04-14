using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DebuggerStepThrough]
	[DataContract(Name = "ListServicePrincipalsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ListServicePrincipalsRequest : Request
	{
		[DataMember]
		public ServicePrincipalSearchDefinition SearchDefinition
		{
			get
			{
				return this.SearchDefinitionField;
			}
			set
			{
				this.SearchDefinitionField = value;
			}
		}

		private ServicePrincipalSearchDefinition SearchDefinitionField;
	}
}
