using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("New", "MSERVEntry", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet", SupportsShouldProcess = true)]
	public sealed class NewMSERVEntry : ManageMSERVEntryBase
	{
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "AddressParameterSet")]
		public int PartnerId
		{
			get
			{
				return (int)base.Fields["PartnerId"];
			}
			set
			{
				base.Fields["PartnerId"] = value;
			}
		}

		[Parameter(Mandatory = false, ParameterSetName = "AddressParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "DomainNameParameterSet")]
		[Parameter(Mandatory = false, ParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
		public int MinorPartnerId
		{
			get
			{
				return (int)base.Fields["MinorPartnerId"];
			}
			set
			{
				base.Fields["MinorPartnerId"] = value;
			}
		}

		protected override LocalizedString ConfirmationMessage
		{
			get
			{
				string id;
				if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
				{
					id = ((Guid)base.Fields["ExternalDirectoryOrganizationId"]).ToString();
				}
				else if (base.Fields.IsModified("DomainName"))
				{
					id = ((SmtpDomain)base.Fields["DomainName"]).Domain;
				}
				else
				{
					id = (string)base.Fields["Address"];
				}
				return Strings.ConfirmationMessageNewMservEntry(id);
			}
		}

		protected override void InternalValidate()
		{
			base.InternalValidate();
			if (!base.Fields.IsModified("PartnerId") && !base.Fields.IsModified("MinorPartnerId"))
			{
				base.WriteError(new ParameterBindingException("Either PartnerId or MinorPartnerId should be specified"), ErrorCategory.InvalidArgument, null);
			}
			if (base.Fields.IsModified("PartnerId"))
			{
				this.partnerId = (int)base.Fields["PartnerId"];
				base.ValidateMservIdValue(this.partnerId);
			}
			if (base.Fields.IsModified("MinorPartnerId"))
			{
				this.minorPartnerId = (int)base.Fields["MinorPartnerId"];
				base.ValidateMservIdValue(this.minorPartnerId);
			}
			if (base.Fields.IsModified("Address"))
			{
				if (base.Fields.IsModified("PartnerId") && base.Fields.IsModified("MinorPartnerId"))
				{
					base.WriteError(new ParameterBindingException("Both PartnerId and MinorPartnerId cannot be specified when address parameter is used"), ErrorCategory.InvalidArgument, null);
				}
				base.ValidateAddressParameter((string)base.Fields["Address"]);
			}
		}

		protected override void InternalProcessRecord()
		{
			MSERVEntry sendToPipeline = new MSERVEntry();
			if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
			{
				sendToPipeline = base.ProcessExternalOrgIdParameter(this.partnerId, this.minorPartnerId, (string address, int id) => base.AddMservEntry(address, id));
			}
			else if (base.Fields.IsModified("DomainName"))
			{
				sendToPipeline = base.ProcessDomainNameParameter(this.partnerId, this.minorPartnerId, (string address, int id) => base.AddMservEntry(address, id));
			}
			else
			{
				sendToPipeline = base.ProcessAddressParameter((this.partnerId != -1) ? this.partnerId : this.minorPartnerId, (string address, int id) => base.AddMservEntry(address, id));
			}
			base.WriteObject(sendToPipeline);
		}

		private int partnerId = -1;

		private int minorPartnerId = -1;
	}
}
