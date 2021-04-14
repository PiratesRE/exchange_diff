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
	[Cmdlet("Set", "InboundConnector", SupportsShouldProcess = true, DefaultParameterSetName = "Identity")]
	public class SetInboundConnector : SetSystemConfigurationObjectTask<InboundConnectorIdParameter, TenantInboundConnector>
	{
		[Parameter(Mandatory = false)]
		public bool BypassValidation
		{
			get
			{
				return base.Fields.Contains("BypassValidation") && (bool)base.Fields["BypassValidation"];
			}
			set
			{
				base.Fields["BypassValidation"] = value;
			}
		}

		[Parameter(Mandatory = false)]
		public MultiValuedProperty<AcceptedDomainIdParameter> AssociatedAcceptedDomains
		{
			get
			{
				return (MultiValuedProperty<AcceptedDomainIdParameter>)base.Fields["AssociatedAcceptedDomains"];
			}
			set
			{
				base.Fields["AssociatedAcceptedDomains"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				return Strings.ConfirmationMessageSetInboundConnector(this.Identity.ToString());
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (base.HasErrors)
			{
				return;
			}
			if (base.Fields.IsModified("AssociatedAcceptedDomains"))
			{
				NewInboundConnector.ValidateAssociatedAcceptedDomains(this.AssociatedAcceptedDomains, base.DataSession, this.DataObject, this.RootId, this, new Func<IIdentityParameter, IConfigDataProvider, ObjectId, LocalizedString?, LocalizedString?, IConfigurable>(base.GetDataObject<AcceptedDomain>));
			}
			if (this.DataObject.SenderDomains == null)
			{
				base.WriteError(new LocalizedException(new LocalizedString("Sender Domain cannot be null.")), ErrorCategory.InvalidArgument, null);
			}
			bool flag = false;
			if (this.DataObject.SenderIPAddresses != null && this.DataObject.Enabled)
			{
				flag = true;
				NewInboundConnector.ValidateSenderIPAddresses(this.DataObject.SenderIPAddresses, this, this.BypassValidation);
				NewInboundConnector.CheckSenderIpAddressesOverlap(base.DataSession, this.DataObject, this);
			}
			if (this.DataObject.ConnectorType == TenantConnectorType.OnPremises)
			{
				bool flag2 = flag || this.DataObject.IsChanged(TenantInboundConnectorSchema.ConnectorType);
				bool flag3 = this.DataObject.IsChanged(TenantInboundConnectorSchema.ConnectorType) || this.DataObject.IsChanged(TenantInboundConnectorSchema.TlsSenderCertificateName);
				if ((flag2 || flag3) && !this.BypassValidation)
				{
					MultiValuedProperty<IPRange> ffoDCIPs;
					MultiValuedProperty<SmtpX509IdentifierEx> ffoFDSmtpCerts;
					MultiValuedProperty<ServiceProviderSettings> serviceProviders;
					if (!HygieneDCSettings.GetSettings(out ffoDCIPs, out ffoFDSmtpCerts, out serviceProviders))
					{
						base.WriteError(new ConnectorValidationFailedException(), ErrorCategory.ConnectionError, null);
					}
					if (flag2)
					{
						NewInboundConnector.ValidateSenderIPAddressRestrictions(this.DataObject.SenderIPAddresses, ffoDCIPs, serviceProviders, this);
					}
					if (flag3)
					{
						NewInboundConnector.ValidateTlsSenderCertificateRestrictions(this.DataObject.TlsSenderCertificateName, ffoFDSmtpCerts, serviceProviders, this);
					}
				}
			}
		}

		protected override void StampChangesOn(IConfigurable dataObject)
		{
			ADObject adobject = dataObject as ADObject;
			if (adobject != null)
			{
				this.dualWriter = new FfoDualWriter(adobject.Name);
			}
			base.StampChangesOn(dataObject);
		}

		protected override void InternalProcessRecord()
		{
			TaskLogger.LogEnter();
			base.InternalProcessRecord();
			this.dualWriter.Save<TenantInboundConnector>(this, this.DataObject);
			TaskLogger.LogExit();
		}

		private const string AssociatedAcceptedDomainsField = "AssociatedAcceptedDomains";

		private FfoDualWriter dualWriter;
	}
}
