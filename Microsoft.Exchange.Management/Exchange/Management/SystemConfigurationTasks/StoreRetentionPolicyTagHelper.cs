using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.ServiceModel;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data.ApplicationLogic.Cafe;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics.Components.InfoWorker.Common;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.InfoWorker.Common.ELC;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	internal class StoreRetentionPolicyTagHelper : IDisposable
	{
		public Dictionary<Guid, StoreTagData> TagData
		{
			get
			{
				return this.tagData;
			}
			set
			{
				this.tagData = value;
			}
		}

		public Dictionary<Guid, StoreTagData> DefaultArchiveTagData
		{
			get
			{
				return this.defaultArchiveTagData;
			}
		}

		public ADUser Mailbox
		{
			get
			{
				return this.user;
			}
		}

		internal ExchangePrincipal UserPrincipal
		{
			get
			{
				return this.exchangePrincipal;
			}
		}

		public RetentionHoldData HoldData
		{
			get
			{
				return this.retentionHoldData;
			}
			set
			{
				this.retentionHoldData = value;
			}
		}

		public void Dispose()
		{
			this.recipientSession = null;
			this.user = null;
			this.tagData = null;
			if (this.mailboxSession != null)
			{
				this.mailboxSession.Dispose();
				this.mailboxSession = null;
			}
		}

		internal static StoreRetentionPolicyTagHelper FromMailboxId(string domainController, MailboxIdParameter mailbox, OrganizationId organizationId)
		{
			return StoreRetentionPolicyTagHelper.FromMailboxId(domainController, mailbox, false, organizationId);
		}

		private static bool HasOnPremArchiveMailbox(ExchangePrincipal exchangePrincipal)
		{
			return exchangePrincipal != null && exchangePrincipal.GetArchiveMailbox() != null;
		}

		internal static bool HasOnPremArchiveMailbox(ADUser user)
		{
			if (user.ArchiveState == ArchiveState.Local)
			{
				ExchangePrincipal exchangePrincipal = ExchangePrincipal.FromADUser(user, RemotingOptions.AllowCrossSite);
				return StoreRetentionPolicyTagHelper.HasOnPremArchiveMailbox(exchangePrincipal);
			}
			return false;
		}

		internal static void SyncOptionalTagsFromPrimaryToArchive(ADUser user)
		{
			if (user == null || !StoreRetentionPolicyTagHelper.HasOnPremArchiveMailbox(user))
			{
				return;
			}
			using (StoreRetentionPolicyTagHelper storeRetentionPolicyTagHelper = StoreRetentionPolicyTagHelper.FromADUser(user, false))
			{
				if (storeRetentionPolicyTagHelper.configItemExists)
				{
					using (StoreRetentionPolicyTagHelper storeRetentionPolicyTagHelper2 = StoreRetentionPolicyTagHelper.FromADUser(user, true))
					{
						List<StoreTagData> list = (from x in storeRetentionPolicyTagHelper.tagData.Values
						where !x.Tag.IsArchiveTag
						select x).ToList<StoreTagData>();
						if (!storeRetentionPolicyTagHelper2.configItemExists || storeRetentionPolicyTagHelper2.tagData == null || storeRetentionPolicyTagHelper2.tagData.Count == 0)
						{
							if (storeRetentionPolicyTagHelper2.tagData == null)
							{
								storeRetentionPolicyTagHelper2.tagData = new Dictionary<Guid, StoreTagData>(list.Count);
							}
							foreach (StoreTagData storeTagData in list)
							{
								if (!storeRetentionPolicyTagHelper2.tagData.Values.Contains(storeTagData))
								{
									storeRetentionPolicyTagHelper2.tagData.Add(storeTagData.Tag.RetentionId, storeTagData);
								}
							}
							storeRetentionPolicyTagHelper2.Save();
						}
					}
				}
			}
		}

		internal static ADUser FetchRecipientFromMailboxId(string domainController, MailboxIdParameter mailbox, out IRecipientSession session, OrganizationId orgId)
		{
			session = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(domainController, true, ConsistencyMode.IgnoreInvalid, orgId.ToADSessionSettings(), 191, "FetchRecipientFromMailboxId", "f:\\15.00.1497\\sources\\dev\\Management\\src\\Management\\Elc\\StoreRetentionPolicyTagHelper.cs");
			LocalizedString? localizedString = null;
			IEnumerable<ADUser> objects = mailbox.GetObjects<ADUser>(null, session, null, out localizedString);
			ADUser aduser = null;
			if (objects != null)
			{
				foreach (ADUser aduser2 in objects)
				{
					if (aduser != null)
					{
						throw new ManagementObjectAmbiguousException(Strings.ErrorRecipientNotUnique(mailbox.ToString()));
					}
					aduser = aduser2;
				}
			}
			if (aduser == null)
			{
				throw new ManagementObjectNotFoundException(localizedString ?? Strings.ErrorObjectNotFound(mailbox.ToString()));
			}
			return aduser;
		}

		internal static StoreRetentionPolicyTagHelper FromMailboxId(string domainController, MailboxIdParameter mailbox, bool isArchiveMailbox, OrganizationId orgId)
		{
			StoreRetentionPolicyTagHelper storeRetentionPolicyTagHelper = new StoreRetentionPolicyTagHelper();
			storeRetentionPolicyTagHelper.user = StoreRetentionPolicyTagHelper.FetchRecipientFromMailboxId(domainController, mailbox, out storeRetentionPolicyTagHelper.recipientSession, orgId);
			storeRetentionPolicyTagHelper.isArchiveMailbox = isArchiveMailbox;
			StoreRetentionPolicyTagHelper.FetchRetentionPolicyTagDataFromMailbox(storeRetentionPolicyTagHelper);
			return storeRetentionPolicyTagHelper;
		}

		private static StoreRetentionPolicyTagHelper FromADUser(ADUser user, bool isArchiveMailbox)
		{
			StoreRetentionPolicyTagHelper storeRetentionPolicyTagHelper = new StoreRetentionPolicyTagHelper();
			storeRetentionPolicyTagHelper.user = user;
			storeRetentionPolicyTagHelper.isArchiveMailbox = isArchiveMailbox;
			StoreRetentionPolicyTagHelper.FetchRetentionPolicyTagDataFromMailbox(storeRetentionPolicyTagHelper);
			return storeRetentionPolicyTagHelper;
		}

		private static void FetchRetentionPolicyTagDataFromMailbox(StoreRetentionPolicyTagHelper srpth)
		{
			StoreRetentionPolicyTagHelper.InitializePrincipal(srpth);
			if (srpth.exchangePrincipal != null)
			{
				if (srpth.exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E14SP1MinVersion)
				{
					ExTraceGlobals.ELCTracer.TraceDebug(0L, "Fetch retention policy tag data from EWS since user's version is " + srpth.exchangePrincipal.MailboxInfo.Location.ServerVersion);
					StoreRetentionPolicyTagHelper.FetchRetentionPolicyTagDataFromService(srpth);
					return;
				}
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "Fetch retention policy tag data from XSO since user's version is " + srpth.exchangePrincipal.MailboxInfo.Location.ServerVersion);
				StoreRetentionPolicyTagHelper.FetchRetentionPolcyTagDataFromXSO(srpth);
			}
		}

		private static void InitializePrincipal(StoreRetentionPolicyTagHelper srpth)
		{
			if (srpth.user != null)
			{
				srpth.exchangePrincipal = ExchangePrincipal.FromADUser(srpth.user.OrganizationId.ToADSessionSettings(), srpth.user);
				if (srpth.exchangePrincipal != null && srpth.isArchiveMailbox && srpth.user.ArchiveState == ArchiveState.Local)
				{
					srpth.exchangePrincipal = srpth.exchangePrincipal.GetArchiveExchangePrincipal(RemotingOptions.LocalConnectionsOnly);
					return;
				}
			}
			else
			{
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "Exchange principal cannot be found because user is not available.");
			}
		}

		private static void FetchRetentionPolcyTagDataFromXSO(StoreRetentionPolicyTagHelper srpth)
		{
			if (srpth.exchangePrincipal == null)
			{
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "Cannot fetch retention policy tag data because Exchange principal is not available.");
				return;
			}
			srpth.mailboxSession = MailboxSession.OpenAsAdmin(srpth.exchangePrincipal, CultureInfo.InvariantCulture, "Client=Management;Action=Get-/Set-RetentionPolicyTag");
			srpth.configItem = ElcMailboxHelper.OpenFaiMessage(srpth.mailboxSession, "MRM", true);
			if (srpth.configItem != null)
			{
				srpth.TagData = MrmFaiFormatter.Deserialize(srpth.configItem, srpth.exchangePrincipal, out srpth.deletedTags, out srpth.retentionHoldData, true, out srpth.defaultArchiveTagData, out srpth.fullCrawlRequired);
				return;
			}
			srpth.TagData = new Dictionary<Guid, StoreTagData>();
		}

		private static void FetchRetentionPolicyTagDataFromService(StoreRetentionPolicyTagHelper srpth)
		{
			StoreRetentionPolicyTagHelper.InitializeServiceBinding(srpth);
			if (srpth.serviceBinding != null)
			{
				GetUserConfigurationType getUserConfiguration = new GetUserConfigurationType
				{
					UserConfigurationName = new UserConfigurationNameType
					{
						Name = "MRM",
						Item = new DistinguishedFolderIdType
						{
							Id = DistinguishedFolderIdNameType.inbox
						}
					},
					UserConfigurationProperties = (UserConfigurationPropertyType.Dictionary | UserConfigurationPropertyType.XmlData | UserConfigurationPropertyType.BinaryData)
				};
				StoreRetentionPolicyTagHelper.CallEwsWithRetries(() => srpth.serviceBinding.GetUserConfiguration(getUserConfiguration), delegate(ResponseMessageType responseMessage, int messageIndex)
				{
					GetUserConfigurationResponseMessageType getUserConfigurationResponseMessageType = responseMessage as GetUserConfigurationResponseMessageType;
					if (getUserConfigurationResponseMessageType.ResponseClass == ResponseClassType.Success && getUserConfigurationResponseMessageType.UserConfiguration != null)
					{
						if (getUserConfigurationResponseMessageType.UserConfiguration.XmlData != null)
						{
							ExTraceGlobals.ELCTracer.TraceDebug(0L, "Acquired MRM user configuration.");
							srpth.TagData = MrmFaiFormatter.Deserialize(getUserConfigurationResponseMessageType.UserConfiguration.XmlData, srpth.exchangePrincipal, out srpth.deletedTags, out srpth.retentionHoldData, true, out srpth.defaultArchiveTagData, out srpth.fullCrawlRequired);
							srpth.configItemExists = true;
						}
						else
						{
							ExTraceGlobals.ELCTracer.TraceDebug(0L, "MRM user configuration is null");
							srpth.TagData = new Dictionary<Guid, StoreTagData>();
							srpth.configItemExists = false;
						}
						return true;
					}
					if (getUserConfigurationResponseMessageType.ResponseClass == ResponseClassType.Error && getUserConfigurationResponseMessageType.ResponseCode == ResponseCodeType.ErrorItemNotFound)
					{
						ExTraceGlobals.ELCTracer.TraceDebug(0L, "MRM user configuration not found");
						srpth.TagData = new Dictionary<Guid, StoreTagData>();
						srpth.configItemExists = false;
						return true;
					}
					ExTraceGlobals.ELCTracer.TraceDebug(0L, "Getting MRM user configuration failed");
					return false;
				}, srpth);
				return;
			}
			throw new ElcUserConfigurationException(Strings.ElcUserConfigurationServiceBindingNotAvailable);
		}

		private static void InitializeServiceBinding(StoreRetentionPolicyTagHelper srpth)
		{
			if (srpth.exchangePrincipal != null)
			{
				string text = StoreRetentionPolicyTagHelper.DiscoverEwsUrl(srpth.exchangePrincipal);
				if (string.IsNullOrEmpty(text))
				{
					return;
				}
				srpth.serviceBinding = StoreRetentionPolicyTagHelper.CreateBinding(srpth.exchangePrincipal.MailboxInfo.PrimarySmtpAddress.ToString());
				srpth.serviceBinding.Timeout = (int)StoreRetentionPolicyTagHelper.DefaultSoapClientTimeout.TotalMilliseconds;
				srpth.serviceBinding.Url = text;
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "Refreshed service binding, new url: " + text);
			}
		}

		private static string DiscoverEwsUrl(ExchangePrincipal exchangePrincipal)
		{
			ExTraceGlobals.ELCTracer.TraceDebug<ExchangePrincipal>(0L, "Will try to discover the URL for EWS with the Backendlocator for mailbox {0}", exchangePrincipal);
			try
			{
				Uri backEndWebServicesUrl = BackEndLocator.GetBackEndWebServicesUrl(exchangePrincipal.MailboxInfo);
				if (backEndWebServicesUrl != null)
				{
					ExTraceGlobals.ELCTracer.TraceDebug<string, ExchangePrincipal>(0L, "Found Uri from the back end locator.{0}, {1}", backEndWebServicesUrl.ToString(), exchangePrincipal);
					return backEndWebServicesUrl.ToString();
				}
				ExTraceGlobals.ELCTracer.TraceDebug<ExchangePrincipal>(0L, "Unable to discover internal URL for EWS for mailbox {0}. BackEndLocator call returned null", exchangePrincipal);
			}
			catch (LocalizedException arg)
			{
				ExTraceGlobals.ELCTracer.TraceError<ExchangePrincipal, LocalizedException>(0L, "Unable to discover internal URL for EWS for mailbox {0} due exception {1}", exchangePrincipal, arg);
			}
			return null;
		}

		private static ExchangeServiceBinding CreateBinding(string email)
		{
			NetworkServiceImpersonator.Initialize();
			if (NetworkServiceImpersonator.Exception != null)
			{
				ExTraceGlobals.ELCTracer.TraceError<LocalizedException>(0L, "Unable to impersonate network service to call EWS due to exception {0}", NetworkServiceImpersonator.Exception);
				throw new ElcUserConfigurationException(Strings.ElcUserConfigurationServiceBindingNotAvailable, NetworkServiceImpersonator.Exception);
			}
			ExchangeServiceBinding exchangeServiceBinding = new ExchangeServiceBinding(new RemoteCertificateValidationCallback(StoreRetentionPolicyTagHelper.CertificateErrorHandler));
			exchangeServiceBinding.UserAgent = WellKnownUserAgent.GetEwsNegoAuthUserAgent("MRMTask");
			exchangeServiceBinding.RequestServerVersionValue = new RequestServerVersion
			{
				Version = ExchangeVersionType.Exchange2010_SP1
			};
			StoreRetentionPolicyTagHelper.SetSecurityHeader(exchangeServiceBinding, email);
			return exchangeServiceBinding;
		}

		private static void SetSecurityHeader(ExchangeServiceBinding binding, string email)
		{
			binding.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
			binding.Authenticator.AdditionalSoapHeaders.Add(new OpenAsAdminOrSystemServiceType
			{
				ConnectingSID = new ConnectingSIDType
				{
					Item = new PrimarySmtpAddressType
					{
						Value = email
					}
				},
				LogonType = SpecialLogonType.SystemService
			});
		}

		internal static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				return true;
			}
			if (SslConfiguration.AllowInternalUntrustedCerts)
			{
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "Accepting SSL certificate because registry config AllowInternalUntrustedCerts tells to ignore errors");
				return true;
			}
			ExTraceGlobals.ELCTracer.TraceError<SslPolicyErrors>(0L, "Failed because SSL certificate contains the following errors: {0}", sslPolicyErrors);
			return false;
		}

		private static void UpdateUserConfiguration(StoreRetentionPolicyTagHelper tagHelper)
		{
			byte[] xmlData = MrmFaiFormatter.Serialize(tagHelper.TagData, tagHelper.defaultArchiveTagData, tagHelper.deletedTags, tagHelper.retentionHoldData, tagHelper.fullCrawlRequired, tagHelper.exchangePrincipal);
			UpdateUserConfigurationType updateUserConfiguration = new UpdateUserConfigurationType
			{
				UserConfiguration = new UserConfigurationType
				{
					UserConfigurationName = new UserConfigurationNameType
					{
						Name = "MRM",
						Item = new DistinguishedFolderIdType
						{
							Id = DistinguishedFolderIdNameType.inbox
						}
					},
					XmlData = xmlData
				}
			};
			StoreRetentionPolicyTagHelper.CallEwsWithRetries(() => tagHelper.serviceBinding.UpdateUserConfiguration(updateUserConfiguration), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Success)
				{
					ExTraceGlobals.ELCTracer.TraceDebug(0L, "Successfully updated MRM user configuration");
					return true;
				}
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "MRM user configuration was not updated");
				return false;
			}, tagHelper);
		}

		private static void CreateUserConfiguration(StoreRetentionPolicyTagHelper tagHelper)
		{
			byte[] xmlData = MrmFaiFormatter.Serialize(tagHelper.TagData, tagHelper.defaultArchiveTagData, tagHelper.deletedTags, tagHelper.retentionHoldData, tagHelper.fullCrawlRequired, tagHelper.exchangePrincipal);
			CreateUserConfigurationType createUserConfiguration = new CreateUserConfigurationType
			{
				UserConfiguration = new UserConfigurationType
				{
					UserConfigurationName = new UserConfigurationNameType
					{
						Name = "MRM",
						Item = new DistinguishedFolderIdType
						{
							Id = DistinguishedFolderIdNameType.inbox
						}
					},
					XmlData = xmlData
				}
			};
			StoreRetentionPolicyTagHelper.CallEwsWithRetries(() => tagHelper.serviceBinding.CreateUserConfiguration(createUserConfiguration), delegate(ResponseMessageType responseMessage, int messageIndex)
			{
				if (responseMessage.ResponseClass == ResponseClassType.Success)
				{
					ExTraceGlobals.ELCTracer.TraceDebug(0L, "Successfully created MRM user configuration");
					return true;
				}
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "MRM user configuration was not created");
				return false;
			}, tagHelper);
		}

		private static void CallEwsWithRetries(Func<BaseResponseMessageType> delegateEwsCall, Func<ResponseMessageType, int, bool> responseMessageProcessor, StoreRetentionPolicyTagHelper srpth)
		{
			ExDateTime t = ExDateTime.UtcNow.Add(StoreRetentionPolicyTagHelper.TotalExecutionTimeWindow);
			Exception ex = null;
			int num = -1;
			bool flag;
			do
			{
				ex = null;
				flag = false;
				try
				{
					BaseResponseMessageType baseResponseMessageType = delegateEwsCall();
					num++;
					int i = 0;
					while (i < baseResponseMessageType.ResponseMessages.Items.Length)
					{
						ResponseMessageType responseMessageType = baseResponseMessageType.ResponseMessages.Items[i];
						if (responseMessageProcessor != null && responseMessageProcessor(responseMessageType, i))
						{
							ExTraceGlobals.ELCTracer.TraceDebug(0L, "Successfully executed EWS call");
							break;
						}
						if (responseMessageType.ResponseClass == ResponseClassType.Error)
						{
							if (responseMessageType.ResponseCode == ResponseCodeType.ErrorCrossSiteRequest)
							{
								ExTraceGlobals.ELCTracer.TraceDebug(0L, "Crosssite request error , recreate exchange binding and reset the url caches");
								flag = true;
								StoreRetentionPolicyTagHelper.InitializePrincipal(srpth);
								StoreRetentionPolicyTagHelper.InitializeServiceBinding(srpth);
								break;
							}
							if (!StoreRetentionPolicyTagHelper.TransientServiceErrors.Contains(responseMessageType.ResponseCode))
							{
								ExTraceGlobals.ELCTracer.TraceError(0L, string.Format("Permanent error encountered:  {0}, {1}, {2}", responseMessageType.ResponseClass.ToString(), responseMessageType.ResponseCode.ToString(), responseMessageType.MessageText.ToString()));
								throw new ElcUserConfigurationException(Strings.FailedToGetElcUserConfigurationFromService(responseMessageType.ResponseClass.ToString(), responseMessageType.ResponseCode.ToString(), responseMessageType.MessageText.ToString()));
							}
							flag = true;
							ex = new ElcUserConfigurationException(Strings.FailedToGetElcUserConfigurationFromService(responseMessageType.ResponseClass.ToString(), responseMessageType.ResponseCode.ToString(), responseMessageType.MessageText.ToString()));
							ExTraceGlobals.ELCTracer.TraceDebug(0L, "Transient error encountered, will attempt to retry, Exception: " + ex);
							break;
						}
						else
						{
							i++;
						}
					}
				}
				catch (CommunicationException ex2)
				{
					ex = ex2;
					ExTraceGlobals.ELCTracer.TraceDebug(0L, "Transient error encountered, will attempt to retry, Exception: " + ex);
					flag = true;
				}
				catch (WebException ex3)
				{
					ex = ex3;
					ExTraceGlobals.ELCTracer.TraceDebug(0L, "Transient error encountered, will attempt to retry, Exception: " + ex);
					flag = true;
				}
				catch (TimeoutException ex4)
				{
					ex = ex4;
					flag = false;
				}
				catch (SoapException ex5)
				{
					ex = ex5;
					flag = false;
				}
				catch (IOException ex6)
				{
					ex = ex6;
					flag = false;
				}
				catch (InvalidOperationException ex7)
				{
					ex = ex7;
					flag = false;
				}
				catch (LocalizedException ex8)
				{
					ex = ex8;
					flag = false;
				}
			}
			while (flag && t > ExDateTime.UtcNow);
			if (ex != null)
			{
				ExTraceGlobals.ELCTracer.TraceError(0L, string.Format("Failed to access elc user configuration.  Total attempts made {0}, Exception: {1} ", num, ex));
				throw new ElcUserConfigurationException(Strings.ErrorElcUserConfigurationServiceCall, ex);
			}
		}

		internal void Save()
		{
			if (this.exchangePrincipal == null)
			{
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "Cannot save changes because Exchange principal is not available to verify version.");
				return;
			}
			if (this.exchangePrincipal.MailboxInfo.Location.ServerVersion >= Server.E14SP1MinVersion)
			{
				ExTraceGlobals.ELCTracer.TraceDebug(0L, "Save changes to EWS since user's version is " + this.exchangePrincipal.MailboxInfo.Location.ServerVersion);
				this.SaveToService();
				return;
			}
			ExTraceGlobals.ELCTracer.TraceDebug(0L, "Save changes to XSO since user's version is " + this.exchangePrincipal.MailboxInfo.Location.ServerVersion);
			this.SaveToXSO();
		}

		private void SaveToService()
		{
			StoreRetentionPolicyTagHelper.InitializePrincipal(this);
			StoreRetentionPolicyTagHelper.InitializeServiceBinding(this);
			if (this.serviceBinding == null)
			{
				throw new ElcUserConfigurationException(Strings.ElcUserConfigurationServiceBindingNotAvailable);
			}
			if (this.configItemExists)
			{
				StoreRetentionPolicyTagHelper.UpdateUserConfiguration(this);
				return;
			}
			StoreRetentionPolicyTagHelper.CreateUserConfiguration(this);
		}

		internal void SaveToXSO()
		{
			if (this.configItem != null)
			{
				MrmFaiFormatter.Serialize(this.TagData, this.defaultArchiveTagData, this.deletedTags, this.retentionHoldData, this.configItem, this.fullCrawlRequired, this.mailboxSession.MailboxOwner);
				this.configItem.Save();
				return;
			}
			throw new ElcUserConfigurationException(Strings.ElcUserConfigurationConfigurationItemNotAvailable);
		}

		private const string ServiceBindingComponentId = "MRMTask";

		private const UserConfigurationPropertyType ElcConfigurationTypes = UserConfigurationPropertyType.Dictionary | UserConfigurationPropertyType.XmlData | UserConfigurationPropertyType.BinaryData;

		private static TimeSpan ServiceTopologyTimeout = TimeSpan.FromSeconds(10.0);

		private static readonly TimeSpan TotalExecutionTimeWindow = TimeSpan.FromSeconds(10.0);

		private static readonly TimeSpan DefaultSoapClientTimeout = TimeSpan.FromSeconds(120.0);

		private static readonly List<ResponseCodeType> TransientServiceErrors = new List<ResponseCodeType>
		{
			ResponseCodeType.ErrorADOperation,
			ResponseCodeType.ErrorADUnavailable,
			ResponseCodeType.ErrorConnectionFailed,
			ResponseCodeType.ErrorInsufficientResources,
			ResponseCodeType.ErrorInternalServerTransientError,
			ResponseCodeType.ErrorMailboxMoveInProgress,
			ResponseCodeType.ErrorMailboxStoreUnavailable,
			ResponseCodeType.ErrorServerBusy,
			ResponseCodeType.ErrorCrossSiteRequest,
			ResponseCodeType.ErrorExceededConnectionCount
		};

		private IRecipientSession recipientSession;

		private ADUser user;

		private Dictionary<Guid, StoreTagData> tagData;

		private bool fullCrawlRequired;

		private Dictionary<Guid, StoreTagData> defaultArchiveTagData;

		private List<Guid> deletedTags;

		private MailboxSession mailboxSession;

		private RetentionHoldData retentionHoldData;

		private ExchangePrincipal exchangePrincipal;

		private bool configItemExists;

		private bool isArchiveMailbox;

		private ExchangeServiceBinding serviceBinding;

		private UserConfiguration configItem;
	}
}
