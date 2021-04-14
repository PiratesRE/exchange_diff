using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public sealed class User
	{
		[DataMember(IsRequired = true)]
		public string Mailbox { get; set; }
	}
}
