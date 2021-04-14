using System;
using System.Globalization;
using System.Security.Principal;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Data.Storage;
using Microsoft.Exchange.Diagnostics;
using Microsoft.Exchange.Management.ControlPanel.WebControls;
using Microsoft.Exchange.Security.Authentication;
using Microsoft.Exchange.Security.Authorization;
using Microsoft.Mapi;

namespace Microsoft.Exchange.Management.ControlPanel
{
	internal class EcpIdentity
	{
		public EcpIdentity(IIdentity logonUserIdentity, string cacheKeySuffix) : this(logonUserIdentity, cacheKeySuffix, false)
		{
			this.accessedUserIdentity = this.LogonUserIdentity;
			this.accessedUserSid = this.logonUserSid;
			this.UserName = logonUserIdentity.GetSafeName(true);
		}

		public EcpIdentity(EcpLogonInformation identity, string cacheKeySuffix) : this(identity.LogonUser, cacheKeySuffix, identity.Impersonated)
		{
			this.logonUserSid = identity.LogonMailboxSid;
			if (identity.Impersonated)
			{
				this.accessedUserIdentity = identity.ImpersonatedUser;
				this.accessedUserSid = identity.ImpersonatedUser.GetSecurityIdentifier();
			}
			else
			{
				this.accessedUserIdentity = this.LogonUserIdentity;
				this.accessedUserSid = this.logonUserSid;
			}
			this.UserName = identity.Name;
		}

		public EcpIdentity(IPrincipal logonUserPrincipal, string explicitUserSmtpAddress, string cacheKeySuffix) : this(logonUserPrincipal.Identity, cacheKeySuffix, true)
		{
			if (string.IsNullOrEmpty(explicitUserSmtpAddress))
			{
				throw new ArgumentException("ExplicitUserSmtpAddress cannot be null or empty", explicitUserSmtpAddress);
			}
			this.logonUserPrincipal = logonUserPrincipal;
			this.accessedUserSmtpAddress = explicitUserSmtpAddress;
			this.UserName = Strings.OnbehalfOf(this.LogonUserIdentity.GetSafeName(true), explicitUserSmtpAddress);
		}

		private EcpIdentity(IIdentity logonUserIdentity, string cacheKeySuffix, bool isExplicitSignon)
		{
			this.IsExplicitSignon = isExplicitSignon;
			this.identityResolved = !isExplicitSignon;
			this.cacheKeySuffix = cacheKeySuffix;
			this.LogonUserIdentity = logonUserIdentity;
			if (logonUserIdentity.AuthenticationType != DelegatedPrincipal.DelegatedAuthenticationType && !DatacenterRegistry.IsForefrontForOffice())
			{
				this.logonUserSid = logonUserIdentity.GetSecurityIdentifier();
			}
		}

		public IIdentity AccessedUserIdentity
		{
			get
			{
				if (!this.identityResolved)
				{
					this.ResolveAccessedUser();
				}
				return this.accessedUserIdentity;
			}
		}

		public SecurityIdentifier AccessedUserSid
		{
			get
			{
				if (!this.identityResolved)
				{
					this.ResolveAccessedUser();
				}
				return this.accessedUserSid;
			}
		}

		public bool HasFullAccess
		{
			get
			{
				if (!this.identityResolved)
				{
					this.ResolveAccessedUser();
				}
				return this.hasFullAccess;
			}
		}

		public string GetCacheKey()
		{
			if (this.cacheKey == null)
			{
				if (this.logonUserSid == null)
				{
					this.cacheKey = this.LogonUserIdentity.Name + this.cacheKeySuffix;
				}
				else
				{
					this.cacheKey = this.logonUserSid.Value + this.cacheKeySuffix;
				}
				if (this.IsExplicitSignon)
				{
					if (string.IsNullOrEmpty(this.accessedUserSmtpAddress))
					{
						this.cacheKey += this.accessedUserSid.Value;
					}
					else
					{
						this.cacheKey += this.accessedUserSmtpAddress;
					}
				}
			}
			return this.cacheKey;
		}

		public ExchangePrincipal GetLogonUserExchangePrincipal()
		{
			if (!this.logonUserResolved)
			{
				try
				{
					if (this.logonUserSid != null)
					{
						this.logonUserExchangePrincipal = ExchangePrincipal.FromUserSid(this.GetOrganizationIdFromIdentity(this.LogonUserIdentity).ToADSessionSettings(), this.logonUserSid);
					}
				}
				catch (ObjectNotFoundException)
				{
				}
				finally
				{
					this.logonUserResolved = true;
				}
			}
			return this.logonUserExchangePrincipal;
		}

		public ExchangePrincipal GetAccessedUserExchangePrincipal()
		{
			if (this.IsExplicitSignon)
			{
				if (!this.identityResolved)
				{
					this.ResolveAccessedUser();
				}
				return this.accessedUserExchangePrincipal;
			}
			return this.GetLogonUserExchangePrincipal();
		}

		private void ResolveAccessedUser()
		{
			if (string.IsNullOrEmpty(this.accessedUserSmtpAddress))
			{
				this.accessedUserExchangePrincipal = ExchangePrincipal.FromUserSid(this.GetOrganizationIdFromIdentity(this.accessedUserIdentity).ToADSessionSettings(), this.accessedUserSid);
				this.logonUserPrincipal = new GenericPrincipal(this.LogonUserIdentity, null);
			}
			else
			{
				OrganizationId organizationId = OrganizationId.ForestWideOrgId;
				SidBasedIdentity sidBasedIdentity = this.LogonUserIdentity as SidBasedIdentity;
				if (sidBasedIdentity != null)
				{
					organizationId = sidBasedIdentity.UserOrganizationId;
				}
				else
				{
					DelegatedPrincipal delegatedPrincipal = this.logonUserPrincipal as DelegatedPrincipal;
					if (delegatedPrincipal != null)
					{
						SmtpDomain domain;
						if (SmtpDomain.TryParse(delegatedPrincipal.DelegatedOrganization, out domain))
						{
							organizationId = DomainCache.Singleton.Get(new SmtpDomainWithSubdomains(domain, false)).OrganizationId;
						}
					}
					else
					{
						ExchangePrincipal exchangePrincipal = this.GetLogonUserExchangePrincipal();
						if (exchangePrincipal != null)
						{
							organizationId = exchangePrincipal.MailboxInfo.OrganizationId;
						}
					}
				}
				ADSessionSettings adSettings = organizationId.ToADSessionSettings();
				string partitionId = null;
				if (organizationId != null && organizationId != OrganizationId.ForestWideOrgId && organizationId.PartitionId != null)
				{
					partitionId = organizationId.PartitionId.ToString();
				}
				this.accessedUserExchangePrincipal = ExchangePrincipal.FromProxyAddress(adSettings, this.accessedUserSmtpAddress, RemotingOptions.AllowCrossSite);
				this.accessedUserIdentity = new GenericSidIdentity(this.accessedUserExchangePrincipal.Sid.Value, this.LogonUserIdentity.AuthenticationType + "-ExplicitSignOn", this.accessedUserExchangePrincipal.Sid, partitionId);
				this.accessedUserSid = this.accessedUserIdentity.GetSecurityIdentifier();
			}
			this.hasFullAccess = this.CanOpenAccessedUserMailbox();
			this.identityResolved = true;
		}

		private static ClientSecurityContext TryMungeTokenFromSlaveAccount(ClientSecurityContext account)
		{
			ClientSecurityContext result;
			try
			{
				result = new SlaveAccountTokenMunger().MungeToken(account, OrganizationId.ForestWideOrgId);
			}
			catch (TokenMungingException)
			{
				result = null;
			}
			return result;
		}

		private bool CanOpenAccessedUserMailbox()
		{
			bool result = false;
			if (this.LogonUserIdentity.AuthenticationType != DelegatedPrincipal.DelegatedAuthenticationType)
			{
				if (this.logonUserSid.Value == this.accessedUserSid.Value)
				{
					result = true;
					this.logonUserEsoSelf = true;
				}
				else
				{
					try
					{
						using (ClientSecurityContext clientSecurityContext = this.logonUserPrincipal.Identity.CreateClientSecurityContext(true))
						{
							using (ClientSecurityContext clientSecurityContext2 = Util.IsDataCenter ? null : EcpIdentity.TryMungeTokenFromSlaveAccount(clientSecurityContext))
							{
								using (MailboxSession.Open(this.accessedUserExchangePrincipal, clientSecurityContext2 ?? clientSecurityContext, CultureInfo.CurrentCulture, "Client=Management;Action=ECP"))
								{
								}
							}
						}
						result = true;
					}
					catch (ConnectionFailedTransientException ex)
					{
						if (!(ex.InnerException is MapiExceptionLogonFailed))
						{
							throw;
						}
					}
					catch (StoragePermanentException)
					{
					}
				}
			}
			return result;
		}

		private OrganizationId GetOrganizationIdFromIdentity(IIdentity identity)
		{
			OrganizationId result = OrganizationId.ForestWideOrgId;
			SidBasedIdentity sidBasedIdentity = identity as SidBasedIdentity;
			if (sidBasedIdentity != null && sidBasedIdentity.UserOrganizationId != null)
			{
				result = sidBasedIdentity.UserOrganizationId;
			}
			return result;
		}

		public bool LogonUserEsoSelf
		{
			get
			{
				if (!this.identityResolved)
				{
					this.ResolveAccessedUser();
				}
				return this.logonUserEsoSelf;
			}
		}

		public readonly bool IsExplicitSignon;

		public readonly string UserName;

		public readonly IIdentity LogonUserIdentity;

		private readonly string cacheKeySuffix;

		private readonly SecurityIdentifier logonUserSid;

		private readonly string accessedUserSmtpAddress;

		private IPrincipal logonUserPrincipal;

		private ExchangePrincipal logonUserExchangePrincipal;

		private IIdentity accessedUserIdentity;

		private SecurityIdentifier accessedUserSid;

		private ExchangePrincipal accessedUserExchangePrincipal;

		private bool identityResolved;

		private bool logonUserResolved;

		private bool hasFullAccess;

		private bool logonUserEsoSelf;

		private string cacheKey;
	}
}
