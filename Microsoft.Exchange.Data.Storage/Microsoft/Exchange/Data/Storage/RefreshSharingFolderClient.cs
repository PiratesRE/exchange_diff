using System;
using System.Collections.Generic;
using System.IO;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;
using System.Security.Principal;
using System.Web.Services.Protocols;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage.Principal;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Diagnostics.Components.Data.Storage;
using Microsoft.Exchange.ExchangeSystem;
using Microsoft.Exchange.Net;
using Microsoft.Exchange.Net.Protocols;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Exchange.SoapWebClient;
using Microsoft.Exchange.SoapWebClient.EWS;
using Microsoft.Mapi.Security;

namespace Microsoft.Exchange.Data.Storage
{
	[ClassAccessLevel(AccessLevel.MSInternal)]
	internal static class RefreshSharingFolderClient
	{
		internal static void Sync(Folder folder)
		{
			MailboxSession mailboxSession = folder.Session as MailboxSession;
			if (mailboxSession == null)
			{
				RefreshSharingFolderClient.Tracer.TraceDebug<string>(0L, "Sync for folder {0} called with a non-mailbox session. Ignoring.", folder.DisplayName);
				return;
			}
			SecurityAccessToken securityAccessToken = new SecurityAccessToken();
			using (ClientSecurityContext clientSecurityContext = RefreshSharingFolderClient.GetClientSecurityContext(mailboxSession))
			{
				clientSecurityContext.SetSecurityAccessToken(securityAccessToken);
			}
			ExDateTime valueOrDefault = folder.GetValueOrDefault<ExDateTime>(FolderSchema.SubscriptionLastSuccessfulSyncTime, ExDateTime.MinValue);
			RefreshSharingFolderClient.SyncInternal(mailboxSession, valueOrDefault, folder.Id, folder.DisplayName, securityAccessToken);
		}

		internal static bool CanSyncNow(ExDateTime lastSyncTime)
		{
			bool flag = lastSyncTime + RefreshSharingFolderClient.MinimumSyncWait > ExDateTime.UtcNow;
			if (flag)
			{
				RefreshSharingFolderClient.Tracer.TraceDebug<ExDateTime>(0L, "Not syncing now because already synced at {0}", lastSyncTime);
			}
			return !flag;
		}

		private static void SyncInternal(MailboxSession mailboxSession, ExDateTime lastSyncTime, StoreId folderId, string folderDisplayname, SecurityAccessToken securityAccessToken)
		{
			RefreshSharingFolderClient.Tracer.TraceDebug<string, IExchangePrincipal>(0L, "Sync sharing folder {0} from mailbox {1}", folderDisplayname, mailboxSession.MailboxOwner);
			if (!RefreshSharingFolderClient.CanSyncNow(lastSyncTime))
			{
				return;
			}
			NetworkServiceImpersonator.Initialize();
			if (NetworkServiceImpersonator.Exception != null)
			{
				RefreshSharingFolderClient.Tracer.TraceError<LocalizedException>(0L, "Unable to impersonate network service to call EWS due exception {0}", NetworkServiceImpersonator.Exception);
				return;
			}
			string id = StoreId.StoreIdToEwsId(mailboxSession.MailboxOwner.MailboxInfo.MailboxGuid, folderId);
			IList<WebServicesService> list;
			try
			{
				ClientAccessType clientAccessType = RefreshSharingFolderClient.ClientAccessType;
				RefreshSharingFolderClient.Tracer.TraceDebug<ClientAccessType>(0L, "Will try to discover the URL for EWS with the following client access type: {0}", clientAccessType);
				ServiceTopology currentServiceTopology = ServiceTopology.GetCurrentServiceTopology("f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\RefreshSharingFolderClient.cs", "SyncInternal", 153);
				list = currentServiceTopology.FindAll<WebServicesService>(mailboxSession.MailboxOwner, clientAccessType, "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\RefreshSharingFolderClient.cs", "SyncInternal", 154);
			}
			catch (LocalizedException arg)
			{
				RefreshSharingFolderClient.Tracer.TraceError<IExchangePrincipal, LocalizedException>(0L, "Unable to discover internal URL for EWS for mailbox {0} due exception {1}", mailboxSession.MailboxOwner, arg);
				return;
			}
			if (list.Count != 0)
			{
				string text = list[0].Url.ToString();
				RefreshSharingFolderClient.Tracer.TraceDebug<string>(0L, "Using {0} to call BeginRefreshSharingFolder", text);
				ExchangeServiceBinding exchangeServiceBinding = new ExchangeServiceBinding("RefreshSharingFolder", new RemoteCertificateValidationCallback(RefreshSharingFolderClient.CertificateErrorHandler));
				exchangeServiceBinding.Url = text;
				exchangeServiceBinding.RequestServerVersionValue = new RequestServerVersion();
				exchangeServiceBinding.RequestServerVersionValue.Version = ExchangeVersionType.Exchange2010;
				exchangeServiceBinding.UserAgent = WellKnownUserAgent.GetEwsNegoAuthUserAgent("RefreshSharingFolder");
				exchangeServiceBinding.Authenticator = SoapHttpClientAuthenticator.CreateNetworkService();
				exchangeServiceBinding.Authenticator.AdditionalSoapHeaders.Add(new SerializedSecurityContextType
				{
					UserSid = securityAccessToken.UserSid,
					GroupSids = RefreshSharingFolderClient.SidStringAndAttributesConverter(securityAccessToken.GroupSids),
					RestrictedGroupSids = RefreshSharingFolderClient.SidStringAndAttributesConverter(securityAccessToken.RestrictedGroupSids),
					PrimarySmtpAddress = mailboxSession.MailboxOwner.MailboxInfo.PrimarySmtpAddress.ToString()
				});
				Exception ex = null;
				try
				{
					exchangeServiceBinding.BeginRefreshSharingFolder(new RefreshSharingFolderType
					{
						SharingFolderId = new FolderIdType
						{
							Id = id
						}
					}, new AsyncCallback(RefreshSharingFolderClient.TraceResponse), exchangeServiceBinding);
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
				catch (LocalizedException ex6)
				{
					ex = ex6;
				}
				if (ex != null)
				{
					RefreshSharingFolderClient.Tracer.TraceError<StoreId, IExchangePrincipal, Exception>(0L, "Unable to sync sharing folder {0} in mailbox {1}. Exception {2}", folderId, mailboxSession.MailboxOwner, ex);
				}
				return;
			}
			RefreshSharingFolderClient.Tracer.TraceError<IExchangePrincipal>(0L, "Unable to find internal URL for EWS for mailbox {0}", mailboxSession.MailboxOwner);
		}

		private static void TraceResponse(IAsyncResult asyncResult)
		{
			ExchangeServiceBinding exchangeServiceBinding = (ExchangeServiceBinding)asyncResult.AsyncState;
			RefreshSharingFolderClient.Tracer.TraceDebug<string>(0L, "Received RefreshSharingFolder response from {0}", exchangeServiceBinding.Url);
			Exception ex = null;
			try
			{
				exchangeServiceBinding.EndRefreshSharingFolder(asyncResult);
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
			catch (LocalizedException ex6)
			{
				ex = ex6;
			}
			if (ex != null)
			{
				RefreshSharingFolderClient.Tracer.TraceError<Exception>(0L, "Exception when receiving response from RefreshSharingFolder: {0}", ex);
			}
		}

		private static SidAndAttributesType[] SidStringAndAttributesConverter(SidStringAndAttributes[] sidStringAndAttributesArray)
		{
			if (sidStringAndAttributesArray == null)
			{
				return null;
			}
			return Array.ConvertAll<SidStringAndAttributes, SidAndAttributesType>(sidStringAndAttributesArray, (SidStringAndAttributes sidStringAndAttributes) => new SidAndAttributesType
			{
				SecurityIdentifier = sidStringAndAttributes.SecurityIdentifier,
				Attributes = sidStringAndAttributes.Attributes
			});
		}

		private static ClientSecurityContext GetClientSecurityContext(MailboxSession mailboxSession)
		{
			WindowsIdentity windowsIdentity = mailboxSession.Identity as WindowsIdentity;
			if (windowsIdentity != null)
			{
				return new ClientSecurityContext(windowsIdentity);
			}
			ClientIdentityInfo clientIdentityInfo = mailboxSession.Identity as ClientIdentityInfo;
			if (clientIdentityInfo != null)
			{
				return ClientSecurityContext.DuplicateAuthZContextHandle(clientIdentityInfo.hAuthZ);
			}
			RefreshSharingFolderClient.Tracer.TraceError(0L, "Mailbox session has identity object that we cannot handle: {0}", new object[]
			{
				mailboxSession.Identity
			});
			throw new InvalidOperationException();
		}

		private static bool CertificateErrorHandler(object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			if (sslPolicyErrors == SslPolicyErrors.None)
			{
				return true;
			}
			if (sslPolicyErrors == SslPolicyErrors.RemoteCertificateNameMismatch)
			{
				RefreshSharingFolderClient.Tracer.TraceDebug(0L, "Accepting SSL certificate because the only error is invalid hostname");
				return true;
			}
			if (SslConfiguration.AllowInternalUntrustedCerts)
			{
				RefreshSharingFolderClient.Tracer.TraceDebug(0L, "Accepting SSL certificate because registry config AllowInternalUntrustedCerts tells to ignore errors");
				return true;
			}
			RefreshSharingFolderClient.Tracer.TraceError<SslPolicyErrors>(0L, "Failed because SSL certificate contains the following errors: {0}", sslPolicyErrors);
			return false;
		}

		private static ClientAccessType ClientAccessType
		{
			get
			{
				ITopologyConfigurationSession topologyConfigurationSession = DirectorySessionFactory.Default.CreateTopologyConfigurationSession(ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromOrganizationIdWithoutRbacScopesServiceOnly(OrganizationId.ForestWideOrgId), 430, "ClientAccessType", "f:\\15.00.1497\\sources\\dev\\data\\src\\storage\\Sharing\\RefreshSharingFolderClient.cs");
				if (!topologyConfigurationSession.FindLocalServer().IsClientAccessServer)
				{
					return ClientAccessType.Internal;
				}
				return ClientAccessType.InternalNLBBypass;
			}
		}

		private const string CertificateValidationComponentId = "RefreshSharingFolder";

		private static readonly TimeSpan MinimumSyncWait = TimeSpan.FromMinutes(30.0);

		private static readonly Trace Tracer = ExTraceGlobals.SharingTracer;
	}
}
