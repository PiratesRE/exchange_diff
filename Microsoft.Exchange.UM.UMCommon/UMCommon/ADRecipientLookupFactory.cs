using System;
using System.Collections.Generic;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.UnifiedMessaging;

namespace Microsoft.Exchange.UM.UMCommon
{
	internal class ADRecipientLookupFactory
	{
		public static IADRecipientLookup CreateFromUmUser(UMRecipient user)
		{
			if (user == null)
			{
				throw new ArgumentNullException("user");
			}
			if (user.ADRecipient == null)
			{
				throw new ArgumentException("user has no ADRecipient");
			}
			return ADRecipientLookupFactory.CreateFromADRecipient(user.ADRecipient, true);
		}

		public static IADRecipientLookup CreateFromADRecipient(IADRecipient recipient, bool readOnly)
		{
			if (recipient == null)
			{
				throw new ArgumentNullException("recipient");
			}
			ADObjectId addressListScope = null;
			if (recipient.AddressBookPolicy != null)
			{
				addressListScope = recipient.GlobalAddressListFromAddressBookPolicy;
			}
			return ADRecipientLookupFactory.CreateFromOrganizationId(recipient.OrganizationId, null, addressListScope, readOnly);
		}

		public static IADRecipientLookup CreateFromOrganizationId(OrganizationId orgId, ADObjectId searchRoot)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			return ADRecipientLookupFactory.CreateFromOrganizationId(orgId, searchRoot, null, true);
		}

		public static IADRecipientLookup CreateFromOrganizationId(OrganizationId orgId, ADObjectId searchRoot, ADObjectId addressListScope, bool readOnly)
		{
			if (orgId == null)
			{
				throw new ArgumentNullException("orgId");
			}
			return new ADRecipientLookupFactory.ADRecipientLookup(orgId, searchRoot, addressListScope, readOnly);
		}

		public static IADRecipientLookup CreateFromTenantGuid(Guid tenantGuid)
		{
			return new ADRecipientLookupFactory.ADRecipientLookup(tenantGuid);
		}

		public static IADRecipientLookup CreateFromExistingSession(IRecipientSession session, bool isDatacenter)
		{
			if (session == null)
			{
				throw new ArgumentNullException("session");
			}
			return new ADRecipientLookupFactory.ADRecipientLookup(session, isDatacenter);
		}

		public static IADRecipientLookup CreateUmProxyAddressLookup(UMDialPlan dialPlan)
		{
			ValidateArgument.NotNull(dialPlan, "dialPlan");
			CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Creating recipient lookup object scoped to organization of dial plan '{0}'", new object[]
			{
				dialPlan.DistinguishedName
			});
			return ADRecipientLookupFactory.CreateFromOrganizationId(dialPlan.OrganizationId, null);
		}

		private class ADRecipientLookup : IADRecipientLookup
		{
			public ADRecipientLookup(Guid tenantGuid)
			{
				this.latencyStopwatch = new LatencyStopwatch();
				this.isDatacenter = CommonConstants.DataCenterADPresent;
				base..ctor();
				if (this.isDatacenter && tenantGuid == Guid.Empty)
				{
					throw new InvalidTenantGuidException(tenantGuid);
				}
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Creating recipient lookup object - Tenant Guid = '{0}'", new object[]
				{
					tenantGuid
				});
				ADSessionSettings sessionSettings;
				if (this.isDatacenter)
				{
					try
					{
						sessionSettings = ADSessionSettings.FromExternalDirectoryOrganizationId(tenantGuid);
						goto IL_89;
					}
					catch (CannotResolveExternalDirectoryOrganizationIdException innerException)
					{
						throw new InvalidTenantGuidException(tenantGuid, innerException);
					}
				}
				sessionSettings = ADSessionSettings.FromRootOrgScopeSet();
				IL_89:
				this.session = this.InvokeWithStopwatch<IRecipientSession>("DirectorySessionFactory.GetTenantOrRootOrgRecipientSession", () => DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, null, 0, true, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 406, ".ctor", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADRecipientLookup.cs"));
				this.ValidateOrgScopeInDataCenter();
			}

			public ADRecipientLookup(OrganizationId orgId, ADObjectId searchRoot, ADObjectId addressListScope, bool readOnly)
			{
				ADRecipientLookupFactory.ADRecipientLookup.<>c__DisplayClass4 CS$<>8__locals1 = new ADRecipientLookupFactory.ADRecipientLookup.<>c__DisplayClass4();
				CS$<>8__locals1.searchRoot = searchRoot;
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
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Creating recipient lookup object - Organization OU = '{0}', Organization CU = '{1}', Search Root = '{2}', Address List Scope = '{3}'", new object[]
				{
					(orgId.OrganizationalUnit != null) ? orgId.OrganizationalUnit.ToString() : "<null>",
					(orgId.ConfigurationUnit != null) ? orgId.ConfigurationUnit.ToString() : "<null>",
					(CS$<>8__locals1.searchRoot != null) ? CS$<>8__locals1.searchRoot.ToString() : "<null>",
					(addressListScope != null) ? addressListScope.ToString() : "<null>"
				});
				ADSessionSettings sessionSettings;
				if (addressListScope == null)
				{
					sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, null, false);
				}
				else
				{
					sessionSettings = ADSessionSettings.FromOrganizationIdWithAddressListScope(ADSystemConfigurationSession.GetRootOrgContainerIdForLocalForest(), orgId, addressListScope, null);
				}
				this.session = this.InvokeWithStopwatch<IRecipientSession>("DirectorySessionFactory.GetTenantOrRootOrgRecipientSession", () => DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, CS$<>8__locals1.searchRoot, 0, CS$<>8__locals1.readOnly, ConsistencyMode.IgnoreInvalid, null, sessionSettings, 475, ".ctor", "f:\\15.00.1497\\sources\\dev\\um\\src\\umcommon\\ADRecipientLookup.cs"));
				this.ValidateOrgScopeInDataCenter();
			}

			public ADRecipientLookup(IRecipientSession session, bool isDatacenter)
			{
				this.latencyStopwatch = new LatencyStopwatch();
				this.isDatacenter = CommonConstants.DataCenterADPresent;
				base..ctor();
				this.session = session;
				this.isDatacenter = isDatacenter;
			}

			public IRecipientSession ScopedRecipientSession
			{
				get
				{
					return this.session;
				}
			}

			public ADUser GetUMDataStorageMailbox()
			{
				this.ValidateTenantLocalScope();
				List<ADUser> organizationMailboxesByCapability = OrganizationMailbox.GetOrganizationMailboxesByCapability(this.session, OrganizationCapability.UMDataStorage);
				return CommonUtil.ValidateAndReturnUMDataStorageOrgMbx(organizationMailboxesByCapability);
			}

			public ADRecipient LookupByExchangeGuid(Guid exchangeGuid)
			{
				return this.InvokeWithStopwatch<ADRecipient>("IRecipientSession.FindByExchangeGuid", () => this.session.FindByExchangeGuid(exchangeGuid));
			}

			public ADRecipient LookupByObjectId(ADObjectId objectId)
			{
				return this.InvokeWithStopwatch<ADRecipient>("IRecipientSession.Read", () => this.session.Read(objectId));
			}

			public ADRecipient LookupByExtensionAndDialPlan(string extension, UMDialPlan dialPlan)
			{
				return this.GetADRecipientFromExtensionAndPhoneContext(this.session, extension, dialPlan.PhoneContext);
			}

			public ADRecipient LookupByExtensionAndEquivalentDialPlan(string extension, UMDialPlan dialPlan)
			{
				return this.GetADRecipientFromExtensionAndEquivalentDialPlan(this.session, extension, dialPlan);
			}

			public ADRecipient LookupByExchangePrincipal(IExchangePrincipal exchangePrincipal)
			{
				return this.InvokeWithStopwatch<ADRecipient>("IRecipientSession.FindByExchangeGuid", () => this.session.FindByExchangeGuid(exchangePrincipal.MailboxInfo.MailboxGuid));
			}

			public ADRecipient LookupByLegacyExchangeDN(string legacyExchangeDN)
			{
				return this.InvokeWithStopwatch<ADRecipient>("IRecipientSession.FindByLegacyExchangeDN", () => this.session.FindByLegacyExchangeDN(legacyExchangeDN));
			}

			public ADRecipient LookupBySmtpAddress(string emailAddress)
			{
				string proxyAddressString = "SMTP:" + emailAddress;
				ProxyAddress proxyAddress = ProxyAddress.Parse(proxyAddressString);
				return this.LookupByProxyAddress(proxyAddress);
			}

			public ADRecipient[] LookupBySmtpAddresses(List<string> smtpAddresses)
			{
				ValidateArgument.NotNull(smtpAddresses, "smtpAddresses");
				List<ProxyAddress> list = new List<ProxyAddress>(smtpAddresses.Count);
				foreach (string str in smtpAddresses)
				{
					string proxyAddressString = "SMTP:" + str;
					list.Add(ProxyAddress.Parse(proxyAddressString));
				}
				return this.LookupByProxyAddresses(list.ToArray());
			}

			public ADRecipient LookupByUmAddress(string proxyAddressStr)
			{
				if (proxyAddressStr == null)
				{
					throw new ArgumentNullException("proxyAddressStr");
				}
				if (!proxyAddressStr.StartsWith(ProxyAddressPrefix.UM.PrimaryPrefix, StringComparison.OrdinalIgnoreCase))
				{
					throw new ArgumentOutOfRangeException("proxyAddressStr", proxyAddressStr, "proxyAddressStr is not a valid UM proxy address string");
				}
				ProxyAddress proxyAddress = ProxyAddress.Parse(proxyAddressStr);
				return this.LookupByProxyAddress(proxyAddress);
			}

			public ADRecipient LookupByParticipant(Participant p)
			{
				ADRecipient adrecipient = null;
				try
				{
					string routingType;
					if ((routingType = p.RoutingType) != null)
					{
						if (!(routingType == "EX"))
						{
							if (routingType == "SMTP")
							{
								if (p.EmailAddress != null && SmtpAddress.IsValidSmtpAddress(p.EmailAddress))
								{
									SmtpProxyAddress proxyAddress = new SmtpProxyAddress(p.EmailAddress, true);
									adrecipient = this.LookupByProxyAddress(proxyAddress);
								}
							}
						}
						else
						{
							adrecipient = this.InvokeWithStopwatch<ADRecipient>("IRecipientSession.FindByLegacyExchangeDN", () => this.session.FindByLegacyExchangeDN(p.EmailAddress));
						}
					}
				}
				catch (RusOperationException)
				{
				}
				if (adrecipient == null)
				{
					PIIMessage data = PIIMessage.Create(PIIType._PII, p);
					CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "Recipient _PII is not found.", new object[0]);
				}
				return adrecipient;
			}

			public ADRecipient LookupBySipExtension(string extension)
			{
				string proxyAddressString = "SIP:" + extension;
				ProxyAddress proxyAddress = ProxyAddress.Parse(proxyAddressString);
				ADRecipient adrecipient = this.LookupByProxyAddress(proxyAddress);
				if (adrecipient == null)
				{
					proxyAddressString = "SIP:".ToLowerInvariant() + extension;
					proxyAddress = ProxyAddress.Parse(proxyAddressString);
					adrecipient = this.LookupByProxyAddress(proxyAddress);
				}
				return adrecipient;
			}

			public ADRecipient LookupByEumSipResourceIdentifierPrefix(string sipUri)
			{
				PIIMessage data = PIIMessage.Create(PIIType._Uri, sipUri);
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, data, "LookupByEumSipResourceIdentifierPrefix _Uri", new object[0]);
				sipUri = Utils.RemoveSIPPrefix(sipUri);
				if (!Utils.IsUriValid(sipUri, UMUriType.SipName))
				{
					data = PIIMessage.Create(PIIType._Uri, sipUri);
					CallIdTracer.TraceError(ExTraceGlobals.UtilTracer, 0, data, "Invalid sip uri _Uri", new object[0]);
					return null;
				}
				ADRecipient[] array = this.LookupByQueryFilter(ADRecipientLookupFactory.ADRecipientLookup.GetEumSipResourceIdentifierPrefixFilter(sipUri));
				if (array.Length == 0)
				{
					return null;
				}
				if (array.Length == 1)
				{
					return array[0];
				}
				ValidationError error = new NonUniqueProxyAddressError(DirectoryStrings.ErrorNonUniqueProxy(sipUri), array[0].Id, string.Empty);
				throw new NonUniqueRecipientException(sipUri, error);
			}

			public ADRecipient[] LookupByDtmfMap(string mode, string dtmf, bool removeHiddenUsers, bool anonymousCaller, UMDialPlan targetDialPlan, int numberOfResults)
			{
				return this.FindByDtmfMap(mode, dtmf, removeHiddenUsers, anonymousCaller, targetDialPlan, numberOfResults);
			}

			public ADRecipient[] LookupByQueryFilter(QueryFilter filter)
			{
				return this.InvokeWithStopwatch<ADRecipient[]>("IRecipientSession.Find", () => this.session.Find(null, QueryScope.SubTree, filter, null, 0));
			}

			public void ProcessRecipients(QueryFilter recipientFilter, PropertyDefinition[] properties, ADConfigurationProcessor<ADRawEntry> configurationProcessor, int retryCount)
			{
				ADNotificationAdapter.ReadConfigurationPaged<ADRawEntry>(() => this.InvokeWithStopwatch<ADPagedReader<ADRawEntry>>("IRecipientSession.FindPagedADRawEntryWithDefaultFilters", () => this.session.FindPagedADRawEntryWithDefaultFilters<ADRecipient>(null, QueryScope.SubTree, recipientFilter, null, 100, properties)), configurationProcessor, retryCount);
			}

			public bool TenantSizeExceedsThreshold(QueryFilter filter, int threshold)
			{
				PropertyDefinition[] properties = new PropertyDefinition[]
				{
					ADObjectSchema.Guid
				};
				ADRawEntry[] array = this.InvokeWithStopwatch<ADRawEntry[]>("IRecipientSession.Find", () => this.session.Find(null, QueryScope.SubTree, filter, null, threshold + 1, properties));
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, null, "Count of retrieved entries for org '{0}' is {1}", new object[]
				{
					this.session.SessionSettings.CurrentOrganizationId,
					array.Length
				});
				return array.Length > threshold;
			}

			private static QueryFilter GetEumSipResourceIdentifierPrefixFilter(string sipResourceIdentifier)
			{
				ProxyAddress proxyAddress = ProxyAddress.Parse(ProxyAddressPrefix.UM.PrimaryPrefix, EumAddress.BuildAddressString(sipResourceIdentifier, string.Empty));
				return new TextFilter(ADRecipientSchema.EmailAddresses, proxyAddress.ToString(), MatchOptions.Prefix, MatchFlags.IgnoreCase);
			}

			private ADRecipient GetADRecipientFromExtensionAndPhoneContext(IRecipientSession session, string extension, string phoneContext)
			{
				ProxyAddress proxy = UMMailbox.BuildProxyAddressFromExtensionAndPhoneContext(extension, ProxyAddressPrefix.UM.SecondaryPrefix, phoneContext);
				return this.InvokeWithStopwatch<ADRecipient>("IRecipientSession.FindByProxyAddress", () => session.FindByProxyAddress(proxy));
			}

			private ADRecipient GetADRecipientFromExtensionAndEquivalentDialPlan(IRecipientSession session, string extension, UMDialPlan dialPlan)
			{
				ADRecipient result = null;
				if (dialPlan != null)
				{
					if (dialPlan.EquivalentDialPlanPhoneContexts == null || dialPlan.EquivalentDialPlanPhoneContexts.Count == 0)
					{
						result = this.GetADRecipientFromExtensionAndPhoneContext(session, extension, dialPlan.PhoneContext);
					}
					else
					{
						ProxyAddress[] proxies = new ProxyAddress[1 + dialPlan.EquivalentDialPlanPhoneContexts.Count];
						proxies[0] = UMMailbox.BuildProxyAddressFromExtensionAndPhoneContext(extension, ProxyAddressPrefix.UM.SecondaryPrefix, dialPlan.PhoneContext);
						for (int i = 0; i < dialPlan.EquivalentDialPlanPhoneContexts.Count; i++)
						{
							proxies[i + 1] = UMMailbox.BuildProxyAddressFromExtensionAndPhoneContext(extension, ProxyAddressPrefix.UM.SecondaryPrefix, dialPlan.EquivalentDialPlanPhoneContexts[i]);
						}
						Result<ADRecipient>[] array = this.InvokeWithStopwatch<Result<ADRecipient>[]>("IRecipientSession.FindByProxyAddresses", () => session.FindByProxyAddresses(proxies));
						for (int j = 0; j < array.Length; j++)
						{
							if (array[j].Error == null && array[j].Data != null)
							{
								result = array[j].Data;
								break;
							}
						}
					}
				}
				return result;
			}

			private void ValidateTenantLocalScope()
			{
				ExAssert.RetailAssert(this.session.ConfigScope == ConfigScopes.TenantLocal, "Incorrectly scoped session - ConfigScope is not TenantLocal");
			}

			private ADRecipient LookupByProxyAddress(ProxyAddress proxyAddress)
			{
				return this.InvokeWithStopwatch<ADRecipient>("IRecipientSession.FindByProxyAddress", () => this.session.FindByProxyAddress(proxyAddress));
			}

			private ADRecipient[] LookupByProxyAddresses(ProxyAddress[] proxyAddresses)
			{
				List<ADRecipient> list = new List<ADRecipient>(proxyAddresses.Length);
				Result<ADRecipient>[] array = this.InvokeWithStopwatch<Result<ADRecipient>[]>("IRecipientSession.FindByProxyAddresses", () => this.session.FindByProxyAddresses(proxyAddresses));
				foreach (Result<ADRecipient> result in array)
				{
					list.Add(result.Data);
				}
				return list.ToArray();
			}

			private ADRecipient[] FindByDtmfMap(string mode, string dtmf, bool removeHiddenUsers, bool anonymousCaller, UMDialPlan targetDialPlan, int numberOfResults)
			{
				string text = mode + dtmf;
				List<QueryFilter> list = new List<QueryFilter>();
				if (text.EndsWith("*"))
				{
					text = text.TrimEnd(new char[]
					{
						'*'
					});
					list.Add(new TextFilter(ADRecipientSchema.UMDtmfMap, text, MatchOptions.Prefix, MatchFlags.Default));
				}
				else
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.UMDtmfMap, text));
				}
				if (removeHiddenUsers)
				{
					list.Add(ADRecipientLookupFactory.ADRecipientLookup.HiddenUsersFilter);
				}
				if (anonymousCaller)
				{
					list.Add(ADRecipientLookupFactory.ADRecipientLookup.AnonymousCallerRecipientFilter);
				}
				if (targetDialPlan != null)
				{
					list.Add(new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.UMRecipientDialPlanId, targetDialPlan.Id));
				}
				AndFilter andFilter = new AndFilter(list.ToArray());
				CallIdTracer.TraceDebug(ExTraceGlobals.UtilTracer, this, "ADRecipientLookup::FindByDtmfMap() filter = '{0}'", new object[]
				{
					andFilter.GenerateInfixString(FilterLanguage.Monad)
				});
				return this.InvokeWithStopwatch<ADRecipient[]>("IRecipientSession.Find", () => this.session.Find(null, QueryScope.SubTree, andFilter, null, numberOfResults));
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

			protected T InvokeWithStopwatch<T>(string operationName, Func<T> func)
			{
				return this.latencyStopwatch.Invoke<T>(operationName, func);
			}

			private const int ADReadPageSize = 100;

			private LatencyStopwatch latencyStopwatch;

			private static readonly QueryFilter HiddenUsersFilter = new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.HiddenFromAddressListsEnabled, false);

			private static readonly QueryFilter AnonymousCallerRecipientFilter = new OrFilter(new QueryFilter[]
			{
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.User),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.UserMailbox),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailContact),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.MailUser),
				new ComparisonFilter(ComparisonOperator.Equal, ADRecipientSchema.RecipientType, RecipientType.Contact)
			});

			private readonly bool isDatacenter;

			private readonly IRecipientSession session;
		}
	}
}
