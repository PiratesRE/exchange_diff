using System;
using System.Collections.Generic;
using System.IdentityModel.Claims;
using System.Security.Principal;
using System.ServiceModel;
using System.ServiceModel.Channels;
using Microsoft.Exchange.Collections.TimeoutCache;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Directory.SystemConfiguration.ConfigurationSettings;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.MailboxReplicationService
{
	public class MRSProxyAuthorizationManager : AuthorizationManagerBase
	{
		private TestIntegration TestIntegration
		{
			get
			{
				if (this.testIntegration == null)
				{
					this.testIntegration = new TestIntegration(false);
				}
				return this.testIntegration;
			}
		}

		internal override AdminRoleDefinition[] ComputeAdminRoles(IRootOrganizationRecipientSession recipientSession, ITopologyConfigurationSession configSession)
		{
			string containerDN = configSession.ConfigurationNamingContext.ToDNString();
			ADGroup adgroup = recipientSession.ResolveWellKnownGuid<ADGroup>(WellKnownGuid.EmaWkGuid, containerDN);
			List<AdminRoleDefinition> list = new List<AdminRoleDefinition>(4);
			list.Add(new AdminRoleDefinition(adgroup.Sid, "RecipientAdmins"));
			list.Add(new AdminRoleDefinition(recipientSession.GetExchangeServersUsgSid(), "ExchangeServers"));
			list.Add(new AdminRoleDefinition(new SecurityIdentifier(WellKnownSidType.LocalSystemSid, null), "LocalSystem"));
			list.Add(new AdminRoleDefinition(new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, null), "BuiltinAdmins"));
			SecurityIdentifier[] additionalSids = this.GetAdditionalSids();
			for (int i = 0; i < additionalSids.Length; i++)
			{
				string roleName = string.Format("AdditionalAdminRoleFromConfiguration {0}", i);
				list.Add(new AdminRoleDefinition(additionalSids[i], roleName));
			}
			return list.ToArray();
		}

		internal override IAuthenticationInfo Authenticate(OperationContext operationContext)
		{
			if (operationContext.ServiceSecurityContext.IsAnonymous)
			{
				return null;
			}
			if (this.IsCertificateAuthentication(operationContext))
			{
				return new AuthenticationInfo(true);
			}
			Guid fromHeaders = this.GetFromHeaders<Guid>(operationContext, "MailboxGuid");
			if (fromHeaders == Guid.Empty)
			{
				MrsTracer.ProxyService.Debug("MRSProxyAuthorizationManager.Authenticate: did not find MailboxGuid header on the message. It is valid for remote PST.", new object[0]);
			}
			MrsTracer.ActivityID = fromHeaders.GetHashCode();
			return this.GetPrincipal(operationContext);
		}

		internal override bool PostAuthnCheck(OperationContext operationContext, IAuthenticationInfo authenticationInfo)
		{
			if (operationContext.ServiceSecurityContext.IsAnonymous)
			{
				return false;
			}
			MRSProxyAuthorizationManager.AuthenticationData authenticationData = this.GetAuthenticationData(operationContext);
			if (authenticationData.IsAuthorized)
			{
				return true;
			}
			if (authenticationInfo.IsCertificateAuthentication)
			{
				authenticationData.IsAuthorized = true;
			}
			else
			{
				authenticationData.IsAuthorized = base.PostAuthnCheck(operationContext, authenticationInfo);
			}
			return authenticationData.IsAuthorized;
		}

		private IAuthenticationInfo GetPrincipal(OperationContext operationContext)
		{
			MRSProxyAuthorizationManager.AuthenticationData authenticationData = this.GetAuthenticationData(operationContext);
			if (authenticationData.AuthenticationInfo != null)
			{
				return authenticationData.AuthenticationInfo;
			}
			IAuthenticationInfo authenticationInfo = base.Authenticate(operationContext);
			if (authenticationInfo == null)
			{
				return null;
			}
			if (operationContext.Channel.LocalAddress.Uri.Scheme == "net.tcp" || this.TestIntegration.UseHttpsForLocalMoves)
			{
				return authenticationInfo;
			}
			WindowsPrincipal windowsPrincipal = authenticationInfo.WindowsPrincipal;
			WindowsIdentity windowsIdentity = windowsPrincipal.Identity as WindowsIdentity;
			using (ClientSecurityContext clientSecurityContext = new ClientSecurityContext(windowsIdentity))
			{
				if (!LocalServer.AllowsTokenSerializationBy(clientSecurityContext))
				{
					MrsTracer.ProxyService.Debug("MRSProxyAuthorizationManager: User {0} does not have the permission to serialize security token.", new object[]
					{
						authenticationInfo.PrincipalName
					});
					return null;
				}
			}
			object obj;
			if (!OperationContext.Current.IncomingMessageProperties.TryGetValue(HttpRequestMessageProperty.Name, out obj))
			{
				return null;
			}
			HttpRequestMessageProperty httpRequestMessageProperty = obj as HttpRequestMessageProperty;
			if (httpRequestMessageProperty == null)
			{
				return null;
			}
			string[] values = httpRequestMessageProperty.Headers.GetValues("X-CommonAccessToken");
			if (values == null || values.Length != 1)
			{
				return null;
			}
			string text = values[0];
			if (string.IsNullOrEmpty(text))
			{
				return null;
			}
			using (ClientSecurityContext clientSecurityContext2 = new ClientSecurityContext(windowsIdentity))
			{
				if (!LocalServer.AllowsTokenSerializationBy(clientSecurityContext2))
				{
					MrsTracer.ProxyService.Debug("MRSProxyAuthorizationManager: User {0} does not have the permission to serialize security token.", new object[]
					{
						windowsIdentity
					});
					return null;
				}
			}
			CommonAccessToken commonAccessToken = CommonAccessToken.Deserialize(text);
			SecurityIdentifier securityIdentifier = new SecurityIdentifier(commonAccessToken.WindowsAccessToken.UserSid);
			IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.PartiallyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 300, "GetPrincipal", "f:\\15.00.1497\\sources\\dev\\mrs\\src\\ProxyService\\MRSProxyAuthorizationManager.cs");
			ADRawEntry adrawEntry = rootOrganizationRecipientSession.FindADRawEntryBySid(securityIdentifier, MRSProxyAuthorizationManager.userPrincipalName);
			if (adrawEntry == null)
			{
				authenticationData.AuthenticationInfo = new AuthenticationInfo(securityIdentifier);
			}
			else
			{
				string sUserPrincipalName = (string)adrawEntry[ADUserSchema.UserPrincipalName];
				windowsIdentity = new WindowsIdentity(sUserPrincipalName);
				authenticationData.AuthenticationInfo = new AuthenticationInfo(windowsIdentity, windowsIdentity.Name);
			}
			return authenticationData.AuthenticationInfo;
		}

		private T GetFromHeaders<T>(OperationContext operationContext, string headerName)
		{
			int num = operationContext.IncomingMessageHeaders.FindHeader(headerName, "http://schemas.microsoft.com/exchange/services/2006/types");
			if (num != -1)
			{
				return operationContext.IncomingMessageHeaders.GetHeader<T>(num);
			}
			return default(T);
		}

		private MRSProxyAuthorizationManager.AuthenticationData GetAuthenticationData(OperationContext operationContext)
		{
			MRSProxyAuthorizationManager.AuthenticationData authenticationData = null;
			if (!MRSProxyAuthorizationManager.Sessions.TryGetValue(operationContext.SessionId, out authenticationData))
			{
				authenticationData = new MRSProxyAuthorizationManager.AuthenticationData();
				MRSProxyAuthorizationManager.Sessions.AddAbsolute(operationContext.SessionId, authenticationData, MRSProxyAuthorizationManager.authenticationDataExpiration, null);
			}
			return authenticationData;
		}

		private SecurityIdentifier[] GetAdditionalSids()
		{
			string config = ConfigBase<MRSConfigSchema>.GetConfig<string>("AdditionalSidsForMrsProxyAuthorization");
			if (string.IsNullOrWhiteSpace(config))
			{
				return Array<SecurityIdentifier>.Empty;
			}
			string[] array = config.Split(new char[]
			{
				','
			});
			List<SecurityIdentifier> list = new List<SecurityIdentifier>(array.Length);
			foreach (string text in array)
			{
				if (!string.IsNullOrWhiteSpace(text))
				{
					try
					{
						list.Add(new SecurityIdentifier(text));
					}
					catch (ArgumentException ex)
					{
						CommonUtils.LogEvent(MRSEventLogConstants.Tuple_ServiceConfigCorrupt, new object[]
						{
							CommonUtils.FullExceptionMessage(ex)
						});
						MrsTracer.ProxyService.Debug("MRSProxyAuthorizationManager.GetAdditionalSids: Cannot parse SID '{0}'. Skipping.", new object[]
						{
							text
						});
					}
				}
			}
			if (list.Count == 0)
			{
				return Array<SecurityIdentifier>.Empty;
			}
			return list.ToArray();
		}

		private bool IsCertificateAuthentication(OperationContext operationContext)
		{
			if (!ConfigBase<MRSConfigSchema>.GetConfig<bool>("ProxyServiceCertificateEndpointEnabled"))
			{
				return false;
			}
			ServiceSecurityContext serviceSecurityContext = operationContext.ServiceSecurityContext;
			if (serviceSecurityContext == null || serviceSecurityContext.AuthorizationContext == null || serviceSecurityContext.AuthorizationContext.ClaimSets == null || serviceSecurityContext.AuthorizationContext.ClaimSets.Count != 1)
			{
				return false;
			}
			X509CertificateClaimSet x509CertificateClaimSet = serviceSecurityContext.AuthorizationContext.ClaimSets[0] as X509CertificateClaimSet;
			return x509CertificateClaimSet != null && !(x509CertificateClaimSet.X509Certificate.Subject != ConfigBase<MRSConfigSchema>.GetConfig<string>("ProxyClientCertificateSubject"));
		}

		internal const string MailboxGuidKey = "MRSProxy.MailboxGuid";

		private static readonly TimeoutCache<string, MRSProxyAuthorizationManager.AuthenticationData> Sessions = new TimeoutCache<string, MRSProxyAuthorizationManager.AuthenticationData>(16, 1024, false);

		private static TimeSpan authenticationDataExpiration = TimeSpan.FromHours(1.0);

		private static PropertyDefinition[] userPrincipalName = new PropertyDefinition[]
		{
			ADUserSchema.UserPrincipalName
		};

		private TestIntegration testIntegration;

		private class AuthenticationData
		{
			public bool IsAuthorized { get; set; }

			public IAuthenticationInfo AuthenticationInfo { get; set; }
		}
	}
}
