using System;
using System.CodeDom.Compiler;
using System.Diagnostics;
using System.Runtime.Serialization;

namespace Microsoft.Online.Administration
{
	[DataContract(Name = "DomainDnsMXRecord", Namespace = "http://schemas.datacontract.org/2004/07/Microsoft.Online.Administration")]
	[DebuggerStepThrough]
	[GeneratedCode("System.Runtime.Serialization", "4.0.0.0")]
	public class DomainDnsMXRecord : DomainDnsRecord
	{
		[DataMember]
		public string MailExchange
		{
			get
			{
				return this.MailExchangeField;
			}
			set
			{
				this.MailExchangeField = value;
			}
		}

		[DataMember]
		public int? Preference
		{
			get
			{
				return this.PreferenceField;
			}
			set
			{
				this.PreferenceField = value;
			}
		}

		private string MailExchangeField;

		private int? PreferenceField;
	}
}
