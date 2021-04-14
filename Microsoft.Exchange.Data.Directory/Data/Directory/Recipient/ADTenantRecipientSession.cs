using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Directory;

namespace Microsoft.Exchange.Data.Directory.Recipient
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	[Serializable]
	internal class ADTenantRecipientSession : ADRecipientObjectSession, ITenantRecipientSession, IRecipientSession, IDirectorySession, IConfigDataProvider
	{
		public ADTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings) : base(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
		}

		public ADTenantRecipientSession(string domainController, ADObjectId searchRoot, int lcid, bool readOnly, ConsistencyMode consistencyMode, NetworkCredential networkCredential, ADSessionSettings sessionSettings, ConfigScopes configScope) : this(domainController, searchRoot, lcid, readOnly, consistencyMode, networkCredential, sessionSettings)
		{
			base.CheckConfigScopeParameter(configScope);
			base.ConfigScope = configScope;
		}

		public ADRawEntry ChooseBetweenAmbiguousUsers(ADRawEntry[] entries)
		{
			if (entries.Length == 0)
			{
				return null;
			}
			if (entries.Length == 1)
			{
				return entries[0];
			}
			return this.ChooseBetweenAmbiguousUsers(entries, 0, entries.Length);
		}

		public ADObjectId ChooseBetweenAmbiguousUsers(ADObjectId user1Id, ADObjectId user2Id)
		{
			ADObjectId result = null;
			ADUser aduser = base.FindADUserByObjectId(user1Id);
			ADUser aduser2 = base.FindADUserByObjectId(user2Id);
			if (aduser != null && aduser2 != null && !user1Id.Equals(user2Id) && aduser.RecipientType == aduser2.RecipientType && aduser.RecipientTypeDetails == aduser2.RecipientTypeDetails && aduser.NetID != null && aduser2.NetID != null && aduser.WindowsLiveID != SmtpAddress.Empty && aduser2.WindowsLiveID != SmtpAddress.Empty && aduser.NetID.Equals(aduser2.NetID) && aduser.WindowsLiveID == aduser2.WindowsLiveID && aduser.OrganizationId != null && aduser.OrganizationId.ConfigurationUnit != null && aduser2.OrganizationId != null && aduser2.OrganizationId.ConfigurationUnit != null)
			{
				IConfigurationSession tenantOrTopologyConfigurationSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromAllTenantsPartitionId(base.SessionSettings.PartitionId), 142, "ChooseBetweenAmbiguousUsers", "f:\\15.00.1497\\sources\\dev\\data\\src\\directory\\Recipient\\ADTenantRecipientSession.cs");
				ExchangeConfigurationUnit exchangeConfigurationUnit = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(aduser.OrganizationId.ConfigurationUnit);
				ExchangeConfigurationUnit exchangeConfigurationUnit2 = tenantOrTopologyConfigurationSession.Read<ExchangeConfigurationUnit>(aduser2.OrganizationId.ConfigurationUnit);
				if (exchangeConfigurationUnit != null && exchangeConfigurationUnit2 != null)
				{
					if (exchangeConfigurationUnit.MSOSyncEnabled && exchangeConfigurationUnit2.MSOSyncEnabled && !string.IsNullOrEmpty(aduser.ExternalDirectoryObjectId) && !string.IsNullOrEmpty(aduser2.ExternalDirectoryObjectId) && !aduser.ExternalDirectoryObjectId.Equals(aduser2.ExternalDirectoryObjectId, StringComparison.OrdinalIgnoreCase))
					{
						return result;
					}
					if (!aduser.OrganizationId.Equals(aduser2.OrganizationId))
					{
						if (exchangeConfigurationUnit.WhenCreatedUTC != null && exchangeConfigurationUnit2.WhenCreatedUTC != null && exchangeConfigurationUnit.WhenCreatedUTC != exchangeConfigurationUnit2.WhenCreatedUTC)
						{
							if (exchangeConfigurationUnit.WhenCreatedUTC > exchangeConfigurationUnit2.WhenCreatedUTC)
							{
								result = aduser.Id;
							}
							else
							{
								result = aduser2.Id;
							}
						}
					}
					else if (aduser.WhenCreatedUTC != null && aduser2.WhenCreatedUTC != null && aduser.WhenCreatedUTC != aduser2.WhenCreatedUTC)
					{
						if (aduser.WhenCreatedUTC > aduser2.WhenCreatedUTC)
						{
							result = aduser.Id;
						}
						else
						{
							result = aduser2.Id;
						}
					}
					else if (aduser.Guid.CompareTo(aduser2.Guid) > 0)
					{
						result = aduser.Id;
					}
					else
					{
						result = aduser2.Id;
					}
				}
			}
			return result;
		}

		private ADRawEntry ChooseUserForLiveIdMemberName(ADRawEntry[] entries, string liveIdMemberName)
		{
			if (entries.Length == 1)
			{
				return entries[0];
			}
			if (entries.Length == 0)
			{
				return null;
			}
			int num = 0;
			ADRawEntry adrawEntry = null;
			foreach (ADRawEntry adrawEntry2 in entries)
			{
				string a = (string)adrawEntry2[ADUserSchema.UserPrincipalName];
				if (string.Equals(a, liveIdMemberName, StringComparison.OrdinalIgnoreCase))
				{
					adrawEntry = adrawEntry2;
					if (++num >= 2)
					{
						break;
					}
				}
			}
			if (num == 1)
			{
				return adrawEntry;
			}
			ExTraceGlobals.ADReadTracer.TraceDebug<string, int>((long)this.GetHashCode(), "ADTenantRecipientSession::ChooseUserForLiveIdMemberName - could not choose as entires contains more than one element for liveIdMemberName {0}. The number of elements is {1}", liveIdMemberName, entries.Length);
			adrawEntry = this.ChooseBetweenAmbiguousUsers(entries, 0, entries.Length);
			if (adrawEntry != null)
			{
				return adrawEntry;
			}
			ExTraceGlobals.ADReadTracer.TraceDebug<string, ADObjectId>((long)this.GetHashCode(), "ADTenantRecipientSession::ChooseUserForLiveIdMemberName - could not resolve ambiguous case by ChooseBetweenAmbiguousUsers for liveIdMemberName {0}. The id is {1}", liveIdMemberName, entries[0].Id);
			throw new NonUniqueRecipientException(liveIdMemberName, new NonUniqueAddressError(DirectoryStrings.ErrorNonUniqueLiveIdMemberName(liveIdMemberName), entries[0].Id, string.Empty));
		}

		public ADRawEntry[] FindByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			ADObjectId rootId = null;
			QueryFilter excludeExternalNetIDFilter = null;
			if (string.IsNullOrEmpty(organizationContext))
			{
				TextFilter filter = new TextFilter(IADSecurityPrincipalSchema.AltSecurityIdentities, "ExWLID:", MatchOptions.Prefix, MatchFlags.IgnoreCase);
				excludeExternalNetIDFilter = new NotFilter(filter);
			}
			ADRawEntry[] array = this.SearchByNetIDType(rootId, netID, false, excludeExternalNetIDFilter, properties);
			if (array.Length == 0)
			{
				array = this.SearchByNetIDType(rootId, netID, true, excludeExternalNetIDFilter, properties);
			}
			return array;
		}

		public ADRawEntry[] FindByNetID(string netID, params PropertyDefinition[] properties)
		{
			return this.FindByNetID(netID, string.Empty, properties);
		}

		public MiniRecipient FindRecipientByNetID(NetID netId)
		{
			if (netId == null)
			{
				throw new ArgumentNullException("netId");
			}
			QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, IADSecurityPrincipalSchema.NetID, netId);
			MiniRecipient[] array = base.Find<MiniRecipient>(null, QueryScope.SubTree, filter, null, 2, null, false);
			if (array == null || array.Length == 0)
			{
				return null;
			}
			if (array.Length > 1)
			{
				throw new MoreThanOneRecipientWithNetId(netId.ToString());
			}
			return array[0];
		}

		public ADRawEntry FindUniqueEntryByNetID(string netID, string organizationContext, params PropertyDefinition[] properties)
		{
			ADRawEntry[] array = this.FindByNetID(netID, organizationContext, properties);
			if (array.Length == 1)
			{
				return array[0];
			}
			if (array.Length == 0)
			{
				return null;
			}
			ExTraceGlobals.ADReadTracer.TraceDebug<int, string>((long)this.GetHashCode(), "ADTenantRecipientSession::ChooseBetweenAmbiguousUsers - found {0} entries matching NETID {1} - resolving ambiguous case.", array.Length, netID);
			ADRawEntry adrawEntry = this.ChooseBetweenAmbiguousUsers(array);
			if (adrawEntry == null)
			{
				ExTraceGlobals.ADReadTracer.TraceDebug<string>((long)this.GetHashCode(), "ADTenantRecipientSession::ChooseBetweenAmbiguousUsers - could not resolve ambiguous case for NETID {0}.", netID);
				throw new NonUniqueRecipientException(netID, new NonUniqueAddressError(DirectoryStrings.ErrorNonUniqueNetId(netID), array[0].Id, string.Empty));
			}
			ExTraceGlobals.ADReadTracer.TraceDebug<string, ADObjectId>((long)this.GetHashCode(), "ADTenantRecipientSession::ChooseBetweenAmbiguousUsers - resolved ambiguous case for NETID {0} = {1}.", netID, adrawEntry.Id);
			return adrawEntry;
		}

		public ADRawEntry FindUniqueEntryByNetID(string netID, params PropertyDefinition[] properties)
		{
			return this.FindUniqueEntryByNetID(netID, string.Empty, properties);
		}

		public ADRawEntry FindByLiveIdMemberName(string liveIdMemberName, params PropertyDefinition[] properties)
		{
			if (string.IsNullOrWhiteSpace(liveIdMemberName))
			{
				throw new ArgumentException("liveIdMemberName is null or white space.");
			}
			if (properties == null)
			{
				throw new ArgumentNullException("properties");
			}
			ProxyAddress proxyAddress = new SmtpProxyAddress(liveIdMemberName, true);
			QueryFilter filter = new OrFilter(new QueryFilter[]
			{
				base.QueryFilterFromProxyAddress(proxyAddress),
				base.QueryFilterFromUserPrincipalName(liveIdMemberName)
			});
			IEnumerable<PropertyDefinition> first = new PropertyDefinition[]
			{
				ADUserSchema.UserPrincipalName
			};
			ADRawEntry[] entries = base.Find<ADRawEntry>(null, QueryScope.SubTree, filter, null, 0, first.Concat(properties), false);
			return this.ChooseUserForLiveIdMemberName(entries, liveIdMemberName);
		}

		public Result<ADRawEntry>[] ReadMultipleByLinkedPartnerId(LinkedPartnerGroupInformation[] entryIds, params PropertyDefinition[] properties)
		{
			return base.ReadMultiple<LinkedPartnerGroupInformation, ADRawEntry>(entryIds, new Converter<LinkedPartnerGroupInformation, QueryFilter>(ADTenantRecipientSession.LinkedPartnerGroupFilterBuilder), new ADDataSession.HashInserter<ADRawEntry>(ADTenantRecipientSession.LinkedPartnerGroupHashInserter<ADRawEntry>), new ADDataSession.HashLookup<LinkedPartnerGroupInformation, ADRawEntry>(ADTenantRecipientSession.LinkedPartnerGroupHashLookup<ADRawEntry>), properties);
		}

		private static QueryFilter LinkedPartnerGroupFilterBuilder(LinkedPartnerGroupInformation id)
		{
			return new ComparisonFilter(ComparisonOperator.Equal, ADGroupSchema.LinkedPartnerGroupAndOrganizationId, id);
		}

		private static void LinkedPartnerGroupHashInserter<T>(Hashtable hash, T entry) where T : ADRawEntry
		{
			hash.Add(entry[ADGroupSchema.LinkedPartnerGroupAndOrganizationId], new Result<T>(entry, null));
		}

		private static Result<T> LinkedPartnerGroupHashLookup<T>(Hashtable hash, LinkedPartnerGroupInformation key) where T : ADRawEntry
		{
			return (Result<T>)(hash.ContainsKey(key) ? hash[key] : new Result<T>(default(T), ProviderError.NotFound));
		}

		private ADRawEntry[] SearchByNetIDType(ADObjectId rootId, string netID, bool searchForConsumerNetID, QueryFilter excludeExternalNetIDFilter, params PropertyDefinition[] properties)
		{
			QueryFilter queryFilter;
			if (!searchForConsumerNetID)
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, IADSecurityPrincipalSchema.NetID, NetID.Parse(netID));
			}
			else
			{
				queryFilter = new ComparisonFilter(ComparisonOperator.Equal, IADSecurityPrincipalSchema.ConsumerNetID, NetID.Parse(netID));
			}
			QueryFilter filter;
			if (excludeExternalNetIDFilter != null)
			{
				filter = new AndFilter(new QueryFilter[]
				{
					queryFilter,
					excludeExternalNetIDFilter
				});
			}
			else
			{
				filter = queryFilter;
			}
			return base.Find<ADRawEntry>(rootId, QueryScope.SubTree, filter, null, 0, properties, false);
		}

		private ADRawEntry ChooseBetweenAmbiguousUsers(ADRawEntry[] entries, int lower, int upper)
		{
			if (lower >= upper)
			{
				throw new ArgumentException("lower bound is equal or greater than upper bound");
			}
			if (lower + 1 == upper)
			{
				return entries[lower];
			}
			int num = (upper - lower) / 2;
			ADRawEntry adrawEntry = this.ChooseBetweenAmbiguousUsers(entries, lower, lower + num);
			ADRawEntry adrawEntry2 = this.ChooseBetweenAmbiguousUsers(entries, lower + num, upper);
			if (adrawEntry == null || adrawEntry2 == null)
			{
				return null;
			}
			ADObjectId adobjectId = this.ChooseBetweenAmbiguousUsers(adrawEntry.Id, adrawEntry2.Id);
			if (adobjectId == null)
			{
				return null;
			}
			if (ADObjectId.Equals(adrawEntry.Id, adobjectId))
			{
				return adrawEntry;
			}
			return adrawEntry2;
		}
	}
}
