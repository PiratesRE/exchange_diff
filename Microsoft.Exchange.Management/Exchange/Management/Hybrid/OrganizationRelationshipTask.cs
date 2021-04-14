using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class OrganizationRelationshipTask : SessionTask
	{
		public OrganizationRelationshipTask() : base(HybridStrings.OrganizationRelationshipTaskName, 2)
		{
			this.addOnPremisesFedDomains = new List<string>();
		}

		private SmtpDomain AutoDiscoverHybridDomain
		{
			get
			{
				return (from d in this.HybridDomains
				where d.AutoDiscover
				select d).FirstOrDefault<AutoDiscoverSmtpDomain>();
			}
		}

		private SmtpDomain PreferredHybridDomain
		{
			get
			{
				AutoDiscoverSmtpDomain autoDiscoverSmtpDomain = (from d in this.HybridDomains
				where d.AutoDiscover
				select d).FirstOrDefault<AutoDiscoverSmtpDomain>();
				if (autoDiscoverSmtpDomain != null)
				{
					return autoDiscoverSmtpDomain;
				}
				return this.HybridDomains.First<AutoDiscoverSmtpDomain>();
			}
		}

		private IEnumerable<AutoDiscoverSmtpDomain> HybridDomains
		{
			get
			{
				return from d in base.TaskContext.HybridConfigurationObject.Domains
				orderby d.Domain
				select d;
			}
		}

		private string TenantCoexistenceDomain
		{
			get
			{
				return base.TaskContext.Parameters.Get<string>("_hybridDomain");
			}
		}

		private IFederationInformation TenantFederationInfo
		{
			get
			{
				if (this.RequiresFederationTrust() && this.tenantFederationInfo == null)
				{
					this.tenantFederationInfo = base.TenantSession.GetFederationInformation(this.PreferredHybridDomain.Domain);
					if (this.tenantFederationInfo == null)
					{
						this.tenantFederationInfo = base.TenantSession.GetFederationInformation(this.PreferredHybridDomain.Domain);
						if (this.tenantFederationInfo == null)
						{
							string text = string.Format("https://autodiscover.{0}/autodiscover/autodiscover.svc/WSSecurity", this.PreferredHybridDomain.Domain);
							string text2 = null;
							IFederatedOrganizationIdentifier federatedOrganizationIdentifier = base.OnPremisesSession.GetFederatedOrganizationIdentifier();
							if (federatedOrganizationIdentifier != null && federatedOrganizationIdentifier.AccountNamespace != null && !string.IsNullOrEmpty(federatedOrganizationIdentifier.AccountNamespace.Domain) && federatedOrganizationIdentifier.Domains != null)
							{
								text2 = federatedOrganizationIdentifier.AccountNamespace.Domain;
							}
							if (text2 != null && text != null)
							{
								this.tenantFederationInfo = base.OnPremisesSession.BuildFederationInformation(text2, text);
								base.AddLocalizedStringWarning(HybridStrings.WarningTenantGetFedInfoFailed(this.tenantFederationInfo.TargetAutodiscoverEpr));
							}
						}
					}
					if (this.tenantFederationInfo == null)
					{
						throw new LocalizedException(HybridStrings.ErrorFederationInfoNotFound(this.PreferredHybridDomain.Domain));
					}
				}
				return this.tenantFederationInfo;
			}
		}

		private IFederationInformation OnpremisesFederationInfo
		{
			get
			{
				if (this.RequiresFederationTrust() && this.onpremisesFederationInfo == null)
				{
					this.onpremisesFederationInfo = base.OnPremisesSession.GetFederationInformation(this.TenantCoexistenceDomain);
					if (this.onpremisesFederationInfo == null)
					{
						throw new LocalizedException(HybridStrings.WarningUnableToCommunicateWithAutoDiscoveryEP);
					}
				}
				return this.onpremisesFederationInfo;
			}
		}

		private IOrganizationConfig OnPremOrgConfig
		{
			get
			{
				return base.OnPremisesSession.GetOrganizationConfig();
			}
		}

		public override bool CheckPrereqs(ITaskContext taskContext)
		{
			if (!base.CheckPrereqs(taskContext))
			{
				base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.HybridInfoBasePrereqsFailed));
				return false;
			}
			return this.CheckTaskPrerequisites(taskContext);
		}

		public override bool NeedsConfiguration(ITaskContext taskContext)
		{
			bool flag = base.NeedsConfiguration(taskContext);
			OrganizationRelationship existingOrgRel = taskContext.Parameters.Get<OrganizationRelationship>("_onPremOrgRel");
			OrganizationRelationship existingOrgRel2 = taskContext.Parameters.Get<OrganizationRelationship>("_tenantOrgRel");
			return flag || this.NeedProvisionOrganizationRelationship(base.OnPremisesSession, existingOrgRel, this.OnpremisesFederationInfo, new SmtpDomain[]
			{
				new SmtpDomain(this.TenantCoexistenceDomain)
			}, TaskCommon.GetOnPremOrgRelationshipName(this.OnPremOrgConfig)) || this.NeedProvisionOrganizationRelationship(base.TenantSession, existingOrgRel2, this.TenantFederationInfo, this.HybridDomains, TaskCommon.GetTenantOrgRelationshipName(this.OnPremOrgConfig)) || (this.RequiresFederationTrust() && (this.updateOnPremisesFedOrgId || this.updateTenantFedOrgId || this.addOnPremisesFedDomains.Count > 0));
		}

		public override bool Configure(ITaskContext taskContext)
		{
			return base.Configure(taskContext) && this.TaskConfigure(taskContext);
		}

		public override bool ValidateConfiguration(ITaskContext taskContext)
		{
			return base.ValidateConfiguration(taskContext) && this.CheckTaskPrerequisites(taskContext) && !this.NeedsConfiguration(taskContext);
		}

		private static bool VerifyAcceptedTokenIssuerUri(ICommonSession session, List<Uri> acceptedTokenIssuerUris)
		{
			IFederatedOrganizationIdentifier federatedOrganizationIdentifier = session.GetFederatedOrganizationIdentifier();
			if (federatedOrganizationIdentifier == null)
			{
				throw new LocalizedException(HybridStrings.ErrorFederationIDNotProvisioned);
			}
			IFederationTrust fedTrust = session.GetFederationTrust(federatedOrganizationIdentifier.DelegationTrustLink);
			if (fedTrust == null)
			{
				throw new LocalizedException(Strings.ErrorFederationTrustNotFound(federatedOrganizationIdentifier.DelegationTrustLink.Name));
			}
			return acceptedTokenIssuerUris.Any((Uri u) => u.Equals(fedTrust.TokenIssuerUri));
		}

		private void Reset()
		{
			this.addOnPremisesFedDomains.Clear();
			this.accountNamespace = null;
			this.updateOnPremisesFedOrgId = false;
			this.updateTenantFedOrgId = false;
		}

		private bool CheckTaskPrerequisites(ITaskContext taskContext)
		{
			this.Reset();
			if (this.RequiresFederationTrust())
			{
				string value = Configuration.SignupDomainSuffix(taskContext.HybridConfigurationObject.ServiceInstance);
				foreach (AutoDiscoverSmtpDomain autoDiscoverSmtpDomain in this.HybridDomains)
				{
					if (!autoDiscoverSmtpDomain.Domain.EndsWith(value, StringComparison.InvariantCultureIgnoreCase) && autoDiscoverSmtpDomain.Domain.Length <= 32)
					{
						this.accountNamespace = autoDiscoverSmtpDomain.Domain;
						break;
					}
				}
				if (string.IsNullOrEmpty(this.accountNamespace))
				{
					base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.ErrorAccountNamespace));
					base.AddLocalizedStringError(HybridStrings.ErrorAccountNamespace);
					return false;
				}
				int num = base.OnPremisesSession.GetFederationTrust().Count<IFederationTrust>();
				if (num == 0)
				{
					base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, Strings.ErrorFederationTrustNotFound("")));
					base.AddLocalizedStringError(HybridStrings.ErrorNoFederationTrustFound);
					return false;
				}
				if (num > 1)
				{
					base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.ErrorMultipleFederationTrusts));
					base.AddLocalizedStringError(HybridStrings.ErrorMultipleFederationTrusts);
					return false;
				}
				IFederatedOrganizationIdentifier federatedOrganizationIdentifier = base.OnPremisesSession.GetFederatedOrganizationIdentifier();
				if (federatedOrganizationIdentifier == null)
				{
					this.updateOnPremisesFedOrgId = true;
				}
				else if (federatedOrganizationIdentifier.AccountNamespace == null || string.IsNullOrEmpty(federatedOrganizationIdentifier.AccountNamespace.Domain))
				{
					this.updateOnPremisesFedOrgId = true;
				}
				else
				{
					if (!federatedOrganizationIdentifier.Enabled)
					{
						base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.ErrorFederatedIdentifierDisabled));
						base.AddLocalizedStringError(HybridStrings.ErrorFederatedIdentifierDisabled);
						return false;
					}
					this.accountNamespace = federatedOrganizationIdentifier.AccountNamespace.Domain.Replace(FederatedOrganizationId.HybridConfigurationWellKnownSubDomain + ".", string.Empty);
					if (!string.Equals(TaskCommon.ToStringOrNull(federatedOrganizationIdentifier.DelegationTrustLink), Configuration.FederatedTrustIdentity))
					{
						this.updateOnPremisesFedOrgId = true;
					}
					else
					{
						this.updateOnPremisesFedOrgId = false;
						using (IEnumerator<string> enumerator2 = (from d in this.HybridDomains
						select d.Domain).GetEnumerator())
						{
							while (enumerator2.MoveNext())
							{
								string hybridDomain = enumerator2.Current;
								if (!federatedOrganizationIdentifier.Domains.Any((FederatedDomain d) => string.Equals(d.Domain.Domain, hybridDomain, StringComparison.InvariantCultureIgnoreCase)))
								{
									this.addOnPremisesFedDomains.Add(hybridDomain);
								}
							}
						}
					}
					if (!TaskCommon.AreEqual(federatedOrganizationIdentifier.DefaultDomain, this.AutoDiscoverHybridDomain))
					{
						this.updateOnPremisesFedOrgId = true;
					}
				}
				if (this.updateOnPremisesFedOrgId)
				{
					foreach (string text in from d in this.HybridDomains
					select d.Domain)
					{
						if (!text.Equals(this.accountNamespace, StringComparison.InvariantCultureIgnoreCase))
						{
							this.addOnPremisesFedDomains.Add(text);
						}
					}
				}
				IFederatedOrganizationIdentifier federatedOrganizationIdentifier2 = base.TenantSession.GetFederatedOrganizationIdentifier();
				string text2 = (federatedOrganizationIdentifier2 != null && federatedOrganizationIdentifier2.DefaultDomain != null) ? federatedOrganizationIdentifier2.DefaultDomain.ToString() : null;
				if (text2 == null || !text2.Equals(this.TenantCoexistenceDomain, StringComparison.InvariantCultureIgnoreCase))
				{
					this.updateTenantFedOrgId = true;
				}
			}
			return true;
		}

		private bool TaskConfigure(ITaskContext taskContext)
		{
			IOrganizationConfig organizationConfig = taskContext.TenantSession.GetOrganizationConfig();
			if (organizationConfig.IsDehydrated)
			{
				try
				{
					taskContext.TenantSession.EnableOrganizationCustomization();
				}
				catch
				{
				}
			}
			if (this.RequiresFederationTrust())
			{
				if (this.updateOnPremisesFedOrgId)
				{
					IFederatedOrganizationIdentifier federatedOrganizationIdentifier = base.OnPremisesSession.GetFederatedOrganizationIdentifier();
					string text = (federatedOrganizationIdentifier != null && federatedOrganizationIdentifier.DelegationTrustLink != null) ? federatedOrganizationIdentifier.DelegationTrustLink.ToString() : Configuration.FederatedTrustIdentity;
					taskContext.OnPremisesSession.SetFederationTrustRefreshMetadata(text);
					SmtpDomain autoDiscoverHybridDomain = this.AutoDiscoverHybridDomain;
					string defaultDomain = (autoDiscoverHybridDomain != null && autoDiscoverHybridDomain.Domain != null) ? autoDiscoverHybridDomain.Domain : null;
					taskContext.OnPremisesSession.SetFederatedOrganizationIdentifier(this.accountNamespace, text, defaultDomain);
				}
				List<Uri> acceptedTokenIssuerUris = taskContext.Parameters.Get<List<Uri>>("_onPremAcceptedTokenIssuerUris");
				if (!OrganizationRelationshipTask.VerifyAcceptedTokenIssuerUri(base.OnPremisesSession, acceptedTokenIssuerUris))
				{
					throw new LocalizedException(HybridStrings.ErrorOnPremUsingConsumerLiveID);
				}
				acceptedTokenIssuerUris = taskContext.Parameters.Get<List<Uri>>("_tenantAcceptedTokenIssuerUris");
				if (!OrganizationRelationshipTask.VerifyAcceptedTokenIssuerUri(base.TenantSession, acceptedTokenIssuerUris))
				{
					throw new LocalizedException(HybridStrings.ErrorTenantUsingConsumerLiveID);
				}
				if (this.updateTenantFedOrgId)
				{
					base.TenantSession.SetFederatedOrganizationIdentifier(this.TenantCoexistenceDomain);
				}
				foreach (string domainName in this.addOnPremisesFedDomains)
				{
					taskContext.OnPremisesSession.AddFederatedDomain(domainName);
				}
			}
			OrganizationRelationship value = OrganizationRelationshipTask.ProvisionOrganizationRelationship(base.OnPremisesSession, taskContext.Parameters.Get<OrganizationRelationship>("_onPremOrgRel"), this.OnpremisesFederationInfo, new SmtpDomain[]
			{
				new SmtpDomain(this.TenantCoexistenceDomain)
			}, TaskCommon.GetOnPremOrgRelationshipName(this.OnPremOrgConfig));
			taskContext.Parameters.Set<OrganizationRelationship>("_onPremOrgRel", value);
			value = OrganizationRelationshipTask.ProvisionOrganizationRelationship(base.TenantSession, taskContext.Parameters.Get<OrganizationRelationship>("_tenantOrgRel"), this.TenantFederationInfo, this.HybridDomains, TaskCommon.GetTenantOrgRelationshipName(this.OnPremOrgConfig));
			taskContext.Parameters.Set<OrganizationRelationship>("_tenantOrgRel", value);
			return true;
		}

		private static OrganizationRelationship ProvisionOrganizationRelationship(ICommonSession session, OrganizationRelationship existingOrgRel, IFederationInformation federationInfo, IEnumerable<SmtpDomain> domains, string relationshipName)
		{
			if (existingOrgRel != null)
			{
				session.RemoveOrganizationRelationship(existingOrgRel.Identity.ToString());
			}
			string targetApplicationUri = (federationInfo != null) ? federationInfo.TargetApplicationUri : null;
			string targetAutodiscoverEpr = (federationInfo != null) ? federationInfo.TargetAutodiscoverEpr : null;
			session.NewOrganizationRelationship(relationshipName, targetApplicationUri, targetAutodiscoverEpr, domains);
			existingOrgRel = TaskCommon.GetOrganizationRelationship(session, relationshipName, from d in domains
			select d.Domain);
			if (existingOrgRel == null)
			{
				throw new LocalizedException(HybridStrings.ErrorOrgRelProvisionFailed(domains.First<SmtpDomain>().ToString()));
			}
			return existingOrgRel;
		}

		private bool NeedProvisionOrganizationRelationship(ICommonSession session, OrganizationRelationship existingOrgRel, IFederationInformation federationInfo, IEnumerable<SmtpDomain> domains, string relationshipName)
		{
			return existingOrgRel == null || !TaskCommon.ContainsSame<SmtpDomain>(existingOrgRel.DomainNames, domains) || (this.RequiresFederationTrust() && (!string.Equals(TaskCommon.ToStringOrNull(existingOrgRel.TargetApplicationUri), federationInfo.TargetApplicationUri, StringComparison.InvariantCultureIgnoreCase) || !string.Equals(TaskCommon.ToStringOrNull(existingOrgRel.TargetAutodiscoverEpr), federationInfo.TargetAutodiscoverEpr, StringComparison.InvariantCultureIgnoreCase)));
		}

		private bool RequiresFederationTrust()
		{
			return Configuration.RequiresFederationTrust(base.TaskContext.HybridConfigurationObject.ServiceInstance);
		}

		private const int MaxAccountNamespaceLength = 32;

		private List<string> addOnPremisesFedDomains;

		private string accountNamespace;

		private bool updateOnPremisesFedOrgId;

		private bool updateTenantFedOrgId;

		private IFederationInformation tenantFederationInfo;

		private IFederationInformation onpremisesFederationInfo;
	}
}
