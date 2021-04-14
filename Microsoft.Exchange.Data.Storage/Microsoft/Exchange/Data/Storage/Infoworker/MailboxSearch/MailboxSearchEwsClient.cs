using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Threading;
using System.Web.Services.Description;
using System.Web.Services.Protocols;
using System.Xml.Serialization;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Search.AqsParser;
using Microsoft.Exchange.Data.Storage.Authentication;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.WSTrust;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.AutoDiscover;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Data.Storage.Infoworker.MailboxSearch
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal sealed class MailboxSearchEwsClient : IDisposable
	{
		private static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (SslConfiguration.AllowExternalUntrustedCerts)
			{
				ExTraceGlobals.SessionTracer.TraceDebug<string, SslPolicyErrors>((long)sender.GetHashCode(), "MailboxSearchEwsClient::CertificateErrorHandler. Allowed SSL certificate {0} with error {1}", certificate.Subject, sslPolicyErrors);
				return true;
			}
			return false;
		}

		public MailboxSearchEwsClient(ExchangePrincipal principal, ADUser executingUser)
		{
			try
			{
				string url = null;
				DelegationTokenRequest request = null;
				this.Discover(principal, executingUser, out url, out request);
				RequestedToken token = this.securityTokenService.IssueToken(request);
				SoapHttpClientAuthenticator authenticator = SoapHttpClientAuthenticator.Create(token);
				this.binding = new MailboxSearchEwsClient.MailboxSearchEwsBinding("ExchangeEDiscovery", new RemoteCertificateValidationCallback(MailboxSearchEwsClient.CertificateErrorHandler));
				this.binding.Url = url;
				this.binding.RequestServerVersionValue = MailboxSearchEwsClient.RequestServerVersionExchange2010;
				this.binding.Authenticator = authenticator;
				this.binding.Proxy = this.WebProxy;
				this.binding.UserAgent = "ExchangeEDiscovery";
				this.binding.Timeout = 600000;
			}
			catch (WSTrustException ex)
			{
				ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), ex.ToString());
				throw new MailboxSearchEwsFailedException(ex.Message);
			}
		}

		private void Discover(ExchangePrincipal principal, ADUser executingUser, out string ewsEndpoint, out DelegationTokenRequest ewsTokenRequest)
		{
			SmtpAddress value = principal.MailboxInfo.RemoteIdentity.Value;
			ADSessionSettings sessionSettings = ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId);
			IRecipientSession tenantOrRootOrgRecipientSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(null, true, ConsistencyMode.FullyConsistent, null, sessionSettings, ConfigScopes.TenantSubTree, 168, "Discover", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Search\\MailboxSearch\\MailboxSearchEwsClient.cs");
			ADUser aduser = null;
			TransportConfigContainer transportConfigContainer = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(true, ConsistencyMode.IgnoreInvalid, sessionSettings, 171, "Discover", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Search\\MailboxSearch\\MailboxSearchEwsClient.cs").FindSingletonConfigurationObject<TransportConfigContainer>();
			if (transportConfigContainer != null && transportConfigContainer.OrganizationFederatedMailbox != SmtpAddress.NullReversePath)
			{
				SmtpAddress organizationFederatedMailbox = transportConfigContainer.OrganizationFederatedMailbox;
				ProxyAddress proxyAddress = null;
				try
				{
					proxyAddress = ProxyAddress.Parse(organizationFederatedMailbox.ToString());
				}
				catch (ArgumentException ex)
				{
					ExTraceGlobals.SessionTracer.TraceError<string>((long)this.GetHashCode(), "Proxy address of organization federated mailbox is invalid: {0}", ex.ToString());
				}
				if (proxyAddress != null && !(proxyAddress is InvalidProxyAddress))
				{
					aduser = (tenantOrRootOrgRecipientSession.FindByProxyAddress(proxyAddress) as ADUser);
				}
			}
			OrganizationIdCacheValue organizationIdCacheValue = OrganizationIdCache.Singleton.Get(OrganizationId.ForestWideOrgId);
			OrganizationRelationship organizationRelationship = organizationIdCacheValue.GetOrganizationRelationship(value.Domain);
			if (aduser == null || organizationRelationship == null)
			{
				throw new OrganizationNotFederatedException();
			}
			DelegationTokenRequest request = new DelegationTokenRequest
			{
				FederatedIdentity = aduser.GetFederatedIdentity(),
				EmailAddress = aduser.GetFederatedSmtpAddress().ToString(),
				Target = organizationRelationship.GetTokenTarget(),
				Offer = Offer.Autodiscover
			};
			FedOrgCredentials credentials = new FedOrgCredentials(request, this.GetSecurityTokenService(aduser.OrganizationId));
			Uri uri = null;
			using (AutoDiscoverUserSettingsClient autoDiscoverUserSettingsClient = AutoDiscoverUserSettingsClient.CreateInstance(DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, sessionSettings, 215, "Discover", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Search\\MailboxSearch\\MailboxSearchEwsClient.cs"), credentials, value, organizationRelationship.TargetAutodiscoverEpr, MailboxSearchEwsClient.AutoDiscoverRequestedSettings))
			{
				UserSettings userSettings = autoDiscoverUserSettingsClient.Discover();
				StringSetting stringSetting = userSettings.GetSetting("ExternalEwsUrl") as StringSetting;
				if (stringSetting == null || !Uri.TryCreate(stringSetting.Value, UriKind.Absolute, out uri))
				{
					throw new AutoDAccessException(ServerStrings.AutoDRequestFailed);
				}
			}
			ewsEndpoint = EwsWsSecurityUrl.Fix(uri.ToString());
			string text = null;
			if (executingUser.EmailAddresses != null && executingUser.EmailAddresses.Count > 0)
			{
				List<string> federatedEmailAddresses = executingUser.GetFederatedEmailAddresses();
				if (federatedEmailAddresses != null && federatedEmailAddresses.Count > 0)
				{
					text = federatedEmailAddresses[0];
				}
			}
			if (string.IsNullOrEmpty(text))
			{
				ewsTokenRequest = new DelegationTokenRequest
				{
					FederatedIdentity = aduser.GetFederatedIdentity(),
					EmailAddress = aduser.GetFederatedSmtpAddress().ToString(),
					Target = organizationRelationship.GetTokenTarget(),
					Offer = Offer.MailboxSearch
				};
				return;
			}
			ewsTokenRequest = new DelegationTokenRequest
			{
				FederatedIdentity = executingUser.GetFederatedIdentity(),
				EmailAddress = text.ToString(),
				Target = organizationRelationship.GetTokenTarget(),
				Offer = Offer.MailboxSearch
			};
		}

		public IList<KeywordHit> FindMailboxStatisticsByKeywords(ExchangePrincipal principal, SearchObject searchObject)
		{
			FindMailboxStatisticsByKeywordsType findMailboxStatisticsByKeywordsType = new FindMailboxStatisticsByKeywordsType
			{
				Mailboxes = new UserMailboxType[]
				{
					new UserMailboxType
					{
						Id = (principal.MailboxInfo.IsArchive ? principal.MailboxInfo.MailboxGuid.ToString() : principal.MailboxInfo.RemoteIdentity.Value.ToString()),
						IsArchive = principal.MailboxInfo.IsArchive
					}
				},
				Keywords = new string[]
				{
					searchObject.SearchQuery
				}
			};
			if (searchObject.Language != null)
			{
				findMailboxStatisticsByKeywordsType.Language = searchObject.Language.ToString();
			}
			if (searchObject.Senders != null && searchObject.Senders.Count > 0)
			{
				findMailboxStatisticsByKeywordsType.Senders = searchObject.Senders.ToArray();
			}
			if (searchObject.Recipients != null && searchObject.Recipients.Count > 0)
			{
				findMailboxStatisticsByKeywordsType.Recipients = searchObject.Recipients.ToArray();
			}
			if (searchObject.StartDate != null)
			{
				findMailboxStatisticsByKeywordsType.FromDate = (DateTime)searchObject.StartDate.Value.ToUtc();
				findMailboxStatisticsByKeywordsType.FromDateSpecified = true;
			}
			if (searchObject.EndDate != null)
			{
				findMailboxStatisticsByKeywordsType.ToDate = (DateTime)searchObject.EndDate.Value.ToUtc();
				findMailboxStatisticsByKeywordsType.ToDateSpecified = true;
			}
			if (searchObject.SearchDumpster)
			{
				findMailboxStatisticsByKeywordsType.SearchDumpster = true;
				findMailboxStatisticsByKeywordsType.SearchDumpsterSpecified = true;
			}
			if (searchObject.IncludePersonalArchive)
			{
				findMailboxStatisticsByKeywordsType.IncludePersonalArchive = true;
				findMailboxStatisticsByKeywordsType.IncludePersonalArchiveSpecified = true;
			}
			if (searchObject.IncludeUnsearchableItems)
			{
				findMailboxStatisticsByKeywordsType.IncludeUnsearchableItems = true;
				findMailboxStatisticsByKeywordsType.IncludeUnsearchableItemsSpecified = true;
			}
			if (searchObject.MessageTypes != null && searchObject.MessageTypes.Count > 0)
			{
				List<SearchItemKindType> list = new List<SearchItemKindType>(searchObject.MessageTypes.Count);
				foreach (KindKeyword kindKeyword in searchObject.MessageTypes)
				{
					list.Add((SearchItemKindType)Enum.Parse(typeof(SearchItemKindType), kindKeyword.ToString(), true));
				}
				findMailboxStatisticsByKeywordsType.MessageTypes = list.ToArray();
			}
			FindMailboxStatisticsByKeywordsResponseType findMailboxStatisticsByKeywordsResponseType = null;
			int num = 0;
			Exception ex = null;
			do
			{
				try
				{
					ex = null;
					findMailboxStatisticsByKeywordsResponseType = this.binding.FindMailboxStatisticsByKeywords(findMailboxStatisticsByKeywordsType);
					break;
				}
				catch (SoapException ex2)
				{
					ex = ex2;
				}
				catch (WebException ex3)
				{
					ex = ex3;
				}
				catch (IOException ex4)
				{
					ex = ex4;
				}
				catch (InvalidOperationException ex5)
				{
					ex = ex5;
				}
			}
			while (++num < 3);
			if (ex != null)
			{
				ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), ex.ToString());
				throw new MailboxSearchEwsFailedException(ex.Message);
			}
			if (findMailboxStatisticsByKeywordsResponseType != null && findMailboxStatisticsByKeywordsResponseType.ResponseMessages != null && findMailboxStatisticsByKeywordsResponseType.ResponseMessages.Items != null && findMailboxStatisticsByKeywordsResponseType.ResponseMessages.Items.Length > 0)
			{
				List<KeywordHit> list2 = new List<KeywordHit>(findMailboxStatisticsByKeywordsResponseType.ResponseMessages.Items.Length);
				foreach (FindMailboxStatisticsByKeywordsResponseMessageType findMailboxStatisticsByKeywordsResponseMessageType in findMailboxStatisticsByKeywordsResponseType.ResponseMessages.Items)
				{
					if (findMailboxStatisticsByKeywordsResponseMessageType.ResponseClass != ResponseClassType.Success)
					{
						ExTraceGlobals.SessionTracer.TraceError<ResponseClassType, ResponseCodeType, string>((long)this.GetHashCode(), "FindMailboxStatisticsByKeywords EWS call failed with response. ResponseClass={0}, ResponseCode={1}, MessageText={2}", findMailboxStatisticsByKeywordsResponseMessageType.ResponseClass, findMailboxStatisticsByKeywordsResponseMessageType.ResponseCode, findMailboxStatisticsByKeywordsResponseMessageType.MessageText);
						throw new MailboxSearchEwsFailedException(findMailboxStatisticsByKeywordsResponseMessageType.MessageText);
					}
					MailboxStatisticsSearchResultType mailboxStatisticsSearchResult = findMailboxStatisticsByKeywordsResponseMessageType.MailboxStatisticsSearchResult;
					KeywordStatisticsSearchResultType keywordStatisticsSearchResult = mailboxStatisticsSearchResult.KeywordStatisticsSearchResult;
					list2.Add(new KeywordHit
					{
						Phrase = keywordStatisticsSearchResult.Keyword,
						Count = keywordStatisticsSearchResult.ItemHits,
						MailboxCount = ((keywordStatisticsSearchResult.ItemHits > 0) ? 1 : 0),
						Size = new ByteQuantifiedSize((ulong)keywordStatisticsSearchResult.Size)
					});
				}
				return list2;
			}
			ExTraceGlobals.SessionTracer.TraceError((long)this.GetHashCode(), "Response of FindMailboxStatisticsByKeywords is empty");
			throw new MailboxSearchEwsFailedException(ServerStrings.MailboxSearchEwsEmptyResponse);
		}

		public void Dispose()
		{
			if (this.binding != null)
			{
				this.binding.Dispose();
				this.binding = null;
			}
		}

		private SecurityTokenService GetSecurityTokenService(OrganizationId organizationId)
		{
			if (this.securityTokenService == null)
			{
				ExternalAuthentication current = ExternalAuthentication.GetCurrent();
				if (current.Enabled)
				{
					this.securityTokenService = current.GetSecurityTokenService(organizationId);
					if (this.securityTokenService == null)
					{
						throw new AutoDAccessException(ServerStrings.AutoDFailedToGetToken);
					}
				}
			}
			return this.securityTokenService;
		}

		private IWebProxy WebProxy
		{
			get
			{
				if (this.webProxy == null)
				{
					Server localServer = LocalServerCache.LocalServer;
					if (localServer != null && localServer.InternetWebProxy != null)
					{
						this.webProxy = new WebProxy(localServer.InternetWebProxy);
					}
				}
				return this.webProxy;
			}
		}

		private const string ComponentId = "ExchangeEDiscovery";

		private const string ExternalEwsUrl = "ExternalEwsUrl";

		private const int DefaultSoapClientTimeout = 600000;

		private const int MaximumTransientFailureRetries = 3;

		private static readonly string[] AutoDiscoverRequestedSettings = new string[]
		{
			"ExternalEwsUrl"
		};

		private static readonly RequestServerVersion RequestServerVersionExchange2010 = new RequestServerVersion
		{
			Version = ExchangeVersionType.Exchange2010_SP1
		};

		private MailboxSearchEwsClient.MailboxSearchEwsBinding binding;

		private SecurityTokenService securityTokenService;

		private IWebProxy webProxy;

		private sealed class MailboxSearchEwsBinding : ExchangeServiceBinding
		{
			public event MailboxSearchEwsClient.MailboxSearchEwsBinding.FindMailboxStatisticsByKeywordsCompletedEventHandler FindMailboxStatisticsByKeywordsCompleted;

			public MailboxSearchEwsBinding(string componentId, RemoteCertificateValidationCallback remoteCertificateValidationCallback) : base(componentId, remoteCertificateValidationCallback)
			{
			}

			[SoapDocumentMethod("http://schemas.microsoft.com/exchange/services/2006/messages/FindMailboxStatisticsByKeywords", Use = SoapBindingUse.Literal, ParameterStyle = SoapParameterStyle.Bare)]
			[SoapHeader("ServerVersionInfoValue", Direction = SoapHeaderDirection.Out)]
			[SoapHeader("RequestServerVersionValue")]
			[SoapHttpClientTraceExtension]
			[return: XmlElement("FindMailboxStatisticsByKeywordsResponse", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")]
			public FindMailboxStatisticsByKeywordsResponseType FindMailboxStatisticsByKeywords([XmlElement("FindMailboxStatisticsByKeywords", Namespace = "http://schemas.microsoft.com/exchange/services/2006/messages")] FindMailboxStatisticsByKeywordsType findMailboxStatisticsByKeywords1)
			{
				object[] array = this.Invoke("FindMailboxStatisticsByKeywords", new object[]
				{
					findMailboxStatisticsByKeywords1
				});
				return (FindMailboxStatisticsByKeywordsResponseType)array[0];
			}

			public IAsyncResult BeginFindMailboxStatisticsByKeywords(FindMailboxStatisticsByKeywordsType findMailboxStatisticsByKeywords1, AsyncCallback callback, object asyncState)
			{
				return base.BeginInvoke("FindMailboxStatisticsByKeywords", new object[]
				{
					findMailboxStatisticsByKeywords1
				}, callback, asyncState);
			}

			public FindMailboxStatisticsByKeywordsResponseType EndFindMailboxStatisticsByKeywords(IAsyncResult asyncResult)
			{
				object[] array = base.EndInvoke(asyncResult);
				return (FindMailboxStatisticsByKeywordsResponseType)array[0];
			}

			public void FindMailboxStatisticsByKeywordsAsync(FindMailboxStatisticsByKeywordsType findMailboxStatisticsByKeywords1)
			{
				this.FindMailboxStatisticsByKeywordsAsync(findMailboxStatisticsByKeywords1, null);
			}

			public void FindMailboxStatisticsByKeywordsAsync(FindMailboxStatisticsByKeywordsType findMailboxStatisticsByKeywords1, object userState)
			{
				if (this.findMailboxStatisticsByKeywordsOperationCompleted == null)
				{
					this.findMailboxStatisticsByKeywordsOperationCompleted = new SendOrPostCallback(this.OnFindMailboxStatisticsByKeywordsOperationCompleted);
				}
				base.InvokeAsync("FindMailboxStatisticsByKeywords", new object[]
				{
					findMailboxStatisticsByKeywords1
				}, this.findMailboxStatisticsByKeywordsOperationCompleted, userState);
			}

			private void OnFindMailboxStatisticsByKeywordsOperationCompleted(object arg)
			{
				if (this.FindMailboxStatisticsByKeywordsCompleted != null)
				{
					InvokeCompletedEventArgs invokeCompletedEventArgs = (InvokeCompletedEventArgs)arg;
					this.FindMailboxStatisticsByKeywordsCompleted(this, new MailboxSearchEwsClient.MailboxSearchEwsBinding.FindMailboxStatisticsByKeywordsCompletedEventArgs(invokeCompletedEventArgs.Results, invokeCompletedEventArgs.Error, invokeCompletedEventArgs.Cancelled, invokeCompletedEventArgs.UserState));
				}
			}

			private SendOrPostCallback findMailboxStatisticsByKeywordsOperationCompleted;

			public delegate void FindMailboxStatisticsByKeywordsCompletedEventHandler(object sender, MailboxSearchEwsClient.MailboxSearchEwsBinding.FindMailboxStatisticsByKeywordsCompletedEventArgs e);

			[DesignerCategory("code")]
			[DebuggerStepThrough]
			public class FindMailboxStatisticsByKeywordsCompletedEventArgs : AsyncCompletedEventArgs
			{
				internal FindMailboxStatisticsByKeywordsCompletedEventArgs(object[] results, Exception exception, bool cancelled, object userState) : base(exception, cancelled, userState)
				{
					this.results = results;
				}

				public FindMailboxStatisticsByKeywordsResponseType Result
				{
					get
					{
						base.RaiseExceptionIfNecessary();
						return (FindMailboxStatisticsByKeywordsResponseType)this.results[0];
					}
				}

				private object[] results;
			}
		}
	}
}
