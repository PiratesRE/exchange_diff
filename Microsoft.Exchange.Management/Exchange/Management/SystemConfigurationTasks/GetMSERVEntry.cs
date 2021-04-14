using System;
using System.Management.Automation;
using Microsoft.Exchange.Data.Directory.SystemConfiguration;

namespace Microsoft.Exchange.Management.SystemConfigurationTasks
{
	[Cmdlet("Get", "MSERVEntry", DefaultParameterSetName = "ExternalDirectoryOrganizationIdParameterSet")]
	public sealed class GetMSERVEntry : ManageMSERVEntryBase
	{
		protected override void InternalProcessRecord()
		{
			MSERVEntry sendToPipeline = new MSERVEntry();
			if (base.Fields.IsModified("ExternalDirectoryOrganizationId"))
			{
				sendToPipeline = base.ProcessExternalOrgIdParameter((string address, int partnerId) => base.ReadMservEntry(address));
			}
			else if (base.Fields.IsModified("DomainName"))
			{
				sendToPipeline = base.ProcessDomainNameParameter((string address, int partnerId) => base.ReadMservEntry(address));
			}
			else
			{
				sendToPipeline = base.ProcessAddressParameter((string address, int partnerId) => base.ReadMservEntry(address));
			}
			base.WriteObject(sendToPipeline);
		}
	}
}
