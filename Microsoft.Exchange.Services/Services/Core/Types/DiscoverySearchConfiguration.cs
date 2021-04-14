using System;
using System.Runtime.Serialization;
using System.Xml.Serialization;

namespace Microsoft.Exchange.Services.Core.Types
{
	[DataContract(Name = "DiscoverySearchConfiguration", Namespace = "http://schemas.datacontract.org/2004/07/Exchange")]
	[XmlType(TypeName = "DiscoverySearchConfigurationType", Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
	[Serializable]
	public class DiscoverySearchConfiguration
	{
		public DiscoverySearchConfiguration()
		{
		}

		internal DiscoverySearchConfiguration(string id, SearchableMailbox[] mailboxes, string query, string inPlaceHoldIdentity, string managedByOrganization, string language)
		{
			this.SearchId = id;
			this.SearchableMailboxes = mailboxes;
			this.SearchQuery = query;
			this.InPlaceHoldIdentity = inPlaceHoldIdentity;
			if (!string.IsNullOrEmpty(managedByOrganization))
			{
				this.ManagedByOrganization = managedByOrganization;
			}
			if (!string.IsNullOrEmpty(language))
			{
				this.Language = language;
			}
		}

		[XmlElement("SearchId")]
		[DataMember(Name = "SearchId", IsRequired = true)]
		public string SearchId { get; set; }

		[DataMember(Name = "SearchQuery", IsRequired = true)]
		[XmlElement("SearchQuery")]
		public string SearchQuery { get; set; }

		[DataMember(Name = "SearchableMailbox", IsRequired = false)]
		[XmlArray]
		[XmlArrayItem("SearchableMailbox", Type = typeof(SearchableMailbox), Namespace = "http://schemas.microsoft.com/exchange/services/2006/types")]
		public SearchableMailbox[] SearchableMailboxes { get; set; }

		[DataMember(Name = "InPlaceHoldIdentity", IsRequired = false)]
		[XmlElement("InPlaceHoldIdentity")]
		public string InPlaceHoldIdentity { get; set; }

		[DataMember(Name = "ManagedByOrganization", IsRequired = false)]
		[XmlElement("ManagedByOrganization")]
		public string ManagedByOrganization { get; set; }

		[XmlElement("Language")]
		[DataMember(Name = "Language", IsRequired = false)]
		public string Language { get; set; }
	}
}
