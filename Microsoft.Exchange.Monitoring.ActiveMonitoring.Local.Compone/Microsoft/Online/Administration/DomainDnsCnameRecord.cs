using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "DomainDnsCnameRecord", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class DomainDnsCnameRecord : DomainDnsRecord
	{
		[DataMember]
		public string CanonicalName
		{
			get
			{
				return this.CanonicalNameField;
			}
			set
			{
				this.CanonicalNameField = value;
			}
		}

		private string CanonicalNameField;
	}
}
