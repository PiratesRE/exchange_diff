using System;
using Microsoft.Exchange.Data.Transport;

namespace Microsoft.Exchange.Data.Directory.SystemConfiguration
{
	internal class AcceptedDomainEntry : AcceptedDomain, DomainMatchMap<AcceptedDomainEntry>.IDomainEntry
	{
		public AcceptedDomainEntry(AcceptedDomain domain, OrganizationId organizationId)
		{
			this.name = domain.Name;
			this.domain = domain.DomainName;
			this.DomainType = domain.DomainType;
			this.MatchSubDomains = domain.MatchSubDomains;
			this.IsDefault = domain.Default;
			this.AddressBookEnabled = domain.AddressBookEnabled;
			this.Guid = domain.Guid;
			this.syncedToPerimeterAsGuidDomain = (domain.PerimeterDuplicateDetected || domain.OutboundOnly);
			if (domain.CatchAllRecipientID != null)
			{
				this.catchAllRecipientID = domain.CatchAllRecipientID.ObjectGuid;
			}
			if (organizationId == null || organizationId == OrganizationId.ForestWideOrgId || organizationId.ConfigurationUnit == null)
			{
				this.tenantId = Guid.Empty;
				return;
			}
			this.tenantId = organizationId.ConfigurationUnit.ObjectGuid;
		}

		public bool IsAuthoritative
		{
			get
			{
				return this.DomainType == AcceptedDomainType.Authoritative;
			}
		}

		public bool IsInternal
		{
			get
			{
				return this.DomainType != AcceptedDomainType.ExternalRelay;
			}
		}

		public SmtpDomainWithSubdomains DomainName
		{
			get
			{
				if (this.domain.IncludeSubDomains || !this.MatchSubDomains)
				{
					return this.domain;
				}
				return new SmtpDomainWithSubdomains(this.domain.Domain, true);
			}
		}

		public int EstimatedSize
		{
			get
			{
				return RemoteDomainEntry.GetLenghAfterNullCheck(this.DomainName.Domain) * 2 + 1 + RemoteDomainEntry.GetLenghAfterNullCheck(this.Name) * 2 + RemoteDomainEntry.GetLenghAfterNullCheck(this.NameSpecification) * 2 + 3 + 4 + 64;
			}
		}

		public override bool IsInCorporation
		{
			get
			{
				return this.IsInternal;
			}
		}

		public override bool UseAddressBook
		{
			get
			{
				return this.AddressBookEnabled;
			}
		}

		public override string NameSpecification
		{
			get
			{
				return this.domain.ToString();
			}
		}

		public override Guid TenantId
		{
			get
			{
				return this.tenantId;
			}
		}

		public Guid CatchAllRecipientID
		{
			get
			{
				return this.catchAllRecipientID;
			}
		}

		public string Name
		{
			get
			{
				return this.name;
			}
		}

		public bool SyncedToPerimeterAsGuidDomain
		{
			get
			{
				return this.syncedToPerimeterAsGuidDomain;
			}
		}

		public override string ToString()
		{
			return this.NameSpecification;
		}

		private const int GuidLength = 16;

		public readonly AcceptedDomainType DomainType;

		public readonly bool MatchSubDomains;

		public readonly bool IsDefault;

		public readonly bool AddressBookEnabled;

		public readonly Guid Guid;

		private readonly string name;

		private readonly SmtpDomainWithSubdomains domain;

		private readonly Guid tenantId;

		private readonly Guid catchAllRecipientID;

		private readonly bool syncedToPerimeterAsGuidDomain;
	}
}
