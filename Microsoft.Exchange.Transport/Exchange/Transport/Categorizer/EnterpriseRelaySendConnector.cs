using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Transport.Categorizer
{
	internal class EnterpriseRelaySendConnector : SmtpSendConnectorConfig
	{
		public EnterpriseRelaySendConnector(Server localServerConfig, ADObjectId localRoutingGroupId, bool disableExchangeServerAuth)
		{
			if (localServerConfig == null)
			{
				throw new ArgumentNullException("localServerConfig");
			}
			if (localRoutingGroupId == null)
			{
				throw new ArgumentNullException("localRoutingGroupId");
			}
			ADObjectId childId = localRoutingGroupId.GetChildId("Connections").GetChildId(Strings.IntraorgSendConnectorName);
			base.SetId(childId);
			base.Name = Strings.IntraorgSendConnectorName;
			base.CloudServicesMailEnabled = true;
			string text = localServerConfig.Fqdn;
			if (string.IsNullOrEmpty(text))
			{
				text = "local";
			}
			base.Fqdn = new Fqdn(text);
			base.SmtpMaxMessagesPerConnection = localServerConfig.IntraOrgConnectorSmtpMaxMessagesPerConnection;
			base.ProtocolLoggingLevel = localServerConfig.IntraOrgConnectorProtocolLoggingLevel;
			if (disableExchangeServerAuth)
			{
				base.SmartHostAuthMechanism = SmtpSendConnectorConfig.AuthMechanisms.None;
			}
			else
			{
				base.SmartHostAuthMechanism = SmtpSendConnectorConfig.AuthMechanisms.ExchangeServer;
			}
			if (EnterpriseRelaySendConnector.SecurityDescriptor == null)
			{
				try
				{
					EnterpriseRelaySendConnector.securityDescriptor = EnterpriseRelaySendConnector.CreateSecurityDescriptor(disableExchangeServerAuth);
				}
				catch (ErrorExchangeGroupNotFoundException inner)
				{
					throw new TransportComponentLoadFailedException(Strings.ReadingADConfigFailed, inner);
				}
				catch (ADTransientException inner2)
				{
					throw new TransportComponentLoadFailedException(Strings.ReadingADConfigFailed, inner2);
				}
			}
			this.PopulateCalculatedProperties();
		}

		public static RawSecurityDescriptor SecurityDescriptor
		{
			get
			{
				return EnterpriseRelaySendConnector.securityDescriptor;
			}
		}

		internal override RawSecurityDescriptor GetSecurityDescriptor()
		{
			return EnterpriseRelaySendConnector.SecurityDescriptor;
		}

		private static RawSecurityDescriptor CreateSecurityDescriptor(bool disableExchangeServerAuth)
		{
			PrincipalPermissionList permissions = EnterpriseRelaySendConnector.GetPermissions(disableExchangeServerAuth);
			SecurityIdentifier principal = permissions[0].Principal;
			SecurityIdentifier group = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, principal.AccountDomainSid);
			return permissions.CreateExtendedRightsSecurityDescriptor(principal, group);
		}

		private static PrincipalPermissionList GetPermissions(bool disableExchangeServerAuth)
		{
			IConfigurationSession configSession = null;
			IRecipientSession recipSession = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				configSession = DirectorySessionFactory.Default.GetTenantOrTopologyConfigurationSession(ConsistencyMode.IgnoreInvalid, ADSessionSettings.FromRootOrgScopeSet(), 223, "GetPermissions", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Categorizer\\Routing\\EnterpriseRelaySendConnector.cs");
				configSession.UseConfigNC = false;
				recipSession = DirectorySessionFactory.Default.GetTenantOrRootOrgRecipientSession(ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 229, "GetPermissions", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\Categorizer\\Routing\\EnterpriseRelaySendConnector.cs");
			}, 0);
			PrincipalPermissionList sids = new PrincipalPermissionList(5);
			sids.Add(WellKnownSids.HubTransportServers, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SMTPSendXShadow);
			sids.Add(WellKnownSids.EdgeTransportServers, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SMTPSendXShadow);
			sids.Add(WellKnownSids.LegacyExchangeServers, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders);
			ADNotificationAdapter.RunADOperation(delegate()
			{
				SecurityIdentifier sidForExchangeKnownGuid = ReceiveConnector.PermissionGroupPermissions.GetSidForExchangeKnownGuid(recipSession, WellKnownGuid.ExSWkGuid, configSession.ConfigurationNamingContext.DistinguishedName);
				sids.Add(sidForExchangeKnownGuid, Permission.SMTPSendEXCH50 | Permission.SendRoutingHeaders | Permission.SendForestHeaders | Permission.SendOrganizationHeaders | Permission.SMTPSendXShadow);
			});
			if (disableExchangeServerAuth)
			{
				sids.Add(SmtpSendConnectorConfig.AnonymousSecurityIdentifier, Permission.SMTPSendXShadow);
			}
			return sids;
		}

		private void PopulateCalculatedProperties()
		{
			foreach (PropertyDefinition propertyDefinition in this.Schema.AllProperties)
			{
				ADPropertyDefinition adpropertyDefinition = (ADPropertyDefinition)propertyDefinition;
				if (adpropertyDefinition.IsCalculated && !base.ExchangeVersion.IsOlderThan(adpropertyDefinition.VersionAdded))
				{
					object obj = this.propertyBag[adpropertyDefinition];
				}
			}
		}

		private static RawSecurityDescriptor securityDescriptor;
	}
}
