using System;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[DataContract(Name = "GetOrganizationRelationshipSettingsResponse", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class GetOrganizationRelationshipSettingsResponse : AutodiscoverResponse
	{
		[DataMember(Name = "OrganizationRelationshipSettingsCollection", IsRequired = false)]
		public OrganizationRelationshipSettingsCollection OrganizationRelationships { get; set; }
	}
}
