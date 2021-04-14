using System;
using System.Security.AccessControl;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Security.Authorization;

namespace Microsoft.Exchange.Transport
{
	internal class InMemoryReceiveConnector : ReceiveConnector
	{
		public InMemoryReceiveConnector(string name, Server localServer, bool acceptAnonymousUsers)
		{
			if (string.IsNullOrEmpty(name))
			{
				throw new ArgumentException("name");
			}
			if (localServer == null)
			{
				throw new ArgumentNullException("localServer");
			}
			this.acceptAnonymousUsers = acceptAnonymousUsers;
			base.Name = name;
			this.ApplyLocalServerConfiguration(localServer);
		}

		public void ApplyLocalServerConfiguration(Server localServer)
		{
			ADObjectId childId = localServer.Id.GetChildId(ProtocolsContainer.DefaultName).GetChildId(ReceiveConnector.DefaultName).GetChildId(base.Name);
			base.SetId(childId);
			base.Fqdn = new Fqdn(localServer.Fqdn);
			base.ProtocolLoggingLevel = localServer.InMemoryReceiveConnectorProtocolLoggingLevel;
			base.SmtpUtf8Enabled = localServer.InMemoryReceiveConnectorSmtpUtf8Enabled;
		}

		internal override RawSecurityDescriptor GetSecurityDescriptor()
		{
			if (this.securityDescriptor == null)
			{
				try
				{
					this.securityDescriptor = this.CreateSecurityDescriptor();
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
			return this.securityDescriptor;
		}

		private RawSecurityDescriptor CreateSecurityDescriptor()
		{
			SecurityIdentifier exchangeServersSid = null;
			ADNotificationAdapter.RunADOperation(delegate()
			{
				IRootOrganizationRecipientSession rootOrganizationRecipientSession = DirectorySessionFactory.Default.CreateRootOrgRecipientSession(true, ConsistencyMode.FullyConsistent, ADSessionSettings.FromRootOrgScopeSet(), 101, "CreateSecurityDescriptor", "f:\\15.00.1497\\sources\\dev\\Transport\\src\\transport\\InMemoryReceiveConnector.cs");
				exchangeServersSid = rootOrganizationRecipientSession.GetExchangeServersUsgSid();
			}, 0);
			SecurityIdentifier sid = new SecurityIdentifier(WellKnownSidType.AnonymousSid, null);
			SecurityIdentifier group = new SecurityIdentifier(WellKnownSidType.BuiltinAdministratorsSid, exchangeServersSid.AccountDomainSid);
			PrincipalPermissionList principalPermissionList = new PrincipalPermissionList(this.acceptAnonymousUsers ? 2 : 1);
			principalPermissionList.Add(exchangeServersSid, Permission.SMTPSubmit | Permission.SMTPAcceptAnyRecipient | Permission.SMTPAcceptAuthenticationFlag | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.BypassAntiSpam | Permission.BypassMessageSizeLimit | Permission.SMTPAcceptEXCH50 | Permission.AcceptRoutingHeaders | Permission.AcceptForestHeaders | Permission.AcceptOrganizationHeaders | Permission.SMTPAcceptXShadow | Permission.SMTPAcceptXProxyFrom | Permission.SMTPAcceptXSessionParams | Permission.SMTPAcceptXMessageContextADRecipientCache | Permission.SMTPAcceptXMessageContextExtendedProperties | Permission.SMTPAcceptXMessageContextFastIndex | Permission.SMTPAcceptXAttr | Permission.SMTPAcceptXSysProbe);
			if (this.acceptAnonymousUsers)
			{
				principalPermissionList.Add(sid, Permission.SMTPSubmit | Permission.SMTPAcceptAnySender | Permission.SMTPAcceptAuthoritativeDomainSender | Permission.AcceptRoutingHeaders);
			}
			return principalPermissionList.CreateExtendedRightsSecurityDescriptor(exchangeServersSid, group);
		}

		protected void TouchCalculatedProperties()
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

		private RawSecurityDescriptor securityDescriptor;

		private readonly bool acceptAnonymousUsers;
	}
}
