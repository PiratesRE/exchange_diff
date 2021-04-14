using System;
using System.Management.Automation;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Add", "FederatedDomain", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public sealed class AddFederatedDomain : SetFederatedOrganizationIdBase
	{
		[Parameter(Mandatory = true)]
		public SmtpDomain DomainName
		{
			get
			{
				return base.Fields["DomainName"] as SmtpDomain;
			}
			set
			{
				base.Fields["DomainName"] = value;
			}
		}

		protected override void InternalValidate()
		{
			TaskLogger.LogEnter();
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			this.ValidateParameters();
			bool hasErrors = base.HasErrors;
		}

		protected override void InternalProcessRecord()
		{
			this.ProvisionSTS();
			AcceptedDomain acceptedDomain = (AcceptedDomain)base.DataSession.Read<AcceptedDomain>(this.matchedAcceptedDomain.Id);
			acceptedDomain.FederatedOrganizationLink = this.DataObject.Id;
			acceptedDomain.PendingFederatedAccountNamespace = false;
			acceptedDomain.PendingFederatedDomain = false;
			base.DataSession.Save(acceptedDomain);
		}

		private void ValidateParameters()
		{
			if (this.DomainName == null || string.IsNullOrEmpty(this.DomainName.Domain))
			{
				base.WriteError(new NoAccountNamespaceException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.DataObject.DelegationTrustLink == null || this.DataObject.AccountNamespace == null || string.IsNullOrEmpty(this.DataObject.AccountNamespace.Domain))
			{
				base.WriteError(new NoTrustConfiguredException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			ADObjectId deletedObjectsContainer = base.GlobalConfigSession.DeletedObjectsContainer;
			ADObjectId adobjectId = ADObjectIdResolutionHelper.ResolveDN(this.DataObject.DelegationTrustLink);
			if (adobjectId.Parent.Equals(deletedObjectsContainer))
			{
				base.WriteError(new NoTrustConfiguredException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			this.federationTrust = base.GlobalConfigSession.Read<FederationTrust>(adobjectId);
			if (this.federationTrust == null)
			{
				base.WriteError(new ManagementObjectNotFoundException(Strings.ErrorFederationTrustNotFound(adobjectId.ToDNString())), ErrorCategory.ObjectNotFound, null);
			}
			this.matchedAcceptedDomain = base.GetAcceptedDomain(this.DomainName, false);
			if (this.matchedAcceptedDomain.FederatedOrganizationLink != null && !this.matchedAcceptedDomain.FederatedOrganizationLink.Parent.Equals(deletedObjectsContainer))
			{
				base.WriteError(new DomainAlreadyFederatedException(this.DomainName.Domain), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			TaskLogger.LogExit();
		}

		private void ProvisionSTS()
		{
			FederationProvision federationProvision = FederationProvision.Create(this.federationTrust, this);
			try
			{
				federationProvision.OnAddFederatedDomain(this.DomainName);
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidResult, null);
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetFederatedDomain(this.DomainName.Domain, base.CurrentOrgContainerId.Name);
			}
		}

		private AcceptedDomain matchedAcceptedDomain;

		private FederationTrust federationTrust;
	}
}
