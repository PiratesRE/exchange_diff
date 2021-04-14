using System;
using System.Management.Automation;
using System.Text;
using Microsoft.Exchange.Configuration.Tasks;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;
using Microsoft.Exchange.VariantConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "FederatedDomain", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High, DefaultParameterSetName = "Identity")]
	public sealed class RemoveFederatedDomain : SetFederatedOrganizationIdBase
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

		[Parameter(Mandatory = false)]
		public SwitchParameter Force { get; set; }

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageRemoveFederatedDomain(this.DomainName.Domain, base.CurrentOrgContainerId.Name);
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
		}

		protected override void InternalProcessRecord()
		{
			try
			{
				this.ProcessRemoveFederatedDomainRequest();
			}
			catch (NullReferenceException ex)
			{
				string text = "NoMatchedAcceptedDomain";
				string text2 = "Unknown";
				if (this.matchedAcceptedDomain != null)
				{
					text = this.matchedAcceptedDomain.Id.ToString();
					text2 = ((this.matchedAcceptedDomain.FederatedOrganizationLink != null) ? this.matchedAcceptedDomain.FederatedOrganizationLink.ToString() : "NotFederated");
				}
				StringBuilder stringBuilder = new StringBuilder(1024);
				if (base.FederatedAcceptedDomains != null)
				{
					foreach (ADObjectId adobjectId in base.FederatedAcceptedDomains)
					{
						stringBuilder.AppendFormat("{0};", adobjectId.ToString());
					}
				}
				string message = string.Format("{0}:{1}:{2}:{3}", new object[]
				{
					ex.ToString(),
					text,
					text2,
					stringBuilder.ToString()
				});
				base.WriteError(new LocalizedException(Strings.ExceptionOccured(message)), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
		}

		private void ProcessRemoveFederatedDomainRequest()
		{
			this.RemoveFederatedDomainFromSTS();
			if (this.DomainName.Equals(this.DataObject.AccountNamespace) && !this.IsDatacenter)
			{
				string domain = this.DataObject.AccountNamespace.Domain;
				base.ZapDanglingDomainTrusts();
				this.DataObject.AccountNamespace = null;
				this.DataObject.DelegationTrustLink = null;
				this.DataObject.Enabled = false;
				if (this.federationTrust != null && null != this.federationTrust.ApplicationUri)
				{
					string text = FederatedOrganizationId.RemoveHybridConfigurationWellKnownSubDomain(this.federationTrust.ApplicationUri.ToString());
					if (text.Equals(domain, StringComparison.InvariantCultureIgnoreCase))
					{
						this.federationTrust.ApplicationUri = null;
						this.federationTrust.ApplicationIdentifier = null;
						base.DataSession.Save(this.federationTrust);
					}
				}
				base.InternalProcessRecord();
				return;
			}
			if (this.matchedAcceptedDomain != null && this.matchedAcceptedDomain.Id != null)
			{
				AcceptedDomain acceptedDomain = (AcceptedDomain)base.DataSession.Read<AcceptedDomain>(this.matchedAcceptedDomain.Id);
				if (acceptedDomain == null)
				{
					this.WriteWarning(Strings.ErrorDomainNameNotAcceptedDomain(this.DomainName.Domain));
					return;
				}
				if (acceptedDomain.FederatedOrganizationLink == null)
				{
					this.WriteWarning(Strings.ErrorDomainIsNotFederated(this.DomainName.Domain));
					return;
				}
				acceptedDomain.FederatedOrganizationLink = null;
				base.DataSession.Save(acceptedDomain);
			}
		}

		private void ValidateParameters()
		{
			if (this.DomainName == null || string.IsNullOrEmpty(this.DomainName.Domain))
			{
				base.WriteError(new NoAccountNamespaceException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (!this.IsDatacenter && (this.DataObject.DelegationTrustLink == null || this.DataObject.AccountNamespace == null || string.IsNullOrEmpty(this.DataObject.AccountNamespace.Domain)))
			{
				base.WriteError(new NoTrustConfiguredException(), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			if (this.DataObject.DelegationTrustLink == null)
			{
				this.federationTrust = null;
			}
			else
			{
				ADObjectId deletedObjectsContainer = this.ConfigurationSession.DeletedObjectsContainer;
				ADObjectId adobjectId = ADObjectIdResolutionHelper.ResolveDN(this.DataObject.DelegationTrustLink);
				if (adobjectId != null)
				{
					if (adobjectId.Parent.Equals(deletedObjectsContainer))
					{
						this.WriteWarning(Strings.ErrorFederationTrustNotFound(adobjectId.ToDNString()));
						this.federationTrust = null;
					}
					else
					{
						IConfigDataProvider configDataProvider = this.IsDatacenter ? base.GlobalConfigSession : base.DataSession;
						this.federationTrust = (configDataProvider.Read<FederationTrust>(adobjectId) as FederationTrust);
						if (this.federationTrust == null)
						{
							this.WriteWarning(Strings.ErrorFederationTrustNotFound(adobjectId.ToDNString()));
						}
					}
				}
				else
				{
					this.WriteWarning(Strings.ErrorFederationTrustNotFound(this.DataObject.DelegationTrustLink.ToDNString()));
					this.federationTrust = null;
				}
			}
			if (!this.IsDatacenter && this.DomainName.Equals(this.DataObject.AccountNamespace) && 1 < base.FederatedAcceptedDomains.Count)
			{
				base.WriteError(new CannotRemoveAccountNamespaceException(this.DomainName.Domain), ErrorCategory.InvalidOperation, this.DataObject.Identity);
			}
			this.matchedAcceptedDomain = base.GetAcceptedDomain(this.DomainName, true);
			if (this.matchedAcceptedDomain.FederatedOrganizationLink == null && !this.DomainName.Equals(this.DataObject.AccountNamespace))
			{
				if (this.Force || this.IsDatacenter)
				{
					this.WriteWarning(Strings.ErrorDomainIsNotFederated(this.DomainName.Domain));
				}
				else
				{
					base.WriteError(new DomainIsNotFederatedException(this.DomainName.Domain), ErrorCategory.InvalidOperation, this.DataObject.Identity);
				}
			}
			TaskLogger.LogExit();
		}

		private void RemoveFederatedDomainFromSTS()
		{
			if (this.federationTrust == null)
			{
				return;
			}
			FederationProvision federationProvision = FederationProvision.Create(this.federationTrust, this);
			try
			{
				if (this.DomainName.Equals(this.DataObject.AccountNamespace))
				{
					federationProvision.OnRemoveAccountNamespace(this.DataObject.AccountNamespaceWithWellKnownSubDomain, this.Force);
				}
				else
				{
					federationProvision.OnRemoveFederatedDomain(this.DomainName, this.Force);
				}
			}
			catch (LocalizedException exception)
			{
				base.WriteError(exception, ErrorCategory.InvalidResult, null);
			}
		}

		private bool IsDatacenter
		{
			get
			{
				return VariantConfiguration.InvariantNoFlightingSnapshot.Global.MultiTenancy.Enabled;
			}
		}

		private AcceptedDomain matchedAcceptedDomain;

		private FederationTrust federationTrust;
	}
}
