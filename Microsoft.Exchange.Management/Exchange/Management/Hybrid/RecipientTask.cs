using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Recipient;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class RecipientTask : SessionTask
	{
		public RecipientTask() : base(HybridStrings.RecipientTaskName, 2)
		{
			this.configureRemoteDomain = false;
			this.configureTargetDeliveryDomain = false;
			this.upgradeLegacyDefaultPolicy = false;
			this.targetDeliveryDomain = null;
			this.configureAcceptedDomain = false;
			this.tenantRoutingDomain = null;
			this.policiesToUpdate = null;
		}

		public override bool CheckPrereqs(ITaskContext taskContext)
		{
			if (!base.CheckPrereqs(taskContext))
			{
				base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.HybridInfoBasePrereqsFailed));
				return false;
			}
			this.tenantRoutingDomain = taskContext.Parameters.Get<string>("_hybridDomain");
			if (string.IsNullOrEmpty(this.tenantRoutingDomain))
			{
				throw new LocalizedException(HybridStrings.ErrorNoHybridDomain);
			}
			return true;
		}

		public override bool NeedsConfiguration(ITaskContext taskContext)
		{
			bool flag = base.NeedsConfiguration(taskContext);
			return this.CheckOrVerifyConfiguration(taskContext, false) || flag;
		}

		public override bool Configure(ITaskContext taskContext)
		{
			if (!base.Configure(taskContext))
			{
				return false;
			}
			HybridConfiguration hybridConfigurationObject = taskContext.HybridConfigurationObject;
			if (this.configureRemoteDomain)
			{
				string name = string.Format("Hybrid Domain - {0}", this.tenantRoutingDomain);
				base.OnPremisesSession.NewRemoteDomain(this.tenantRoutingDomain, name);
			}
			if (this.configureRemoteDomain || this.configureTargetDeliveryDomain)
			{
				string identity = string.Format("Hybrid Domain - {0}", this.targetDeliveryDomain);
				SessionParameters sessionParameters = new SessionParameters();
				sessionParameters.Set("TargetDeliveryDomain", true);
				base.OnPremisesSession.SetRemoteDomain(identity, sessionParameters);
			}
			if (this.configureAcceptedDomain)
			{
				base.OnPremisesSession.NewAcceptedDomain(this.tenantRoutingDomain, this.tenantRoutingDomain);
				IEnumerable<IAcceptedDomain> acceptedDomain = base.OnPremisesSession.GetAcceptedDomain();
				if (acceptedDomain.Count<IAcceptedDomain>() == 0)
				{
					throw new LocalizedException(HybridStrings.ErrorNoOnPremAcceptedDomains);
				}
				taskContext.Parameters.Set<IEnumerable<IAcceptedDomain>>("_onPremAcceptedDomains", acceptedDomain);
			}
			if (this.upgradeLegacyDefaultPolicy)
			{
				base.OnPremisesSession.SetEmailAddressPolicy("default policy", "AllRecipients", null);
			}
			if (this.policiesToUpdate != null)
			{
				foreach (EmailAddressPolicy policy in this.policiesToUpdate)
				{
					this.AddTenantRoutingAddressToPolicy(policy);
				}
				foreach (EmailAddressPolicy emailAddressPolicy in this.policiesToUpdate)
				{
					base.OnPremisesSession.UpdateEmailAddressPolicy(emailAddressPolicy.DistinguishedName);
				}
			}
			return true;
		}

		public override bool ValidateConfiguration(ITaskContext taskContext)
		{
			return base.ValidateConfiguration(taskContext) && !this.CheckOrVerifyConfiguration(taskContext, true);
		}

		private bool CheckOrVerifyConfiguration(ITaskContext taskContext, bool verifyOnly)
		{
			this.configureRemoteDomain = false;
			this.configureTargetDeliveryDomain = false;
			HybridConfiguration hybridConfigurationObject = taskContext.HybridConfigurationObject;
			this.targetDeliveryDomain = this.tenantRoutingDomain;
			IEnumerable<DomainContentConfig> remoteDomain = base.OnPremisesSession.GetRemoteDomain();
			if (remoteDomain.Count<DomainContentConfig>() == 0)
			{
				this.configureRemoteDomain = true;
			}
			else
			{
				this.configureRemoteDomain = true;
				foreach (DomainContentConfig domainContentConfig in remoteDomain)
				{
					if (domainContentConfig.DomainName.Domain.Equals(this.tenantRoutingDomain, StringComparison.InvariantCultureIgnoreCase))
					{
						if (!domainContentConfig.TargetDeliveryDomain)
						{
							this.targetDeliveryDomain = domainContentConfig.DomainName.Domain;
							this.configureTargetDeliveryDomain = true;
						}
						this.configureRemoteDomain = false;
						break;
					}
				}
			}
			IEnumerable<IAcceptedDomain> source = taskContext.Parameters.Get<IEnumerable<IAcceptedDomain>>("_onPremAcceptedDomains");
			this.configureAcceptedDomain = !source.Any((IAcceptedDomain d) => d.DomainNameDomain.Equals(this.tenantRoutingDomain, StringComparison.InvariantCultureIgnoreCase));
			IEnumerable<string> source2 = from domain in hybridConfigurationObject.Domains
			select domain.Domain;
			this.policiesToUpdate = this.GetEmailAddressPoliciesToUpdate(taskContext, verifyOnly, source2.ToArray<string>(), out this.upgradeLegacyDefaultPolicy);
			return this.configureRemoteDomain || this.configureTargetDeliveryDomain || this.configureAcceptedDomain || this.policiesToUpdate.Count<EmailAddressPolicy>() > 0 || this.upgradeLegacyDefaultPolicy;
		}

		private IEnumerable<EmailAddressPolicy> GetEmailAddressPoliciesToUpdate(ITaskContext taskContext, bool verifyOnly, IEnumerable<string> hybridDomains, out bool legacyDefaultPolicyNeedsUpgrade)
		{
			List<EmailAddressPolicy> list = new List<EmailAddressPolicy>();
			legacyDefaultPolicyNeedsUpgrade = false;
			IEnumerable<EmailAddressPolicy> emailAddressPolicy = base.OnPremisesSession.GetEmailAddressPolicy();
			foreach (EmailAddressPolicy emailAddressPolicy2 in emailAddressPolicy)
			{
				bool flag = false;
				bool flag2 = false;
				foreach (ProxyAddressTemplate proxyAddressTemplate in emailAddressPolicy2.EnabledEmailAddressTemplates)
				{
					SmtpProxyAddressTemplate smtpProxyAddressTemplate = proxyAddressTemplate as SmtpProxyAddressTemplate;
					if (!(smtpProxyAddressTemplate == null))
					{
						string proxyAddressTemplateString = smtpProxyAddressTemplate.ProxyAddressTemplateString;
						int num = proxyAddressTemplateString.IndexOf("@");
						string text = proxyAddressTemplateString.Substring(num + 1);
						if (text.Equals(this.tenantRoutingDomain, StringComparison.InvariantCultureIgnoreCase))
						{
							flag2 = true;
						}
						else if (smtpProxyAddressTemplate.IsPrimaryAddress && hybridDomains.Contains(text, StringComparer.InvariantCultureIgnoreCase))
						{
							flag = true;
						}
					}
				}
				if (flag && !flag2)
				{
					if (emailAddressPolicy2.RecipientFilterType == WellKnownRecipientFilterType.Legacy)
					{
						if (emailAddressPolicy2.Name.Equals("default policy", StringComparison.InvariantCultureIgnoreCase))
						{
							legacyDefaultPolicyNeedsUpgrade = true;
							list.Add(emailAddressPolicy2);
						}
						else if (!verifyOnly)
						{
							taskContext.Warnings.Add(HybridStrings.WarningHybridLegacyEmailAddressPolicyNotUpgraded(emailAddressPolicy2.Name));
						}
					}
					else
					{
						list.Add(emailAddressPolicy2);
					}
				}
			}
			return list;
		}

		private void AddTenantRoutingAddressToPolicy(EmailAddressPolicy policy)
		{
			List<ProxyAddressTemplate> list = new List<ProxyAddressTemplate>(policy.EnabledEmailAddressTemplates);
			SmtpProxyAddressTemplate item = new SmtpProxyAddressTemplate("%m@" + this.tenantRoutingDomain, false);
			list.Add(item);
			ProxyAddressTemplateCollection enabledEmailAddressTemplates = new ProxyAddressTemplateCollection(list.ToArray());
			base.OnPremisesSession.SetEmailAddressPolicy(policy.DistinguishedName, null, enabledEmailAddressTemplates);
		}

		private const string defaultLegacyPolicyName = "default policy";

		private const string allRecipients = "AllRecipients";

		private string tenantRoutingDomain;

		private bool configureAcceptedDomain;

		private bool configureRemoteDomain;

		private bool configureTargetDeliveryDomain;

		private bool upgradeLegacyDefaultPolicy;

		private string targetDeliveryDomain;

		private IEnumerable<EmailAddressPolicy> policiesToUpdate;
	}
}
