using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "DocumentSharingLocationCollectionSetting", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class DocumentSharingLocationCollectionSetting : UserSetting
	{
		[DataMember(Name = "DocumentSharingLocations", IsRequired = true)]
		public DocumentSharingLocationCollection DocumentSharingLocations { get; set; }
	}
}
