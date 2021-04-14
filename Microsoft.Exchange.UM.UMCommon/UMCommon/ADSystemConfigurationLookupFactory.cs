using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class ADSystemConfigurationLookupFactory
	{
		public static IADSystemConfigurationLookup CreateFromRootOrg(bool readOnly)
		{
			return new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup(readOnly);
		}

		public static IADSystemConfigurationLookup CreateFromADRecipient(IADRecipient recipient)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			return ADSystemConfigurationLookupFactory.CreateFromOrganizationId(recipient.OrganizationId, true);
		}

		public static IADSystemConfigurationLookup CreateFromOrganizationId(OrganizationId orgId)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			return ADSystemConfigurationLookupFactory.CreateFromOrganizationId(orgId, true);
		}

		public static IADSystemConfigurationLookup CreateFromOrganizationId(OrganizationId orgId, bool readOnly)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			return new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup(orgId, readOnly);
		}

		public static IADSystemConfigurationLookup CreateFromTenantGuid(Guid tenantGuid)
		{
			return new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup(tenantGuid, true);
		}

		public static IADSystemConfigurationLookup CreateFromTenantGuid(Guid tenantGuid, bool readOnly)
		{
			return new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup(tenantGuid, readOnly);
		}

		public static IADSystemConfigurationLookup CreateFromAcceptedDomain(string acceptedDomain)
		{
			return new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup(acceptedDomain);
		}

		public static IADSystemConfigurationLookup CreateFromExistingSession(IConfigurationSession session, bool isDatacenter)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup(session, isDatacenter);
		}

		private class ADSystemConfigurationLookup : IADSystemConfigurationLookup
		{
			public ADSystemConfigurationLookup(OrganizationId orgId, bool readOnly)
			{
				ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup.<>c__DisplayClass1 CS$<>8__locals1 = new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup.<>c__DisplayClass1();
				CS$<>8__locals1.readOnly = readOnly;
				this.latencyStopwatch = new LatencyStopwatch();
				this.isDatacenter = CommonConstants.DataCenterADPresent;
				base..ctor();
				if (orgId == null)
				{
					throw new ArgumentNullException("orgId");
				}
				if (this.isDatacenter && OrganizationId.ForestWideOrgId.Equals(orgId))
				{
					ExAssert.RetailAssert(false, "Incorrectly scoped session - OrganizationalUnit = '{0}', ConfigurationUnit = '{1}'. Both OrganizationalUnit and ConfigurationUnit should be non-null.", new object[]
					{
						(orgId.OrganizationalUnit != null) ? orgId.OrganizationalUnit.ToString() : "<null>",
						(orgId.ConfigurationUnit != null) ? orgId.ConfigurationUnit.ToString() : "<null>"
					});
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Creating config lookup object - Organization OU = '{0}', Organization CU = '{1}'", new object[]
				{
					(orgId.OrganizationalUnit != null) ? orgId.OrganizationalUnit.ToString() : "<null>",
					(orgId.ConfigurationUnit != null) ? orgId.ConfigurationUnit.ToString() : "<null>"
				});
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
				this.session = this.InvokeWithStopwatch<IConfigurationSession>("DirectorySessionFactory.GetTenantOrTopologyConfigurationSession", () => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(CS$<>8__locals1.readOnly, ConsistencyMode.IgnoreInvalid, sessionSettings, 379, ".ctor", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADSystemConfigLookup.cs"));
				this.ValidateOrgScopeInDataCenter();
			}

			public ADSystemConfigurationLookup(Guid tenantGuid, bool readOnly)
			{
				ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup.<>c__DisplayClass8 CS$<>8__locals1 = new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup.<>c__DisplayClass8();
				CS$<>8__locals1.tenantGuid = tenantGuid;
				CS$<>8__locals1.readOnly = readOnly;
				this.latencyStopwatch = new LatencyStopwatch();
				this.isDatacenter = CommonConstants.DataCenterADPresent;
				base..ctor();
				if (this.isDatacenter && CS$<>8__locals1.tenantGuid == Guid.Empty)
				{
					throw new InvalidTenantGuidException(CS$<>8__locals1.tenantGuid);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Creating system config lookup object - Tenant Guid = '{0}'", new object[]
				{
					CS$<>8__locals1.tenantGuid
				});
				ADSessionSettings sessionSettings;
				if (this.isDatacenter)
				{
					try
					{
						sessionSettings = this.InvokeWithStopwatch<ADSessionSettings>("ADSessionSettings.FromExternalDirectoryOrganizationId", () => ADSessionSettings.FromExternalDirectoryOrganizationId(CS$<>8__locals1.tenantGuid));
						goto IL_D3;
					}
					catch (CannotResolveExternalDirectoryOrganizationIdException innerException)
					{
						throw new InvalidTenantGuidException(CS$<>8__locals1.tenantGuid, innerException);
					}
				}
				sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				IL_D3:
				this.session = this.InvokeWithStopwatch<IConfigurationSession>("DirectorySessionFactory.GetTenantOrTopologyConfigurationSession", () => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(CS$<>8__locals1.readOnly, ConsistencyMode.IgnoreInvalid, sessionSettings, 433, ".ctor", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADSystemConfigLookup.cs"));
				this.ValidateOrgScopeInDataCenter();
			}

			public ADSystemConfigurationLookup(bool readOnly)
			{
				ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup.<>c__DisplayClassd CS$<>8__locals1 = new ADSystemConfigurationLookupFactory.ADSystemConfigurationLookup.<>c__DisplayClassd();
				CS$<>8__locals1.readOnly = readOnly;
				this.latencyStopwatch = new LatencyStopwatch();
				this.isDatacenter = CommonConstants.DataCenterADPresent;
				base..ctor();
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Creating Root Org scoped config lookup object", new object[0]);
				ADSessionSettings sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				this.session = this.InvokeWithStopwatch<ITopologyConfigurationSession>("DirectorySessionFactory.CreateTopologyConfigurationSession", () => DirectorySessionFactory.Default.CreateTopologyConfigurationSession(CS$<>8__locals1.readOnly, ConsistencyMode.IgnoreInvalid, sessionSettings, 458, ".ctor", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADSystemConfigLookup.cs"));
			}

			public ADSystemConfigurationLookup(string acceptedDomain)
			{
				this.latencyStopwatch = new LatencyStopwatch();
				this.isDatacenter = CommonConstants.DataCenterADPresent;
				base..ctor();
				ValidateArgument.NotNullOrEmpty(acceptedDomain, "acceptedDomain");
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Creating system config lookup object - Accepted domain = '{0}'", new object[]
				{
					acceptedDomain
				});
				ADSessionSettings sessionSettings;
				if (this.isDatacenter)
				{
					sessionSettings = ADSessionSettings.FromTenantAcceptedDomain(acceptedDomain);
				}
				else
				{
					sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				}
				this.session = this.InvokeWithStopwatch<IConfigurationSession>("DirectorySessionFactory.GetTenantOrTopologyConfigurationSession", () => DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 496, ".ctor", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADSystemConfigLookup.cs"));
				this.ValidateOrgScopeInDataCenter();
			}

			public ADSystemConfigurationLookup(IConfigurationSession session, bool isDatacenter)
			{
				this.latencyStopwatch = new LatencyStopwatch();
				this.isDatacenter = CommonConstants.DataCenterADPresent;
				base..ctor();
				this.session = session;
				this.isDatacenter = isDatacenter;
			}

			public UMDialPlan GetDialPlanFromId(ADObjectId dialPlanId)
			{
				if (dialPlanId == null)
				{
					throw new ArgumentNullException("dialPlanId");
				}
				return this.InvokeWithStopwatch<UMDialPlan>("IConfigurationSession.Read<UMDialPlan>", () => this.session.Read<UMDialPlan>(dialPlanId));
			}

			public UMDialPlan GetDialPlanFromRecipient(IADRecipient recipient)
			{
				if (recipient == null)
				{
					throw new ArgumentNullException("recipient");
				}
				UMDialPlan result = null;
				ADObjectId umrecipientDialPlanId = recipient.UMRecipientDialPlanId;
				if (umrecipientDialPlanId != null)
				{
					result = this.GetDialPlanFromId(umrecipientDialPlanId);
				}
				return result;
			}

			public UMDialPlan GetDialPlanFromPilotIdentifier(string pilotIdentifier)
			{
				if (pilotIdentifier == null)
				{
					throw new ArgumentNullException("pilotIdentifier");
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, UMDialPlanSchema.PilotIdentifierList, pilotIdentifier);
				UMDialPlan[] array = this.InvokeWithStopwatch<UMDialPlan[]>("IConfigurationSession.Read<UMDialPlan>", () => this.session.Find<UMDialPlan>(null, QueryScope.SubTree, filter, null, 0));
				if (array.Length == 1)
				{
					return array[0];
				}
				return null;
			}

			public UMIPGateway GetIPGatewayFromId(ADObjectId gatewayId)
			{
				if (gatewayId == null)
				{
					throw new ArgumentNullException("gatewayId");
				}
				return this.InvokeWithStopwatch<UMIPGateway>("IConfigurationSession.Read<UMIPGateway>", () => this.session.Read<UMIPGateway>(gatewayId));
			}

			public ExchangeConfigurationUnit GetConfigurationUnitByTenantGuid(Guid tenantGuid)
			{
				ExAssert.RetailAssert(this.isDatacenter, "This method is only intended to be used in Datacenter Environments");
				if (tenantGuid.Equals(Guid.Empty))
				{
					throw new InvalidArgumentException("tenantGuid is Empty");
				}
				return ((ITenantConfigurationSession)this.session).GetExchangeConfigurationUnitByExternalId(tenantGuid.ToString());
			}

			public IEnumerable<UMIPGateway> GetAllGlobalGateways()
			{
				this.ValidateTenantLocalScope();
				return this.GetIPGateways((UMIPGateway gw) => gw.GlobalCallRoutingScheme == UMGlobalCallRoutingScheme.E164);
			}

			public IEnumerable<UMDialPlan> GetAllDialPlans()
			{
				return this.InvokeWithStopwatch<ADPagedReader<UMDialPlan>>("IConfigurationSession.FindPaged<UMIPGateway>", () => this.session.FindPaged<UMDialPlan>(null, QueryScope.SubTree, null, null, 100));
			}

			public UMIPGateway GetIPGatewayFromAddress(IList<string> fqdns)
			{
				if (fqdns == null || fqdns.Count == 0)
				{
					throw new ArgumentException("Null or empty", "fqdns");
				}
				IEnumerable<UMIPGateway> ipgateways = this.GetIPGateways((UMIPGateway gw) => fqdns.Contains(gw.Address.ToString(), StringComparer.InvariantCultureIgnoreCase));
				return ipgateways.FirstOrDefault<UMIPGateway>();
			}

			public IEnumerable<UMIPGateway> GetGatewaysLinkedToDialPlan(UMDialPlan dialPlan)
			{
				if (dialPlan == null)
				{
					throw new ArgumentNullException("dialPlan");
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "ADSystemConfigurationLookup::GetGatewaysLinkedToDialPlan() dialPlan={0}", new object[]
				{
					dialPlan.DistinguishedName
				});
				List<QueryFilter> list = new List<QueryFilter>();
				foreach (ADObjectId propertyValue in dialPlan.UMIPGateway)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Id, propertyValue));
				}
				QueryFilter gatewayFilter = new OrFilter(list.ToArray());
				return this.InvokeWithStopwatch<ADPagedReader<UMIPGateway>>("IConfigurationSession.FindPaged<UMIPGateway>", () => this.session.FindPaged<UMIPGateway>(null, QueryScope.SubTree, gatewayFilter, null, 100));
			}

			public UMAutoAttendant GetAutoAttendantFromId(ADObjectId autoAttendantId)
			{
				if (autoAttendantId == null)
				{
					throw new ArgumentNullException("autoAttendantId");
				}
				return this.InvokeWithStopwatch<UMAutoAttendant>("IConfigurationSession.Read<UMAutoAttendant>", () => this.session.Read<UMAutoAttendant>(autoAttendantId));
			}

			public UMAutoAttendant GetAutoAttendantFromPilotIdentifierAndDialPlan(string pilotIdentifier, ADObjectId dialPlanId)
			{
				if (pilotIdentifier == null)
				{
					throw new ArgumentNullException("pilotIdentifier");
				}
				if (dialPlanId == null)
				{
					throw new ArgumentNullException("dialPlanId");
				}
				QueryFilter filter = new AndFilter(new QueryFilter[]
				{
					new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.PilotIdentifierList, pilotIdentifier),
					new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.UMDialPlan, dialPlanId)
				});
				UMAutoAttendant[] array = this.InvokeWithStopwatch<UMAutoAttendant[]>("IConfigurationSession.Find<UMAutoAttendant>", () => this.session.Find<UMAutoAttendant>(null, QueryScope.SubTree, filter, null, 0));
				switch (array.Length)
				{
				case 0:
					return null;
				case 1:
					return array[0];
				default:
					throw new NonUniquePilotIdentifierException(pilotIdentifier, dialPlanId.ToString());
				}
			}

			public UMAutoAttendant GetAutoAttendantWithNoDialplanInformation(string pilotIdentifier)
			{
				if (pilotIdentifier == null)
				{
					throw new ArgumentNullException("pilotIdentifier");
				}
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.PilotIdentifierList, pilotIdentifier);
				UMAutoAttendant[] array = this.InvokeWithStopwatch<UMAutoAttendant[]>("IConfigurationSession.Find<UMAutoAttendant>", () => this.session.Find<UMAutoAttendant>(null, QueryScope.SubTree, filter, null, 0));
				if (array.Length == 1)
				{
					return array[0];
				}
				return null;
			}

			public OrganizationId GetOrganizationIdFromDomainName(string domainName, out bool isAuthoritative)
			{
				AcceptedDomain acceptedDomain = this.InvokeWithStopwatch<AcceptedDomain>("IConfigurationSession.GetAcceptedDomainByDomainName", () => this.session.GetAcceptedDomainByDomainName(domainName));
				isAuthoritative = (acceptedDomain != null && acceptedDomain.DomainType == AcceptedDomainType.Authoritative);
				if (acceptedDomain == null)
				{
					return null;
				}
				return acceptedDomain.OrganizationId;
			}

			public MicrosoftExchangeRecipient GetMicrosoftExchangeRecipient()
			{
				return this.InvokeWithStopwatch<MicrosoftExchangeRecipient>("IConfigurationSession.GetMicrosoftExchangeRecipient", () => this.session.FindMicrosoftExchangeRecipient());
			}

			public AcceptedDomain GetDefaultAcceptedDomain()
			{
				return this.InvokeWithStopwatch<AcceptedDomain>("IConfigurationSession.GetDefaultAcceptedDomain", () => this.session.GetDefaultAcceptedDomain());
			}

			public ExchangeConfigurationUnit GetConfigurationUnitByADObjectId(ADObjectId configUnit)
			{
				ValidateArgument.NotNull(configUnit, "ConfigurationUnit");
				return this.InvokeWithStopwatch<ExchangeConfigurationUnit>("IConfigurationSession.Read<ExchangeConfigurationUnit>", () => this.session.Read<ExchangeConfigurationUnit>(configUnit));
			}

			public UMMailboxPolicy GetPolicyFromRecipient(ADRecipient recipient)
			{
				if (recipient == null)
				{
					throw new ArgumentNullException("recipient");
				}
				ADUser user = recipient as ADUser;
				if (user == null)
				{
					PIIMessage data = PIIMessage.Create(PIIType._SmtpAddress, recipient.PrimarySmtpAddress);
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "Recipient _SmtpAddress[{0}] is not an ADUser.", new object[]
					{
						recipient.RecipientType
					});
					return null;
				}
				if (user.UMMailboxPolicy == null)
				{
					PIIMessage data2 = PIIMessage.Create(PIIType._SmtpAddress, recipient.PrimarySmtpAddress);
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data2, "Recipient _SmtpAddress does not have a UM mailbox policy.", new object[0]);
					return null;
				}
				UMMailboxPolicy ummailboxPolicy = this.InvokeWithStopwatch<UMMailboxPolicy>("IConfigurationSession.Read<UMMailboxPolicy>", () => this.session.Read<UMMailboxPolicy>(user.UMMailboxPolicy));
				if (ummailboxPolicy == null)
				{
					PIIMessage data3 = PIIMessage.Create(PIIType._SmtpAddress, recipient.PrimarySmtpAddress);
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data3, "Cannot find mailbox policy with id {0} for recipient _SmtpAddress.", new object[]
					{
						user.UMMailboxPolicy.DistinguishedName
					});
					return null;
				}
				return ummailboxPolicy;
			}

			public UMMailboxPolicy GetUMMailboxPolicyFromId(ADObjectId mbxPolicyId)
			{
				ValidateArgument.NotNull(mbxPolicyId, "mbxPolicyId");
				return this.InvokeWithStopwatch<UMMailboxPolicy>("IConfigurationSession.Read<UMMailboxPolicy>", () => this.session.Read<UMMailboxPolicy>(mbxPolicyId));
			}

			public UMAutoAttendant GetAutoAttendantFromName(string autoAttendantName)
			{
				if (autoAttendantName == null)
				{
					throw new ArgumentNullException("autoAttendantName");
				}
				UMAutoAttendant result = null;
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, ADObjectSchema.Name, autoAttendantName);
				UMAutoAttendant[] array = this.InvokeWithStopwatch<UMAutoAttendant[]>("IConfigurationSession.Find<UMAutoAttendant>", () => this.session.Find<UMAutoAttendant>(null, QueryScope.SubTree, filter, null, 0));
				if (array != null && array.Length == 1)
				{
					result = array[0];
				}
				return result;
			}

			public IEnumerable<Guid> GetAutoAttendantDialPlans()
			{
				IEnumerable<UMAutoAttendant> enumerable = this.InvokeWithStopwatch<ADPagedReader<UMAutoAttendant>>("IConfigurationSession.FindPaged<UMAutoAttendant>", () => this.session.FindPaged<UMAutoAttendant>(null, QueryScope.SubTree, null, null, 100));
				List<Guid> list = new List<Guid>();
				if (enumerable != null)
				{
					foreach (UMAutoAttendant umautoAttendant in enumerable)
					{
						if (!list.Contains(umautoAttendant.UMDialPlan.ObjectGuid))
						{
							list.Add(umautoAttendant.UMDialPlan.ObjectGuid);
						}
					}
				}
				return list;
			}

			public void GetAutoAttendantAddressLists(HashSet<Guid> addressListGuids)
			{
				ValidateArgument.NotNull(addressListGuids, "addressListGuids");
				QueryFilter filter = new ComparisonFilter(ComparisonOperator.Equal, UMAutoAttendantSchema.ContactScope, DialScopeEnum.AddressList);
				IEnumerable<UMAutoAttendant> enumerable = this.InvokeWithStopwatch<ADPagedReader<UMAutoAttendant>>("IConfigurationSession.FindPaged<UMAutoAttendant>", () => this.session.FindPaged<UMAutoAttendant>(null, QueryScope.SubTree, filter, null, 100));
				if (enumerable != null)
				{
					foreach (UMAutoAttendant umautoAttendant in enumerable)
					{
						if (umautoAttendant.ContactAddressList != null)
						{
							addressListGuids.Add(umautoAttendant.ContactAddressList.ObjectGuid);
						}
					}
				}
			}

			public Guid GetExternalDirectoryOrganizationId()
			{
				Guid result = Guid.Empty;
				if (this.isDatacenter)
				{
					ExchangeConfigurationUnit exchangeConfigurationUnit = this.InvokeWithStopwatch<ExchangeConfigurationUnit>("IConfigurationSession.Read<ExchangeConfigurationUnit>", () => this.session.Read<ExchangeConfigurationUnit>(this.session.SessionSettings.CurrentOrganizationId.ConfigurationUnit));
					if (exchangeConfigurationUnit == null)
					{
						throw new TenantOrgContainerNotFoundException(this.session.SessionSettings.CurrentOrganizationId.ToString());
					}
					string externalDirectoryOrganizationId = exchangeConfigurationUnit.ExternalDirectoryOrganizationId;
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "idStr='{0}' for org='{1}'", new object[]
					{
						externalDirectoryOrganizationId,
						this.session.SessionSettings.CurrentOrganizationId
					});
					result = Guid.Parse(externalDirectoryOrganizationId);
				}
				return result;
			}

			public void GetGlobalAddressLists(HashSet<Guid> addressListGuids)
			{
				ValidateArgument.NotNull(addressListGuids, "addressListGuids");
				IEnumerable<AddressBookMailboxPolicy> enumerable = this.InvokeWithStopwatch<ADPagedReader<AddressBookMailboxPolicy>>("IConfigurationSession.FindPaged<AddressBookMailboxPolicy>", () => this.session.FindPaged<AddressBookMailboxPolicy>(null, QueryScope.SubTree, null, null, 100));
				if (enumerable != null)
				{
					foreach (AddressBookMailboxPolicy addressBookMailboxPolicy in enumerable)
					{
						if (addressBookMailboxPolicy.GlobalAddressList != null)
						{
							addressListGuids.Add(addressBookMailboxPolicy.GlobalAddressList.ObjectGuid);
						}
					}
				}
			}

			private IEnumerable<UMIPGateway> GetIPGateways(Func<UMIPGateway, bool> predicate)
			{
				IEnumerable<UMIPGateway> allIPGateways = this.GetAllIPGateways();
				return allIPGateways.Where(predicate);
			}

			public IEnumerable<UMIPGateway> GetAllIPGateways()
			{
				return this.InvokeWithStopwatch<ADPagedReader<UMIPGateway>>("IConfigurationSession.FindPaged<UMIPGateway>", () => this.session.FindPaged<UMIPGateway>(null, QueryScope.SubTree, null, null, 100));
			}

			private void ValidateOrgScopeInDataCenter()
			{
				OrganizationId currentOrganizationId = this.session.SessionSettings.CurrentOrganizationId;
				if (this.isDatacenter && OrganizationId.ForestWideOrgId.Equals(currentOrganizationId))
				{
					ExAssert.RetailAssert(false, "Incorrectly scoped session - OrganizationalUnit = '{0}', ConfigurationUnit = '{1}'. Both OrganizationalUnit and ConfigurationUnit should be non-null.", new object[]
					{
						(currentOrganizationId.OrganizationalUnit != null) ? currentOrganizationId.OrganizationalUnit.ToString() : "<null>",
						(currentOrganizationId.ConfigurationUnit != null) ? currentOrganizationId.ConfigurationUnit.ToString() : "<null>"
					});
				}
			}

			private void ValidateTenantLocalScope()
			{
				ExAssert.RetailAssert(this.session.ConfigScope == ConfigScopes.TenantLocal, "Incorrectly scoped session - ConfigScope is not TenantLocal");
			}

			private T InvokeWithStopwatch<T>(string operationName, Func<T> func)
			{
				return this.latencyStopwatch.Invoke<T>(operationName, func);
			}

			private const int ADReadPageSize = 100;

			private LatencyStopwatch latencyStopwatch;

			private readonly IConfigurationSession session;

			private readonly bool isDatacenter;
		}
	}
}
