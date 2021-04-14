using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.InfoWorker.Common.MessageTracking
{
	internal sealed class DomainOrgMailboxCache : LazyLookupTimeoutCacheWithDiagnostics<string, SmtpAddress>
	{
		public DomainOrgMailboxCache() : base(2, 100, false, TimeSpan.FromHours(5.0))
		{
		}

		protected override string PreprocessKey(string key)
		{
			return key.ToUpperInvariant();
		}

		protected override SmtpAddress Create(string domain, ref bool shouldAdd)
		{
			IRecipientSession recipientSessionForDomain = this.GetRecipientSessionForDomain(domain);
			if (recipientSessionForDomain != null)
			{
				List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(recipientSessionForDomain, OrganizationCapability.MessageTracking);
				if (organizationMailboxesByCapability != null && organizationMailboxesByCapability.Count != 0)
				{
					int index = DomainOrgMailboxCache.rand.Next(organizationMailboxesByCapability.Count);
					ADUser aduser = organizationMailboxesByCapability.ElementAt(index);
					TraceWrapper.SearchLibraryTracer.TraceDebug<ObjectId, OrganizationId>(this.GetHashCode(), "Found E15 Org mailbox {0} for organization {1}.", aduser.Identity, recipientSessionForDomain.SessionSettings.CurrentOrganizationId);
					shouldAdd = true;
					return aduser.PrimarySmtpAddress;
				}
			}
			SmtpAddress result = new SmtpAddress(DomainOrgMailboxCache.E14EDiscoveryMailbox, domain);
			TraceWrapper.SearchLibraryTracer.TraceError<string, string>(this.GetHashCode(), "Unable to get org mailbox for domain {0}. Will try to use E14 Discovery mailbox {1} for request", domain, result.ToString());
			shouldAdd = false;
			return result;
		}

		private IRecipientSession GetRecipientSessionForDomain(string domain)
		{
			IRecipientSession result = null;
			ADSessionSettings adsessionSettings = null;
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled && SmtpAddress.IsValidDomain(domain))
			{
				try
				{
					adsessionSettings = ADSessionSettings.FromTenantAcceptedDomain(domain);
				}
				catch (CannotResolveTenantNameException)
				{
					TraceWrapper.SearchLibraryTracer.TraceDebug<string>(this.GetHashCode(), "Failed to resolve domain {0}.", domain);
					adsessionSettings = null;
				}
			}
			if (adsessionSettings != null)
			{
				result = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, adsessionSettings, 137, "GetRecipientSessionForDomain", "f:\\15.00.1497\\sources\\dev\\infoworker\\src\\common\\MessageTracking\\Caching\\DomainOrgMailboxCache.cs");
			}
			return result;
		}

		private static readonly string E14EDiscoveryMailbox = "SystemMailbox{e0dc1c29-89c3-4034-b678-e6c29d823ed9}";

		private static readonly Random rand = new Random();
	}
}
