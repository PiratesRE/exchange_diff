using System;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;

namespace Microsoft.Exchange.Management.Hybrid.Entity
{
	internal class OnPremisesOrganization : IOnPremisesOrganization, IEntity<IOnPremisesOrganization>
	{
		public OnPremisesOrganization()
		{
		}

		public OnPremisesOrganization(Guid organizationGuid, string organizationName, MultiValuedProperty<SmtpDomain> hybridDomains, ADObjectId inboundConnector, ADObjectId outboundConnector, string name, ADObjectId organizationRelationship)
		{
			this.OrganizationGuid = organizationGuid;
			this.OrganizationName = organizationName;
			this.HybridDomains = hybridDomains;
			this.InboundConnector = inboundConnector;
			this.OutboundConnector = outboundConnector;
			this.Name = name;
			this.OrganizationRelationship = organizationRelationship;
		}

		public ADObjectId Identity { get; set; }

		public Guid OrganizationGuid { get; set; }

		public string OrganizationName { get; set; }

		public MultiValuedProperty<SmtpDomain> HybridDomains { get; set; }

		public ADObjectId InboundConnector { get; set; }

		public ADObjectId OutboundConnector { get; set; }

		public string Name { get; set; }

		public ADObjectId OrganizationRelationship { get; set; }

		public override string ToString()
		{
			if (this.Identity != null)
			{
				return this.Identity.ToString();
			}
			return "<New>";
		}

		public bool Equals(IOnPremisesOrganization obj)
		{
			return false;
		}

		public IOnPremisesOrganization Clone(ADObjectId identity)
		{
			OnPremisesOrganization onPremisesOrganization = new OnPremisesOrganization();
			onPremisesOrganization.UpdateFrom(this);
			onPremisesOrganization.Identity = identity;
			return onPremisesOrganization;
		}

		public void UpdateFrom(IOnPremisesOrganization obj)
		{
			this.OrganizationGuid = obj.OrganizationGuid;
			this.OrganizationName = obj.OrganizationName;
			this.HybridDomains = obj.HybridDomains;
			this.InboundConnector = obj.InboundConnector;
			this.OutboundConnector = obj.OutboundConnector;
			this.Name = obj.Name;
			this.OrganizationRelationship = obj.OrganizationRelationship;
		}
	}
}
