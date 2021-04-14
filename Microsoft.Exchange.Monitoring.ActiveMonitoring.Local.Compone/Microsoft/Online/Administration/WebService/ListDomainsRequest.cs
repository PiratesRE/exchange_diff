using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ListDomainsRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	[DebuggerStepThrough]
	public class ListDomainsRequest : Request
	{
		[DataMember]
		public DomainSearchFilter SearchFilter
		{
			get
			{
				return this.SearchFilterField;
			}
			set
			{
				this.SearchFilterField = value;
			}
		}

		private DomainSearchFilter SearchFilterField;
	}
}
