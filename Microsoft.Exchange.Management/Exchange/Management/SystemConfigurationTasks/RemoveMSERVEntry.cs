using System;
using System.Management.Automation;
using Microsoft.Exchange.Data;
using Microsoft.Exchange.Data.Common;
using Microsoft.Exchange.Management.Tasks;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Remove", "MSERVEntry", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet", SupportsShouldProcess = true, ConfirmImpact = ConfirmImpact.High)]
	public sealed class RemoveMSERVEntry : ManageMSERVEntryBase
	{
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
				return Strings.ConfirmationMessageRemoveMservEntry(id);
			}
		}

		protected override void InternalProcessRecord()
		{
			if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
			{
				base.ProcessExternalOrgIdParameter((string address, int partnerId) => base.RemoveMservEntry(address));
				return;
			}
			if (base.Fields.IsModified("DomainName"))
			{
				base.ProcessDomainNameParameter((string address, int partnerId) => base.RemoveMservEntry(address));
				return;
			}
			base.ProcessAddressParameter((string address, int partnerId) => base.RemoveMservEntry(address));
		}
	}
}
