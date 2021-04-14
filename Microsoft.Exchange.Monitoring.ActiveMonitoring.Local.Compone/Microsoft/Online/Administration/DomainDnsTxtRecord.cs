using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	[DataContract(Name = "DomainDnsTxtRecord", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	public class DomainDnsTxtRecord : DomainDnsRecord
	{
		[DataMember]
		public string Text
		{
			get
			{
				return this.TextField;
			}
			set
			{
				this.TextField = value;
			}
		}

		private string TextField;
	}
}
