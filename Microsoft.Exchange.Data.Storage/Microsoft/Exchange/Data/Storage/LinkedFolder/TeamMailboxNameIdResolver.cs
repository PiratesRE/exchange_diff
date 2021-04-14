using System;
using System.Security.Principal;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Data.Storage.LinkedFolder
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class TeamMailboxNameIdResolver : LazyLookupTimeoutCache<string, ADRawEntry>
	{
		private TeamMailboxNameIdResolver() : base(10, 5000, false, TimeSpan.FromHours(4.0), TimeSpan.FromHours(12.0))
		{
		}

		public static ADRawEntry Resolve(IRecipientSession dataSession, string id, out Exception ex)
		{
			if (dataSession == null)
			{
				throw new ArgumentNullException("dataSession");
			}
			ADRawEntry result = null;
			ex = null;
			if (!string.IsNullOrEmpty(id))
			{
				TeamMailboxNameIdResolver.instance.dataSession = dataSession;
				try
				{
					TeamMailboxNameIdResolver.instance.newSidEx = null;
					result = TeamMailboxNameIdResolver.instance.Get(id);
					ex = TeamMailboxNameIdResolver.instance.newSidEx;
				}
				catch (ADTransientException ex2)
				{
					ex = ex2;
				}
				catch (ADExternalException ex3)
				{
					ex = ex3;
				}
				catch (ADOperationException ex4)
				{
					ex = ex4;
				}
			}
			return result;
		}

		protected override ADRawEntry CreateOnCacheMiss(string key, ref bool shouldAdd)
		{
			ADRawEntry adrawEntry = null;
			bool useGlobalCatalog = TeamMailboxNameIdResolver.instance.dataSession.UseGlobalCatalog;
			try
			{
				TeamMailboxNameIdResolver.instance.dataSession.UseGlobalCatalog = true;
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					ITenantRecipientSession tenantRecipientSession = TeamMailboxNameIdResolver.instance.dataSession as ITenantRecipientSession;
					if (tenantRecipientSession != null)
					{
						adrawEntry = tenantRecipientSession.FindUniqueEntryByNetID(key, TeamMailboxNameIdResolver.UserObjectProperties);
					}
				}
				else
				{
					SecurityIdentifier securityIdentifier = null;
					try
					{
						securityIdentifier = new SecurityIdentifier(key);
					}
					catch (ArgumentException ex)
					{
						this.newSidEx = ex;
					}
					if (securityIdentifier != null)
					{
						adrawEntry = TeamMailboxNameIdResolver.instance.dataSession.FindADRawEntryBySid(securityIdentifier, TeamMailboxNameIdResolver.UserObjectProperties);
					}
				}
			}
			finally
			{
				TeamMailboxNameIdResolver.instance.dataSession.UseGlobalCatalog = useGlobalCatalog;
			}
			shouldAdd = (adrawEntry != null);
			return adrawEntry;
		}

		private static readonly PropertyDefinition[] UserObjectProperties = new PropertyDefinition[]
		{
			ADObjectSchema.Id,
			ADRecipientSchema.RecipientTypeDetails
		};

		private static TeamMailboxNameIdResolver instance = new TeamMailboxNameIdResolver();

		private IRecipientSession dataSession;

		private ArgumentException newSidEx;
	}
}
