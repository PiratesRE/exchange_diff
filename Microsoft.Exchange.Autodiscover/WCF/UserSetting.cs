using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[KnownType(typeof(AlternateMailboxCollectionSetting))]
	[KnownType(typeof(WebClientUrlCollectionSetting))]
	[KnownType(typeof(StringSetting))]
	[KnownType(typeof(ProtocolConnectionCollectionSetting))]
	[KnownType(typeof(DocumentSharingLocationCollectionSetting))]
	[DataContract(Name = "UserSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public abstract class UserSetting
	{
		public UserSetting()
		{
		}

		[DataMember(IsRequired = true)]
		public string Name { get; set; }
	}
}
