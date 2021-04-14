using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Runtime.Serialization;

namespace Microsoft.Exchange.Autodiscover.WCF
{
	[CollectionDataContract(Name = "OrganizationRelationshipSettingsCollection", ItemName = "OrganizationRelationshipSettings", Namespace = "http://schemas.microsoft.com/exchange/2010/Autodiscover")]
	public class OrganizationRelationshipSettingsCollection : Collection<OrganizationRelationshipSettings>
	{
		public OrganizationRelationshipSettingsCollection()
		{
		}

		public OrganizationRelationshipSettingsCollection(ICollection<OrganizationRelationshipSettings> settings)
		{
			foreach (OrganizationRelationshipSettings item in settings)
			{
				base.Add(item);
			}
		}
	}
}
