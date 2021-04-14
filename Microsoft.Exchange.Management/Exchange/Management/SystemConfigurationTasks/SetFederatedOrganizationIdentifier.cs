using System;
using System.Collections.Generic;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Set", "FederatedOrganizationIdentifier", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class SetFederatedOrganizationIdentifier : SetFederatedOrganizationIdBase
	{
		[Parameter(Mandatory = false)]
		public FederationTrustIdParameter DelegationFederationTrust
		{
			get
			{
				return base.Fields["DelegationFederationTrust"] as FederationTrustIdParameter;
			}
			set
			{
				base.Fields["DelegationFederationTrust"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain AccountNamespace
		{
			get
			{
				return base.Fields["AccountNamespace"] as SmtpDomain;
			}
			set
			{
				base.Fields["AccountNamespace"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpDomain DefaultDomain
		{
			get
			{
				return base.Fields["DefaultDomain"] as SmtpDomain;
			}
			set
			{
				base.Fields["DefaultDomain"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public bool Enabled
		{
			get
			{
				return (bool)base.Fields["Enabled"];
			}
			set
			{
				base.Fields["Enabled"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public SmtpAddress OrganizationContact
		{
			get
			{
				return (SmtpAddress)base.Fields["OrganizationContact"];
			}
			set
			{
				base.Fields["OrganizationContact"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			this.noTrustToUpdate = false;
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.IsModified("Enabled"))
			{
				this.DataObject.Enabled = (bool)base.Fields["Enabled"];
			}
			if (base.Fields.IsModified("OrganizationContact"))
			{
				this.DataObject.OrganizationContact = (SmtpAddress)base.Fields["OrganizationContact"];
			}
			this.ValidateOrganizationContactParameter();
			this.ValidateAccountNamespaceParameter();
			this.ValidateDefaultDomainParameter();
			this.ValidateDelegationFederationTrustParameter();
			this.ValidateSetParameters();
			this.ValidateSetParameters();
		}

		protected override void InternalProcessRecord()
		{
			if (base.Fields.IsModified("DefaultDomain"))
			{
				IEnumerable<AcceptedDomain> enumerable = base.DataSession.FindPaged<AcceptedDomain>(null, base.CurrentOrgContainerId, true, null, 1000);
				foreach (AcceptedDomain acceptedDomain in enumerable)
				{
					if (acceptedDomain.IsDefaultFederatedDomain && (this.matchedDefaultAcceptedDomain == null || !this.matchedDefaultAcceptedDomain.Id.Equals(acceptedDomain.Id)))
					{
						acceptedDomain.IsDefaultFederatedDomain = false;
						base.DataSession.Save(acceptedDomain);
					}
				}
				if (this.matchedDefaultAcceptedDomain != null && !this.matchedDefaultAcceptedDomain.IsDefaultFederatedDomain)
				{
					this.matchedDefaultAcceptedDomain.IsDefaultFederatedDomain = true;
					base.DataSession.Save(this.matchedDefaultAcceptedDomain);
					this.defaultDomainChanged = true;
				}
			}
			if (this.noTrustToUpdate)
			{
				if (this.IsObjectStateChanged())
				{
					base.InternalProcessRecord();
					return;
				}
			}
			else
			{
				base.ZapDanglingDomainTrusts();
				this.ProvisionSTS();
				SmtpDomain smtpDomain = this.AccountNamespace;
				if (this.federationTrust.NamespaceProvisioner == FederationTrust.NamespaceProvisionerType.LiveDomainServices2)
				{
					smtpDomain = new SmtpDomain(FederatedOrganizationId.AddHybridConfigurationWellKnownSubDomain(this.AccountNamespace.Domain));
				}
				this.DataObject.AccountNamespace = smtpDomain;
				this.DataObject.DelegationTrustLink = this.delegationTrustId;
				if (!this.DataObject.IsModified(FederatedOrganizationIdSchema.Enabled))
				{
					this.DataObject.Enabled = true;
				}
				bool flag = false;
				Uri applicationUri = null;
				if (null == this.federationTrust.ApplicationUri)
				{
					if (!Uri.TryCreate(smtpDomain.Domain, UriKind.RelativeOrAbsolute, out applicationUri))
					{
						base.WriteError(new InvalidApplicationUriException(Strings.ErrorInvalidApplicationUri(this.AccountNamespace.Domain)), ErrorCategory.InvalidArgument, this.DataObject.Identity);
					}
					flag = true;
				}
				base.InternalProcessRecord();
				if (flag)
				{
					this.federationTrust.ApplicationUri = applicationUri;
					base.DataSession.Save(this.federationTrust);
				}
				AcceptedDomain acceptedDomain2 = (AcceptedDomain)base.DataSession.Read<AcceptedDomain>(this.matchedAcceptedDomain.Id);
				acceptedDomain2.FederatedOrganizationLink = this.DataObject.Id;
				acceptedDomain2.PendingFederatedAccountNamespace = false;
				acceptedDomain2.PendingFederatedDomain = false;
				base.DataSession.Save(acceptedDomain2);
			}
		}

		protected override bool IsObjectStateChanged()
		{
			return this.defaultDomainChanged || base.IsObjectStateChanged();
		}

		private void ValidateSetParameters()
		{
			ADObjectId adobjectId = ADObjectIdResolutionHelper.ResolveDN(this.DataObject.DelegationTrustLink);
			if (adobjectId != null && (string.IsNullOrEmpty(adobjectId.DistinguishedName) || adobjectId.Parent.Equals(this.ConfigurationSession.DeletedObjectsContainer)))
			{
				adobjectId = null;
			}
			if (this.DelegationFederationTrust == null && this.AccountNamespace == null)
			{
				if (adobjectId == null)
				{
					base.WriteError(new NoTrustConfiguredException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				this.noTrustToUpdate = true;
				return;
			}
			if (this.DelegationFederationTrust != null)
			{
				if (this.AccountNamespace == null)
				{
					if (adobjectId != null)
					{
						return;
					}
					base.WriteError(new NoAccountNamespaceException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
			}
			else
			{
				base.WriteError(new CannotSpecifyAccountNamespaceWithoutTrustException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.DataObject.AccountNamespace != null)
			{
				string x = FederatedOrganizationId.ContainsHybridConfigurationWellKnownSubDomain(this.DataObject.AccountNamespace.Domain) ? FederatedOrganizationId.AddHybridConfigurationWellKnownSubDomain(this.AccountNamespace.Domain) : this.AccountNamespace.Domain;
				if (!StringComparer.OrdinalIgnoreCase.Equals(x, this.DataObject.AccountNamespace.Domain) && adobjectId != null)
				{
					base.WriteError(new TrustAlreadyDefinedException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				if (adobjectId != null && !adobjectId.Equals(this.delegationTrustId))
				{
					base.WriteError(new TrustAlreadyDefinedException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
			}
			ADObjectId adobjectId2 = this.matchedAcceptedDomain.FederatedOrganizationLink;
			if (adobjectId2 != null && adobjectId2.Parent.Equals(this.ConfigurationSession.DeletedObjectsContainer))
			{
				adobjectId2 = null;
			}
			if (adobjectId2 != null)
			{
				if (adobjectId2.ObjectGuid != this.DataObject.Id.ObjectGuid)
				{
					base.WriteError(new DomainAlreadyFederatedException(this.AccountNamespace.Domain), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
				else
				{
					this.noTrustToUpdate = true;
				}
			}
			TaskLogger.LogExit();
		}

		private void ValidateDelegationFederationTrustParameter()
		{
			if (this.DelegationFederationTrust != null)
			{
				ADObjectId descendantId = base.RootOrgContainerId.GetDescendantId(FederationTrust.FederationTrustsContainer);
				IConfigDataProvider configDataProvider = ((IConfigurationSession)base.DataSession).SessionSettings.PartitionId.ForestFQDN.Equals(TopologyProvider.LocalForestFqdn, StringComparison.OrdinalIgnoreCase) ? base.DataSession : base.RootOrgGlobalConfigSession;
				IEnumerable<FederationTrust> objects = this.DelegationFederationTrust.GetObjects<FederationTrust>(descendantId, configDataProvider);
				ADObjectId identity = null;
				using (IEnumerator<FederationTrust> enumerator = objects.GetEnumerator())
				{
					if (!enumerator.MoveNext())
					{
						base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorFederationTrustNotFound(this.DelegationFederationTrust.ToString())), ErrorCategory.ObjectNotFound, null);
					}
					identity = (ADObjectId)enumerator.Current.Identity;
					if (enumerator.MoveNext())
					{
						base.WriteError(new ManagementObjectAmbiguousException(Strings.ErrorFederationTrustNotUnique(this.DelegationFederationTrust.ToString())), ErrorCategory.InvalidData, null);
					}
				}
				FederationTrust federationTrust = (FederationTrust)configDataProvider.Read<FederationTrust>(identity);
				if (federationTrust == null)
				{
					base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorFederationTrustNotFound(this.delegationTrustId.ToDNString())), ErrorCategory.ObjectNotFound, null);
				}
				this.delegationTrustId = identity;
				this.federationTrust = federationTrust;
			}
		}

		private void ValidateAccountNamespaceParameter()
		{
			if (this.AccountNamespace != null)
			{
				this.matchedAcceptedDomain = base.GetAcceptedDomain(this.AccountNamespace, false);
			}
		}

		private void ValidateDefaultDomainParameter()
		{
			if (this.DefaultDomain != null)
			{
				this.matchedDefaultAcceptedDomain = base.GetAcceptedDomain(this.DefaultDomain, false);
			}
		}

		private void ValidateOrganizationContactParameter()
		{
		}

		private void ProvisionSTS()
		{
			FederationProvision federationProvision = FederationProvision.Create(this.federationTrust, this);
			try
			{
				federationProvision.OnSetFederatedOrganizationIdentifier(this.federationTrust, this.AccountNamespace);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidResult, null);
			}
			if (this.federationTrust.ObjectState == ObjectState.Changed)
			{
				base.DataSession.Save(this.federationTrust);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				LocalizedString result = LocalizedString.Empty;
				if (this.AccountNamespace != null)
				{
					result = Strings.ConfirmationMessageSetFederatedOrganizationIdentifier1(base.CurrentOrgContainerId.Name, this.AccountNamespace.Domain, this.delegationTrustId.Name);
				}
				else if (this.DataObject.IsChanged(FederatedOrganizationIdSchema.OrganizationContact))
				{
					result = Strings.ConfirmationMessageSetFederatedOrganizationIdentifier2(base.CurrentOrgContainerId.Name, this.DataObject.OrganizationContact.ToString());
				}
				else if (this.DataObject.IsChanged(FederatedOrganizationIdSchema.Enabled))
				{
					result = (this.DataObject.Enabled ? Strings.ConfirmationMessageEnableFederatedOrgId(base.CurrentOrgContainerId.Name) : Strings.ConfirmationMessageDisableFederatedOrgId(base.CurrentOrgContainerId.Name));
				}
				return result;
			}
		}

		private AcceptedDomain matchedAcceptedDomain;

		private AcceptedDomain matchedDefaultAcceptedDomain;

		private bool defaultDomainChanged;

		private bool noTrustToUpdate;

		private ADObjectId delegationTrustId;

		private FederationTrust federationTrust;
	}
}
