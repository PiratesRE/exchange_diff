using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Transport;
using Microsoft.Exchange.Transport.RecipientAPI;

namespace Microsoft.Exchange.InfoWorker.Common.OrganizationConfiguration
{
	internal class OrganizationDomains
	{
		public IList<Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain> AcceptedDomains
		{
			get
			{
				return this.acceptedDomains.AcceptedDomains;
			}
			internal set
			{
				this.acceptedDomains.AcceptedDomains = (Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain[])value;
			}
		}

		public IList<SmtpDomainWithSubdomains> InternalDomains
		{
			get
			{
				if (this.internalDomains == null || this.cookie != this.acceptedDomains.AcceptedDomains)
				{
					this.cookie = this.acceptedDomains.AcceptedDomains;
					this.BuildInternalDomainList();
				}
				return this.internalDomains;
			}
		}

		public OrganizationId OrganizationId { get; private set; }

		private OrganizationDomains()
		{
		}

		public OrganizationDomains(OrganizationId organizationId)
		{
			this.OrganizationId = organizationId;
			this.acceptedDomains = new PerTenantAcceptedDomainCollection(organizationId);
			this.remoteDomains = new PerTenantRemoteDomainCollection(organizationId);
			this.isInternalResolver = new IsInternalResolver(organizationId, new IsInternalResolver.GetAcceptedDomainCollectionDelegate(this.GetAcceptedDomainCollection), new IsInternalResolver.GetRemoteDomainCollectionDelegate(this.GetRemoteDomainCollection));
		}

		public OrganizationDomains(OrganizationId organizationId, TimeSpan timeoutInterval)
		{
			this.OrganizationId = organizationId;
			this.acceptedDomains = new PerTenantAcceptedDomainCollection(organizationId, timeoutInterval);
			this.remoteDomains = new PerTenantRemoteDomainCollection(organizationId, timeoutInterval);
			this.isInternalResolver = new IsInternalResolver(organizationId, new IsInternalResolver.GetAcceptedDomainCollectionDelegate(this.GetAcceptedDomainCollection), new IsInternalResolver.GetRemoteDomainCollectionDelegate(this.GetRemoteDomainCollection));
		}

		public OrganizationDomains(IList<SmtpDomainWithSubdomains> internalDomains)
		{
			this.acceptedDomains = new PerTenantAcceptedDomainCollection(OrganizationId.ForestWideOrgId);
			this.internalDomains = internalDomains;
		}

		public DomainType GetAcceptedDomainType(string domain)
		{
			bool flag;
			AcceptedDomainCollection acceptedDomainCollection = this.GetAcceptedDomainCollection(this.OrganizationId, out flag);
			AcceptedDomainEntry acceptedDomainEntry = (AcceptedDomainEntry)acceptedDomainCollection.Find(domain);
			if (acceptedDomainEntry == null)
			{
				return DomainType.External;
			}
			switch (acceptedDomainEntry.DomainType)
			{
			case AcceptedDomainType.Authoritative:
				return DomainType.Authoritative;
			case AcceptedDomainType.ExternalRelay:
				return DomainType.External;
			case AcceptedDomainType.InternalRelay:
				return DomainType.InternalRelay;
			default:
				return DomainType.Unknown;
			}
		}

		public bool IsInternal(string domainName)
		{
			bool result;
			try
			{
				RoutingDomain domain = new RoutingDomain(domainName);
				result = this.isInternalResolver.IsInternal(domain);
			}
			catch (FormatException)
			{
				result = false;
			}
			return result;
		}

		public void Initialize()
		{
			this.acceptedDomains.Initialize();
			this.remoteDomains.Initialize();
		}

		private void BuildInternalDomainList()
		{
			List<SmtpDomainWithSubdomains> list = new List<SmtpDomainWithSubdomains>(this.acceptedDomains.AcceptedDomains.Length);
			foreach (Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain acceptedDomain in this.acceptedDomains.AcceptedDomains)
			{
				if (acceptedDomain.DomainType != AcceptedDomainType.ExternalRelay)
				{
					list.Add(acceptedDomain.DomainName);
				}
			}
			this.internalDomains = list;
		}

		private AcceptedDomainCollection GetAcceptedDomainCollection(OrganizationId organizationId, out bool scopedToOrganization)
		{
			scopedToOrganization = false;
			if (organizationId != this.OrganizationId)
			{
				return null;
			}
			if (this.acceptedDomains.AcceptedDomains != null && 0 < this.acceptedDomains.AcceptedDomains.Length)
			{
				AcceptedDomainEntry[] acceptedDomainEntryList = (from domain in this.acceptedDomains.AcceptedDomains
				where domain != null
				select new AcceptedDomainEntry(domain, organizationId)).ToArray<AcceptedDomainEntry>();
				return new AcceptedDomainMap(acceptedDomainEntryList);
			}
			return new AcceptedDomainMap(null);
		}

		private RemoteDomainCollection GetRemoteDomainCollection(OrganizationId organizationId)
		{
			if (this.remoteDomains.RemoteDomains != null && 0 < this.remoteDomains.RemoteDomains.Length)
			{
				RemoteDomainEntry[] remoteDomainEntryList = (from domain in this.remoteDomains.RemoteDomains
				where domain != null
				select new RemoteDomainEntry(domain)).ToArray<RemoteDomainEntry>();
				return new RemoteDomainMap(remoteDomainEntryList);
			}
			return new RemoteDomainMap(null);
		}

		private PerTenantAcceptedDomainCollection acceptedDomains;

		private PerTenantRemoteDomainCollection remoteDomains;

		private object cookie;

		private IList<SmtpDomainWithSubdomains> internalDomains;

		private IsInternalResolver isInternalResolver;
	}
}
