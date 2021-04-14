using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration.WebService
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "ListAccountSkusRequest", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration.WebService")]
	public class ListAccountSkusRequest : Request
	{
		[DataMember]
		public Guid? AccountId
		{
			get
			{
				return this.AccountIdField;
			}
			set
			{
				this.AccountIdField = value;
			}
		}

		private Guid? AccountIdField;
	}
}
