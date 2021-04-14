using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "AlternateMailboxCollectionSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class AlternateMailboxCollectionSetting : UserSetting
	{
		[DataMember(Name = "AlternateMailboxes", IsRequired = true)]
		public AlternateMailboxCollection AlternateMailboxes { get; set; }
	}
}
