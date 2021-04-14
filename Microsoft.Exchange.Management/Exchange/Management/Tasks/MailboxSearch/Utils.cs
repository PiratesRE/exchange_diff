using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.KqlParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.InfoWorker.Common.SearchService;
using Microsoft.Exchange.Rpc;
using Microsoft.Exchange.Rpc.MailboxSearch;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.Tasks.MailboxSearch
{
	internal static class Utils
	{
		internal static IRecipientSession CreateRecipientSession(Fqdn domainController, ADSessionSettings sessionSettings)
		{
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.PartiallyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 92, "CreateRecipientSession", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Search\\Utils.cs");
			if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
			{
				tenantOrRootOrgRecipientSession.SessionSettings.IncludeInactiveMailbox = true;
			}
			tenantOrRootOrgRecipientSession.LinkResolutionServer = ADSession.GetCurrentConfigDC(tenantOrRootOrgRecipientSession.SessionSettings.GetAccountOrResourceForestFqdn());
			tenantOrRootOrgRecipientSession.UseGlobalCatalog = true;
			return tenantOrRootOrgRecipientSession;
		}

		internal static MailboxDataProvider CreateMailboxDataProvider(IRecipientSession recipientSession, Task.TaskErrorLoggingDelegate writeErrorDelegate)
		{
			MailboxDataProvider result = null;
			try
			{
				result = new MailboxDataProvider(recipientSession);
			}
			catch (ObjectNotFoundException ex)
			{
				writeErrorDelegate(new MailboxSearchTaskException(ex.LocalizedString), ErrorCategory.ObjectNotFound, null);
			}
			catch (NonUniqueRecipientException ex2)
			{
				writeErrorDelegate(new MailboxSearchTaskException(ex2.LocalizedString), ErrorCategory.NotSpecified, null);
			}
			return result;
		}

		internal static void CheckDiscoveryEnabled(IRecipientSession recipientSession, Task.TaskErrorLoggingDelegate writeErrorDelegate)
		{
			if (!SearchUtils.DiscoveryEnabled(recipientSession))
			{
				writeErrorDelegate(new TaskException(Strings.MailboxSearchDisabled), ErrorCategory.ResourceUnavailable, null);
			}
		}

		private static bool IsNullOrEmpty<T>(this List<T> list)
		{
			return list == null || list.Count == 0;
		}

		internal static MultiValuedProperty<TRawId> ConvertCollectionToMultiValedProperty<TIdentity, TRawId>(IEnumerable<TIdentity> identities, Utils.IdentityToRawIdDelegate<TIdentity, TRawId> identityToRawIdDelegate, object delegateParam, MultiValuedProperty<TRawId> existingProperty, Task.TaskErrorLoggingDelegate writeErrorDelegate, string parameterName)
		{
			if (identities == null)
			{
				return existingProperty;
			}
			MultiValuedProperty<TRawId> multiValuedProperty = existingProperty ?? new MultiValuedProperty<TRawId>();
			foreach (TIdentity identity in identities)
			{
				TRawId item = identityToRawIdDelegate(identity, delegateParam);
				if (!multiValuedProperty.Contains(item))
				{
					multiValuedProperty.Add(item);
				}
				else
				{
					writeErrorDelegate(new MailboxSearchTaskException(Strings.CollectionConflictionError(identity.ToString(), parameterName)), ErrorCategory.InvalidArgument, null);
				}
			}
			return multiValuedProperty;
		}

		internal static MultiValuedProperty<string> ConvertSourceMailboxesCollection(IEnumerable<RecipientIdParameter> recipientIds, bool inplaceHoldEnabled, Func<RecipientIdParameter, ADRecipient> recipientGetter, IRecipientSession recipientSession, Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate, Func<LocalizedString, bool> shouldContinueDelegate)
		{
			MultiValuedProperty<string> results = null;
			if (recipientIds != null)
			{
				RecipientType[] source = new RecipientType[]
				{
					RecipientType.UserMailbox,
					RecipientType.Group,
					RecipientType.MailUniversalDistributionGroup,
					RecipientType.MailUniversalSecurityGroup,
					RecipientType.MailNonUniversalGroup,
					RecipientType.DynamicDistributionGroup
				};
				bool flag = false;
				foreach (RecipientIdParameter arg in recipientIds)
				{
					if (results == null)
					{
						results = new MultiValuedProperty<string>();
					}
					ADRecipient adrecipient = recipientGetter(arg);
					string text = null;
					ADSessionSettings sessionSettings = adrecipient.OrganizationId.ToADSessionSettings();
					if (Utils.IsPublicFolderMailbox(adrecipient))
					{
						writeErrorDelegate(new MailboxSearchTaskException(Strings.ErrorInvalidRecipientTypeDetails(adrecipient.ToString())), ErrorCategory.InvalidArgument, null);
					}
					else if (Utils.IsValidMailboxType(adrecipient))
					{
						if (adrecipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012) && !RemoteMailbox.IsRemoteMailbox(adrecipient.RecipientTypeDetails) && ExchangePrincipal.FromADUser(sessionSettings, (ADUser)adrecipient, RemotingOptions.AllowCrossSite).MailboxInfo.Location.ServerVersion < Server.E15MinVersion)
						{
							writeErrorDelegate(new MailboxSearchTaskException(Strings.SourceMailboxMustBeE15OrLater(adrecipient.DisplayName)), ErrorCategory.InvalidArgument, null);
						}
						text = adrecipient.LegacyExchangeDN;
					}
					else if (source.Contains(adrecipient.RecipientType))
					{
						if (inplaceHoldEnabled)
						{
							if (!flag)
							{
								if (shouldContinueDelegate(Strings.ShouldContinueToExpandGroupsForHold))
								{
									flag = true;
								}
								else
								{
									writeErrorDelegate(new MailboxSearchTaskException(Strings.GroupsIsNotAllowedForHold(adrecipient.DisplayName)), ErrorCategory.InvalidArgument, null);
								}
							}
							bool invalidTypeSkipped = false;
							bool oldVersionMailboxSkipped = false;
							ADRecipientExpansion adrecipientExpansion = new ADRecipientExpansion(new PropertyDefinition[]
							{
								ADRecipientSchema.LegacyExchangeDN,
								ADRecipientSchema.RecipientTypeDetailsValue,
								ADObjectSchema.ExchangeVersion
							}, adrecipient.OrganizationId);
							adrecipientExpansion.Expand(adrecipient, delegate(ADRawEntry expandedRecipient, ExpansionType expansionType, ADRawEntry parent, ExpansionType parentType)
							{
								if (expansionType == ExpansionType.GroupMembership)
								{
									return ExpansionControl.Continue;
								}
								if (Utils.IsValidMailboxType(expandedRecipient))
								{
									ExchangeObjectVersion exchangeObjectVersion = (ExchangeObjectVersion)expandedRecipient[ADObjectSchema.ExchangeVersion];
									string text2 = (string)expandedRecipient[ADRecipientSchema.LegacyExchangeDN];
									if (!oldVersionMailboxSkipped && exchangeObjectVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012) && !RemoteMailbox.IsRemoteMailbox((RecipientTypeDetails)expandedRecipient[ADRecipientSchema.RecipientTypeDetails]) && ExchangePrincipal.FromLegacyDN(sessionSettings, text2).MailboxInfo.Location.ServerVersion < Server.E15MinVersion)
									{
										oldVersionMailboxSkipped = true;
										writeWarningDelegate(Strings.OldVersionMailboxSkipped);
									}
									if (!results.Contains(text2))
									{
										results.Add(text2);
									}
								}
								else if (!invalidTypeSkipped)
								{
									invalidTypeSkipped = true;
									writeWarningDelegate(Strings.SkippingInvalidTypeInGroupExpansion);
								}
								return ExpansionControl.Skip;
							}, delegate(ExpansionFailure failure, ADRawEntry expandedRecipient, ExpansionType expansionType, ADRawEntry parent, ExpansionType parentType)
							{
								TaskLogger.Trace("Skipping invalid AD entry {0} because of failure: {1}", new object[]
								{
									expandedRecipient,
									failure
								});
								return ExpansionControl.Skip;
							});
						}
						else
						{
							text = adrecipient.LegacyExchangeDN;
						}
					}
					else
					{
						writeErrorDelegate(new MailboxSearchTaskException(Strings.ErrorInvalidRecipientType(adrecipient.ToString(), adrecipient.RecipientType.ToString())), ErrorCategory.InvalidArgument, null);
					}
					if (text != null && !results.Contains(text))
					{
						results.Add(text);
					}
					if (results.Count > Utils.MaxNumberOfMailboxesInSingleHold)
					{
						writeErrorDelegate(new MailboxSearchTaskException(Strings.ErrorTooManyMailboxesInSingleHold(Utils.MaxNumberOfMailboxesInSingleHold)), ErrorCategory.InvalidArgument, null);
					}
				}
				if (results != null)
				{
					uint discoveryMaxMailboxes = SearchUtils.GetDiscoveryMaxMailboxes(recipientSession);
					if ((long)results.Count > (long)((ulong)discoveryMaxMailboxes) && !shouldContinueDelegate(Strings.ShouldContinueMoreMailboxesThanMaxSearch(results.Count, discoveryMaxMailboxes)))
					{
						writeErrorDelegate(new MailboxSearchTaskException(Strings.ErrorTaskCancelledBecauseMoreMailboxesThanSearchSupported), ErrorCategory.InvalidArgument, null);
					}
				}
			}
			return results;
		}

		internal static bool IsValidMailboxType(ADRawEntry recipient)
		{
			return (RecipientType)recipient[ADRecipientSchema.RecipientType] == RecipientType.UserMailbox || RemoteMailbox.IsRemoteMailbox((RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails]);
		}

		internal static bool IsPublicFolderMailbox(ADRawEntry recipient)
		{
			return (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails] == RecipientTypeDetails.PublicFolderMailbox;
		}

		internal static void VerifyIsInHoldScopes(bool isHoldEnabled, ExchangeRunspaceConfiguration runspaceConfig, ADRecipient sourceRecipient, string cmdlet, Task.TaskErrorLoggingDelegate writeErrorDelegate)
		{
			if (isHoldEnabled && !runspaceConfig.IsCmdletAllowedInScope(cmdlet, new string[]
			{
				"InPlaceHoldEnabled"
			}, sourceRecipient, ScopeLocation.RecipientWrite))
			{
				writeErrorDelegate(new TaskInvalidOperationException(Strings.InPlaceHoldScopeException(sourceRecipient.DisplayName)), ErrorCategory.InvalidOperation, null);
			}
		}

		internal static void VerifyIsInScopes(ADObject adObject, ScopeSet scopeSet, Task.TaskErrorLoggingDelegate writeErrorDelegate)
		{
			try
			{
				ADSession.VerifyIsWithinScopes(adObject, scopeSet.RecipientReadScope, scopeSet.RecipientWriteScopes, scopeSet.ExclusiveRecipientScopes, false);
			}
			catch (ADScopeException)
			{
				writeErrorDelegate(new MailboxSearchScopeException(adObject.Id.ToString()), ErrorCategory.PermissionDenied, null);
			}
		}

		internal static void VerifyMailboxVersion(ADObject adObject, Task.TaskErrorLoggingDelegate writeErrorDelegate)
		{
			if (adObject.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010))
			{
				writeErrorDelegate(new TaskException(Strings.ErrorMailboxVersionTooOld(adObject.Id.ToString())), ErrorCategory.ReadError, null);
			}
		}

		internal static bool VerifyMailboxVersionIsSP1OrGreater(ADUser adUser)
		{
			return adUser != null && ExchangePrincipal.FromADUser(adUser.OrganizationId.ToADSessionSettings(), adUser, RemotingOptions.AllowCrossSite).MailboxInfo.Location.ServerVersion >= Server.E14SP1MinVersion;
		}

		internal static string GetExecutingUserDisplayName(string executingUserDisplayName, ExchangeRunspaceConfiguration exchangeRunspaceConfig)
		{
			if (exchangeRunspaceConfig != null)
			{
				return exchangeRunspaceConfig.GetRbacContext().ExecutingUserName;
			}
			if (!string.IsNullOrEmpty(executingUserDisplayName))
			{
				return executingUserDisplayName;
			}
			return null;
		}

		internal static void ResetSearchResults(MailboxDiscoverySearch discoverySearchObject)
		{
			discoverySearchObject.PercentComplete = 0;
			discoverySearchObject.ResultItemCountCopied = 0L;
			discoverySearchObject.ResultItemCountEstimate = 0L;
			discoverySearchObject.ResultSizeCopied = 0L;
			discoverySearchObject.ResultSizeEstimate = 0L;
			discoverySearchObject.ResultsPath = null;
			discoverySearchObject.ResultsLink = null;
			discoverySearchObject.SearchStatistics = null;
			if (discoverySearchObject.Errors != null)
			{
				discoverySearchObject.Errors.Clear();
			}
			if (discoverySearchObject.CompletedMailboxes != null)
			{
				discoverySearchObject.CompletedMailboxes.Clear();
			}
		}

		internal static void CreateMailboxDiscoverySearchRequest(DiscoverySearchDataProvider dataProvider, string name, ActionRequestType requestType, string rbacContext)
		{
			dataProvider.CreateOrUpdate<MailboxDiscoverySearchRequest>(new MailboxDiscoverySearchRequest
			{
				Name = name,
				AsynchronousActionRequest = requestType,
				AsynchronousActionRbacContext = rbacContext
			});
		}

		internal static void CheckSearchRunningStatus(MailboxDiscoverySearch dataObject, Task.TaskErrorLoggingDelegate writeErrorDelegate, LocalizedString errorMessage)
		{
			if (dataObject.Status == SearchState.Queued || dataObject.Status == SearchState.EstimateInProgress || dataObject.Status == SearchState.InProgress || dataObject.Status == SearchState.EstimateStopping || dataObject.Status == SearchState.Stopping || dataObject.Status == SearchState.DeletionInProgress)
			{
				writeErrorDelegate(new MailboxSearchTaskException(errorMessage), ErrorCategory.InvalidOperation, dataObject);
			}
		}

		internal static Exception ValidateSourceAndTargetMailboxes(DiscoverySearchDataProvider dataProvider, MailboxDiscoverySearch dataObject)
		{
			if (dataObject.Sources.Contains(dataProvider.LegacyDistinguishedName))
			{
				return new MailboxSearchTaskException(ServerStrings.DiscoveryMailboxCannotBeSourceOrTarget(dataProvider.LegacyDistinguishedName));
			}
			if (!string.IsNullOrEmpty(dataObject.Target))
			{
				if (dataObject.Sources.Contains(dataObject.Target))
				{
					return new MailboxSearchTaskException(ServerStrings.SearchTargetInSource);
				}
				if (dataObject.Target.Equals(dataProvider.LegacyDistinguishedName))
				{
					return new MailboxSearchTaskException(ServerStrings.DiscoveryMailboxCannotBeSourceOrTarget(dataProvider.LegacyDistinguishedName));
				}
			}
			return null;
		}

		internal static bool IsLegacySearchObjectIdentity(string identity)
		{
			SearchObjectId searchObjectId;
			return identity != null && SearchObjectId.TryParse(identity, out searchObjectId);
		}

		internal static MailboxDataProvider GetMailboxDataProvider(ADObjectId rootOrg, OrganizationId currentOrg, OrganizationId executingOrg, Task.TaskErrorLoggingDelegate writeErrorDelegate)
		{
			MailboxDataProvider result = null;
			try
			{
				ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopes(rootOrg, currentOrg, executingOrg, true);
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 694, "GetMailboxDataProvider", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Search\\Utils.cs");
				result = new MailboxDataProvider(MailboxDataProvider.GetDiscoveryMailbox(tenantOrRootOrgRecipientSession), tenantOrRootOrgRecipientSession);
			}
			catch (ObjectNotFoundException exception)
			{
				writeErrorDelegate(exception, ErrorCategory.WriteError, null);
			}
			catch (NonUniqueRecipientException exception2)
			{
				writeErrorDelegate(exception2, ErrorCategory.WriteError, null);
			}
			return result;
		}

		internal static MailboxDiscoverySearch UpgradeLegacySearchObject(SearchObject searchObject, MailboxDataProvider e14DataProvider, DiscoverySearchDataProvider e15DataProvider, Task.TaskErrorLoggingDelegate writeErrorDelegate, Action<LocalizedString> writeWarningDelegate)
		{
			MailboxDiscoverySearch mailboxDiscoverySearch = new MailboxDiscoverySearch();
			if (e15DataProvider.Find<MailboxDiscoverySearch>(searchObject.Name) != null)
			{
				mailboxDiscoverySearch.Name = searchObject.Name + Guid.NewGuid().ToString();
			}
			else
			{
				mailboxDiscoverySearch.Name = searchObject.Name;
			}
			mailboxDiscoverySearch.Senders = searchObject.Senders;
			mailboxDiscoverySearch.CreatedBy = searchObject.CreatedByEx;
			mailboxDiscoverySearch.Description = Strings.UpgradedSearchObjectDescription;
			mailboxDiscoverySearch.EndDate = searchObject.EndDate;
			mailboxDiscoverySearch.StartDate = searchObject.StartDate;
			mailboxDiscoverySearch.ExcludeDuplicateMessages = searchObject.ExcludeDuplicateMessages;
			mailboxDiscoverySearch.StatisticsOnly = searchObject.EstimateOnly;
			mailboxDiscoverySearch.IncludeUnsearchableItems = searchObject.IncludeUnsearchableItems;
			mailboxDiscoverySearch.IncludeKeywordStatistics = searchObject.IncludeKeywordStatistics;
			mailboxDiscoverySearch.Language = searchObject.Language.ToString();
			mailboxDiscoverySearch.LogLevel = searchObject.LogLevel;
			mailboxDiscoverySearch.ManagedBy = searchObject.ManagedBy;
			mailboxDiscoverySearch.MessageTypes = searchObject.MessageTypes;
			mailboxDiscoverySearch.Query = Utils.ConvertAqsToKql(searchObject.SearchQuery, searchObject.Language);
			mailboxDiscoverySearch.Recipients = searchObject.Recipients;
			mailboxDiscoverySearch.StatusMailRecipients = searchObject.StatusMailRecipients;
			mailboxDiscoverySearch.ManagedByOrganization = e15DataProvider.OrganizationId.ToString();
			mailboxDiscoverySearch.LegacySearchObjectIdentity = searchObject.Identity.ToString();
			if (searchObject.SourceMailboxes != null && searchObject.SourceMailboxes.Count > 0)
			{
				MultiValuedProperty<string> multiValuedProperty = new MultiValuedProperty<string>();
				Result<ADRawEntry>[] first = e14DataProvider.RecipientSession.ReadMultiple(searchObject.SourceMailboxes.ToArray(), new PropertyDefinition[]
				{
					ADRecipientSchema.DisplayName,
					ADRecipientSchema.LegacyExchangeDN,
					ADObjectSchema.ExchangeVersion
				});
				foreach (ADRawEntry adrawEntry in from x in first
				select x.Data)
				{
					if (adrawEntry != null)
					{
						if (((ExchangeObjectVersion)adrawEntry[ADObjectSchema.ExchangeVersion]).IsOlderThan(ExchangeObjectVersion.Exchange2012))
						{
							writeErrorDelegate(new MailboxSearchTaskException(Strings.SourceMailboxMustBeE15OrLater((string)adrawEntry[ADRecipientSchema.DisplayName])), ErrorCategory.InvalidArgument, null);
						}
						else
						{
							multiValuedProperty.Add((string)adrawEntry[ADRecipientSchema.LegacyExchangeDN]);
						}
					}
				}
				if (multiValuedProperty.Count > 0)
				{
					mailboxDiscoverySearch.Sources = multiValuedProperty;
				}
			}
			ADUser aduser = null;
			if (searchObject.TargetMailbox != null)
			{
				aduser = (ADUser)e14DataProvider.RecipientSession.Read(searchObject.TargetMailbox);
				if (aduser != null)
				{
					mailboxDiscoverySearch.Target = aduser.LegacyExchangeDN;
				}
			}
			e15DataProvider.CreateOrUpdate<MailboxDiscoverySearch>(mailboxDiscoverySearch);
			Exception ex = null;
			try
			{
				if (searchObject.SearchStatus != null)
				{
					if (!string.IsNullOrEmpty(searchObject.SearchStatus.ResultsPath) && aduser != null)
					{
						string serverFqdn = ExchangePrincipal.FromADUser(e14DataProvider.RecipientSession.SessionSettings, aduser, RemotingOptions.AllowCrossSite).MailboxInfo.Location.ServerFqdn;
						if (!string.IsNullOrEmpty(serverFqdn))
						{
							goto IL_395;
						}
						Utils.<>c__DisplayClassc CS$<>8__locals1 = new Utils.<>c__DisplayClassc();
						CS$<>8__locals1.searchId = new SearchId(e14DataProvider.ADUser.Id.DistinguishedName, e14DataProvider.ADUser.Id.ObjectGuid, searchObject.Id.Guid.ToString());
						using (MailboxSearchClient client = new MailboxSearchClient(serverFqdn))
						{
							Utils.RpcCallWithRetry(delegate()
							{
								client.Remove(CS$<>8__locals1.searchId, true);
							});
							goto IL_395;
						}
					}
					e14DataProvider.Delete(searchObject.SearchStatus);
				}
				IL_395:
				e14DataProvider.Delete(searchObject);
			}
			catch (RpcConnectionException ex2)
			{
				ex = ex2;
			}
			catch (RpcException ex3)
			{
				ex = ex3;
			}
			catch (StoragePermanentException ex4)
			{
				ex = ex4;
			}
			catch (StorageTransientException ex5)
			{
				ex = ex5;
			}
			if (ex != null)
			{
				writeWarningDelegate(Strings.FailedToDeleteE14SearchObjectWhenUpgrade(searchObject.Name, ex.Message));
			}
			return e15DataProvider.FindByAlternativeId<MailboxDiscoverySearch>(searchObject.Name);
		}

		internal static string ConvertAqsToKql(string aqsString, CultureInfo culture)
		{
			if (string.IsNullOrEmpty(aqsString))
			{
				return aqsString;
			}
			Exception ex = null;
			QueryFilter queryFilter = null;
			try
			{
				queryFilter = AqsParser.ParseAndBuildQuery(aqsString, AqsParser.ParseOption.UseCiKeywordOnly | AqsParser.ParseOption.DisablePrefixMatch | AqsParser.ParseOption.QueryConverting | AqsParser.ParseOption.AllowShortWildcards, culture, RescopedAll.Default, null, null);
			}
			catch (ParserException ex2)
			{
				ex = ex2;
			}
			if (queryFilter == null)
			{
				throw new MailboxSearchTaskException(Strings.CannotConvertAqsToKql(ex.Message ?? string.Empty));
			}
			string text = null;
			ex = null;
			try
			{
				text = queryFilter.GenerateInfixString(FilterLanguage.Kql);
				QueryFilter aqsFilter = AqsParser.ParseAndBuildQuery(aqsString, AqsParser.ParseOption.UseCiKeywordOnly | AqsParser.ParseOption.DisablePrefixMatch | AqsParser.ParseOption.AllowShortWildcards, culture, RescopedAll.Default, null, null);
				QueryFilter kqlFilter = KqlParser.ParseAndBuildQuery(text, KqlParser.ParseOption.ImplicitOr | KqlParser.ParseOption.UseCiKeywordOnly | KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.AllowShortWildcards, culture, RescopedAll.Default, null, null);
				if (!Utils.VerifyQueryFilters(aqsFilter, kqlFilter))
				{
					throw new MailboxSearchTaskException(Strings.CannotConvertAqsToKql(string.Empty));
				}
			}
			catch (ParserException ex3)
			{
				ex = ex3;
			}
			catch (ArgumentOutOfRangeException ex4)
			{
				ex = ex4;
			}
			if (ex != null)
			{
				throw new MailboxSearchTaskException(Strings.CannotConvertAqsToKql(ex.Message));
			}
			return text;
		}

		private static bool VerifyQueryFilters(QueryFilter aqsFilter, QueryFilter kqlFilter)
		{
			if (aqsFilter == kqlFilter)
			{
				return true;
			}
			if (kqlFilter is CompositeFilter)
			{
				if (aqsFilter is CompositeFilter)
				{
					CompositeFilter compositeFilter = (CompositeFilter)aqsFilter;
					CompositeFilter compositeFilter2 = (CompositeFilter)kqlFilter;
					if (compositeFilter.FilterCount == compositeFilter2.FilterCount)
					{
						for (int i = 0; i < compositeFilter.FilterCount; i++)
						{
							if (!Utils.VerifyQueryFilters(compositeFilter.Filters[i], compositeFilter2.Filters[i]))
							{
								return false;
							}
						}
						return true;
					}
				}
			}
			else if (kqlFilter is ComparisonFilter)
			{
				if (kqlFilter.Equals(Utils.KqlImportanceHigh))
				{
					return aqsFilter.Equals(Utils.AqsImportanceHigh);
				}
				if (kqlFilter.Equals(Utils.KqlImportanceNormal))
				{
					return aqsFilter.Equals(Utils.AqsImportanceNormal);
				}
				if (kqlFilter.Equals(Utils.KqlImportanceLow))
				{
					return aqsFilter.Equals(Utils.AqsImportanceLow);
				}
				if (aqsFilter is ComparisonFilter)
				{
					ComparisonFilter comparisonFilter = aqsFilter as ComparisonFilter;
					ComparisonFilter comparisonFilter2 = kqlFilter as ComparisonFilter;
					if (comparisonFilter.Property.Name == comparisonFilter2.Property.Name && comparisonFilter.ComparisonOperator == comparisonFilter2.ComparisonOperator)
					{
						if (comparisonFilter.PropertyValue == null)
						{
							return comparisonFilter2.PropertyValue == null;
						}
						if (comparisonFilter.PropertyValue is ExDateTime && comparisonFilter2.PropertyValue is ExDateTime)
						{
							return comparisonFilter2.PropertyValue.ToString().Equals(new ExDateTime(ExTimeZone.CurrentTimeZone, ((ExDateTime)comparisonFilter.PropertyValue).UniversalTime).ToString());
						}
					}
				}
			}
			return aqsFilter.Equals(kqlFilter);
		}

		internal static void RpcCallWithRetry(Utils.RpcCallDelegate rpcDelegate)
		{
			Utils.RpcCallWithRetry(rpcDelegate, 3);
		}

		internal static void RpcCallWithRetry(Utils.RpcCallDelegate rpcDelegate, int maxRetry)
		{
			if (maxRetry <= 0)
			{
				throw new ArgumentException("maxRetry should be greater than 0");
			}
			for (int i = 0; i < maxRetry - 1; i++)
			{
				try
				{
					rpcDelegate();
					return;
				}
				catch (RpcConnectionException)
				{
				}
			}
			rpcDelegate();
		}

		internal static bool SameNameExists(string name, DiscoverySearchDataProvider e15DataProvider, MailboxDataProvider e14DataProvider)
		{
			return e15DataProvider.Find<MailboxDiscoverySearch>(name) != null || Utils.GetE14SearchObjectByName(name, e14DataProvider) != null;
		}

		internal static SearchObject GetE14SearchObjectByName(string name, MailboxDataProvider e14DataProvider)
		{
			QueryFilter filter = new TextFilter(SearchObjectBaseSchema.Name, name, MatchOptions.FullString, MatchFlags.IgnoreCase);
			IConfigurable[] array = e14DataProvider.Find<SearchObject>(filter, null, false, null);
			if (array == null || array.Length <= 0)
			{
				return null;
			}
			return (SearchObject)array[0];
		}

		private const int MaxRpcRetryCount = 3;

		internal static int MaxNumberOfMailboxesInSingleHold = 10000;

		private static QueryFilter KqlImportanceHigh = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Importance, 2);

		private static QueryFilter KqlImportanceNormal = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Importance, 1);

		private static QueryFilter KqlImportanceLow = new ComparisonFilter(ComparisonOperator.Equal, ItemSchema.Importance, 0);

		private static QueryFilter AqsImportanceHigh = new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.Importance, 2);

		private static QueryFilter AqsImportanceNormal = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.Importance, 1),
			new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.Importance, 2)
		});

		private static QueryFilter AqsImportanceLow = new AndFilter(new QueryFilter[]
		{
			new ComparisonFilter(ComparisonOperator.GreaterThanOrEqual, ItemSchema.Importance, 0),
			new ComparisonFilter(ComparisonOperator.LessThan, ItemSchema.Importance, 1)
		});

		internal delegate TRawId IdentityToRawIdDelegate<TIdentity, TRawId>(TIdentity identity, object param);

		internal delegate void RpcCallDelegate();
	}
}
