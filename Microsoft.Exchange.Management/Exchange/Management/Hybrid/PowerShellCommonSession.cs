using System;
using System.Collections.Generic;
using System.Linq;
using System.Management.Automation;
using System.Threading;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Hybrid.Entity;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal abstract class PowerShellCommonSession : ICommonSession, IDisposable
	{
		private protected RemotePowershellSession RemotePowershellSession { protected get; private set; }

		public event Func<string, bool> ShouldInvokePowershellCommand;

		public PowerShellCommonSession(ILogger logger, string targetServer, PowershellConnectionType connectionType, PSCredential credentials)
		{
			this.RemotePowershellSession = new RemotePowershellSession(logger, targetServer, connectionType, true, new Func<string, bool>(this.ShouldInvokePowershellCommandHandler));
			this.RemotePowershellSession.Connect(credentials, Thread.CurrentThread.CurrentUICulture);
		}

		public IEnumerable<IAcceptedDomain> GetAcceptedDomain()
		{
			return (from a in this.RemotePowershellSession.RunOneCommand<Microsoft.Exchange.Data.Directory.SystemConfiguration.AcceptedDomain>("Get-AcceptedDomain", null, true)
			select new Microsoft.Exchange.Management.Hybrid.Entity.AcceptedDomain(a)).ToList<Microsoft.Exchange.Management.Hybrid.Entity.AcceptedDomain>();
		}

		public IFederatedOrganizationIdentifier GetFederatedOrganizationIdentifier()
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.SetNull<bool>("IncludeExtendedDomainInfo");
			FederatedOrganizationIdWithDomainStatus federatedOrganizationIdWithDomainStatus = this.RemotePowershellSession.RunOneCommandSingleResult<FederatedOrganizationIdWithDomainStatus>("Get-FederatedOrganizationIdentifier", sessionParameters, true);
			if (federatedOrganizationIdWithDomainStatus != null)
			{
				return new FederatedOrganizationIdentifier
				{
					Enabled = federatedOrganizationIdWithDomainStatus.Enabled,
					Domains = federatedOrganizationIdWithDomainStatus.Domains,
					AccountNamespace = federatedOrganizationIdWithDomainStatus.AccountNamespace,
					DelegationTrustLink = federatedOrganizationIdWithDomainStatus.DelegationTrustLink,
					DefaultDomain = federatedOrganizationIdWithDomainStatus.DefaultDomain
				};
			}
			return null;
		}

		public IFederationTrust GetFederationTrust(ObjectId identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity.ToString());
			Microsoft.Exchange.Data.Directory.SystemConfiguration.FederationTrust federationTrust = this.RemotePowershellSession.RunOneCommandSingleResult<Microsoft.Exchange.Data.Directory.SystemConfiguration.FederationTrust>("Get-FederationTrust", sessionParameters, false);
			if (federationTrust != null)
			{
				return new Microsoft.Exchange.Management.Hybrid.Entity.FederationTrust
				{
					Name = federationTrust.Name,
					TokenIssuerUri = federationTrust.TokenIssuerUri
				};
			}
			return null;
		}

		public IFederationInformation GetFederationInformation(string domainName)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("DomainName", domainName);
			sessionParameters.Set("BypassAdditionalDomainValidation", true);
			Dictionary<string, object> dictionary = null;
			try
			{
				dictionary = this.RemotePowershellSession.GetPowershellUntypedObjectAsMembers("Get-FederationInformation", null, sessionParameters);
			}
			catch
			{
			}
			if (dictionary != null)
			{
				FederationInformation federationInformation = new FederationInformation();
				object value;
				if (dictionary.TryGetValue("TargetAutodiscoverEpr", out value))
				{
					federationInformation.TargetAutodiscoverEpr = TaskCommon.ToStringOrNull(value);
				}
				if (dictionary.TryGetValue("TargetApplicationUri", out value))
				{
					federationInformation.TargetApplicationUri = TaskCommon.ToStringOrNull(value);
				}
				return federationInformation;
			}
			return null;
		}

		public void RemoveOrganizationRelationship(string identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Identity", identity);
			sessionParameters.Set("Confirm", false);
			this.RemotePowershellSession.RunCommand("Remove-OrganizationRelationship", sessionParameters);
		}

		public void NewOrganizationRelationship(string name, string targetApplicationUri, string targetAutodiscoverEpr, IEnumerable<SmtpDomain> domainNames)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Name", name);
			sessionParameters.Set("TargetApplicationUri", targetApplicationUri);
			sessionParameters.Set("TargetAutodiscoverEpr", targetAutodiscoverEpr);
			sessionParameters.Set("Enabled", true);
			sessionParameters.Set<SmtpDomain>("DomainNames", domainNames, (SmtpDomain d) => d.Domain);
			this.RemotePowershellSession.RunCommand("New-OrganizationRelationship", sessionParameters);
		}

		public IEnumerable<OrganizationRelationship> GetOrganizationRelationship()
		{
			return this.RemotePowershellSession.RunOneCommand<OrganizationRelationship>("Get-OrganizationRelationship", null, true);
		}

		public IEnumerable<DomainContentConfig> GetRemoteDomain()
		{
			return this.RemotePowershellSession.RunOneCommand<DomainContentConfig>("Get-RemoteDomain", null, true);
		}

		public void RemoveRemoteDomain(ObjectId identity)
		{
			SessionParameters sessionParameters = new SessionParameters();
			sessionParameters.Set("Confirm", false);
			sessionParameters.Set("Identity", identity.ToString());
			this.RemotePowershellSession.RunOneCommand("Remove-RemoteDomain", sessionParameters, false);
		}

		public void SetOrganizationRelationship(ObjectId identity, SessionParameters parameters)
		{
			parameters.Set("Identity", identity.ToString());
			this.RemotePowershellSession.RunCommand("Set-OrganizationRelationship", parameters);
		}

		public IOrganizationConfig GetOrganizationConfig()
		{
			Microsoft.Exchange.Data.Directory.Management.OrganizationConfig organizationConfig = this.RemotePowershellSession.RunOneCommandSingleResult<Microsoft.Exchange.Data.Directory.Management.OrganizationConfig>("Get-OrganizationConfig", null, false);
			if (organizationConfig != null)
			{
				return new Microsoft.Exchange.Management.Hybrid.Entity.OrganizationConfig
				{
					Name = organizationConfig.Name,
					Guid = organizationConfig.Guid,
					AdminDisplayVersion = organizationConfig.AdminDisplayVersion,
					IsUpgradingOrganization = organizationConfig.IsUpgradingOrganization,
					OrganizationConfigHash = organizationConfig.OrganizationConfigHash,
					IsDehydrated = organizationConfig.IsDehydrated
				};
			}
			return null;
		}

		public IFederationInformation BuildFederationInformation(string targetApplicationUri, string targetAutodiscoverEpr)
		{
			return new FederationInformation
			{
				TargetApplicationUri = targetApplicationUri,
				TargetAutodiscoverEpr = targetAutodiscoverEpr
			};
		}

		public void Dispose()
		{
			if (this.RemotePowershellSession != null)
			{
				this.RemotePowershellSession.Dispose();
				this.RemotePowershellSession = null;
			}
		}

		private bool ShouldInvokePowershellCommandHandler(string command)
		{
			return this.ShouldInvokePowershellCommand == null || this.ShouldInvokePowershellCommand(command);
		}

		protected const string Set_FederatedOrganizationIdentifier = "Set-FederatedOrganizationIdentifier";

		protected const string Get_FederationTrust = "Get-FederationTrust";

		private const string Get_AcceptedDomain = "Get-AcceptedDomain";

		private const string Get_FederatedOrganizationIdentifier = "Get-FederatedOrganizationIdentifier";

		private const string Get_FederationInformation = "Get-FederationInformation";

		private const string Remove_OrganizationRelationship = "Remove-OrganizationRelationship";

		private const string New_OrganizationRelationship = "New-OrganizationRelationship";

		private const string Get_OrganizationRelationship = "Get-OrganizationRelationship";

		private const string Get_OrganizationConfig = "Get-OrganizationConfig";

		private const string Get_RemoteDomain = "Get-RemoteDomain";

		private const string Remove_RemoteDomain = "Remove-RemoteDomain";

		private const string Set_OrganizationRelationship = "Set-OrganizationRelationship";
	}
}
