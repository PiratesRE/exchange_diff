using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class GetOrganizationRelationshipSettingsRequest : AutodiscoverRequest
	{
		[DataMember(Name = "Domains", IsRequired = true)]
		public DomainCollection Domains { get; set; }
	}
}
