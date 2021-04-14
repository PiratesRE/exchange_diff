using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Net;
using System.Security;
using Microsoft.Exchange.Collections;
using Microsoft.Exchange.Configuration.Authorization;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Search.KqlParser;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch;
using Microsoft.Exchange.EDiscovery.Export;
using Microsoft.Exchange.InfoWorker.Common;
using Microsoft.Exchange.InfoWorker.Common.MultiMailboxSearch;
using Microsoft.Exchange.InfoWorker.Common.Search;
using Microsoft.Exchange.InfoWorker.Common.SearchService;
using Microsoft.Exchange.Rpc.MailboxSearch;
using Microsoft.Exchange.Security.OAuth;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.EDiscovery.MailboxSearch
{
	internal class MailboxSearchConfigurationProvider : IMailboxSearchConfigurationProvider
	{
		public MailboxSearchConfigurationProvider(ADObjectId discoverySystemMailboxId, string searchName) : this(discoverySystemMailboxId, searchName, null, null)
		{
		}

		public MailboxSearchConfigurationProvider(ADObjectId discoverySystemMailboxId, string searchName, IDiscoverySearchDataProvider searchDataProvider, MailboxDiscoverySearch searchObject)
		{
			Util.ThrowIfNullOrEmpty(searchName, "searchName");
			this.discoverySystemMailboxId = discoverySystemMailboxId;
			if (discoverySystemMailboxId != null)
			{
				OrganizationId organizationId = this.ResolveOrganization(discoverySystemMailboxId);
				ADSessionSettings adsessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(organizationId);
				if (organizationId.Equals(OrganizationId.ForestWideOrgId))
				{
					adsessionSettings = ADSessionSettings.RescopeToSubtree(adsessionSettings);
				}
				if (VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled)
				{
					adsessionSettings.IncludeInactiveMailbox = true;
				}
				this.recipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, adsessionSettings, 146, ".ctor", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\MailboxSearch\\Common\\MailboxSearchConfigurationProvider.cs");
			}
			this.SearchDataProvider = searchDataProvider;
			if (this.SearchDataProvider == null)
			{
				this.SearchDataProvider = new DiscoverySearchDataProvider(this.DiscoverySystemMailboxUser.OrganizationId);
			}
			this.SearchObject = searchObject;
			if (this.SearchObject == null)
			{
				this.SearchObject = this.SearchDataProvider.Find<MailboxDiscoverySearch>(searchName);
				if (this.SearchObject == null)
				{
					Util.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to find SearchObject {0}", searchName);
					throw new SearchObjectNotFoundException(Strings.UnableToFindSearchObject(searchName));
				}
			}
		}

		public IDiscoverySearchDataProvider SearchDataProvider { get; private set; }

		public MailboxDiscoverySearch SearchObject { get; private set; }

		public IRecipientSession RecipientSession
		{
			get
			{
				return this.recipientSession;
			}
		}

		public uint MaxMailboxesToSearch
		{
			get
			{
				uint result;
				try
				{
					this.CheckRecipientSessionIsNotNull();
					result = SearchUtils.GetDiscoveryMaxMailboxes(this.recipientSession);
				}
				catch (SecurityException)
				{
					Util.Tracer.TraceError<string, uint>((long)this.GetHashCode(), "Security exception encountered while retrieving {0} registry value.  Defaulting to {1}", MailboxSearchConfigurationProvider.MaxNumberOfMailboxes, MailboxSearchConfigurationProvider.MaxNumberOfMailboxesDefault);
					result = MailboxSearchConfigurationProvider.MaxNumberOfMailboxesDefault;
				}
				catch (UnauthorizedAccessException)
				{
					Util.Tracer.TraceError<string, uint>((long)this.GetHashCode(), "Unauthorized access exception encountered while retrieving {0} registry value.  Defaulting to {1}", MailboxSearchConfigurationProvider.MaxNumberOfMailboxes, MailboxSearchConfigurationProvider.MaxNumberOfMailboxesDefault);
					result = MailboxSearchConfigurationProvider.MaxNumberOfMailboxesDefault;
				}
				return result;
			}
		}

		public uint MaxNumberOfMailboxesForKeywordStatistics
		{
			get
			{
				uint result;
				try
				{
					this.CheckRecipientSessionIsNotNull();
					result = SearchUtils.GetDiscoveryMaxMailboxesForStatsSearch(this.recipientSession);
				}
				catch (SecurityException)
				{
					Util.Tracer.TraceError<string, uint>((long)this.GetHashCode(), "Security exception encountered while retrieving {0} registry value.  Defaulting to {1}", MailboxSearchConfigurationProvider.MaxNumberOfMailboxes, MailboxSearchConfigurationProvider.MaxNumberOfMailboxesForKeywordStatisticsDefault);
					result = MailboxSearchConfigurationProvider.MaxNumberOfMailboxesForKeywordStatisticsDefault;
				}
				catch (UnauthorizedAccessException)
				{
					Util.Tracer.TraceError<string, uint>((long)this.GetHashCode(), "Unauthorized access exception encountered while retrieving {0} registry value.  Defaulting to {1}", MailboxSearchConfigurationProvider.MaxNumberOfMailboxes, MailboxSearchConfigurationProvider.MaxNumberOfMailboxesForKeywordStatisticsDefault);
					result = MailboxSearchConfigurationProvider.MaxNumberOfMailboxesForKeywordStatisticsDefault;
				}
				return result;
			}
		}

		public uint MaxMailboxSearches
		{
			get
			{
				this.CheckRecipientSessionIsNotNull();
				return SearchUtils.GetDiscoveryMaxConcurrency(this.recipientSession);
			}
		}

		public uint MaxQueryKeywords
		{
			get
			{
				this.CheckRecipientSessionIsNotNull();
				return SearchUtils.GetDiscoveryMaxKeywords(this.recipientSession);
			}
		}

		public string SearchName
		{
			get
			{
				return this.SearchObject.Name;
			}
		}

		public string ExecutingUserId
		{
			get
			{
				return this.executingUserId;
			}
			set
			{
				this.executingUserId = value;
				if (!string.IsNullOrEmpty(this.executingUserId))
				{
					this.executingUserRunspace = this.CreateRunspaceConfigurationFromExecutingUser();
				}
			}
		}

		public string ExecutingUserPrimarySmtpAddress
		{
			get
			{
				if (this.ExecutingUserRunspace.ExecutingUserPrimarySmtpAddress == SmtpAddress.Empty)
				{
					throw new LocalizedException(Strings.ExecutingUserNeedSmtpAddress);
				}
				return this.ExecutingUserRunspace.ExecutingUserPrimarySmtpAddress.ToString();
			}
		}

		public bool UserCanRunMailboxSearch
		{
			get
			{
				return this.ExecutingUserRunspace != null && (this.SearchObject.Target == null || this.SearchObject.StatisticsOnly || this.executingUserRunspace.IsCmdletAllowedInScope("New-MailboxSearch", new string[]
				{
					"TargetMailbox"
				}, this.TargetMailboxUser, ScopeLocation.RecipientWrite));
			}
		}

		public ADUser DiscoverySystemMailboxUser
		{
			get
			{
				if (this.discoverySystemMailboxUser == null)
				{
					if (this.discoverySystemMailboxId == null)
					{
						throw new ArgumentNullException("discoverySystemMailboxId", "The discovery system mailbox id should not be null if calling RecipientSession property.");
					}
					this.CheckRecipientSessionIsNotNull();
					this.discoverySystemMailboxUser = (ADUser)this.recipientSession.Read(this.discoverySystemMailboxId);
					if (this.discoverySystemMailboxUser == null)
					{
						Util.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to find discovery system mailbox: {0}", this.discoverySystemMailboxId.Name);
						throw new ObjectNotFoundException(ServerStrings.ADUserNotFoundId(this.discoverySystemMailboxId.Name));
					}
				}
				return this.discoverySystemMailboxUser;
			}
		}

		public ADUser ExecutingUser
		{
			get
			{
				if (this.executingUser == null && this.ExecutingUserId != null && this.ExecutingUserRunspace != null && this.ExecutingUserRunspace.ExecutingUser != null)
				{
					this.executingUser = new ADUser(this.RecipientSession, this.ExecutingUserRunspace.ExecutingUser.propertyBag);
				}
				return this.executingUser;
			}
		}

		private ExchangeRunspaceConfiguration ExecutingUserRunspace
		{
			get
			{
				if (this.executingUserRunspace == null)
				{
					if (string.IsNullOrEmpty(this.ExecutingUserId))
					{
						throw new ArgumentNullException("ExecutingUserId", "The ExecutingUserId property need to be assigned value.");
					}
					this.executingUserRunspace = this.CreateRunspaceConfigurationFromExecutingUser();
				}
				return this.executingUserRunspace;
			}
		}

		private ADUser TargetMailboxUser
		{
			get
			{
				if (this.targetMailboxUser == null)
				{
					this.CheckRecipientSessionIsNotNull();
					this.targetMailboxUser = (this.recipientSession.FindByLegacyExchangeDN(this.SearchObject.Target) as ADUser);
					if (this.targetMailboxUser == null)
					{
						Util.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to find target mailbox: {0}", this.SearchObject.Target);
						throw new ObjectNotFoundException(ServerStrings.ADUserNotFoundId(this.SearchObject.Target));
					}
				}
				return this.targetMailboxUser;
			}
		}

		public void UpdateSearchObject(string callerMember, int callerLine)
		{
			Util.UpdateSearchObject(this.SearchDataProvider, this.SearchObject);
			SearchEventLogger.Instance.LogDiscoverySearchStatusChangedEvent(this.SearchObject, callerMember + "@" + callerLine, "UpdateSearchObject");
		}

		public void ResetSearchObject(string callerMember, int callerLine)
		{
			if (this.SearchObject.Errors != null)
			{
				this.SearchObject.Errors.Clear();
			}
			else
			{
				this.SearchObject.Errors = new MultiValuedProperty<string>();
			}
			if (this.SearchObject.CompletedMailboxes == null)
			{
				this.SearchObject.CompletedMailboxes = new MultiValuedProperty<string>();
			}
			if (this.SearchObject.Information != null)
			{
				this.SearchObject.Information.Clear();
			}
			else
			{
				this.SearchObject.Information = new MultiValuedProperty<string>();
			}
			if (this.SearchObject.Resume)
			{
				Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Previous completed search: {0} mailboxes", this.SearchObject.CompletedMailboxes.Count.ToString());
				if (this.SearchObject.CompletedMailboxes.Count > 0)
				{
					Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Previous completed mailboxes: '{0}'", string.Join(",", this.SearchObject.CompletedMailboxes.ToArray()));
				}
			}
			else
			{
				this.SearchObject.PercentComplete = 0;
				this.SearchObject.ResultItemCountCopied = 0L;
				this.SearchObject.ResultItemCountEstimate = 0L;
				this.SearchObject.ResultSizeCopied = 0L;
				this.SearchObject.ResultSizeEstimate = 0L;
				this.SearchObject.ResultsPath = null;
				this.SearchObject.ResultsLink = null;
				this.SearchObject.PreviewResultsLink = null;
				this.SearchObject.CompletedMailboxes.Clear();
				this.SearchObject.SearchStatistics = null;
			}
			SearchEventLogger.Instance.LogDiscoverySearchStatusChangedEvent(this.SearchObject, callerMember + "@" + callerLine, "ResetSearchObject");
		}

		public string GenerateOWASearchResultsLink()
		{
			ExchangePrincipal targetPrincipal = ExchangePrincipal.FromADUser(this.TargetMailboxUser.OrganizationId.ToADSessionSettings(), this.TargetMailboxUser);
			Uri owaBaseLink = this.GetOwaBaseLink(targetPrincipal, false);
			if (owaBaseLink != null)
			{
				return LinkUtils.UpdateOwaLinkWithMailbox(owaBaseLink, this.TargetMailboxUser.PrimarySmtpAddress).ToString();
			}
			return null;
		}

		public string GenerateOWAPreviewResultsLink()
		{
			ExchangePrincipal targetPrincipal = ExchangePrincipal.FromADUser(this.DiscoverySystemMailboxUser.OrganizationId.ToADSessionSettings(), this.DiscoverySystemMailboxUser);
			Uri owaBaseLink = this.GetOwaBaseLink(targetPrincipal, false);
			if (owaBaseLink != null)
			{
				return LinkUtils.UpdateOwaLinkToSearchId(owaBaseLink, this.SearchName).ToString();
			}
			return null;
		}

		public string GetExecutingUserName()
		{
			if (this.ExecutingUserRunspace != null)
			{
				string text = (!string.IsNullOrEmpty(this.ExecutingUserRunspace.ExecutingUserDisplayName)) ? this.ExecutingUserRunspace.ExecutingUserDisplayName : this.ExecutingUserRunspace.IdentityName;
				return text ?? string.Empty;
			}
			return null;
		}

		public void CheckDiscoveryBudget(bool isEstimateOnly, MailboxSearchServer server)
		{
			if (server == null)
			{
				throw new ArgumentNullException("server");
			}
			if (this.MaxMailboxSearches == 0U)
			{
				throw new SearchDisabledException();
			}
			IEnumerable<MailboxDiscoverySearch> all = this.SearchDataProvider.GetAll<MailboxDiscoverySearch>();
			if (all != null)
			{
				uint num = 0U;
				uint num2 = 0U;
				foreach (MailboxDiscoverySearch mailboxDiscoverySearch in from so in all
				where so.Status == SearchState.InProgress || so.Status == SearchState.EstimateInProgress
				select so)
				{
					SearchStatus searchStatus = null;
					SearchId searchId = new SearchId(this.SearchDataProvider.DistinguishedName, this.SearchDataProvider.ObjectGuid, mailboxDiscoverySearch.Name);
					server.GetStatus(searchId, out searchStatus);
					if (searchStatus != null)
					{
						if (searchStatus.Status == 0)
						{
							num += 1U;
						}
						else if (searchStatus.Status == 6)
						{
							num2 += 1U;
						}
						uint num3 = isEstimateOnly ? num2 : num;
						if (num3 >= this.MaxMailboxSearches)
						{
							throw new SearchOverBudgetException((int)this.MaxMailboxSearches);
						}
					}
				}
			}
		}

		public bool IsKeywordStatsAllowed()
		{
			return Factory.Current.IsSearchAllowed(this.recipientSession, SearchType.Statistics, this.SearchObject.NumberOfMailboxes);
		}

		public bool IsPreviewAllowed()
		{
			return Factory.Current.IsSearchAllowed(this.recipientSession, SearchType.Preview, this.SearchObject.NumberOfMailboxes);
		}

		public bool ValidateKeywordsLimit()
		{
			if (!string.IsNullOrEmpty(this.SearchObject.CalculatedQuery) && this.SearchObject.CalculatedQuery != MailboxDiscoverySearch.EmptyQueryReplacement)
			{
				QueryFilter queryFilter = KqlParser.ParseAndBuildQuery(this.SearchObject.CalculatedQuery, KqlParser.ParseOption.ImplicitOr | KqlParser.ParseOption.UseCiKeywordOnly | KqlParser.ParseOption.DisablePrefixMatch | KqlParser.ParseOption.AllowShortWildcards | KqlParser.ParseOption.EDiscoveryMode, new CultureInfo(this.SearchObject.Language), RescopedAll.Default, null, null);
				if (queryFilter != null && queryFilter.Keywords() != null && (long)queryFilter.Keywords().Count<string>() > (long)((ulong)this.MaxQueryKeywords))
				{
					return false;
				}
			}
			return true;
		}

		public IList<ISource> ValidateAndGetFinalSourceMailboxes(string searchQuery, IList<string> sourceMailboxes, IList<string> notFoundMailboxes, IList<string> versionSkippedMailboxes, IList<string> rbacDeniedMailboxes, IList<string> crossPremiseFailedMailboxes, IDictionary<Uri, string> crossPremiseUrls)
		{
			Util.ThrowIfNullOrEmpty(searchQuery, "Search query cannot be empty.");
			Util.ThrowIfNull(sourceMailboxes, "sourceMailboxes");
			Util.ThrowIfNull(notFoundMailboxes, "notFoundMailboxes");
			Util.ThrowIfNull(versionSkippedMailboxes, "versionSkippedMailboxes");
			Util.ThrowIfNull(rbacDeniedMailboxes, "rbacDeniedMailboxes");
			Util.ThrowIfNull(crossPremiseFailedMailboxes, "crossPremiseFailedMailboxes");
			Util.ThrowIfNull(crossPremiseUrls, "crossPremiseUrls");
			List<ISource> list = new List<ISource>();
			Dictionary<MiniRecipient, MailboxInfo> dictionary = new Dictionary<MiniRecipient, MailboxInfo>();
			int num = 0;
			if (sourceMailboxes.Count == 0)
			{
				AndFilter filter = new AndFilter(new QueryFilter[]
				{
					new OrFilter(new QueryFilter[]
					{
						Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.UserMailbox),
						Util.CreateRecipientTypeDetailsValueFilter((RecipientTypeDetails)((ulong)int.MinValue)),
						Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteRoomMailbox),
						Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteEquipmentMailbox),
						Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteSharedMailbox),
						Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.RemoteTeamMailbox)
					}),
					new NotFilter(Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.ArbitrationMailbox)),
					new NotFilter(Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MonitoringMailbox)),
					new NotFilter(Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.AuditLogMailbox)),
					new NotFilter(Util.CreateRecipientTypeDetailsValueFilter(RecipientTypeDetails.MailboxPlan))
				});
				this.CheckRecipientSessionIsNotNull();
				ADPagedReader<StorageMiniRecipient> adpagedReader = this.recipientSession.FindPagedMiniRecipient<StorageMiniRecipient>(null, QueryScope.SubTree, filter, null, 1000, SearchMailboxCriteria.RecipientTypeDetailsProperty);
				using (IEnumerator<StorageMiniRecipient> enumerator = adpagedReader.GetEnumerator())
				{
					while (enumerator.MoveNext())
					{
						StorageMiniRecipient storageMiniRecipient = enumerator.Current;
						if (!storageMiniRecipient.LegacyExchangeDN.Equals(this.SearchObject.Target, StringComparison.OrdinalIgnoreCase))
						{
							if ((long)(++num) > (long)((ulong)this.MaxMailboxesToSearch))
							{
								throw new SearchTooManyMailboxesException((int)this.MaxMailboxesToSearch);
							}
							this.ValidateAndAddToFinalSourceMailboxes(searchQuery, storageMiniRecipient, list, versionSkippedMailboxes, rbacDeniedMailboxes, dictionary);
						}
					}
					goto IL_2B6;
				}
			}
			foreach (ADObjectId adobjectId in this.ExpandGroupMembers(sourceMailboxes, this.DiscoverySystemMailboxUser.OrganizationId, notFoundMailboxes))
			{
				if ((this.SearchObject.Target == null || !adobjectId.Equals(this.TargetMailboxUser.Id)) && !adobjectId.Equals(this.DiscoverySystemMailboxUser.Id))
				{
					this.CheckRecipientSessionIsNotNull();
					StorageMiniRecipient storageMiniRecipient2 = this.recipientSession.ReadMiniRecipient<StorageMiniRecipient>(adobjectId, SearchMailboxCriteria.RecipientTypeDetailsProperty);
					if (storageMiniRecipient2 != null)
					{
						if ((long)(++num) > (long)((ulong)this.MaxMailboxesToSearch))
						{
							throw new SearchTooManyMailboxesException((int)this.MaxMailboxesToSearch);
						}
						this.ValidateAndAddToFinalSourceMailboxes(searchQuery, storageMiniRecipient2, list, versionSkippedMailboxes, rbacDeniedMailboxes, dictionary);
					}
					else
					{
						Util.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to find mailbox: {0}", adobjectId.Name);
					}
				}
			}
			IL_2B6:
			if (dictionary.Count > 0)
			{
				string domain = dictionary.First<KeyValuePair<MiniRecipient, MailboxInfo>>().Value.GetDomain();
				Util.Tracer.TraceDebug<string>((long)this.GetHashCode(), "Before autodiscover for cross premise domain: {0}", domain);
				Uri uri = null;
				EndPointDiscoveryInfo endPointDiscoveryInfo;
				bool flag = RemoteDiscoveryEndPoint.TryGetDiscoveryEndPoint(this.SearchDataProvider.OrganizationId, domain, null, null, null, out uri, out endPointDiscoveryInfo);
				if (endPointDiscoveryInfo != null)
				{
					ScenarioData.Current["IOC"] = ((int)endPointDiscoveryInfo.Status).ToString();
					if (endPointDiscoveryInfo.Status != EndPointDiscoveryInfo.DiscoveryStatus.Success)
					{
						SearchEventLogger.Instance.LogSearchErrorEvent(ScenarioData.Current["SID"], endPointDiscoveryInfo.Message);
					}
				}
				if (flag)
				{
					Util.Tracer.TraceDebug<Uri>((long)this.GetHashCode(), "Auto discover Url : {1}", uri);
					ICredentials oauthCredentialsForAppToken = OAuthCredentials.GetOAuthCredentialsForAppToken(this.SearchDataProvider.OrganizationId, domain);
					List<MailboxInfo> list2 = new List<MailboxInfo>(dictionary.Count);
					foreach (KeyValuePair<MiniRecipient, MailboxInfo> keyValuePair in dictionary)
					{
						list2.Add(keyValuePair.Value);
					}
					IAutodiscoveryClient autodiscoveryClient = Factory.Current.CreateUserSettingAutoDiscoveryClient(list2, EwsWsSecurityUrl.FixForAnonymous(uri), oauthCredentialsForAppToken, new CallerInfo(true, null, null, string.Empty, null, null, null));
					IAsyncResult result = autodiscoveryClient.BeginAutodiscover(null, null);
					Dictionary<GroupId, List<MailboxInfo>> dictionary2 = autodiscoveryClient.EndAutodiscover(result);
					if (dictionary2 != null)
					{
						using (Dictionary<GroupId, List<MailboxInfo>>.Enumerator enumerator4 = dictionary2.GetEnumerator())
						{
							while (enumerator4.MoveNext())
							{
								KeyValuePair<GroupId, List<MailboxInfo>> keyValuePair2 = enumerator4.Current;
								if (keyValuePair2.Key.GroupType == GroupType.CrossPremise)
								{
									if (!crossPremiseUrls.ContainsKey(keyValuePair2.Key.Uri))
									{
										crossPremiseUrls.Add(keyValuePair2.Key.Uri, domain);
									}
									using (List<ISource>.Enumerator enumerator5 = list.GetEnumerator())
									{
										while (enumerator5.MoveNext())
										{
											ISource source = enumerator5.Current;
											SourceMailbox sourceMailbox = (SourceMailbox)source;
											foreach (MailboxInfo mailboxInfo in keyValuePair2.Value)
											{
												if (string.Compare(sourceMailbox.LegacyExchangeDN, mailboxInfo.LegacyExchangeDN, StringComparison.OrdinalIgnoreCase) == 0)
												{
													sourceMailbox.ServiceEndpoint = keyValuePair2.Key.Uri;
													break;
												}
											}
										}
										continue;
									}
								}
								if (keyValuePair2.Key.GroupType == GroupType.SkippedError)
								{
									foreach (MailboxInfo mailboxInfo2 in keyValuePair2.Value)
									{
										Util.Tracer.TraceError<SmtpAddress>((long)this.GetHashCode(), "Auto discover skipped error for mailbox: {0}", mailboxInfo2.PrimarySmtpAddress);
										crossPremiseFailedMailboxes.Add(mailboxInfo2.LegacyExchangeDN);
									}
								}
							}
							goto IL_5F2;
						}
					}
					Util.Tracer.TraceError<string>((long)this.GetHashCode(), "Auto discover results is null for domain: {0}", domain);
				}
				else
				{
					Util.Tracer.TraceError<string>((long)this.GetHashCode(), "Organization relationship is not set up for domain: {0}", domain);
					foreach (KeyValuePair<MiniRecipient, MailboxInfo> keyValuePair3 in dictionary)
					{
						crossPremiseFailedMailboxes.Add(keyValuePair3.Value.LegacyExchangeDN);
					}
				}
			}
			IL_5F2:
			if (crossPremiseFailedMailboxes.Count > 0)
			{
				foreach (string b in crossPremiseFailedMailboxes)
				{
					for (int i = list.Count - 1; i >= 0; i--)
					{
						ISource source2 = list[i];
						if (source2.LegacyExchangeDN == b)
						{
							list.RemoveAt(i);
						}
					}
				}
			}
			return list;
		}

		public IList<ISource> GetFinalSources(string searchObjectName, string searchQuery, string executingUserPrimarySmtpAddress, Uri discoveryUserUri)
		{
			List<ISource> list = new List<ISource>();
			DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, this.recipientSession.SessionSettings, 930, "GetFinalSources", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\MailboxSearch\\Common\\MailboxSearchConfigurationProvider.cs");
			if (discoveryUserUri != null)
			{
				EwsClient ewsClient = new EwsClient(discoveryUserUri, new ServerToServerEwsCallingContext(null));
				string empty = string.Empty;
				List<SourceInformation.SourceConfiguration> list2 = ewsClient.FunctionalInterface.RetrieveSearchConfiguration(searchObjectName, out empty, executingUserPrimarySmtpAddress);
				int num = 0;
				foreach (SourceInformation.SourceConfiguration sourceConfiguration in list2)
				{
					string name = string.Empty;
					string id = string.Empty;
					bool flag = sourceConfiguration.Name.StartsWith("\\");
					if (flag)
					{
						name = sourceConfiguration.Id;
						id = sourceConfiguration.Name;
					}
					else
					{
						name = sourceConfiguration.Name;
						id = sourceConfiguration.Id;
					}
					string legacyDN = flag ? executingUserPrimarySmtpAddress : sourceConfiguration.LegacyExchangeDN;
					SourceMailbox item = new SourceMailbox(id, name, legacyDN, discoveryUserUri, searchQuery);
					list.Add(item);
					if ((long)(++num) > (long)((ulong)this.MaxMailboxesToSearch))
					{
						throw new SearchTooManyMailboxesException((int)this.MaxMailboxesToSearch);
					}
				}
				return list;
			}
			return new List<ISource>();
		}

		private ExchangeRunspaceConfiguration CreateRunspaceConfigurationFromExecutingUser()
		{
			this.CheckRecipientSessionIsNotNull();
			ExchangeRunspaceConfiguration exchangeRunspaceConfiguration = Util.ReadExchangeRunspaceConfiguration(this.recipientSession, this.ExecutingUserId);
			if (exchangeRunspaceConfiguration == null)
			{
				Util.Tracer.TraceError<string>((long)this.GetHashCode(), "Unable to find executing user: {0}", this.ExecutingUserId);
				throw new ObjectNotFoundException(ServerStrings.ADUserNotFoundId(this.ExecutingUserId));
			}
			return exchangeRunspaceConfiguration;
		}

		private IEnumerable<ADObjectId> ExpandGroupMembers(IEnumerable<string> sourceLegacyDNs, OrganizationId organizationId, IList<string> notFoundMailboxes)
		{
			ADRecipientExpansion adrecipientExpansion = new ADRecipientExpansion(organizationId);
			HashSet<ADObjectId> expandedMemberIds = new HashSet<ADObjectId>();
			foreach (string text in sourceLegacyDNs)
			{
				this.CheckRecipientSessionIsNotNull();
				ADRecipient adrecipient = this.recipientSession.FindByLegacyExchangeDN(text);
				if (adrecipient == null)
				{
					notFoundMailboxes.Add(text);
				}
				else
				{
					adrecipientExpansion.Expand(adrecipient, delegate(ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentType)
					{
						if (recipientExpansionType == ExpansionType.GroupMembership)
						{
							return ExpansionControl.Continue;
						}
						if (((RecipientType)recipient[ADRecipientSchema.RecipientType] == RecipientType.UserMailbox || (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails] == (RecipientTypeDetails)((ulong)-2147483648) || (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails] == RecipientTypeDetails.RemoteRoomMailbox || (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails] == RecipientTypeDetails.RemoteEquipmentMailbox || (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails] == RecipientTypeDetails.RemoteTeamMailbox || (RecipientTypeDetails)recipient[ADRecipientSchema.RecipientTypeDetails] == RecipientTypeDetails.RemoteSharedMailbox) && !expandedMemberIds.Contains(recipient.Id))
						{
							expandedMemberIds.Add(recipient.Id);
						}
						return ExpansionControl.Skip;
					}, delegate(ExpansionFailure expansionFailure, ADRawEntry recipient, ExpansionType recipientExpansionType, ADRawEntry parent, ExpansionType parentExpansionType)
					{
						Util.Tracer.TraceDebug<ADObjectId, ExpansionType>((long)this.GetHashCode(), "Expand group member failed for {0}, reason {1}", recipient.Id, recipientExpansionType);
						return ExpansionControl.Skip;
					});
				}
			}
			return expandedMemberIds;
		}

		private void ValidateAndAddToFinalSourceMailboxes(string searchQuery, StorageMiniRecipient recipient, IList<ISource> finalSourceMailboxes, IList<string> versionSkippedMailboxes, IList<string> rbacDeniedMailboxes, Dictionary<MiniRecipient, MailboxInfo> crossPremiseMailboxes)
		{
			if (!this.ExecutingUserRunspace.IsCmdletAllowedInScope("New-MailboxSearch", new string[]
			{
				"EstimateOnly"
			}, recipient, ScopeLocation.RecipientWrite))
			{
				rbacDeniedMailboxes.Add(recipient.LegacyExchangeDN);
				return;
			}
			if (!this.ValidExchangeVersion(recipient))
			{
				versionSkippedMailboxes.Add(recipient.LegacyExchangeDN);
				return;
			}
			MailboxInfo value = new MailboxInfo(recipient, MailboxType.Primary);
			SourceMailbox item;
			if ((recipient.RecipientTypeDetails & (RecipientTypeDetails)((ulong)-2147483648)) == (RecipientTypeDetails)((ulong)-2147483648) || (recipient.RecipientTypeDetails & RecipientTypeDetails.RemoteRoomMailbox) == RecipientTypeDetails.RemoteRoomMailbox || (recipient.RecipientTypeDetails & RecipientTypeDetails.RemoteEquipmentMailbox) == RecipientTypeDetails.RemoteEquipmentMailbox || (recipient.RecipientTypeDetails & RecipientTypeDetails.RemoteTeamMailbox) == RecipientTypeDetails.RemoteTeamMailbox || (recipient.RecipientTypeDetails & RecipientTypeDetails.RemoteSharedMailbox) == RecipientTypeDetails.RemoteSharedMailbox)
			{
				crossPremiseMailboxes.Add(recipient, value);
				item = new SourceMailbox(recipient.ExternalEmailAddress.AddressString, recipient.DisplayName, recipient.LegacyExchangeDN, BackEndLocator.GetBackEndWebServicesUrl(recipient), searchQuery);
				finalSourceMailboxes.Add(item);
				return;
			}
			item = new SourceMailbox(recipient.PrimarySmtpAddress.ToString(), recipient.DisplayName, recipient.LegacyExchangeDN, BackEndLocator.GetBackEndWebServicesUrl(recipient), searchQuery);
			ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromMiniRecipient(recipient, RemotingOptions.AllowCrossSite);
			if (exchangePrincipal.MailboxInfo.Location.ServerVersion < Server.E15MinVersion || exchangePrincipal.MailboxInfo.Location == null || string.IsNullOrEmpty(exchangePrincipal.MailboxInfo.Location.ServerFqdn))
			{
				versionSkippedMailboxes.Add(recipient.LegacyExchangeDN);
				return;
			}
			finalSourceMailboxes.Add(item);
		}

		private bool ValidExchangeVersion(MiniRecipient recipient)
		{
			return !recipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2012) || (recipient.RecipientType == RecipientType.MailUser && !recipient.ExchangeVersion.IsOlderThan(ExchangeObjectVersion.Exchange2010));
		}

		private OrganizationId ResolveOrganization(ADObjectId entryId)
		{
			OrganizationId organizationId;
			try
			{
				if (entryId == null)
				{
					throw new ArgumentNullException("entryId");
				}
				IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromAllTenantsOrRootOrgAutoDetect(entryId), 1191, "ResolveOrganization", "f:\\15.00.1497\\sources\\dev\\EDiscovery\\src\\MailboxSearch\\Common\\MailboxSearchConfigurationProvider.cs");
				ADRecipient adrecipient = tenantOrRootOrgRecipientSession.Read(entryId);
				if (adrecipient == null)
				{
					throw new ObjectNotFoundException(ServerStrings.ADUserNotFoundId(entryId));
				}
				organizationId = adrecipient.OrganizationId;
			}
			catch (InvalidOperationException arg)
			{
				Util.Tracer.TraceError<InvalidOperationException>((long)this.GetHashCode(), "ResolveUserOrganization error: {0}", arg);
				throw;
			}
			catch (DataSourceOperationException arg2)
			{
				Util.Tracer.TraceError<DataSourceOperationException>((long)this.GetHashCode(), "ResolveUserOrganization error: {0}", arg2);
				throw;
			}
			catch (TransientException arg3)
			{
				Util.Tracer.TraceError<TransientException>((long)this.GetHashCode(), "ResolveUserOrganization error: {0}", arg3);
				throw;
			}
			catch (DataValidationException arg4)
			{
				Util.Tracer.TraceError<DataValidationException>((long)this.GetHashCode(), "ResolveUserOrganization error: {0}", arg4);
				throw;
			}
			return organizationId;
		}

		private void CheckRecipientSessionIsNotNull()
		{
			if (this.recipientSession == null)
			{
				throw new ArgumentNullException("recipientSession", "The recipient session should not be null if calling this method.");
			}
		}

		private Uri GetOwaBaseLink(ExchangePrincipal targetPrincipal, bool supportsIntegratedAuth)
		{
			return LinkUtils.GetOwaBaseLink(delegate()
			{
				string item = Strings.OWAServiceUrlFailure(this.TargetMailboxUser.PrimarySmtpAddress.ToString(), "Topology service cannot find the OWA service.");
				if (!this.SearchObject.Errors.Contains(item))
				{
					this.SearchObject.Errors.Add(item);
				}
			}, targetPrincipal, supportsIntegratedAuth);
		}

		private static readonly string MaxNumberOfMailboxes = "MaxNumberOfMailboxes";

		private static readonly uint MaxNumberOfMailboxesDefault = 5000U;

		private static readonly uint MaxNumberOfMailboxesForKeywordStatisticsDefault = 50U;

		private readonly ADObjectId discoverySystemMailboxId;

		private IRecipientSession recipientSession;

		private ADUser discoverySystemMailboxUser;

		private ADUser targetMailboxUser;

		private ADUser executingUser;

		private ExchangeRunspaceConfiguration executingUserRunspace;

		private string executingUserId;
	}
}
