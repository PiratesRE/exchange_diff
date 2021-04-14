using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.Management;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.Hybrid
{
	internal class OnOffSettingsTask : SessionTask
	{
		public OnOffSettingsTask() : base(HybridStrings.OnOffSettingsTaskName, 2)
		{
		}

		private IOrganizationConfig OnPremOrgConfig
		{
			get
			{
				return base.OnPremisesSession.GetOrganizationConfig();
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

		public override bool CheckPrereqs(ITaskContext taskContext)
		{
			if (!base.CheckPrereqs(taskContext))
			{
				base.Logger.LogInformation(HybridStrings.HybridInfoTaskLogTemplate(base.Name, HybridStrings.HybridInfoBasePrereqsFailed));
				return false;
			}
			HybridConfiguration hybridConfigurationObject = taskContext.HybridConfigurationObject;
			this.onPremOrgRel = taskContext.Parameters.Get<OrganizationRelationship>("_onPremOrgRel");
			if (this.onPremOrgRel == null)
			{
				throw new LocalizedException(HybridStrings.ErrorOrgRelNotFoundOnPrem);
			}
			this.tenantOrgRel = taskContext.Parameters.Get<OrganizationRelationship>("_tenantOrgRel");
			if (this.tenantOrgRel == null)
			{
				throw new LocalizedException(HybridStrings.ErrorOrgRelNotFoundOnTenant);
			}
			if (this.RequiresFederationTrust())
			{
				this.fedOrgId = base.TenantSession.GetFederatedOrganizationIdentifier();
				if (this.fedOrgId == null)
				{
					throw new LocalizedException(HybridStrings.ErrorFederationIDNotProvisioned);
				}
				if (!this.fedOrgId.Enabled || string.IsNullOrEmpty(this.fedOrgId.AccountNamespace.Domain) || this.fedOrgId.Domains.Count == 0)
				{
					throw new LocalizedException(HybridStrings.ErrorFederationIDNotProvisioned);
				}
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
			SessionParameters sessionParameters = new SessionParameters();
			SessionParameters sessionParameters2 = new SessionParameters();
			if (hybridConfigurationObject.MoveMailboxEnabled != this.onPremOrgRel.MailboxMoveEnabled)
			{
				sessionParameters.Set("MailboxMoveEnabled", hybridConfigurationObject.MoveMailboxEnabled);
			}
			if (hybridConfigurationObject.FreeBusySharingEnabled != this.onPremOrgRel.FreeBusyAccessEnabled || hybridConfigurationObject.FreeBusySharingEnabled != this.tenantOrgRel.FreeBusyAccessEnabled)
			{
				sessionParameters.Set("FreeBusyAccessEnabled", hybridConfigurationObject.FreeBusySharingEnabled);
				sessionParameters2.Set("FreeBusyAccessEnabled", hybridConfigurationObject.FreeBusySharingEnabled);
			}
			if (hybridConfigurationObject.FreeBusySharingEnabled && (this.onPremOrgRel.FreeBusyAccessLevel != FreeBusyAccessLevel.LimitedDetails || this.tenantOrgRel.FreeBusyAccessLevel != FreeBusyAccessLevel.LimitedDetails))
			{
				sessionParameters.Set("FreeBusyAccessLevel", FreeBusyAccessLevel.LimitedDetails);
				sessionParameters2.Set("FreeBusyAccessLevel", FreeBusyAccessLevel.LimitedDetails);
			}
			if (hybridConfigurationObject.OnlineArchiveEnabled != this.onPremOrgRel.ArchiveAccessEnabled)
			{
				sessionParameters.Set("ArchiveAccessEnabled", hybridConfigurationObject.OnlineArchiveEnabled);
			}
			if (hybridConfigurationObject.MailtipsEnabled != this.onPremOrgRel.MailTipsAccessEnabled || hybridConfigurationObject.MailtipsEnabled != this.tenantOrgRel.MailTipsAccessEnabled)
			{
				sessionParameters.Set("MailTipsAccessEnabled", hybridConfigurationObject.MailtipsEnabled);
				sessionParameters2.Set("MailTipsAccessEnabled", hybridConfigurationObject.MailtipsEnabled);
			}
			if (hybridConfigurationObject.MailtipsEnabled && (this.onPremOrgRel.MailTipsAccessLevel != MailTipsAccessLevel.All || this.tenantOrgRel.MailTipsAccessLevel != MailTipsAccessLevel.All))
			{
				sessionParameters.Set("MailTipsAccessLevel", MailTipsAccessLevel.All);
				sessionParameters2.Set("MailTipsAccessLevel", MailTipsAccessLevel.All);
			}
			if (hybridConfigurationObject.MessageTrackingEnabled != this.onPremOrgRel.DeliveryReportEnabled || hybridConfigurationObject.MessageTrackingEnabled != this.tenantOrgRel.DeliveryReportEnabled)
			{
				sessionParameters.Set("DeliveryReportEnabled", hybridConfigurationObject.MessageTrackingEnabled);
				sessionParameters2.Set("DeliveryReportEnabled", hybridConfigurationObject.MessageTrackingEnabled);
			}
			if (hybridConfigurationObject.PhotosEnabled != this.onPremOrgRel.PhotosEnabled || hybridConfigurationObject.PhotosEnabled != this.tenantOrgRel.PhotosEnabled)
			{
				sessionParameters.Set("PhotosEnabled", hybridConfigurationObject.PhotosEnabled);
				sessionParameters2.Set("PhotosEnabled", hybridConfigurationObject.PhotosEnabled);
			}
			string text = (this.onPremOrgRel.TargetOwaURL != null) ? this.onPremOrgRel.TargetOwaURL.ToString() : string.Empty;
			if (hybridConfigurationObject.OwaRedirectionEnabled || !string.IsNullOrEmpty(text))
			{
				if (hybridConfigurationObject.OwaRedirectionEnabled)
				{
					IEnumerable<IAcceptedDomain> acceptedDomains = taskContext.Parameters.Get<IEnumerable<IAcceptedDomain>>("_tenantAcceptedDomains");
					if (!string.IsNullOrEmpty(text))
					{
						if (!this.ValidateOwaUri(text, acceptedDomains))
						{
							sessionParameters.Set("TargetOwaURL", this.GetDefaultOwaUri(acceptedDomains, this.fedOrgId));
						}
					}
					else
					{
						sessionParameters.Set("TargetOwaURL", this.GetDefaultOwaUri(acceptedDomains, this.fedOrgId));
					}
				}
				else
				{
					sessionParameters.SetNull<string>("TargetOwaURL");
				}
			}
			if (sessionParameters.Count > 0)
			{
				OrganizationRelationship organizationRelationship = taskContext.Parameters.Get<OrganizationRelationship>("_onPremOrgRel");
				base.OnPremisesSession.SetOrganizationRelationship(organizationRelationship.Identity, sessionParameters);
				List<string> list = new List<string>();
				string item = taskContext.Parameters.Get<string>("_hybridDomain");
				list.Add(item);
				this.onPremOrgRel = TaskCommon.GetOrganizationRelationship(taskContext.OnPremisesSession, TaskCommon.GetOnPremOrgRelationshipName(this.OnPremOrgConfig), list);
				taskContext.Parameters.Set<OrganizationRelationship>("_onPremOrgRel", this.onPremOrgRel);
			}
			if (sessionParameters2.Count > 0)
			{
				OrganizationRelationship organizationRelationship2 = taskContext.Parameters.Get<OrganizationRelationship>("_tenantOrgRel");
				base.TenantSession.SetOrganizationRelationship(organizationRelationship2.Identity, sessionParameters2);
				IEnumerable<string> domains = taskContext.Parameters.Get<IEnumerable<string>>("_hybridDomainList");
				this.tenantOrgRel = TaskCommon.GetOrganizationRelationship(taskContext.TenantSession, TaskCommon.GetTenantOrgRelationshipName(this.OnPremOrgConfig), domains);
				taskContext.Parameters.Set<OrganizationRelationship>("_onPremOrgRel", this.tenantOrgRel);
			}
			this.ConfigureAvailabilityAddressSpace(taskContext);
			return true;
		}

		public override bool ValidateConfiguration(ITaskContext taskContext)
		{
			return base.ValidateConfiguration(taskContext) && !this.CheckOrVerifyConfiguration(taskContext, true);
		}

		private bool ValidateOwaUri(string uri, IEnumerable<IAcceptedDomain> acceptedDomains)
		{
			string text = Configuration.TargetOwaPrefix(base.TaskContext.HybridConfigurationObject.ServiceInstance);
			if (uri.StartsWith(text, StringComparison.InvariantCultureIgnoreCase))
			{
				string value = uri.Substring(text.Length);
				foreach (IAcceptedDomain acceptedDomain in acceptedDomains)
				{
					if (acceptedDomain.DomainNameDomain.Equals(value, StringComparison.InvariantCultureIgnoreCase))
					{
						return true;
					}
				}
				return false;
			}
			return false;
		}

		private string GetDefaultOwaUri(IEnumerable<IAcceptedDomain> acceptedDomains, IFederatedOrganizationIdentifier fedOrgId)
		{
			string str = Configuration.TargetOwaPrefix(base.TaskContext.HybridConfigurationObject.ServiceInstance);
			foreach (IAcceptedDomain acceptedDomain in acceptedDomains)
			{
				if (fedOrgId != null)
				{
					using (MultiValuedProperty<FederatedDomain>.Enumerator enumerator2 = fedOrgId.Domains.GetEnumerator())
					{
						while (enumerator2.MoveNext())
						{
							FederatedDomain federatedDomain = enumerator2.Current;
							if (federatedDomain.Domain.Domain.Equals(acceptedDomain.DomainNameDomain, StringComparison.InvariantCultureIgnoreCase))
							{
								return str + acceptedDomain.DomainNameDomain;
							}
						}
						continue;
					}
				}
				foreach (SmtpDomain smtpDomain in this.HybridDomains)
				{
					if (smtpDomain.Domain.Equals(acceptedDomain.DomainNameDomain, StringComparison.InvariantCultureIgnoreCase))
					{
						return str + acceptedDomain.DomainNameDomain;
					}
				}
			}
			throw new LocalizedException(HybridStrings.ErrorNoFederatedDomainsOnTenant);
		}

		private bool CheckOrVerifyConfiguration(ITaskContext taskContext, bool fVerifyOnly)
		{
			bool result = false;
			HybridConfiguration hybridConfigurationObject = taskContext.HybridConfigurationObject;
			if (hybridConfigurationObject.MoveMailboxEnabled != this.onPremOrgRel.MailboxMoveEnabled)
			{
				result = true;
			}
			if (hybridConfigurationObject.FreeBusySharingEnabled != this.onPremOrgRel.FreeBusyAccessEnabled || hybridConfigurationObject.FreeBusySharingEnabled != this.tenantOrgRel.FreeBusyAccessEnabled)
			{
				result = true;
			}
			else if (hybridConfigurationObject.FreeBusySharingEnabled && (this.onPremOrgRel.FreeBusyAccessLevel != FreeBusyAccessLevel.LimitedDetails || this.tenantOrgRel.FreeBusyAccessLevel != FreeBusyAccessLevel.LimitedDetails))
			{
				result = true;
			}
			if (hybridConfigurationObject.OnlineArchiveEnabled != this.onPremOrgRel.ArchiveAccessEnabled)
			{
				result = true;
			}
			if (hybridConfigurationObject.MailtipsEnabled != this.onPremOrgRel.MailTipsAccessEnabled || hybridConfigurationObject.MailtipsEnabled != this.tenantOrgRel.MailTipsAccessEnabled)
			{
				result = true;
			}
			else if (hybridConfigurationObject.MailtipsEnabled && (this.onPremOrgRel.MailTipsAccessLevel != MailTipsAccessLevel.All || this.tenantOrgRel.MailTipsAccessLevel != MailTipsAccessLevel.All))
			{
				result = true;
			}
			if (hybridConfigurationObject.MessageTrackingEnabled != this.onPremOrgRel.DeliveryReportEnabled || hybridConfigurationObject.MessageTrackingEnabled != this.tenantOrgRel.DeliveryReportEnabled)
			{
				result = true;
			}
			if (hybridConfigurationObject.PhotosEnabled != this.onPremOrgRel.PhotosEnabled || hybridConfigurationObject.PhotosEnabled != this.tenantOrgRel.PhotosEnabled)
			{
				result = true;
			}
			string value = (this.onPremOrgRel.TargetOwaURL != null) ? this.onPremOrgRel.TargetOwaURL.ToString() : string.Empty;
			if (fVerifyOnly)
			{
				if (hybridConfigurationObject.OwaRedirectionEnabled == string.IsNullOrEmpty(value))
				{
					result = true;
				}
			}
			else if (hybridConfigurationObject.OwaRedirectionEnabled || !string.IsNullOrEmpty(value))
			{
				result = true;
			}
			return result;
		}

		private void ConfigureAvailabilityAddressSpace(ITaskContext taskContext)
		{
			string forestName = taskContext.Parameters.Get<string>("_hybridDomain");
			AvailabilityAddressSpace availabilityAddressSpace = (from x in base.OnPremisesSession.GetAvailabilityAddressSpace()
			where string.Equals(x.ForestName, forestName, StringComparison.CurrentCultureIgnoreCase)
			select x).FirstOrDefault<AvailabilityAddressSpace>();
			if (availabilityAddressSpace != null && availabilityAddressSpace.AccessMethod == AvailabilityAccessMethod.InternalProxy && availabilityAddressSpace.UseServiceAccount && availabilityAddressSpace.ProxyUrl != null)
			{
				return;
			}
			Uri uri = (from x in base.OnPremisesSession.GetExchangeServer()
			where x.AdminDisplayVersion.Major >= 15
			select base.OnPremisesSession.GetWebServicesVirtualDirectory(x.Identity.Name).FirstOrDefault<ADWebServicesVirtualDirectory>() into x
			where x != null && x.ExternalUrl != null
			select x.InternalUrl).FirstOrDefault<Uri>();
			if (uri == null)
			{
				throw new LocalizedException(HybridStrings.ErrorHybridNoCASWithEWSURL);
			}
			if (availabilityAddressSpace != null)
			{
				base.OnPremisesSession.RemoveAvailabilityAddressSpace(availabilityAddressSpace.Identity.ToString());
			}
			base.OnPremisesSession.AddAvailabilityAddressSpace(forestName, AvailabilityAccessMethod.InternalProxy, true, uri);
		}

		private bool RequiresFederationTrust()
		{
			return Configuration.RequiresFederationTrust(base.TaskContext.HybridConfigurationObject.ServiceInstance);
		}

		private OrganizationRelationship onPremOrgRel;

		private OrganizationRelationship tenantOrgRel;

		private IFederatedOrganizationIdentifier fedOrgId;
	}
}
